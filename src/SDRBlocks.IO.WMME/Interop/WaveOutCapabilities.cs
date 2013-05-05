using System;
using System.Runtime.InteropServices;

namespace SDRBlocks.IO.WMME.Interop
{
    /// <summary>
    /// Flags indicating what features this WaveOut device supports
    /// </summary>
    [Flags]
    internal enum WaveOutSupport
    {
        /// <summary>supports pitch control (WAVECAPS_PITCH)</summary>
        Pitch = 0x0001,
        /// <summary>supports playback rate control (WAVECAPS_PLAYBACKRATE)</summary>
        PlaybackRate = 0x0002,
        /// <summary>supports volume control (WAVECAPS_VOLUME)</summary>
        Volume = 0x0004,
        /// <summary>supports separate left-right volume control (WAVECAPS_LRVOLUME)</summary>
        LRVolume = 0x0008,
        /// <summary>(WAVECAPS_SYNC)</summary>
        Sync = 0x0010,
        /// <summary>(WAVECAPS_SAMPLEACCURATE)</summary>
        SampleAccurate = 0x0020,
    }

    /// <summary>
    /// WaveOutCapabilities structure (based on WAVEOUTCAPS from mmsystem.h)
    /// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/multimed/htm/_win32_waveoutcaps_str.asp
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct WaveOutCapabilities
    {
        /// <summary>
        /// wMid
        /// </summary>
        private short manufacturerId;
        /// <summary>
        /// wPid
        /// </summary>
        private short productId;
        /// <summary>
        /// vDriverVersion
        /// </summary>
        private int driverVersion;
        /// <summary>
        /// Product Name (szPname)
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MaxProductNameLength)]
        private string productName;
        /// <summary>
        /// Supported formats (bit flags) dwFormats 
        /// </summary>
        private SupportedWaveFormat supportedFormats;
        /// <summary>
        /// Supported channels (1 for mono 2 for stereo) (wChannels)
        /// Seems to be set to -1 on a lot of devices
        /// </summary>
        private short channels;
        /// <summary>
        /// wReserved1
        /// </summary>
        private short reserved;
        /// <summary>
        /// Optional functionality supported by the device
        /// </summary>
        private WaveOutSupport support;

        private const int MaxProductNameLength = 32;

        /// <summary>
        /// Number of channels supported
        /// </summary>
        public int Channels
        {
            get
            {
                return channels;
            }
        }

        /// <summary>
        /// Whether playback control is supported
        /// </summary>
        public bool SupportsPlaybackRateControl
        {
            get
            {
                return (support & WaveOutSupport.PlaybackRate) == WaveOutSupport.PlaybackRate;
            }
        }

        /// <summary>
        /// The product name
        /// </summary>
        public string ProductName
        {
            get
            {
                return productName;
            }
        }

        /// <summary>
        /// Checks to see if a given SupportedWaveFormat is supported
        /// </summary>
        /// <param name="waveFormat">The SupportedWaveFormat</param>
        /// <returns>true if supported</returns>
        public bool SupportsWaveFormat(SupportedWaveFormat waveFormat)
        {
            return (supportedFormats & waveFormat) == waveFormat;
        }
    }
}
