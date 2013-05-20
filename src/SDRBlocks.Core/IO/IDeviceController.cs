using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDRBlocks.Core.IO
{
    /// <summary>
    /// Control interface for I/O devices.
    /// </summary>
    public interface IDeviceController
    {
        /// <summary>
        /// RX center frequency.
        /// </summary>
        public long CenterFrequency { get; set; }
    }
}
