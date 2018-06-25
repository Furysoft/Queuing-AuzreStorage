// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PumpProcessor.cs" company="Simon Paramore">
// © 2017, Simon Paramore
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Furysoft.Queuing.AzureStorage.Logic.Pump
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Entities.Configuration;
    using Interfaces.Pump;
    using Interfaces.Wrappers;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The Pump Processor
    /// </summary>
    internal sealed class PumpProcessor : IPumpProcessor
    {
        /// <summary>
        /// The buffer
        /// </summary>
        [NotNull]
        private readonly IBuffer buffer;

        /// <summary>
        /// The delay calculator
        /// </summary>
        [NotNull]
        private readonly IDelayCalculator delayCalculator;

        /// <summary>
        /// The logger
        /// </summary>
        [NotNull]
        private readonly ILogger logger;

        /// <summary>
        /// The schedule settings
        /// </summary>
        [NotNull]
        private readonly ScheduleSettings scheduleSettings;

        /// <summary>
        /// The stopwatch
        /// </summary>
        [NotNull]
        private readonly IStopwatchFactory stopwatchFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="PumpProcessor" /> class.
        /// </summary>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="stopwatchFactory">The stopwatch factory.</param>
        /// <param name="delayCalculator">The delay calculator.</param>
        /// <param name="scheduleSettings">The schedule settings.</param>
        public PumpProcessor(
            [NotNull] ILoggerFactory loggerFactory,
            [NotNull] IBuffer buffer,
            [NotNull] IStopwatchFactory stopwatchFactory,
            [NotNull] IDelayCalculator delayCalculator,
            [NotNull] ScheduleSettings scheduleSettings)
        {
            this.buffer = buffer;
            this.stopwatchFactory = stopwatchFactory;
            this.delayCalculator = delayCalculator;
            this.scheduleSettings = scheduleSettings;

            this.logger = loggerFactory.CreateLogger<PumpProcessor>();
        }

        /// <summary>
        /// Occurs when [batch submitted].
        /// </summary>
        public event EventHandler<int> BatchSubmitted;

        /// <summary>
        /// Occurs when [buffer empty].
        /// </summary>
        public event EventHandler BufferEmpty;

        /// <summary>
        /// Starts this instance.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        public void Run(CancellationToken cancellationToken)
        {
            Task.Run(
                async () =>
                {
                    this.logger.LogInformation("Starting Queue Pump Processor @ {0}", DateTime.UtcNow);

                    var delay = this.scheduleSettings.ThrottleTime;

                    /*  Main Pump Loop */
                    while (true)
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            break;
                        }

                        await Task.Delay(delay, CancellationToken.None).ConfigureAwait(false);

                        /* Process the buffer */
                        var sw = this.stopwatchFactory.StartNew();
                        var bufferProcessResponse = await this.buffer.ProcessBufferAsync(CancellationToken.None).ConfigureAwait(false);
                        sw.Stop();

                        /* Trigger events based on what was processed */
                        if (bufferProcessResponse.Processed > 0)
                        {
                            this.BatchSubmitted?.Invoke(this, bufferProcessResponse.Processed);
                        }

                        if (bufferProcessResponse.Remaining == 0)
                        {
                            this.BufferEmpty?.Invoke(this, EventArgs.Empty);
                        }

                        delay = this.delayCalculator.GetNextDelay(sw.Elapsed, this.scheduleSettings.ThrottleTime);
                    }
                }, CancellationToken.None);

            this.logger.LogInformation("Stopping Queue Pump Processor @ {0}", DateTime.UtcNow);
        }
    }
}