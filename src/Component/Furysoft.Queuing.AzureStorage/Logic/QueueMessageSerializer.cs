// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueueMessageSerializer.cs" company="Email Hippo Ltd">
//   © Email Hippo Ltd
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Furysoft.Queuing.AzureStorage.Logic
{
    using System.Collections.Generic;
    using System.Linq;
    using Entities;
    using Entities.Configuration;
    using Helpers;
    using Interfaces;
    using JetBrains.Annotations;
    using Microsoft.WindowsAzure.Storage.Queue;
    using Versioning;

    /// <summary>
    /// The Queue Message Serializer
    /// </summary>
    internal sealed class QueueMessageSerializer : IQueueMessageSerializer
    {
        /// <summary>
        /// The batch settings
        /// </summary>
        [NotNull]
        private readonly BatchSettings batchSettings;

        /// <summary>
        /// The message serializer
        /// </summary>
        [NotNull]
        private readonly IMessageSerializer messageSerializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueMessageSerializer" /> class.
        /// </summary>
        /// <param name="batchSettings">The batch settings.</param>
        /// <param name="messageSerializer">The message serializer.</param>
        public QueueMessageSerializer(
            [NotNull] BatchSettings batchSettings,
            [NotNull] IMessageSerializer messageSerializer)
        {
            this.batchSettings = batchSettings;
            this.messageSerializer = messageSerializer;
        }

        /// <summary>
        /// Gets the queue messages.
        /// </summary>
        /// <param name="versionedMessages">The versioned messages.</param>
        /// <returns>The list of <see cref="CloudQueueMessage"/></returns>
        public QueueMessageSerializerResponse GetQueueMessages(IEnumerable<VersionedMessage> versionedMessages)
        {
            var messageList = versionedMessages.ToList();

            var batchSize = this.batchSettings.MaxMessagesPerQueueMessage;
            var maxMessages = this.batchSettings.MaxQueueMessagesPerSchedule;
            var totalMessages = batchSize * maxMessages;

            var shouldBatch = batchSize > 1;

            var takeMessages = messageList.Take(totalMessages).ToList();
            var skipMessages = messageList.Skip(totalMessages).ToList();

            var queueMessages = shouldBatch
                ? this.GetBatchedQueueMessages(takeMessages, batchSize)
                : this.GetNonBatchedQueueMessages(takeMessages);

            return new QueueMessageSerializerResponse
            {
                QueueMessages = queueMessages,
                UnusedMessages = skipMessages
            };
        }

        /// <summary>
        /// Gets the versioned message.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns>The <see cref="VersionedMessage"/></returns>
        public VersionedMessage GetVersionedMessage<TEntity>(TEntity entity)
            where TEntity : class
        {
            return this.messageSerializer.SerializeToVersionedMessage(entity);
        }

        /// <summary>
        /// Gets the batched queue messages.
        /// </summary>
        /// <param name="messages">The messages.</param>
        /// <param name="batchSize">Size of the batch.</param>
        /// <returns>The List of <see cref="CloudQueueMessage"/></returns>
        private List<CloudQueueMessage> GetBatchedQueueMessages(List<VersionedMessage> messages, int batchSize)
        {
            var batchedVersionedMessages = messages.BatchMessages(batchSize).ToList();

            var rtn = new List<CloudQueueMessage>();
            foreach (var batchedVersionedMessage in batchedVersionedMessages)
            {
                var body = this.messageSerializer.SerializeToString(batchedVersionedMessage);
                var message = new CloudQueueMessage(body);
                rtn.Add(message);
            }

            return rtn;
        }

        /// <summary>
        /// Gets the non batched queue messages.
        /// </summary>
        /// <param name="messages">The messages.</param>
        /// <returns>The List of <see cref="CloudQueueMessage"/></returns>
        private List<CloudQueueMessage> GetNonBatchedQueueMessages(List<VersionedMessage> messages)
        {
            var rtn = new List<CloudQueueMessage>();
            foreach (var versionedMessage in messages)
            {
                var body = this.messageSerializer.SerializeToString(versionedMessage);
                var message = new CloudQueueMessage(body);
                rtn.Add(message);
            }

            return rtn;
        }
    }
}