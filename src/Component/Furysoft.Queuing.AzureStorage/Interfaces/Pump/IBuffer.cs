// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBuffer.cs" company="Simon Paramore">
// © 2017, Simon Paramore
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Furysoft.Queuing.AzureStorage.Interfaces.Pump
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Entities.Buffer;
    using Versioning;

    /// <summary>
    /// The Pump Processor Interface
    /// </summary>
    internal interface IBuffer
    {
        /// <summary>
        /// Gets the buffer.
        /// </summary>
        IReadOnlyCollection<VersionedMessage> InternalBuffer { get; }

        /// <summary>
        /// Adds the message.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="message">The message.</param>
        void AddMessage<TEntity>(TEntity message)
            where TEntity : class;

        /// <summary>
        /// Processes the buffer asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="BufferProcessResponse"/></returns>
        Task<BufferProcessResponse> ProcessBufferAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}