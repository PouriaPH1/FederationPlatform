using Xunit;
using FluentAssertions;
using FederationPlatform.Application.Services;
using FederationPlatform.Application.DTOs;
using FederationPlatform.Domain.Entities;
using Moq;

namespace FederationPlatform.UnitTests.Services
{
    public class NewsServiceTests
    {
        private readonly Mock<INewsRepository> _mockNewsRepository;
        private readonly Mock<IEmailService> _mockEmailService;
        private readonly NewsService _newsService;

        public NewsServiceTests()
        {
            _mockNewsRepository = new Mock<INewsRepository>();
            _mockEmailService = new Mock<IEmailService>();
            _newsService = new NewsService(_mockNewsRepository.Object, _mockEmailService.Object);
        }

        [Fact]
        public async Task CreateNews_ValidData_ReturnsSuccess()
        {
            // Arrange
            var createDto = new CreateNewsDto
            {
                Title = \"Important News\",
                Content = \"News content here\",
                CreatedBy = 1
            };

            _mockNewsRepository.Setup(x => x.AddAsync(It.IsAny<News>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _newsService.CreateNewsAsync(createDto);

            // Assert
            result.Success.Should().BeTrue();
            _mockNewsRepository.Verify(x => x.AddAsync(It.IsAny<News>()), Times.Once);
        }

        [Fact]
        public async Task GetAllNews_ReturnsPublishedNews()
        {
            // Arrange
            var newsList = new List<News>
            {
                new News { Id = 1, Title = \"News 1\", IsPublished = true },
                new News { Id = 2, Title = \"News 2\", IsPublished = false },
                new News { Id = 3, Title = \"News 3\", IsPublished = true }
            };

            _mockNewsRepository.Setup(x => x.GetAllAsync())
                .ReturnsAsync(newsList);

            // Act
            var result = await _newsService.GetAllNewsAsync();

            // Assert
            result.Should().HaveCount(2);
            result.Should().AllSatisfy(x => x.IsPublished.Should().BeTrue());
        }

        [Fact]
        public async Task UpdateNews_ValidData_UpdatesSuccessfully()
        {
            // Arrange
            var newsId = 1;
            var updateDto = new UpdateNewsDto
            {
                Title = \"Updated Title\",
                Content = \"Updated Content\"
            };

            var news = new News { Id = newsId, Title = \"Old Title\" };

            _mockNewsRepository.Setup(x => x.GetByIdAsync(newsId))
                .ReturnsAsync(news);
            _mockNewsRepository.Setup(x => x.UpdateAsync(It.IsAny<News>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _newsService.UpdateNewsAsync(newsId, updateDto);

            // Assert
            result.Success.Should().BeTrue();
            news.Title.Should().Be(\"Updated Title\");
        }

        [Fact]
        public async Task DeleteNews_ValidId_RemovesSuccessfully()
        {
            // Arrange
            var newsId = 1;
            var news = new News { Id = newsId };

            _mockNewsRepository.Setup(x => x.GetByIdAsync(newsId))
                .ReturnsAsync(news);
            _mockNewsRepository.Setup(x => x.DeleteAsync(news))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _newsService.DeleteNewsAsync(newsId);

            // Assert
            result.Success.Should().BeTrue();
            _mockNewsRepository.Verify(x => x.DeleteAsync(news), Times.Once);
        }

        [Fact]
        public async Task PublishNews_ValidId_PublishesSuccessfully()
        {
            // Arrange
            var newsId = 1;
            var news = new News { Id = newsId, IsPublished = false };

            _mockNewsRepository.Setup(x => x.GetByIdAsync(newsId))
                .ReturnsAsync(news);
            _mockNewsRepository.Setup(x => x.UpdateAsync(It.IsAny<News>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _newsService.PublishNewsAsync(newsId);

            // Assert
            result.Success.Should().BeTrue();
            news.IsPublished.Should().BeTrue();
        }
    }
}
