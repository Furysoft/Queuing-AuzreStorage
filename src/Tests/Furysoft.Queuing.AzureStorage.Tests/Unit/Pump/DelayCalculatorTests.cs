// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DelayCalculatorTests.cs" company="Simon Paramore">
// © 2017, Simon Paramore
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Furysoft.Queuing.AzureStorage.Tests.Unit.Pump
{
    using System;
    using System.Diagnostics;
    using Logic.Pump;
    using NUnit.Framework;

    /// <summary>
    /// The Delay Calculator Tests
    /// </summary>
    [TestFixture]
    public sealed class DelayCalculatorTests : TestBase
    {
        /// <summary>
        /// Gets the next delay when operation took less time expect difference back.
        /// </summary>
        [Test]
        public void GetNextDelay_WhenOperationTookLessTime_ExpectDifferenceBack()
        {
            // Arrange
            var lastOperationTime = TimeSpan.FromMilliseconds(250);
            var minTime = TimeSpan.FromMilliseconds(1000);

            var delayCalculator = new DelayCalculator();

            // Act
            var stopwatch = Stopwatch.StartNew();
            var nextDelay = delayCalculator.GetNextDelay(lastOperationTime, minTime);
            stopwatch.Stop();

            // Assert
            this.WriteTimeElapsed(stopwatch);

            Assert.That(nextDelay, Is.EqualTo(TimeSpan.FromMilliseconds(750)));
        }

        /// <summary>
        /// Gets the next delay when operation took more time expect instant run.
        /// </summary>
        [Test]
        public void GetNextDelay_WhenOperationTookMoreTime_ExpectInstantRun()
        {
            // Arrange
            var lastOperationTime = TimeSpan.FromMilliseconds(1250);
            var minTime = TimeSpan.FromMilliseconds(1000);

            var delayCalculator = new DelayCalculator();

            // Act
            var stopwatch = Stopwatch.StartNew();
            var nextDelay = delayCalculator.GetNextDelay(lastOperationTime, minTime);
            stopwatch.Stop();

            // Assert
            this.WriteTimeElapsed(stopwatch);

            Assert.That(nextDelay, Is.EqualTo(TimeSpan.Zero));
        }

        /// <summary>
        /// Gets the next delay when operation took same time expect instant run.
        /// </summary>
        [Test]
        public void GetNextDelay_WhenOperationTookSameTime_ExpectInstantRun()
        {
            // Arrange
            var lastOperationTime = TimeSpan.FromMilliseconds(1000);
            var minTime = TimeSpan.FromMilliseconds(1000);

            var delayCalculator = new DelayCalculator();

            // Act
            var stopwatch = Stopwatch.StartNew();
            var nextDelay = delayCalculator.GetNextDelay(lastOperationTime, minTime);
            stopwatch.Stop();

            // Assert
            this.WriteTimeElapsed(stopwatch);

            Assert.That(nextDelay, Is.EqualTo(TimeSpan.Zero));
        }
    }
}