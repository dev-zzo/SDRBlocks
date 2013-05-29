using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDRBlocks.Core.Maths
{
    public unsafe static class Resample
    {
        public static void Downsample(Complex* input, int length, int m, Complex* output)
        {
            int i = 0;
            int o = 0;
            while (i < length)
            {
                output[o] = input[i];
                i += m + 1;
                o += 1;
            }
        }

        public static void Downsample(float* input, int length, int m, float* output)
        {
            int i = 0;
            int o = 0;
            while (i < length)
            {
                output[o] = input[i];
                i += m + 1;
                o += 1;
            }
        }

        public static void Upsample(Complex* input, int length, int l, Complex* output)
        {
            int i = 0;
            int o = 0;
            while (i < length)
            {
                for (int t = 0; t < l; ++t)
                {
                    output[o + t] = input[i];
                }
                i += 1;
                o += l + 1;
            }
        }
    }
}
