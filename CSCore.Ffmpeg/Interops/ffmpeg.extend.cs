#pragma warning disable CS0649
#pragma warning disable IDE1006

using System.Runtime.InteropServices;

namespace CSCore.Ffmpeg.Interops
{
    internal unsafe partial class ffmpeg
    {
        [DllImport(libavformat, EntryPoint = "avio_alloc_context", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern AVIOContext* avio_alloc_context(byte* @buffer, int @buffer_size, int @write_flag, void* @opaque,
            [MarshalAs(UnmanagedType.FunctionPtr)] FfmpegCalls.AvioReadData @read_packet,
            [MarshalAs(UnmanagedType.FunctionPtr)] FfmpegCalls.AvioWriteData @write_packet,
            [MarshalAs(UnmanagedType.FunctionPtr)] FfmpegCalls.AvioSeek @seek);
    }
}
