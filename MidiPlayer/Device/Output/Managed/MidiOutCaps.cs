﻿using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using static MidiStream.Helpers.Extension.Enumerations;

namespace Midi.Device.Output.Managed
{
    /// <summary>
    /// describes the capabilities of a MIDI output device.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [StructLayout(LayoutKind.Sequential)]
    internal struct MIDIOUTCAPS
    {
        /// <summary>
        /// Manufacturer identifier of the device driver for the MIDI output device.
        /// </summary>
        private ushort wMid;

        /// <summary>
        /// Product identifier of the MIDI output device.
        /// </summary>
        private ushort wPid;

        /// <summary>
        /// Version number of the device driver for the MIDI output device.
        /// </summary>
        private uint vDriverVersion; //MMVERSION

        /// <summary>
        /// Product name in a null-terminated string.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        private string szPname;

        /// <summary>
        /// Type of the MIDI output device.
        /// </summary>
        private Technology wTechnology;

        /// <summary>
        /// Number of voices supported by an internal synthesizer device. 
        /// </summary>
        private ushort wVoices;

        /// <summary>
        /// Maximum number of simultaneous notes that can be played by an internal synthesizer device.
        /// </summary>
        private ushort wNotes;

        /// <summary>
        /// Channels that an internal synthesizer device responds to, where the least significant bit refers to channel 0 and the most significant bit to channel 15.
        /// </summary>
        private ushort wChannelMask;

        /// <summary>
        /// Optional functionality supported by the device.
        /// </summary>
        private Support dwSupport;

        private enum Technology : ushort
        {
            /// <summary>
            /// output port
            /// </summary>
            [Description("MIDI hardware port")]
            MOD_MIDIPORT = 1,

            /// <summary>
            /// generic internal synth
            /// </summary>
            [Description("Synthesizer")]
            MOD_SYNTH = 2,

            /// <summary>
            /// square wave internal synth
            /// </summary>
            [Description("Square wave synthesizer")]
            MOD_SQSYNTH = 3,

            /// <summary>
            /// FM internal synth
            /// </summary>
            [Description("FM synthesizer")]
            MOD_FMSYNTH = 4,

            /// <summary>
            /// MIDI mapper
            /// </summary>
            [Description("Microsoft MIDI mapper")]
            MOD_MAPPER = 5,

            /// <summary>
            /// hardware wavetable synth
            /// </summary>
            [Description("Hardware wavetable synthesizer")]
            MOD_WAVETABLE = 6,

            /// <summary>
            /// software synth
            /// </summary>
            [Description("Software synthesizer")]
            MOD_SWSYNTH = 7
        }

        [Flags]
        private enum Support : uint
        {
            [Description("Supports patch caching.")]
            MIDICAPS_CACHE = 1,
            [Description("Supports separate left and right volume control.")]
            MIDICAPS_LRVOLUME = 2,
            [Description("Provides direct support for the midiStreamOut function.")]
            MIDICAPS_STREAM = 4,
            [Description("Supports volume control.")]
            MIDICAPS_VOLUME = 8 
        }

        public override string ToString()
        {
            string formatted = $@"Manufacturer Id : {wMid}
Product Id : {wPid}
Driver Version : {vDriverVersion}
Product Name : {szPname}
Midi Output Device Type : {GetAttribute<DescriptionAttribute>(wTechnology).Description}
Number of voices supported : {wVoices}
Maximum number of simultaneous notes : {wNotes}
Channel Mask : {Convert.ToString(wChannelMask, 2)}
{string.Join(Environment.NewLine,
                GetAllFlags(dwSupport)
                    .Select(GetAttribute<DescriptionAttribute>)
                    .Select(a => a.Description))}";

            return formatted;
        }
    }
}
