// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueueConfiguration.cs" company="Simon Paramore">
// © 2017, Simon Paramore
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Furysoft.Queuing.AzureStorage
{
    using System;
    using JetBrains.Annotations;
    using Serializers.Entities;
    using Versioning;

    /// <summary>
    /// The Queue Configuration
    /// </summary>
    public sealed class QueueConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueueConfiguration" /> class.
        /// </summary>
        /// <param name="queueConnectionString">The queue connection string.</param>
        /// <param name="queueName">Name of the queue.</param>
        public QueueConfiguration([NotNull] string queueConnectionString, [NotNull] string queueName)
        {
            this.QueueConnectionString = queueConnectionString;
            this.QueueName = queueName;
        }

        /// <summary>
        /// Gets or sets the maximum messages per queue message.
        /// </summary>
        /// <value>
        /// The maximum messages per queue message.
        /// </value>
        /// <remarks>
        /// <para>
        /// If > 1, will use a <see cref="BatchedVersionedMessage"/>
        /// If 1, will use a <see cref="VersionedMessage"/>
        /// </para>
        /// <para>Default : 1</para>
        /// </remarks>
        public int MaxMessagesPerQueueMessage { get; set; } = 1;

        /// <summary>
        /// Gets or sets the max number of messages that can be submitted to the queue per cycle of the pump processor.
        /// </summary>
        /// <value>
        /// The maximum queue messages per schedule.
        /// </value>
        /// <remarks>
        /// <para>Based off the limitations of the underlying queue technology</para>
        /// <para>Default: 2000</para>
        /// </remarks>
        public int MaxQueueMessagesPerSchedule { get; set; } = 2000;

        /// <summary>
        /// Gets or sets the queue connection string.
        /// </summary>
        public string QueueConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the name of the queue.
        /// </summary>
        public string QueueName { get; set; }

        /// <summary>
        /// Gets or sets the type of the serializer.
        /// </summary>
        /// <remarks>
        /// Default: SerializerType.ProtocolBuffers
        /// </remarks>
        public SerializerType SerializerType { get; set; } = SerializerType.ProtocolBuffers;

        /// <summary>
        /// Gets or sets the Timespan to limit requests to.
        /// </summary>
        /// <remarks>
        /// Default 1 second
        /// </remarks>
        public TimeSpan ThrottleTime { get; set; } = TimeSpan.FromSeconds(1);
    }
}