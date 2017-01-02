using System;
using System.Collections.Generic;
using MidiStream.Components.Containers.Events;
using MidiStream.Components.Containers.Messages;
using MidiStream.Components.Containers.Tracks;
using MidiStream.Enums;

namespace Midi.PlaybackComponents
{
    using Sequencer;

    internal class Playback : IDisposable
    {
        private readonly Sequence<VoiceType, VoiceMessage> _voice;
        private readonly Sequence<MetaType, MetaMessage> _meta;

        /// <summary>
        /// Gets whether sequencer reached end of sequences or not.
        /// </summary>
        public bool Ends => _voice.Ends && _meta.Ends;

        public Playback(MidiTrack track)
        {
            _voice = new Sequence<VoiceType, VoiceMessage>(track.VoiceEvents);
            _meta = new Sequence<MetaType, MetaMessage>(track.MetaEvents);
        }

        /// <summary>
        /// get all remaining voice events from current position upto given position and update current position.
        /// </summary>
        public IEnumerable<VoiceMessage> NextVoiceMessages(double position)
        {
            return _voice.Next(position);
        }

        /// <summary>
        /// get all remaining meta events from current position upto given position and update current position.
        /// </summary>
        public IEnumerable<MetaMessage> NextMetaMessages(double position)
        {
            return _meta.Next(position);
        }

        /// <summary>
        /// get all voice events from specified keys.
        /// </summary>
        /// <param name="include">indicates whether include or exclude keys.</param>
        /// <param name="keys">which keys to include or exclude.</param>
        public IEnumerable<MidiEvent<VoiceMessage>> GetVoiceEvents(bool include = true, params VoiceType[] keys)
        {
            return _voice.All(include, keys);
        }

        /// <summary>
        /// get all meta events from specified keys.
        /// </summary>
        /// <param name="include">indicates whether include or exclude keys.</param>
        /// <param name="keys">which keys to include or exclude.</param>
        public IEnumerable<MidiEvent<MetaMessage>> GetMetaEvents(bool include = true, params MetaType[] keys)
        {
            return _meta.All(include, keys);
        }

        /// <summary>
        /// Sets current position.
        /// </summary>
        public void Seek(double position)
        {
            _voice.Seek(position);
            _meta.Seek(position);
        }

        public void Dispose()
        {
            _voice.Dispose();
            _meta.Dispose();
        }
    }
}
