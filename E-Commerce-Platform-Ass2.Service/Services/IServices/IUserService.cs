using System;
using System.Threading.Tasks;
using E_Commerce_Platform_Ass2.Service.Services;

namespace E_Commerce_Platform_Ass2.Service.Services.IServices
{
    public interface IUserService
    {
        Task<bool> RegisterAsync(string name, string email, string password);

        Task<AuthenticatedUser?> ValidateUserAsync(string email, string password);

        Task<AuthenticatedUser?> GetUserByIdAsync(Guid userId);

        /// <summary>
        /// Đăng ký tài khoản với xác thực email
        /// </summary>
        Task<RegisterResult> RegisterWithVerificationAsync(string name, string email, string password, string baseUrl);

        /// <summary>
        /// Xác thực email bằng token
        /// </summary>
        Task<VerifyEmailResult> VerifyEmailAsync(string token);

        /// <summary>
        /// Gửi lại email xác thực
        /// </summary>
        Task<ResendVerificationResult> ResendVerificationEmailAsync(string email, string baseUrl);

        /// <summary>
        /// Kiểm tra email đã được xác thực chưa
        /// </summary>
        Task<bool> IsEmailVerifiedAsync(string email);
    }

    public class RegisterResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public bool EmailSent { get; set; }
    }

    public class VerifyEmailResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public string? UserName { get; set; }
    }

    public class ResendVerificationResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
    }
}

