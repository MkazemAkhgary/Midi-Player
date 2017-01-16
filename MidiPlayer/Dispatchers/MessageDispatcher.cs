using System.Collections.Generic;
using System.Linq;
using MidiStream.Components.Containers.Messages;
using MidiStream.Enums;
using Synthesizer.Device.Output;
using Utilities.Extensions;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable RedundantEmptyDefaultSwitchBranch

namespace MidiPlayer.Dispatchers
{
    using PlaybackComponents;

    internal static class MessageDispatcher
    {
        public static void DispatchTo(this IEnumerable<VoiceMessage> soruce, MidiOutput output)
        {
            foreach (var msg in soruce)
            {
                msg.DispatchTo(output);
            }
        }

        public static void DispatchTo(this IEnumerable<MetaMessage> soruce, PlaybackControl control)
        {
            foreach (var msg in soruce)
            {
                msg.DispatchTo(control);
            }
        }

        public static void DispatchTo(this VoiceMessage message, MidiOutput output)
        {
            output.SendMessage(message.PackMessage());
        }

        public static void DispatchTo(this MetaMessage message, PlaybackControl control)
        {
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

        public static int ReadTempo(byte[] data)
        {
            return NumericExtensions.ByteArrayToInt(data.Skip(2).ToArray());
        }
    }
}
