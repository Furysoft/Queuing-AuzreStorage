// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageSerializer.cs" company="Simon Paramore">
// © 2017, Simon Paramore
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Furysoft.Queuing.AzureStorage.Logic
{
    using Entities.Configuration;
    using Interfaces;
    using JetBrains.Annotations;
    using Serializers;
    using Serializers.Versioning;

    /// <summary>
    /// The Message Serializer
    /// </summary>
    public sealed class MessageSerializer : IMessageSerializer
    {
        /// <summary>
        /// The serializer settings
        /// </summary>
        [NotNull]
        private readonly SerializerSettings serializerSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageSerializer"/> class.
        /// </summary>
        /// <param name="serializerSettings">The serializer settings.</param>
        public MessageSerializer([NotNull] SerializerSettings serializerSettings)
        {
            this.serializerSettings = serializerSettings;
        }

        /// <summary>
        /// Serializes the message.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns>The <see cref="string"/></returns>
        public string SerializeMessage<TEntity>(TEntity source)
            where TEntity : class
        {
            var serializerType = this.serializerSettings.SerializerType;
            var useVersionedMessages = this.serializerSettings.UseVersionedMessages;

            if (useVersionedMessages)
            {
                var vm = source.SerializeToVersionedMessage(serializerType);
                var rtn = vm.SerializeToString(serializerType);
                return rtn;
            }

            return source.SerializeToString(serializerType);
        }
    }
}