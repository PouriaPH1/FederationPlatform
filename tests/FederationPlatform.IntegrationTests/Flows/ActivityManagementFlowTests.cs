using Xunit;
using FluentAssertions;
using FederationPlatform.Application.DTOs;
using FederationPlatform.Domain.Entities;

namespace FederationPlatform.IntegrationTests.Flows
{
    public class ActivityManagementFlowTests : IntegrationTestBase
    {
        [Fact]
        public async Task CompleteActivityLifecycle_ActivityProgresses()
        {
            // Arrange
            var representative = await CreateTestRepresentativeAsync("activity@example.com");
            var university = await CreateTestUniversityAsync();

            var createDto = new CreateActivityDto
            {
                Title = "Integration Test Activity",
                Description = "Testing the complete activity workflow",
                ActivityType = ActivityType.Event,
                UniversityId = university.Id,
                UserId = representative.Id,
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(2)
            };

            // Act - Create activity
            var createResult = await _activityService.CreateActivityAsync(createDto);
            createResult.Should().BeGreaterThan(0);

            // Assert - Activity created as pending
            var activity = await _activityService.GetActivityByIdAsync(createResult);
            activity.Should().NotBeNull();
            activity.Status.Should().Be(ActivityStatus.Pending);

            // Act - Approve activity
            var approveResult = await _activityService.ApproveActivityAsync(activity.Id);
            approveResult.Success.Should().BeTrue();

            // Assert - Activity approved
            var approvedActivity = await _activityService.GetActivityByIdAsync(activity.Id);
            approvedActivity.Status.Should().Be(ActivityStatus.Approved);

            // Act - Get approved activities
            var approvedActivities = await _activityService.GetApprovedActivitiesAsync();

            // Assert
            approvedActivities.Should().Contain(x => x.Id == activity.Id);
        }

        [Fact]
        public async Task ActivityRejectionFlow_ActivityCanBeRejected()
        {
            // Arrange
            var representative = await CreateTestRepresentativeAsync("reject@example.com");
            var university = await CreateTestUniversityAsync();

            var createDto = new CreateActivityDto
            {
                Title = "Activity to Reject",
                Description = "This activity will be rejected",
                ActivityType = ActivityType.Workshop,
                UniversityId = university.Id,
                UserId = representative.Id,
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(2)
            };

            var activityId = await _activityService.CreateActivityAsync(createDto);

            // Act - Reject activity
            var rejectReason = "Insufficient details";
            var rejectResult = await _activityService.RejectActivityAsync(activityId, rejectReason);

            // Assert
            rejectResult.Success.Should().BeTrue();

            var activity = await _activityService.GetActivityByIdAsync(activityId);
            activity.Status.Should().Be(ActivityStatus.Rejected);
        }

        [Fact]
        public async Task ActivityUpdateFlow_CanUpdatePendingActivity()
        {
            // Arrange
            var representative = await CreateTestRepresentativeAsync("update@example.com");
            var university = await CreateTestUniversityAsync();

            var createDto = new CreateActivityDto
            {
                Title = "Original Title",
                Description = "Original Description",
                ActivityType = ActivityType.Event,
                UniversityId = university.Id,
                UserId = representative.Id,
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(2)
            };

            var activityId = await _activityService.CreateActivityAsync(createDto);

            // Act - Update activity
            var updateDto = new UpdateActivityDto
            {
                Title = "Updated Title",
                Description = "Updated Description",
                StartDate = DateTime.UtcNow.AddDays(3),
                EndDate = DateTime.UtcNow.AddDays(4)
            };

            var updateResult = await _activityService.UpdateActivityAsync(activityId, updateDto);

            // Assert
            updateResult.Success.Should().BeTrue();

            var activity = await _activityService.GetActivityByIdAsync(activityId);
            activity.Title.Should().Be("Updated Title");
            activity.Description.Should().Be("Updated Description");
        }

        [Fact]
        public async Task ActivityDeleteFlow_CanDeletePendingActivity()
        {
            // Arrange
            var representative = await CreateTestRepresentativeAsync("delete@example.com");
            var university = await CreateTestUniversityAsync();

            var createDto = new CreateActivityDto
            {
                Title = "Activity to Delete",
                Description = "This will be deleted",
                ActivityType = ActivityType.Event,
                UniversityId = university.Id,
                UserId = representative.Id,
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(2)
            };

            var activityId = await _activityService.CreateActivityAsync(createDto);

            // Act - Delete activity
            var deleteResult = await _activityService.DeleteActivityAsync(activityId);

            // Assert
            deleteResult.Success.Should().BeTrue();

            var deletedActivity = await _activityService.GetActivityByIdAsync(activityId);
            deletedActivity.Should().BeNull();
        }

        [Fact]
        public async Task ActivityFileUpload_FilesPersistsWithActivity()
        {
            // Arrange
            var representative = await CreateTestRepresentativeAsync("files@example.com");
            var university = await CreateTestUniversityAsync();

            var createDto = new CreateActivityDto
            {
                Title = "Activity with Files",
                Description = "Activity that includes files",
                ActivityType = ActivityType.Event,
                UniversityId = university.Id,
                UserId = representative.Id,
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(2)
            };

            var activityId = await _activityService.CreateActivityAsync(createDto);

            // Act - Upload file
            var fileContent = "Test file content";
            var fileName = "test-file.pdf";
            var uploadResult = await _activityService.UploadActivityFileAsync(activityId, fileName, fileContent);

            // Assert
            uploadResult.Success.Should().BeTrue();

            var activity = await _activityService.GetActivityByIdAsync(activityId);
            activity.Files.Should().HaveCount(1);
            activity.Files.First().FileName.Should().Be(fileName);
        }

        [Fact]
        public async Task PendingActivitiesNotification_AdminReceivesNotification()
        {
            // Arrange
            var representative = await CreateTestRepresentativeAsync("notify@example.com");
            var admin = await CreateTestAdminAsync("admin@example.com");
            var university = await CreateTestUniversityAsync();

            var createDto = new CreateActivityDto
            {
                Title = "Activity Needing Review",
                Description = "This should trigger a notification",
                ActivityType = ActivityType.Event,
                UniversityId = university.Id,
                UserId = representative.Id,
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(2)
            };

            var activityId = await _activityService.CreateActivityAsync(createDto);

            // Act - Notify admin
            var notifyResult = await _notificationService.NotifyPendingActivitiesAsync(admin.Id);

            // Assert
            notifyResult.Success.Should().BeTrue();
        }
    }
}
