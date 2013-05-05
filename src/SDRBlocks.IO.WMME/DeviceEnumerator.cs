using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SDRBlocks.Core.IO;
using SDRBlocks.IO.WMME.Interop;

namespace SDRBlocks.IO.WMME
{
    public class DeviceEnumerator : IDeviceEnumerator
    {
        #region IDeviceEnumerator Members

        public List<DeviceInformation> EnumerateInputDevices()
        {
            List<DeviceInformation> list = new List<DeviceInformation>();
            int count = Wave.waveInGetNumDevs();
            for (int i = 0; i < count; ++i)
            {
                WaveInCapabilities caps = new WaveInCapabilities();
                MmResult rv = Wave.waveInGetDevCaps((IntPtr)i, out caps, Marshal.SizeOf(caps));
                if (rv != MmResult.NoError)
                {
                    continue;
                }

                DeviceInformation devInfo = new DeviceInformation();
                devInfo.Id = i.ToString();
                devInfo.DeviceName = caps.ProductName;
                devInfo.MaxChannels = caps.Channels != -1 ? caps.Channels : 2;
                list.Add(devInfo);
            }

            return list;
        }

        public List<DeviceInformation> EnumerateOutputDevices()
        {
            List<DeviceInformation> list = new List<DeviceInformation>();
            int count = Wave.waveOutGetNumDevs();
            for (int i = 0; i < count; ++i)
            {
                WaveOutCapabilities caps = new WaveOutCapabilities();
                MmResult rv = Wave.waveOutGetDevCaps((IntPtr)i, out caps, Marshal.SizeOf(caps));
                if (rv != MmResult.NoError)
                {
                    continue;
                }

                DeviceInformation devInfo = new DeviceInformation();
                devInfo.Id = i.ToString();
                devInfo.DeviceName = caps.ProductName;
                devInfo.MaxChannels = caps.Channels != -1 ? caps.Channels : 2;
                list.Add(devInfo);
            }

            return list;
        }

        #endregion
    }
}
