using System;
using System.Runtime.InteropServices;
using SDRBlocks.IO.WMME.Interop;

namespace SDRBlocks.IO.WMME
{
    public abstract class WaveBuffer : IDisposable
    {
        public WaveBuffer(uint numFrames, uint frameSize)
        {
            this.Size = numFrames;
            int bufferSize = (int)(numFrames * frameSize);
            this.backBuffer = new byte[bufferSize];
            this.dataHandle = GCHandle.Alloc(this.backBuffer, GCHandleType.Pinned);
            this.Buffer = this.dataHandle.AddrOfPinnedObject();
            this.selfHandle = GCHandle.Alloc(this);

            this.header = new WaveHeader();
            this.header.bufferLength = bufferSize;
            this.header.dataBuffer = this.dataHandle.AddrOfPinnedObject();
            this.header.userData = (IntPtr)this.selfHandle;
            this.headerHandle = GCHandle.Alloc(this.header, GCHandleType.Pinned);
            this.headerPtr = this.headerHandle.AddrOfPinnedObject();
        }

        /// <summary>
        /// Size of the buffer, in frames.
        /// </summary>
        public uint Size { get; private set; }

        public IntPtr Buffer { get; private set; }

        public bool IsDone
        {
            get
            {
                return this.header.flags.HasFlag(WaveHeaderFlags.Done);
            }
        }

        /// <summary>
        /// Submit the buffer to the driver for processing.
        /// </summary>
        public abstract void Submit();

        public abstract void Prepare(IntPtr hWave);

        public abstract void Unprepare();

        #region IDisposable Members

        public void Dispose()
        {
            this.Unprepare();
            this.dataHandle.Free();
            this.selfHandle.Free();
            this.headerHandle.Free();
        }

        #endregion

        #region Implementation details

        protected IntPtr hWave;
        protected WaveHeader header;
        protected IntPtr headerPtr;

        private Array backBuffer;
        private GCHandle dataHandle;
        private GCHandle selfHandle;
        private GCHandle headerHandle;

        #endregion
    }

    internal class WaveBufferOut : WaveBuffer
    {
        public WaveBufferOut(uint numFrames, uint frameSize)
            : base(numFrames, frameSize)
        {
        }

        public override void Submit()
        {
            WMMEException.Check(Wave.waveOutWrite(this.hWave, this.headerPtr, Marshal.SizeOf(this.header)));
        }

        public override void Prepare(IntPtr hWave)
        {
            this.hWave = hWave;
            WMMEException.Check(Wave.waveOutPrepareHeader(this.hWave, this.headerPtr, Marshal.SizeOf(this.header)));
        }

        public override void Unprepare()
        {
            WMMEException.Check(Wave.waveOutUnprepareHeader(this.hWave, this.headerPtr, Marshal.SizeOf(this.header)));
            this.header.flags = 0;
        }
    }

    internal class WaveBufferIn : WaveBuffer
    {
        public WaveBufferIn(uint numFrames, uint frameSize)
            : base(numFrames, frameSize)
        {
        }

        public override void Submit()
        {
            WMMEException.Check(Wave.waveInAddBuffer(this.hWave, this.headerPtr, Marshal.SizeOf(this.header)));
        }

        public override void Prepare(IntPtr hWave)
        {
            this.hWave = hWave;
            WMMEException.Check(Wave.waveInPrepareHeader(this.hWave, this.headerPtr, Marshal.SizeOf(this.header)));
        }

        public override void Unprepare()
        {
            WMMEException.Check(Wave.waveInUnprepareHeader(this.hWave, this.headerPtr, Marshal.SizeOf(this.header)));
            this.header.flags = 0;
        }
    }
}
