// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueueEndpoint.cs" company="Simon Paramore">
// © 2017, Simon Paramore
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Furysoft.Queuing.AzureStorage.Entities
{
    /// <summary>
    /// The Queue Endpoint
    /// </summary>
    public sealed class QueueEndpoint
    {
        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the name of the queue.
        /// </summary>
        public string QueueName { get; set; }
    }
}