using System;
using System.Threading;
using SDRBlocks.Core;
using SDRBlocks.IO.WMME.Interop;

namespace SDRBlocks.IO.WMME
{
    public abstract class WMMEAudioDevice : IDspBlock, IProcessTrigger, IDisposable
    {
        public WMMEAudioDevice(int deviceIndex, uint channels, uint frameRate, uint bufferCount, uint framesPerBuffer)
        {
            this.FrameRate = frameRate;
            this.ChannelCount = channels;

            this.bufferPumpThread = new Thread(this.BufferPumpProc);
            this.bufferPumpThread.Start();

            this.CreateBuffers(bufferCount, framesPerBuffer, channels * sizeof(float));
            WaveFormat format = WaveFormat.CreateIeeeFloatWaveFormat((int)frameRate, (int)channels);
            this.Open(deviceIndex, ref format);
            this.PrepareAndSubmitBuffers();
        }

        public uint FrameRate { get; private set; }

        public uint ChannelCount { get; private set; }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #region IDspBlock implementation

        public abstract bool IsIndependent { get; }

        public bool IsReadyToProcess
        {
            // This is always false since there is no actual processing to be done.
            get { return false; }
        }

        public void Process()
        {
        }

        #endregion

        #region IProcessTrigger implementation

        public event ProcessTriggerDelegate ProcessTriggerEvent;

        #endregion

        #region Implementation details

        protected uint FrameSize { get { return this.ChannelCount * sizeof(float); } }

        protected bool isClosing;
        protected IntPtr hWave;
        protected WaveBuffer[] buffers;
        protected readonly AutoResetEvent bufferAvailableEvent = new AutoResetEvent(false);

        protected abstract void Open(int deviceIndex, ref WaveFormat format);
        protected abstract WaveBuffer CreateBuffer(uint numFrames, uint frameSize);
        protected abstract void ProcessBuffer(WaveBuffer waveBuffer);
        protected abstract void Close();

        protected void InvokeProcessTrigger()
        {
            if (this.ProcessTriggerEvent != null)
            {
                this.ProcessTriggerEvent(this);
            }
        }

        protected void ReleaseBuffers()
        {
            foreach (WaveBuffer buffer in this.buffers)
            {
                bool bufferDisposed = false;
                do
                {
                    try
                    {
                        buffer.Dispose();
                        bufferDisposed = true;
                    }
                    catch (WMMEException ex)
                    {
                        if (ex.ResultCode != MmResult.WaveStillPlaying)
                        {
                            throw;
                        }
                        else
                        {
                            Thread.Sleep(250);
                        }
                    }
                } while (!bufferDisposed);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.Close();
                }
                this.disposed = true;
            }
        }

        private bool disposed;
        private readonly Thread bufferPumpThread;

        private void CreateBuffers(uint bufferCount, uint framesPerBuffer, uint frameSize)
        {
            this.buffers = new WaveBuffer[bufferCount];
            for (int i = 0; i < this.buffers.Length; ++i)
            {
                WaveBuffer buffer = this.CreateBuffer(framesPerBuffer, frameSize);
                this.buffers[i] = buffer;
            }
        }

        private void PrepareAndSubmitBuffers()
        {
            foreach (WaveBuffer buffer in this.buffers)
            {
                buffer.Prepare(this.hWave);
                buffer.Submit();
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
        }

        #endregion
    }
}
