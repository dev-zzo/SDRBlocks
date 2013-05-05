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
            this.Input = new StreamInputSimple();
            this.Inputs.Add(this.Input);
        }

        public IStreamInput Input { get; private set; }

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
            uint frameCount = waveBuffer.Size;

            if (this.Input.AttachedOutput != null)
            {
                FrameBuffer frameBuffer = this.Input.AttachedOutput.Buffer;

                uint framesAvailable = frameBuffer.FrameCount;
                uint framesToCopy = Math.Min(frameCount, framesAvailable);
                MemFuncs.memcpy(waveBuffer.Buffer, frameBuffer.Ptr, (UIntPtr)(this.FrameSize * framesToCopy));
                frameBuffer.Consume(framesToCopy);

                if (framesToCopy < frameCount)
                {
                    uint framesToZero = frameCount - framesToCopy;
                    IntPtr ptr = frameBuffer.Ptr + (int)(this.FrameSize * framesToCopy);
                    MemFuncs.memset(ptr, 0, (UIntPtr)(this.FrameSize * framesToZero));
                }
            }
            else
            {
                MemFuncs.memset(waveBuffer.Buffer, 0, (UIntPtr)(this.FrameSize * frameCount));
            }

            waveBuffer.Submit();
        }

        protected override void Close()
        {
            if (this.hWave != IntPtr.Zero)
            {
                this.isClosing = true;
                this.queueEmptyEvent.WaitOne();
                foreach (WaveBuffer buffer in this.buffers)
                {
                    buffer.Dispose();
                }
                Wave.waveOutClose(this.hWave);
                this.hWave = IntPtr.Zero;
            }
        }

        #endregion
    }
}
