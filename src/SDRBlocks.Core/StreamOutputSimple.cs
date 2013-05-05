using System;

namespace SDRBlocks.Core
{
    public class StreamOutputSimple : StreamOutputBase
    {
        public StreamOutputSimple(uint channelCount, FrameFormat format, uint frameRate, uint bufferSize)
            : base(channelCount, format, frameRate, bufferSize)
        {
        }
    }
}
