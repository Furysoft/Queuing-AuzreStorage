// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueueWrapper.cs" company="Simon Paramore">
// © 2017, Simon Paramore
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Furysoft.Queuing.AzureStorage.Logic
{
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
        /// <param name="connectionString">The connection string.</param>
        /// <param name="queueName">Name of the queue.</param>
        public QueueWrapper([NotNull] string connectionString, [NotNull] string queueName)
        {
            var cloudStorageAccount = CloudStorageAccount.Parse(connectionString);

            var queueClient = cloudStorageAccount.CreateCloudQueueClient();

            this.cloudQueue = queueClient.GetQueueReference(queueName);
        }
    }
}