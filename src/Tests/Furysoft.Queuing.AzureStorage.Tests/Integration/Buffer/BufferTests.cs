// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BufferTests.cs" company="Email Hippo Ltd">
//   © Email Hippo Ltd
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Furysoft.Queuing.AzureStorage.Tests.Integration.Buffer
{
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Entities.Configuration;
    using Logic;
    using Mocks;
    using NUnit.Framework;
    using Serializers;
    using Serializers.Entities;
    using TestHelpers;
    using Versioning;
    using Buffer = Logic.Pump.Buffer;

    /// <summary>
    /// The Buffer Tests
    /// </summary>
    [TestFixture]
    public sealed class BufferTests : TestBase
    {
        /// <summary>
        /// Processes the buffer asynchronous when messages on buffer expect sent to queue.
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        [Test]
        public async Task ProcessBufferAsync_WhenMessagesOnBuffer_ExpectSentToQueue()
        {
            // Arrange
            const SerializerType SerializerType = SerializerType.Json;

            var queueWrapper = new MockQueueWrapper();

            var batchSettings = new BatchSettings { MaxQueueMessagesPerSchedule = 10, MaxMessagesPerQueueMessage = 10 };

            var serializerSettings = new SerializerSettings { SerializerType = SerializerType };
            var messageSerializer = new MessageSerializer(serializerSettings);

            var queueMessageSerializer = new QueueMessageSerializer(batchSettings, messageSerializer);

            var buffer = new Buffer(this.LoggerFactory, queueWrapper, queueMessageSerializer);

            // Act
            var stopwatch = Stopwatch.StartNew();

            buffer.AddMessage(new TestEntity { Data = "d1" });
            buffer.AddMessage(new TestEntity { Data = "d2" });
            buffer.AddMessage(new TestEntity { Data = "d3" });

            await buffer.ProcessBufferAsync(CancellationToken.None).ConfigureAwait(false);

            stopwatch.Stop();

            // Assert
            this.WriteTimeElapsed(stopwatch);

            var cloudQueueMessage = queueWrapper.Get();

            Assert.That(cloudQueueMessage, Is.Not.Null);

            var asString = cloudQueueMessage.AsString;

            var batchedVersionedMessage = asString.Deserialize<BatchedVersionedMessage>(SerializerType);

            Assert.That(batchedVersionedMessage, Is.Not.Null);

            var messages = batchedVersionedMessage.Messages.ToList();
            Assert.That(messages.Count, Is.EqualTo(3));
            Assert.That(messages.All(r => r.Version == new DtoVersion(typeof(TestEntity), 1, 0, 0)), Is.True);
        }
    }
}