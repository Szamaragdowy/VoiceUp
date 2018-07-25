using Concentus.Enums;
using Concentus.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace VoiceUP.UDP
{
    class OpusCodec : INetworkChatCodec
    {
        public string Name => "OPUS";

        public bool IsAvailable => true;

        public int BitsPerSecond => RecordFormat.SampleRate * 8;

        public WaveFormat RecordFormat => new WaveFormat(8000, 16, 1);

        public void Dispose()
        {
            // nothing to do
        }

        public byte[] Encode(byte[] data, int offset, int length)
        {
            OpusEncoder encoder = OpusEncoder.Create(48000, 1, OpusApplication.OPUS_APPLICATION_AUDIO);
            encoder.Bitrate = 12000;

            short[] samples = new short[data.Length/2];
            int i = 0;
            for (int n = 0; n < data.Length; n += 2)
            {
                samples[i] = BitConverter.ToInt16(data, n);
                i++;
            }

            byte[] outputBuffer = new byte[length / 2];
            int frameSize = 960;
            int thisPacketSize = encoder.Encode(samples, offset, frameSize, outputBuffer, 0, outputBuffer.Length);

            return outputBuffer;
        }

        public byte[] Decode(byte[] data, int offset, int length)
        {
            OpusDecoder decoder = OpusDecoder.Create(48000, 1);
            int frameSize = 960; 
            short[] outputBuffer = new short[frameSize];

            int thisFrameSize = decoder.Decode(data, 0, data.Length, outputBuffer, 0, frameSize, false);
            return data;
        }
    }
}
