using System;
using System.Collections.Generic;

namespace SDRBlocks.Core
{
    public sealed class DspProcessor
    {
        public void AddBlock(IDspBlock block)
        {
            if (block.IsIndependent)
            {
                if (inpdepBlocks.Contains(block))
                {
                    throw new SDRBlocksException("A block cannot be added twice.");
                }

                this.inpdepBlocks.Add(block);
            }
        }

        public void RemoveBlock(IDspBlock block)
        {
            this.inpdepBlocks.Remove(block);
        }

        /// <summary>
        /// Iterate over the signal graph and ask each block to perform the processing.
        /// </summary>
        public void StartProcessing()
        {
        }

        private readonly List<IDspBlock> inpdepBlocks = new List<IDspBlock>();
    }
}
