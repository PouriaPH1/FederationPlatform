using Xunit;
using FluentAssertions;
using FederationPlatform.Application.Services;
using FederationPlatform.Application.DTOs;
using FederationPlatform.Domain.Entities;
using Moq;

namespace FederationPlatform.UnitTests.Services
{
    public class WorkshopServiceTests
    {
        private readonly Mock<IWorkshopRepository> _mockWorkshopRepository;
        private readonly Mock<IEmailService> _mockEmailService;
        private readonly WorkshopService _workshopService;

        public WorkshopServiceTests()
        {
            _mockWorkshopRepository = new Mock<IWorkshopRepository>();
            _mockEmailService = new Mock<IEmailService>();
            _workshopService = new WorkshopService(_mockWorkshopRepository.Object, _mockEmailService.Object);
        }

        [Fact]
        public async Task CreateWorkshop_ValidData_ReturnsSuccess()
        {
            // Arrange
            var createDto = new CreateWorkshopDto
            {
                Title = "Leadership Workshop",
                Description = "Learn leadership skills",
                StartDate = DateTime.UtcNow.AddDays(7),
                EndDate = DateTime.UtcNow.AddDays(8),
                Location = "Tehran Hall",
                CreatedBy = 1
            };

            _mockWorkshopRepository.Setup(x => x.AddAsync(It.IsAny<Workshop>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _workshopService.CreateWorkshopAsync(createDto);

            // Assert
            result.Success.Should().BeTrue();
        }

        [Fact]
        public async Task GetAllWorkshops_ReturnsUpcomingWorkshops()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var workshops = new List<Workshop>
            {
                new Workshop { Id = 1, Title = "Past Workshop", StartDate = now.AddDays(-10) },
                new Workshop { Id = 2, Title = "Upcoming Workshop", StartDate = now.AddDays(7) },
                new Workshop { Id = 3, Title = "Future Workshop", StartDate = now.AddDays(30) }
            };

            _mockWorkshopRepository.Setup(x => x.GetAllAsync())
                .ReturnsAsync(workshops);

            // Act
            var result = await _workshopService.GetUpcomingWorkshopsAsync();

            // Assert
            result.Should().HaveCount(2);
            result.Should().AllSatisfy(x => x.StartDate.Should().BeAfter(now));
        }

        [Fact]
        public async Task RegisterForWorkshop_ValidId_RegistersSuccessfully()
        {
            // Arrange
            var workshopId = 1;
            var userId = 1;
            var workshop = new Workshop { Id = workshopId, Title = \"Workshop\" };

            _mockWorkshopRepository.Setup(x => x.GetByIdAsync(workshopId))
                .ReturnsAsync(workshop);

            // Act
            var result = await _workshopService.RegisterUserAsync(workshopId, userId);

            // Assert
            result.Success.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateWorkshop_ValidData_UpdatesSuccessfully()
        {
            // Arrange
            var workshopId = 1;
            var updateDto = new UpdateWorkshopDto
            {
                Title = \"Updated Workshop Title\",
                Location = \"New Location\"
            };

            var workshop = new Workshop { Id = workshopId, Title = \"Old Title\" };

            _mockWorkshopRepository.Setup(x => x.GetByIdAsync(workshopId))
                .ReturnsAsync(workshop);

            // Act
            var result = await _workshopService.UpdateWorkshopAsync(workshopId, updateDto);

            // Assert
            result.Success.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteWorkshop_ValidId_DeletesSuccessfully()
        {
            // Arrange
            var workshopId = 1;
            var workshop = new Workshop { Id = workshopId };

            _mockWorkshopRepository.Setup(x => x.GetByIdAsync(workshopId))
                .ReturnsAsync(workshop);
            _mockWorkshopRepository.Setup(x => x.DeleteAsync(workshop))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _workshopService.DeleteWorkshopAsync(workshopId);

            // Assert
            result.Success.Should().BeTrue();
        }

        [Fact]
        public async Task CancelWorkshop_ValidId_CancelsAndNotifies()
        {
            // Arrange
            var workshopId = 1;
            var workshop = new Workshop { Id = workshopId, Title = \"Workshop\", IsCancelled = false };

            _mockWorkshopRepository.Setup(x => x.GetByIdAsync(workshopId))
                .ReturnsAsync(workshop);

            // Act
            var result = await _workshopService.CancelWorkshopAsync(workshopId);

            // Assert
            result.Success.Should().BeTrue();
            workshop.IsCancelled.Should().BeTrue();
            _mockEmailService.Verify(
                x => x.SendWorkshopCancellationEmailAsync(It.IsAny<int>(), workshopId),
                Times.Once);
        }
    }
}
