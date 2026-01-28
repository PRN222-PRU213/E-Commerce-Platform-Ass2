using System.Net.Http.Headers;
using System.Text;
using E_Commerce_Platform_Ass2.Service.DTOs;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace E_Commerce_Platform_Ass2.Service.Services
{
    public class VnptEKycService : IVnptEKycService
    {
        private readonly VnptEKycConfig _config;

        public VnptEKycService(IOptions<VnptEKycConfig> config)
        {
            _config = config.Value;
        }

        public async Task<EKycResult> VerifyAsync(IFormFile frontCccd, IFormFile backCccd, IFormFile selfie)
        {
            try
            {
                var ocr = await CallOcrAsync(frontCccd, backCccd);
                if (ocr == null || !ocr.Success)
                    return EKycResult.Fail("OCR CCCD thất bại hoặc không nhận diện được thông tin");

                var faceMatch = await CallFaceMatchAsync(frontCccd, selfie);
                if (faceMatch == null || faceMatch.similarity < 0.8)
                    return EKycResult.Fail("Khuôn mặt không khớp với ảnh trên CCCD (Tỉ lệ khớp thấp)");

                var frontLiveness = await CallCardLivenessAsync(frontCccd);
                if (frontLiveness == null || !frontLiveness.Success)
                    return EKycResult.Fail($"Giấy tờ mặt trước không hợp lệ: {frontLiveness?.Message ?? "Lỗi kiểm tra liveness"}");

                var backLiveness = await CallCardLivenessAsync(backCccd);
                if (backLiveness == null || !backLiveness.Success)
                    return EKycResult.Fail($"Giấy tờ mặt sau không hợp lệ: {backLiveness?.Message ?? "Lỗi kiểm tra liveness"}");

                return new EKycResult
                {
                    IsSuccess = true,
                    CCCDNumber = ocr.IdNumber,
                    FullName = ocr.FullName,
                    FaceMatchScore = faceMatch.similarity
                };
            }
            catch (Exception ex)
            {
                return EKycResult.Fail($"Lỗi hệ thống trong quá trình xác thực: {ex.Message}");
            }
        }

        private async Task<OrcResultDto?> CallOcrAsync(IFormFile front, IFormFile back)
        {
            string frontHash = await UploadFileAsync(front);
            string backHash = await UploadFileAsync(back);

            using var client = CreateHttpClient();

            var requestBody = new
            {
                img_front = frontHash,
                img_back = backHash,
                client_session = $"WEB_DESKTOP_WINDOWS_DEVICE_1.0.0_EShopper_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}",
                type = -1,
                validate_postcode = true,
                token = Guid.NewGuid().ToString(),
                crop_param = "0,0"
            };

            var jsonContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("ai/v1/ocr/id", jsonContent);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                string errMsg = GetVNPTErrorMessage(content, "OCR");
                throw new Exception(errMsg);
            }

            return JsonConvert.DeserializeObject<OrcResultDto>(content);
        }

        private async Task<dynamic?> CallFaceMatchAsync(IFormFile front, IFormFile selfie)
        {
            string frontHash = await UploadFileAsync(front);
            string faceHash = await UploadFileAsync(selfie);

            using var client = CreateHttpClient();

            var requestBody = new
            {
                img_front = frontHash,
                img_face = faceHash,
                client_session = $"WEB_DESKTOP_WINDOWS_DEVICE_1.0.0_EShopper_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}",
                token = Guid.NewGuid().ToString()
            };

            var jsonContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("ai/v1/face/compare", jsonContent);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                string errMsg = GetVNPTErrorMessage(content, "FaceMatch");
                throw new Exception(errMsg);
            }

            var jo = Newtonsoft.Json.Linq.JObject.Parse(content);
            if (jo["message"]?.ToString() != "IDG-00000000")
            {
                string errMsg = GetVNPTErrorMessage(content, "Face Match");
                throw new Exception(errMsg);
            }

            var obj = jo["object"];
            return new
            {
                result = obj?["result"]?.ToString(),
                msg = obj?["msg"]?.ToString(),
                similarity = (double)(obj?["prob"] ?? 0) / 100.0
            };
        }

        private async Task<dynamic?> CallCardLivenessAsync(IFormFile cardImg)
        {
            string hash = await UploadFileAsync(cardImg);

            using var client = CreateHttpClient();

            var requestBody = new
            {
                img = hash,
                client_session = $"WEB_DESKTOP_WINDOWS_DEVICE_1.0.0_EShopper_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}"
            };

            var jsonContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("ai/v1/card/liveness", jsonContent);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                string errMsg = GetVNPTErrorMessage(content, "CardLiveness");
                throw new Exception(errMsg);
            }

            var jo = Newtonsoft.Json.Linq.JObject.Parse(content);
            var message = jo["message"]?.ToString();
            var obj = jo["object"];

            bool isSuccess = message == "IDG-00000000" && obj?["liveness"]?.ToString() == "success";
            string displayMsg = obj?["liveness_msg"]?.ToString() ?? "Không xác định";

            if (isSuccess && ((bool?)obj?["fake_liveness"] == true || (bool?)obj?["face_swapping"] == true))
            {
                isSuccess = false;
                displayMsg = "Phát hiện giấy tờ có dấu hiệu giả mạo (chụp lại hoặc dán ảnh)";
            }

            return new
            {
                Success = isSuccess,
                Message = isSuccess ? displayMsg : GetVNPTErrorMessage(content, "Card Liveness")
            };
        }

        private string GetVNPTErrorMessage(string jsonContent, string defaultContext)
        {
            try
            {
                if (string.IsNullOrEmpty(jsonContent) || jsonContent.Trim().StartsWith("<"))
                    return $"Lỗi kết nối VNPT API ({defaultContext}).";

                var jo = Newtonsoft.Json.Linq.JObject.Parse(jsonContent);

                var errors = jo["errors"] as Newtonsoft.Json.Linq.JArray;
                if (errors != null && errors.Count > 0)
                {
                    return string.Join("; ", errors.Select(e => e.ToString()));
                }

                var message = jo["message"]?.ToString();
                if (!string.IsNullOrEmpty(message) && message != "IDG-00000000")
                {
                    if (message.StartsWith("IDG-")) return $"Lỗi VNPT ({message})";
                    return message;
                }

                return $"Lỗi không xác định từ VNPT ({defaultContext}).";
            }
            catch
            {
                return $"Lỗi phản hồi từ VNPT ({defaultContext}).";
            }
        }

        private async Task<string> UploadFileAsync(IFormFile file)
        {
            using var client = CreateHttpClient();
            var form = new MultipartFormDataContent();

            var streamContent = new StreamContent(file.OpenReadStream());
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType ?? "image/jpeg");
            form.Add(streamContent, "file", file.FileName);
            form.Add(new StringContent("KYC_ID_Card"), "title");
            form.Add(new StringContent("Identity Verification Document"), "description");

            var response = await client.PostAsync("file-service/v1/addFile", form);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                string errMsg = GetVNPTErrorMessage(content, "Upload");
                throw new Exception(errMsg);
            }

            var jo = Newtonsoft.Json.Linq.JObject.Parse(content);
            var message = jo["message"]?.ToString();

            if (message != "IDG-00000000")
            {
                string errMsg = GetVNPTErrorMessage(content, "Upload");
                throw new Exception(errMsg);
            }

            var obj = jo["object"];
            if (obj == null)
                throw new Exception("Trường 'object' không tồn tại trong phản hồi upload.");

            string hash = obj.Type == Newtonsoft.Json.Linq.JTokenType.Object
                ? obj["hash"]?.ToString() ?? obj["id"]?.ToString() ?? obj.ToString()
                : obj.ToString();

            if (string.IsNullOrEmpty(hash))
                throw new Exception("Không lấy được mã hash từ phản hồi của server.");

            return hash;
        }

        private HttpClient CreateHttpClient()
        {
            var baseUrl = _config.BaseUrl;
            if (!baseUrl.EndsWith("/")) baseUrl += "/";

            var client = new HttpClient
            {
                BaseAddress = new Uri(baseUrl),
                Timeout = TimeSpan.FromSeconds(60)
            };

            var token = _config.AccessToken ?? "";
            if (token.StartsWith("bearer ", StringComparison.OrdinalIgnoreCase))
            {
                token = token.Substring(7);
            }

            client.DefaultRequestHeaders.Add("Token-id", _config.TokenId);
            client.DefaultRequestHeaders.Add("Token-key", _config.TokenKey);
            client.DefaultRequestHeaders.Add("mac-address", "TEST1");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return client;
        }
    }
}

