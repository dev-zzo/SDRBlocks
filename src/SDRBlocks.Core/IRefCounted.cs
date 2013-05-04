using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDRBlocks.Core
{
    /// <summary>
    /// Implements reference counting for objects that need it.
    /// Currently only used by DataChunk.
    /// </summary>
    public interface IRefCounted
    {
        void Refer(int count = 1);

        void Release();
    }
}
