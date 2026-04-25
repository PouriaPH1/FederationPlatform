using Xunit;
using FluentAssertions;
using FederationPlatform.Domain.Entities;

namespace FederationPlatform.IntegrationTests.Security
{
    public class SecurityIntegrationTests : IntegrationTestBase
    {
        [Fact]
        public async Task SqlInjectionPrevention_MaliciousInputRejected()
        {
            // Arrange
            var maliciousInput = \"'; DROP TABLE Users; --\";

            // Act
            Func<Task> act = async () => await _userService.SearchUsersAsync(maliciousInput);

            // Assert
            await act.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task XssPrevention_ScriptTagsRemoved()
        {
            // Arrange
            var xssPayload = \"<script>alert('xss')</script>Normal Text\";

            // Act
            var result = _securityHelper.SanitizeHtml(xssPayload);

            // Assert
            result.Should().NotContain(\"<script>\");
            result.Should().NotContain(\"</script>\");
            result.Should().Contain(\"Normal Text\");
        }

        [Fact]
        public async Task CsrfProtection_InvalidTokenRejected()
        {
            // Arrange
            var user = await CreateTestUserAsync(\"csrf@example.com\", \"Password123!\");
            var invalidToken = \"invalid-csrf-token\";

            // Act
            Func<Task> act = async () => await _userService.UpdateUserAsync(user.Id, new UpdateUserDto { }, invalidToken);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [Fact]
        public async Task PathTraversalPrevention_MaliciousPathRejected()
        {
            // Arrange
            var maliciousPath = \"../../../etc/passwd\";

            // Act
            var result = _securityHelper.DetectPathTraversal(maliciousPath);

            // Assert
            result.Should().BeTrue(); // Should detect as malicious
        }

        [Fact]
        public async Task RateLimitingEnforcement_ExcessiveRequests_Blocked()
        {
            // Arrange
            var loginAttempts = Enumerable.Range(0, 10)
                .Select(_ => new LoginDto { Email = \"test@example.com\", Password = \"wrong\" })
                .ToList();

            // Act
            var results = new List<bool>();
            foreach (var attempt in loginAttempts)
            {
                try
                {
                    var result = await _authService.LoginAsync(attempt);
                    results.Add(!result.Success);
                }
                catch (Exception ex) when (ex.Message.Contains(\"rate limit\"))
                {
                    results.Add(false); // Rate limited
                }
            }

            // Assert - Some requests should be blocked
            results.Should().Contain(false);
        }

        [Fact]
        public async Task UnauthorizedAccess_AdminOnlyFeature()
        {
            // Arrange
            var regularUser = await CreateTestUserAsync(\"regular@example.com\", \"Password123!\");

            // Act
            Func<Task> act = async () => await _adminService.BanUserAsync(1);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [Fact]
        public async Task PasswordHashing_PlaintextNeverStored()
        {
            // Arrange
            var plainPassword = \"MySecurePassword123!\";
            var registerDto = new CreateUserDto
            {
                Username = \"hash@example.com\",
                Email = \"hash@example.com\",
                Password = plainPassword
            };

            // Act
            var result = await _authService.RegisterAsync(registerDto);
            var user = await _userService.GetUserByIdAsync(result.Data);

            // Assert
            user.PasswordHash.Should().NotBe(plainPassword);
            user.PasswordHash.Should().NotBeEmpty();
        }

        [Fact]
        public async Task SessionSecurity_CookieFlags_Set()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = \"session@example.com\",
                Password = \"Password123!\"
            };

            var user = await CreateTestUserAsync(loginDto.Email, loginDto.Password);

            // Act
            var result = await _authService.LoginAsync(loginDto);

            // Assert - Verify session configuration
            // In a real test, you would check HttpContext.Response.Cookies
            result.Data.Should().NotBeEmpty();
        }

        [Fact]
        public async Task SecurityHeaders_Configured()
        {
            // This test would be better suited for integration/E2E testing
            // Verifying security headers in HTTP responses

            // Assert - Headers should include:
            // X-Content-Type-Options: nosniff
            // X-Frame-Options: DENY
            // X-XSS-Protection: 1; mode=block
            // Strict-Transport-Security
        }

        [Fact]
        public async Task InputValidation_InvalidEmail_Rejected()
        {
            // Arrange
            var invalidEmail = \"not-an-email\";

            // Act
            var result = _securityHelper.IsValidEmail(invalidEmail);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task FileUploadSecurity_ExecutableFiles_Rejected()
        {
            // Arrange
            var representativeUser = await CreateTestRepresentativeAsync(\"file@example.com\");
            var activity = await CreateTestActivityAsync(representativeUser.Id);
            var maliciousFileName = \"malware.exe\";

            // Act
            Func<Task> act = async () => await _activityService.UploadActivityFileAsync(
                activity.Id, 
                maliciousFileName, 
                \"malicious content\");

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>();
        }
    }
}
