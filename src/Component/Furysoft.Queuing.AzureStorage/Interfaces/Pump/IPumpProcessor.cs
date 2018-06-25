// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPumpProcessor.cs" company="Simon Paramore">
// © 2017, Simon Paramore
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Furysoft.Queuing.AzureStorage.Interfaces.Pump
{
    using System;
    using System.Threading;

    /// <summary>
    /// The Pump Processor Interface
    /// </summary>
    internal interface IPumpProcessor
    {
        /// <summary>
        /// Occurs when [batch submitted].
        /// </summary>
        event EventHandler<int> BatchSubmitted;

        /// <summary>
        /// Occurs when [buffer empty].
        /// </summary>
        event EventHandler BufferEmpty;

        /// <summary>
        /// Starts this instance.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        void Run(CancellationToken cancellationToken);
    }
}