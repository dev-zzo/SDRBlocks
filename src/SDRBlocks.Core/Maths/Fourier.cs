using System;
using SDRBlocks.Core.Interop;

namespace SDRBlocks.Core.Maths
{
    public static class Fourier
    {
        public unsafe static void ForwardTransformCorr(IntPtr input, IntPtr output, int length)
        {
            // Naive approach: DFT by correlation.
            // Complexity is: O(n^2)
            MemFuncs.MemSet(output, 0, (UIntPtr)(sizeof(Complex) * length));
            Complex* pIn = (Complex*)input.ToPointer();
            Complex* pOut = (Complex*)output.ToPointer();

            for (int k = 0; k < length; ++k)
            {
                for (int i = 0; i < length; ++i)
                {
                    double w = 2.0 * Math.PI * k * i / length;
                    // TODO: Might kill the conj and tweak signs below.
                    Complex s = ~FastMath.SinCos(w);
                    pOut[k] = pIn[i] * s;
                }
            }
        }

        public unsafe static void ForwardTransformFast(IntPtr input, IntPtr output, int length)
        {
            Complex* pIn = (Complex*)input.ToPointer();
            Complex* pOut = (Complex*)output.ToPointer();

            // m = log2(length)
            int m = 0;
            for (int i = length; i > 1; i >>= 1)
            {
                m++;
            }

            // Bit-reversal sorting
            for (uint i = 0; i < length; ++i)
            {
                pOut[FastMath.BitReverse(i)] = pIn[i];
            }

            int nm1 = length - 1;
            // Loop for each stage
            for (int l = 1; l <= m; ++l)
            {
                int le = 1 << l;
                int le2 = 1 << (l - 1);
                Complex u = new Complex(1.0f, 0.0f);
                Complex s = ~FastMath.SinCos(Math.PI / le2);
                // Loop for each sub-DFT
                for (int j = 1; j <= le2; ++j)
                {
                    for (int i = j - 1; i <= nm1; i += le)
                    {
                        int ip = i + le2;
                        // Butterfly operation
                        Complex t = pOut[ip] * u;
                        pOut[ip] = pOut[i] - t;
                        pOut[i] = pOut[i] + t;
                    }
                    u = u * s;
                }
            }
        }

        /// <summary>
        /// Reorder the freq domain data to place zero freq at the center of the array.
        /// Calling this again undoes the reordering.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="length"></param>
        public unsafe static void ReorderFreqDomain(IntPtr buffer, int length)
        {
            int nd2 = length >> 1;
            int nd4 = length >> 2;
            Complex* data = (Complex*)buffer.ToPointer();
            for (int i = 0; i < nd4; i++)
            {
                Complex tmp = data[i];
                data[i] = data[nd2 - i - 1];
                data[nd2 - i - 1] = tmp;

                tmp = data[nd2 + i];
                data[nd2 + i] = data[nd2 + nd2 - i - 1];
                data[nd2 + nd2 - i - 1] = tmp;
            }
        }
    }
}
