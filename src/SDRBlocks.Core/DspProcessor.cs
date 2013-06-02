using System;
using System.Collections.Generic;
using SDRBlocks.Core.Processing;
using System.Reflection;
using System.Threading;

namespace SDRBlocks.Core
{
    public delegate void ItemProcessingCompleteDelegate(IDspBlock block);

    /// <summary>
    /// This encapsulates a single DSP block processing task.
    /// </summary>
    public sealed class DspBlockProcessingItem : IProcessingItem
    {
        public DspBlockProcessingItem(IDspBlock block)
        {
            this.block = block;
        }

        public event ItemProcessingCompleteDelegate ProcessingCompleteEvent;

        public void Invoke()
        {
            this.block.Process();
            if (this.ProcessingCompleteEvent != null)
            {
                this.ProcessingCompleteEvent(this.block);
            }
        }

        private IDspBlock block;
    }

    public sealed class DspProcessor : IDisposable
    {
        public DspProcessor()
        {
            this.processingThread = new Thread(this.ProcessingThread);
            this.processingThread.Name = "Processor Thread";
            this.processingThread.Start();
        }

        public void AddBlock(IDspBlock block)
        {
            if (this.allBlocks.Contains(block))
            {
                throw new SDRBlocksException("A block cannot be added twice.");
            }
            this.allBlocks.Add(block);
            if (block.IsIndependent)
            {
                this.indepBlocks.Add(block);
            }
        }

        public void RemoveBlock(IDspBlock block)
        {
            this.allBlocks.Remove(block);
            this.indepBlocks.Remove(block);
        }

        /// <summary>
        /// Iterate over the signal graph and ask each block to perform the processing.
        /// </summary>
        public void StartProcessing(object sender)
        {
            this.procStartEvent.Set();
        }

        public void Dispose()
        {
            this.isStopping = true;
            this.procStartEvent.Set();
            this.processingThread.Join();

            ProcessingPool.Dispose();
        }

        private readonly List<IDspBlock> allBlocks = new List<IDspBlock>();
        private readonly List<IDspBlock> indepBlocks = new List<IDspBlock>();
        private readonly Thread processingThread;
        private readonly AutoResetEvent procStartEvent = new AutoResetEvent(false);
        private readonly AutoResetEvent procFinishEvent = new AutoResetEvent(false);
        private int processingCounter;
        private object processingCounterLock = new object();
        private bool isStopping = false;

        private void EnqueueBlock(IDspBlock block)
        {
            DspBlockProcessingItem item = new DspBlockProcessingItem(block);
            item.ProcessingCompleteEvent += new ItemProcessingCompleteDelegate(this.OnBlockProcessingComplete);
            ProcessingPool.EnqueueTask(item);
        }

        private void ProcessingThread()
        {
            Console.WriteLine("Processing thread started.");
            while (!this.isStopping)
            {
                this.procStartEvent.WaitOne();

                if (this.isStopping)
                    break;

                this.processingCounter = this.allBlocks.Count;

                foreach (IDspBlock block in this.indepBlocks)
                {
                    this.EnqueueBlock(block);
                }

                this.procFinishEvent.WaitOne();
            }
            Console.WriteLine("Processing thread exits.");
        }

        private void OnBlockProcessingComplete(IDspBlock block)
        {
            lock (this.processingCounterLock)
            {
                --this.processingCounter;
                if (this.processingCounter == 0)
                {
                    this.procFinishEvent.Set();
                    return;
                }
            }

            IDspBlock[] deps = GetOutputDependencies(block);
            foreach (IDspBlock dep in deps)
            {
                this.EnqueueBlock(dep);
            }
        }

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
}
