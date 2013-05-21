using System;
using System.Threading;
using SDRBlocks.Core.DspBlocks;
using SDRBlocks.IO.WMME;
using SDRBlocks.Core;
using SDRBlocks.IO.FunCubeDongle;

namespace SDRBlocks.Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            FunCubeDongleController ctrl = new FunCubeDongleController();
            Console.WriteLine("Freq: {0}", ctrl.CenterFrequency);
            ctrl.CenterFrequency = 104400000;
            Console.WriteLine("Freq: {0}", ctrl.CenterFrequency);
        }
    }
}
