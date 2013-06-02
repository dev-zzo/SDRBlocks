using System;
using System.Threading;
using SDRBlocks.Core.DspBlocks;
using SDRBlocks.IO.WMME;
using SDRBlocks.Core;
using SDRBlocks.IO.FunCubeDongle;
using SDRBlocks.Core.IO;
using SDRBlocks.Core.Processing;

namespace SDRBlocks.Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            FunCubeDongleController ctrl = new FunCubeDongleController();
            ctrl.CenterFrequency = 104400000;
            ctrl.LNAEnabled = true;
            ctrl.MixerGainEnabled = true;
            ctrl.RFFilter = TunerRFFilter.BANDPASS_75M_125M;
            ctrl.IFGain = 0;
            ctrl.IFFilter = TunerIFFilter.BANDPASS_200KHZ;

            IDeviceEnumerator en = new DeviceEnumerator();
            var id = en.EnumerateInputDevices();
            var od = en.EnumerateOutputDevices();

            DspProcessor proc = new DspProcessor();
            ProcessTriggerDelegate procDelegate = new ProcessTriggerDelegate(proc.StartProcessing);

            WMMEInputDevice input = new WMMEInputDevice(0, 2, 192000);
            proc.AddBlock(input);
            WMMEOutputDevice output = new WMMEOutputDevice(0, 1, 192000);
            proc.AddBlock(output);
            FmDetector detector = new FmDetector();
            proc.AddBlock(detector);
            FirFilter filter = new FirFilter();
            filter.CutoffFrequency = 15000.0f;
            filter.Length = 301;
            proc.AddBlock(filter);

            Signal s1 = new Signal(192000, 1, FrameFormat.Complex, 65536);
            s1.Name = "In->FM";
            Signal s2 = new Signal(192000, 1, FrameFormat.Float32, 65536);
            s2.Name = "FM->FIR";
            Signal s3 = new Signal(192000, 1, FrameFormat.Float32, 65536);
            s3.Name = "FIR->Out";
            input.Output.AttachedSignal = s1;
            detector.InputIQ.AttachedSignal = s1;
            detector.Output.AttachedSignal = s2;
            filter.Input.AttachedSignal = s2;
            filter.Output.AttachedSignal = s3;
            output.Input.AttachedSignal = s3;

            input.ProcessTriggerEvent += procDelegate;

            //Thread.Sleep(60000);
            Console.ReadLine();

            Console.WriteLine("Done!");
            input.Dispose();
            output.Dispose();
            proc.Dispose();
        }
    }
}
