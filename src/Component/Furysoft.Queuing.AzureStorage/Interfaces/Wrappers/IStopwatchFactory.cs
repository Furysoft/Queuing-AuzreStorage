// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IStopwatchFactory.cs" company="Simon Paramore">
// © 2017, Simon Paramore
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Furysoft.Queuing.AzureStorage.Interfaces.Wrappers
{
    /// <summary>
    /// The Stopwatch Factory
    /// </summary>
    internal interface IStopwatchFactory
    {
        /// <summary>
        /// Starts the new.
        /// </summary>
        /// <returns>The <see cref="IStopwatch"/></returns>
        IStopwatch StartNew();
    }
}