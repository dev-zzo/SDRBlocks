using System;
using PortAudioSharp;
using SDRBlocks.Core;

namespace SDRBlocks.IO.PortAudio
{
    public sealed class PortAudioInputDevice : PortAudioDevice
    {
        public PortAudioInputDevice(int deviceIndex, uint channels, uint frameRate)
            : base(deviceIndex, channels, frameRate)
        {
            this.Output = new StreamOutputSimple(channels, FrameFormat.Float32, frameRate, 256 * 1024);
            this.Outputs.Add(this.Output);
        }

        public IStreamOutput Output { get; private set; }

        #region Implementation details

        protected override void InitializePaStream(int deviceIndex)
        {
            PortAudioAPI.PaStreamParameters inputParams = new PortAudioAPI.PaStreamParameters();
            inputParams.device = deviceIndex;
            inputParams.channelCount = (int)this.ChannelCount;
            inputParams.suggestedLatency = 0;
            inputParams.sampleFormat = PortAudioAPI.PaSampleFormat.paFloat32;
            this.InitializePaStream(inputParams, null, 65536);
        }

        protected override void StreamCallback(IntPtr input, IntPtr output, uint frameCount)
        {
            FrameBuffer buffer = this.Output.Buffer;
            uint count = Math.Min(frameCount, buffer.Size - buffer.FrameCount);
            buffer.Refill(input, count);
        }

        #endregion
    }
}
