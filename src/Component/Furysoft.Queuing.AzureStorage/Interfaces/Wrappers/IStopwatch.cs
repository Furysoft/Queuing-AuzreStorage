// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IStopwatch.cs" company="Simon Paramore">
// © 2017, Simon Paramore
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Furysoft.Queuing.AzureStorage.Interfaces.Wrappers
{
    using System;

    /// <summary>
    /// The Stopwatch Interface
    /// </summary>
    internal interface IStopwatch
    {
        /// <summary>
        /// Gets the elapsed.
        /// </summary>
        TimeSpan Elapsed { get; }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        void Stop();
    }
}