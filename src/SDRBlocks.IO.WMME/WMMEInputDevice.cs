using System;
using SDRBlocks.Core;
using SDRBlocks.IO.WMME.Interop;
using SDRBlocks.Core.Interop;

namespace SDRBlocks.IO.WMME
{
    public sealed class WMMEInputDevice : WMMEAudioDevice
    {
        public WMMEInputDevice(int deviceIndex, uint channels, uint frameRate)
            : base(deviceIndex, channels, frameRate, 3, 2048)
        {
            SourcePin output = new SourcePin();
            this.Output = output;
        }

        public ISourcePin Output { get; private set; }

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

        protected override void ProcessBuffer(WaveBuffer waveBuffer)
        {
            //Console.WriteLine("Input::ProcessBuffer");
            if (this.Output.IsConnected)
            {
                Signal signal = this.Output.AttachedSignal;
                IntPtr dest = signal.Data + signal.FrameCount * signal.FrameSize;
                int framesToCopy = Math.Min((int)waveBuffer.Size, signal.Size - signal.FrameCount);
                MemFuncs.memcpy(dest, waveBuffer.Buffer, (UIntPtr)(framesToCopy * signal.FrameSize));
                signal.NotifyOnRefill(framesToCopy);
            }

            waveBuffer.Submit();
        }

        protected override void Close()
        {
            if (this.hWave != IntPtr.Zero)
            {
                this.isClosing = true;
                Wave.waveInStop(this.hWave);
                this.ReleaseBuffers();
                Wave.waveInClose(this.hWave);
                this.hWave = IntPtr.Zero;
            }
        }

        #endregion
    }
}
