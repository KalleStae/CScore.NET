using System.Runtime.InteropServices;

namespace CSCore.SoundOut
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct WaveOutCaps
    {
        public short wMid;
        public short wPid;
        public int vDriverVersion;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string szPname;

        public MmDeviceFormats dwFormats;
        public short wChannels;
        public short wReserved1;
        public MmDeviceSupported dwSupport;

        public WaveFormat[] GetSupportedFormats()
        {
            return MMInterop.Utils.SupportedFormatsFlagsToWaveFormats(dwFormats);
        }
    }
}