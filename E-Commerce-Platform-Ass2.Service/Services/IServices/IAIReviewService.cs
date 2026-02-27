using E_Commerce_Platform_Ass2.Service.DTOs;

namespace E_Commerce_Platform_Ass2.Service.Services.IServices
{
    public interface IAIReviewService
    {
        Task<ReviewAnalysisResult> AnalyzeReviewAsync(string comment);
    }
}
