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

        private MidiTrack(
            ReadOnlyGrouping<VoiceType, MidiEvent<VoiceMessage>> voiceEvents,
            ReadOnlyGrouping<MetaType, MidiEvent<MetaMessage>> metaEvents,
            ReadOnlyCollection<IMidiEvent<MidiMessage>> unknownEvents)
        {
            var e1 = voiceEvents.LastOrDefault()?.AbsoluteTicks ?? 0;
            var e2 = metaEvents.LastOrDefault()?.AbsoluteTicks ?? 0;

            TotalTicks = Math.Max(e1, e2);

            UnknownEvents = unknownEvents;
            VoiceEvents = voiceEvents;
            MetaEvents = metaEvents;
        }

        #endregion Constructors

        #region Factory Methods

        internal static MidiTrack CreateTrack(
            [NotNull, ItemNotNull, NoEnumeration] IEnumerable<IMidiEvent<MidiMessage>> events)
        {
            if (events == null) throw new ArgumentNullException(nameof(events));

            var voiceEvents = new Grouping<VoiceType, MidiEvent<VoiceMessage>>(k => k.Message.VoiceType);
            var metaEvents = new Grouping<MetaType, MidiEvent<MetaMessage>>(k => k.Message.MetaType);

            // filter different events into their respective collection.
            var unknownEvents = events.Sieve(voiceEvents).Sieve(metaEvents).ToReadOnlyCollection(x => x);

            return new MidiTrack(voiceEvents.AsReadOnly(), metaEvents.AsReadOnly(), unknownEvents);
        }

        internal static async Task<MidiTrack> CreateTrackAsync(
            [NotNull, ItemNotNull, NoEnumeration] IEnumerable<Task<IMidiEvent<MidiMessage>>> events)
        {
            if (events == null) throw new ArgumentNullException(nameof(events));

            var voiceEvents = new Grouping<VoiceType, MidiEvent<VoiceMessage>>(k => k.Message.VoiceType);
            var metaEvents = new Grouping<MetaType, MidiEvent<MetaMessage>>(k => k.Message.MetaType);
            var unknownEvents = new Collection<IMidiEvent<MidiMessage>>();

            var actions = new Dictionary<Type, Action<IMidiEvent>>
            {
                {typeof(MidiEvent<VoiceMessage>), e => voiceEvents.Add((MidiEvent<VoiceMessage>) e)},
                {typeof(MidiEvent<MetaMessage>), e => metaEvents.Add((MidiEvent<MetaMessage>) e)},

                {typeof(MidiEvent<SysexMessage>), e => unknownEvents.Add((IMidiEvent<MidiMessage>) e)},
                {typeof(MidiEvent<SysRealtimeMessage>), e => unknownEvents.Add((IMidiEvent<MidiMessage>) e)},
                {typeof(MidiEvent<SysCommonMessage>), e => unknownEvents.Add((IMidiEvent<MidiMessage>) e)},
            };

            // filter different events into their respective collection.
            await events.AwaitForeach(e =>
            {
                actions[e.GetType()](e);
            });

            return new MidiTrack(
                voiceEvents.AsReadOnly(),
                metaEvents.AsReadOnly(),
                unknownEvents.ToReadOnlyCollection(x => x));
        }

        #endregion
    }
}
