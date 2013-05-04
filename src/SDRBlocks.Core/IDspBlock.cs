using System;
using System.Collections.Generic;

namespace SDRBlocks.Core
{
    public interface IDspBlock : IDisposable
    {
        IList<IStreamInput> Inputs { get; }

        IList<IStreamOutput> Outputs { get; }
    }
}
