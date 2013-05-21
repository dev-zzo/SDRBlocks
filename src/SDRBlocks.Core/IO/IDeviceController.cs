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
        long CenterFrequency { get; set; }

        /// <summary>
        /// Frequency correction, ppm.
        /// </summary>
        int CorrectionCoefficient { get; set; }
    }
}
