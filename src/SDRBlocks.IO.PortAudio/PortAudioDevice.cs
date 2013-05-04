using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using PortAudioSharp;
using SDRBlocks.Core;

namespace SDRBlocks.IO.PortAudio
{
    public abstract class PortAudioDevice : DspBlockBase
    {
        public PortAudioDevice(int deviceIndex, uint channels, uint frameeRate)
        {
            this.paCallback = new PortAudioAPI.PaStreamCallbackDelegate(this.PaStreamCallback);
            this.FrameRate = frameeRate;
            this.ChannelCount = channels;
            InitializePaStream(deviceIndex);
        }

        public uint FrameRate { get; protected set; }

        public uint ChannelCount { get; protected set; }

        public void Start()
        {
            PortAudioAPI.PaError pe = PortAudioAPI.Pa_StartStream(this.streamHandle);
            if (pe != PortAudioAPI.PaError.paNoError)
            {
                throw new SDRBlocksPortAudioException(pe);
            }
        }

        public void Stop()
        {
            // This is REALLY ugly:
            // The 0/1 int flag gets converted into PaError along the way,
            // without appropriate enum value to cover the "1".
            if (PortAudioAPI.Pa_IsStreamStopped(this.streamHandle) != PortAudioAPI.PaError.paNoError)
            {
                PortAudioAPI.PaError pe = PortAudioAPI.Pa_StopStream(this.streamHandle);
                if (pe != PortAudioAPI.PaError.paNoError)
                {
                    throw new SDRBlocksPortAudioException(pe);
                }
            }
        }

        protected IntPtr StreamHandle { get; set; }

        protected override void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (this.streamHandle != IntPtr.Zero)
                    {
                        this.Stop();
                        PortAudioAPI.PaError pe = PortAudioAPI.Pa_CloseStream(this.streamHandle);
                        // TODO: check this value?
                    }
                }
                this.streamHandle = IntPtr.Zero;
                this.disposed = true;
            }
            base.Dispose(disposing);
        }

        protected abstract void InitializePaStream(int deviceIndex);

        protected void InitializePaStream(
            PortAudioAPI.PaStreamParameters? inputParams, 
            PortAudioAPI.PaStreamParameters? outputParams,
            uint framesPerBuffer)
        {
            double sr = this.FrameRate;

            var pe = PortAudioAPI.Pa_IsFormatSupported(inputParams, outputParams, sr);
            if (pe != PortAudioAPI.PaError.paNoError)
            {
                throw new SDRBlocksPortAudioException(pe);
            }

            pe = PortAudioAPI.Pa_OpenStream(
                out streamHandle,
                inputParams,
                outputParams,
                sr,
                framesPerBuffer,
                PortAudioAPI.PaStreamFlags.paNoFlag,
                this.paCallback,
                IntPtr.Zero);

            if (pe != PortAudioAPI.PaError.paNoError)
            {
                throw new SDRBlocksPortAudioException(pe);
            }
        }

        protected abstract void StreamCallback(IntPtr input, IntPtr output, uint frameCount);

        #region Implementation details

        private bool disposed = false;
        private IntPtr streamHandle;
        private readonly PortAudioAPI.PaStreamCallbackDelegate paCallback;

        private unsafe PortAudioAPI.PaStreamCallbackResult PaStreamCallback(
             IntPtr input,
             IntPtr output,
             uint frameCount,
             ref PortAudioAPI.PaStreamCallbackTimeInfo timeInfo,
             PortAudioAPI.PaStreamCallbackFlags statusFlags,
             IntPtr userData)
        {
            this.StreamCallback(input, output, frameCount);
            return PortAudioAPI.PaStreamCallbackResult.paContinue;
        }

        #endregion
    }
}
