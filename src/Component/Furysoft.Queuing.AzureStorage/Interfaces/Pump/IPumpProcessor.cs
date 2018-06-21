// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPumpProcessor.cs" company="Simon Paramore">
// © 2017, Simon Paramore
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Furysoft.Queuing.AzureStorage.Interfaces.Pump
{
    using System.Threading;
    using Microsoft.WindowsAzure.Storage.Queue;

    /// <summary>
    /// The Pump Processor Interface
    /// </summary>
    public interface IPumpProcessor
    {
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