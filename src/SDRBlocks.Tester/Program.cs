using System;
using System.Threading;
using SDRBlocks.Core.DspBlocks;
using SDRBlocks.IO.WMME;
using SDRBlocks.Core;

namespace SDRBlocks.Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            string inputId = String.Empty;
            string outputId = String.Empty;

            var en = new DeviceEnumerator();

            var inputList = en.EnumerateInputDevices();
            Console.WriteLine("Input devices available:");
            foreach (var item in inputList)
            {
                Console.WriteLine("{0}: {1}", item.Id, item.DeviceName);
                if (item.DeviceName.StartsWith("Realtek"))
                {
                    inputId = item.Id;
                }
            }

            var outputList = en.EnumerateOutputDevices();
            Console.WriteLine("Output devices available:");
            foreach (var item in outputList)
            {
                Console.WriteLine("{0}: {1}", item.Id, item.DeviceName);
                if (item.DeviceName.StartsWith("Realtek"))
                {
                    outputId = item.Id;
                }
            }

            var input = new WMMEInputDevice(0, 2, 44100);
            var osc = new Oscillator(1f, 440.0f);
            var output = new WMMEOutputDevice(0, 2, 44100);
            Signal signal = new Signal(44100, 2, FrameFormat.Float32, 44100);
            input.Output.AttachedSignal = signal;
            //osc.Output.AttachedSignal = signal;
            output.Input.AttachedSignal = signal;

            Thread.Sleep(15000);

            input.Dispose();
            output.Dispose();
        }
    }
}
