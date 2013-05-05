using System.Runtime.InteropServices;

namespace SDRBlocks.IO.WMME.Interop
{
    /// <summary>
    /// WaveInCapabilities structure (based on WAVEINCAPS from mmsystem.h)
    /// http://msdn.microsoft.com/en-us/library/ms713726(VS.85).aspx
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct WaveInCapabilities
    {
        /// <summary>
        /// wMid
        /// </summary>
        public short ManufacturerId;

        /// <summary>
        /// wPid
        /// </summary>
        public short ProductId;

        /// <summary>
        /// vDriverVersion
        /// </summary>
        public int DriverVersion;

        /// <summary>
        /// Product Name (szPname)
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string ProductName;

        /// <summary>
        /// Supported formats (bit flags) dwFormats 
        /// </summary>
        public SupportedWaveFormat SupportedFormats;

        /// <summary>
        /// Supported channels (1 for mono 2 for stereo) (wChannels)
        /// Seems to be set to -1 on a lot of devices
        /// </summary>
        public short Channels;

        /// <summary>
        /// wReserved1
        /// </summary>
        public short Reserved;
    }
}
