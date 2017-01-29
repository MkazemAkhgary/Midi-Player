using System;
using System.Runtime.InteropServices;
using System.Text;
// ReSharper disable UnusedMember.Global

namespace Synthesizer.Extensions
{
    using Device.Output.Callback;
    using Device.Output.Managed;

    internal static class NativeMethods
    {
        private const string WinmmDll = "winmm.dll";

        internal static class MidiOutput
        {
            #region Function Imports

            #region //=================== Opening and Closing Devices ===================

            /// <summary>
            /// opens a MIDI output device for playback.
            /// </summary>
            /// <param name="midiOut">
            /// Pointer to an HMIDIOUT handle.
            /// This location is filled with a handle identifying the opened MIDI output device.
            /// The handle is used to identify the device in calls to other MIDI output functions.</param>
            /// <param name="deviceId">
            /// Identifier of the MIDI output device that is to be opened.
            /// </param>
            /// <param name="callback">
            /// Pointer to a callback function, an event handle, a thread identifier, 
            /// or a handle of a window or thread called during MIDI playback to process messages related to the progress of the playback. 
            /// If no callback is desired, specify NULL for this parameter.</param>
            /// <param name="instance">
            /// User instance data passed to the callback. This parameter is not used with window callbacks or threads. 
            /// </param>
            /// <param name="flags">
            /// Callback flag for opening the device.
            /// </param>
            /// <returns></returns>
            [DllImport(WinmmDll)]
            internal static extern MMRESULT midiOutOpen(
                [Out] out IntPtr midiOut, 
                [In] uint deviceId, 
                [In] MidiOutProc callback,
                [In] IntPtr instance, 
                [In] CALLBACK flags);

            /// <summary>
            /// closes the specified MIDI output device.
            /// </summary>
            /// <param name="midiOut">
            /// Handle to the MIDI output device.
            /// If the function is successful, the handle is no longer valid after the call to this function.
            /// </param>
            [DllImport(WinmmDll)]
            internal static extern MMRESULT midiOutClose([In] IntPtr midiOut);
            #endregion

            #region //======================= Device Capabilities ========================

            /// <summary>
            /// queries a specified MIDI output device to determine its capabilities.
            /// </summary>
            /// <param name="deviceId">
            /// Identifier of the MIDI output device.
            /// The device identifier specified by this parameter varies from zero to one less than the number of devices present.
            /// </param>
            /// <param name="lpMidiOutCaps">
            /// Pointer to a <see cref="MIDIOUTCAPS"/> structure.
            /// </param>
            /// <param name="cbMidiOutCaps">
            /// Size, in bytes, of the <see cref="MIDIOUTCAPS"/> structure. 
            /// </param>
            /// <returns></returns>
            [DllImport("winmm.dll")]
            internal static extern MMRESULT midiOutGetDevCaps(
                [In] IntPtr deviceId, 
                [In, Out] ref MIDIOUTCAPS lpMidiOutCaps,
                [In] uint cbMidiOutCaps);

            /// <summary>
            /// retrieves the device identifier for the given MIDI output device.
            /// </summary>
            /// <param name="midiOut">Handle to the MIDI output device.</param>
            /// <param name="deviceId">Pointer to a variable to be filled with the device identifier.</param>
            [DllImport("winmm.dll")]
            internal static extern MMRESULT midiOutGetID(
                [In] IntPtr midiOut, 
                [Out, MarshalAs(UnmanagedType.SysUInt)] out IntPtr deviceId);

            /// <summary>
            /// retrieves the number of MIDI output devices present in the system.
            /// </summary>
            [DllImport("winmm.dll")]
            internal static extern uint midiOutGetNumDevs();
            #endregion

            #region //=========================== Output Devices =========================

            /// <summary>
            /// 
            /// </summary>
            /// <param name="midiOut">
            /// Handle to an open MIDI output device. 
            /// This parameter can also contain the handle of a MIDI stream, as long as it is cast to HMIDIOUT. 
            /// This parameter can also be a device identifier.
            /// </param>
            /// <param name="lpdwVolume">
            /// Pointer to the location to contain the current volume setting.
            /// The low-order word of this location contains the left-channel volume setting,
            /// and the high-order word contains the right-channel setting.
            /// </param>
            [DllImport("winmm.dll")]
            internal static extern MMRESULT midiOutGetVolume(
                [In] IntPtr midiOut, 
                [In] IntPtr lpdwVolume);

