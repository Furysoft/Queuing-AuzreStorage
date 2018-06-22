// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPumpProcessor.cs" company="Simon Paramore">
// © 2017, Simon Paramore
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Furysoft.Queuing.AzureStorage.Interfaces.Pump
{
    using System;
    using System.Threading;
    using Microsoft.WindowsAzure.Storage.Queue;

    /// <summary>
    /// The Pump Processor Interface
    /// </summary>
    public interface IPumpProcessor
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
        /// Adds the message.
        /// </summary>
        /// <param name="message">The message.</param>
        void AddMessage(CloudQueueMessage message);

        /// <summary>
        /// Starts the specified cancellation token.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        void Start(CancellationToken cancellationToken);
    }
}