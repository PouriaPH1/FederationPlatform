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
    public class NotificationServiceTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<INotificationRepository> _mockNotificationRepository;
        private readonly Mock<IEmailService> _mockEmailService;
        private readonly NotificationService _notificationService;

        public NotificationServiceTests()
        {
            _fixture = new Fixture();
            _mockNotificationRepository = new Mock<INotificationRepository>();
            _mockEmailService = new Mock<IEmailService>();
            _notificationService = new NotificationService(
                _mockNotificationRepository.Object,
                _mockEmailService.Object);
        }

        [Fact]
        public async Task CreateNotification_ValidData_CreatesSuccessfully()
        {
            // Arrange
            var userId = _fixture.Create<int>();
            var message = "Your activity has been approved";

            _mockNotificationRepository.Setup(x => x.AddAsync(It.IsAny<Notification>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _notificationService.CreateNotificationAsync(userId, message);

            // Assert
            result.Success.Should().BeTrue();
            _mockNotificationRepository.Verify(x => x.AddAsync(It.IsAny<Notification>()), Times.Once);
        }

        [Fact]
        public async Task GetUserNotifications_ValidUserId_ReturnsNotifications()
        {
            // Arrange
            var userId = _fixture.Create<int>();
            var notifications = _fixture.Build<Notification>()
                .With(x => x.UserId, userId)
                .CreateMany(5)
                .ToList();

            _mockNotificationRepository.Setup(x => x.GetByUserIdAsync(userId))
                .ReturnsAsync(notifications);

            // Act
            var result = await _notificationService.GetUserNotificationsAsync(userId);

            // Assert
            result.Should().HaveCount(5);
        }

        [Fact]
        public async Task MarkAsRead_ValidId_UpdatesNotification()
        {
            // Arrange
            var notificationId = _fixture.Create<int>();
            var notification = _fixture.Build<Notification>()
                .With(x => x.Id, notificationId)
                .With(x => x.IsRead, false)
                .Create();

            _mockNotificationRepository.Setup(x => x.GetByIdAsync(notificationId))
                .ReturnsAsync(notification);
            _mockNotificationRepository.Setup(x => x.UpdateAsync(It.IsAny<Notification>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _notificationService.MarkAsReadAsync(notificationId);

            // Assert
            result.Success.Should().BeTrue();
            notification.IsRead.Should().BeTrue();
        }

        [Fact]
        public async Task GetUnreadNotifications_ReturnsOnlyUnread()
        {
            // Arrange
            var userId = _fixture.Create<int>();
            var notifications = new List<Notification>
            {
                _fixture.Build<Notification>()
                    .With(x => x.UserId, userId)
                    .With(x => x.IsRead, false)
                    .Create(),
                _fixture.Build<Notification>()
                    .With(x => x.UserId, userId)
                    .With(x => x.IsRead, true)
                    .Create(),
                _fixture.Build<Notification>()
                    .With(x => x.UserId, userId)
                    .With(x => x.IsRead, false)
                    .Create()
            };

            _mockNotificationRepository.Setup(x => x.GetByUserIdAsync(userId))
                .ReturnsAsync(notifications);

            // Act
            var result = await _notificationService.GetUnreadNotificationsAsync(userId);

            // Assert
            result.Should().HaveCount(2);
            result.Should().AllSatisfy(x => x.IsRead == false);
        }

        [Fact]
        public async Task DeleteNotification_ValidId_RemovesSuccessfully()
        {
            // Arrange
            var notificationId = _fixture.Create<int>();
            var notification = _fixture.Build<Notification>()
                .With(x => x.Id, notificationId)
                .Create();

            _mockNotificationRepository.Setup(x => x.GetByIdAsync(notificationId))
                .ReturnsAsync(notification);
            _mockNotificationRepository.Setup(x => x.DeleteAsync(notification))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _notificationService.DeleteNotificationAsync(notificationId);

            // Assert
            result.Success.Should().BeTrue();
            _mockNotificationRepository.Verify(x => x.DeleteAsync(notification), Times.Once);
        }

        [Fact]
        public async Task SendEmailNotification_ValidData_SendsEmail()
        {
            // Arrange
            var userId = _fixture.Create<int>();
            var email = "test@example.com";
            var message = "Important notification";

            _mockEmailService.Setup(x => x.SendNotificationEmailAsync(email, message))
                .Returns(Task.CompletedTask);

            // Act
            await _notificationService.SendEmailNotificationAsync(email, message);

            // Assert
            _mockEmailService.Verify(
                x => x.SendNotificationEmailAsync(email, message),
                Times.Once);
        }
    }
}
