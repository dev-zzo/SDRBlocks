using System.Runtime.InteropServices;
using SDRBlocks.Core.Maths;

namespace SDRBlocks.Core
{
    public enum FrameFormat
    {
        Float32,
        Complex,
    }

    public static class FrameFormatExtensions
    {
        public static int Size(this FrameFormat ff)
        {
            switch (ff)
            {
                case FrameFormat.Float32:
                    return sizeof(float);
                case FrameFormat.Complex:
                    return Marshal.SizeOf(typeof(Complex));
                default:
                    throw new SDRBlocksException("Invalid enumeration value");
            }
        }
    }
}
