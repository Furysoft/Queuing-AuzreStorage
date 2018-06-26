// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageSerializerTests.cs" company="Simon Paramore">
// © 2017, Simon Paramore
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Furysoft.Queuing.AzureStorage.Tests.Unit.MessageSerializer
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Entities.Configuration;
    using Logic;
    using NUnit.Framework;
    using Serializers;
    using Serializers.Entities;
    using Serializers.Versioning;
    using TestHelpers;
    using Versioning;

    /// <summary>
    /// The Message Serializer Tests
    /// </summary>
    [TestFixture]
    public sealed class MessageSerializerTests : TestBase
    {
        /// <summary>
        /// Serializes to string when batched versioned message expect correct string.
        /// </summary>
        [Test]
        public void SerializeToString_WhenBatchedVersionedMessage_ExpectCorrectString()
        {
            // Arrange
            var serializerSettings = new SerializerSettings { SerializerType = SerializerType.Json };
            var messageSerializer = new MessageSerializer(serializerSettings);

            var testEntity = new TestEntity { Data = "val" }.SerializeToVersionedMessage(SerializerType.Json);

            var batchedVersionedMessage = new BatchedVersionedMessage { Messages = new List<VersionedMessage> { testEntity } };

            // Act
            var stopwatch = Stopwatch.StartNew();
            var serializeToString = messageSerializer.SerializeToString(batchedVersionedMessage);
            stopwatch.Stop();

            // Assert
            this.WriteTimeElapsed(stopwatch);

            Assert.That(serializeToString, Is.EqualTo("{\"m\":[{\"v\":{\"T\":\"TestEntity\",\"Ma\":1,\"Mi\":0,\"P\":0,\"VT\":0,\"VTV\":0},\"d\":\"{\\\"Data\\\":\\\"val\\\"}\"}]}"));
        }

        /// <summary>
        /// Serializes to string when batched versioned message with proto buf expect data.
        /// </summary>
        [Test]
        public void SerializeToString_WhenBatchedVersionedMessageWithProtoBuf_ExpectData()
        {
            // Arrange
            var serializerSettings = new SerializerSettings { SerializerType = SerializerType.ProtocolBuffers };
            var messageSerializer = new MessageSerializer(serializerSettings);

            var testEntity = new TestEntity { Data = "val" }.SerializeToVersionedMessage(SerializerType.ProtocolBuffers);

            var batchedVersionedMessage = new BatchedVersionedMessage { Messages = new List<VersionedMessage> { testEntity } };

            // Act
            var stopwatch = Stopwatch.StartNew();
            var serializeToString = messageSerializer.SerializeToString(batchedVersionedMessage);
            stopwatch.Stop();

            // Assert
            this.WriteTimeElapsed(stopwatch);

            var deserialize = serializeToString.Deserialize<BatchedVersionedMessage>();

            Assert.That(deserialize, Is.Not.Null);

            var message = deserialize.Messages.First();
            Assert.That(message, Is.Not.Null);
            Assert.That(message.Version, Is.EqualTo(new DtoVersion(typeof(TestEntity), 1, 0, 0)));
        }

        /// <summary>
        /// Serializes to string when versioned message expect correct string.
        /// </summary>
        [Test]
        public void SerializeToString_WhenVersionedMessage_ExpectCorrectString()
        {
            // Arrange
            var serializerSettings = new SerializerSettings { SerializerType = SerializerType.Json };
            var messageSerializer = new MessageSerializer(serializerSettings);

            var testEntity = new TestEntity { Data = "val" }.SerializeToVersionedMessage(SerializerType.Json);

            // Act
            var stopwatch = Stopwatch.StartNew();
            var serializeToString = messageSerializer.SerializeToString(testEntity);
            stopwatch.Stop();

            // Assert
            this.WriteTimeElapsed(stopwatch);

            Assert.That(serializeToString, Is.EqualTo("{\"v\":{\"T\":\"TestEntity\",\"Ma\":1,\"Mi\":0,\"P\":0,\"VT\":0,\"VTV\":0},\"d\":\"{\\\"Data\\\":\\\"val\\\"}\"}"));
        }
    }
}