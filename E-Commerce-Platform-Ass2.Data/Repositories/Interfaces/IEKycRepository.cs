using E_Commerce_Platform_Ass2.Data.Database.Entities;

namespace E_Commerce_Platform_Ass2.Data.Repositories.Interfaces
{
    public interface IEKycRepository
    {
        Task AddAsync(EKycVerification entity);
        Task<bool> IsUserVerifiedAsync(Guid userId);
        Task<bool> IsCccdNumberUsedAsync(string cccdNumber);
    }
}

