using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SDRBlocks.Core.Maths;

namespace SDRBlocks.Core.DspBlocks.Cores
{
    public enum FirFilterType
    {
        LowPass,
        HighPass,
        BandPass,
        BandReject,
    }

    public class FirFilterCore
    {
        public FirFilterCore()
        {
            this.Configure(FirFilterType.LowPass, 1000.0f, 0.0f, 11, 48000);
        }

        /// <summary>
        /// Cutoff frequency, in Hertz.
        /// </summary>
        public float Frequency { get { return this.frequency; } }

        /// <summary>
        /// Bandwidth, in Hertz.
        /// </summary>
        public float Bandwidth { get { return this.bandwidth; } }

        /// <summary>
        /// Kernel length, in points.
        /// </summary>
        public int Length { get { return this.kernel.Length; } }

        public void Configure(FirFilterType type, float frequency, float bandwidth, int length, int sampleRate)
        {
            float[] newKernel = null;

            switch (type)
            {
                case FirFilterType.LowPass:
                    newKernel = MakeLowPassKernel(frequency, length, sampleRate, WindowType.Blackman);
                    break;

                case FirFilterType.HighPass:
                    newKernel = MakeLowPassKernel(frequency, length, sampleRate, WindowType.Blackman);
                    FilterHelper.InvertSpectrum(newKernel);
                    break;

                case FirFilterType.BandPass:
                    newKernel = MakeLowPassKernel(frequency, length, sampleRate, WindowType.Blackman);

                    // Shift the filter response
                    float w = FastMath.TWOPI * frequency / sampleRate;
                    for (int i = 0; i < length; ++i)
                    {
                        var n = i - length / 2;
                        newKernel[i] *= 2.0f * (float)Math.Cos(w * n);
                    }
                    break;

                default:
                    throw new NotImplementedException();
            }

            this.type = type;
            this.frequency = frequency;
            this.bandwidth = bandwidth;
            this.kernel = newKernel;
        }

        public unsafe Complex GetNextSample(Complex* input)
        {
            fixed(float* h = this.kernel)
            {
                return Convolution.Convolve(input, h, this.kernel.Length);
            }
        }

        private FirFilterType type;
        private float frequency;
        private float bandwidth;
        private float[] kernel;

        private static float[] MakeLowPassKernel(float cutoffFrequency, int length, int sampleRate, WindowType window)
        {
            float[] newKernel = FilterHelper.BuildSincFilter(cutoffFrequency, length, sampleRate);
            FilterHelper.Multiply(newKernel, WindowFuncs.Build(window, length));
            FilterHelper.Normalize(newKernel);
            return newKernel;
        }
    }
}
