using System;

namespace SDRBlocks.Core
{
    public class StreamOutputSimple : StreamOutputBase
    {
        public StreamOutputSimple(uint sampleRate, uint channelCount, FrameFormat format, uint bufferSize)
            : base(sampleRate, channelCount, format, bufferSize)
        {
        }
    }
}
