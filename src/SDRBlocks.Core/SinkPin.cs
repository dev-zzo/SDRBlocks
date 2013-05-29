using System;

namespace SDRBlocks.Core
{
    public sealed class SinkPin : Pin
    {
        public SinkPin(IDspBlock block)
            : base(block)
        {
        }
    }
}
