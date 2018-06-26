// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MockQueueWrapper.cs" company="Simon Paramore">
// © 2017, Simon Paramore
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Furysoft.Queuing.AzureStorage.Tests.Integration.Mocks
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Interfaces;
    using Microsoft.WindowsAzure.Storage.Queue;

    /// <summary>
    /// The Mock Queue Wrapper
    /// </summary>
    public sealed class MockQueueWrapper : IQueueWrapper
    {
        /// <summary>
        /// The local queue
        /// </summary>
        private readonly ConcurrentQueue<CloudQueueMessage> localQueue = new ConcurrentQueue<CloudQueueMessage>();

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns>The <see cref="CloudQueueMessage"/></returns>
        public CloudQueueMessage Get()
        {
            this.localQueue.TryDequeue(out var val);
            return val;
        }

        /// <summary>
        /// Submits the message asynchronous.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The <see cref="System.Threading.Tasks.Task" />
        /// </returns>
        public Task SubmitMessageAsync(CloudQueueMessage message, CancellationToken cancellationToken = default(CancellationToken))
        {
            this.localQueue.Enqueue(message);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Submits the messages asynchronous.
        /// </summary>
        /// <param name="messages">The messages.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The <see cref="System.Threading.Tasks.Task" />
        /// </returns>
        public async Task SubmitMessagesAsync(IEnumerable<CloudQueueMessage> messages, CancellationToken cancellationToken = default(CancellationToken))
        {
            foreach (var cloudQueueMessage in messages)
            {
                await this.SubmitMessageAsync(cloudQueueMessage, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}