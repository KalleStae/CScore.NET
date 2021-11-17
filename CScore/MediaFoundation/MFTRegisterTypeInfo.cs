using System;
using System.Runtime.InteropServices;

namespace CSCore.MediaFoundation
{
    /// <summary>
    /// Contains media type information for registering a Media Foundation transform (MFT).
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public class MFTRegisterTypeInfo
    {
        /// <summary>
        /// The major media type.
        /// </summary>
        public Guid GuidMajorType;
        /// <summary>
        /// The media subtype.
        /// </summary>
        public Guid GuidSubType;
    }
}