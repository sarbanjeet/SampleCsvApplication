using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using SampleApplication.Interfaces;
using SampleApplication.Services;
using Xunit;

namespace SampleApplicationTests.Services
{
    public class DataServiceUnderTests
    {
        private readonly IDataService dataService;
        private readonly Mock<IWebHostEnvironment> mockWebHostEnvironment;
        private readonly Mock<ILogger<DataService>> mockLogger;

        public DataServiceUnderTests()
        {
            mockWebHostEnvironment = new Mock<IWebHostEnvironment>();
            mockLogger = new Mock<ILogger<DataService>>();
            dataService = new DataService(mockLogger.Object, mockWebHostEnvironment.Object);
        }

        [Fact]
        public async Task Read_Csv_File_Valid_File_Returns_NonEmpty_Dictionary()
        {
            // Arrange
            var validFileName = "sampleSheet.csv";
            var content = new StringBuilder()
                .AppendLine("Date,Price")
                .AppendLine("10/01/2017 00:30,100.5")
                .ToString();

            var folderPath = Path.Combine(AppContext.BaseDirectory, "files");
            var filePath = Path.Combine(folderPath, validFileName);

            mockWebHostEnvironment.Setup(x => x.WebRootPath).Returns(AppContext.BaseDirectory);

            // Ensure the directory exists
            Directory.CreateDirectory(folderPath);
            await File.WriteAllTextAsync(filePath, content);

            // Act
            var result = await dataService.GetDataAsync(validFileName);

            // Assert
            Assert.NotEmpty(result);

            // Cleanup
            File.Delete(filePath);
            Directory.Delete(folderPath);
        }

        [Fact]
        public async Task Read_Csv_File_Invalid_Line_Logs_Error()
        {
            // Arrange
            var invalidFileName = "invalidFile.csv";
            var content = new StringBuilder()
                .AppendLine("Date,Price")
                .AppendLine("InvalidLine")
                .ToString();

            var folderPath = Path.Combine(AppContext.BaseDirectory, "files");
            var filePath = Path.Combine(folderPath, invalidFileName);

            mockWebHostEnvironment.Setup(x => x.WebRootPath).Returns(AppContext.BaseDirectory);

            // Ensure the directory exists
            Directory.CreateDirectory(folderPath);
            await File.WriteAllTextAsync(filePath, content);

            // Act
            await dataService.GetDataAsync(invalidFileName);

            // Assert
            mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((o, t) => true)),
                Times.Once);

            File.Delete(filePath);
        }

        [Fact]
        public async Task Read_Csv_File_FileNotFound_Logs_Critical()
        {
            // Arrange
            var nonExistentFileName = "nonExistentFile.csv";
            mockWebHostEnvironment.Setup(x => x.WebRootPath).Returns("");
            mockWebHostEnvironment.Setup(x => x.WebRootPath).Throws(new FileNotFoundException("File not found!"));

            // Act
            await dataService.GetDataAsync(nonExistentFileName);

            // Assert
            mockLogger.Verify(
                x => x.Log(
                    LogLevel.Critical,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => true),
                    It.IsAny<FileNotFoundException>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((o, t) => true)),
                Times.Once);
        }

        [Fact]
        public async Task Read_Csv_File_OtherException_Logs_Critical()
        {
            // Arrange
            var fileNameCausingException = "exceptionFile.csv";
            mockWebHostEnvironment.Setup(x => x.WebRootPath).Returns("");
            mockWebHostEnvironment.Setup(x => x.WebRootPath).Throws(new Exception("Demo Exception"));

            // Act
            await dataService.GetDataAsync(fileNameCausingException);

            // Assert
            mockLogger.Verify(
                x => x.Log(
                    LogLevel.Critical,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((o, t) => true)),
                Times.Once);
        }
    }
}
