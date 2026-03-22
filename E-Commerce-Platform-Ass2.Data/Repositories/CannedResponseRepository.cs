using E_Commerce_Platform_Ass2.Data.Database;
using E_Commerce_Platform_Ass2.Data.Database.Entities;
using E_Commerce_Platform_Ass2.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Commerce_Platform_Ass2.Data.Repositories
{
    public class CannedResponseRepository : ICannedResponseRepository
    {
        private readonly ApplicationDbContext _context;

        public CannedResponseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CannedResponse> CreateAsync(CannedResponse response)
        {
            response.Id = Guid.NewGuid();
            response.CreatedAt = DateTime.UtcNow;
            await _context.CannedResponses.AddAsync(response);
            await _context.SaveChangesAsync();
            return response;
        }

        public async Task<CannedResponse?> GetByIdAsync(Guid id)
        {
            return await _context.CannedResponses
                .Include(x => x.CreatedBy)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<CannedResponse>> GetAllAsync(string? category = null, bool activeOnly = true)
        {
            var query = _context.CannedResponses
                .Include(x => x.CreatedBy)
                .AsQueryable();

            if (activeOnly)
            {
                query = query.Where(x => x.IsActive);
            }

            if (!string.IsNullOrEmpty(category) && category != "All")
            {
                query = query.Where(x => x.Category == category);
            }

            return await query
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.Title)
                .ToListAsync();
        }

        public async Task<List<CannedResponse>> GetByCategoryAsync(string category)
        {
            return await _context.CannedResponses
                .Where(x => x.Category == category && x.IsActive)
                .OrderBy(x => x.SortOrder)
                .ToListAsync();
        }

        public async Task<CannedResponse> UpdateAsync(CannedResponse response)
        {
            response.UpdatedAt = DateTime.UtcNow;
            _context.CannedResponses.Update(response);
            await _context.SaveChangesAsync();
            return response;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var response = await _context.CannedResponses.FindAsync(id);
            if (response == null) return false;
            _context.CannedResponses.Remove(response);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
