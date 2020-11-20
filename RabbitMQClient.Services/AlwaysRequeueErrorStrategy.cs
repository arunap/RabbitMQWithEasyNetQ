using EasyNetQ.Consumer;
using EasyNetQ.Logging;
using System;

namespace RabbitMQClient.Services
{
    public class AlwaysRequeueErrorStrategy : EasyNetQ.Consumer.IConsumerErrorStrategy
    {
        private readonly ILog logger = LogProvider.For<DefaultConsumerErrorStrategy>();
        public AlwaysRequeueErrorStrategy()
        {
        }

        public void Dispose()
        {
        }

        public AckStrategy HandleConsumerCancelled(ConsumerExecutionContext context)
        {
            return AckStrategies.NackWithRequeue;
        }

        public AckStrategy HandleConsumerError(ConsumerExecutionContext context, Exception exception)
        {
            logger.Error(
                exception,
                "Exception thrown by subscription callback, receivedInfo={receivedInfo}, properties={properties}",
                context.Info,
                context.Properties
            );

            return AckStrategies.NackWithRequeue;
        }
    }
}
