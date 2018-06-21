// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueueWrapperTests.cs" company="Simon Paramore">
// © 2017, Simon Paramore
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Furysoft.Queuing.AzureStorage.Tests.Integration
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Entities;
    using Logic;
    using Microsoft.WindowsAzure.Storage.Queue;
    using NUnit.Framework;
    using Serializers;
    using Serializers.Entities;
    using Serializers.Versioning;
    using TestHelpers;

    /// <summary>
    /// The Queue Wrapper Tests
    /// </summary>
    [Explicit]
    [TestFixture]
    public sealed class QueueWrapperTests : TestBase
    {
        /// <summary>
        /// The test prefix
        /// </summary>
        private const string TestPrefix = "eea5564a";

        /// <summary>
        /// The test queue name
        /// </summary>
        private string testQueueName;

        /// <summary>
        /// Tears down.
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            await StorageHelpers.DeletePrefixed(TestPrefix).ConfigureAwait(false);
        }

        /// <summary>
        /// Sets up.
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        [SetUp]
        public async Task SetUp()
        {
            this.testQueueName = StorageHelpers.GetRandomQueueName(TestPrefix);

            var queueReference = StorageHelpers.GetQueueReference(this.testQueueName);
            await queueReference.DeleteIfExistsAsync().ConfigureAwait(false);
            await queueReference.CreateIfNotExistsAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Submits the message asynchronous when message submitted expect on the queue.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task SubmitMessageAsync_WhenMessageSubmitted_ExpectOnTheQueue()
        {
            // Arrange
            var queueEndpoint = new QueueEndpoint
            {
                ConnectionString = StorageHelpers.ConnectionString,
                QueueName = this.testQueueName
            };

            var queueWrapper = new QueueWrapper(queueEndpoint);

            var data = new TestEntity { Data = "Test" }.SerializeToVersionedMessage(SerializerType.ProtocolBuffers).SerializeToString();
            var cloudMessage = new CloudQueueMessage(data);

            // Act
            var stopwatch = Stopwatch.StartNew();
            await queueWrapper.SubmitMessageAsync(cloudMessage, CancellationToken.None).ConfigureAwait(false);
            stopwatch.Stop();

            // Assert
            this.WriteTimeElapsed(stopwatch);

            var cloudQueueMessage = await StorageHelpers.GetQueueReference(this.testQueueName).GetMessageAsync().ConfigureAwait(false);

            Assert.That(cloudQueueMessage, Is.Not.Null);

            var testEntity = cloudQueueMessage
                .AsString
                .DeserializeToVersionedMessage(SerializerType.ProtocolBuffers)
                .Data
                .Deserialize<TestEntity>();

            Assert.That(testEntity, Is.Not.Null);
            Assert.That(testEntity.Data, Is.EqualTo("Test"));
        }

        /// <summary>
        /// Submits the messages asynchronous when messages submitted expect on the queue.
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        [Test]
        public async Task SubmitMessagesAsync_WhenMessagesSubmitted_ExpectOnTheQueue()
        {
            // Arrange
            var queueEndpoint = new QueueEndpoint
            {
                ConnectionString = StorageHelpers.ConnectionString,
                QueueName = this.testQueueName
            };

            var queueWrapper = new QueueWrapper(queueEndpoint);

            var data = new List<CloudQueueMessage>();
            for (var i = 0; i < 10; i++)
            {
                var testEntity = new TestEntity { Data = $"Test{i}" }.SerializeToString();
                var queueMessage = new CloudQueueMessage(testEntity);
                data.Add(queueMessage);
            }

            // Act
            var stopwatch = Stopwatch.StartNew();
            await queueWrapper.SubmitMessagesAsync(data, CancellationToken.None).ConfigureAwait(false);
            stopwatch.Stop();

            // Assert
            this.WriteTimeElapsed(stopwatch);

            var cloudQueueMessage = await StorageHelpers.GetQueueReference(this.testQueueName).GetMessagesAsync(10).ConfigureAwait(false);
            var list = cloudQueueMessage.ToList();

            Assert.That(list, Is.Not.Null);
            Assert.That(list.Count, Is.EqualTo(10));
        }

        /// <summary>
        /// Tears down.
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        [TearDown]
        public async Task TearDown()
        {
            var queueReference = StorageHelpers.GetQueueReference(this.testQueueName);
            await queueReference.DeleteIfExistsAsync().ConfigureAwait(false);
        }
    }
}