            /// <summary>
            /// 
            /// </summary>
            /// <param name="midiOut">
            /// Handle to an open MIDI output device. 
            /// This parameter can also contain the handle of a MIDI stream, as long as it is cast to HMIDIOUT. 
            /// This parameter can also be a device identifier.
            /// </param>
            /// <param name="dwVolume">
            /// New volume setting. The low-order word contains the left-channel volume setting, 
            /// and the high-order word contains the right-channel setting. 
            /// </param>
            [DllImport("winmm.dll")]
            internal static extern MMRESULT midiOutSetVolume(
                [In] IntPtr midiOut, 
                [In] uint dwVolume);
            #endregion

            #region //======================= Playing Message(s) =========================

            /// <summary>
            /// sends a short MIDI message to the specified MIDI output device.
            /// </summary>
            /// <param name="midiOut">
            /// Handle to the MIDI output device. 
            /// This parameter can also be the handle of a MIDI stream cast to HMIDIOUT.
            /// </param>
            /// <param name="dwMsg">MIDI message.</param>
            [DllImport("winmm.dll")]
            internal static extern MMRESULT midiOutShortMsg(
                [In] IntPtr midiOut, 
                [In] uint dwMsg);

            /// <summary>
            /// sends a system-exclusive MIDI message to the specified MIDI output device.
            /// </summary>
            /// <param name="midiOut">
            /// Handle to the MIDI output device. 
            /// This parameter can also be the handle of a MIDI stream cast to HMIDIOUT.
            /// </param>
            /// <param name="lpMidiOutHdr">
            /// Pointer to a <see cref="MIDIHDR"/> structure that identifies the MIDI buffer.
            /// </param>
            /// <param name="cbMidiOutHdr">
            /// Size, in bytes, of the <see cref="MIDIHDR"/> structure.
            /// </param>
            [DllImport("winmm.dll")]
            internal static extern MMRESULT midiOutLongMsg(
                [In] IntPtr midiOut,
                [In] IntPtr lpMidiOutHdr,
                [In] uint cbMidiOutHdr);

            /// <summary>
            /// sends a message to the MIDI device drivers. 
            /// <remarks>
            /// This function is used only for driver-specific messages that are not supported by the MIDI API.
            /// </remarks>
            /// </summary>
            /// <param name="deviceId">
            /// Identifier of the MIDI device that receives the message. 
            /// must cast the device ID to the HMIDIOUT handle type. 
            /// </param>
            /// <param name="msg">Message to send.</param>
            /// <param name="dw1">Message parameter.</param>
            /// <param name="dw2">Message parameter.</param>
            [DllImport("winmm.dll")]
            internal static extern uint midiOutMessage(
                [In] IntPtr deviceId,
                [In] uint msg,
                [In] IntPtr dw1,
                [In] IntPtr dw2);

            /// <summary>
            /// turns off all notes on all MIDI channels for the specified MIDI output device.
            /// </summary>
            /// <param name="midiOut">
            /// Handle to the MIDI output device. 
            /// This parameter can also be the handle of a MIDI stream cast to HMIDIOUT.
            /// </param>
            [DllImport("winmm.dll")]
            internal static extern MMRESULT midiOutReset([In] IntPtr midiOut);
            #endregion

            #region //========================= Error Processing =========================

            /// <summary>
            /// retrieves a textual description for an error identified by the specified error code.
            /// </summary>
            /// <param name="mmrError">Error code.</param>
            /// <param name="lpText">Pointer to a buffer to be filled with the textual error description.</param>
            /// <param name="cchText">Length, in characters, of the buffer pointed to by lpText.</param>
            /// <returns></returns>
            [DllImport("winmm.dll", BestFitMapping = false, ThrowOnUnmappableChar = true)]
            internal static extern uint midiOutGetErrorText(
                [In] MMRESULT mmrError, 
                [In, Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder lpText,
                [In] uint cchText);
            #endregion

            #endregion
        }
    }
}