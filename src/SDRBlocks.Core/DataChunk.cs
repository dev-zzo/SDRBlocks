using System;
using System.Runtime.InteropServices;

namespace SDRBlocks.Core
{
    public unsafe sealed class DataChunk : IDisposable, IRefCounted
    {
        /// <summary>
        /// Allocate an empty data chunk.
        /// </summary>
        /// <param name="itemCount"></param>
        /// <param name="itemSize"></param>
        public DataChunk(uint itemCount, uint itemSize)
        {
            this.ItemSize = itemSize;
            this.ItemCount = itemCount;

            this.length = itemSize * itemCount;
            uint totalSize = this.length + 16;
            Array buffer = new byte[totalSize];
            this.gcHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            this.pointer = (void*)(((long)this.gcHandle.AddrOfPinnedObject() + 0x0000000F) & ~0x0000000F);
        }

        /// <summary>
        /// Creates the chunk and copies the data
        /// </summary>
        /// <param name="itemCount"></param>
        /// <param name="itemSize"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static DataChunk CreateFrom(uint itemCount, uint itemSize, IntPtr data)
        {
            DataChunk chunk = new DataChunk(itemCount, itemSize);
            memcpy((IntPtr)chunk.Address, data, (UIntPtr)chunk.LengthBytes);
            return chunk;
        }

        /// <summary>
        /// Address of the chunk in memory.
        /// </summary>
        public void* Address { get { return this.pointer; } }

        /// <summary>
        /// Length of the chunk in bytes.
        /// </summary>
        public uint LengthBytes { get { return this.length; } }

        /// <summary>
        /// Item size this chunk was created with.
        /// </summary>
        public uint ItemSize { get; private set; }

        /// <summary>
        /// Item count this chnuk was created with.
        /// </summary>
        public uint ItemCount { get; private set; }

        #region IRefCounted Members

        public void Refer(int count = 1)
        {
            this.refCount += count;
        }

        public void Release()
        {
            --this.refCount;
            if (this.refCount == 0)
            {
                this.Dispose();
            }
        }

        private int refCount = 0;

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (this.gcHandle.IsAllocated)
            {
                this.gcHandle.Free();
            }
            this.pointer = null;
        }

        #endregion

        private GCHandle gcHandle;
        private void* pointer;
        private uint length;

        [DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        private static extern IntPtr memcpy(IntPtr dest, IntPtr src, UIntPtr count);
    }
}
