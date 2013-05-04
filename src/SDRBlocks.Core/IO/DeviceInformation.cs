using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDRBlocks.Core.IO
{
    public sealed class DeviceInformation
    {
        /// <summary>
        /// Provides a way to identify this particular device.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Human-readable device name.
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// Maximum channel count for this device.
        /// </summary>
        public int MaxChannels { get; set; }
    }
}
