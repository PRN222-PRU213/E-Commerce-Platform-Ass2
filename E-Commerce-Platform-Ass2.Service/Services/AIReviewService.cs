using System.Text.Json;
using E_Commerce_Platform_Ass2.Service.DTOs;
using E_Commerce_Platform_Ass2.Service.Services.IServices;

namespace E_Commerce_Platform_Ass2.Service.Services
{
    public class AIReviewService : IAIReviewService
    {
        private readonly IGeminiService _geminiService;

        public AIReviewService(IGeminiService geminiService)
        {
            _geminiService = geminiService;
        }

        public async Task<ReviewAnalysisResult> AnalyzeReviewAsync(string comment)
        {
            if (string.IsNullOrWhiteSpace(comment))
            {
                return new ReviewAnalysisResult
                {
                    Suggestion = "Reject",
                    Reason = "Nội dung trống."
                };
            }

            var lower = comment.ToLower().Trim();

            // 1. Fast-Track: Manual black-list for absolute rejects
            var blackList = new[] { "đụ", "lồn", "cặc", "đm", "dm", "vcl", "vl", "con mẹ", "thàng chó" };
            if (blackList.Any(w => lower.Contains(w)))
            {
                return new ReviewAnalysisResult { Suggestion = "Reject", Reason = "Chứa từ ngữ thô tục (Bộ lọc nhanh)." };
            }

            // 2. Fast-Track: Manual white-list for absolute approvals
            var whiteList = new[] { "tốt", "đẹp", "tuyệt", "ưng ý", "chất lượng", "cảm ơn", "ok", "đã", "đỉnh" };
            if (whiteList.Any(w => lower.Contains(w)) && lower.Length < 50 && !lower.Contains("?"))
            {
                // If it's short and contains positive words, approve immediately
                return new ReviewAnalysisResult { Suggestion = "Approve", Reason = "Nội dung tích cực (Bộ lọc nhanh)." };
            }

            try
            {
                var prompt = $@"
Bạn là một trợ lý kiểm duyệt nội dung công bằng và chính xác cho hệ thống thương mại điện tử tại Việt Nam.

Nội dung cần kiểm tra: ""{comment}""

QUY TẮC PHÂN LOẠI:
1. 'Approve' (Rõ ràng): 
   - Khen sản phẩm: ""Sản phẩm tốt"", ""Quá đã"", ""Tuyệt vời"", ""Hàng đẹp"",...
   - Chê sản phẩm (Lịch sự): ""Sản phẩm tệ"", ""Bán hàng không tốt"", ""Giao hàng chậm"" -> Những lời chê văn minh này BẮT BUỘC phải là 'Approve'.
   - Nhận xét trung lập, đánh giá sao.
2. 'Reject' (Vi phạm): 
   - Chứa từ chửi thề, thô tục (đụ, má, lồn, cặc, chó, đm, dm, vcl, vl, đéo, cl, cc,...).
   - Xúc phạm, đe dọa, hoặc quảng cáo rác.
3. 'Potential' (Cần xem xét): 
   - Chuỗi ký tự vô nghĩa lặp lại (aaaaa, hhhhh).
   - Viết tắt quá nhiều không hiểu được.

HÃY TRẢ VỀ CHỈ DUY NHẤT JSON:
{{
  ""Suggestion"": ""[Approve/Potential/Reject]"",
  ""Reason"": ""[Lý do chi tiết bằng tiếng Việt]""
}}
";

                var response = await _geminiService.GenerateContentAsync(prompt);
                
                // Extract JSON from response (sometimes AI wraps it in markdown blocks)
                var jsonMatch = System.Text.RegularExpressions.Regex.Match(response, @"\{.*\}", System.Text.RegularExpressions.RegexOptions.Singleline);
                if (jsonMatch.Success)
                {
                    var result = JsonSerializer.Deserialize<ReviewAnalysisResult>(jsonMatch.Value, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return result ?? new ReviewAnalysisResult { Suggestion = "Potential", Reason = "Không thể phân tích kết quả AI." };
                }

                return new ReviewAnalysisResult
                {
                    Suggestion = "Potential",
                    Reason = "Kết quả AI không đúng định dạng."
                };
            }
            catch (Exception)
            {
                // Fallback to basic safety check or manual review if API fails
                return new ReviewAnalysisResult
                {
                    Suggestion = "Potential",
                    Reason = "Lỗi kết nối hệ thống AI. Cần xem xét thủ công."
                };
            }
        }
    }
}
