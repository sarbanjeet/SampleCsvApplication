using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SampleApplication.Interfaces;
using SampleApplication.Models;

namespace SampleApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConsumptionForecastController : ControllerBase
    {
        private readonly ILogger<ConsumptionForecastController> logger;
        private readonly IStatisticsService statisticsService;

        public ConsumptionForecastController(ILogger<ConsumptionForecastController> logger,
            IStatisticsService statisticsService)
        {
            this.logger = logger;
            this.statisticsService = statisticsService;
        }

        [HttpGet]
        public async Task<OkObjectResult> Get()
        {
            var data = await statisticsService.GetStatistics("sampleSheet.csv");
            return Ok(data);
        }
    }
}
