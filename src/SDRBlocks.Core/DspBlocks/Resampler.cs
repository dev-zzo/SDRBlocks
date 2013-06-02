using System;
using SDRBlocks.Core.Maths;

namespace SDRBlocks.Core.DspBlocks
{
    public class Resampler : IDspBlock
    {
        public SinkPin Input { get; private set; }

        public SourcePin Output { get; private set; }

        public int M { get; set; }

        public int L { get; set; }

        public bool IsIndependent
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsReadyToProcess
        {
            get { throw new NotImplementedException(); }
        }

        public void Process()
        {
            if (this.L > 1)
            {
                // Upsample
            }

            // Filter

            // Downsample
            //Resample.Downsample(

            throw new NotImplementedException();
        }
    }
}
