namespace FederationPlatform.Application.Services;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body, bool isHtml = true);
    Task SendActivityApprovalEmailAsync(string toEmail, string userName, string activityTitle);
    Task SendActivityRejectionEmailAsync(string toEmail, string userName, string activityTitle, string? reason = null);
    Task SendNewActivityNotificationEmailAsync(string toEmail, string adminName, string activityTitle, string representativeName);
    Task SendWelcomeEmailAsync(string toEmail, string userName);
    Task SendPasswordResetEmailAsync(string toEmail, string userName, string resetLink);
    Task SendBulkEmailAsync(List<string> recipients, string subject, string body, bool isHtml = true);
}
