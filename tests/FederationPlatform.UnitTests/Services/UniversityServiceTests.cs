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
    public class UniversityServiceTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IUniversityRepository> _mockUniversityRepository;
        private readonly Mock<IActivityRepository> _mockActivityRepository;
        private readonly UniversityService _universityService;

        public UniversityServiceTests()
        {
            _fixture = new Fixture();
            _mockUniversityRepository = new Mock<IUniversityRepository>();
            _mockActivityRepository = new Mock<IActivityRepository>();
            _universityService = new UniversityService(
                _mockUniversityRepository.Object,
                _mockActivityRepository.Object);
        }

        [Fact]
        public async Task GetAllUniversities_ReturnsAllUniversities()
        {
            // Arrange
            var universities = _fixture.CreateMany<University>(25).ToList();
            _mockUniversityRepository.Setup(x => x.GetAllAsync())
                .ReturnsAsync(universities);

            // Act
            var result = await _universityService.GetAllUniversitiesAsync();

            // Assert
            result.Should().HaveCount(25);
        }

        [Fact]
        public async Task GetUniversityById_ValidId_ReturnsUniversity()
        {
            // Arrange
            var universityId = _fixture.Create<int>();
            var university = _fixture.Build<University>()
                .With(x => x.Id, universityId)
                .Create();

            _mockUniversityRepository.Setup(x => x.GetByIdAsync(universityId))
                .ReturnsAsync(university);

            // Act
            var result = await _universityService.GetUniversityByIdAsync(universityId);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(universityId);
        }

        [Fact]
        public async Task GetUniversityActivities_ValidId_ReturnsActivities()
        {
            // Arrange
            var universityId = _fixture.Create<int>();
            var university = _fixture.Build<University>()
                .With(x => x.Id, universityId)
                .Create();

            var activities = _fixture.Build<Activity>()
                .With(x => x.UniversityId, universityId)
                .With(x => x.Status, ActivityStatus.Approved)
                .CreateMany(10)
                .ToList();

            _mockUniversityRepository.Setup(x => x.GetByIdAsync(universityId))
                .ReturnsAsync(university);
            _mockActivityRepository.Setup(x => x.GetByUniversityIdAsync(universityId))
                .ReturnsAsync(activities);

            // Act
            var result = await _universityService.GetUniversityActivitiesAsync(universityId);

            // Assert
            result.Should().HaveCount(10);
            result.Should().AllSatisfy(x => x.UniversityId == universityId);
        }

        [Fact]
        public async Task SearchUniversities_ValidQuery_ReturnsMatches()
        {
            // Arrange
            var query = "Tehran";
            var universities = _fixture.Build<University>()
                .With(x => x.Name, "University of Tehran")
                .CreateMany(1)
                .ToList();

            _mockUniversityRepository.Setup(x => x.SearchAsync(query))
                .ReturnsAsync(universities);

            // Act
            var result = await _universityService.SearchUniversitiesAsync(query);

            // Assert
            result.Should().HaveCount(1);
            result.First().Name.Should().Contain("Tehran");
        }

        [Fact]
        public async Task GetActiveUniversities_ReturnsOnlyActive()
        {
            // Arrange
            var universities = new List<University>
            {
                _fixture.Build<University>().With(x => x.IsActive, true).Create(),
                _fixture.Build<University>().With(x => x.IsActive, false).Create(),
                _fixture.Build<University>().With(x => x.IsActive, true).Create()
            };

            _mockUniversityRepository.Setup(x => x.GetAllAsync())
                .ReturnsAsync(universities);

            // Act
            var result = await _universityService.GetActiveUniversitiesAsync();

            // Assert
            result.Should().HaveCount(2);
            result.Should().AllSatisfy(x => x.IsActive == true);
        }

        [Fact]
        public async Task GetUniversitiesByProvince_ValidProvince_ReturnsUniversities()
        {
            // Arrange
            var province = "Tehran";
            var universities = _fixture.Build<University>()
                .With(x => x.Province, province)
                .CreateMany(5)
                .ToList();

            _mockUniversityRepository.Setup(x => x.GetByProvinceAsync(province))
                .ReturnsAsync(universities);

            // Act
            var result = await _universityService.GetUniversitiesByProvinceAsync(province);

            // Assert
            result.Should().HaveCount(5);
            result.Should().AllSatisfy(x => x.Province == province);
        }
    }
}
