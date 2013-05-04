using System;
using System.Collections.Generic;
using System.Threading;

namespace SDRBlocks.Core.Processing
{
    internal static class ProcessingPool
    {
        static ProcessingPool()
        {
            threads = new Thread[Environment.ProcessorCount];
            for (int i = 0; i < threads.Length; ++i)
            {
                Thread thread = new Thread(ThreadProc);
                threads[i] = thread;
                thread.Priority = ThreadPriority.AboveNormal;
                thread.Start();
            }
        }

        public static void EnqueueTask(IProcessingItem item)
        {
            lock (queue)
            {
                queue.Enqueue(item);
                if (waitingThreads > 0)
                {
                    Monitor.Pulse(queue);
                }
            }
        }

        public static void Dispose()
        {
            terminating = true;
            lock (queue)
            {
                Monitor.PulseAll(queue);
            }
            foreach (Thread thread in threads)
            {
                thread.Join();
            }
        }

        private static readonly Thread[] threads;
        private static int waitingThreads = 0;
        private static Queue<IProcessingItem> queue = new Queue<IProcessingItem>();
        private static bool terminating = false;

        private static void ThreadProc()
        {
            for (; ; )
            {
                if (terminating)
                    return;

                IProcessingItem item;

                lock (queue)
                {
                    while (queue.Count == 0)
                    {
                        ++waitingThreads;
                        try
                        {
                            Monitor.Wait(queue);
                        }
                        finally
                        {
                            --waitingThreads;
                        }
                        if (terminating)
                            return;
                    }
                    item = queue.Dequeue();
                }

                item.Invoke();
            }
        }
    }
}
