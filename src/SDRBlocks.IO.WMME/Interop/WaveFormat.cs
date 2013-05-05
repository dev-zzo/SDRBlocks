using System;
using System.Runtime.InteropServices;

namespace SDRBlocks.IO.WMME.Interop
{
    /// <summary>
    /// Represents a Wave file format
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 2)]
    public struct WaveFormat
    {
        /// <summary>format type</summary>
        private WaveFormatEncoding waveFormatTag;
        /// <summary>number of channels</summary>
        private short channels;
        /// <summary>sample rate</summary>
        private int sampleRate;
        /// <summary>for buffer estimation</summary>
        private int averageBytesPerSecond;
        /// <summary>block size of data</summary>
        private short blockAlign;
        /// <summary>number of bits per sample of mono data</summary>
        private short bitsPerSample;
        /// <summary>number of following bytes</summary>
        private short extraSize;

        /// <summary>
        /// Creates a new 16 bit wave format with the specified sample
        /// rate and channel count
        /// </summary>
        /// <param name="sampleRate">Sample Rate</param>
        /// <param name="channels">Number of channels</param>
        public WaveFormat(int sampleRate, int channels)
            : this(sampleRate, 16, channels)
        {
        }

        /// <summary>
        /// Creates a WaveFormat with custom members
        /// </summary>
        /// <param name="tag">The encoding</param>
        /// <param name="sampleRate">Sample Rate</param>
        /// <param name="channels">Number of channels</param>
        /// <param name="averageBytesPerSecond">Average Bytes Per Second</param>
        /// <param name="blockAlign">Block Align</param>
        /// <param name="bitsPerSample">Bits Per Sample</param>
        /// <returns></returns>
        public WaveFormat(
            WaveFormatEncoding tag, 
            int sampleRate, 
            int channels, 
            int averageBytesPerSecond, 
            int blockAlign, 
            int bitsPerSample)
        {
            this.waveFormatTag = tag;
            this.channels = (short)channels;
            this.sampleRate = sampleRate;
            this.averageBytesPerSecond = averageBytesPerSecond;
            this.blockAlign = (short)blockAlign;
            this.bitsPerSample = (short)bitsPerSample;
            this.extraSize = 0;
        }

        /// <summary>
        /// Creates a new PCM format with the specified sample rate, bit depth and channels
        /// </summary>
        public WaveFormat(int rate, int bits, int channels)
        {
            if (channels < 1)
            {
                throw new ArgumentOutOfRangeException("Channels must be 1 or greater", "channels");
            }
            // minimum 16 bytes, sometimes 18 for PCM
            this.waveFormatTag = WaveFormatEncoding.Pcm;
            this.channels = (short)channels;
            this.sampleRate = rate;
            this.bitsPerSample = (short)bits;
            this.extraSize = 0;

            this.blockAlign = (short)(channels * (bits / 8));
            this.averageBytesPerSecond = this.sampleRate * this.blockAlign;
        }

        /// <summary>
        /// Creates a new 32 bit IEEE floating point wave format
        /// </summary>
        /// <param name="sampleRate">sample rate</param>
        /// <param name="channels">number of channels</param>
        public static WaveFormat CreateIeeeFloatWaveFormat(int sampleRate, int channels)
        {
            WaveFormat wf = new WaveFormat();
            wf.waveFormatTag = WaveFormatEncoding.IeeeFloat;
            wf.channels = (short)channels;
            wf.bitsPerSample = 32;
            wf.sampleRate = sampleRate;
            wf.blockAlign = (short)(4 * channels);
            wf.averageBytesPerSecond = sampleRate * wf.blockAlign;
            wf.extraSize = 0;
            return wf;
        }

        /// <summary>
        /// Returns the encoding type used
        /// </summary>
        public WaveFormatEncoding Encoding
        {
            get
            {
                return waveFormatTag;
            }
        }

        /// <summary>
        /// Returns the number of channels (1=mono,2=stereo etc)
        /// </summary>
        public int Channels
        {
            get
            {
                return channels;
            }
        }

        /// <summary>
        /// Returns the sample rate (samples per second)
        /// </summary>
        public int SampleRate
        {
            get
            {
                return sampleRate;
            }
        }

        /// <summary>
        /// Returns the average number of bytes used per second
        /// </summary>
        public int AverageBytesPerSecond
        {
            get
            {
                return averageBytesPerSecond;
            }
        }

        /// <summary>
        /// Returns the block alignment
        /// </summary>
        public int BlockAlign
        {
            get
            {
                return blockAlign;
            }
        }

        /// <summary>
        /// Returns the number of bits per sample (usually 16 or 32, sometimes 24 or 8)
        /// Can be 0 for some codecs
        /// </summary>
        public int BitsPerSample
        {
            get
            {
                return bitsPerSample;
            }
        }

        /// <summary>
        /// Returns the number of extra bytes used by this waveformat. Often 0,
        /// except for compressed formats which store extra data after the WAVEFORMATEX header
        /// </summary>
        public int ExtraSize
        {
            get
            {
                return extraSize;
            }
        }
    }
}
