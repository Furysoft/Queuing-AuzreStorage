// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueuePump.cs" company="Simon Paramore">
// © 2017, Simon Paramore
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Furysoft.Queuing.AzureStorage.Logic
{
    using System;
    using System.Threading;
    using Core;
    using Interfaces;
    using Interfaces.Pump;
    using JetBrains.Annotations;
    using Microsoft.WindowsAzure.Storage.Queue;

    /// <summary>
    /// The Queue Pump
    /// </summary>
    /// <seealso cref="IQueuePump" />
    public sealed class QueuePump : IQueuePump
    {
        /// <summary>
        /// The message serializer
        /// </summary>
        [NotNull]
        private readonly IMessageSerializer messageSerializer;

        /// <summary>
        /// The pump processor
        /// </summary>
        [NotNull]
        private readonly IPumpProcessor pumpProcessor;

        /// <summary>
        /// The cancellation token source
        /// </summary>
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        /// <summary>
        /// Initializes a new instance of the <see cref="QueuePump" /> class.
        /// </summary>
        /// <param name="pumpProcessor">The pump processor.</param>
        /// <param name="messageSerializer">The message serializer.</param>
        public QueuePump(
            [NotNull] IPumpProcessor pumpProcessor,
            [NotNull] IMessageSerializer messageSerializer)
        {
            this.pumpProcessor = pumpProcessor;
            this.messageSerializer = messageSerializer;

            pumpProcessor.BatchSubmitted += (sender, i) => this.BatchSubmitted?.Invoke(sender, i);
            pumpProcessor.BufferEmpty += (sender, i) => this.BufferEmpty?.Invoke(sender, i);
        }

        /// <summary>
        /// Occurs when [batch submitted].
        /// </summary>
        public event EventHandler<int> BatchSubmitted;

        /// <summary>
        /// Occurs when [buffer empty].
        /// </summary>
        public event EventHandler BufferEmpty;

        /// <summary>Adds the message.</summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="message">The message.</param>
        public void AddMessage<TEntity>(TEntity message)
            where TEntity : class
        {
            var serializeMessage = this.messageSerializer.SerializeMessage(message);

            var cloudQueueMessage = new CloudQueueMessage(serializeMessage);

            this.pumpProcessor.AddMessage(cloudQueueMessage);
        }

        /// <summary>Starts the Queue Pump.</summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        public void Start(CancellationToken cancellationToken = default(CancellationToken))
        {
            var token = this.cancellationTokenSource.Token;
            this.pumpProcessor.Start(token);
        }

        /// <summary>
        /// Stops the queue pump.
        /// </summary>
        public void Stop()
        {
            this.cancellationTokenSource.Cancel();
            this.cancellationTokenSource = new CancellationTokenSource();
        }
    }
}