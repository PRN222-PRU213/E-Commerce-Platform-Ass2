using E_Commerce_Platform_Ass2.Data.Database.Entities;
using E_Commerce_Platform_Ass2.Data.Repositories.Interfaces;
using E_Commerce_Platform_Ass2.Service.DTOs;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using Microsoft.AspNetCore.Http;

namespace E_Commerce_Platform_Ass2.Service.Services
{
    public class EKycService : IEkycService
    {
        private readonly IVnptEKycService _vnptEKycService;
        private readonly IEKycRepository _eKycRepository;

        public EKycService(IVnptEKycService vnptEKycService, IEKycRepository eKycRepository)
        {
            _vnptEKycService = vnptEKycService;
            _eKycRepository = eKycRepository;
        }

        public async Task<bool> IsUserVerifiedAsync(Guid userId)
        {
            return await _eKycRepository.IsUserVerifiedAsync(userId);
        }

        public async Task<EKycResult> VerifyAndSaveAsync(Guid userId, IFormFile front, IFormFile back, IFormFile selfie)
        {
            var result = await _vnptEKycService.VerifyAsync(front, back, selfie);
            if (!result.IsSuccess)
            {
                return result;
            }

            bool isCccdUsed = await _eKycRepository.IsCccdNumberUsedAsync(result.CCCDNumber);
            if (isCccdUsed)
            {
                return EKycResult.Fail("Số CCCD này đã được sử dụng cho một tài khoản khác.");
            }

            var entity = new EKycVerification
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CccdNumber = result.CCCDNumber,
                FullName = result.FullName,
                FaceMatchScore = result.FaceMatchScore,
                Liveness = true,
                Status = "VERIFIED",
                CreatedAt = DateTime.Now
            };

            await _eKycRepository.AddAsync(entity);
            return result;
        }
    }
}

