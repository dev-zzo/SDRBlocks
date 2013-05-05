using System;
using System.Runtime.InteropServices;
using SDRBlocks.IO.WMME.Interop;

namespace SDRBlocks.IO.WMME
{
    public abstract class WaveBuffer : IDisposable
    {
        public WaveBuffer(IntPtr hWave, uint numFrames, uint frameSize)
        {
            this.hWave = hWave;

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

            this.PrepareBuffer();
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
        public abstract void SubmitBuffer();

        #region IDisposable Members

        public void Dispose()
        {
            this.UnprepareBuffer();
            this.dataHandle.Free();
            this.selfHandle.Free();
            this.headerHandle.Free();
        }

        #endregion

        #region Implementation details

        protected IntPtr hWave;
        protected WaveHeader header;
        protected IntPtr headerPtr;

        protected abstract void PrepareBuffer();

        protected abstract void UnprepareBuffer();

        private Array backBuffer;
        private GCHandle dataHandle;
        private GCHandle selfHandle;
        private GCHandle headerHandle;

        #endregion
    }

    internal class WaveBufferOut : WaveBuffer
    {
        public WaveBufferOut(IntPtr hWaveOut, uint numFrames, uint frameSize)
            : base(hWaveOut, numFrames, frameSize)
        {
        }

        public override void SubmitBuffer()
        {
            WMMEException.Check(Wave.waveOutWrite(this.hWave, this.headerPtr, Marshal.SizeOf(this.header)));
        }

        protected override void PrepareBuffer()
        {
            WMMEException.Check(Wave.waveOutPrepareHeader(this.hWave, this.headerPtr, Marshal.SizeOf(this.header)));
        }

        protected override void UnprepareBuffer()
        {
            WMMEException.Check(Wave.waveOutUnprepareHeader(this.hWave, this.headerPtr, Marshal.SizeOf(this.header)));
        }
    }

    internal class WaveBufferIn : WaveBuffer
    {
        public WaveBufferIn(IntPtr hWaveIn, uint numFrames, uint frameSize)
            : base(hWaveIn, numFrames, frameSize)
        {
        }

        public override void SubmitBuffer()
        {
            WMMEException.Check(Wave.waveInAddBuffer(this.hWave, this.headerPtr, Marshal.SizeOf(this.header)));
        }

        protected override void PrepareBuffer()
        {
            WMMEException.Check(Wave.waveInPrepareHeader(this.hWave, this.headerPtr, Marshal.SizeOf(this.header)));
        }

        protected override void UnprepareBuffer()
        {
            WMMEException.Check(Wave.waveInUnprepareHeader(this.hWave, this.headerPtr, Marshal.SizeOf(this.header)));
        }
    }
}
