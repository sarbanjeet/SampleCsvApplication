using System.Threading.Tasks;
using SampleApplication.Models;

namespace SampleApplication.Interfaces
{
    public interface IStatisticsService
    {
        Task<Statistic> GetStatistics(string fileName);
    }
}
