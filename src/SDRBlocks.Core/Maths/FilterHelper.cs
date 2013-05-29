using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDRBlocks.Core.Maths
{
    public static class FilterHelper
    {
        /// <summary>
        /// Builds a sinc filter kernel.
        /// </summary>
        /// <param name="cutoffFreq"></param>
        /// <param name="length"></param>
        /// <param name="sampleRate"></param>
        /// <returns></returns>
        public static float[] BuildSincFilter(float cutoffFreq, int length, int sampleRate)
        {
            if (length % 2 != 0)
            {
                throw new ArgumentException("Length must be odd.");
            }

            float[] h = new float[length];
            double w = 2.0 * Math.PI * cutoffFreq / sampleRate;

            for (int i = 0; i < length; ++i)
            {
                int n = i - length / 2;
                if (n == 0)
                {
                    h[i] = (float)w;
                }
                else
                {
                    h[i] = (float)(Math.Sin(w * n) / n);
                }
            }

            return h;
        }

        #region Filter kernel operations

        /// <summary>
        /// Normalize the filter kernel for the gain of 1 at DC.
        /// </summary>
        /// <param name="h">Filter kernel</param>
        public static void Normalize(float[] h)
        {
            int length = h.Length;
            float sum = 0.0f;
            for (int i = 0; i < length; ++i)
            {
                sum += h[i];
            }
            sum = 1 / sum;
            for (int i = 0; i < length; ++i)
            {
                h[i] *= sum;
            }
        }

        /// <summary>
        /// Multiply each element of hc with the corresponding element of hw.
        /// </summary>
        /// <param name="hc">Kernel to be modified (usually filter)</param>
        /// <param name="hw">Kernel to apply (usually window)</param>
        public static void Multiply(float[] hc, float[] hw)
        {
            if (hc.Length != hw.Length)
            {
                throw new ArgumentException("Kernel lengths must match.");
            }

            int length = hc.Length;
            for (int i = 0; i < length; ++i)
            {
                hc[i] *= hw[i];
            }
        }

        /// <summary>
        /// Perform spectral inversion of the given filter kernel.
        /// </summary>
        /// <param name="h"></param>
        /// <returns></returns>
        public static void InvertSpectrum(float[] h)
        {
            if (h.Length % 2 != 0)
            {
                throw new ArgumentException("Kernel length must be odd.");
            }

            int length = h.Length;
            for (var i = 0; i < length; i++)
            {
                h[i] = -h[i];
            }
            h[(length - 1) / 2] += 1.0f;
        }

        /// <summary>
        /// Perform spectral reversal of the given filter kernel.
        /// </summary>
        /// <param name="h"></param>
        public static void ReverseSpectrum(float[] h)
        {
            if (h.Length % 2 != 0)
            {
                throw new ArgumentException("Kernel length must be odd.");
            }

            int length = h.Length;
            for (var i = 0; i < length; i += 2)
            {
                h[i] = -h[i];
            }
        }

        #endregion
    }
}
