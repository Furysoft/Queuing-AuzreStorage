// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessageSerializer.cs" company="Simon Paramore">
// © 2017, Simon Paramore
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Furysoft.Queuing.AzureStorage.Interfaces
{
    /// <summary>
    /// The Message Serializer
    /// </summary>
    public interface IMessageSerializer
    {
        /// <summary>
        /// Serializes the message.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns>The <see cref="string"/></returns>
        string SerializeMessage<TEntity>(TEntity source)
            where TEntity : class;
    }
}