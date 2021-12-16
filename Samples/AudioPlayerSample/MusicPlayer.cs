using CSCore;
using CSCore.Codecs;
using CSCore.Codecs.AAC;
using CSCore.Codecs.MP3;
using CSCore.Codecs.WAV;
using CSCore.Codecs.WMA;
using CSCore.CoreAudioAPI;
using CSCore.SoundOut;

using System;
using System.ComponentModel;
using System.IO;

namespace AudioPlayerSample
{
    public class MusicPlayer : Component
    {
        private ISoundOut _soundOut;
        private IWaveSource _waveSource;

        public event EventHandler<PlaybackStoppedEventArgs> PlaybackStopped;

        public PlaybackState PlaybackState
        {
            get
            {
                if (_soundOut != null)
                {
                    return _soundOut.PlaybackState;
                }

                return PlaybackState.Stopped;
            }
        }

        public TimeSpan Position
        {
            get
            {
                if (_waveSource != null)
                {
                    return _waveSource.GetPosition();
                }

                return TimeSpan.Zero;
            }
            set
            {
                if (_waveSource != null)
                {
                    _waveSource.SetPosition(value);
                }
            }
        }

        public TimeSpan Length
        {
            get
            {
                if (_waveSource != null)
                {
                    return _waveSource.GetLength();
                }

                return TimeSpan.Zero;
            }
        }

        public int Volume
        {
            get
            {
                if (_soundOut != null)
                {
                    return Math.Min(100, Math.Max((int)(_soundOut.Volume * 100), 0));
                }

                return 100;
            }
            set
            {
                if (_soundOut != null)
                {
                    _soundOut.Volume = Math.Min(1.0f, Math.Max(value / 100f, 0f));
                }
            }
        }

        public void Open(string filename, MMDevice device)
        {
            CleanupPlayback();

      string extension = new FileInfo(filename).Extension.ToLowerInvariant();
      switch (extension)
      {
        case ".wav":
          _waveSource = new WaveFileReader(filename);
          break;

        case ".mp3":
          _waveSource = new DmoMp3Decoder(filename);
          break;

        case ".ogg":
          // _waveSource = new OggSource(filename).ToWaveSource();
          break;

        case ".flac":
          // _waveSource = new FlacFile(filename);
          break;

        case ".wma":
          _waveSource = new WmaDecoder(filename);
          break;

        case ".aac":
        case ".m4a":
        case ".mp4":
          _waveSource = new AacDecoder(filename);
          break;

        case ".opus":
          //_waveSource = new OpusSource(resourceItem.Stream, resourceItem.MediaDetails.SampleRate, resourceItem.MediaDetails.Channels);
          break;

        default:
          throw new NotSupportedException($"Extension '{extension}' is not supported");
      }
      //_waveSource =
      //          CodecFactory.Instance.GetCodec(filename)
      //              .ToSampleSource()
      //              .ToMono()
      //              .ToWaveSource();
            _soundOut = new WasapiOut() { Latency = 100, Device = device };

      WaveFormat waveFormat1 = new WaveFormat(41000,16,2);

     
    





      _soundOut.Initialize(_waveSource);




            if (PlaybackStopped != null)
            {
                _soundOut.Stopped += PlaybackStopped;
            }
        }

        public void Play()
        {
            if (_soundOut != null)
            {
                _soundOut.Play();
            }
        }

        public void Pause()
        {
            if (_soundOut != null)
            {
                _soundOut.Pause();
            }
        }

        public void Stop()
        {
            if (_soundOut != null)
            {
                _soundOut.Stop();
            }
        }

        private void CleanupPlayback()
        {
            if (_soundOut != null)
            {
                _soundOut.Dispose();
                _soundOut = null;
            }
            if (_waveSource != null)
            {
                _waveSource.Dispose();
                _waveSource = null;
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            CleanupPlayback();
        }
    }
}