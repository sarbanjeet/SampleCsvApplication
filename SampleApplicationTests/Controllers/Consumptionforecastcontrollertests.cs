using Xunit;
using Microsoft.Extensions.Logging;
using Moq;
using SampleApplication.Interfaces;
using SampleApplication.Models;

namespace SampleApplication.Controllers.Tests
{
    public class ConsumptionForecastControllerTests
    {
        private readonly Mock<ILogger<ConsumptionForecastController>> mockLogger = new();
        private readonly Mock<IStatisticsService> mockStatisticsService = new();
        private readonly Mock<IDataService> mockDataService = new();


        [Fact]
        public async Task GetTest()
        {
            var fileName = "sampleSheet.csv";

            var data = new Dictionary<DateTime, decimal>
            {
                { new DateTime(2023, 9, 28, 0, 0, 0), 100m },
                { new DateTime(2023, 9, 28, 0, 30, 0), 200m },
                { new DateTime(2023, 9, 28, 1, 0, 0), 150m },
                { new DateTime(2023, 9, 28, 1, 30, 0), 80m },
            };

            var expected = new List<DataSet>()
            {
                new(new DateTime(2023, 9, 28, 0, 0, 0), 100m),
                new(new DateTime(2023, 9, 28, 0, 30, 0), 200m),
                new(new DateTime(2023, 9, 28, 1, 0, 0), 150m),
                new(new DateTime(2023, 9, 28, 1, 30, 0), 80m),
            };

            //Arrange
            mockDataService.Setup(e => e.GetDataAsync("sampleSheet.csv")).ReturnsAsync(data);

            var staticExpectedData = new Models.Statistic(new Max(new DateTime(2023, 9, 28, 1, 0, 0), 150m),
                new Min(new DateTime(2023, 9, 28, 1, 30, 0), 80m), 132.5m,
                new ExpensiveHour((new DateTime(2023, 9, 28)).Date.ToShortDateString(), "From 00:30:00 to 01:30:00",
                    350m), expected);
            mockStatisticsService.Setup(e => e.GetStatistics(fileName)).ReturnsAsync(staticExpectedData);


            var controller = new ConsumptionForecastController(mockLogger.Object, mockStatisticsService.Object);

            // Act
            var result = await controller.Get();

            // Assert
            Assert.NotNull(result);
        }
    }
}
