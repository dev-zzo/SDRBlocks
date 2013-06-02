using System;
using SDRBlocks.Core;
using SDRBlocks.Core.Interop;
using SDRBlocks.IO.WMME.Interop;
using System.Collections.Generic;

namespace SDRBlocks.IO.WMME
{
    public sealed class WMMEInputDevice : WMMEAudioDevice
    {
        public WMMEInputDevice(int deviceIndex, uint channels, uint frameRate)
            : base(deviceIndex, channels, frameRate, 3, 2048/* * (frameRate / 48000)*/)
        {
            this.Output = new SourcePin(this);
        }

        public SourcePin Output { get; private set; }

        public override bool IsIndependent
        {
            get { return true; }
        }

        public override void Process()
        {
            lock (this.availableBuffers)
            {
                while (this.availableBuffers.Count > 0)
                {
                    WaveBuffer waveBuffer = this.availableBuffers.Dequeue();

                    if (this.Output.IsConnected)
                    {
                        Signal signal = this.Output.AttachedSignal;
                        IntPtr dest = signal.Data + signal.FrameCount * (int)this.FrameSize;
                        int framesToCopy = Math.Min((int)waveBuffer.Size, signal.Size - signal.FrameCount);
                        MemFuncs.MemCopy(dest, waveBuffer.Buffer, (UIntPtr)(this.FrameSize * framesToCopy));
                        signal.Refilled(framesToCopy);
                    }

                    waveBuffer.Prepare(this.hWave);
                    waveBuffer.Submit();
                }
            }
        }

        #region Implementation details

        protected override void Open(int deviceIndex, ref WaveFormat format)
        {
            this.isClosing = false;

            WMMEException.Check(Wave.waveInOpen(
                out this.hWave,
                (IntPtr)deviceIndex,
                ref format,
                this.bufferAvailableEvent.SafeWaitHandle,
                IntPtr.Zero,
                WaveOpenFlags.CallbackEvent));
            WMMEException.Check(Wave.waveInStart(this.hWave));
        }

        protected override WaveBuffer CreateBuffer(uint numFrames, uint frameSize)
        {
            return new WaveBufferIn(numFrames, frameSize);
        }

        protected override void Close()
        {
            if (this.hWave != IntPtr.Zero)
            {
                this.isClosing = true;
                //Wave.waveInStop(this.hWave);
                //Wave.waveInReset(this.hWave);
                this.ReleaseBuffers();
                Wave.waveInClose(this.hWave);
                this.hWave = IntPtr.Zero;
            }
        }

        #endregion
    }
}
