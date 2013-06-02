using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SDRBlocks.Core.Maths;

namespace SDRBlocks.Core.DspBlocks.Cores
{
    /// <summary>
    /// Oscillator core functionality.
    /// Factored out to avoid code duplication in cases where this needs to be embedded into other blocks.
    /// </summary>
    public class OscillatorCore
    {
        public OscillatorCore()
        {
            this.phase = 0.0f;

            // Sane defaults.
            this.Amplitude = 1.0f;
            this.Frequency = 1000.0f;
            this.SampleRate = 48000;
        }

        /// <summary>
        /// Oscillator amplitude, 1.0f being full swing.
        /// </summary>
        public float Amplitude { get; set; }

        /// <summary>
        /// Oscillator frequency, in Hertz.
        /// </summary>
        public float Frequency { get; set; }

        /// <summary>
        /// Sample rate of the output samples.
        /// </summary>
        public int SampleRate
        {
            get { return this.sampleRate; }
            set
            {
                this.sampleRate = value;
                this.phaseDelta = (float)(this.Frequency * 2.0 * Math.PI / value);
            }
        }

        /// <summary>
        /// Generate a sample and update the internal state.
        /// </summary>
        /// <returns></returns>
        public Complex GetNextSample()
        {
            Complex sample = FastMath.SinCos(this.phase) * this.Amplitude;
            this.phase += phaseDelta;
            while (this.phase > FastMath.TWOPI)
            {
                this.phase -= FastMath.TWOPI;
            }
            return sample;
        }

        #region Implementation details

        private int sampleRate;
        private float phase;
        private float phaseDelta;

        #endregion
    }
}
