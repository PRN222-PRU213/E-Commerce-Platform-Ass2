using E_Commerce_Platform_Ass2.Data.Database.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace E_Commerce_Platform_Ass2.Data.Repositories.Interfaces
{
    public interface ICannedResponseRepository
    {
        Task<CannedResponse> CreateAsync(CannedResponse response);
        Task<CannedResponse?> GetByIdAsync(Guid id);
        Task<List<CannedResponse>> GetAllAsync(string? category = null, bool activeOnly = true);
        Task<List<CannedResponse>> GetByCategoryAsync(string category);
        Task<CannedResponse> UpdateAsync(CannedResponse response);
        Task<bool> DeleteAsync(Guid id);
    }
}
