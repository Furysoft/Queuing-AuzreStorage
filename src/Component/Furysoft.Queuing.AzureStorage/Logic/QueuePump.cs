// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueuePump.cs" company="Simon Paramore">
// © 2017, Simon Paramore
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Furysoft.Queuing.AzureStorage.Logic
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using Core;
    using Interfaces.Pump;
    using JetBrains.Annotations;

    /// <summary>
    /// The Queue Pump
    /// </summary>
    /// <seealso cref="IQueuePump" />
    public sealed class QueuePump : IQueuePump
    {
        /// <summary>
        /// The buffer
        /// </summary>
        [NotNull]
        private readonly IBuffer buffer;

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
        /// <param name="buffer">The buffer.</param>
        /// <param name="pumpProcessor">The pump processor.</param>
        internal QueuePump(
            [NotNull] IBuffer buffer,
            [NotNull] IPumpProcessor pumpProcessor)
        {
            this.buffer = buffer;
            this.pumpProcessor = pumpProcessor;

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
            this.buffer.AddMessage(message);
        }

        /// <summary>Starts the Queue Pump.</summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        public void Start(CancellationToken cancellationToken = default(CancellationToken))
        {
            var token = this.cancellationTokenSource.Token;
            this.pumpProcessor.Run(token);
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