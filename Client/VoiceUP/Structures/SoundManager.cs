using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceUP.Structures
{
    public class SoundManager
    {
        NAudio.Wave.WaveIn sourceStream = null;
        NAudio.Wave.DirectSoundOut waveOut = null;
        int microphoneIndex = -1;


        public List<NAudio.Wave.WaveInCapabilities> ListOfMicrophones()
        {
            List<NAudio.Wave.WaveInCapabilities> sources = new List<NAudio.Wave.WaveInCapabilities>();

            for (int i = 0; i < NAudio.Wave.WaveIn.DeviceCount; i++)
            {
                sources.Add(NAudio.Wave.WaveIn.GetCapabilities(i));
            }
            
            return sources;
        }
    
        public void setMicrophoneIndex(int selectedDeviceNumber)
        {
            if (selectedDeviceNumber == -1) return;
            microphoneIndex = selectedDeviceNumber;

        }

        public bool StartREcording()
        {
            if (microphoneIndex < 0) return false;


            sourceStream = new NAudio.Wave.WaveIn();
            sourceStream.DeviceNumber = microphoneIndex;
            sourceStream.WaveFormat = new NAudio.Wave.WaveFormat(22500, NAudio.Wave.WaveIn.GetCapabilities(microphoneIndex).Channels);
            NAudio.Wave.WaveInProvider waveIn = new NAudio.Wave.WaveInProvider(sourceStream);

            waveOut = new NAudio.Wave.DirectSoundOut();
            waveOut.Init(waveIn);

            sourceStream.StartRecording();
            waveOut.Play();

            return true;
        }

        public bool StopRecording()
        {
            if (waveOut != null)
            {
                waveOut.Stop();
                waveOut.Dispose();
                waveOut = null;
            }
            if (sourceStream != null)
            {
                sourceStream.StopRecording();
                sourceStream.Dispose();
                sourceStream = null;
            }

            return true;
        }
    }
}
