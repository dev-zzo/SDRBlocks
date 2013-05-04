using System;

namespace SDRBlocks.Core
{
    public delegate void OutputBufferConsumedDelegate(IStreamOutput sender);

    public interface IStreamOutput : IDisposable
    {
        /// <summary>
        /// Associated input.
        /// </summary>
        IStreamInput AttachedInput { get; set; }

        /// <summary>
        /// Samples per second.
        /// </summary>
        uint SampleRate { get; }

        /// <summary>
        /// How many channels interleaved in the stream.
        /// </summary>
        uint ChannelCount { get; }

        /// <summary>
        /// What kind of data the frames carry.
        /// </summary>
        FrameFormat Format { get; }

        /// <summary>
        /// The buffer containing samples.
        /// </summary>
        FrameBuffer Buffer { get; }

        event OutputBufferConsumedDelegate ConsumedEvent; 
    }
}
