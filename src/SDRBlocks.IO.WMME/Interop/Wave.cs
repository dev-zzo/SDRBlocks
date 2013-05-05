using System;
using System.Runtime.InteropServices;

namespace SDRBlocks.IO.WMME.Interop
{
    internal enum WaveMessage
    {
        /// <summary>
        /// WIM_OPEN
        /// </summary>
        WaveInOpen = 0x3BE,
        /// <summary>
        /// WIM_CLOSE
        /// </summary>
        WaveInClose = 0x3BF,
        /// <summary>
        /// WIM_DATA
        /// </summary>
        WaveInData = 0x3C0,
        /// <summary>
        /// WOM_CLOSE
        /// </summary>
        WaveOutClose = 0x3BC,
        /// <summary>
        /// WOM_DONE
        /// </summary>
        WaveOutDone = 0x3BD,
        /// <summary>
        /// WOM_OPEN
        /// </summary>
        WaveOutOpen = 0x3BB
    }

    // WaveOutProc http://msdn.microsoft.com/en-us/library/dd743869%28VS.85%29.aspx
    // WaveInProc http://msdn.microsoft.com/en-us/library/dd743849%28VS.85%29.aspx
    internal delegate void WaveCallback(IntPtr hWaveOut, WaveMessage message, IntPtr dwInstance, IntPtr param1, IntPtr param2);

    internal static class Wave
    {
        [DllImport("winmm.dll")]
        public static extern Int32 mmioStringToFOURCC([MarshalAs(UnmanagedType.LPStr)] String s, int flags);

        [DllImport("winmm.dll")]
        public static extern Int32 waveOutGetNumDevs();

        // http://msdn.microsoft.com/en-us/library/dd743857%28VS.85%29.aspx
        [DllImport("winmm.dll", CharSet = CharSet.Auto)]
        public static extern MmResult waveOutGetDevCaps(IntPtr deviceID, out WaveOutCapabilities waveOutCaps, int waveOutCapsSize);

        // http://msdn.microsoft.com/en-us/library/dd743866%28VS.85%29.aspx
        [DllImport("winmm.dll")]
        public static extern MmResult waveOutOpen(out IntPtr hWaveOut, IntPtr uDeviceID, ref WaveFormat lpFormat, WaveCallback dwCallback, IntPtr dwInstance, WaveOpenFlags dwFlags);

        [DllImport("winmm.dll")]
        public static extern MmResult waveOutPrepareHeader(IntPtr hWaveOut, IntPtr lpWaveOutHdr, int uSize);

        [DllImport("winmm.dll")]
        public static extern MmResult waveOutUnprepareHeader(IntPtr hWaveOut, IntPtr lpWaveOutHdr, int uSize);

        [DllImport("winmm.dll")]
        public static extern MmResult waveOutWrite(IntPtr hWaveOut, IntPtr lpWaveOutHdr, int uSize);

        [DllImport("winmm.dll")]
        public static extern MmResult waveOutReset(IntPtr hWaveOut);

        [DllImport("winmm.dll")]
        public static extern MmResult waveOutPause(IntPtr hWaveOut);

        [DllImport("winmm.dll")]
        public static extern MmResult waveOutRestart(IntPtr hWaveOut);

        // http://msdn.microsoft.com/en-us/library/dd743863%28VS.85%29.aspx
        [DllImport("winmm.dll")]
        public static extern MmResult waveOutGetPosition(IntPtr hWaveOut, out MmTime mmTime, int uSize);

        // http://msdn.microsoft.com/en-us/library/dd743874%28VS.85%29.aspx
        [DllImport("winmm.dll")]
        public static extern MmResult waveOutSetVolume(IntPtr hWaveOut, int dwVolume);

        [DllImport("winmm.dll")]
        public static extern MmResult waveOutGetVolume(IntPtr hWaveOut, out int dwVolume);

        [DllImport("winmm.dll")]
        public static extern MmResult waveOutClose(IntPtr hWaveOut);


        [DllImport("winmm.dll")]
        public static extern Int32 waveInGetNumDevs();

        // http://msdn.microsoft.com/en-us/library/dd743841%28VS.85%29.aspx
        [DllImport("winmm.dll", CharSet = CharSet.Auto)]
        public static extern MmResult waveInGetDevCaps(IntPtr deviceID, out WaveInCapabilities waveInCaps, int waveInCapsSize);

        // http://msdn.microsoft.com/en-us/library/dd743838%28VS.85%29.aspx
        [DllImport("winmm.dll")]
        public static extern MmResult waveInAddBuffer(IntPtr hWaveIn, ref WaveHeader pwh, int cbwh);

        // http://msdn.microsoft.com/en-us/library/dd743847%28VS.85%29.aspx
        [DllImport("winmm.dll")]
        public static extern MmResult waveInOpen(out IntPtr hWaveIn, IntPtr uDeviceID, ref WaveFormat lpFormat, WaveCallback dwCallback, IntPtr dwInstance, WaveOpenFlags dwFlags);

        // http://msdn.microsoft.com/en-us/library/dd743848%28VS.85%29.aspx
        [DllImport("winmm.dll")]
        public static extern MmResult waveInPrepareHeader(IntPtr hWaveIn, ref WaveHeader lpWaveInHdr, int uSize);

        [DllImport("winmm.dll")]
        public static extern MmResult waveInUnprepareHeader(IntPtr hWaveIn, ref WaveHeader lpWaveInHdr, int uSize);

        // http://msdn.microsoft.com/en-us/library/dd743850%28VS.85%29.aspx
        [DllImport("winmm.dll")]
        public static extern MmResult waveInReset(IntPtr hWaveIn);

        // http://msdn.microsoft.com/en-us/library/dd743851%28VS.85%29.aspx
        [DllImport("winmm.dll")]
        public static extern MmResult waveInStart(IntPtr hWaveIn);

        // http://msdn.microsoft.com/en-us/library/dd743852%28VS.85%29.aspx
        [DllImport("winmm.dll")]
        public static extern MmResult waveInStop(IntPtr hWaveIn);

        [DllImport("winmm.dll")]
        public static extern MmResult waveInClose(IntPtr hWaveIn);
    }
}
