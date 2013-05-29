using System;
using System.Collections.Generic;
using SDRBlocks.Core.Processing;
using System.Reflection;

namespace SDRBlocks.Core
{
    /// <summary>
    /// This encapsulates a single DSP block processing task.
    /// </summary>
    public sealed class DspBlockProcessingItem : IProcessingItem
    {
        public DspBlockProcessingItem(IDspBlock block)
        {
            this.block = block;
        }

        public void Invoke()
        {
            this.block.Process();

            IDspBlock[] deps = GetOutputDependencies(this.block);
            foreach (IDspBlock dep in deps)
            {
                if (dep.IsReadyToProcess)
                {
                    ProcessingPool.EnqueueTask(new DspBlockProcessingItem(dep));
                }
            }
        }

        private IDspBlock block;

        // TODO: This should really be performed only ONCE.
        private static IDspBlock[] GetOutputDependencies(IDspBlock block)
        {
            List<IDspBlock> tempDeps = new List<IDspBlock>();

            // Enumerate all outputs
            foreach (PropertyInfo p in block.GetType().GetProperties())
            {
                if (p.PropertyType != typeof(SourcePin))
                    continue;
                SourcePin src = p.GetValue(block, null) as SourcePin;
                if (!src.IsConnected)
                    continue;
                tempDeps.Add(src.AttachedSignal.SinkPin.Owner);
            }

            return tempDeps.ToArray();
        }
    }

    public sealed class DspProcessor
    {
        public void AddBlock(IDspBlock block)
        {
            if (block.IsIndependent)
            {
                if (indepBlocks.Contains(block))
                {
                    throw new SDRBlocksException("A block cannot be added twice.");
                }

                this.indepBlocks.Add(block);
            }
        }

        public void RemoveBlock(IDspBlock block)
        {
            this.indepBlocks.Remove(block);
        }

        /// <summary>
        /// Iterate over the signal graph and ask each block to perform the processing.
        /// </summary>
        public void StartProcessing()
        {
            foreach (IDspBlock block in this.indepBlocks)
            {
                ProcessingPool.EnqueueTask(new DspBlockProcessingItem(block));
            }
        }

        private readonly List<IDspBlock> indepBlocks = new List<IDspBlock>();
    }
}
