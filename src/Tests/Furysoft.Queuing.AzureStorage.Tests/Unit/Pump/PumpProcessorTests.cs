// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PumpProcessorTests.cs" company="Simon Paramore">
// © 2017, Simon Paramore
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Furysoft.Queuing.AzureStorage.Tests.Unit.Pump
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Entities.Buffer;
    using Entities.Configuration;
    using Interfaces.Pump;
    using Interfaces.Wrappers;
    using Logic.Pump;
    using Moq;
    using NUnit.Framework;

    /// <summary>
    /// The Pump Processor Tests
    /// </summary>
    [TestFixture]
    public sealed class PumpProcessorTests : TestBase
    {
        /// <summary>
        /// Runs the when canceled expect exits.
        /// </summary>
        [Test]
        public void Run_WhenCanceled_ExpectExits()
        {
            // Arrange
            var mockBuffer = new Mock<IBuffer>();
            var mockStopwatchFactory = new Mock<IStopwatchFactory>();
            var mockDelayCalculator = new Mock<IDelayCalculator>();

            var scheduleSettings = new ScheduleSettings
            {
                ThrottleTime = TimeSpan.FromMilliseconds(10)
            };

            var pumpProcessor = new PumpProcessor(
                this.LoggerFactory,
                mockBuffer.Object,
                mockStopwatchFactory.Object,
                mockDelayCalculator.Object,
                scheduleSettings);

            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            // Act
            var stopwatch = Stopwatch.StartNew();
            pumpProcessor.Run(cancellationTokenSource.Token);
            stopwatch.Stop();

            // Assert
            this.WriteTimeElapsed(stopwatch);

            mockBuffer.Verify(r => r.ProcessBufferAsync(It.IsAny<CancellationToken>()), Times.Never());
        }

        /// <summary>
        /// Runs the when data processed expect batch submitted event.
        /// </summary>
        [Test]
        public void Run_WhenDataProcessed_ExpectBatchSubmittedEvent()
        {
            // Arrange
            var mockBuffer = new Mock<IBuffer>();
            var mockStopwatchFactory = new Mock<IStopwatchFactory>();
            var mockStopwatch = new Mock<IStopwatch>();
            var mockDelayCalculator = new Mock<IDelayCalculator>();

            mockStopwatchFactory.Setup(r => r.StartNew()).Returns(mockStopwatch.Object);

            mockDelayCalculator.Setup(r => r.GetNextDelay(It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>()))
                .Returns(TimeSpan.FromMilliseconds(100));

            mockBuffer.Setup(r => r.ProcessBufferAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new BufferProcessResponse
                {
                    Processed = 10,
                    Remaining = 10
                });

            var scheduleSettings = new ScheduleSettings
            {
                ThrottleTime = TimeSpan.FromMilliseconds(10)
            };

            var pumpProcessor = new PumpProcessor(
                this.LoggerFactory,
                mockBuffer.Object,
                mockStopwatchFactory.Object,
                mockDelayCalculator.Object,
                scheduleSettings);

            // Act
            var stopwatch = Stopwatch.StartNew();
            pumpProcessor.Run(CancellationToken.None);
            stopwatch.Stop();

            // Assert
            this.WriteTimeElapsed(stopwatch);

            var manualResetEventSlim = new ManualResetEventSlim(false);

            pumpProcessor.BatchSubmitted += (sender, i) => manualResetEventSlim.Set();

            var wasTriggered = manualResetEventSlim.Wait(TimeSpan.FromMilliseconds(100));
            Assert.That(wasTriggered, Is.True);

            mockBuffer.Verify(r => r.ProcessBufferAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce());
        }

        /// <summary>
        /// Runs the when no data remaining processed expect buffer empty event.
        /// </summary>
        [Test]
        public void Run_WhenNoDataRemainingProcessed_ExpectBufferEmptyEvent()
        {
            // Arrange
            var mockBuffer = new Mock<IBuffer>();
            var mockStopwatchFactory = new Mock<IStopwatchFactory>();
            var mockStopwatch = new Mock<IStopwatch>();
            var mockDelayCalculator = new Mock<IDelayCalculator>();

            mockStopwatchFactory.Setup(r => r.StartNew()).Returns(mockStopwatch.Object);

            mockDelayCalculator.Setup(r => r.GetNextDelay(It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>()))
                .Returns(TimeSpan.FromMilliseconds(100));

            mockBuffer.Setup(r => r.ProcessBufferAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new BufferProcessResponse
                {
                    Processed = 10,
                    Remaining = 0
                });

            var scheduleSettings = new ScheduleSettings
            {
                ThrottleTime = TimeSpan.FromMilliseconds(10)
            };

            var pumpProcessor = new PumpProcessor(
                this.LoggerFactory,
                mockBuffer.Object,
                mockStopwatchFactory.Object,
                mockDelayCalculator.Object,
                scheduleSettings);

            // Act
            var stopwatch = Stopwatch.StartNew();
            pumpProcessor.Run(CancellationToken.None);
            stopwatch.Stop();

            // Assert
            this.WriteTimeElapsed(stopwatch);

            var manualResetEventSlim = new ManualResetEventSlim(false);

            pumpProcessor.BufferEmpty += (sender, i) => manualResetEventSlim.Set();

            var wasTriggered = manualResetEventSlim.Wait(TimeSpan.FromMilliseconds(100));
            Assert.That(wasTriggered, Is.True);

            mockBuffer.Verify(r => r.ProcessBufferAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce());
        }

        /// <summary>
        /// Runs the when run expect buffer processed.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [Test]
        public async Task Run_WhenRun_ExpectBufferProcessed()
        {
            // Arrange
            var mockBuffer = new Mock<IBuffer>();
            var mockStopwatchFactory = new Mock<IStopwatchFactory>();
            var mockStopwatch = new Mock<IStopwatch>();
            var mockDelayCalculator = new Mock<IDelayCalculator>();

            mockStopwatchFactory.Setup(r => r.StartNew()).Returns(mockStopwatch.Object);

            mockBuffer.Setup(r => r.ProcessBufferAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new BufferProcessResponse());

            mockDelayCalculator.Setup(r => r.GetNextDelay(It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>()))
                .Returns(TimeSpan.FromMilliseconds(100));

            var scheduleSettings = new ScheduleSettings
            {
                ThrottleTime = TimeSpan.FromMilliseconds(10)
            };

            var pumpProcessor = new PumpProcessor(
                this.LoggerFactory,
                mockBuffer.Object,
                mockStopwatchFactory.Object,
                mockDelayCalculator.Object,
                scheduleSettings);

            // Act
            var stopwatch = Stopwatch.StartNew();
            Assert.DoesNotThrow(() => pumpProcessor.Run(CancellationToken.None));
            stopwatch.Stop();

            // Assert
            this.WriteTimeElapsed(stopwatch);

            await Task.Delay(200).ConfigureAwait(false);

            mockBuffer.Verify(r => r.ProcessBufferAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce());
        }
    }
}