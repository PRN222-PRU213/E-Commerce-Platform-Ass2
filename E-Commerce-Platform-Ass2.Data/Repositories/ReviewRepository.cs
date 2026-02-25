using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce_Platform_Ass2.Data.Database;
using E_Commerce_Platform_Ass2.Data.Database.Entities;
using E_Commerce_Platform_Ass2.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_Platform_Ass2.Data.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly ApplicationDbContext _context;

        public ReviewRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Review> AddAsync(Review newReview)
        {
            _context.Reviews.Add(newReview);
            await _context.SaveChangesAsync();
            return newReview;
        }

        public async Task DeleteAsync(Review review)
        {
            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Review>> GetAllAsync()
        {
            return await _context.Reviews
                .Include(x => x.User)
                .ToListAsync();
        }

        public async Task<Review?> GetByIdAndUserIdAsync(Guid id, Guid userId)
        {
            return await _context.Reviews
                .Where(x => x.Id == id && x.UserId == userId)
                .Include(x => x.User)
                .FirstOrDefaultAsync();
        }

        public async Task<Review?> GetByIdAsync(Guid id)
        {
            return await _context.Reviews.FindAsync(id);
        }

        public async Task<IEnumerable<Review>> GetByProductIdAsync(Guid productId)
        {
            return await _context.Reviews
                .Where(r => r.ProductId == productId)
                .Include(x => x.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetByRatingAsync(int rating)
        {
            return await _context.Reviews
                .Where(x => x.Rating == rating)
                .Include(x => x.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Reviews
                .Where(r => r.UserId == userId)
                .Include(r => r.User)
                .ToListAsync();
        }

        public async Task<Review> UpdateAsync(Review review)
        {
            _context.Reviews.Update(review);
            await _context.SaveChangesAsync();
            return review;
        }
    }
}
