using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDRBlocks.Core.IO
{
    /// <summary>
    /// Module providing I/O functionality.
    /// </summary>
    public interface IModuleIO
    {
        IDeviceEnumerator Enumerator { get; }

        IDspBlock CreateInputDevice(string id, uint channels, uint sampleRate);
    }
}
