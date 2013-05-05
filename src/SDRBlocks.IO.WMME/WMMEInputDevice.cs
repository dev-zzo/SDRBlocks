using System;
using SDRBlocks.Core;
using SDRBlocks.IO.WMME.Interop;

namespace SDRBlocks.IO.WMME
{
    public sealed class WMMEInputDevice : WMMEAudioDevice
    {
        public WMMEInputDevice(int deviceIndex, uint channels, uint frameRate)
            : base(deviceIndex, channels, frameRate, 3, 2048)
        {
            uint outputBufferSize = 4 * 2048;
            this.Output = new StreamOutputSimple(channels, FrameFormat.Float32, frameRate, outputBufferSize);
            this.Outputs.Add(this.Output);
        }

        public IStreamOutput Output { get; private set; }

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
            if (this.Output.AttachedInput != null)
            {
                this.Output.Buffer.Refill(waveBuffer.Buffer, waveBuffer.Size);
            }

            waveBuffer.Submit();
        }

        protected override void Close()
        {
            if (this.hWave != IntPtr.Zero)
            {
                this.isClosing = true;
                Wave.waveInStop(this.hWave);
                this.queueEmptyEvent.WaitOne();
                foreach (WaveBuffer buffer in this.buffers)
                {
                    buffer.Dispose();
                }
                Wave.waveInClose(this.hWave);
                this.hWave = IntPtr.Zero;
            }
        }

        #endregion
    }
}
