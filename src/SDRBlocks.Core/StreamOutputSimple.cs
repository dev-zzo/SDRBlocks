using System;

namespace SDRBlocks.Core
{
    public class StreamOutputSimple : StreamOutputBase
    {
        public StreamOutputSimple(uint sampleRate, uint channelCount, uint bufferSize, uint sampleSize)
            : base(sampleRate, channelCount, bufferSize, sampleSize)
        {
        }
    }
}
