
namespace SDRBlocks.IO.FunCubeDongle
{
    internal enum FunCubeDongleCommand
    {
        Query = 1,
        SetFrequencyHertz = 101,
        GetFrequencyHertz = 102,
        SetLNAGain = 110,
        SetRFFilter = 113,
        SetMixerGain = 114,
        SetIFGain = 117,
        SetIFFilter = 122,
        SetBiasTee = 126,
        GetLNAGain = 150,
        GetRFFilter = 153,
        GetMixerGain = 154,
        GetIFGain = 157,
        GetIFFilter = 162,
        GetBiasTee = 166,
    }
}
