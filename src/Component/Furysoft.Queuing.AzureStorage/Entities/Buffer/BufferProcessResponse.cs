// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BufferProcessResponse.cs" company="Simon Paramore">
// © 2017, Simon Paramore
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Furysoft.Queuing.AzureStorage.Entities.Buffer
{
    /// <summary>
    /// The Buffer Process Response
    /// </summary>
    internal sealed class BufferProcessResponse
    {
        /// <summary>
        /// Gets or sets the number of messages processed.
        /// </summary>
        public int Processed { get; set; }

        /// <summary>
        /// Gets or sets the number of items remaining in the buffer
        /// </summary>
        public int Remaining { get; set; }
    }
}