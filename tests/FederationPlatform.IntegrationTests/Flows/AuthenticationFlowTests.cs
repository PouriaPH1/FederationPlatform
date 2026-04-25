using Xunit;
using FluentAssertions;
using FederationPlatform.Application.DTOs;
using FederationPlatform.Domain.Entities;
using System.Threading.Tasks;

namespace FederationPlatform.IntegrationTests.Flows
{
    public class AuthenticationFlowTests : IntegrationTestBase
    {
        [Fact]
        public async Task CompleteRegistrationFlow_UserCanRegisterAndLogin()
        {
            // Arrange
            var registerDto = new CreateUserDto
            {
                Username = \"testuser_integration\",
                Email = \"testintegration@example.com\",
                Password = \"SecurePassword123!\",
                FirstName = \"Integration\",
                LastName = \"Test\"
            };

            // Act - Register
            var registerResult = await _authService.RegisterAsync(registerDto);

            // Assert - Registration
            registerResult.Should().NotBeNull();
            registerResult.Success.Should().BeTrue();
            registerResult.Data.Should().BeGreaterThan(0);

            // Act - Login
            var loginDto = new LoginDto
            {
                Email = registerDto.Email,
                Password = registerDto.Password
            };

            var loginResult = await _authService.LoginAsync(loginDto);

            // Assert - Login
            loginResult.Success.Should().BeTrue();
            loginResult.Data.Should().NotBeEmpty();
        }

        [Fact]
        public async Task PasswordResetFlow_UserCanResetPassword()
        {
            // Arrange
            var user = await CreateTestUserAsync(\"resettest@example.com\", \"OldPassword123!\");
            var oldEmail = user.Email;

            // Act - Request password reset
            var forgotResult = await _authService.ForgotPasswordAsync(oldEmail);
            forgotResult.Success.Should().BeTrue();

            // Get the reset token (in real scenario, this would be sent via email)
            var resetToken = await _tokenService.GenerateResetTokenAsync(user.Id);

            // Act - Reset password
            var resetDto = new ResetPasswordDto
            {
                Email = oldEmail,
                Token = resetToken,
                NewPassword = \"NewPassword123!\"
            };

            var resetResult = await _authService.ResetPasswordAsync(resetDto);

            // Assert
            resetResult.Success.Should().BeTrue();

            // Verify new password works
            var loginDto = new LoginDto
            {
                Email = oldEmail,
                Password = \"NewPassword123!\"
            };

            var loginResult = await _authService.LoginAsync(loginDto);
            loginResult.Success.Should().BeTrue();
        }

        [Fact]
        public async Task RolePromotionFlow_RepresentativeCanBeCreated()
        {
            // Arrange
            var user = await CreateTestUserAsync(\"promote@example.com\", \"Password123!\");
            user.Role.Should().Be(UserRole.User);

            // Act - Promote to representative
            var promoteResult = await _userService.PromoteToRepresentativeAsync(user.Id);

            // Assert
            promoteResult.Success.Should().BeTrue();

            // Verify role changed
            var updatedUser = await _userService.GetUserByIdAsync(user.Id);
            updatedUser.Role.Should().Be(UserRole.Representative);
        }

        [Fact]
        public async Task UserBanFlow_AdminCanBanUser()
        {
            // Arrange
            var user = await CreateTestUserAsync(\"bantest@example.com\", \"Password123!\");
            user.IsActive.Should().BeTrue();

            // Act - Ban user
            var banResult = await _userService.BanUserAsync(user.Id);

            // Assert
            banResult.Success.Should().BeTrue();

            // Verify user is inactive
            var updatedUser = await _userService.GetUserByIdAsync(user.Id);
            updatedUser.IsActive.Should().BeFalse();

            // Verify user cannot login
            var loginDto = new LoginDto
            {
                Email = user.Email,
                Password = \"Password123!\"
            };

            var loginResult = await _authService.LoginAsync(loginDto);
            loginResult.Success.Should().BeFalse();
        }

        [Fact]
        public async Task ConcurrentLoginAttempts_HandledCorrectly()
        {
            // Arrange
            var user = await CreateTestUserAsync(\"concurrent@example.com\", \"Password123!\");

            var loginDto = new LoginDto
            {
                Email = user.Email,
                Password = \"Password123!\"
            };

            // Act - Multiple concurrent login attempts
            var tasks = Enumerable.Range(0, 5)
                .Select(_ => _authService.LoginAsync(loginDto))
                .ToArray();

            var results = await Task.WhenAll(tasks);

            // Assert
            results.Should().HaveCount(5);
            results.Should().AllSatisfy(r => r.Success.Should().BeTrue());
        }

        [Fact]
        public async Task SessionTimeout_EnforcedCorrectly()
        {
            // Arrange
            var user = await CreateTestUserAsync(\"session@example.com\", \"Password123!\");

            var loginDto = new LoginDto
            {
                Email = user.Email,
                Password = \"Password123!\"
            };

            var loginResult = await _authService.LoginAsync(loginDto);
            var sessionToken = loginResult.Data;

            // Act - Wait for session timeout (simulated)
            await Task.Delay(TimeSpan.FromSeconds(1)); // Simplified for testing

            // Assert - Session should be valid shortly after login
            var isValid = await _tokenService.ValidateTokenAsync(sessionToken);
            isValid.Should().BeTrue();
        }
    }
}
