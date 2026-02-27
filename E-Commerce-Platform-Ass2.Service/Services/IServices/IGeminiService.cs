namespace E_Commerce_Platform_Ass2.Service.Services.IServices
{
    public interface IGeminiService
    {
        Task<string> GenerateContentAsync(string prompt);
    }
}
