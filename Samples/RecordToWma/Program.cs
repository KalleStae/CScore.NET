using CSCore;
using CSCore.Codecs.WAV;
using CSCore.SoundIn;
using CSCore.Streams;

using System;

namespace RecordToWma
{
    class Program
    {
        static void Main(string[] args)
        {
            using (WasapiLoopbackCapture wasapiCapture = new WasapiLoopbackCapture())
            {
                wasapiCapture.Initialize();
                SoundInSource wasapiCaptureSource = new SoundInSource(wasapiCapture);
                using (IWaveSource stereoSource = wasapiCaptureSource.ToStereo())
                {
                    //using (var writer = MediaFoundationEncoder.CreateWMAEncoder(stereoSource.WaveFormat, "output.wma"))
                    using (WaveWriter writer = new WaveWriter("output.wav", stereoSource.WaveFormat))
                    {
                        byte[] buffer = new byte[stereoSource.WaveFormat.BytesPerSecond];
                        wasapiCaptureSource.DataAvailable += (s, e) =>
                        {
                            int read = stereoSource.Read(buffer, 0, buffer.Length);
                            writer.Write(buffer, 0, read);
                        };

                        wasapiCapture.Start();

                        Console.ReadKey();

                        wasapiCapture.Stop();
                    }
                }
            }
        }
    }
}
