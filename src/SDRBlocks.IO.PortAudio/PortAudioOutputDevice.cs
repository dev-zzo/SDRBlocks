﻿using System;
using PortAudioSharp;
using SDRBlocks.Core;
using SDRBlocks.Core.Interop;

namespace SDRBlocks.IO.PortAudio
{
    public sealed class PortAudioOutputDevice : PortAudioDevice
    {
        public PortAudioOutputDevice(int deviceIndex, uint channels, uint frameRate)
            : base(deviceIndex, channels, frameRate)
        {
            this.Input = new StreamInputSimple();
            this.Inputs.Add(this.Input);
        }

        public IStreamInput Input { get; private set; }

        #region Implementation details

        protected override void InitializePaStream(int deviceIndex)
        {
            PortAudioAPI.PaStreamParameters outputParams = new PortAudioAPI.PaStreamParameters();
            outputParams.device = deviceIndex;
            outputParams.channelCount = (int)this.ChannelCount;
            outputParams.suggestedLatency = 0;
            outputParams.sampleFormat = PortAudioAPI.PaSampleFormat.paFloat32;
            this.InitializePaStream(null, outputParams, 1024);
        }

        protected override void StreamCallback(IntPtr input, IntPtr output, uint frameCount)
        {
            Console.WriteLine("Output::StreamCallback called.");
            uint frameSize = this.ChannelCount * sizeof(float);
            if (this.Input.AttachedOutput != null)
            {
                FrameBuffer buffer = this.Input.AttachedOutput.Buffer;

                uint framesAvailable = buffer.FrameCount;
                uint framesToCopy = Math.Min(frameCount, framesAvailable);
                Console.WriteLine("Output::StreamCallback: copying {0} frames.", framesToCopy);
                MemFuncs.memcpy(output, buffer.Ptr, (UIntPtr)(frameSize * framesToCopy));
                buffer.Consume(framesToCopy);

                // If we cannot fill the buffer completely, zero out the rest.
                // Better than nothing.
                if (framesToCopy < frameCount)
                {
                    uint framesToZero = frameCount - framesToCopy;
                    IntPtr ptr = buffer.Ptr + (int)(frameSize * framesToCopy);
                    MemFuncs.memset(ptr, 0, (UIntPtr)(frameSize * framesToZero));
                }
            }
            else
            {
                MemFuncs.memset(output, 0, (UIntPtr)(frameSize * frameCount));
            }
        }

        #endregion
    }
}
