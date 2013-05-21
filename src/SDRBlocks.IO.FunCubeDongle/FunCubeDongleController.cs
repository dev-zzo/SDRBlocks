using System;
using System.IO;
using SDRBlocks.Core.IO;
using SDRBlocks.Misc.USBAPI;

namespace SDRBlocks.IO.FunCubeDongle
{
    public enum TunerRFFilter
    {
        BANDPASS_0M_4M = 0,
        BANDPASS_4M_8M = 1,
        BANDPASS_8M_16M = 2,
        BANDPASS_16M_32M = 3,
        BANDPASS_32M_75M = 4,
        BANDPASS_75M_125M = 5,
        BANDPASS_125M_250M = 6,
        BANDPASS_145M = 7,
        BANDPASS_410M_875M = 8,
        BANDPASS_435M = 9,
        BANDPASS_875M_2000M = 10,
    }

    public enum TunerIFFilter
    {
        BANDPASS_200KHZ = 0,
        BANDPASS_300KHZ = 1,
        BANDPASS_600KHZ = 2,
        BANDPASS_1536KHZ = 3,
        BANDPASS_5MHZ = 4,
        BANDPASS_6MHZ = 5,
        BANDPASS_7MHZ = 6,
        BANDPASS_8MHZ = 7,
    }
    
    public sealed class FunCubeDongleController : IDeviceController, IDisposable
    {
        public FunCubeDongleController()
        {
            UsbAPI.EnumerateHidDevices((hDevInfo, devInfoData, deviceInstanceId) =>
            {
                if (deviceInstanceId.Contains("VID_04D8&PID_FB31"))
                {
                    this.deviceStream = UsbAPI.OpenHidDevice(hDevInfo, ref devInfoData);
                }
            });
        }

        #region IDeviceController Members

        public long CenterFrequency
        {
            get { return this.GetFrequency(); }
            set { this.SetFrequency(value); }
        }

        public int CorrectionCoefficient
        {
            get { return this.corrCoefficient; }
            set 
            {
                long frequency = this.CenterFrequency;
                this.corrCoefficient = value;
                this.correction = 1.0 + value * 1e-6;
                this.CenterFrequency = frequency;
            }
        }

        #endregion

        public TunerIFFilter IFFilter
        {
            get { return this.GetIFFilter(); }
            set { this.SetIFFilter(value); }
        }

        public TunerRFFilter RFFilter
        {
            get { return this.GetRFFilter(); }
            set { this.SetRFFilter(value); }
        }

        public bool LNAEnabled
        {
            get { return this.GetLNAState(); }
            set { this.SetLNAState(value); }
        }

        public bool MixerGainEnabled
        {
            get { return this.GetMixerGainState(); }
            set { this.SetMixerGainState(value); }
        }

        public bool BiasTeeEnabled
        {
            get { return this.GetBiasTeeState(); }
            set { this.SetBiasTeeState(value); }
        }

        public int IFGain
        {
            get { return this.GetIFGain(); }
            set { this.SetIFGain(value); }
        }

        public void Dispose()
        {
            this.deviceStream.Dispose();
        }

        private FileStream deviceStream;
        private int corrCoefficient;
        private double correction;

        #region High-level commands

        private long GetFrequency()
        {
            uint frequency;
            this.WriteCommand(FunCubeDongleCommand.GetFrequencyHertz);
            this.ReadResponse(out frequency);
            return (long)(frequency / this.correction);
        }

        private void SetFrequency(long value)
        {
            value = (long)(value * this.correction);
            this.WriteCommand(FunCubeDongleCommand.SetFrequencyHertz, (uint)value);
            this.ReadResponse();
        }

        private TunerRFFilter GetRFFilter()
        {
            byte value;
            this.WriteCommand(FunCubeDongleCommand.GetRFFilter);
            this.ReadResponse(out value);
            return (TunerRFFilter)value;
        }

        private void SetRFFilter(TunerRFFilter value)
        {
            this.WriteCommand(FunCubeDongleCommand.SetRFFilter, (byte)value);
            this.ReadResponse();
        }

        private TunerIFFilter GetIFFilter()
        {
            byte value;
            this.WriteCommand(FunCubeDongleCommand.GetIFFilter);
            this.ReadResponse(out value);
            return (TunerIFFilter)value;
        }

        private void SetIFFilter(TunerIFFilter value)
        {
            this.WriteCommand(FunCubeDongleCommand.SetIFFilter, (byte)value);
            this.ReadResponse();
        }

        private bool GetLNAState()
        {
            bool value;
            this.WriteCommand(FunCubeDongleCommand.GetLNAGain);
            this.ReadResponse(out value);
            return value;
        }

        private void SetLNAState(bool value)
        {
            this.WriteCommand(FunCubeDongleCommand.SetLNAGain, value ? 1u : 0u);
            this.ReadResponse();
        }

        private bool GetMixerGainState()
        {
            bool value;
            this.WriteCommand(FunCubeDongleCommand.GetMixerGain);
            this.ReadResponse(out value);
            return value;
        }

        private void SetMixerGainState(bool value)
        {
            this.WriteCommand(FunCubeDongleCommand.SetMixerGain, value ? 1u : 0u);
            this.ReadResponse();
        }

        private bool GetBiasTeeState()
        {
            bool value;
            this.WriteCommand(FunCubeDongleCommand.GetBiasTee);
            this.ReadResponse(out value);
            return value;
        }

        private void SetBiasTeeState(bool value)
        {
            this.WriteCommand(FunCubeDongleCommand.SetBiasTee, value ? 1u : 0u);
            this.ReadResponse();
        }

        private int GetIFGain()
        {
            uint value;
            this.WriteCommand(FunCubeDongleCommand.GetIFGain);
            this.ReadResponse(out value);
            return (int)value;
        }

        private void SetIFGain(int value)
        {
            this.WriteCommand(FunCubeDongleCommand.SetIFGain, (uint)value);
            this.ReadResponse();
        }

        #endregion

        #region Raw command interface

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
            result = buffer[3] == 1;
        }

        private void ReadResponse(out byte result)
        {
            byte[] buffer = new byte[65];
            this.deviceStream.Read(buffer, 0, buffer.Length);
            result = buffer[3];
        }

        private void ReadResponse(out uint result)
        {
            byte[] buffer = new byte[65];
            this.deviceStream.Read(buffer, 0, buffer.Length);
            result = BitConverter.ToUInt32(buffer, 3);
        }

        #endregion
    }
}
