using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SDRBlocks.Misc.USBAPI
{
    public delegate void EnumerateHidDevicesDelegate(IntPtr hDevInfo, ref SpDevInfoData devInfoData, string deviceInstanceId);

    public static class UsbAPI
    {
        public static void EnumerateHidDevices(EnumerateHidDevicesDelegate cb)
        {
            Guid guid;
            SetupAPI.HidD_GetHidGuid(out guid);
            IntPtr hDevInfo = SetupAPI.SetupDiGetClassDevs(
                ref guid,
                IntPtr.Zero,
                IntPtr.Zero,
                SetupAPI.DIGCF_DEVICEINTERFACE | SetupAPI.DIGCF_PRESENT);

            var deviceInfoData = new SpDevInfoData();
            deviceInfoData.cbSize = (uint)Marshal.SizeOf(deviceInfoData);

            for (uint i = 0; SetupAPI.SetupDiEnumDeviceInfo(hDevInfo, i, ref deviceInfoData); i++)
            {
                uint bufferSize = 1024;
                var sb = new StringBuilder((int)bufferSize);

                SetupAPI.SetupDiGetDeviceInstanceId(
                    hDevInfo,
                    ref deviceInfoData,
                    sb,
                    bufferSize,
                    out bufferSize);
                cb(hDevInfo, ref deviceInfoData, sb.ToString());
            }

            SetupAPI.SetupDiDestroyDeviceInfoList(hDevInfo);
        }

        /// <summary>
        /// Obtain the device interface path for the given device.
        /// </summary>
        /// <param name="hDevInfo"></param>
        /// <param name="devInfoData"></param>
        /// <returns></returns>
        public static string GetHidDevicePath(IntPtr hDevInfo, ref SpDevInfoData devInfoData)
        {
            Guid guid;
            SetupAPI.HidD_GetHidGuid(out guid);

            var interfaceData = new SpDeviceInterfaceData();
            interfaceData.cbSize = Marshal.SizeOf(interfaceData);
            SetupAPI.SetupDiEnumDeviceInterfaces(
                hDevInfo,
                ref devInfoData,
                ref guid,
                0,
                ref interfaceData);

            int requiredSize;
            SetupAPI.SetupDiGetDeviceInterfaceDetail(
                hDevInfo,
                ref interfaceData,
                IntPtr.Zero,
                0,
                out requiredSize,
                IntPtr.Zero);

            var interfaceDetail = new SpDeviceInterfaceDetailData();
            interfaceDetail.cbSize = IntPtr.Size == 8 ? 8 : 4 + Marshal.SystemDefaultCharSize;
            SetupAPI.SetupDiGetDeviceInterfaceDetail(
                hDevInfo,
                ref interfaceData,
                ref interfaceDetail,
                requiredSize,
                IntPtr.Zero,
                IntPtr.Zero);

            return interfaceDetail.DevicePath;
        }
    }
}
