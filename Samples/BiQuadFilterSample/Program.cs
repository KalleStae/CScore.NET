using CSCore;
using CSCore.Codecs;
using CSCore.DSP;
using CSCore.SoundOut;

using System;
using System.Windows.Forms;

namespace BiQuadFilterSample
{
    class Program
    {
  //Project Sdk = "Microsoft.NET.Sdk" >
  //< PropertyGroup >
  //  < TargetFramework > net5.0-windows</TargetFramework>
  //  <OutputType>Exe</OutputType>
  //  <GenerateAssemblyInfo>false</GenerateAssemblyInfo>
  //  <UseWindowsForms>true</UseWindowsForms>
  //  !!!!!!!!!!!!!!!!!!!!       <DisableWinExeOutputInference>true</DisableWinExeOutputInference> !!!!!!!!!!!!!!!!!!!!! for use console screen in .net5
  //  <ImportWindowsDesktopTargets>true</ImportWindowsDesktopTargets>
  //</PropertyGroup>



        [STAThread]
        static void Main()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog { Filter = CodecFactory.SupportedFilesFilterEn };

            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            using (BiQuadFilterSource source = CodecFactory.Instance.GetCodec(openFileDialog.FileName)
                .ToSampleSource()
                .AppendSource(x => new BiQuadFilterSource(x)))
            {
                using (WasapiOut soundOut = new WasapiOut())
                {
                    soundOut.Initialize(source.ToWaveSource());
                    soundOut.Play();

                    Console.WriteLine("Playing without any filter.");
                    Console.ReadKey();

                    source.Filter = new HighpassFilter(source.WaveFormat.SampleRate, 4000);
                    Console.WriteLine("HighpassFilter @4kHz");
                    Console.ReadKey();

                    source.Filter = new LowpassFilter(source.WaveFormat.SampleRate, 1000);
                    Console.WriteLine("LowpassFilter @1kHz");
                    Console.ReadKey();

                    source.Filter = new PeakFilter(source.WaveFormat.SampleRate, 2000, 15, 10);
                    Console.WriteLine("PeakFilter @2kHz; bandWidth = 15; gain = 10dB");
                    Console.ReadKey();
                }
            }
        }
    }

    public class BiQuadFilterSource : SampleAggregatorBase
    {
        private readonly object _lockObject = new object();
        private BiQuad _biquad;

        public BiQuad Filter
        {
            get { return _biquad; }
            set
            {
                lock (_lockObject)
                {
                    _biquad = value;
                }
            }
        }

        public BiQuadFilterSource(ISampleSource source) : base(source)
        {
        }

        public override int Read(float[] buffer, int offset, int count)
        {
            int read = base.Read(buffer, offset, count);
            lock (_lockObject)
            {
                if (Filter != null)
                {
                    for (int i = 0; i < read; i++)
                    {
                        buffer[i + offset] = Filter.Process(buffer[i + offset]);
                    }
                }
            }

            return read;
        }
    }
}
