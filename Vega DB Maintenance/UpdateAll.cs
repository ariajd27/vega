using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Vega_DB_Maintenance
{
    public class UpdateAll
    {
        private readonly ILogger _logger;

        public UpdateAll(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<UpdateAll>();
        }

        [Function("UpdateAll")]
        public async Task Run([TimerTrigger("0 2 * * *")] TimerInfo myTimer)
        {
            _logger.LogInformation($"Full database update triggered at: {DateTime.Now}");
            
            if (myTimer.ScheduleStatus is not null)
            {
                _logger.LogInformation($"Next update scheduled at: {myTimer.ScheduleStatus.Next}");
            }

            
        }
    }
}
