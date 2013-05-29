using System;

namespace SDRBlocks.Core
{
    public sealed class SourcePin : Pin
    {
        public SourcePin(IDspBlock block)
            : base(block)
        {
        }
    }
}
