using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SampleApplication.Interfaces;
using SampleApplication.Models;

namespace SampleApplication.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly IDataService dataService;

        public StatisticsService(IDataService dataService)
        {
            this.dataService = dataService;
        }

        public async Task<Statistic> GetStatistics(string fileName)
        {
            var csvData = await dataService.GetDataAsync(fileName);

            //Calculate max/min
            var maxEntry = csvData.Aggregate((l, r) => l.Value > r.Value ? l : r);
            var maxAmount = new Max(maxEntry.Key, maxEntry.Value);

            // Min
            var minEntry = csvData.Aggregate((l, r) => l.Value < r.Value ? l : r);
            var minAmount = new Min(minEntry.Key, minEntry.Value);


            // Average
            var averageAmount = csvData.Values.Average();

            //Calculate most expensive hour 
            var convertToList = csvData.ToList();
            var start = convertToList[0].Key;
            var maxSum = convertToList[0].Value + convertToList[1].Value;

            var dataSets = csvData.Select(r => new DataSet(r.Key, r.Value)).ToList();

            var expensiveHour = FindExpensiveHour(dataSets, maxSum, start, convertToList);


            return new Statistic(maxAmount, minAmount, averageAmount, expensiveHour, dataSets);
        }

        private static ExpensiveHour FindExpensiveHour(List<DataSet> dataSets, decimal maxSum, DateTime start,
            List<KeyValuePair<DateTime, decimal>> convertToList)
        {
            for (var i = 0; i < dataSets.Count - 1; i++)
            {
                if (dataSets[i].DateTime.AddMinutes(30) != dataSets[i + 1].DateTime)
                {
                    continue;
                }

                var sum = dataSets[i].Amount + dataSets[i + 1].Amount;
                if (sum <= maxSum)
                    continue;

                start = convertToList[i].Key;
                maxSum = sum;
            }


            var hour = "From " + start.TimeOfDay.ToString() + " to " + start.AddHours(1).TimeOfDay.ToString();
            var expensiveHour = new ExpensiveHour(start.ToShortDateString(), hour, maxSum);
            return expensiveHour;
        }
    }
}
