using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace SDRBlocks.Misc.USBAPI.Interop
{
    internal static class Kernel32
    {
        public const uint GENERIC_READ = 0x80000000;
        public const uint GENERIC_WRITE = 0x40000000;
        public const uint FILE_SHARE_READ = 0x00000001;
        public const uint FILE_SHARE_WRITE = 0x00000002;
        public const uint OPEN_EXISTING = 0x00000003;

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr CreateFile(
            string FileName,
            UInt32 DesiredAccess,
            UInt32 ShareMode,
            IntPtr SecurityAttributes,
            UInt32 CreationDisposition,
            UInt32 FlagsAndAttributes,
            IntPtr TemplateFile);
    }
}
