using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using E_Commerce_Platform_Ass2.Data.Database.Entities;
using E_Commerce_Platform_Ass2.Data.Repositories.Interfaces;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using E_Commerce_Platform_Ass2.Service.Utils;

namespace E_Commerce_Platform_Ass2.Service.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IEmailService _emailService;
        private const int TokenExpiryHours = 24;

        public UserService(IUserRepository userRepository, IRoleRepository roleRepository, IEmailService emailService)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _emailService = emailService;
        }

        public async Task<bool> RegisterAsync(string name, string email, string password)
        {
            var existing = await _userRepository.GetByEmailAsync(email);
            if (existing != null)
            {
                return false;
            }

            // Get default "User" role
            var userRole = await _roleRepository.GetByNameAsync("Customer");
            if (userRole == null)
            {
                throw new InvalidOperationException("Default 'Customer' role not found. Please seed roles first.");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = name,
                Email = email,
                PasswordHash = PasswordHasher.HashPassword(password),
                RoleId = userRole.RoleId,
                Status = true,
                CreatedAt = DateTime.UtcNow,
                EmailVerified = true, // Giữ backward compatibility - đăng ký cũ không cần verify
            };

            await _userRepository.CreateAsync(user);
            return true;
        }

        public async Task<RegisterResult> RegisterWithVerificationAsync(string name, string email, string password, string baseUrl)
        {
            var existing = await _userRepository.GetByEmailAsync(email);
            if (existing != null)
            {
                return new RegisterResult
                {
                    Success = false,
                    ErrorMessage = "Email này đã được sử dụng."
                };
            }

            // Get default role
            var userRole = await _roleRepository.GetByNameAsync("Customer");
            if (userRole == null)
            {
                return new RegisterResult
                {
                    Success = false,
                    ErrorMessage = "Lỗi hệ thống: Không tìm thấy role mặc định."
                };
            }

            // Generate verification token
            var token = GenerateVerificationToken();

            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = name,
                Email = email,
                PasswordHash = PasswordHasher.HashPassword(password),
                RoleId = userRole.RoleId,
                Status = true,
                CreatedAt = DateTime.UtcNow,
                EmailVerified = false,
                EmailVerificationToken = token,
                EmailVerificationTokenExpiry = DateTime.UtcNow.AddHours(TokenExpiryHours)
            };

            await _userRepository.CreateAsync(user);

            // Send verification email
            var verificationLink = $"{baseUrl}/Authentication/VerifyEmail?token={token}";
            var emailSent = false;

            try
            {
                await _emailService.SendVerificationEmailAsync(email, name, verificationLink);
                emailSent = true;
            }
            catch (Exception)
            {
                // Log error but don't fail registration
                // User can resend verification email later
            }

            return new RegisterResult
            {
                Success = true,
                EmailSent = emailSent
            };
        }

        public async Task<VerifyEmailResult> VerifyEmailAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return new VerifyEmailResult
                {
                    Success = false,
                    ErrorMessage = "Token không hợp lệ."
                };
            }

            var user = await _userRepository.GetByVerificationTokenAsync(token);
            if (user == null)
            {
                return new VerifyEmailResult
                {
                    Success = false,
                    ErrorMessage = "Token không tồn tại hoặc đã được sử dụng."
                };
            }

            if (user.EmailVerified)
            {
                return new VerifyEmailResult
                {
                    Success = true,
                    UserName = user.Name,
                    ErrorMessage = "Email đã được xác thực trước đó."
                };
            }

            if (user.EmailVerificationTokenExpiry < DateTime.UtcNow)
            {
                return new VerifyEmailResult
                {
                    Success = false,
                    ErrorMessage = "Token đã hết hạn. Vui lòng yêu cầu gửi lại email xác thực."
                };
            }

            // Verify email
            user.EmailVerified = true;
            user.EmailVerificationToken = null;
            user.EmailVerificationTokenExpiry = null;
            await _userRepository.UpdateAsync(user);

            // Send success email
            try
            {
                await _emailService.SendVerificationSuccessEmailAsync(user.Email, user.Name);
            }
            catch { /* Ignore email errors */ }

            return new VerifyEmailResult
            {
                Success = true,
                UserName = user.Name
            };
        }

        public async Task<ResendVerificationResult> ResendVerificationEmailAsync(string email, string baseUrl)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                return new ResendVerificationResult
                {
                    Success = false,
                    ErrorMessage = "Email không tồn tại trong hệ thống."
                };
            }

            if (user.EmailVerified)
            {
                return new ResendVerificationResult
                {
                    Success = false,
                    ErrorMessage = "Email đã được xác thực. Bạn có thể đăng nhập ngay."
                };
            }

            // Generate new token
            var token = GenerateVerificationToken();
            user.EmailVerificationToken = token;
            user.EmailVerificationTokenExpiry = DateTime.UtcNow.AddHours(TokenExpiryHours);
            await _userRepository.UpdateAsync(user);

            // Send email
            var verificationLink = $"{baseUrl}/Authentication/VerifyEmail?token={token}";
            try
            {
                await _emailService.SendVerificationEmailAsync(email, user.Name, verificationLink);
            }
            catch (Exception ex)
            {
                return new ResendVerificationResult
                {
                    Success = false,
                    ErrorMessage = $"Không thể gửi email: {ex.Message}"
                };
            }

            return new ResendVerificationResult
            {
                Success = true
            };
        }

        public async Task<bool> IsEmailVerifiedAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            return user?.EmailVerified ?? false;
        }

        public async Task<AuthenticatedUser?> ValidateUserAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                return null;
            }

            // Check if email is verified
            if (!user.EmailVerified)
            {
                return null; // Will be handled differently in controller
            }

            // Verify password with backward compatibility (supports both hash and plain text)
            if (!PasswordHasher.VerifyPasswordWithBackwardCompat(password, user.PasswordHash))
            {
                return null;
            }

            // If password is stored as plain text, automatically hash it for security
            if (!PasswordHasher.IsBcryptHash(user.PasswordHash))
            {
                user.PasswordHash = PasswordHasher.HashPassword(password);
                await _userRepository.UpdateAsync(user);
            }

            return new AuthenticatedUser
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role?.Name ?? "Unknown",
            };
        }

        public async Task<AuthenticatedUser?> GetUserByIdAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return null;
            }

            return new AuthenticatedUser
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role?.Name ?? "Unknown"
            };
        }

        private static string GenerateVerificationToken()
        {
            var bytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            return Convert.ToBase64String(bytes)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");
        }
    }
}