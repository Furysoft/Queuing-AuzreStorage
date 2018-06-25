// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IQueueMessageSerializer.cs" company="Simon Paramore">
// © 2017, Simon Paramore
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Furysoft.Queuing.AzureStorage.Interfaces
{
    using System.Collections.Generic;
    using Entities;
    using Microsoft.WindowsAzure.Storage.Queue;
    using Versioning;

    /// <summary>
    /// The Queue Message Serializer
    /// </summary>
    internal interface IQueueMessageSerializer
    {
        /// <summary>
        /// Gets the queue messages.
        /// </summary>
        /// <param name="versionedMessages">The versioned messages.</param>
        /// <returns>The list of <see cref="CloudQueueMessage"/></returns>
        QueueMessageSerializerResponse GetQueueMessages(IEnumerable<VersionedMessage> versionedMessages);

        /// <summary>
        /// Gets the versioned message.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns>The <see cref="VersionedMessage"/></returns>
        VersionedMessage GetVersionedMessage<TEntity>(TEntity entity)
            where TEntity : class;
    }
}