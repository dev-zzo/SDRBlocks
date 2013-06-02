using System;
using System.Runtime.InteropServices;
using SDRBlocks.Core.Interop;

namespace SDRBlocks.Core
{
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

        #region Public properties

        /// <summary>
        /// Human-readable signal name.
        /// </summary>
        public string Name { get; set; }

        public SourcePin SourcePin { get; private set; }

        public SinkPin SinkPin { get; private set; }

        public bool IsConnected { get { return this.SourcePin != null && this.SinkPin != null; } }

        /// <summary>
        /// Frame rate of this signal, samples/s.
        /// </summary>
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

        #endregion

        /// <summary>
        /// Called by a sink pin's owner when data has been consumed.
        /// </summary>
        /// <param name="frameCount">How many frames have been consumed.</param>
        public void Consumed(int frameCount)
        {
            lock (this)
            {
                this.MoveData(frameCount);
                this.FrameCount -= frameCount;
            }
            // Console.WriteLine("Signal {0} consumed: {1}, now {2}", this.Name, frameCount, this.FrameCount);
        }

        /// <summary>
        /// Called by a source pin's owner.
        /// </summary>
        /// <param name="frameCount"></param>
        public void Refilled(int frameCount)
        {
            lock (this)
            {
                this.FrameCount += frameCount;
            }
            // Console.WriteLine("Signal {0} refilled: {1}, now {2}", this.Name, frameCount, this.FrameCount);
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

        #region Implementation details

        internal void NotifyOnAttach(Pin pin)
        {
            SourcePin srcPin = pin as SourcePin;
            if (srcPin != null)
            {
                this.SourcePin = srcPin;
                return;
            }
            SinkPin sinkPin = pin as SinkPin;
            if (sinkPin != null)
            {
                this.SinkPin = sinkPin;
                return;
            }
            // Didn't expect that.
        }

        internal void NotifyOnDetach(Pin pin)
        {
            if (pin == this.SourcePin)
            {
                this.SourcePin = null;
            }
            else if (pin == this.SinkPin)
            {
                this.SinkPin = null;
            }
            else
            {
                // Didn't expect that.
            }
        }

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

        #endregion
    }
}
