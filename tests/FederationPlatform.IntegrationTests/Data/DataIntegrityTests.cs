using Xunit;
using FluentAssertions;
using FederationPlatform.Domain.Entities;
using FederationPlatform.Application.DTOs;

namespace FederationPlatform.IntegrationTests.Data
{
    public class DataIntegrityTests : IntegrationTestBase
    {
        [Fact]
        public async Task ForeignKeyConstraint_OnActivityUniversity()
        {
            // Arrange
            var createDto = new CreateActivityDto 
            { 
                Title = "Test",
                UniversityId = 999,
                UserId = 1
            };

            // Act
            Func<Task> act = async () => await _activityService.CreateActivityAsync(createDto);

            // Assert
            await act.Should().ThrowAsync<Exception>(); // FK constraint violation
        }

        [Fact]
        public async Task CascadingDelete_ActivityFiles()
        {
            // Arrange
            var representative = await CreateTestRepresentativeAsync("cascade@example.com");
            var activity = await CreateTestActivityAsync(representative.Id);

            // Act - Delete activity
            await _activityService.DeleteActivityAsync(activity.Id);

            // Assert - Activity should be deleted
            var deletedActivity = await _activityService.GetActivityByIdAsync(activity.Id);
            deletedActivity.Should().BeNull();
        }

        [Fact]
        public async Task UniqueConstraint_UserEmail()
        {
            // Arrange
            var email = "unique@example.com";
            var user1 = new CreateUserDto
            {
                Username = "user1",
                Email = email,
                Password = "Password123!"
            };

            await _authService.RegisterAsync(user1);

            var user2 = new CreateUserDto
            {
                Username = "user2",
                Email = email,
                Password = "Password123!"
            };

            // Act
            Func<Task> act = async () => await _authService.RegisterAsync(user2);

            // Assert
            await act.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task ConcurrentDataModification_Handled()
        {
            // Arrange
            var university = await CreateTestUniversityAsync();
            var updateDto = new UpdateUniversityDto { Description = "Updated" };

            // Act - Concurrent updates
            var tasks = Enumerable.Range(0, 5)
                .Select(_ => _universityService.UpdateUniversityAsync(university.Id, updateDto))
                .ToArray();

            var results = await Task.WhenAll(tasks);

            // Assert
            results.Should().AllSatisfy(r => r.Success.Should().BeTrue());
        }

        [Fact]
        public async Task SessionConsistency_Ensured()
        {
            // Arrange
            var user = await CreateTestUserAsync("consistent@example.com", "Password123!");

            // Act - Create activity
            var activity = await CreateTestActivityAsync(user.Id);

            // Assert
            activity.UserId.Should().Be(user.Id);
            activity.Should().NotBeNull();
        }

        [Fact]
        public async Task InvalidStatusTransition_ThrowsException()
        {
            // Arrange
            var representative = await CreateTestRepresentativeAsync("orphan@example.com", "Password123!");
            var activity = await CreateTestActivityAsync(representative.Id);

            // Act - Invalid status transition
            Func<Task> act = async () => 
            {
                // Try to transition from Pending directly to invalid state
                var invalidStatus = (ActivityStatus)999;
                await _activityService.UpdateActivityStatusAsync(activity.Id, invalidStatus);
            };

            // Assert
            await act.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task PreventOrphanedData_OnUserDeletion()
        {
            // Arrange
            var user = await CreateTestUserAsync("orphan@example.com", "Password123!");

            // Act - Attempt to delete user
            Func<Task> act = async () => await _userService.DeleteUserAsync(user.Id);

            // Assert - Should prevent deletion or cascade
            await act.Should().ThrowAsync<InvalidOperationException>();
        }
    }
}