using Moq;
using SampleApplication.Interfaces;
using SampleApplication.Models;
using SampleApplication.Services;
using Xunit;

namespace SampleApplicationTests.Services
{
    public class StatisticsServiceUnderTests
    {
        private readonly IStatisticsService statisticsService;

        private readonly Mock<IDataService> mockDataService;

        public StatisticsServiceUnderTests()
        {
            mockDataService = new Mock<IDataService>();
            statisticsService = new StatisticsService(mockDataService.Object);
        }

        [Fact]
        public async Task GetStatistics_WhenCalled_ReturnsCorrectStatistics()
        {
            // Arrange
            mockDataService.Setup(e => e.GetDataAsync("sampleSheet.csv"))
                .ReturnsAsync(() =>
                    new Dictionary<DateTime, decimal>
                    {
                        { new DateTime(2023, 9, 28, 0, 0, 0), 100m },
                        { new DateTime(2023, 9, 28, 0, 30, 0), 200m },
                        { new DateTime(2023, 9, 28, 1, 0, 0), 150m },
                        { new DateTime(2023, 9, 28, 1, 30, 0), 80m },
                    });

            var expectedExpensiveHour = new ExpensiveHour((new DateTime(2023, 9, 28)).Date.ToShortDateString(),
                "From 00:30:00 to 01:30:00", 350m);

            // Act
            var result = await statisticsService.GetStatistics("sampleSheet.csv");

            // Assert
            Assert.Equal(new Max(new DateTime(2023, 9, 28, 0, 30, 0), 200m), result.Max);
            Assert.Equal(new Min(new DateTime(2023, 9, 28, 1, 30, 0), 80m), result.Min);
            Assert.Equal(132.5m, result.Average);
            Assert.Equal(expectedExpensiveHour, result.ExpensiveHour);
            Assert.NotNull(result.Data);
        }
    }
}
