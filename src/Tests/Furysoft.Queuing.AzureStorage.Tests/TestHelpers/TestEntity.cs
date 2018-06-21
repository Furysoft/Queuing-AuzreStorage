// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestEntity.cs" company="Simon Paramore">
// © 2017, Simon Paramore
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Furysoft.Queuing.AzureStorage.Tests.TestHelpers
{
    using System.Runtime.Serialization;
    using Versioning;

    /// <summary>
    /// The Test Entity
    /// </summary>
    [DataContract]
    [DtoVersion(typeof(TestEntity), 1, 0, 0)]
    public sealed class TestEntity
    {
        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        [DataMember(Name = nameof(Data), Order = 1)]
        public string Data { get; set; }
    }
}