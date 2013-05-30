﻿using System;
using SDRBlocks.Core.Maths;

namespace SDRBlocks.Core.DspBlocks
{
    /// <summary>
    /// Quadrature FM detector.
    /// Note that it does not perform any DC removal, filtering or other ops.
    /// </summary>
    public class FmDetector : IDspBlock
    {
        public FmDetector()
        {
            this.Input = new SinkPin(this);
            this.Output = new SourcePin(this);
            // TODO: get a proper estimate for gain.
            this.Gain = 0.00001f;
            this.storedState = Complex.RealOne;
        }

        /// <summary>
        /// Complex IQ input.
        /// </summary>
        public SinkPin Input { get; private set; }

        /// <summary>
        /// Scalar output.
        /// </summary>
        public SourcePin Output { get; private set; }

        /// <summary>
        /// Signal gain (linear).
        /// </summary>
        public float Gain { get; set; }

        #region IDspBlock implementation

        public bool IsIndependent { get { return false; } }

        public bool IsReadyToProcess
        {
            get 
            {
                if (!this.Input.IsConnected)
                    return false;
                if (this.Input.AttachedSignal.FrameCount < 1)
                    return false;
                return true;
            }
        }

        public unsafe void Process()
        {
            if (!this.Input.IsConnected || !this.Output.IsConnected)
                return;

            // Get the input buffer
            Signal sInput = this.Input.AttachedSignal;
            Complex* inputBuffer = (Complex*)sInput.Data.ToPointer();
            // Get the output buffer
            // Note that it is shifted by the frame count.
            Signal sOutput = this.Output.AttachedSignal;
            float* outputBuffer = (float*)sOutput.Data.ToPointer() + sOutput.FrameCount;
            // See how many samples we can process.
            int framesToProcess = Math.Min(sInput.FrameCount, sOutput.Size - sOutput.FrameCount);
            Complex state = this.storedState;
            for (int i = 0; i < framesToProcess; ++i)
            {
                // Let s0 = state, s1 = input[i].
                // Then: s0 = M0*e^(j*phi0)
                // s1 = M*e^(j*phi)
                // f = M0*M*e^(j*phi - j*phi0)
                // Thus, arg(f) is the measure of phase change between s0 and s1.
                Complex f = inputBuffer[i] * ~state;

                // Angle estimate is our signal.
                outputBuffer[i] = f.Arg() * this.Gain;

                // Keep the value for the next iteration
                state = inputBuffer[i];
            }

            // Update the stored state only once.
            this.storedState = state;
        }

        #endregion

        private Complex storedState;
    }
}
