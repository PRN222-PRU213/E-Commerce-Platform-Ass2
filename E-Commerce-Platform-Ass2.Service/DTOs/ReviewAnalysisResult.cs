namespace E_Commerce_Platform_Ass2.Service.DTOs
{
    public class ReviewAnalysisResult
    {
        public string Suggestion { get; set; } = string.Empty; // "Approve", "Reject", "Potential"
        public string Reason { get; set; } = string.Empty;
    }
}
