// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Buffer.cs" company="Simon Paramore">
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
    using Entities.Buffer;
    using Interfaces;
    using Interfaces.Pump;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Logging;
    using Versioning;

    /// <summary>
    /// The Pump Processor
    /// </summary>
    internal sealed class Buffer : IBuffer
    {
        /// <summary>
        /// The logger
        /// </summary>
        [NotNull]
        private readonly ILogger logger;

        /// <summary>
        /// The queue message serializer
        /// </summary>
        [NotNull]
        private readonly IQueueMessageSerializer queueMessageSerializer;

        /// <summary>
        /// The queue wrapper
        /// </summary>
        [NotNull]
        private readonly IQueueWrapper queueWrapper;

        /// <summary>
        /// The subject
        /// </summary>
        private ConcurrentBag<VersionedMessage> subject;

        /// <summary>
        /// Initializes a new instance of the <see cref="Buffer" /> class.
        /// </summary>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="queueWrapper">The queue wrapper.</param>
        /// <param name="queueMessageSerializer">The queue message serializer.</param>
        public Buffer(
            [NotNull] ILoggerFactory loggerFactory,
            [NotNull] IQueueWrapper queueWrapper,
            [NotNull] IQueueMessageSerializer queueMessageSerializer)
        {
            this.subject = new ConcurrentBag<VersionedMessage>();

            this.queueWrapper = queueWrapper;
            this.queueMessageSerializer = queueMessageSerializer;

            this.logger = loggerFactory.CreateLogger<Buffer>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Buffer"/> class.
        /// </summary>
        /// <param name="initialBag">The initial bag.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="queueWrapper">The queue wrapper.</param>
        /// <param name="queueMessageSerializer">The queue message serializer.</param>
        public Buffer(
            ConcurrentBag<VersionedMessage> initialBag,
            [NotNull] ILoggerFactory loggerFactory,
            [NotNull] IQueueWrapper queueWrapper,
            [NotNull] IQueueMessageSerializer queueMessageSerializer)
        {
            this.subject = initialBag;

            this.queueWrapper = queueWrapper;
            this.queueMessageSerializer = queueMessageSerializer;

            this.logger = loggerFactory.CreateLogger<Buffer>();
        }

        /// <summary>
        /// Gets the buffer.
        /// </summary>
        public IReadOnlyCollection<VersionedMessage> InternalBuffer => this.subject;

        /// <summary>
        /// Adds the message.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="message">The message.</param>
        public void AddMessage<TEntity>(TEntity message)
            where TEntity : class
        {
            var versionedMessage = this.queueMessageSerializer.GetVersionedMessage(message);

            this.subject.Add(versionedMessage);
        }

        /// <summary>
        /// Processes the buffer asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="BufferProcessResponse"/></returns>
        public async Task<BufferProcessResponse> ProcessBufferAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (this.subject.Count == 0)
            {
                return new BufferProcessResponse { Processed = 0, Remaining = 0 };
            }

            var newBuffer = new ConcurrentBag<VersionedMessage>();
            var oldBuffer = Interlocked.Exchange(ref this.subject, newBuffer).ToList();

            var queueMessageSerializerResponse = this.queueMessageSerializer.GetQueueMessages(oldBuffer);

            /* Re-add the non processed messages to the buffer */
            foreach (var versionedMessage in queueMessageSerializerResponse.UnusedMessages)
            {
                newBuffer.Add(versionedMessage);
            }

            /* Process the items selected for submission */
            var queueMessages = queueMessageSerializerResponse.QueueMessages.ToList();

            /* Submit the messages to the queue */
            var sw = Stopwatch.StartNew();
            await this.queueWrapper.SubmitMessagesAsync(queueMessages, cancellationToken).ConfigureAwait(false);
            sw.Stop();

            this.logger.LogDebug(
                "Submitted {0} of {1} in {2}ms at @ {3}",
                queueMessages.Count,
                oldBuffer.Count,
                sw.ElapsedMilliseconds,
                DateTime.UtcNow.Second);

            return new BufferProcessResponse
            {
                Remaining = newBuffer.Count,
                Processed = queueMessages.Count
            };
        }
    }
}