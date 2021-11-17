using CSCore;
using CSCore.Codecs;
using CSCore.XAudio2;

using System;
using System.IO;
using System.Windows.Forms;

namespace XAudio2Playback
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = CodecFactory.SupportedFilesFilterEn;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (IWaveSource source = CodecFactory.Instance.GetCodec(openFileDialog.FileName))
                {
                    using (XAudio2 xaudio2 = XAudio2.CreateXAudio2())
                    using (XAudio2MasteringVoice masteringVoice = xaudio2.CreateMasteringVoice())
                    //ALWAYS create at least one masteringVoice.
                    using (StreamingSourceVoice streamingSourceVoice = new StreamingSourceVoice(xaudio2, source))
                    {
                        StreamingSourceVoiceListener.Default.Add(streamingSourceVoice);
                        //add the streamingSourceVoice to the default sourcevoicelistener which processes the data requests.
                        streamingSourceVoice.Start();

                        Console.WriteLine("Press any key to exit.");
                        Console.ReadKey();

                        StreamingSourceVoiceListener.Default.Remove(streamingSourceVoice);
                        streamingSourceVoice.Stop();
                    }
                }
            }
        }

        private static void PlayWithoutStreaming(IWaveSource waveSource)
        {
            using (XAudio2 xaudio2 = XAudio2.CreateXAudio2())
            using (XAudio2MasteringVoice masteringVoice = xaudio2.CreateMasteringVoice()) //ALWAYS create at least one masteringVoice.
            using (XAudio2SourceVoice sourceVoice = xaudio2.CreateSourceVoice(waveSource.WaveFormat))
            {
                byte[] buffer = waveSource.ToByteArray();
                using (XAudio2Buffer sourceBuffer = new XAudio2Buffer(buffer.Length))
                {
                    using (Stream stream = sourceBuffer.GetStream())
                    {
                        stream.Write(buffer, 0, buffer.Length);
                    }

                    sourceVoice.SubmitSourceBuffer(sourceBuffer);
                }

                sourceVoice.Start();

                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();

                sourceVoice.Stop();
            }
        }
    }

    static class Extensions
    {
        public static byte[] ToByteArray(this IWaveSource source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            using (MemoryStream buffer = new MemoryStream())
            {
                int read;
                byte[] temporaryBuffer = new byte[source.WaveFormat.BytesPerSecond];
                while ((read = source.Read(temporaryBuffer, 0, temporaryBuffer.Length)) > 0)
                {
                    buffer.Write(temporaryBuffer, 0, read);
                }

                return buffer.ToArray();
            }
        }
    }
}
