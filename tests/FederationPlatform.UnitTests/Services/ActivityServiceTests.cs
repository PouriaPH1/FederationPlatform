using Xunit;
using Moq;
using AutoFixture;
using FluentAssertions;
using FederationPlatform.Application.Services;
using FederationPlatform.Application.DTOs;
using FederationPlatform.Domain.Entities;
using FederationPlatform.Application.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace FederationPlatform.UnitTests.Services
{
    public class ActivityServiceTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IActivityRepository> _mockActivityRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<INotificationService> _mockNotificationService;
        private readonly ActivityService _activityService;

        public ActivityServiceTests()
        {
            _fixture = new Fixture();
            _mockActivityRepository = new Mock<IActivityRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockNotificationService = new Mock<INotificationService>();
            _activityService = new ActivityService(
                _mockActivityRepository.Object,
                _mockUserRepository.Object,
                _mockNotificationService.Object);
        }

        [Fact]
        public async Task CreateActivity_ValidData_ReturnsActivityId()
        {
            // Arrange
            var createDto = _fixture.Create<CreateActivityDto>();
            var activity = _fixture.Build<Activity>()
                .With(x => x.Title, createDto.Title)
                .Create();

            _mockActivityRepository.Setup(x => x.AddAsync(It.IsAny<Activity>()))
                .Returns(Task.CompletedTask);
            _mockActivityRepository.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _activityService.CreateActivityAsync(createDto);

            // Assert
            result.Should().BeGreaterThan(0);
            _mockActivityRepository.Verify(x => x.AddAsync(It.IsAny<Activity>()), Times.Once);
        }

        [Fact]
        public async Task CreateActivity_InvalidData_ThrowsException()
        {
            // Arrange
            var createDto = new CreateActivityDto { Title = "" }; // Invalid

            // Act
            Func<Task> act = async () => await _activityService.CreateActivityAsync(createDto);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task GetActivityById_ExistingId_ReturnsActivity()
        {
            // Arrange
            var activityId = _fixture.Create<int>();
            var activity = _fixture.Build<Activity>()
                .With(x => x.Id, activityId)
                .Create();

            _mockActivityRepository.Setup(x => x.GetByIdAsync(activityId))
                .ReturnsAsync(activity);

            // Act
            var result = await _activityService.GetActivityByIdAsync(activityId);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(activityId);
        }

        [Fact]
        public async Task GetActivityById_NonExistentId_ReturnsNull()
        {
            // Arrange
            var activityId = _fixture.Create<int>();
            _mockActivityRepository.Setup(x => x.GetByIdAsync(activityId))
                .ReturnsAsync((Activity)null);

            // Act
            var result = await _activityService.GetActivityByIdAsync(activityId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task ApproveActivity_ValidId_UpdatesStatus()
        {
            // Arrange
            var activityId = _fixture.Create<int>();
            var activity = _fixture.Build<Activity>()
                .With(x => x.Id, activityId)
                .With(x => x.Status, ActivityStatus.Pending)
                .Create();

            _mockActivityRepository.Setup(x => x.GetByIdAsync(activityId))
                .ReturnsAsync(activity);
            _mockActivityRepository.Setup(x => x.UpdateAsync(It.IsAny<Activity>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _activityService.ApproveActivityAsync(activityId);

            // Assert
            result.Success.Should().BeTrue();
            _mockActivityRepository.Verify(x => x.UpdateAsync(It.IsAny<Activity>()), Times.Once);
            _mockNotificationService.Verify(
                x => x.CreateNotificationAsync(It.IsAny<int>(), It.IsAny<string>()),
                Times.Once);
        }

        [Fact]
        public async Task RejectActivity_ValidId_UpdatesStatus()
        {
            // Arrange
            var activityId = _fixture.Create<int>();
            var rejectionReason = "Does not meet criteria";
            var activity = _fixture.Build<Activity>()
                .With(x => x.Id, activityId)
                .With(x => x.Status, ActivityStatus.Pending)
                .Create();

            _mockActivityRepository.Setup(x => x.GetByIdAsync(activityId))
                .ReturnsAsync(activity);
            _mockActivityRepository.Setup(x => x.UpdateAsync(It.IsAny<Activity>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _activityService.RejectActivityAsync(activityId, rejectionReason);

            // Assert
            result.Success.Should().BeTrue();
            _mockActivityRepository.Verify(x => x.UpdateAsync(It.IsAny<Activity>()), Times.Once);
        }

        [Fact]
        public async Task GetPendingActivities_ReturnsOnlyPending()
        {
            // Arrange
            var activities = new List<Activity>
            {
                _fixture.Build<Activity>()
                    .With(x => x.Status, ActivityStatus.Pending)
                    .Create(),
                _fixture.Build<Activity>()
                    .With(x => x.Status, ActivityStatus.Approved)
                    .Create(),
                _fixture.Build<Activity>()
                    .With(x => x.Status, ActivityStatus.Pending)
                    .Create()
            };

            _mockActivityRepository.Setup(x => x.GetAllAsync())
                .ReturnsAsync(activities);

            // Act
            var result = await _activityService.GetPendingActivitiesAsync();

            // Assert
            result.Should().HaveCount(2);
            result.Should().AllSatisfy(x => x.Status == ActivityStatus.Pending);
        }

        [Fact]
        public async Task DeleteActivity_ValidId_RemovesActivity()
        {
            // Arrange
            var activityId = _fixture.Create<int>();
            var activity = _fixture.Build<Activity>()
                .With(x => x.Id, activityId)
                .Create();

            _mockActivityRepository.Setup(x => x.GetByIdAsync(activityId))
                .ReturnsAsync(activity);
            _mockActivityRepository.Setup(x => x.DeleteAsync(activity))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _activityService.DeleteActivityAsync(activityId);

            // Assert
            result.Success.Should().BeTrue();
            _mockActivityRepository.Verify(x => x.DeleteAsync(activity), Times.Once);
        }

        [Fact]
        public async Task UpdateActivity_ValidData_UpdatesSuccessfully()
        {
            // Arrange
            var activityId = _fixture.Create<int>();
            var updateDto = _fixture.Create<UpdateActivityDto>();
            var activity = _fixture.Build<Activity>()
                .With(x => x.Id, activityId)
                .Create();

            _mockActivityRepository.Setup(x => x.GetByIdAsync(activityId))
                .ReturnsAsync(activity);
            _mockActivityRepository.Setup(x => x.UpdateAsync(It.IsAny<Activity>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _activityService.UpdateActivityAsync(activityId, updateDto);

            // Assert
            result.Success.Should().BeTrue();
            _mockActivityRepository.Verify(x => x.UpdateAsync(It.IsAny<Activity>()), Times.Once);
        }
    }
}
