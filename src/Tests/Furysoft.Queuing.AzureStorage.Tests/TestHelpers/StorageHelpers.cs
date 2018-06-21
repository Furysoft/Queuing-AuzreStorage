// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StorageHelpers.cs" company="Simon Paramore">
// © 2017, Simon Paramore
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Furysoft.Queuing.AzureStorage.Tests.TestHelpers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Queue;

    /// <summary>
    /// The Storage Helpers
    /// </summary>
    public static class StorageHelpers
    {
        /// <summary>
        /// The connection string
        /// </summary>
        public const string ConnectionString = "UseDevelopmentStorage=true;";

        /// <summary>
        /// Gets the queue reference.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <returns>The <see cref="CloudQueue"/></returns>
        public static CloudQueue GetQueueReference(string queueName)
        {
            var storageAccount = CloudStorageAccount.Parse(ConnectionString);
            var queueClient = storageAccount.CreateCloudQueueClient();

            return queueClient.GetQueueReference(queueName);
        }

        /// <summary>
        /// Deletes the prefixed.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <returns>The <see cref="Task"/></returns>
        public static async Task DeletePrefixed(string prefix)
        {
            var storageAccount = CloudStorageAccount.Parse(ConnectionString);
            var queueClient = storageAccount.CreateCloudQueueClient();
            var queueResultSegment = await queueClient.ListQueuesSegmentedAsync(prefix, null).ConfigureAwait(false);

            foreach (var cloudQueue in queueResultSegment.Results)
            {
                await cloudQueue.DeleteIfExistsAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Gets the random name of the queue.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <returns>The Unique Queue Name</returns>
        public static string GetRandomQueueName(string prefix)
        {
            return $"{prefix}-{Guid.NewGuid().ToString().Split('-').First().ToLowerInvariant()}";
        }
    }
}