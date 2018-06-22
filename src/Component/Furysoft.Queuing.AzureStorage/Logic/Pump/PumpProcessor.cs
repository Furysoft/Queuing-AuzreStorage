// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PumpProcessor.cs" company="Simon Paramore">
// © 2017, Simon Paramore
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Furysoft.Queuing.AzureStorage.Logic.Pump
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Interfaces;
    using Interfaces.Pump;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Logging;
    using Microsoft.WindowsAzure.Storage.Queue;

    /// <summary>
    /// The Pump Processor
    /// </summary>
    internal sealed class PumpProcessor : IPumpProcessor
    {
        /// <summary>
        /// The logger
        /// </summary>
        [NotNull]
        private readonly ILogger logger;

        /// <summary>
        /// The queue wrapper
        /// </summary>
        [NotNull]
        private readonly IQueueWrapper queueWrapper;

        /// <summary>
        /// The subject
        /// </summary>
        private ConcurrentBag<CloudQueueMessage> subject = new ConcurrentBag<CloudQueueMessage>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PumpProcessor" /> class.
        /// </summary>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="queueWrapper">The queue wrapper.</param>
        public PumpProcessor([NotNull] ILoggerFactory loggerFactory, [NotNull] IQueueWrapper queueWrapper)
        {
            this.queueWrapper = queueWrapper;

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
        /// <param name="message">The message.</param>
        public void AddMessage(CloudQueueMessage message)
        {
            this.subject.Add(message);
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
                    var delay = TimeSpan.FromSeconds(1);
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

                        await Task.Delay(delay).ConfigureAwait(false);
                        var exchange = Interlocked.Exchange(ref this.subject, new ConcurrentBag<CloudQueueMessage>()).ToList();
                        foreach (var cloudQueueMessage in exchange.Skip(2000))
                        {
                            this.subject.Add(cloudQueueMessage);
                        }

                        var enumerable = exchange.Take(2000).ToList();

                        var sw = Stopwatch.StartNew();
                        await this.queueWrapper.SubmitMessagesAsync(enumerable).ConfigureAwait(false);
                        sw.Stop();

                        var timeRemaining = TimeSpan.FromSeconds(1) - sw.Elapsed;
                        delay = timeRemaining.Milliseconds > 0 ? timeRemaining : TimeSpan.Zero;

                        this.logger.LogDebug(
                            "Submitted {0} of {1} in {2}ms at @ {3}",
                            enumerable.Count,
                            exchange.Count,
                            sw.ElapsedMilliseconds,
                            DateTime.UtcNow.Second);

                        this.BatchSubmitted?.Invoke(this, enumerable.Count);
                    }
                });
        }
    }
}