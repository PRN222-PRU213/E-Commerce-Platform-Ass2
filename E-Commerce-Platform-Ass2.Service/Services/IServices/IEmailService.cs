using System.Threading.Tasks;

namespace E_Commerce_Platform_Ass2.Service.Services.IServices
{
    public interface IEmailService
    {
        /// <summary>
        /// Gửi email với nội dung HTML tùy chỉnh
        /// </summary>
        Task SendEmailAsync(string toEmail, string subject, string htmlBody);

        /// <summary>
        /// Gửi email xác thực tài khoản
        /// </summary>
        Task SendVerificationEmailAsync(string toEmail, string userName, string verificationLink);

        /// <summary>
        /// Gửi email thông báo xác thực thành công
        /// </summary>
        Task SendVerificationSuccessEmailAsync(string toEmail, string userName);
    }
}
