// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StopwatchFactory.cs" company="Simon Paramore">
// © 2017, Simon Paramore
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Furysoft.Queuing.AzureStorage.Logic.Wrappers
{
    using System.Diagnostics;
    using Interfaces.Wrappers;

    /// <summary>
    /// The Stopwatch Factory
    /// </summary>
    internal sealed class StopwatchFactory : IStopwatchFactory
    {
        /// <summary>
        /// Starts the new.
        /// </summary>
        /// <returns>The <see cref="IStopwatch"/></returns>
        public IStopwatch StartNew()
        {
            return new StopwatchWrapper(Stopwatch.StartNew());
        }
    }
}