using Concentus.Enums;
using Concentus.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceUP
{
    class Codec
    {
        public byte[] OpusEncoding(short[] inputAudioSamples)
        {
            OpusEncoder encoder = OpusEncoder.Create(48000, 1, OpusApplication.OPUS_APPLICATION_AUDIO);
            encoder.Bitrate = 12000;

            // Encoding loop
            byte[] outputBuffer = new byte[1000];
            int frameSize = 960;
            int thisPacketSize = encoder.Encode(inputAudioSamples, 0, frameSize, outputBuffer, 0, outputBuffer.Length); // this throws OpusException on a failure, rather than returning a negative number

            return outputBuffer;
        }
        public short[] OpusDecoding(byte[] compressedPacket)
        {
            OpusDecoder decoder = OpusDecoder.Create(48000, 1);

            // Decoding loop
            int frameSize = 960; // must be same as framesize used in input, you can use OpusPacketInfo.GetNumSamples() to determine this dynamically
            short[] outputBuffer = new short[frameSize];

            int thisFrameSize = decoder.Decode(compressedPacket, 0, compressedPacket.Length, outputBuffer, 0, frameSize, false);
            return outputBuffer;
        }
    }
}
