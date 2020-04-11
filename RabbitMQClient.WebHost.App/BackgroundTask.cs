using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQClient.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMQClient.WebHost.App
{
    public class BackgroundTask : BackgroundService
    {
        private readonly ILogger<BackgroundTask> _logger;
        private readonly IRabbitMQService _rabbitMQService;
        public BackgroundTask(ILogger<BackgroundTask> logger, IRabbitMQService rabbitMQService)
        {
            _logger = logger;
            _rabbitMQService = rabbitMQService;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("StartAsync().");
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("StopAsync().");
            return base.StopAsync(cancellationToken);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ExecuteAsync().");

            await _rabbitMQService.ConsumeAsync(default, MsgConsumer);
        }

        private async Task MsgConsumer(string msg, CancellationToken token)
        {
            _logger.LogInformation("MsgConsumer started.");
            _logger.LogInformation("Waiting for 4 sec");

            await Task.Delay(TimeSpan.FromSeconds(4));
            _logger.LogInformation("MsgConsumer completed.");

        }
    }
}
