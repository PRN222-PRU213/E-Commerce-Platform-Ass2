using Microsoft.AspNetCore.Http;

namespace E_Commerce_Platform_Ass2.Service.Services.IServices
{
    public interface ICloudinaryService
    {
        Task<string> UploadImageAsync(IFormFile file, string folder);
    }
}
