using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RealEstatePlatform.Application.Interfaces.Services;
using System.Net;
using System.Net.Mail;

namespace RealEstatePlatform.Infrastructure.Services;

/// <summary>
/// Email service implementation using SMTP
/// </summary>
public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;
    private readonly string _fromEmail;
    private readonly string _fromName;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        _fromEmail = _configuration["Email:From"] ?? "noreply@realestate.com";
        _fromName = _configuration["Email:FromName"] ?? "Real Estate Platform";
    }

    public async Task SendEmailConfirmationAsync(string email, string name, string confirmationToken, CancellationToken cancellationToken = default)
    {
        var confirmUrl = $"{_configuration["AppUrl"]}/confirm-email?token={Uri.EscapeDataString(confirmationToken)}&email={Uri.EscapeDataString(email)}";
        
        var subject = "Xác nhận địa chỉ email của bạn";
        var htmlBody = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
</head>
<body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
        <h2 style='color: #2c3e50;'>Xin chào {name}!</h2>
        <p>Cảm ơn bạn đã đăng ký tài khoản tại Real Estate Platform.</p>
        <p>Vui lòng nhấn vào nút bên dưới để xác nhận địa chỉ email của bạn:</p>
        <div style='text-align: center; margin: 30px 0;'>
            <a href='{confirmUrl}' style='background-color: #3498db; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; display: inline-block;'>
                Xác nhận Email
            </a>
        </div>
        <p style='font-size: 12px; color: #7f8c8d;'>
            Nếu bạn không thể nhấn vào nút, vui lòng sao chép và dán đường link sau vào trình duyệt:<br>
            <a href='{confirmUrl}'>{confirmUrl}</a>
        </p>
        <p style='font-size: 12px; color: #7f8c8d; margin-top: 30px;'>
            Nếu bạn không đăng ký tài khoản này, vui lòng bỏ qua email này.
        </p>
    </div>
</body>
</html>";

        await SendEmailAsync(email, subject, htmlBody, cancellationToken);
    }

    public async Task SendPasswordResetAsync(string email, string name, string resetToken, CancellationToken cancellationToken = default)
    {
        var resetUrl = $"{_configuration["AppUrl"]}/reset-password?token={Uri.EscapeDataString(resetToken)}&email={Uri.EscapeDataString(email)}";
        
        var subject = "Đặt lại mật khẩu";
        var htmlBody = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
</head>
<body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
        <h2 style='color: #2c3e50;'>Xin chào {name}!</h2>
        <p>Chúng tôi nhận được yêu cầu đặt lại mật khẩu cho tài khoản của bạn.</p>
        <p>Nhấn vào nút bên dưới để đặt lại mật khẩu:</p>
        <div style='text-align: center; margin: 30px 0;'>
            <a href='{resetUrl}' style='background-color: #e74c3c; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; display: inline-block;'>
                Đặt lại mật khẩu
            </a>
        </div>
        <p style='font-size: 12px; color: #7f8c8d;'>
            Link này sẽ hết hạn sau 24 giờ.
        </p>
        <p style='font-size: 12px; color: #7f8c8d; margin-top: 30px;'>
            Nếu bạn không yêu cầu đặt lại mật khẩu, vui lòng bỏ qua email này.
        </p>
    </div>
</body>
</html>";

        await SendEmailAsync(email, subject, htmlBody, cancellationToken);
    }

    public async Task SendWelcomeEmailAsync(string email, string name, CancellationToken cancellationToken = default)
    {
        var subject = "Chào mừng bạn đến với Real Estate Platform!";
        var htmlBody = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
</head>
<body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
        <h2 style='color: #2c3e50;'>Chào mừng {name}!</h2>
        <p>Cảm ơn bạn đã tham gia Real Estate Platform - nền tảng giao dịch bất động sản hàng đầu Việt Nam.</p>
        <p>Bạn có thể bắt đầu:</p>
        <ul>
            <li>Tìm kiếm và lưu các tin đăng yêu thích</li>
            <li>Đăng tin bán/cho thuê bất động sản</li>
            <li>Theo dõi các môi giới uy tín</li>
            <li>Nhận thông báo về tin đăng phù hợp</li>
        </ul>
        <div style='text-align: center; margin: 30px 0;'>
            <a href='{_configuration["AppUrl"]}' style='background-color: #27ae60; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; display: inline-block;'>
                Khám phá ngay
            </a>
        </div>
    </div>
</body>
</html>";

        await SendEmailAsync(email, subject, htmlBody, cancellationToken);
    }

    public async Task SendContactRequestNotificationAsync(string adminEmail, string userName, string subject, string message, CancellationToken cancellationToken = default)
    {
        var emailSubject = $"Yêu cầu liên hệ mới từ {userName}";
        var htmlBody = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
</head>
<body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
        <h2>Yêu cầu liên hệ mới</h2>
        <p><strong>Từ:</strong> {userName}</p>
        <p><strong>Chủ đề:</strong> {subject}</p>
        <p><strong>Nội dung:</strong></p>
        <div style='background-color: #f5f5f5; padding: 15px; border-radius: 5px;'>
            {message}
        </div>
    </div>
</body>
</html>";

        await SendEmailAsync(adminEmail, emailSubject, htmlBody, cancellationToken);
    }

    public async Task SendContactResponseAsync(string email, string name, string subject, string response, CancellationToken cancellationToken = default)
    {
        var emailSubject = $"Phản hồi: {subject}";
        var htmlBody = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
</head>
<body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
        <h2>Xin chào {name}!</h2>
        <p>Cảm ơn bạn đã liên hệ với chúng tôi. Dưới đây là phản hồi của chúng tôi:</p>
        <div style='background-color: #f5f5f5; padding: 15px; border-radius: 5px; margin: 20px 0;'>
            {response}
        </div>
        <p>Nếu bạn có bất kỳ câu hỏi nào khác, vui lòng liên hệ lại với chúng tôi.</p>
    </div>
</body>
</html>";

        await SendEmailAsync(email, emailSubject, htmlBody, cancellationToken);
    }

    public async Task SendListingStatusChangeAsync(string email, string name, string listingTitle, string status, CancellationToken cancellationToken = default)
    {
        var subject = $"Cập nhật trạng thái tin đăng: {listingTitle}";
        var htmlBody = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
</head>
<body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
        <h2>Xin chào {name}!</h2>
        <p>Tin đăng của bạn <strong>{listingTitle}</strong> đã được cập nhật trạng thái thành: <strong>{status}</strong></p>
        <div style='text-align: center; margin: 30px 0;'>
            <a href='{_configuration["AppUrl"]}/my-listings' style='background-color: #3498db; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; display: inline-block;'>
                Xem tin đăng
            </a>
        </div>
    </div>
</body>
</html>";

        await SendEmailAsync(email, subject, htmlBody, cancellationToken);
    }

    public async Task SendNewMatchingListingAsync(string email, string name, string listingTitle, string listingUrl, CancellationToken cancellationToken = default)
    {
        var subject = "Tin đăng mới phù hợp với yêu cầu của bạn";
        var htmlBody = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
</head>
<body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
        <h2>Xin chào {name}!</h2>
        <p>Có tin đăng mới phù hợp với tiêu chí tìm kiếm của bạn:</p>
        <p><strong>{listingTitle}</strong></p>
        <div style='text-align: center; margin: 30px 0;'>
            <a href='{listingUrl}' style='background-color: #27ae60; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; display: inline-block;'>
                Xem chi tiết
            </a>
        </div>
    </div>
</body>
</html>";

        await SendEmailAsync(email, subject, htmlBody, cancellationToken);
    }

    public async Task SendPaymentConfirmationAsync(string email, string name, string transactionId, decimal amount, CancellationToken cancellationToken = default)
    {
        var subject = "Xác nhận thanh toán thành công";
        var htmlBody = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
</head>
<body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
        <h2>Xin chào {name}!</h2>
        <p>Giao dịch của bạn đã được xử lý thành công.</p>
        <table style='width: 100%; margin: 20px 0; border-collapse: collapse;'>
            <tr>
                <td style='padding: 10px; border-bottom: 1px solid #ddd;'><strong>Mã giao dịch:</strong></td>
                <td style='padding: 10px; border-bottom: 1px solid #ddd;'>{transactionId}</td>
            </tr>
            <tr>
                <td style='padding: 10px; border-bottom: 1px solid #ddd;'><strong>Số tiền:</strong></td>
                <td style='padding: 10px; border-bottom: 1px solid #ddd;'>{amount:N0} VNĐ</td>
            </tr>
        </table>
        <p>Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi!</p>
    </div>
</body>
</html>";

        await SendEmailAsync(email, subject, htmlBody, cancellationToken);
    }

    public async Task SendEmailAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken = default)
    {
        try
        {
            var smtpHost = _configuration["Email:SmtpHost"];
            var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
            var smtpUser = _configuration["Email:SmtpUser"];
            var smtpPass = _configuration["Email:SmtpPassword"];

            if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(smtpUser))
            {
                _logger.LogWarning("Email configuration is missing. Email not sent to {Email}", to);
                return;
            }

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = true
            };

            var message = new MailMessage
            {
                From = new MailAddress(_fromEmail, _fromName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };
            message.To.Add(to);

            await client.SendMailAsync(message, cancellationToken);
            _logger.LogInformation("Email sent successfully to {Email}", to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}", to);
            throw;
        }
    }
}
