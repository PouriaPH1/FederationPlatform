using Xunit;
using FluentAssertions;
using FederationPlatform.Application.DTOs;
using FederationPlatform.Domain.Entities;

namespace FederationPlatform.IntegrationTests.Security
{
    public class AuthorizationTests : IntegrationTestBase
    {
        [Fact]
        public async Task AdminCanAccessAllFeatures()
        {
            // Arrange
            var admin = await CreateTestAdminAsync(\"admin@example.com\");

            // Act - Try admin-only operations
            var users = await _adminService.GetAllUsersAsync();
            var pendingActivities = await _adminService.GetPendingActivitiesAsync();

            // Assert
            users.Should().NotBeNull();
            pendingActivities.Should().NotBeNull();
        }

        [Fact]
        public async Task RepresentativeCanOnlyCreateActivitiesForTheirUniversity()
        {
            // Arrange
            var representative = await CreateTestRepresentativeAsync(\"rep@example.com\");
            var university1 = await CreateTestUniversityAsync();
            var university2 = await CreateTestUniversityAsync();

            // Assign representative to university1
            await AssignRepresentativeToUniversityAsync(representative.Id, university1.Id);

            // Act - Create activity for assigned university
            var createDto1 = new CreateActivityDto
            {
                Title = \"Activity in assigned university\",
                UniversityId = university1.Id,
                UserId = representative.Id
            };

            var result1 = await _activityService.CreateActivityAsync(createDto1);

            // Assert
            result1.Should().BeGreaterThan(0);

            // Act - Try to create activity for different university
            var createDto2 = new CreateActivityDto
            {
                Title = \"Activity in different university\",
                UniversityId = university2.Id,
                UserId = representative.Id
            };

            Func<Task> act = async () => await _activityService.CreateActivityAsync(createDto2);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [Fact]
        public async Task RegularUserCannotApproveActivities()
        {
            // Arrange
            var regularUser = await CreateTestUserAsync(\"user@example.com\", \"Password123!\");
            var activity = await CreateTestActivityAsync(regularUser.Id);

            // Act
            Func<Task> act = async () => await _activityService.ApproveActivityAsync(activity.Id);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [Fact]
        public async Task OnlyActivityCreatorCanEditPendingActivity()
        {
            // Arrange
            var creator = await CreateTestRepresentativeAsync(\"creator@example.com\");
            var otherUser = await CreateTestRepresentativeAsync(\"other@example.com\");
            
            var activity = await CreateTestActivityAsync(creator.Id);

            var updateDto = new UpdateActivityDto { Title = \"Updated Title\" };

            // Act - Creator can update
            var result1 = await _activityService.UpdateActivityAsync(activity.Id, updateDto);

            // Assert
            result1.Success.Should().BeTrue();

            // Act - Other user cannot update
            Func<Task> act = async () => await _activityService.UpdateActivityAsync(activity.Id, updateDto);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [Fact]
        public async Task UserCannotModifyAnotherUserProfile()
        {
            // Arrange
            var user1 = await CreateTestUserAsync(\"user1@example.com\", \"Password123!\");
            var user2 = await CreateTestUserAsync(\"user2@example.com\", \"Password123!\");

            var updateDto = new UpdateUserProfileDto { FirstName = \"Hacked\" };

            // Act
            Func<Task> act = async () => await _userProfileService.UpdateProfileAsync(user2.Id, updateDto);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [Fact]
        public async Task AdminCanModifyAnyUserProfile()
        {
            // Arrange
            var admin = await CreateTestAdminAsync(\"admin@example.com\");
            var user = await CreateTestUserAsync(\"user@example.com\", \"Password123!\");

            var updateDto = new UpdateUserProfileDto { FirstName = \"AdminModified\" };

            // Act
            var result = await _userProfileService.UpdateProfileAsync(user.Id, updateDto);

            // Assert
            result.Success.Should().BeTrue();
        }

        [Fact]
        public async Task RoleBasedMenuAccess()
        {
            // Arrange
            var admin = await CreateTestAdminAsync(\"admin@example.com\");
            var representative = await CreateTestRepresentativeAsync(\"rep@example.com\");
            var regularUser = await CreateTestUserAsync(\"user@example.com\", \"Password123!\");

            // Act - Get menu items for each role
            var adminMenu = await _menuService.GetMenuItemsAsync(admin.Role);
            var repMenu = await _menuService.GetMenuItemsAsync(representative.Role);
            var userMenu = await _menuService.GetMenuItemsAsync(regularUser.Role);

            // Assert - Admin should see all menu items
            adminMenu.Count().Should().BeGreaterThan(repMenu.Count());
            repMenu.Count().Should().BeGreaterThan(userMenu.Count());

            // Admin menu should include admin features
            adminMenu.Should().Contain(x => x.Name.Contains(\"Admin\"));

            // User menu should not include admin features
            userMenu.Should().NotContain(x => x.Name.Contains(\"Admin\"));
        }

        [Fact]
        public async Task TokenBasedAuthorization()
        {
            // Arrange
            var user = await CreateTestUserAsync(\"token@example.com\", \"Password123!\");
            var loginResult = await _authService.LoginAsync(
                new LoginDto { Email = user.Email, Password = \"Password123!\" });

            var token = loginResult.Data;

            // Act
            var isValid = await _tokenService.ValidateTokenAsync(token);

            // Assert
            isValid.Should().BeTrue();
        }

        [Fact]
        public async Task ExpiredTokenRejected()
        {
            // Arrange
            var expiredToken = \"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJleHAiOjE2MDAwMDAwMDB9...\"; // Mock expired token

            // Act
            var isValid = await _tokenService.ValidateTokenAsync(expiredToken);

            // Assert
            isValid.Should().BeFalse();
        }
    }
}
