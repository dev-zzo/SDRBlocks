using System;
using System.IO;
using SDRBlocks.Core.IO;

namespace SDRBlocks.IO.FunCubeDongle
{
    public sealed class FunCubeDongleController : IDeviceController, IDisposable
    {
        public FunCubeDongleController(string devicePath)
        {
            this.deviceStream = new FileStream(devicePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
        }

        #region IDeviceController Members

        public long CenterFrequency
        {
            get { return this.GetFrequency(); }
            set { this.SetFrequency(value); }
        }

        #endregion

        public void Dispose()
        {
            this.deviceStream.Dispose();
        }

        private FileStream deviceStream;

        private long GetFrequency()
        {
            uint frequency;
            this.WriteCommand(FunCubeDongleCommand.GetFrequencyHertz);
            this.ReadResponse(out frequency);
            return frequency;
        }

        private void SetFrequency(long frequency)
        {
            this.WriteCommand(FunCubeDongleCommand.SetFrequencyHertz, (uint)frequency);
            this.ReadResponse();
        }

        private void WriteCommand(FunCubeDongleCommand cmd)
        {
            byte[] buffer = new byte[65];
            buffer[0] = 0;
            buffer[1] = (byte)cmd;
            this.deviceStream.Write(buffer, 0, buffer.Length);
        }

        private void WriteCommand(FunCubeDongleCommand cmd, uint data)
        {
            byte[] buffer = new byte[65];
            buffer[0] = 0;
            buffer[1] = (byte)cmd;
            buffer[2] = (byte)(data & 0x000000ff);
            buffer[3] = (byte)((data & 0x0000ff00) >> 8);
            buffer[4] = (byte)((data & 0x00ff0000) >> 16);
            buffer[5] = (byte)((data & 0xff000000) >> 24);
            this.deviceStream.Write(buffer, 0, buffer.Length);
        }

        private void ReadResponse()
        {
            byte[] buffer = new byte[65];
            this.deviceStream.Read(buffer, 0, buffer.Length);
        }

        private void ReadResponse(out bool result)
        {
            byte[] buffer = new byte[65];
            this.deviceStream.Read(buffer, 0, buffer.Length);
            result = buffer[1] == 1;
        }

        private void ReadResponse(out uint result)
        {
            byte[] buffer = new byte[65];
            this.deviceStream.Read(buffer, 0, buffer.Length);
            result = BitConverter.ToUInt32(buffer, 1);
        }
    }
}
