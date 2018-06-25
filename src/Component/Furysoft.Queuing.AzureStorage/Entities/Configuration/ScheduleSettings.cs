// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScheduleSettings.cs" company="Simon Paramore">
// © 2017, Simon Paramore
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Furysoft.Queuing.AzureStorage.Entities.Configuration
{
    using System;

    /// <summary>
    /// The Schedule Settings
    /// </summary>
    public sealed class ScheduleSettings
    {
        /// <summary>
        /// Gets or sets the Timespan to limit requests to.
        /// </summary>
        /// <remarks>
        /// Default 1 second
        /// </remarks>
        public TimeSpan ThrottleTime { get; set; } = TimeSpan.FromSeconds(1);
    }
}