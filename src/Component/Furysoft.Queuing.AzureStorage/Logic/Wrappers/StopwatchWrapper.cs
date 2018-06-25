// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StopwatchWrapper.cs" company="Simon Paramore">
// © 2017, Simon Paramore
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Furysoft.Queuing.AzureStorage.Logic.Wrappers
{
    using System;
    using System.Diagnostics;
    using Interfaces.Wrappers;
    using JetBrains.Annotations;

    /// <summary>
    /// The Stopwatch Wrapper
    /// </summary>
    internal sealed class StopwatchWrapper : IStopwatch
    {
        /// <summary>
        /// The stopwatch
        /// </summary>
        [NotNull]
        private readonly Stopwatch stopwatch;

        /// <summary>
        /// Initializes a new instance of the <see cref="StopwatchWrapper" /> class.
        /// </summary>
        /// <param name="stopwatch">The stopwatch.</param>
        public StopwatchWrapper([NotNull] Stopwatch stopwatch)
        {
            this.stopwatch = stopwatch;
        }

        /// <summary>
        /// Gets the elapsed.
        /// </summary>
        public TimeSpan Elapsed => this.stopwatch.Elapsed;

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            this.stopwatch.Stop();
        }
    }
}