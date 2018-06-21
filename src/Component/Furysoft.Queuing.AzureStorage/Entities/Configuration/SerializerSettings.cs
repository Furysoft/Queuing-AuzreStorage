// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializerSettings.cs" company="Simon Paramore">
// © 2017, Simon Paramore
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Furysoft.Queuing.AzureStorage.Entities.Configuration
{
    using Serializers.Entities;

    /// <summary>
    /// The Serializer Settings
    /// </summary>
    public sealed class SerializerSettings
    {
        /// <summary>
        /// Gets or sets the type of the serializer.
        /// </summary>
        public SerializerType SerializerType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [use versioned messages].
        /// </summary>
        public bool UseVersionedMessages { get; set; }
    }
}