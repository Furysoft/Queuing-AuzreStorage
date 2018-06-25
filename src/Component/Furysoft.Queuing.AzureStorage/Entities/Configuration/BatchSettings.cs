// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BatchSettings.cs" company="Simon Paramore">
// © 2017, Simon Paramore
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Furysoft.Queuing.AzureStorage.Entities.Configuration
{
    using Versioning;

    /// <summary>
    /// The Batch Settings
    /// </summary>
    public sealed class BatchSettings
    {
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
    }
}