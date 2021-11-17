using CSCore;
using CSCore.Codecs;
using CSCore.SoundOut;

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace AdjustPosition
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
                    Debug.Assert(source.CanSeek, "Source does not support seeking.");

                    using (WasapiOut soundOut = new WasapiOut())
                    {
                        soundOut.Initialize(source);
                        soundOut.Play();

                        Debug.WriteLine("Press any key to skip half the track.");

                        source.Position = source.Length / 2;

                        while (true)
                        {
                            IAudioSource s = source;
                            string str = String.Format(@"New position: {0:mm\:ss\.f}/{1:mm\:ss\.f}",
                                TimeConverterFactory.Instance.GetTimeConverterForSource(s)
                                    .ToTimeSpan(s.WaveFormat, s.Position),
                                TimeConverterFactory.Instance.GetTimeConverterForSource(s)
                                    .ToTimeSpan(s.WaveFormat, s.Length));
                            str += String.Concat(Enumerable.Repeat(" ",  str.Length));
                            Debug.WriteLine(str);
                         //   Console.SetCursorPosition(0, Console.CursorTop);

                            Thread.Sleep(100);
                        }
                    }
                }
            }
        }
    }
}
