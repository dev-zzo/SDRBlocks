using System;
using System.Threading;
using SDRBlocks.Core.DspBlocks;
using SDRBlocks.IO.WMME;

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

            var osc = new Oscillator(44100, 1f, 666);
            var output = new WMMEOutputDevice(0, 2, 44100);
            osc.IQ.AttachedInput = output.Input;
            Thread.Sleep(3000);
            output.Dispose();
            osc.Dispose();
        }
    }
}
