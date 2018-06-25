// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueueMessageSerializerResponse.cs" company="Simon Paramore">
// © 2017, Simon Paramore
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Furysoft.Queuing.AzureStorage.Entities
{
    using System.Collections.Generic;
    using Microsoft.WindowsAzure.Storage.Queue;
    using Versioning;

    /// <summary>
    /// The Queue Message Serializer Response
    /// </summary>
    internal sealed class QueueMessageSerializerResponse
    {
        /// <summary>
        /// Gets or sets the queue messages.
        /// </summary>
        public IEnumerable<CloudQueueMessage> QueueMessages { get; set; }

        /// <summary>
        /// Gets or sets the unused messages.
        /// </summary>
        public IEnumerable<VersionedMessage> UnusedMessages { get; set; }
    }
}