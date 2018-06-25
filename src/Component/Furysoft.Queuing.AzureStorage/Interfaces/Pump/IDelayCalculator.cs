// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDelayCalculator.cs" company="Simon Paramore">
// © 2017, Simon Paramore
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Furysoft.Queuing.AzureStorage.Interfaces.Pump
{
    using System;

    /// <summary>
    /// The Delay Calculator Interface
    /// </summary>
    internal interface IDelayCalculator
    {
        /// <summary>
        /// Gets the next delay.
        /// </summary>
        /// <param name="lastOperationTime">The last operation time.</param>
        /// <param name="minTimeBeforeNextRun">The minimum time before next run.</param>
        /// <returns>
        /// The <see cref="TimeSpan" />
        /// </returns>
        TimeSpan GetNextDelay(TimeSpan lastOperationTime, TimeSpan minTimeBeforeNextRun);
    }
}