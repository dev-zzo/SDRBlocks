using System;
using SDRBlocks.Core;
using SDRBlocks.Core.Interop;
using SDRBlocks.IO.WMME.Interop;

namespace SDRBlocks.IO.WMME
{
    public sealed class WMMEOutputDevice : WMMEAudioDevice
    {
        public WMMEOutputDevice(int deviceIndex, uint channels, uint frameRate)
            : base(deviceIndex, channels, frameRate, 3, 1024)
        {
            SinkPin input = new SinkPin();
            this.Input = input;
        }

        public SinkPin Input { get; private set; }

        public override bool IsIndependent
        {
            get { return false; }
        }

        #region Implementation details

        protected override void Open(int deviceIndex, ref WaveFormat format)
        {
            this.isClosing = false;

            WMMEException.Check(Wave.waveOutOpen(
                out this.hWave,
                (IntPtr)deviceIndex,
                ref format,
                this.bufferAvailableEvent.SafeWaitHandle,
                IntPtr.Zero,
                WaveOpenFlags.CallbackEvent));
        }

        protected override WaveBuffer CreateBuffer(uint numFrames, uint frameSize)
        {
            return new WaveBufferOut(numFrames, frameSize);
        }

        protected override void ProcessBuffer(WaveBuffer waveBuffer)
        {
            int frameCount = (int)waveBuffer.Size;

            if (this.Input.IsConnected)
            {
                Signal signal = this.Input.AttachedSignal;
                if (signal.FrameCount > frameCount)
                {
                    int framesAvailable = signal.FrameCount;
                    int framesToCopy = Math.Min(frameCount, framesAvailable);
                    MemFuncs.MemCopy(waveBuffer.Buffer, signal.Data, (UIntPtr)(this.FrameSize * framesToCopy));
                    signal.NotifyOnConsume(framesToCopy);
                }
            }
            else
            {
                MemFuncs.MemSet(waveBuffer.Buffer, 0, (UIntPtr)(this.FrameSize * frameCount));
            }

            waveBuffer.Submit();
            this.InvokeProcessTrigger();
        }

        protected override void Close()
        {
            if (this.hWave != IntPtr.Zero)
            {
                this.isClosing = true;
                this.ReleaseBuffers();
                Wave.waveOutClose(this.hWave);
                this.hWave = IntPtr.Zero;
            }
        }

        #endregion
    }
}
