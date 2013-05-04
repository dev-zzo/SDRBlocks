using System;
using PortAudioSharp;
using SDRBlocks.Core;

namespace SDRBlocks.IO.PortAudio
{
    [Serializable]
    public class SDRBlocksPortAudioException : SDRBlocksException
    {
        public SDRBlocksPortAudioException() { }
        public SDRBlocksPortAudioException(PortAudioAPI.PaError error) 
            : base(PortAudioAPI.Pa_GetErrorText(error)) { }
        public SDRBlocksPortAudioException(string message) : base(message) { }
        public SDRBlocksPortAudioException(string message, Exception inner) : base(message, inner) { }
        protected SDRBlocksPortAudioException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
