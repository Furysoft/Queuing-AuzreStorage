// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueueFactory.cs" company="Simon Paramore">
// © 2017, Simon Paramore
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Furysoft.Queuing.AzureStorage
{
    using Core;
    using Entities;
    using Interfaces.Pump;
    using Logic;
    using Logic.Pump;
    using Microsoft.Extensions.Logging;
    using Serializers;

    /// <summary>
    /// The Queue Factory
    /// </summary>
    public static class QueueFactory
    {
        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <param name="queueEndpoint">The queue endpoint.</param>
        /// <param name="queueConfiguration">The queue configuration.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <returns>
        /// The <see cref="IQueue" />
        /// </returns>
        public static IQueue Create(
            QueueEndpoint queueEndpoint,
            QueueConfiguration queueConfiguration,
            ILoggerFactory loggerFactory = null)
        {
            var logger = loggerFactory ?? new LoggerFactory();

            var serializer = SerializerFactory.Create(queueConfiguration.SerializerSettings.SerializerType);

            var messageSerializer = new MessageSerializer(queueConfiguration.SerializerSettings);
            var queueWrapper = new QueueWrapper(queueEndpoint);

            IPumpProcessor pumpProcessor;
            if (queueConfiguration.SerializerSettings.UseVersionedMessages)
            {
                pumpProcessor = new VersionedMessagePumpProcessor(
                    logger,
                    queueWrapper,
                    serializer,
                    queueConfiguration.SerializerSettings,
                    queueConfiguration.PumpConfiguration);
            }
            else
            {
                pumpProcessor = new PumpProcessor(logger, queueWrapper, serializer);
            }

            var queuePump = new QueuePump(pumpProcessor, messageSerializer);

            return new Queue(queuePump);
        }
    }
}