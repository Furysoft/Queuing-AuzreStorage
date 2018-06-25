// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VersionedMessagePumpProcessor.cs" company="Simon Paramore">
// © 2017, Simon Paramore
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Furysoft.Queuing.AzureStorage.Logic.Pump
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Entities.Configuration;
    using Helpers;
    using Interfaces;
    using Interfaces.Pump;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Logging;
    using Microsoft.WindowsAzure.Storage.Queue;
    using Serializers;
    using Serializers.Versioning;
    using Versioning;

    /// <summary>
    /// The Versioned Message Pump Processor
    /// </summary>
    internal sealed class VersionedMessagePumpProcessor : IPumpProcessor
    {
        /// <summary>
        /// The logger
        /// </summary>
        [NotNull]
        private readonly ILogger logger;

        /// <summary>
        /// The pump configuration
        /// </summary>
        [NotNull]
        private readonly PumpConfiguration pumpConfiguration;

        /// <summary>
        /// The queue wrapper
        /// </summary>
        [NotNull]
        private readonly IQueueWrapper queueWrapper;

        /// <summary>
        /// The serializer
        /// </summary>
        [NotNull]
        private readonly ISerializer serializer;

        /// <summary>
        /// The serializer settings
        /// </summary>
        [NotNull]
        private readonly SerializerSettings serializerSettings;

        /// <summary>
        /// The subject
        /// </summary>
        private ConcurrentBag<VersionedMessage> subject = new ConcurrentBag<VersionedMessage>();

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionedMessagePumpProcessor" /> class.
        /// </summary>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="queueWrapper">The queue wrapper.</param>
        /// <param name="serializer">The serializer.</param>
        /// <param name="serializerSettings">The serializer settings.</param>
        /// <param name="pumpConfiguration">The pump configuration.</param>
        public VersionedMessagePumpProcessor(
            [NotNull] ILoggerFactory loggerFactory,
            [NotNull] IQueueWrapper queueWrapper,
            [NotNull] ISerializer serializer,
            [NotNull] SerializerSettings serializerSettings,
            [NotNull] PumpConfiguration pumpConfiguration)
        {
            this.queueWrapper = queueWrapper;
            this.serializer = serializer;
            this.serializerSettings = serializerSettings;
            this.pumpConfiguration = pumpConfiguration;

            this.logger = loggerFactory.CreateLogger<PumpProcessor>();
        }

        /// <summary>
        /// Occurs when [batch submitted].
        /// </summary>
        public event EventHandler<int> BatchSubmitted;

        /// <summary>
        /// Occurs when [buffer empty].
        /// </summary>
        public event EventHandler BufferEmpty;

        /// <summary>
        /// Adds the message.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="message">The message.</param>
        public void AddMessage<TEntity>(TEntity message)
            where TEntity : class
        {
            var body = message.SerializeToVersionedMessage(this.serializerSettings.SerializerType);

            this.subject.Add(body);
        }

        /// <summary>
        /// Starts the specified cancellation token.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        public void Start(CancellationToken cancellationToken)
        {
            Task.Run(
                async () =>
                {
                    var delay = this.pumpConfiguration.ThrottleTime;
                    while (true)
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            break;
                        }

                        if (this.subject.Count == 0)
                        {
                            this.BufferEmpty?.Invoke(this, EventArgs.Empty);
                            continue;
                        }

                        var takeCount = this.pumpConfiguration.MaxRequestsPerThrottleTime *
                                        this.pumpConfiguration.MaxBatchSize;

                        await Task.Delay(delay).ConfigureAwait(false);
                        var exchange = Interlocked.Exchange(ref this.subject, new ConcurrentBag<VersionedMessage>()).ToList();
                        foreach (var cloudQueueMessage in exchange.Skip(takeCount))
                        {
                            this.subject.Add(cloudQueueMessage);
                        }

                        List<CloudQueueMessage> submitMessages;
                        if (this.pumpConfiguration.MaxBatchSize > 1)
                        {
                            submitMessages = exchange
                                .Take(takeCount)
                                .BatchMessages(this.pumpConfiguration.MaxBatchSize)
                                .Select(
                                    r =>
                                    {
                                        var body = this.serializer.SerializeToString(r);
                                        return new CloudQueueMessage(body);
                                    })
                                .ToList();
                        }
                        else
                        {
                            submitMessages = exchange
                                .Take(takeCount)
                                .Select(
                                r =>
                                {
                                    var body = this.serializer.SerializeToString(r);
                                    return new CloudQueueMessage(body);
                                }).ToList();
                        }

                        var sw = Stopwatch.StartNew();
                        await this.queueWrapper.SubmitMessagesAsync(submitMessages).ConfigureAwait(false);
                        sw.Stop();

                        var timeRemaining = delay - sw.Elapsed;
                        delay = timeRemaining.Milliseconds > 0 ? timeRemaining : TimeSpan.Zero;

                        this.logger.LogDebug(
                            "Submitted {0} of {1} in {2}ms at @ {3}",
                            submitMessages.Count,
                            exchange.Count,
                            sw.ElapsedMilliseconds,
                            DateTime.UtcNow.Second);

                        this.BatchSubmitted?.Invoke(this, submitMessages.Count);
                    }
                });
        }
    }
}