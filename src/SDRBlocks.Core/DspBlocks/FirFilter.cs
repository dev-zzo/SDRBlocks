using System;
using SDRBlocks.Core.DspBlocks.Cores;
using SDRBlocks.Core.Maths;

namespace SDRBlocks.Core.DspBlocks
{
    public class FirFilter : IDspBlock
    {
        public FirFilter()
        {
            this.Input = new SinkPin(this);
            this.Output = new SourcePin(this);

            this.Type = FirFilterType.LowPass;
            this.CutoffFrequency = 1000.0f;
            this.Length = 51;
        }

        public SinkPin Input { get; private set; }

        public SourcePin Output { get; private set; }

        public FirFilterType Type
        {
            get { return this.type; }
            set
            {
                this.type = value;
                this.dirty = true;
            }
        }

        public float CutoffFrequency
        {
            get { return this.frequency; }
            set
            {
                this.frequency = value;
                this.dirty = true;
            }
        }

        public int Length
        {
            get { return this.length; }
            set
            {
                this.length = value;
                this.dirty = true;
            }
        }

        public bool IsIndependent
        {
            get { return false; }
        }

        public bool IsReadyToProcess
        {
            get { return true; }
        }

        public unsafe void Process()
        {
            if (!this.Input.IsConnected || !this.Output.IsConnected)
                return;

            Signal sInput = this.Input.AttachedSignal;
            Signal sOutput = this.Output.AttachedSignal;

            if (this.dirty)
            {
                this.core.Configure(this.type, this.frequency, 0.0f, this.length, sInput.FrameRate);
                this.dirty = false;
            }

            if (sInput.FrameCount < this.core.Length)
                return;

            int framesToProcess = Math.Min(sInput.FrameCount - this.core.Length + 1, sOutput.Size - sOutput.FrameCount);

            switch (sInput.Format)
            {
                case FrameFormat.Complex:
                    {
                        Complex* inputBuffer = (Complex*)sInput.Data.ToPointer();
                        Complex* outputBuffer = (Complex*)sOutput.Data.ToPointer() + sOutput.FrameCount;
                        for (int i = 0; i < framesToProcess; ++i)
                        {
                            outputBuffer[i] = this.core.GetNextSample(inputBuffer + i);
                        }
                    }
                    break;

                case FrameFormat.Float32:
                    {
                        float* inputBuffer = (float*)sInput.Data.ToPointer();
                        float* outputBuffer = (float*)sOutput.Data.ToPointer() + sOutput.FrameCount;
                        int stride = sInput.ChannelCount;
                        if (stride == 1)
                        {
                            for (int i = 0; i < framesToProcess; ++i)
                            {
                                outputBuffer[i] = this.core.GetNextSample(inputBuffer + i);
                            }
                        }
                        else
                        {
                            for (int i = 0; i < framesToProcess; ++i)
                            {
                                outputBuffer[i] = this.core.GetNextSample(inputBuffer + i * stride, stride);
                            }
                        }
                    }
                    break;
            }

            sInput.Consumed(framesToProcess);
            sOutput.Refilled(framesToProcess);
        }

        private readonly FirFilterCore core = new FirFilterCore();
        private bool dirty = false;
        private FirFilterType type;
        private float frequency;
        private int length;
    }
}
