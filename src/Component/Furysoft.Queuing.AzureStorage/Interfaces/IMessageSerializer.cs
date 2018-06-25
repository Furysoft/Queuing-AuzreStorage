// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessageSerializer.cs" company="Simon Paramore">
// © 2017, Simon Paramore
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Furysoft.Queuing.AzureStorage.Interfaces
{
    using Versioning;

    /// <summary>
    /// The Message Serializer
    /// </summary>
    internal interface IMessageSerializer
    {
        /// <summary>
        /// Serializes the message.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>
        /// The <see cref="string" />
        /// </returns>
        string SerializeToString(VersionedMessage source);

        /// <summary>
        /// Serializes to string.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>
        /// The <see cref="string" />
        /// </returns>
        string SerializeToString(BatchedVersionedMessage source);

        /// <summary>
        /// Serializes to versioned message.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns>The <see cref="VersionedMessage"/></returns>
        VersionedMessage SerializeToVersionedMessage<TEntity>(TEntity source)
            where TEntity : class;
    }
}