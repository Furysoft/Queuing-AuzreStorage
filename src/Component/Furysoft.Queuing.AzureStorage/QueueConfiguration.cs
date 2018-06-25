// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueueConfiguration.cs" company="Simon Paramore">
// © 2017, Simon Paramore
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Furysoft.Queuing.AzureStorage
{
    using Entities.Configuration;

    /// <summary>
    /// The Queue Configuration
    /// </summary>
    public sealed class QueueConfiguration
    {
        /// <summary>
        /// Gets the pump configuration.
        /// </summary>
        public PumpConfiguration PumpConfiguration { get; } = new PumpConfiguration();

        /// <summary>
        /// Gets the serializer settings.
        /// </summary>
        public SerializerSettings SerializerSettings { get; } = new SerializerSettings();
    }
}