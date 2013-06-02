using System;
using SDRBlocks.Core;
using SDRBlocks.Core.Interop;
using SDRBlocks.IO.WMME.Interop;

namespace SDRBlocks.IO.WMME
{
    public sealed class WMMEOutputDevice : WMMEAudioDevice
    {
        public WMMEOutputDevice(int deviceIndex, uint channels, uint frameRate)
            : base(deviceIndex, channels, frameRate, 3, 2048 * (frameRate / 48000))
        {
            this.Input = new SinkPin(this);
        }

        public SinkPin Input { get; private set; }

        public override bool IsIndependent
        {
            get { return false; }
        }

        public override void Process()
        {
            lock (this.availableBuffers)
            {
                while (this.availableBuffers.Count > 0)
                {
                    WaveBuffer waveBuffer = this.availableBuffers.Dequeue();

                    if (this.Input.IsConnected)
                    {
                        Signal signal = this.Input.AttachedSignal;
                        int frameCount = (int)waveBuffer.Size;

                        if (signal.FrameCount >= frameCount)
                        {
                            MemFuncs.MemCopy(waveBuffer.Buffer, signal.Data, (UIntPtr)(this.FrameSize * frameCount));
                            signal.Consumed(frameCount);
                        }
                        else
                        {
                            this.availableBuffers.Enqueue(waveBuffer);
                            break;
                        }
                    }
                    else
                    {
                        MemFuncs.MemSet(waveBuffer.Buffer, 0, (UIntPtr)(this.FrameSize * waveBuffer.Size));
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

        protected override void Close()
        {
            if (this.hWave != IntPtr.Zero)
            {
                this.isClosing = true;
                //Wave.waveOutReset(this.hWave);
                this.ReleaseBuffers();
                Wave.waveOutClose(this.hWave);
                this.hWave = IntPtr.Zero;
            }
        }

        #endregion
    }
}
