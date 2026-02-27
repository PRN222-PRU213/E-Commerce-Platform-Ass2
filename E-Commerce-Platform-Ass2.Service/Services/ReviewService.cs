using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce_Platform_Ass2.Data.Database.Entities;
using E_Commerce_Platform_Ass2.Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using E_Commerce_Platform_Ass2.Service.DTOs;
using E_Commerce_Platform_Ass2.Service.Services.IServices;

namespace E_Commerce_Platform_Ass2.Service.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IUserRepository _userRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly ICloudinaryService _cloudinaryService;

        public ReviewService(IReviewRepository reviewRepository, IUserRepository userRepository, IOrderRepository orderRepository, ICloudinaryService cloudinaryService)
        {
            _reviewRepository = reviewRepository;
            _userRepository = userRepository;
            _orderRepository = orderRepository;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<ReviewDto> CreateReviewAsync(Guid userId, Guid productId, int rating, string comment, IFormFile? image)
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

            // Kiểm tra xem user đã mua sản phẩm này chưa
            var exists = await _orderRepository.ExistsOrderAsync(userId, productId);
            if (!exists)
            {
                throw new Exception("Bạn không thể đánh giá sản phẩm chưa từng mua.");
            }

            var imageUrl = string.Empty;
            if (image != null && image.Length > 0)
            {
                imageUrl = await _cloudinaryService.UploadImageAsync(image, "reviews");
            }

            var review = new Review
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ProductId = productId,
                Rating = rating,
                Comment = comment,
                ImageUrl = imageUrl,
                Status = "Pending",
                CreatedAt = DateTime.Now
            };

            await _reviewRepository.AddAsync(review);

            return new ReviewDto
            {
                Id = review.Id,
                UserId = review.UserId,
                UserName = (await _userRepository.GetByIdAsync(userId))?.Name ?? "N/A",
                ProductId = review.ProductId,
                Rating = review.Rating,
                Comment = review.Comment,
                Status = review.Status,
                CreatedAt = review.CreatedAt,
                ImageUrl = review.ImageUrl
            };
        }

        public async Task<ReviewDto> ApproveReviewAsync(Guid id)
        {
            var review = await _reviewRepository.GetByIdAsync(id);
            if (review == null) throw new Exception("Không tìm thấy review.");

            review.Status = "Approved";
            review.ModeratedAt = DateTime.Now;
            await _reviewRepository.UpdateAsync(review);

            return new ReviewDto
            {
                Id = review.Id,
                UserId = review.UserId,
                UserName = review.User?.Name ?? "N/A",
                ProductId = review.ProductId,
                Rating = review.Rating,
                Comment = review.Comment,
                Status = review.Status,
                CreatedAt = review.CreatedAt,
                ModeratedAt = review.ModeratedAt,
                ImageUrl = review.ImageUrl
            };
        }

        public async Task<ReviewDto> RejectReviewAsync(Guid id)
        {
            var review = await _reviewRepository.GetByIdAsync(id);
            if (review == null) throw new Exception("Không tìm thấy review.");

            review.Status = "Rejected";
            review.ModeratedAt = DateTime.Now;
            await _reviewRepository.UpdateAsync(review);

            return new ReviewDto
            {
                Id = review.Id,
                UserId = review.UserId,
                UserName = review.User?.Name ?? "N/A",
                ProductId = review.ProductId,
                Rating = review.Rating,
                Comment = review.Comment,
                Status = review.Status,
                CreatedAt = review.CreatedAt,
                ModeratedAt = review.ModeratedAt
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
                CreatedAt = review.CreatedAt,
                ImageUrl = review.ImageUrl
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
                CreatedAt = review.CreatedAt,
                ImageUrl = review.ImageUrl
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
                CreatedAt = r.CreatedAt,
                ImageUrl = r.ImageUrl
            });
        }

        public async Task<IEnumerable<ReviewDto>> GetReviewsByProductIdAsync(Guid productId)
        {
            var reviews = await _reviewRepository.GetByProductIdAsync(productId);

            if (!reviews.Any())
            {
                return new List<ReviewDto>();
            }

            return reviews
                .Where(r => r.Status == "Approved")
                .Select(r => new ReviewDto
            {
                Id = r.Id,
                UserId = r.UserId,
                UserName = r.User.Name,
                ProductId = r.ProductId,
                Rating = r.Rating,
                Comment = r.Comment,
                Status = r.Status,
                CreatedAt = r.CreatedAt,
                ImageUrl = r.ImageUrl
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
                CreatedAt = r.CreatedAt,
                ImageUrl = r.ImageUrl
            });
        }

        public async Task<ReviewDto> UpdateReviewAsync(Guid id, Guid userId, int rating, string comment)
        {
            var review = await _reviewRepository.GetByIdAndUserIdAsync(id, userId);
            if (review == null)
            {
                throw new Exception("Bạn không có bình luận này.");
            }

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

            review.Rating = rating;
            review.Comment = comment;
            review.Status = "Pending";
            review.ModeratedAt = null;
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
                ImageUrl = review.ImageUrl
            };
        }
    }
}
