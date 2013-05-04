using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SDRBlocks.Core.IO;
using SDRBlocks.Core;

namespace SDRBlocks.IO.PortAudio
{
    public class Module : IModuleIO
    {
        #region IModuleIO Members

        private readonly DeviceEnumerator _enumerator = new DeviceEnumerator();
        public IDeviceEnumerator Enumerator
        {
            get { return this._enumerator; }
        }

        public IStreamOutput CreateInputDevice(string id, uint channels, uint sampleRate)
        {
            return null;
        }

        #endregion
    }
}
