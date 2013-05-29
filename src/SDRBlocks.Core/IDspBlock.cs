using System;

namespace SDRBlocks.Core
{
    /// <summary>
    /// The interface each DSP block implements.
    /// </summary>
    public interface IDspBlock
    {
        /// <summary>
        /// True if this block does not require any input.
        /// Usually this is a root signal source (e.g., audio input, file reader, or generator).
        /// </summary>
        bool IsIndependent { get; }

        /// <summary>
        /// True if the block can be scheduled for processing (i.e., all inputs can be consumed).
        /// Used to properly schedule jobs.
        /// </summary>
        bool IsReadyToProcess { get; }

        /// <summary>
        /// Perform the actual signal processing.
        /// </summary>
        void Process();
    }
}
