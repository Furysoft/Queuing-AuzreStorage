// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DelayCalculator.cs" company="Simon Paramore">
// © 2017, Simon Paramore
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Furysoft.Queuing.AzureStorage.Logic.Pump
{
    using System;
    using Interfaces.Pump;

    /// <summary>
    /// The Delay Calculator
    /// </summary>
    internal sealed class DelayCalculator : IDelayCalculator
    {
        /// <summary>
        /// Gets the next delay.
        /// </summary>
        /// <param name="lastOperationTime">The last operation time.</param>
        /// <param name="minTimeBeforeNextRun">The minimum time before next run.</param>
        /// <returns>
        /// The <see cref="TimeSpan" />
        /// </returns>
        public TimeSpan GetNextDelay(TimeSpan lastOperationTime, TimeSpan minTimeBeforeNextRun)
        {
            if (lastOperationTime < minTimeBeforeNextRun)
            {
                return minTimeBeforeNextRun - lastOperationTime;
            }

            return TimeSpan.Zero;
        }
    }
}