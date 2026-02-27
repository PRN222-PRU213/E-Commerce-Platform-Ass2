using System.Text;
using System.Text.Json;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using Microsoft.Extensions.Configuration;

namespace E_Commerce_Platform_Ass2.Service.Services
{
    public class GeminiService : IGeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private const string ApiUrl = "https://generativelanguage.googleapis.com/v1/models/gemini-1.5-flash:generateContent";

        public GeminiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["Gemini:ApiKey"] ?? throw new ArgumentNullException("Gemini:ApiKey is missing.");
        }

        public async Task<string> GenerateContentAsync(string prompt)
        {
            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                },
                safetySettings = new[]
                {
                    new { category = "HARM_CATEGORY_HARASSMENT", threshold = "BLOCK_NONE" },
                    new { category = "HARM_CATEGORY_HATE_SPEECH", threshold = "BLOCK_NONE" },
                    new { category = "HARM_CATEGORY_SEXUALLY_EXPLICIT", threshold = "BLOCK_NONE" },
                    new { category = "HARM_CATEGORY_DANGEROUS_CONTENT", threshold = "BLOCK_NONE" }
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{ApiUrl}?key={_apiKey}", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Gemini API error: {response.StatusCode} - {errorContent}");
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseBody);
            
            try
            {
                return doc.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString() ?? string.Empty;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to parse Gemini API response.", ex);
            }
        }
    }
}
