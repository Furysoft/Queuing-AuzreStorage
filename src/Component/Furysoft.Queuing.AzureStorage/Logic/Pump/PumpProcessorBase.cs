// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PumpProcessorBase.cs" company="Simon Paramore">
// © 2017, Simon Paramore
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Furysoft.Queuing.AzureStorage.Logic.Pump
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;
    using Interfaces;
    using Interfaces.Pump;
    using JetBrains.Annotations;
    using Microsoft.Extensions.Logging;
    using Microsoft.WindowsAzure.Storage.Queue;

    /// <summary>
    /// The Pump Processor Base
    /// </summary>
    /// <typeparam name="TDataType">The type of the data type.</typeparam>
    /// <seealso cref="IPumpProcessor" />
    internal abstract class PumpProcessorBase<TDataType> : IPumpProcessor
    {
        /// <summary>
        /// The logger
        /// </summary>
        [NotNull]
        private readonly ILogger logger;

        /// <summary>
        /// The queue wrapper
        /// </summary>
        [NotNull]
        private readonly IQueueWrapper queueWrapper;

        /// <summary>
        /// The subject
        /// </summary>
        private ConcurrentBag<TDataType> subject = new ConcurrentBag<TDataType>();

        /// <summary>
        /// Occurs when [batch submitted].
        /// </summary>
        public event EventHandler<int> BatchSubmitted;

        /// <summary>
        /// Occurs when [buffer empty].
        /// </summary>
        public event EventHandler BufferEmpty;

        /// <summary>
        /// Initializes a new instance of the <see cref="PumpProcessorBase{TDataType}" /> class.
        /// </summary>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="queueWrapper">The queue wrapper.</param>
        protected PumpProcessorBase([NotNull] ILoggerFactory loggerFactory, [NotNull] IQueueWrapper queueWrapper)
        {
            this.queueWrapper = queueWrapper;

            this.logger = loggerFactory.CreateLogger<PumpProcessor>();
        }

        /// <summary>
        /// Adds the message.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="message">The message.</param>
        public abstract void AddMessage<TEntity>(TEntity message) 
            where TEntity : class;

        /// <summary>
        /// Starts the specified cancellation token.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        public abstract void Start(CancellationToken cancellationToken);
    }
}