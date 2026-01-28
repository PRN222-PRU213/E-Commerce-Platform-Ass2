using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using E_Commerce_Platform_Ass2.Data.Momo;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace E_Commerce_Platform_Ass2.Service.Services
{
    public class MomoService : IMomoService
    {
        private readonly MomoConfig _config;

        public MomoService(IOptions<MomoConfig> config)
        {
            _config = config.Value;
        }

        public async Task<string> CreatePaymentAsync(long amount, string orderInfo)
        {
            var orderId = Guid.NewGuid().ToString();
            var requestId = Guid.NewGuid().ToString();

            string rawHash =
                $"accessKey={_config.AccessKey}" +
                $"&amount={amount}" +
                $"&extraData=" +
                $"&ipnUrl={_config.NotifyUrl}" +
                $"&orderId={orderId}" +
                $"&orderInfo={orderInfo}" +
                $"&partnerCode={_config.PartnerCode}" +
                $"&redirectUrl={_config.ReturnUrl}" +
                $"&requestId={requestId}" +
                $"&requestType={_config.RequestType}";

            string signature = SignSHA256(rawHash, _config.SecretKey);

            var request = new MomoPaymentRequest
            {
                PartnerCode = _config.PartnerCode,
                AccessKey = _config.AccessKey,
                RequestId = requestId,
                Amount = amount,
                OrderId = orderId,
                OrderInfo = orderInfo,
                RedirectUrl = _config.ReturnUrl,
                IpnUrl = _config.NotifyUrl,
                ExtraData = "",
                RequestType = "captureWallet",
                Signature = signature,
                Lang = "en"
            };

            using var client = new HttpClient
            {
                BaseAddress = new Uri(_config.MomoApiUrl)
            };

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var json = JsonConvert.SerializeObject(request, settings);

            var response = await client.PostAsync(
                "",
                new StringContent(json, Encoding.UTF8, "application/json"));

            var responseContent = await response.Content.ReadAsStringAsync();
            var momoResponse = JsonConvert.DeserializeObject<MomoPaymentResponse>(responseContent);

            if (momoResponse == null || momoResponse.resultCode != 0)
            {
                throw new Exception(
                    $"MoMo error {momoResponse?.resultCode}: {momoResponse?.message}");
            }

            return momoResponse.payUrl;
        }

        private static string SignSHA256(string message, string key)
        {
            var encoding = new UTF8Encoding();
            byte[] keyByte = encoding.GetBytes(key);
            byte[] messageBytes = encoding.GetBytes(message);

            using var hmac = new HMACSHA256(keyByte);
            byte[] hashMessage = hmac.ComputeHash(messageBytes);
            return BitConverter.ToString(hashMessage).Replace("-", "").ToLower();
        }
    }
}
