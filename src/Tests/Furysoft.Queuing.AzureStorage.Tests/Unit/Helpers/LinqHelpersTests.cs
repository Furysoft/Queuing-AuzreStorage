// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LinqHelpersTests.cs" company="Simon Paramore">
// © 2017, Simon Paramore
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Furysoft.Queuing.AzureStorage.Tests.Unit.Helpers
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Logic.Helpers;
    using NUnit.Framework;
    using Versioning;

    /// <summary>
    /// The LINQ Helpers Tests
    /// </summary>
    [TestFixture]
    public sealed class LinqHelpersTests : TestBase
    {
        /// <summary>
        /// Batches the messages when list sent in expect batched.
        /// </summary>
        [Test]
        public void BatchMessages_WhenListSentIn_ExpectBatched()
        {
            // Arrange
            var list = new List<VersionedMessage>
            {
                new VersionedMessage(),
                new VersionedMessage(),
                new VersionedMessage(),
                new VersionedMessage(),
                new VersionedMessage()
            };

            // Act
            var stopwatch = Stopwatch.StartNew();
            var batchedVersionedMessages = list.BatchMessages(2).ToList();
            stopwatch.Stop();

            // Assert
            this.WriteTimeElapsed(stopwatch);

            Assert.That(batchedVersionedMessages, Is.Not.Null);

            Assert.That(batchedVersionedMessages.Count, Is.EqualTo(3));

            Assert.That(batchedVersionedMessages[0].Messages.Count(), Is.EqualTo(2));
            Assert.That(batchedVersionedMessages[1].Messages.Count(), Is.EqualTo(2));
            Assert.That(batchedVersionedMessages[2].Messages.Count(), Is.EqualTo(1));
        }
    }
}