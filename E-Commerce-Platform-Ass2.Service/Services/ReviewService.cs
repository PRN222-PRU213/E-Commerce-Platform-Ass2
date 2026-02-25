using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce_Platform_Ass2.Data.Database.Entities;
using E_Commerce_Platform_Ass2.Data.Repositories.Interfaces;
using E_Commerce_Platform_Ass2.Service.DTOs;
using E_Commerce_Platform_Ass2.Service.Services.IServices;

namespace E_Commerce_Platform_Ass2.Service.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IUserRepository _userRepository;

        public ReviewService(IReviewRepository reviewRepository, IUserRepository userRepository)
        {
            _reviewRepository = reviewRepository;
            _userRepository = userRepository;
        }

        public async Task<ReviewDto> CreateReviewAsync(Guid userId, int rating, string comment)
        {
            // 1️⃣ Validate rating
            if (rating < 1 || rating > 5)
            {
                throw new ArgumentException("Rating phải từ 1 đến 5.");
            }

            // 2️⃣ Validate comment null / empty
            if (string.IsNullOrWhiteSpace(comment))
            {
                throw new ArgumentException("Nội dung đánh giá không được để trống.");
            }

            comment = comment.Trim();

            // 3️⃣ Kiểm tra viết tắt phổ biến
            var forbiddenWords = new List<string>
            {
                "ko", "k", "dc", "đc", "ok", "tks", "sp", "nv",
                "cx", "vs", "bt", "mik", "mk", "j", "z"
            };

            var lowerComment = comment.ToLower();

            foreach (var word in forbiddenWords)
            {
                // kiểm tra theo word boundary để tránh dính chữ khác
                if (lowerComment.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                                .Contains(word))
                {
                    throw new ArgumentException(
                        $"Không được sử dụng từ viết tắt '{word}'. Vui lòng viết đầy đủ và rõ ràng."
                    );
                }
            }

            var review = new Review
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Rating = rating,
                Comment = comment,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            await _reviewRepository.AddAsync(review);

            return new ReviewDto
            {
                Id = review.Id,
                UserId = review.UserId,
                UserName = review.User.Name,
                Rating = review.Rating,
                Comment = review.Comment,
                Status = review.Status,
                CreatedAt = review.CreatedAt
            };
        }

        public async Task DeleteReviewAsync(Guid id, Guid userId)
        {
            var review = await _reviewRepository.GetByIdAndUserIdAsync(id, userId);
            if (review == null)
            {
                throw new Exception("Bạn không có bình luận này.");
            }

            await _reviewRepository.DeleteAsync(review);
        }

        public async Task<IEnumerable<ReviewDto>> GetAllAsync()
        {
            var reviews = await _reviewRepository.GetAllAsync();
            return reviews.Select(review => new ReviewDto
            {
                Id = review.Id,
                UserId = review.UserId,
                UserName = review.User.Name,
                ProductId = review.ProductId,
                Rating = review.Rating,
                Comment = review.Comment,
                Status = review.Status,
                CreatedAt = review.CreatedAt
            });
        }

        public async Task<ReviewDto> GetReviewByIdAsync(Guid id)
        {
            var review = await _reviewRepository.GetByIdAsync(id);

            if (review == null)
            {
                throw new Exception("Không tồn tại bình luận này.");
            }

            return new ReviewDto
            {
                Id = review.Id,
                UserId = review.UserId,
                UserName = review.User.Name,
                ProductId = review.ProductId,
                Rating = review.Rating,
                Comment = review.Comment,
                Status = review.Status,
                CreatedAt = review.CreatedAt
            };
        }

        public async Task<IEnumerable<ReviewDto>> GetReviewByRatingAsync(int rating)
        {
            var reviews = await _reviewRepository.GetByRatingAsync(rating);

            if (!reviews.Any())
            {
                return new List<ReviewDto>();
            }

            return reviews.Select(r => new ReviewDto
            {
                Id = r.Id,
                UserId = r.UserId,
                UserName = r.User.Name,
                ProductId = r.ProductId,
                Rating = r.Rating,
                Comment = r.Comment,
                Status = r.Status,
                CreatedAt = r.CreatedAt
            });
        }

        public async Task<IEnumerable<ReviewDto>> GetReviewsByProductIdAsync(Guid productId)
        {
            var reviews = await _reviewRepository.GetByProductIdAsync(productId);

            if (!reviews.Any())
            {
                return new List<ReviewDto>();
            }

            return reviews.Select(r => new ReviewDto
            {
                Id = r.Id,
                UserId = r.UserId,
                UserName = r.User.Name,
                ProductId = r.ProductId,
                Rating = r.Rating,
                Comment = r.Comment,
                Status = r.Status,
                CreatedAt = r.CreatedAt
            });
        }

        public async Task<IEnumerable<ReviewDto>> GetReviewsByUserIdAsync(Guid userId)
        {
            var reviews = await _reviewRepository.GetByUserIdAsync(userId);
            if (!reviews.Any())
            {
                return new List<ReviewDto>();
            }

            return reviews.Select(r => new ReviewDto
            {
                Id = r.Id,
                UserId = r.UserId,
                UserName = r.User.Name,
                ProductId = r.ProductId,
                Rating = r.Rating,
                Comment = r.Comment,
                Status = r.Status,
                CreatedAt = r.CreatedAt
            });
        }

        public async Task<ReviewDto> UpdateReviewAsync(Guid id, Guid userId, int rating, string comment)
        {
            var review = await _reviewRepository.GetByIdAndUserIdAsync(id, userId);
            if (review == null)
            {
                throw new Exception("Bạn không có bình luận này.");
            }

            review.Rating = rating;
            review.Comment = comment;
            await _reviewRepository.UpdateAsync(review);

            return new ReviewDto
            {
                Id = review.Id,
                UserId = review.UserId,
                UserName = review.User.Name,
                ProductId = review.ProductId,
                Rating = review.Rating,
                Comment = review.Comment,
                Status = review.Status,
                CreatedAt = review.CreatedAt,
                ModeratedAt = DateTime.UtcNow
            };
        }
    }
}
