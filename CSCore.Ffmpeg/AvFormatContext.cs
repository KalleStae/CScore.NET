using CSCore.Ffmpeg.Interops;

using System;
using System.Collections.Generic;

namespace CSCore.Ffmpeg
{
    internal class AvFormatContext : IDisposable
    {
        private unsafe AVFormatContext* _formatContext;
        private AvStream _stream;

        public unsafe IntPtr FormatPtr
        {
            get { return (IntPtr)_formatContext; }
        }

        public int BestAudioStreamIndex { get; private set; }

        public AvStream SelectedStream
        {
            get { return _stream; }
        }

        public unsafe bool CanSeek
        {
            get
            {
                if (_formatContext == null || _formatContext->pb == null)
                {
                    return false;
                }

                return _formatContext->pb->seekable == 1;
            }
        }

        public double LengthInSeconds
        {
            get
            {
                if (SelectedStream == null || SelectedStream.Stream.duration < 0)
                {
                    return 0;
                }

                AVRational timebase = SelectedStream.Stream.time_base;
                if (timebase.den == 0)
                {
                    return 0;
                }

                return SelectedStream.Stream.duration * timebase.num / (double)timebase.den;
            }
        }

        public unsafe AVFormatContext FormatContext
        {
            get
            {
                if (_formatContext == null)
                {
                    return default(AVFormatContext);
                }

                return *_formatContext;
            }
        }

        public Dictionary<string, string> Metadata { get; private set; }

        public unsafe AvFormatContext(FfmpegStream stream)
        {
            _formatContext = FfmpegCalls.AvformatAllocContext();
            fixed (AVFormatContext** pformatContext = &_formatContext)
            {
                FfmpegCalls.AvformatOpenInput(pformatContext, stream.AvioContext);
            }
            Initialize();
        }

        public unsafe AvFormatContext(string url)
        {
            _formatContext = FfmpegCalls.AvformatAllocContext();
            fixed (AVFormatContext** pformatContext = &_formatContext)
            {
                FfmpegCalls.AvformatOpenInput(pformatContext, url);
            }
            Initialize();
        }

        private unsafe void Initialize()
        {
            FfmpegCalls.AvFormatFindStreamInfo(_formatContext);
            BestAudioStreamIndex = FfmpegCalls.AvFindBestStreamInfo(_formatContext);
            _stream = new AvStream((IntPtr)_formatContext->streams[BestAudioStreamIndex]);

            Metadata = new Dictionary<string, string>();
            if (_formatContext->metadata != null)
            {
                AVDictionaryEntry[] metadata = _formatContext->metadata->Elements;
                foreach (AVDictionaryEntry element in metadata)
                {
                    Metadata.Add(element.Key, element.Value);
                }
            }
        }

        public void SeekFile(double seconds)
        {
            AVRational streamTimeBase = SelectedStream.Stream.time_base;
            double time = seconds * streamTimeBase.den / streamTimeBase.num;

            FfmpegCalls.AvFormatSeekFile(this, time);
        }

        public unsafe void Dispose()
        {
            GC.SuppressFinalize(this);

            if (SelectedStream != null)
            {
                SelectedStream.Dispose();
                _stream = null;
            }

            if (_formatContext != null)
            {
                fixed (AVFormatContext** pformatContext = &_formatContext)
                {
                    FfmpegCalls.AvformatCloseInput(pformatContext);
                }

                _formatContext = null;
                BestAudioStreamIndex = 0;
            }
        }

        ~AvFormatContext()
        {
            Dispose();
        }
    }
}