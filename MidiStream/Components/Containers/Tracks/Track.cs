using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Utilities.Collections;
using Utilities.Extensions;

namespace MidiStream.Components.Containers.Tracks
{
    using Events;
    using Messages;
    using Enums;

    /// <summary>
    /// Initializes a new instance of <see cref="MidiTrack"/>.
    /// </summary>
    public class MidiTrack
    {
        #region Properties

        [NotNull, ItemNotNull]
        public ReadOnlyCollection<IMidiEvent<MidiMessage>> UnknownEvents { get; }

        /// <summary>
        /// Gets a read-only branched list of voice events.
        /// </summary>
        [NotNull, ItemNotNull]
        public ReadOnlyGrouping<VoiceType, MidiEvent<VoiceMessage>> VoiceEvents { get; }

        /// <summary>
        /// Gets a read-only branched list of meta events.
        /// </summary>
        [NotNull, ItemNotNull]
        public ReadOnlyGrouping<MetaType, MidiEvent<MetaMessage>> MetaEvents { get; }

        /// <summary>
        /// gets the maximum absolute tick from available events in this track.
        /// </summary>
        public long TotalTicks { get; }

        #endregion Properties

        #region Constructors

        internal MidiTrack([NotNull] [ItemNotNull, NoEnumeration]IEnumerable<IMidiEvent<MidiMessage>> events)
        {
            if (events == null) throw new ArgumentNullException(nameof(events));

            var voiceEvents = new Grouping<VoiceType, MidiEvent<VoiceMessage>>(k => k.Message.VoiceType);
            var metaEvents = new Grouping<MetaType, MidiEvent<MetaMessage>>(k => k.Message.MetaType);

            // filter different events into their respective collection.
            UnknownEvents = events.Sieve(voiceEvents).Sieve(metaEvents).ToReadOnlyCollection(x => x);
            
            // calculate total duration.
            var e1 = voiceEvents.LastOrDefault()?.AbsoluteTicks ?? 0;
            var e2 = metaEvents.LastOrDefault()?.AbsoluteTicks ?? 0;
            TotalTicks = Math.Max(e1, e2);

            VoiceEvents = voiceEvents.AsReadOnly();
            MetaEvents = metaEvents.AsReadOnly();
        }

        #endregion Constructors
    }
}
