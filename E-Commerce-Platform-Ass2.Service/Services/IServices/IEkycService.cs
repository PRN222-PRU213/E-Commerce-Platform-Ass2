using E_Commerce_Platform_Ass2.Service.DTOs;
using Microsoft.AspNetCore.Http;

namespace E_Commerce_Platform_Ass2.Service.Services.IServices
{
    public interface IEkycService
    {
        Task<EKycResult> VerifyAndSaveAsync(Guid userId, IFormFile front, IFormFile back, IFormFile selfie);

        Task<bool> IsUserVerifiedAsync(Guid userId);
    }
}

