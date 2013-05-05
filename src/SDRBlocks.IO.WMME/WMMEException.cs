using System;
using SDRBlocks.Core;
using SDRBlocks.IO.WMME.Interop;

namespace SDRBlocks.IO.WMME
{
    [Serializable]
    public class WMMEException : SDRBlocksException
    {
        public WMMEException() { }
        internal WMMEException(MmResult rc) { this.ResultCode = rc; }
        public WMMEException(string message) : base(message) { }
        public WMMEException(string message, Exception inner) : base(message, inner) { }
        protected WMMEException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
        internal MmResult ResultCode { get; private set; }

        internal static void Check(MmResult rv)
        {
            if (rv != MmResult.NoError)
            {
                throw new WMMEException(rv);
            }
        }
    }
}
