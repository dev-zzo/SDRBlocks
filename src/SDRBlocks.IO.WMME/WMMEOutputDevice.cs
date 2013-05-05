using System;
using System.Runtime.InteropServices;
using SDRBlocks.Core;
using SDRBlocks.Core.Interop;
using SDRBlocks.IO.WMME.Interop;
using System.Threading;

namespace SDRBlocks.IO.WMME
{
    public sealed class WMMEOutputDevice : DspBlockBase
    {
        public WMMEOutputDevice(int deviceIndex, uint channels, uint frameRate)
        {
            this.FrameRate = frameRate;
            this.ChannelCount = channels;

            this.Input = new StreamInputSimple();
            this.Inputs.Add(this.Input);

            // Try FP format.
            WaveFormat format = WaveFormat.CreateIeeeFloatWaveFormat((int)frameRate, (int)channels);
            this.Open(deviceIndex, ref format);

            this.bufferPumpThread = new Thread(this.BufferPumpProc);
            this.bufferPumpThread.Start();
        }

        public uint FrameRate { get; private set; }

        public uint ChannelCount { get; private set; }

        public IStreamInput Input { get; private set; }

        #region Implementation details

        protected override void Dispose(bool disposing)
        {
            this.Close();
            base.Dispose(disposing);
        }

        private IntPtr hWaveOut;
        private bool isClosing;
        private WaveBuffer[] buffers;
        private readonly AutoResetEvent queueEmptyEvent = new AutoResetEvent(false);
        private readonly Thread bufferPumpThread;
        private readonly AutoResetEvent bufferAvailableEvent = new AutoResetEvent(false);

        private void Open(int deviceIndex, ref WaveFormat format)
        {
            this.isClosing = false;

            MmResult rv = Wave.waveOutOpen(
                out this.hWaveOut,
                (IntPtr)deviceIndex,
                ref format,
                this.bufferAvailableEvent.SafeWaitHandle,
                IntPtr.Zero,
                WaveOpenFlags.CallbackEvent);
            if (rv != MmResult.NoError)
            {
                throw new WMMEException(rv);
            }
            
            this.buffers = new WaveBuffer[2];
            for (int i = 0; i < this.buffers.Length; ++i)
            {
                WaveBuffer buffer = new WaveBufferOut(this.hWaveOut, 22000, 8);
                this.buffers[i] = buffer;
                RefillAndSubmitBuffer(buffer);
            }
        }

        private void Close()
        {
            if (this.hWaveOut != IntPtr.Zero)
            {
                this.isClosing = true;
                //Wave.waveOutReset(this.hWaveOut);
                this.queueEmptyEvent.WaitOne();
                foreach (WaveBuffer buffer in this.buffers)
                {
                    buffer.Dispose();
                }
                Wave.waveOutClose(this.hWaveOut);
                this.hWaveOut = IntPtr.Zero;
            }
        }

        private void RefillAndSubmitBuffer(WaveBuffer waveBuffer)
        {
            uint frameSize = this.ChannelCount * sizeof(float);
            uint frameCount = waveBuffer.Size;

            if (this.Input.AttachedOutput != null)
            {
                FrameBuffer frameBuffer = this.Input.AttachedOutput.Buffer;

                uint framesAvailable = frameBuffer.FrameCount;
                uint framesToCopy = Math.Min(frameCount, framesAvailable);
                MemFuncs.memcpy(waveBuffer.Buffer, frameBuffer.Ptr, (UIntPtr)(frameSize * framesToCopy));
                frameBuffer.Consume(framesToCopy);

                if (framesToCopy < frameCount)
                {
                    uint framesToZero = frameCount - framesToCopy;
                    IntPtr ptr = frameBuffer.Ptr + (int)(frameSize * framesToCopy);
                    MemFuncs.memset(ptr, 0, (UIntPtr)(frameSize * framesToZero));
                }
            }
            else
            {
                MemFuncs.memset(waveBuffer.Buffer, 0, (UIntPtr)(frameSize * frameCount));
            }

            waveBuffer.SubmitBuffer();
        }

        private void BufferPumpProc()
        {
            while (!this.isClosing)
            {
                this.bufferAvailableEvent.WaitOne();
                Console.WriteLine("BufferPumpProc awakens.");
                foreach (WaveBuffer buffer in this.buffers)
                {
                    if (buffer.IsDone)
                    {
                        this.RefillAndSubmitBuffer(buffer);
                    }
                }
            }

            Console.Write("BufferPumpProc waits for all buffers.");
            bool allDone;
            do
            {
                allDone = true;
                foreach (WaveBuffer buffer in this.buffers)
                {
                    allDone &= buffer.IsDone;
                }
                Thread.Sleep(250);
                Console.Write(".");
            } while (!allDone);
            this.queueEmptyEvent.Set();
            Console.WriteLine("BufferPumpProc thread exits.");
        }

        #endregion
    }
}
