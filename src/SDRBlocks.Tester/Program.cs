using System;
using System.Threading;
using SDRBlocks.Core.DspBlocks;
using SDRBlocks.IO.WMME;
using SDRBlocks.Core;
using SDRBlocks.IO.FunCubeDongle;
using SDRBlocks.Core.IO;

namespace SDRBlocks.Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            FunCubeDongleController ctrl = new FunCubeDongleController();
            ctrl.CenterFrequency = 104400000;

            IDeviceEnumerator en = new DeviceEnumerator();
            var id = en.EnumerateInputDevices();

            DspProcessor proc = new DspProcessor();
            ProcessTriggerDelegate procDelegate = new ProcessTriggerDelegate(proc.StartProcessing);

            WMMEInputDevice input = new WMMEInputDevice(0, 2, 192000);
            proc.AddBlock(input);

            Signal s1 = new Signal(192000, 2, FrameFormat.Complex, 65536);
            input.Output.AttachedSignal = s1;

            input.ProcessTriggerEvent += procDelegate;

            Thread.Sleep(10000);

            Console.WriteLine("Done!");
        }
    }
}
