using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using E_Commerce_Platform_Ass2.Service.DTOs;

namespace E_Commerce_Platform_Ass2.Service.Services.IServices
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewDto>> GetAllAsync();
        Task<IEnumerable<ReviewDto>> GetReviewsByUserIdAsync(Guid userId);
        Task<IEnumerable<ReviewDto>> GetReviewsByProductIdAsync(Guid productId);
        Task<ReviewDto> GetReviewByIdAsync(Guid id);
        Task<IEnumerable<ReviewDto>> GetReviewByRatingAsync(int rating);
        Task<ReviewDto> CreateReviewAsync(Guid userId, Guid productId, int rating, string comment, IFormFile? image);
        Task<ReviewDto> ApproveReviewAsync(Guid id);
        Task<ReviewDto> RejectReviewAsync(Guid id);
        Task<ReviewDto> UpdateReviewAsync(Guid id, Guid userId, int rating, string comment);
        Task DeleteReviewAsync(Guid id, Guid userId);
    }
}
