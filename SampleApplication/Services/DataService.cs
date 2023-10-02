using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SampleApplication.Interfaces;

namespace SampleApplication.Services;

public class DataService : IDataService
{
    private readonly ILogger<DataService> logger;
    private readonly IWebHostEnvironment webHostEnvironment;
    
    //Hint: Memory catch can be useful here

    public DataService(ILogger<DataService> logger,
        IWebHostEnvironment webHostEnvironment)
    {
        this.logger = logger;
        this.webHostEnvironment = webHostEnvironment;
    }
    
    public async Task<Dictionary<DateTime, decimal>> GetDataAsync(string fileName)
    {
        //Read the file 
        var data = await ReadCsvFile(fileName);
        return data;
    }

    private async Task<Dictionary<DateTime, decimal>> ReadCsvFile(string fileName)
    {
        var consumption = new Dictionary<DateTime, decimal>();

        try
        {
            var filePath = Path.Combine(webHostEnvironment.WebRootPath, "files", fileName);
            using var streamReader = new StreamReader(filePath);
            var isFirstLine = true;
            var lineNumber = 1;
            var duplicateOrInvalidLines = 0;
            const int numColumnsFormat = 2;

            while (await streamReader.ReadLineAsync() is { } line)
            {
                lineNumber++;

                if (isFirstLine)
                {
                    isFirstLine = false;
                    continue;
                }

                var splitData = line.Split(',');
                if (line.Split(',').Length != numColumnsFormat)
                {
                    logger.LogError($"Line is not in correct format at line number {lineNumber}: {line}");
                    duplicateOrInvalidLines++;
                    continue;
                }

                if (DateTime.TryParse(splitData[0], out var date)
                    && decimal.TryParse(splitData[1], out var price))
                {
                    if (consumption.TryAdd(date, price))
                        continue;

                    logger.LogError($"Duplicate data at line number {lineNumber}: {line}");
                    duplicateOrInvalidLines++;
                } else
                {
                    logger.LogError($"Invalid data at line number {lineNumber}: {line}");
                    duplicateOrInvalidLines++;
                }
            }

            logger.LogCritical(
                $"Total lines are processed {lineNumber} out these {duplicateOrInvalidLines} are duplicate.");
        }
        catch (FileNotFoundException notFoundException)
        {
            logger.LogCritical(notFoundException, "File not found");
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Issue in processing the file");
        }

        return consumption;
    }
}
