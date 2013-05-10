using System;
using System.Runtime.InteropServices;
using SDRBlocks.Core.Interop;

namespace SDRBlocks.Core
{
    public delegate void SignalRefilledDelegate();

    public delegate void SignalConsumedDelegate();

    /// <summary>
    /// A signal is what connects two pins.
    /// Basically, this is a buffer to hold the data.
    /// </summary>
    public class Signal : IDisposable
    {
        public Signal(int frameRate, int channelCount, FrameFormat format, int size)
        {
            this.Name = "anonymous$" + (++signalCounter);

            this.FrameRate = frameRate;
            this.ChannelCount = channelCount;
            this.Format = format;
            this.FrameCount = 0;
            this.Size = size;

            // Allocate buffer
            this.backBuffer = new byte[this.FrameSize * this.Size];
            this.backBufferHandle = GCHandle.Alloc(this.backBuffer, GCHandleType.Pinned);
            this.data = this.backBufferHandle.AddrOfPinnedObject();
        }

        public string Name { get; set; }


        public ISourcePin SourcePin { get; private set; }

        public ISinkPin SinkPin { get; private set; }

        public bool IsConnected { get { return this.SourcePin != null && this.SinkPin != null; } }


        public int FrameRate { get; private set; }

        /// <summary>
        /// How many channels of a given type are interleaved in the buffer.
        /// </summary>
        public int ChannelCount { get; private set; }

        /// <summary>
        /// Suggested buffer data format.
        /// </summary>
        public FrameFormat Format { get; private set; }

        /// <summary>
        /// Raw buffer pointer.
        /// </summary>
        public IntPtr Data { get { return this.data; } }

        /// <summary>
        /// Size of each frame, bytes.
        /// </summary>
        public int FrameSize { get { return this.Format.Size() * this.ChannelCount; } }

        /// <summary>
        /// Count of valid frames in the buffer.
        /// </summary>
        public int FrameCount { get; private set; }

        /// <summary>
        /// Total buffer size, in frames.
        /// </summary>
        public int Size { get; private set; }


        public event SignalRefilledDelegate RefilledEvent;

        public event SignalConsumedDelegate ConsumedEvent;


        internal void NotifyOnAttach(IPin pin)
        {
            ISourcePin source = pin as ISourcePin;
            if (source != null)
            {
                if (this.SourcePin != null)
                {
                }
                this.SourcePin = source;
                return;
            }

            ISinkPin sink = pin as ISinkPin;
            if (sink != null)
            {
                if (this.SinkPin != null)
                {
                }
                this.SinkPin = sink;
                return;
            }
            throw new SDRBlocksException("Unknown IPin impementation.");
        }

        internal void NotifyOnDetach(IPin pin)
        {
            ISourcePin source = pin as ISourcePin;
            if (source != null)
            {
                this.SourcePin = null;
                return;
            }

            ISinkPin sink = pin as ISinkPin;
            if (sink != null)
            {
                this.SinkPin = null;
                return;
            }
            throw new SDRBlocksException("Unknown IPin impementation.");
        }

        /// <summary>
        /// Called by a sink pin's owner when data has been consumed.
        /// </summary>
        /// <param name="frameCount">How many frames have been consumed.</param>
        public void NotifyOnConsume(int frameCount)
        {
            this.MoveData(frameCount);
            this.FrameCount -= frameCount;
            if (this.ConsumedEvent != null)
            {
                this.ConsumedEvent();
            }
        }

        /// <summary>
        /// Called by a source pin's owner.
        /// </summary>
        /// <param name="frameCount"></param>
        public void NotifyOnRefill(int frameCount)
        {
            this.FrameCount += frameCount;
            if (this.RefilledEvent != null)
            {
                this.RefilledEvent();
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (this.backBufferHandle.IsAllocated)
            {
                this.backBufferHandle.Free();
                this.data = IntPtr.Zero;
            }
        }

        #endregion

        private static int signalCounter = 0;
        private readonly Array backBuffer;
        private GCHandle backBufferHandle;
        private IntPtr data;

        private void MoveData(int frameCount)
        {
            IntPtr src = this.data + frameCount * this.FrameSize;
            // NOTE: Potentially use this.FrameCount to decrease amount of data moved.
            MemFuncs.MemMove(this.data, src, (UIntPtr)((this.Size - frameCount) * this.FrameSize));
        }
    }
}
