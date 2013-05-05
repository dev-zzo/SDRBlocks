using System;

namespace SDRBlocks.Core.DspBlocks
{
    public class Oscillator : DspBlockBase
    {
        public Oscillator(uint sampleRate)
            : this(sampleRate, 0.6667f, 1000.0f)
        {
        }

        public Oscillator(uint sampleRate, float amplitude, float freq)
        {
            this.SampleRate = sampleRate;
            this.Amplitude = amplitude;
            this.Frequency = freq;

            StreamOutputSimple iq = new StreamOutputSimple(sampleRate, 1, FrameFormat.Complex, 65536);
            iq.ConsumedEvent += new OutputBufferConsumedDelegate(OnOutputConsumed);
            this.IQ = iq;
            this.Outputs.Add(iq);

            this.currentAngle = 0.0f;
            this.Refill();
        }

        public IStreamOutput IQ { get; private set; }

        public uint SampleRate { get; private set; }

        public float Amplitude { get; set; }

        public float Frequency
        {
            get
            {
                return this.frequency;
            }
            set
            {
                this.frequency = value;
                this.sampleStep = value * 2.0 * Math.PI / this.SampleRate;
            }
        }

        #region Implementation details

        protected virtual void OnOutputConsumed(IStreamOutput sender)
        {
            this.Refill();
        }

        protected unsafe void Refill()
        {
            var buffer = this.IQ.Buffer;
            Complex* data = (Complex*)buffer.Ptr.ToPointer();
            for (uint i = buffer.FrameCount; i < buffer.Size; ++i)
            {
                data[i] = FastMath.SinCos(this.currentAngle) * this.Amplitude;
                this.currentAngle += this.sampleStep;
                while (this.currentAngle > FastMath.TWOPI)
                {
                    this.currentAngle -= FastMath.TWOPI;
                }
            }
            buffer.FrameCount = buffer.Size;
        }

        private double currentAngle;
        private float frequency;
        private double sampleStep;

        #endregion
    }
}
