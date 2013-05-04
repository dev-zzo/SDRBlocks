using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SDRBlocks.IO.PortAudio;
using System.Threading;

namespace SDRBlocks.Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            Module m = new Module();
            var list = m.Enumerator.EnumerateInputDevices();
            var mod = new PortAudioInputDevice(1, 2, 44100);
            mod.Start();
            Thread.Sleep(5000);
            mod.Stop();
            mod.Dispose();
        }
    }
}
