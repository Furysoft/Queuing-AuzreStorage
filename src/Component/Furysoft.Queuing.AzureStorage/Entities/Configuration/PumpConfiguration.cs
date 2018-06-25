// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PumpConfiguration.cs" company="Simon Paramore">
// © 2017, Simon Paramore
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Furysoft.Queuing.AzureStorage.Entities.Configuration
{
    using System;
    using Versioning;

    /// <summary>
    /// The Pump Configuration
    /// </summary>
    public sealed class PumpConfiguration
    {
        /// <summary>
        /// Gets or sets the Timespan to limit requests to.
        /// </summary>
        /// <remarks>
        /// Default 1 second
        /// </remarks>
        public TimeSpan ThrottleTime { get; set; } = TimeSpan.FromSeconds(1);

        /// <summary>
        /// Gets or sets the Maximum number of requests that can be sent during the throttle time. 
        /// </summary>
        /// <remarks>
        /// Default 2000
        /// </remarks>>
        public int MaxRequestsPerThrottleTime { get; set; } = 2000;

        /// <summary>
        /// Gets or sets the number of queue messages to batch into a single <see cref="BatchedVersionedMessage"/>
        /// </summary>
        /// <remarks>
        /// Default 10
        /// </remarks>>
        public int MaxBatchSize { get; set; } = 10;
    }
}