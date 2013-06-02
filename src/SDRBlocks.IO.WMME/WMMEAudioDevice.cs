using System;
using System.Threading;
using SDRBlocks.Core;
using SDRBlocks.IO.WMME.Interop;
using System.Collections.Generic;

namespace SDRBlocks.IO.WMME
{
    public abstract class WMMEAudioDevice : IDspBlock, IProcessTrigger, IDisposable
    {
        public WMMEAudioDevice(int deviceIndex, uint channels, uint frameRate, uint bufferCount, uint framesPerBuffer)
        {
            this.FrameRate = frameRate;
            this.ChannelCount = channels;

            this.bufferPumpThread = new Thread(this.BufferPumpProc);
            this.bufferPumpThread.Priority = ThreadPriority.Highest;
            this.bufferPumpThread.Name = "WMME Buffer Pump";
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
            get { return true; }
        }

        public virtual void Process()
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
        protected readonly Queue<WaveBuffer> availableBuffers = new Queue<WaveBuffer>();

        protected abstract void Open(int deviceIndex, ref WaveFormat format);
        protected abstract WaveBuffer CreateBuffer(uint numFrames, uint frameSize);
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

                lock (this.availableBuffers)
                {
                    // NOTE: this can potentially mess up the buffer order.
                    foreach (WaveBuffer buffer in this.buffers)
                    {
                        if (buffer.IsDone)
                        {
                            buffer.Unprepare();
                            this.availableBuffers.Enqueue(buffer);
                            // Placed here since this is the time reference generator.
                            this.InvokeProcessTrigger();
                        }
                    }
                }
            }
            Console.WriteLine("Buffer pump thread exited.");
        }

        #endregion
    }
}
