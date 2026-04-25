using System.Text.RegularExpressions;

namespace FederationPlatform.Web.Helpers;

public static class SecurityHelper
{
    // XSS Prevention - Sanitize HTML input
    public static string SanitizeHtml(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        // Remove potentially dangerous tags
        var dangerousTags = new[] { "script", "iframe", "object", "embed", "link", "style" };
        foreach (var tag in dangerousTags)
        {
            input = Regex.Replace(input, $"<{tag}[^>]*>.*?</{tag}>", string.Empty, RegexOptions.IgnoreCase);
            input = Regex.Replace(input, $"<{tag}[^>]*/>", string.Empty, RegexOptions.IgnoreCase);
        }

        // Remove event handlers
        input = Regex.Replace(input, @"on\w+\s*=\s*[""'][^""']*[""']", string.Empty, RegexOptions.IgnoreCase);
        input = Regex.Replace(input, @"javascript:", string.Empty, RegexOptions.IgnoreCase);

        return input;
    }

    // SQL Injection Prevention - Validate input
    public static bool ContainsSqlInjection(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return false;

        var sqlKeywords = new[] { "DROP", "DELETE", "INSERT", "UPDATE", "EXEC", "EXECUTE", "SCRIPT", "--", "/*", "*/" };
        return sqlKeywords.Any(keyword => input.ToUpper().Contains(keyword));
    }

    // Path Traversal Prevention
    public static bool IsValidFileName(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return false;

        // Check for path traversal attempts
        if (fileName.Contains("..") || fileName.Contains("/") || fileName.Contains("\\"))
            return false;

        // Check for invalid characters
        var invalidChars = Path.GetInvalidFileNameChars();
        return !fileName.Any(c => invalidChars.Contains(c));
    }

    // Validate file extension
    public static bool IsAllowedFileExtension(string fileName, string[] allowedExtensions)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return false;

        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return allowedExtensions.Contains(extension);
    }

    // Validate file size
    public static bool IsValidFileSize(long fileSize, long maxSizeInBytes)
    {
        return fileSize > 0 && fileSize <= maxSizeInBytes;
    }

    // Generate secure random token
    public static string GenerateSecureToken(int length = 32)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    // Validate email format
    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    // Validate Persian phone number
    public static bool IsValidPersianPhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return false;

        // Iranian phone number format: 09xxxxxxxxx
        var pattern = @"^09\d{9}$";
        return Regex.IsMatch(phoneNumber, pattern);
    }

    // Encode output to prevent XSS
    public static string HtmlEncode(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        return System.Net.WebUtility.HtmlEncode(input);
    }

    // Decode HTML entities
    public static string HtmlDecode(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        return System.Net.WebUtility.HtmlDecode(input);
    }
}
