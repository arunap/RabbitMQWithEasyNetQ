using EasyNetQ;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMQClient.Contracts
{
    public interface IRabbitMQService
    {/// <summary>
     /// Consume Messages from given Message Queue
     /// </summary>
     /// <param name="name">Name of the Message Queue</param>
     /// <param name="stoppingToken">Cancellation token of the parent process</param>
     /// <param name="actionToPerform">Method to execute when a message is receieved from the queue</param>
     /// <returns></returns>
        Task ConsumeAsync(CancellationToken stoppingToken, Func<string, CancellationToken, Task> actionToPerform);

        /// <summary>
        /// Publish Messages to the given Queue
        /// </summary>
        /// <param name="exchangename"></param>
        /// <param name="name"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        Task PublishAsync<T>(Message<T> message);

    }
}
