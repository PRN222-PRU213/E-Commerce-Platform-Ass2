using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce_Platform_Ass1.Data.Database.Entities;

namespace E_Commerce_Platform_Ass1.Data.Repositories.Interfaces
{
    public interface IReviewRepository
    {
        Task<IEnumerable<Review>> GetAllAsync();
        Task<Review?> GetByIdAsync(Guid id);
        Task<Review?> GetByProductIdAsync(Guid productId);
        Task<IEnumerable<Review>> GetByUserIdAsync(Guid userId);
        Task<Review> AddAsync(Review newReview);
        Task<Review> UpdateAsync(Review review);
    }
}
