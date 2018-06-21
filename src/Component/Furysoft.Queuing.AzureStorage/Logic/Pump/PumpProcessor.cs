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
    using Microsoft.WindowsAzure.Storage.Queue;

    /// <summary>
    /// The Pump Processor
    /// </summary>
    internal sealed class PumpProcessor : IPumpProcessor
    {
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
        /// Initializes a new instance of the <see cref="PumpProcessor"/> class.
        /// </summary>
        /// <param name="queueWrapper">The queue wrapper.</param>
        public PumpProcessor([NotNull] IQueueWrapper queueWrapper)
        {
            this.queueWrapper = queueWrapper;
        }

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

                        Console.WriteLine($"Submitted {enumerable.Count} of {exchange.Count} in {sw.ElapsedMilliseconds}ms at @ {DateTime.UtcNow.Second}");
                    }
                });
        }
    }
}