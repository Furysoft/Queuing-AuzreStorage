// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IQueueWrapper.cs" company="Simon Paramore">
// © 2017, Simon Paramore
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Furysoft.Queuing.AzureStorage.Interfaces
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Storage.Queue;

    /// <summary>
    /// The Queue Wrapper Interface
    /// </summary>
    internal interface IQueueWrapper
    {
        /// <summary>
        /// Submits the message asynchronous.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The <see cref="Task" />
        /// </returns>
        Task SubmitMessageAsync(
            CloudQueueMessage message,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Submits the messages asynchronous.
        /// </summary>
        /// <param name="messages">The messages.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The <see cref="Task" />
        /// </returns>
        Task SubmitMessagesAsync(
            IEnumerable<CloudQueueMessage> messages,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}