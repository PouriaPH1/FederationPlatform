using Xunit;
using Moq;
using AutoFixture;
using FluentAssertions;
using FederationPlatform.Application.Services;
using FederationPlatform.Application.DTOs;
using FederationPlatform.Domain.Entities;
using FederationPlatform.Application.Repositories;
using System.Collections.Generic;

namespace FederationPlatform.UnitTests.Services
{
    public class UserServiceTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IUserProfileRepository> _mockUserProfileRepository;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _fixture = new Fixture();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockUserProfileRepository = new Mock<IUserProfileRepository>();
            _userService = new UserService(
                _mockUserRepository.Object,
                _mockUserProfileRepository.Object);
        }

        [Fact]
        public async Task GetAllUsers_ReturnsAllUsers()
        {
            // Arrange
            var users = _fixture.CreateMany<User>(5).ToList();
            _mockUserRepository.Setup(x => x.GetAllAsync())
                .ReturnsAsync(users);

            // Act
            var result = await _userService.GetAllUsersAsync();

            // Assert
            result.Should().HaveCount(5);
        }

        [Fact]
        public async Task GetUserById_ValidId_ReturnsUser()
        {
            // Arrange
            var userId = _fixture.Create<int>();
            var user = _fixture.Build<User>()
                .With(x => x.Id, userId)
                .Create();

            _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.GetUserByIdAsync(userId);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(userId);
        }

        [Fact]
        public async Task BanUser_ValidId_UpdatesActiveStatus()
        {
            // Arrange
            var userId = _fixture.Create<int>();
            var user = _fixture.Build<User>()
                .With(x => x.Id, userId)
                .With(x => x.IsActive, true)
                .Create();

            _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync(user);
            _mockUserRepository.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _userService.BanUserAsync(userId);

            // Assert
            result.Success.Should().BeTrue();
            user.IsActive.Should().BeFalse();
        }

        [Fact]
        public async Task PromoteToRepresentative_ValidId_UpdatesRole()
        {
            // Arrange
            var userId = _fixture.Create<int>();
            var user = _fixture.Build<User>()
                .With(x => x.Id, userId)
                .With(x => x.Role, UserRole.User)
                .Create();

            _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync(user);
            _mockUserRepository.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _userService.PromoteToRepresentativeAsync(userId);

            // Assert
            result.Success.Should().BeTrue();
            user.Role.Should().Be(UserRole.Representative);
        }

        [Fact]
        public async Task UpdateUser_ValidData_UpdatesSuccessfully()
        {
            // Arrange
            var userId = _fixture.Create<int>();
            var updateDto = _fixture.Create<UpdateUserDto>();
            var user = _fixture.Build<User>()
                .With(x => x.Id, userId)
                .Create();

            _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync(user);
            _mockUserRepository.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _userService.UpdateUserAsync(userId, updateDto);

            // Assert
            result.Success.Should().BeTrue();
        }

        [Fact]
        public async Task GetUsersByRole_ValidRole_ReturnsUsers()
        {
            // Arrange
            var users = _fixture.Build<User>()
                .With(x => x.Role, UserRole.Representative)
                .CreateMany(3)
                .ToList();

            _mockUserRepository.Setup(x => x.GetByRoleAsync(UserRole.Representative))
                .ReturnsAsync(users);

            // Act
            var result = await _userService.GetUsersByRoleAsync(UserRole.Representative);

            // Assert
            result.Should().HaveCount(3);
            result.Should().AllSatisfy(x => x.Role == UserRole.Representative);
        }

        [Fact]
        public async Task GetActiveUsers_ReturnsOnlyActive()
        {
            // Arrange
            var users = new List<User>
            {
                _fixture.Build<User>().With(x => x.IsActive, true).Create(),
                _fixture.Build<User>().With(x => x.IsActive, false).Create(),
                _fixture.Build<User>().With(x => x.IsActive, true).Create()
            };

            _mockUserRepository.Setup(x => x.GetAllAsync())
                .ReturnsAsync(users);

            // Act
            var result = await _userService.GetActiveUsersAsync();

            // Assert
            result.Should().HaveCount(2);
            result.Should().AllSatisfy(x => x.IsActive == true);
        }
    }
}
