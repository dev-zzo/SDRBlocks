using System;

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
            SourcePin output = new SourcePin();
            output.SignalAttachedEvent += new PinEvent(this.OnOutputSignalAttached);
            output.ConsumedEvent += new SignalConsumedDelegate(this.OnConsumed);

            this.Output = output;
            this.Amplitude = amplitude;
            this.Frequency = freq;

            this.currentAngle = 0.0f;
        }

        public ISourcePin Output { get; private set; }

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
                this.UpdateSampleStep();
            }
        }

        #region Implementation details

        private double currentAngle;
        private float frequency;
        private double sampleStep;

        private void OnOutputSignalAttached()
        {
            this.UpdateSampleStep();
            this.OnConsumed();
        }

        private void OnConsumed()
        {
            this.RefillIQ();
        }

        private void UpdateSampleStep()
        {
            if (this.Output.AttachedSignal != null)
            {
                this.sampleStep = this.Frequency * 2.0 * Math.PI / this.Output.AttachedSignal.FrameRate;
            }
        }

        /// <summary>
        /// Refill the buffer with quadrature signal.
        /// </summary>
        private unsafe void RefillIQ()
        {
            Signal signal = this.Output.AttachedSignal;
            Complex* data = (Complex*)signal.Data.ToPointer();
            for (int i = signal.FrameCount; i < signal.Size; ++i)
            {
                data[i] = FastMath.SinCos(this.currentAngle) * this.Amplitude;
                this.currentAngle += this.sampleStep;
                while (this.currentAngle > FastMath.TWOPI)
                {
                    this.currentAngle -= FastMath.TWOPI;
                }
            }
            signal.NotifyOnRefill(signal.Size - signal.FrameCount);
        }

        #endregion
    }
}
