using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Synthesizer.Device.Output.Managed
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [StructLayout(LayoutKind.Sequential)]
    internal struct MIDIHDR
    {
        /// <summary>
        /// Pointer to MIDI data.
        /// </summary>
        public IntPtr data;

        /// <summary>
        /// Size of the buffer.
        /// </summary>
        public int bufferLength;

        /// <summary>
        /// Actual amount of data in the buffer. This value should be less than 
        /// or equal to the value given in the bufferLength member.
        /// </summary>
        public int bytesRecorded;

        /// <summary>
        /// Custom user data.
        /// </summary>
        public int user;

        /// <summary>
        /// Flags giving information about the buffer.
        /// </summary>
        public int flags;

        /// <summary>
        /// Reserved; do not use.
        /// </summary>
        public IntPtr next;

        /// <summary>
        /// Reserved; do not use.
        /// </summary>
        public int reserved;

        /// <summary>
        /// Offset into the buffer when a callback is performed.
        /// </summary>
        public int offset;

        /// <summary>
        /// Reserved; do not use.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public int[] reservedArray;
    }
}
