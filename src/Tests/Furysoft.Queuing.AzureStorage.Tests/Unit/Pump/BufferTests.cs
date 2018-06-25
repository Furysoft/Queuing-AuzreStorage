// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BufferTests.cs" company="Simon Paramore">
// © 2017, Simon Paramore
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Furysoft.Queuing.AzureStorage.Tests.Unit.Pump
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Entities;
    using Interfaces;
    using Logic.Pump;
    using Microsoft.WindowsAzure.Storage.Queue;
    using Moq;
    using NUnit.Framework;
    using Versioning;

    /// <summary>
    /// The Buffer Tests
    /// </summary>
    [TestFixture]
    public sealed class BufferTests : TestBase
    {
        /// <summary>
        /// Adds the message when message added expect on buffer.
        /// </summary>
        [Test]
        public void AddMessage_WhenMessageAdded_ExpectOnBuffer()
        {
            // Arrange
            var mockQueueWrapper = new Mock<IQueueWrapper>();
            var mockQueueMessageSerializer = new Mock<IQueueMessageSerializer>();

            mockQueueMessageSerializer.Setup(r => r.GetVersionedMessage(It.IsAny<string>()))
                .Returns(new VersionedMessage());

            var buffer = new Buffer(this.LoggerFactory, mockQueueWrapper.Object, mockQueueMessageSerializer.Object);

            // Act
            var stopwatch = Stopwatch.StartNew();
            buffer.AddMessage("data");
            stopwatch.Stop();

            // Assert
            this.WriteTimeElapsed(stopwatch);

            var versionedMessages = buffer.InternalBuffer.ToList();

            Assert.That(versionedMessages.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Processes the buffer asynchronous when nothing in buffer expect returns.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task ProcessBufferAsync_WhenNothingInBuffer_ExpectReturns()
        {
            // Arrange
            var mockQueueWrapper = new Mock<IQueueWrapper>();
            var mockQueueMessageSerializer = new Mock<IQueueMessageSerializer>();

            var initialBag = new ConcurrentBag<VersionedMessage>();

            var buffer = new Buffer(
                initialBag,
                this.LoggerFactory,
                mockQueueWrapper.Object,
                mockQueueMessageSerializer.Object);

            // Act
            var stopwatch = Stopwatch.StartNew();
            var bufferProcessResponse = await buffer.ProcessBufferAsync(CancellationToken.None).ConfigureAwait(false);
            stopwatch.Stop();

            // Assert
            this.WriteTimeElapsed(stopwatch);

            Assert.That(bufferProcessResponse, Is.Not.Null);
            Assert.That(bufferProcessResponse.Processed, Is.EqualTo(0));
            Assert.That(bufferProcessResponse.Remaining, Is.EqualTo(0));

            mockQueueWrapper.Verify(
                r => r.SubmitMessagesAsync(It.IsAny<IEnumerable<CloudQueueMessage>>(), It.IsAny<CancellationToken>()),
                Times.Never());
        }

        /// <summary>
        /// Processes the buffer asynchronous when entire buffer processed expect messages submitted.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task ProcessBufferAsync_WhenEntireBufferProcessed_ExpectMessagesSubmitted()
        {
            // Arrange
            var mockQueueWrapper = new Mock<IQueueWrapper>();
            var mockQueueMessageSerializer = new Mock<IQueueMessageSerializer>();

            mockQueueMessageSerializer.Setup(r => r.GetQueueMessages(It.IsAny<IEnumerable<VersionedMessage>>()))
                .Returns(
                    (IEnumerable<VersionedMessage> messages) =>
                    {
                        return new QueueMessageSerializerResponse
                        {
                            QueueMessages = messages.Select(r => new CloudQueueMessage("content")),
                            UnusedMessages = new List<VersionedMessage>()
                        };
                    });

            var callback = default(List<CloudQueueMessage>);
            mockQueueWrapper
                .Setup(r => r.SubmitMessagesAsync(It.IsAny<IEnumerable<CloudQueueMessage>>(), It.IsAny<CancellationToken>()))
                .Callback((IEnumerable<CloudQueueMessage> messages, CancellationToken cancellationToken) => callback = messages.ToList())
                .Returns(Task.CompletedTask);

            var initialBag = new ConcurrentBag<VersionedMessage>
            {
                new VersionedMessage(),
                new VersionedMessage(),
                new VersionedMessage(),
                new VersionedMessage(),
                new VersionedMessage()
            };

            var buffer = new Buffer(
                initialBag,
                this.LoggerFactory,
                mockQueueWrapper.Object,
                mockQueueMessageSerializer.Object);

            // Act
            var stopwatch = Stopwatch.StartNew();
            var bufferProcessResponse = await buffer.ProcessBufferAsync(CancellationToken.None).ConfigureAwait(false);
            stopwatch.Stop();

            // Assert
            this.WriteTimeElapsed(stopwatch);

            Assert.That(bufferProcessResponse, Is.Not.Null);
            Assert.That(bufferProcessResponse.Processed, Is.EqualTo(5));
            Assert.That(bufferProcessResponse.Remaining, Is.EqualTo(0));

            // Assert the Buffer is now empty
            Assert.That(buffer.InternalBuffer.Count, Is.EqualTo(0));

            // Assert the Callback
            Assert.That(callback, Is.Not.Null);
            Assert.That(callback.Count(), Is.EqualTo(5));

            mockQueueWrapper.Verify(
                r => r.SubmitMessagesAsync(It.IsAny<IEnumerable<CloudQueueMessage>>(), It.IsAny<CancellationToken>()),
                Times.Once());
        }

        /// <summary>
        /// Processes the buffer asynchronous when partial buffer processed expect messages inside buffer.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task ProcessBufferAsync_WhenPartialBufferProcessed_ExpectMessagesInsideBuffer()
        {
            // Arrange
            var mockQueueWrapper = new Mock<IQueueWrapper>();
            var mockQueueMessageSerializer = new Mock<IQueueMessageSerializer>();

            mockQueueMessageSerializer.Setup(r => r.GetQueueMessages(It.IsAny<IEnumerable<VersionedMessage>>()))
                .Returns(
                    (IEnumerable<VersionedMessage> messages) => new QueueMessageSerializerResponse
                    {
                        QueueMessages = new List<CloudQueueMessage>
                        {
                            new CloudQueueMessage("content"),
                            new CloudQueueMessage("content"),
                            new CloudQueueMessage("content")
                        },
                        UnusedMessages = new List<VersionedMessage>
                        {
                            new VersionedMessage(),
                            new VersionedMessage()
                        }
                    });

            var callback = default(List<CloudQueueMessage>);
            mockQueueWrapper
                .Setup(r => r.SubmitMessagesAsync(It.IsAny<IEnumerable<CloudQueueMessage>>(), It.IsAny<CancellationToken>()))
                .Callback((IEnumerable<CloudQueueMessage> messages, CancellationToken cancellationToken) => callback = messages.ToList())
                .Returns(Task.CompletedTask);

            var initialBag = new ConcurrentBag<VersionedMessage>
            {
                new VersionedMessage(),
                new VersionedMessage(),
                new VersionedMessage(),
                new VersionedMessage(),
                new VersionedMessage()
            };

            var buffer = new Buffer(
                initialBag,
                this.LoggerFactory,
                mockQueueWrapper.Object,
                mockQueueMessageSerializer.Object);

            // Act
            var stopwatch = Stopwatch.StartNew();
            var bufferProcessResponse = await buffer.ProcessBufferAsync(CancellationToken.None).ConfigureAwait(false);
            stopwatch.Stop();

            // Assert
            this.WriteTimeElapsed(stopwatch);

            Assert.That(bufferProcessResponse, Is.Not.Null);
            Assert.That(bufferProcessResponse.Processed, Is.EqualTo(3));
            Assert.That(bufferProcessResponse.Remaining, Is.EqualTo(2));

            // Assert the Buffer is now empty
            Assert.That(buffer.InternalBuffer.Count, Is.EqualTo(2));

            // Assert the Callback
            Assert.That(callback, Is.Not.Null);
            Assert.That(callback.Count, Is.EqualTo(3));

            mockQueueWrapper.Verify(
                r => r.SubmitMessagesAsync(It.IsAny<IEnumerable<CloudQueueMessage>>(), It.IsAny<CancellationToken>()),
                Times.Once());
        }
    }
}