// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueueWrapper.cs" company="Simon Paramore">
// © 2017, Simon Paramore
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Furysoft.Queuing.AzureStorage.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;
    using Entities;
    using Interfaces;
    using JetBrains.Annotations;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Queue;

    /// <summary>
    /// The Queue Wrapper
    /// </summary>
    public sealed class QueueWrapper : IQueueWrapper
    {
        /// <summary>
        /// The cloud queue client
        /// </summary>
        [NotNull]
        private readonly CloudQueue cloudQueue;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueWrapper" /> class.
        /// </summary>
        /// <param name="queueEndpoint">The queue endpoint.</param>
        public QueueWrapper([NotNull] QueueEndpoint queueEndpoint)
        {
            var cloudStorageAccount = CloudStorageAccount.Parse(queueEndpoint.ConnectionString);
            var queueClient = cloudStorageAccount.CreateCloudQueueClient();
            this.cloudQueue = queueClient.GetQueueReference(queueEndpoint.QueueName);
        }

        /// <summary>
        /// Submits the message asynchronous.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The <see cref="Task" />
        /// </returns>
        public async Task SubmitMessageAsync(
            CloudQueueMessage message,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            await this.cloudQueue.AddMessageAsync(message).ConfigureAwait(false);
        }

        /// <summary>
        /// Submits the messages asynchronous.
        /// </summary>
        /// <param name="messages">The messages.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The <see cref="Task" />
        /// </returns>
        public async Task SubmitMessagesAsync(
            IEnumerable<CloudQueueMessage> messages,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var executionDatafileBlockOptions = new ExecutionDataflowBlockOptions
            {
                CancellationToken = cancellationToken,
                MaxDegreeOfParallelism = 100
            };

            var actionBlock = new ActionBlock<CloudQueueMessage>(
                async entity => { await this.SubmitMessageAsync(entity, cancellationToken).ConfigureAwait(false); },
                executionDatafileBlockOptions);

            foreach (var message in messages)
            {
                actionBlock.Post(message);
            }

            actionBlock.Complete();

            await actionBlock.Completion.ConfigureAwait(false);
        }
    }
}