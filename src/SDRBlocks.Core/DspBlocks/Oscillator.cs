using System;
using SDRBlocks.Core.Maths;

namespace SDRBlocks.Core.DspBlocks
{
    public class Oscillator : IDspBlock
    {
        public Oscillator()
            : this(0.6667f, 1000.0f)
        {
        }

        public Oscillator(float amplitude, float freq)
        {
            this.Output = new SourcePin(this);

            this.Amplitude = amplitude;
            this.Frequency = freq;

            this.phase = 0.0f;
        }

        public SourcePin Output { get; private set; }

        public float Amplitude { get; set; }

        public float Frequency { get; set; }

        #region IDspBlock members

        public bool IsIndependent
        {
            get { return true; }
        }

        public bool IsReadyToProcess
        {
            get { return true; }
        }

        public unsafe void Process()
        {
            Signal signal = this.Output.AttachedSignal;
            if (signal != null)
            {
                Complex* data = (Complex*)signal.Data.ToPointer();
                double phaseDelta = this.Frequency * 2.0 * Math.PI / this.Output.AttachedSignal.FrameRate;
                for (int i = signal.FrameCount; i < signal.Size; ++i)
                {
                    data[i] = FastMath.SinCos(this.phase) * this.Amplitude;
                    this.phase += phaseDelta;
                    while (this.phase > FastMath.TWOPI)
                    {
                        this.phase -= FastMath.TWOPI;
                    }
                }
            }
        }

        #endregion

        private double phase;
    }
}
