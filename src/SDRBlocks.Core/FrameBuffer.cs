using System;
using System.Runtime.InteropServices;
using SDRBlocks.Core.Interop;

namespace SDRBlocks.Core
{
    public delegate void FrameBufferRefilledDelegate();

    public delegate void FrameBufferConsumedDelegate();

    public unsafe sealed class FrameBuffer : IDisposable
    {
        public FrameBuffer(uint frameCount, uint frameSize)
        {
            this.Size = frameCount;
            this.frameSize = frameSize;
            long length = frameSize * frameCount;
            long totalSize = length + 16;
            this.buffer = new byte[totalSize];
            this.gcHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            this.pointer = (IntPtr)(((long)this.gcHandle.AddrOfPinnedObject() + 0x0000000F) & ~0x0000000F);
        }

        public void* Address { get { return this.pointer.ToPointer(); } }

        public IntPtr Ptr { get { return this.pointer; } }

        /// <summary>
        /// Total size of the buffer, in frames.
        /// </summary>
        public uint Size { get; private set; }

        /// <summary>
        /// Count of actual frames in the buffer.
        /// </summary>
        public uint FrameCount { get; set; }

        public event FrameBufferRefilledDelegate RefilledEvent;

        public event FrameBufferConsumedDelegate ConsumedEvent;

        /// <summary>
        /// Append the given count of frames at the end of the buffer.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="count"></param>
        public void Refill(IntPtr data, uint count)
        {
            if (count == 0)
                return;

            if (this.FrameCount + count > this.Size)
            {
                throw new SDRBlocksException(String.Format("Buffer overflow when trying to append {0} samples.", count));
            }

            IntPtr dest = (IntPtr)((long)this.pointer + this.FrameCount * this.frameSize);

            MemFuncs.memcpy(dest, data, (UIntPtr)(count * this.frameSize));

            this.FrameCount += count;

            if (this.RefilledEvent != null)
            {
                this.RefilledEvent();
            }
        }

        /// <summary>
        /// Remove the given count of frames at the beginning of the buffer.
        /// </summary>
        /// <param name="count"></param>
        public void Consume(uint count)
        {
            IntPtr src = (IntPtr)((long)this.pointer + count * this.frameSize);

            MemFuncs.memmove(this.pointer, src, (UIntPtr)(this.FrameCount - count));
            
            this.FrameCount -= count;

            if (this.ConsumedEvent != null)
            {
                this.ConsumedEvent();
            }
        }

        public void Dispose()
        {
            if (this.gcHandle.IsAllocated)
            {
                this.gcHandle.Free();
            }
            this.pointer = IntPtr.Zero;
        }

        private Array buffer;
        private uint frameSize;
        private GCHandle gcHandle;
        private IntPtr pointer;
    }
}
