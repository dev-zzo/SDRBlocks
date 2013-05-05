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
        private AutoResetEvent queueEmptyEvent = new AutoResetEvent(false);

        private void Open(int deviceIndex, ref WaveFormat format)
        {
            this.isClosing = false;

            MmResult rv = Wave.waveOutOpen(
                out this.hWaveOut,
                (IntPtr)deviceIndex,
                ref format,
                new WaveCallback(this.CallbackProc),
                IntPtr.Zero,
                WaveOpenFlags.CallbackFunction);
            if (rv != MmResult.NoError)
            {
                throw new WMMEException(rv);
            }
            
            this.buffers = new WaveBuffer[3];
            for (int i = 0; i < this.buffers.Length; ++i)
            {
                WaveBuffer buffer = new WaveBufferOut(this.hWaveOut, 44100, 8);
                this.buffers[i] = buffer;
                RefillAndSubmitBuffer(buffer);
            }
        }

        private void Close()
        {
            if (this.hWaveOut != IntPtr.Zero)
            {
                this.isClosing = true;
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

        private void CallbackProc(IntPtr hWaveOut, WaveMessage message, IntPtr instance, IntPtr param1, IntPtr param2)
        {
            Console.WriteLine("Wave output callback called.");

            switch (message)
            {
                case WaveMessage.WaveOutDone:
                    if (this.isClosing)
                    {
                        bool queueEmpty = false;
                        foreach (WaveBuffer buffer in this.buffers)
                        {
                            queueEmpty |= buffer.InQueue;
                        }
                        if (queueEmpty)
                        {
                            this.queueEmptyEvent.Set();
                        }
                    }
                    else
                    {
                        WaveHeader wavhdr = new WaveHeader();
                        Marshal.PtrToStructure(param1, wavhdr);
                        GCHandle bufferHandle = GCHandle.FromIntPtr(wavhdr.userData);
                        WaveBuffer buffer = (WaveBuffer)bufferHandle.Target;
                        RefillAndSubmitBuffer(buffer);
                    }
                    break;

                default:
                    break;
            }
        }

        #endregion
    }
}
