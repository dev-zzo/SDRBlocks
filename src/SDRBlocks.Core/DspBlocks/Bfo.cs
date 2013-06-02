using System;
using SDRBlocks.Core.DspBlocks.Cores;
using SDRBlocks.Core.Maths;

namespace SDRBlocks.Core.DspBlocks
{
    /// <summary>
    /// Different filtering to be performed in the BFO.
    /// </summary>
    public enum BfoFilterType
    {
        LowerSideband,
        DoubleSideband,
        UpperSideband,
    }

    /// <summary>
    /// Complex unit designed to act similar to beat frequency oscillator/mixer.
    /// </summary>
    public class Bfo : IDspBlock
    {
        public Bfo()
        {
            this.InputIQ = new SinkPin(this);
            this.OutputIQ = new SourcePin(this);
        }

        public SinkPin InputIQ { get; private set; }

        public SourcePin OutputIQ { get; private set; }

        public float ZeroFrequency
        {
            get { return this.zeroFreq; }
            set
            {
                this.zeroFreq = value;
                this.dirty = true;
            }
        }

        public BfoFilterType FilterType
        {
            get { return this.filterType; }
            set
            {
                this.filterType = value;
                this.dirty = true;
            }
        }

        public float FilterBandwidth
        {
            get { return this.filterBw; }
            set
            {
                this.filterBw = value;
                this.dirty = true;
            }
        }

        public bool IsIndependent
        {
            get { return false; }
        }

        public bool IsReadyToProcess
        {
            get
            {
                if (!this.InputIQ.IsConnected || !this.OutputIQ.IsConnected)
                    return false;
                Signal sInput = this.InputIQ.AttachedSignal;
                if (sInput.FrameCount < this.filter.Length)
                    return false;
                return true;
            }
        }

        public unsafe void Process()
        {
            if (!this.InputIQ.IsConnected || !this.OutputIQ.IsConnected)
                return;

            // Perform the actual core adjustments
            if (this.dirty)
            {
                this.UpdateCores();
            }

            // Get the input buffer
            Signal sInput = this.InputIQ.AttachedSignal;
            Complex* inputBuffer = (Complex*)sInput.Data.ToPointer();

            // Get the output buffer
            // Note that it is shifted by the frame count.
            Signal sOutput = this.OutputIQ.AttachedSignal;
            Complex* outputBuffer = (Complex*)sOutput.Data.ToPointer() + sOutput.FrameCount;

            // See how many samples we can process.
            int framesToProcess = Math.Min(sInput.FrameCount - this.filter.Length + 1, sOutput.Size - sOutput.FrameCount);

            for (int i = 0; i < framesToProcess; ++i)
            {
                // Apply filter
                Complex filteredSample = filter.GetNextSample(inputBuffer + i);

                // Get oscillator sample
                Complex oscSample = this.osc.GetNextSample();

                // Mix
                Complex mixedSample = filteredSample * oscSample;

                // Output
                outputBuffer[i] = mixedSample;
            }

            sInput.Consumed(framesToProcess);
        }

        #region Implementation details

        private readonly FirFilterCore filter = new FirFilterCore();
        private readonly OscillatorCore osc = new OscillatorCore();

        private bool dirty;
        private float zeroFreq;
        private BfoFilterType filterType;
        private float filterBw;

        private void UpdateCores()
        {
            this.osc.Frequency = -this.zeroFreq;

            int filterLength = 301;
            int sampleRate = this.InputIQ.AttachedSignal.FrameRate;

            float filterF = 0.0f;
            switch (this.filterType)
            {
                case BfoFilterType.LowerSideband:
                    filterF = this.zeroFreq - this.filterBw * 0.5f;
                    break;
                case BfoFilterType.DoubleSideband:
                    filterF = this.zeroFreq;
                    break;
                case BfoFilterType.UpperSideband:
                    filterF = this.zeroFreq + this.filterBw * 0.5f;
                    break;
            }
            this.filter.Configure(FirFilterType.BandPass, filterF, this.filterBw, filterLength, sampleRate);

            this.dirty = false;
        }

        #endregion
    }
}
