﻿// --------------------------------------------------------------------------------------------------------------------
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
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The Queue Factory
    /// </summary>
    public static class QueueFactory
    {
        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <param name="queueEndpoint">The queue endpoint.</param>
        /// <param name="serializerSettings">The serializer settings.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <returns>
        /// The <see cref="IQueue" />
        /// </returns>
        public static IQueue Create(
            QueueEndpoint queueEndpoint,
            SerializerSettings serializerSettings,
            ILoggerFactory loggerFactory = null)
        {
            var logger = loggerFactory ?? new LoggerFactory();

            var messageSerializer = new MessageSerializer(serializerSettings);

            var queueWrapper = new QueueWrapper(queueEndpoint);

            var pumpProcessor = new PumpProcessor(logger, queueWrapper);

            var queuePump = new QueuePump(pumpProcessor, messageSerializer);

            return new Queue(queuePump);
        }
    }
}