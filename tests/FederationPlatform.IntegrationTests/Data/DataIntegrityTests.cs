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
            var representative = await CreateTestRepresentativeAsync(\"cascade@example.com\");
            var activity = await CreateTestActivityAsync(representati"cascade@example.com");
            var activity = await CreateTestActivityAsync(representative.Id);

            // Act - Delete activity
            await _activityService.DeleteActivityAsync(activity.Id);

            // Assert - Activity should be deleted
            var deletedActivity = await _activityService.GetActivityByIdAsync(activity.Id);
            deletedActivity.Should().BeNull

        [Fact]
        public async Task UniqueConstraint_UserEmail()
        {
            // Arrange
            var email = \"unique@example.com\";
            var user1 = new CreateUserDto
            {
                Username = \"user1\",
                Email = email,
                Password = \"Password123!\"
            };"unique@example.com";
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
            var countBefore = (await _universityService.GetAllUniversitiesAsync()).Count();

            // Act - Count after should match before
            var countAfter = (await _universityService.GetAllUniversitiesAsync()).Count();

            // Assert - Count should not change
            countAfter.Should().Be(countBefore);
        }

        [Fact]
        public async Task ConcurrentDataModification_Handled()
        {
            // Arrange
            var university = await CreateTestUniversityAsync();
            var updateDto = new UpdateUniversityDto { Description = \"Updated\" };

            // Act - Concurrent updates
            var tasks = Enumerable.Range(0, 5)
                .Select(_ => _universityService.UpdateUniversityAsync(university.Id, updateDto))
                .ToArray();

            var results = await Task.WhenAll(tasks);

            // Act - Get university multiple times
            var tasks = Enumerable.Range(0, 5)
                .Select(_ => _universityService.GetUniversityByIdAsync(university.Id))
                .ToArray();

            var results = await Task.WhenAll(tasks);

            // Assert
            results.Should().AllSatisfy(r => r.Should().NotBeNull
            // Act - Create profile
            var profileDto = new UpdateUserProfileDto
            {
                FirstName = \"Test\",
                LastName = \"User\",
                UniversityId = 1,
                PhoneNumber = \"09123456789\"
            };"profile@example.com", "Password123!");

            // Act - Create profile
            var profileDto = new UpdateUserProfileDto
            {
                FirstName = "Test",
                LastName = "User",
                UniversityId = 1,
                PhoneNumber = "09123456789"
            };

            var result = await _userProfileService.UpdateProfileAsync(user.Id, profileDto);

            // Assert
            result.Success.Should().BeTrue();
            user
            // Act - Invalid status transition
            Func<Task> act = async () => 
            {
                // Try to transition from Pending directly to invalid state
                var invalidStatus = (ActivityStatus)999;
                await _activityService.UpdateActivityStatusAsync(activity.Id, invalidStatus);
            };

            // Assert
            await act.Should().ThrowAsync<ArgumentException>();"consistent@example.com");
            var activity = await CreateTestActivityAsync(representative.Id);

            // Assert
            activity.Status.Should().Be(ActivityStatus.Pending
            Func<Task> act = async () => await _userService.DeleteUserAsync(user.Id);

            // Assert - Should prevent deletion or cascade
            await act.Should().ThrowAsync<InvalidOperationException>();
        }
    }
}"orphan@example.com", "Password123!");

            // Act - Create activity
            var activity = await CreateTestActivityAsync(user.Id);

            // Assert
            activity.UserId.Should().Be(user.Id);
            activity.Should().NotBeNull