// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Queue.cs" company="Simon Paramore">
// © 2017, Simon Paramore
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Furysoft.Queuing.AzureStorage
{
    using Core;
    using JetBrains.Annotations;

    /// <summary>
    /// The Queue
    /// </summary>
    public sealed class Queue : IQueue
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Queue"/> class.
        /// </summary>
        /// <param name="queuePump">The queue pump.</param>
        public Queue([NotNull] IQueuePump queuePump)
        {
            this.QueuePump = queuePump;
        }

        /// <summary>Gets the queue pump.</summary>
        [NotNull]
        public IQueuePump QueuePump { get; }

        /// <summary>Gets the queue receiver.</summary>
        public IQueueReceiver QueueReceiver { get; }

        /// <summary>Gets the queue submitter.</summary>
        public IQueueSubmitter QueueSubmitter { get; }

        /// <summary>Gets the queue subscriber.</summary>
        public IQueueSubscriber QueueSubscriber { get; }
    }
}