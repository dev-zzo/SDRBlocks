using System;

namespace SDRBlocks.Core.DspBlocks
{
    public class FmDetector : IDspBlock
    {
        public FmDetector()
        {
        }

        public float Gain { get; set; }

        #region IDspBlock implementation

        public bool IsIndependent { get { return false; } }

        public bool IsReadyToProcess
        {
            // TBD...
            get { return true; }
        }

        public void Process()
        {
        }

        #endregion
    }
}
