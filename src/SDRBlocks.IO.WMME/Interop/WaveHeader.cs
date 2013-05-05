using System;
using System.Runtime.InteropServices;

namespace SDRBlocks.IO.WMME.Interop
{
    /// <summary>
    /// Wave Header Flags enumeration
    /// </summary>
    [Flags]
    public enum WaveHeaderFlags
    {
        /// <summary>
        /// WHDR_DONE
        /// Set by the device driver to indicate that it is finished with the buffer and is returning it to the application.
        /// </summary>
        Done = 0x00000001,
        /// <summary>
        /// WHDR_PREPARED
        /// Set by Windows to indicate that the buffer has been prepared with the waveInPrepareHeader or waveOutPrepareHeader function.
        /// </summary>
        Prepared = 0x00000002,
        /// <summary>
        /// WHDR_BEGINLOOP
        /// This buffer is the first buffer in a loop.  This flag is used only with output buffers.
        /// </summary>
        BeginLoop = 0x00000004,
        /// <summary>
        /// WHDR_ENDLOOP
        /// This buffer is the last buffer in a loop.  This flag is used only with output buffers.
        /// </summary>
        EndLoop = 0x00000008,
        /// <summary>
        /// WHDR_INQUEUE
        /// Set by Windows to indicate that the buffer is queued for playback.
        /// </summary>
        InQueue = 0x00000010,
    }

    /// <summary>
    /// WaveHeader interop structure (WAVEHDR)
    /// http://msdn.microsoft.com/en-us/library/dd743837%28VS.85%29.aspx
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public class WaveHeader
    {
        /// <summary>
        /// Pointer to the waveform buffer. (lpData)
        /// </summary>
        public IntPtr dataBuffer;
        /// <summary>
        /// Length, in bytes, of the buffer. (dwBufferLength)
        /// </summary>
        public int bufferLength;
        /// <summary>
        /// When the header is used in input, specifies how much data is in the buffer. (dwBytesRecorded)
        /// </summary>
        public int bytesRecorded;
        /// <summary>
        /// User data. (dwUser)
        /// </summary>
        public IntPtr userData;
        /// <summary>
        /// Assorted flags (dwFlags)
        /// </summary>
        public WaveHeaderFlags flags;
        /// <summary>
        /// Number of times to play the loop. This member is used only with output buffers. (dwLoops)
        /// </summary>
        public int loops;
        /// <summary>
        /// Reserved. (lpNext)
        /// </summary>
        public IntPtr next;
        /// <summary>
        /// Reserved.
        /// </summary>
        public IntPtr reserved;
    }
}
