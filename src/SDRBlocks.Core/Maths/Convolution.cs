﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDRBlocks.Core.Maths
{
    public unsafe static class Convolution
    {
        /// <summary>
        /// Convolution, output-side algorithm.
        /// NOTE: The input buffer has to contain (sampleCount + kernelSize - 1) samples.
        /// NOTE: It is up to the user to appropriately pad the input buffer with zeros at the beginning.
        /// </summary>
        /// <param name="input">Input data</param>
        /// <param name="kernel">Convolution kernel</param>
        /// <param name="kernelSize">Convoltion kernel size, samples</param>
        /// <param name="output">Output buffer, of sampleCount size</param>
        /// <param name="sampleCount">Output sample count</param>
        public static void Convolve(Complex* input, Complex* kernel, int kernelSize, Complex* output, int sampleCount)
        {
            for (int o = 0; o < sampleCount; ++o)
            {
                Complex s = Complex.Zero;
                for (int h = kernelSize - 1; h >= 0; --h)
                {
                    s += input[o + h] * kernel[h];
                }
                output[o] = s;
            }
        }

        public static void Convolve(Complex* input, float* kernel, int kernelSize, Complex* output, int sampleCount)
        {
            for (int o = 0; o < sampleCount; ++o)
            {
                output[o] = Convolve(input + o, kernel, kernelSize);
            }
        }

        /// <summary>
        /// Convolution, output-side algorithm (one output sample version).
        /// NOTE: The input buffer has to contain kernelSize samples.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="kernel"></param>
        /// <param name="kernelSize"></param>
        /// <returns></returns>
        public static Complex Convolve(Complex* input, float* kernel, int kernelSize)
        {
            Complex s = Complex.Zero;
            for (int i = 0; i < kernelSize; ++i)
            {
                s += input[i] * kernel[kernelSize - i];
            }
            return s;
        }

        public static float Convolve(float* input, float* kernel, int kernelSize)
        {
            float s = 0.0f;
            for (int i = 0; i < kernelSize; ++i)
            {
                s += input[i] * kernel[kernelSize - i];
            }
            return s;
        }

        public static float Convolve(float* input, int stride, float* kernel, int kernelSize)
        {
            float s = 0.0f;
            for (int i = 0; i < kernelSize; ++i)
            {
                s += input[i * stride] * kernel[kernelSize - i];
            }
            return s;
        }
    }
}
