using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using E_Commerce_Platform_Ass2.Service.Services.IServices;

namespace E_Commerce_Platform_Ass2.Service.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly string _mail;
        private readonly string _displayName;
        private readonly string _password;
        private readonly string _host;
        private readonly int _port;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
            _mail = configuration["EmailSettings:Mail"] ?? throw new InvalidOperationException("EmailSettings:Mail not configured");
            _displayName = configuration["EmailSettings:DisplayName"] ?? "E-Commerce Support";
            _password = configuration["EmailSettings:Password"] ?? throw new InvalidOperationException("EmailSettings:Password not configured");
            _host = configuration["EmailSettings:Host"] ?? "smtp.gmail.com";
            _port = int.Parse(configuration["EmailSettings:Port"] ?? "587");
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            var email = new MimeMessage();
            email.Sender = new MailboxAddress(_displayName, _mail);
            email.From.Add(new MailboxAddress(_displayName, _mail));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;

            var builder = new BodyBuilder
            {
                HtmlBody = htmlBody
            };
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            try
            {
                await smtp.ConnectAsync(_host, _port, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_mail, _password);
                await smtp.SendAsync(email);
            }
            finally
            {
                await smtp.DisconnectAsync(true);
            }
        }

        public async Task SendVerificationEmailAsync(string toEmail, string userName, string verificationLink)
        {
            var subject = "Xác thực tài khoản - E-Commerce Platform";
            var htmlBody = GetVerificationEmailTemplate(userName, verificationLink);
            await SendEmailAsync(toEmail, subject, htmlBody);
        }

        public async Task SendVerificationSuccessEmailAsync(string toEmail, string userName)
        {
            var subject = "Xác thực thành công - E-Commerce Platform";
            var htmlBody = GetVerificationSuccessTemplate(userName);
            await SendEmailAsync(toEmail, subject, htmlBody);
        }

        private string GetVerificationEmailTemplate(string userName, string verificationLink)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Xác thực Email</title>
</head>
<body style='margin: 0; padding: 0; font-family: Arial, sans-serif; background-color: #f4f4f4;'>
    <table role='presentation' style='width: 100%; border-collapse: collapse;'>
        <tr>
            <td style='padding: 40px 0;'>
                <table role='presentation' style='max-width: 600px; margin: 0 auto; background-color: #ffffff; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1);'>
                    <!-- Header -->
                    <tr>
                        <td style='background: linear-gradient(135deg, #D19C97 0%, #c88a84 100%); padding: 40px 30px; text-align: center; border-radius: 10px 10px 0 0;'>
                            <h1 style='color: #ffffff; margin: 0; font-size: 28px;'>🛒 E-Commerce Platform</h1>
                        </td>
                    </tr>
                    
                    <!-- Content -->
                    <tr>
                        <td style='padding: 40px 30px;'>
                            <h2 style='color: #333333; margin: 0 0 20px 0;'>Xin chào {userName}! 👋</h2>
                            <p style='color: #666666; font-size: 16px; line-height: 1.6; margin: 0 0 20px 0;'>
                                Cảm ơn bạn đã đăng ký tài khoản tại <strong>E-Commerce Platform</strong>. 
                                Để hoàn tất quá trình đăng ký, vui lòng xác thực email của bạn bằng cách nhấn vào nút bên dưới:
                            </p>
                            
                            <!-- Button -->
                            <table role='presentation' style='margin: 30px auto;'>
                                <tr>
                                    <td style='background: linear-gradient(135deg, #D19C97 0%, #c88a84 100%); border-radius: 8px;'>
                                        <a href='{verificationLink}' style='display: inline-block; padding: 15px 40px; color: #ffffff; text-decoration: none; font-size: 16px; font-weight: bold;'>
                                            ✅ Xác thực Email
                                        </a>
                                    </td>
                                </tr>
                            </table>
                            
                            <p style='color: #666666; font-size: 14px; line-height: 1.6; margin: 20px 0;'>
                                Hoặc copy và dán link sau vào trình duyệt:<br>
                                <a href='{verificationLink}' style='color: #D19C97; word-break: break-all;'>{verificationLink}</a>
                            </p>
                            
                            <div style='background-color: #fff3cd; border: 1px solid #ffc107; border-radius: 8px; padding: 15px; margin: 20px 0;'>
                                <p style='color: #856404; font-size: 14px; margin: 0;'>
                                    ⏰ <strong>Lưu ý:</strong> Link xác thực này sẽ hết hạn sau <strong>24 giờ</strong>.
                                </p>
                            </div>
                            
                            <p style='color: #999999; font-size: 14px; margin: 20px 0 0 0;'>
                                Nếu bạn không đăng ký tài khoản này, vui lòng bỏ qua email này.
                            </p>
                        </td>
                    </tr>
                    
                    <!-- Footer -->
                    <tr>
                        <td style='background-color: #f8f9fa; padding: 20px 30px; text-align: center; border-radius: 0 0 10px 10px;'>
                            <p style='color: #999999; font-size: 12px; margin: 0;'>
                                © 2026 E-Commerce Platform. All rights reserved.
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
        }

        private string GetVerificationSuccessTemplate(string userName)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Xác thực thành công</title>
</head>
<body style='margin: 0; padding: 0; font-family: Arial, sans-serif; background-color: #f4f4f4;'>
    <table role='presentation' style='width: 100%; border-collapse: collapse;'>
        <tr>
            <td style='padding: 40px 0;'>
                <table role='presentation' style='max-width: 600px; margin: 0 auto; background-color: #ffffff; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1);'>
                    <!-- Header -->
                    <tr>
                        <td style='background: linear-gradient(135deg, #28a745 0%, #20c997 100%); padding: 40px 30px; text-align: center; border-radius: 10px 10px 0 0;'>
                            <h1 style='color: #ffffff; margin: 0; font-size: 28px;'>✅ Xác thực thành công!</h1>
                        </td>
                    </tr>
                    
                    <!-- Content -->
                    <tr>
                        <td style='padding: 40px 30px; text-align: center;'>
                            <div style='font-size: 64px; margin-bottom: 20px;'>🎉</div>
                            <h2 style='color: #333333; margin: 0 0 20px 0;'>Chào mừng {userName}!</h2>
                            <p style='color: #666666; font-size: 16px; line-height: 1.6; margin: 0 0 20px 0;'>
                                Email của bạn đã được xác thực thành công. Bây giờ bạn có thể đăng nhập và khám phá hàng ngàn sản phẩm tuyệt vời!
                            </p>
                            
                            <div style='background-color: #d4edda; border: 1px solid #28a745; border-radius: 8px; padding: 15px; margin: 20px 0;'>
                                <p style='color: #155724; font-size: 14px; margin: 0;'>
                                    🛒 Tài khoản của bạn đã sẵn sàng để mua sắm!
                                </p>
                            </div>
                        </td>
                    </tr>
                    
                    <!-- Footer -->
                    <tr>
                        <td style='background-color: #f8f9fa; padding: 20px 30px; text-align: center; border-radius: 0 0 10px 10px;'>
                            <p style='color: #999999; font-size: 12px; margin: 0;'>
                                © 2026 E-Commerce Platform. All rights reserved.
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
        }
    }
}
