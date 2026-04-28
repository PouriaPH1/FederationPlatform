using FederationPlatform.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;

namespace FederationPlatform.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly bool _enabled;
    private readonly string _smtpHost;
    private readonly int _smtpPort;
    private readonly string _smtpUsername;
    private readonly string _smtpPassword;
    private readonly string _fromEmail;
    private readonly string _fromName;
    private readonly bool _enableSsl;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _logger = logger;

        _enabled = bool.TryParse(configuration["Email:Enabled"], out var enabled) ? enabled : true;
        _smtpHost = configuration["Email:SmtpHost"] ?? "smtp.gmail.com";
        _smtpPort = int.TryParse(configuration["Email:SmtpPort"], out var smtpPort) ? smtpPort : 587;
        _smtpUsername = configuration["Email:SmtpUsername"] ?? "";
        _smtpPassword = configuration["Email:SmtpPassword"] ?? "";
        _fromEmail = configuration["Email:FromEmail"] ?? "noreply@federation.ir";
        _fromName = configuration["Email:FromName"] ?? "فدراسیون اقتصاد سلامت";
        _enableSsl = bool.TryParse(configuration["Email:EnableSsl"], out var enableSsl) ? enableSsl : true;
    }

    public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = true)
    {
        if (!_enabled)
        {
            _logger.LogInformation("Email sending is disabled by configuration. Skipping email to {Email}.", to);
            return;
        }

        try
        {
            using var client = new SmtpClient(_smtpHost, _smtpPort)
            {
                Credentials = new NetworkCredential(_smtpUsername, _smtpPassword),
                EnableSsl = _enableSsl
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_fromEmail, _fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };

            mailMessage.To.Add(to);

            await client.SendMailAsync(mailMessage);
            _logger.LogInformation($"Email sent successfully to {to}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to send email to {to}");
            throw;
        }
    }

    public async Task SendActivityApprovalEmailAsync(string toEmail, string userName, string activityTitle)
    {
        var subject = "✅ فعالیت شما تایید شد";
        var body = $@"
            <div dir='rtl' style='font-family: Tahoma, Arial; padding: 20px;'>
                <h2 style='color: #28a745;'>سلام {userName} عزیز</h2>
                <p>فعالیت شما با عنوان <strong>{activityTitle}</strong> توسط مدیر سیستم تایید شد.</p>
                <p>اکنون می‌توانید جزئیات فعالیت خود را در پنل کاربری مشاهده کنید.</p>
                <br>
                <p>با تشکر،</p>
                <p><strong>تیم فدراسیون اقتصاد سلامت</strong></p>
            </div>
        ";

        await SendEmailAsync(toEmail, subject, body, true);
    }

    public async Task SendActivityRejectionEmailAsync(string toEmail, string userName, string activityTitle, string? reason = null)
    {
        var subject = "❌ فعالیت شما رد شد";
        var reasonText = !string.IsNullOrEmpty(reason) ? $"<p><strong>دلیل رد:</strong> {reason}</p>" : "";
        
        var body = $@"
            <div dir='rtl' style='font-family: Tahoma, Arial; padding: 20px;'>
                <h2 style='color: #dc3545;'>سلام {userName} عزیز</h2>
                <p>متاسفانه فعالیت شما با عنوان <strong>{activityTitle}</strong> توسط مدیر سیستم رد شد.</p>
                {reasonText}
                <p>لطفاً اطلاعات فعالیت را بررسی و در صورت نیاز مجدداً ثبت کنید.</p>
                <br>
                <p>با تشکر،</p>
                <p><strong>تیم فدراسیون اقتصاد سلامت</strong></p>
            </div>
        ";

        await SendEmailAsync(toEmail, subject, body, true);
    }

    public async Task SendNewActivityNotificationEmailAsync(string toEmail, string adminName, string activityTitle, string representativeName)
    {
        var subject = "📝 فعالیت جدید برای بررسی";
        var body = $@"
            <div dir='rtl' style='font-family: Tahoma, Arial; padding: 20px;'>
                <h2 style='color: #007bff;'>سلام {adminName} عزیز</h2>
                <p>نماینده <strong>{representativeName}</strong> فعالیت جدیدی با عنوان <strong>{activityTitle}</strong> ثبت کرده است.</p>
                <p>لطفاً برای بررسی و تایید/رد این فعالیت به پنل مدیریت مراجعه کنید.</p>
                <br>
                <p>با تشکر،</p>
                <p><strong>سیستم فدراسیون اقتصاد سلامت</strong></p>
            </div>
        ";

        await SendEmailAsync(toEmail, subject, body, true);
    }

    public async Task SendWelcomeEmailAsync(string toEmail, string userName)
    {
        var subject = "🎉 خوش آمدید به فدراسیون اقتصاد سلامت";
        var body = $@"
            <div dir='rtl' style='font-family: Tahoma, Arial; padding: 20px;'>
                <h2 style='color: #28a745;'>سلام {userName} عزیز</h2>
                <p>به پلتفرم مدیریت فعالیت‌های کمیته دانشجویی فدراسیون اقتصاد سلامت خوش آمدید!</p>
                <p>اکنون می‌توانید فعالیت‌های خود را ثبت و مدیریت کنید.</p>
                <br>
                <p>در صورت نیاز به راهنمایی، با تیم پشتیبانی تماس بگیرید.</p>
                <br>
                <p>با تشکر،</p>
                <p><strong>تیم فدراسیون اقتصاد سلامت</strong></p>
            </div>
        ";

        await SendEmailAsync(toEmail, subject, body, true);
    }

    public async Task SendPasswordResetEmailAsync(string toEmail, string userName, string resetLink)
    {
        var subject = "🔐 بازیابی رمز عبور";
        var body = $@"
            <div dir='rtl' style='font-family: Tahoma, Arial; padding: 20px;'>
                <h2>سلام {userName} عزیز</h2>
                <p>درخواست بازیابی رمز عبور برای حساب کاربری شما دریافت شد.</p>
                <p>برای تنظیم رمز عبور جدید، روی لینک زیر کلیک کنید:</p>
                <p><a href='{resetLink}' style='color: #007bff; text-decoration: none;'>بازیابی رمز عبور</a></p>
                <p><small>این لینک تا 24 ساعت معتبر است.</small></p>
                <br>
                <p>اگر شما این درخواست را ارسال نکرده‌اید، این ایمیل را نادیده بگیرید.</p>
                <br>
                <p>با تشکر،</p>
                <p><strong>تیم فدراسیون اقتصاد سلامت</strong></p>
            </div>
        ";

        await SendEmailAsync(toEmail, subject, body, true);
    }

    public async Task SendBulkEmailAsync(List<string> recipients, string subject, string body, bool isHtml = true)
    {
        var tasks = recipients.Select(recipient => SendEmailAsync(recipient, subject, body, isHtml));
        await Task.WhenAll(tasks);
    }

    public async Task SendUserBannedEmailAsync(string toEmail, string userName, string reason)
    {
        var subject = "⚠️ حساب شما مسدود شد";
        var body = $@"
            <div dir='rtl' style='font-family: Tahoma, Arial; padding: 20px;'>
                <h2 style='color: #dc3545;'>سلام {userName} عزیز</h2>
                <p>متاسفانه حساب کاربری شما توسط مدیر سیستم مسدود شد.</p>
                <p><strong>دلیل مسدود کردن:</strong> {reason}</p>
                <br>
                <p>برای پرسش‌ها و درخواست بررسی مجدد با تیم پشتیبانی تماس بگیرید.</p>
                <br>
                <p>با تشکر،</p>
                <p><strong>تیم فدراسیون اقتصاد سلامت</strong></p>
            </div>
        ";

        await SendEmailAsync(toEmail, subject, body, true);
    }

    public async Task SendUserActivatedEmailAsync(string toEmail, string userName)
    {
        var subject = "✅ حساب شما فعال شد";
        var body = $@"
            <div dir='rtl' style='font-family: Tahoma, Arial; padding: 20px;'>
                <h2 style='color: #28a745;'>سلام {userName} عزیز</h2>
                <p>خوشحالیم که اطلاع دهیم حساب کاربری شما مجدداً فعال شد.</p>
                <p>اکنون می‌توانید وارد سیستم شده و کارهای خود را ادامه دهید.</p>
                <br>
                <p>با تشکر،</p>
                <p><strong>تیم فدراسیون اقتصاد سلامت</strong></p>
            </div>
        ";

        await SendEmailAsync(toEmail, subject, body, true);
    }

    public async Task SendUserPromotedEmailAsync(string toEmail, string userName, string newRole)
    {
        var subject = "🎉 ترفیع شغلی شما";
        var body = $@"
            <div dir='rtl' style='font-family: Tahoma, Arial; padding: 20px;'>
                <h2 style='color: #0EA5E9;'>سلام {userName} عزیز</h2>
                <p>مبارک باشد! شما به نقش <strong>{newRole}</strong> ترفیع یافتید.</p>
                <p>اکنون می‌توانید فعالیت‌های جدید ایجاد کرده و آن‌ها را برای تایید ثبت کنید.</p>
                <p>لطفاً تعلیمات و ضوابط سیستم را با دقت مطالعه کنید.</p>
                <br>
                <p>با تشکر و تبریک،</p>
                <p><strong>تیم فدراسیون اقتصاد سلامت</strong></p>
            </div>
        ";

        await SendEmailAsync(toEmail, subject, body, true);
    }
}
