using System;
using System.Threading;
using SDRBlocks.Core;
using SDRBlocks.IO.WMME.Interop;

namespace SDRBlocks.IO.WMME
{
    public abstract class WMMEAudioDevice : DspBlockBase
    {
        public WMMEAudioDevice(int deviceIndex, uint channels, uint frameRate, uint bufferCount, uint framesPerBuffer)
        {
            this.FrameRate = frameRate;
            this.ChannelCount = channels;

            this.bufferPumpThread = new Thread(this.BufferPumpProc);
            this.bufferPumpThread.Start();

            WaveFormat format = WaveFormat.CreateIeeeFloatWaveFormat((int)frameRate, (int)channels);
            this.Open(deviceIndex, ref format);

            this.CreateBuffers(bufferCount, framesPerBuffer, channels * sizeof(float));
        }

        public uint FrameRate { get; private set; }

        public uint ChannelCount { get; private set; }

        protected uint FrameSize { get { return this.ChannelCount * sizeof(float); } }

        protected bool isClosing;
        protected IntPtr hWave;
        protected WaveBuffer[] buffers;
        protected readonly AutoResetEvent bufferAvailableEvent = new AutoResetEvent(false);
        protected readonly AutoResetEvent queueEmptyEvent = new AutoResetEvent(false);

        protected abstract void Open(int deviceIndex, ref WaveFormat format);
        protected abstract WaveBuffer CreateBuffer(IntPtr hWave, uint numFrames, uint frameSize);
        protected abstract void ProcessBuffer(WaveBuffer waveBuffer);
        protected abstract void Close();

        protected override void Dispose(bool disposing)
        {
            this.Close();
            base.Dispose(disposing);
        }

        private readonly Thread bufferPumpThread;

        private void CreateBuffers(uint bufferCount, uint framesPerBuffer, uint frameSize)
        {
            if (this.hWave == IntPtr.Zero)
            {
                throw new WMMEException("The device has to be opened prior to buffer creation.");
            }

            this.buffers = new WaveBuffer[bufferCount];
            for (int i = 0; i < this.buffers.Length; ++i)
            {
                WaveBuffer buffer = this.CreateBuffer(this.hWave, framesPerBuffer, frameSize);
                this.buffers[i] = buffer;
                buffer.SubmitBuffer();
            }
        }

        private void BufferPumpProc()
        {
            while (!this.isClosing)
            {
                this.bufferAvailableEvent.WaitOne();
                foreach (WaveBuffer buffer in this.buffers)
                {
                    if (buffer.IsDone)
                    {
                        this.ProcessBuffer(buffer);
                    }
                }
            }

            for (bool allDone = true; allDone; )
            {
                foreach (WaveBuffer buffer in this.buffers)
                {
                    allDone &= buffer.IsDone;
                }
                if (!allDone)
                {
                    Thread.Sleep(250);
                }
            }
            this.queueEmptyEvent.Set();
        }

    }
}
