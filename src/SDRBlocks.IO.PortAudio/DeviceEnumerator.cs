using System.Collections.Generic;
using PortAudioSharp;
using SDRBlocks.Core.IO;

namespace SDRBlocks.IO.PortAudio
{
    sealed class DeviceEnumerator : IDeviceEnumerator
    {
        #region IDeviceEnumerator Members

        public List<DeviceInformation> EnumerateInputDevices()
        {
            var list = new List<DeviceInformation>();
            int count = PortAudioAPI.Pa_GetDeviceCount();
            for (int i = 0; i < count; i++)
            {
                var devInfo = PortAudioAPI.Pa_GetDeviceInfo(i);
                if (devInfo.maxInputChannels == 0)
                    continue;

                var hostApiInfo = PortAudioAPI.Pa_GetHostApiInfo(devInfo.hostApi);
                DeviceInformation myDevInfo = new DeviceInformation();
                myDevInfo.DeviceName = devInfo.name;
                myDevInfo.Id = i.ToString();
                myDevInfo.MaxChannels = devInfo.maxInputChannels;
                list.Add(myDevInfo);
            }

            return list;
        }

        public List<DeviceInformation> EnumerateOutputDevices()
        {
            var list = new List<DeviceInformation>();
            int count = PortAudioAPI.Pa_GetDeviceCount();
            for (int i = 0; i < count; i++)
            {
                var devInfo = PortAudioAPI.Pa_GetDeviceInfo(i);
                if (devInfo.maxOutputChannels == 0)
                    continue;

                var hostApiInfo = PortAudioAPI.Pa_GetHostApiInfo(devInfo.hostApi);
                DeviceInformation myDevInfo = new DeviceInformation();
                myDevInfo.DeviceName = devInfo.name;
                myDevInfo.Id = i.ToString();
                myDevInfo.MaxChannels = devInfo.maxOutputChannels;
                list.Add(myDevInfo);
            }

            return list;
        }

        #endregion
    }
}
