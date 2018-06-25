// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LinqHelpers.cs" company="Simon Paramore">
// © 2017, Simon Paramore
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Furysoft.Queuing.AzureStorage.Logic.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using Versioning;

    /// <summary>
    /// The Linq Helpers
    /// </summary>
    internal static class LinqHelpers
    {
        /// <summary>
        /// Batches the messages.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="batchSize">Size of the batch.</param>
        /// <returns>The List of <see cref="BatchedVersionedMessage"/></returns>
        public static IEnumerable<BatchedVersionedMessage> BatchMessages(
            this IEnumerable<VersionedMessage> source,
            int batchSize)
        {
            var rtn = new List<BatchedVersionedMessage>();

            var currentBatch = new List<VersionedMessage>();
            foreach (var versionedMessage in source)
            {
                if (currentBatch.Count == batchSize)
                {
                    rtn.Add(new BatchedVersionedMessage { Messages = currentBatch });
                    currentBatch = new List<VersionedMessage>();
                }

                currentBatch.Add(versionedMessage);
            }

            if (currentBatch.Any())
            {
                rtn.Add(new BatchedVersionedMessage { Messages = currentBatch });
            }

            return rtn;
        }
    }
}