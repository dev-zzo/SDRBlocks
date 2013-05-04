using System.Runtime.InteropServices;

namespace SDRBlocks.Core
{
    public enum FrameFormat
    {
        Float32,
        Complex,
    }

    public static class FrameFormatExtensions
    {
        public static uint Size(this FrameFormat ff)
        {
            switch (ff)
            {
                case FrameFormat.Float32:
                    return sizeof(float);
                case FrameFormat.Complex:
                    return (uint)Marshal.SizeOf(typeof(Complex));
                default:
                    throw new SDRBlocksException("Invalid enumeration value");
            }
        }
    }
}
