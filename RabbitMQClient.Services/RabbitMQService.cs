using EasyNetQ;
using EasyNetQ.Topology;
using Microsoft.Extensions.Logging;
using RabbitMQClient.Contracts;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMQClient.Services
{
    public class RabbitMQService: IRabbitMQService
    {
        const string _queueName = "default_rb_queue";
        const string _exchangeName = "default_rb_exchange";

        private readonly IBus _serviceBus;
        private readonly IAdvancedBus _advancedBus;
        private readonly ILogger<RabbitMQService> _logger;

        private static EasyNetQ.Topology.IQueue _taskQueue;
        public RabbitMQService(IBus serviceBus, ILogger<RabbitMQService> logger)
        {
            _serviceBus = serviceBus;
            _logger = logger;
            if (_serviceBus == null) throw new Exception();
            this._advancedBus = _serviceBus.Advanced;
        }

        private async Task<EasyNetQ.Topology.IQueue> InitializeGetQueuesAsync()
        {
            _taskQueue ??= await _advancedBus.QueueDeclareAsync(_queueName, maxPriority: byte.MaxValue);
            return _taskQueue;
        }

        public async Task ConsumeAsync(CancellationToken stoppingToken, Func<string, CancellationToken, Task> actionToPerform)
        {
            _logger.LogDebug("InitializeGetQueuesAsync() started.");
            var queue = await InitializeGetQueuesAsync();
            _logger.LogDebug("InitializeGetQueuesAsync() completed.");

            IDisposable disposable = _advancedBus.Consume(queue, async (body, properties, info) =>
            {
                var message = Encoding.UTF8.GetString(body);

                throw new Exception("Test Exception.");

                _logger.LogDebug($"ConsumeAsync() started.");
                await actionToPerform(message, stoppingToken);
                _logger.LogDebug($"ConsumeAsync() completed.");

            }, consumerConfiguration => { consumerConfiguration.WithPrefetchCount(1); });
        }

        public async Task PublishAsync<T>(Message<T> message)
        {
            _logger.LogDebug("PublishAsync() started.");
            if (message != null)
            {
                var exchange = await BindExchangeWithMessageQueueAsync(_exchangeName);
                await _advancedBus.PublishAsync(exchange, string.Empty, true, message);
            }
            else
            {
                _logger.LogError("Unsupported message received. Message ignored.");
            }
            _logger.LogDebug("PublishAsync() completed.");
        }

        private async Task<EasyNetQ.Topology.IExchange> BindExchangeWithMessageQueueAsync(string exchangename)
        {
            _logger.LogDebug("BindExchangeWithMessageQueueAsync() started.");

            var exchange = await _advancedBus.ExchangeDeclareAsync(exchangename, ExchangeType.Topic);
            var queue = await InitializeGetQueuesAsync();
            var binding = await _advancedBus.BindAsync(exchange, queue, string.Empty);

            _logger.LogDebug("BindExchangeWithMessageQueueAsync() completed.");
            return exchange;
        }
    }
}
