using Xunit;
using Moq;
using AutoFixture;
using FluentAssertions;
using FederationPlatform.Application.Services;
using FederationPlatform.Application.DTOs;
using FederationPlatform.Domain.Entities;
using FederationPlatform.Application.Repositories;

namespace FederationPlatform.UnitTests.Services
{
    public class AuthServiceTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IEmailService> _mockEmailService;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _fixture = new Fixture();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockEmailService = new Mock<IEmailService>();
            _authService = new AuthService(_mockUserRepository.Object, _mockEmailService.Object);
        }

        [Fact]
        public async Task Register_ValidData_ReturnsSuccess()
        {
            // Arrange
            var registerDto = new CreateUserDto
            {
                Username = "testuser",
                Email = "test@example.com",
                Password = "Password123!",
                FirstName = "Test",
                LastName = "User"
            };

            _mockUserRepository.Setup(x => x.GetByEmailAsync(registerDto.Email))
                .ReturnsAsync((User)null);

            // Act
            var result = await _authService.RegisterAsync(registerDto);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            _mockUserRepository.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task Register_DuplicateEmail_ReturnsFalse()
        {
            // Arrange
            var registerDto = new CreateUserDto
            {
                Username = "testuser",
                Email = "existing@example.com",
                Password = "Password123!"
            };

            var existingUser = _fixture.Create<User>();
            _mockUserRepository.Setup(x => x.GetByEmailAsync(registerDto.Email))
                .ReturnsAsync(existingUser);

            // Act
            var result = await _authService.RegisterAsync(registerDto);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("already exists");
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsToken()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "test@example.com",
                Password = "Password123!"
            };

            var user = _fixture.Build<User>()
                .With(x => x.Email, loginDto.Email)
                .Create();

            _mockUserRepository.Setup(x => x.GetByEmailAsync(loginDto.Email))
                .ReturnsAsync(user);

            // Act
            var result = await _authService.LoginAsync(loginDto);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeEmpty();
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsFalse()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "nonexistent@example.com",
                Password = "Password123!"
            };

            _mockUserRepository.Setup(x => x.GetByEmailAsync(loginDto.Email))
                .ReturnsAsync((User)null);

            // Act
            var result = await _authService.LoginAsync(loginDto);

            // Assert
            result.Success.Should().BeFalse();
        }

        [Fact]
        public async Task ChangePassword_ValidData_ReturnsSuccess()
        {
            // Arrange
            var userId = _fixture.Create<int>();
            var changePasswordDto = new ChangePasswordDto
            {
                CurrentPassword = "OldPassword123!",
                NewPassword = "NewPassword123!"
            };

            var user = _fixture.Build<User>()
                .With(x => x.Id, userId)
                .Create();

            _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync(user);

            // Act
            var result = await _authService.ChangePasswordAsync(userId, changePasswordDto);

            // Assert
            result.Success.Should().BeTrue();
        }

        [Fact]
        public async Task ForgotPassword_ValidEmail_SendsResetLink()
        {
            // Arrange
            var email = "test@example.com";
            var user = _fixture.Build<User>()
                .With(x => x.Email, email)
                .Create();

            _mockUserRepository.Setup(x => x.GetByEmailAsync(email))
                .ReturnsAsync(user);

            // Act
            var result = await _authService.ForgotPasswordAsync(email);

            // Assert
            result.Success.Should().BeTrue();
            _mockEmailService.Verify(
                x => x.SendPasswordResetEmailAsync(email, It.IsAny<string>()), 
                Times.Once);
        }

        [Fact]
        public async Task ResetPassword_ValidToken_ReturnsSuccess()
        {
            // Arrange
            var resetDto = new ResetPasswordDto
            {
                Email = "test@example.com",
                Token = "valid-token",
                NewPassword = "NewPassword123!"
            };

            var user = _fixture.Build<User>()
                .With(x => x.Email, resetDto.Email)
                .Create();

            _mockUserRepository.Setup(x => x.GetByEmailAsync(resetDto.Email))
                .ReturnsAsync(user);

            // Act
            var result = await _authService.ResetPasswordAsync(resetDto);

            // Assert
            result.Success.Should().BeTrue();
        }
    }
}
