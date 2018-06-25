// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueueFactory.cs" company="Simon Paramore">
// © 2017, Simon Paramore
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Furysoft.Queuing.AzureStorage
{
    using Core;
    using Entities;
    using Entities.Configuration;
    using Logic;
    using Logic.Pump;
    using Logic.Wrappers;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The Queue Factory
    /// </summary>
    public static class QueueFactory
    {
        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <param name="queueConfiguration">The queue configuration.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <returns>
        /// The <see cref="IQueue" />
        /// </returns>
        public static IQueue Create(
            QueueConfiguration queueConfiguration,
            ILoggerFactory loggerFactory = null)
        {
            var logger = loggerFactory ?? new LoggerFactory();

            var queueEndpoint = new QueueEndpoint
            {
                ConnectionString = queueConfiguration.QueueConnectionString,
                QueueName = queueConfiguration.QueueName
            };

            var serializerSettings = new SerializerSettings
            {
                SerializerType = queueConfiguration.SerializerType
            };

            var batchSettings = new BatchSettings
            {
                MaxQueueMessagesPerSchedule = queueConfiguration.MaxQueueMessagesPerSchedule,
                MaxMessagesPerQueueMessage = queueConfiguration.MaxMessagesPerQueueMessage
            };

            var scheduleSettings = new ScheduleSettings
            {
                ThrottleTime = queueConfiguration.ThrottleTime
            };

            var queueWrapper = new QueueWrapper(queueEndpoint);
            var messageSerializer = new MessageSerializer(serializerSettings);
            var queueMessageSerializer = new QueueMessageSerializer(batchSettings, messageSerializer);

            var buffer = new Buffer(logger, queueWrapper, queueMessageSerializer);

            var stopwatchFactory = new StopwatchFactory();
            var delayCalculator = new DelayCalculator();
            var pumpProcessor = new PumpProcessor(
                logger,
                buffer,
                stopwatchFactory,
                delayCalculator,
                scheduleSettings);

            var queuePump = new QueuePump(buffer, pumpProcessor);

            return new Queue(queuePump);
        }
    }
}