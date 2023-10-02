using System;
using System.Collections.Generic;

namespace SampleApplication.Models
{
    public record class Max(DateTime Date, decimal Amount);

    public record class Min(DateTime Date, decimal Amount);

    public record class DataSet(DateTime DateTime, decimal Amount);

    public record class ExpensiveHour(string Date, string Time, decimal Amount);

    public record class Statistic(Max Max, Min Min, decimal Average, ExpensiveHour ExpensiveHour, List<DataSet> Data);
}
