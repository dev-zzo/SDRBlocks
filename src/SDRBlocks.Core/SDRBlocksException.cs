using System;

namespace SDRBlocks.Core
{
    [Serializable]
    public class SDRBlocksException : Exception
    {
        public SDRBlocksException() { }
        public SDRBlocksException(string message) : base(message) { }
        public SDRBlocksException(string message, Exception inner) : base(message, inner) { }
        protected SDRBlocksException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
