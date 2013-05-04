using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDRBlocks.Core.IO
{
    public interface IDeviceEnumerator
    {
        List<DeviceInformation> EnumerateInputDevices();

        List<DeviceInformation> EnumerateOutputDevices();
    }
}
