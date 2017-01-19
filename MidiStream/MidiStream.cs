using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace MidiStream
{
    using Components.Containers.Tracks;
    using Components.Header;

    /// <summary>
    /// Initializes a new instance of <see cref="MidiStream"></see> with given list of <see cref="MidiTrack"/>'s and <see cref="MidiFormat"/>
    /// </summary>
    public class MidiStream
    {
        #region Properties
        /// <summary>
        /// Name of the stream.
        /// </summary>
        [NotNull]
        public string Name { get; }

        /// <summary>
        /// Gets the format of midi sequence.
        /// </summary>
        [NotNull]
        public MidiFormat Format { get; }

        /// <summary>
        /// Gets a read-only list of tracks.
        /// </summary>
        [NotNull]
        public IReadOnlyList<MidiTrack> Tracks { get; }

        #endregion Properties
        
        internal MidiStream(
            [NotNull] IReadOnlyList<MidiTrack> tracks,
            [NotNull] MidiFormat format,
            [CanBeNull] string name = null)
        {
            if(tracks == null) throw new ArgumentNullException(nameof(tracks));
            if(format == null) throw new ArgumentNullException(nameof(format));

            Tracks = tracks;
            Format = format;
            Name = name ?? "";
        }
    }
}
