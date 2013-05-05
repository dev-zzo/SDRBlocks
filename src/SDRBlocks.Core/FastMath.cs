using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDRBlocks.Core
{
    /// <summary>
    /// A tradeoff implementation of math routines favoring speed.
    /// TODO: Implement lerp of the two values for added precision.
    /// </summary>
    public static class FastMath
    {
        public const float TWOPI = (float)(2.0 * Math.PI);

        /// <summary>
        /// Compute sine of the argument.
        /// </summary>
        /// <param name="x">Argument, in radians.</param>
        /// <returns></returns>
        public static float Sin(float x)
        {
            return sinSamples[ArgToIndex(x)];
        }

        /// <summary>
        /// Compute cosine of the argument.
        /// </summary>
        /// <param name="x">Argument, in radians.</param>
        /// <returns></returns>
        public static float Cos(float x)
        {
            return cosSamples[ArgToIndex(x)];
        }

        /// <summary>
        /// Compute both sine and cosine of the argument.
        /// </summary>
        /// <param name="x">Argument, in radians.</param>
        /// <returns>Complex value {cos, sin}.</returns>
        public static Complex SinCos(float x)
        {
            int i = ArgToIndex(x);
            return new Complex(cosSamples[i], sinSamples[i]);
        }

        public static Complex SinCos(double x)
        {
            int i = ArgToIndex(x);
            return new Complex(cosSamples[i], sinSamples[i]);
        }

        #region Implementation details

        static FastMath()
        {
            int sampleCount = 1 + (1 << RESOLUTION_BITS);

            sinSamples = new float[sampleCount];
            cosSamples = new float[sampleCount];

            double indexToAngle = 2 * Math.PI / (sampleCount - 1);
            for (int i = 0; i < sampleCount; ++i)
            {
                double a = i * indexToAngle;
                sinSamples[i] = (float)Math.Sin(a);
                cosSamples[i] = (float)Math.Cos(a);
            }
        }

        private const int RESOLUTION_BITS = 16;
        private const int ARG_MASK = (1 << RESOLUTION_BITS) - 1;
        private const float ARG_SCALE = (1 << RESOLUTION_BITS) / TWOPI;
        private static readonly float[] sinSamples;
        private static readonly float[] cosSamples;

        private static int ArgToIndex(float x)
        {
            return (int)(x * ARG_SCALE) & ARG_MASK;
        }

        private static int ArgToIndex(double x)
        {
            return (int)(x * ARG_SCALE) & ARG_MASK;
        }

        #endregion
    }
}
