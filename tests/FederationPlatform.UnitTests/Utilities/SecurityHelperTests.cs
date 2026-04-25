using Xunit;
using FluentAssertions;
using FederationPlatform.Infrastructure.Security;

namespace FederationPlatform.UnitTests.Utilities
{
    public class SecurityHelperTests
    {
        [Theory]
        [InlineData(\"<script>alert('xss')</script>hello\", \"hello\")]
        [InlineData(\"<img src=x onerror='alert(1)'>\", \"\")]
        [InlineData(\"normal text\", \"normal text\")]
        public void SanitizeHtml_RemovesScripts(string input, string expected)
        {
            // Act
            var result = SecurityHelper.SanitizeHtml(input);

            // Assert
            result.Should().NotContain(\"<script>\");
            result.Should().NotContain(\"onerror\");
        }

        [Theory]
        [InlineData(\"test@example.com\", true)]
        [InlineData(\"invalid.email\", false)]
        [InlineData(\"test@\", false)]
        [InlineData(\"@example.com\", false)]
        public void IsValidEmail_ValidatesEmail(string email, bool expected)
        {
            // Act
            var result = SecurityHelper.IsValidEmail(email);

            // Assert
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData(\"09123456789\", true)]
        [InlineData(\"09351234567\", true)]
        [InlineData(\"0912345\", false)]
        [InlineData(\"invalid\", false)]
        public void IsValidPersianPhone_ValidatesPhone(string phone, bool expected)
        {
            // Act
            var result = SecurityHelper.IsValidPersianPhone(phone);

            // Assert
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData(\"test.txt\", true)]
        [InlineData(\"document.pdf\", true)]
        [InlineData(\"image.jpg\", true)]
        [InlineData(\"script.exe\", false)]
        [InlineData(\"malware.bat\", false)]
        public void IsAllowedFileExtension_ChecksExtension(string filename, bool expected)
        {
            // Act
            var result = SecurityHelper.IsAllowedFileExtension(filename);

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void EncodeHtml_EncodesSpecialChars()
        {
            // Arrange
            var input = \"<script>alert('xss')</script>\";

            // Act
            var result = SecurityHelper.EncodeHtml(input);

            // Assert
            result.Should().Contain(\"&lt;\");
            result.Should().Contain(\"&gt;\");
            result.Should().NotContain(\"<\");
            result.Should().NotContain(\">\");
        }

        [Fact]
        public void DecodeHtml_DecodesSpecialChars()
        {
            // Arrange
            var input = \"&lt;div&gt;test&lt;/div&gt;\";

            // Act
            var result = SecurityHelper.DecodeHtml(input);

            // Assert
            result.Should().Contain(\"<\");
            result.Should().Contain(\">\");
            result.Should().Be(\"<div>test</div>\");
        }

        [Theory]
        [InlineData(\"'; DROP TABLE users; --\", true)]
        [InlineData(\"admin' or '1'='1\", true)]
        [InlineData(\"normal input\", false)]
        public void DetectSqlInjection_IdentifiesSuspiciousPatterns(string input, bool expected)
        {
            // Act
            var result = SecurityHelper.DetectSqlInjection(input);

            // Assert
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData(\"../../../etc/passwd\", true)]
        [InlineData(\"..\\..\\..\\windows\\system32\", true)]
        [InlineData(\"normal/path/file.txt\", false)]
        public void DetectPathTraversal_IdentifiesSuspiciousPaths(string path, bool expected)
        {
            // Act
            var result = SecurityHelper.DetectPathTraversal(path);

            // Assert
            result.Should().Be(expected);
        }
    }
}
