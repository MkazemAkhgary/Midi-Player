using System;
using System.Runtime.InteropServices;
// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global

namespace Synthesizer.Device.Output.Managed
{
    /// <summary>
    /// Midi header
    /// </summary>
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
        public readonly int bufferLength;

        /// <summary>
        /// Actual amount of data in the buffer. This value should be less than 
        /// or equal to the value given in the bufferLength member.
        /// </summary>
        public readonly int bytesRecorded;

        /// <summary>
        /// Custom user data.
        /// </summary>
        public readonly int user;

        /// <summary>
        /// Flags giving information about the buffer.
        /// </summary>
        public readonly int flags;

        /// <summary>
        /// Reserved; do not use.
        /// </summary>
        public readonly IntPtr next;

        /// <summary>
        /// Reserved; do not use.
        /// </summary>
        public readonly int reserved;

        /// <summary>
        /// Offset into the buffer when a callback is performed.
        /// </summary>
        public readonly int offset;

        /// <summary>
        /// Reserved; do not use.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public readonly int[] reservedArray;
    }
}
