using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using MidiStream.Components.Containers.Messages;
using MidiStream.Enums;
using Synthesizer.Device.Output;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable RedundantEmptyDefaultSwitchBranch

namespace MidiPlayer.Dispatchers
{
    using PlaybackComponents;

    /// <summary>
    /// Dispatches midi messages over Midi output device.
    /// </summary>
    internal static class MessageDispatcher
    {
        public static void DispatchTo([NotNull] this IEnumerable<VoiceMessage> soruce, [NotNull] MidiOutput output)
        {
            Debug.Assert(soruce != null, "soruce != null");
            Debug.Assert(output != null, "output != null");

            foreach (var msg in soruce)
            {
                msg.DispatchTo(output);
            }
        }

        public static void DispatchTo([NotNull] this IEnumerable<MetaMessage> soruce, [NotNull] PlaybackControl control)
        {
            Debug.Assert(soruce != null, "soruce != null");
            Debug.Assert(control != null, "control != null");

            foreach (var msg in soruce)
            {
                msg.DispatchTo(control);
            }
        }

        public static void DispatchTo([NotNull] this VoiceMessage message, [NotNull] MidiOutput output)
        {
            Debug.Assert(message != null, "message != null");
            Debug.Assert(output != null, "output != null");

            output.SendMessage(message.PackMessage());
        }

        public static void DispatchTo([NotNull] this MetaMessage message, [NotNull] PlaybackControl control)
        {
            Debug.Assert(message != null, "message != null");
            Debug.Assert(control != null, "control != null");

            // todo implement other actions for meta messages.
            switch (message.MetaType)
            {
                case MetaType.SetTempo:
                    control.SetPlaybackSpeed(ReadTempo(message.Data));
                    break;
                case MetaType.SequenceNumber:
                    break;
                case MetaType.Text:
                    break;
                case MetaType.CopyrightNotice:
                    break;
                case MetaType.TrackName:
                    break;
                case MetaType.InstrumentName:
                    break;
                case MetaType.Lyrics:
                    break;
                case MetaType.Marker:
                    break;
                case MetaType.CuePoint:
                    break;
                case MetaType.ChannelPrefix:
                    break;
                case MetaType.EndOfTrack:
                    break;
                case MetaType.SMPTE_Offset:
                    break;
                case MetaType.TimeSignature:
                    break;
                case MetaType.KeySignature:
                    break;
                case MetaType.SequencerSpecific:
                    break;
                case MetaType.Reset:
                    break;
                default:
                    //throw new ArgumentOutOfRangeException();
                    break;
            }
        }

        public static int ReadTempo([NotNull] byte[] data)
        {
            Debug.Assert(data.Length == 6, "data.Length == 6");

            if (data == null) throw new ArgumentNullException(nameof(data));

            return (data[3] << 16) | (data[4] << 8) | data[3];
        }
    }
}
