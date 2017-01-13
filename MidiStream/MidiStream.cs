using System.Collections.Generic;
using JetBrains.Annotations;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

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
        public string Name { get; }

        /// <summary>
        /// Gets the format of midi sequence.
        /// </summary>
        public MidiFormat Format { get; }

        /// <summary>
        /// Gets a read-only list of tracks.
        /// </summary>
        public IReadOnlyList<MidiTrack> Tracks { get; }

        #endregion Properties
        
        internal MidiStream([NotNull] IReadOnlyList<MidiTrack> tracks, [NotNull] MidiFormat format, string name = null)
        {
            Tracks = tracks;
            Format = format;
            Name = name ?? "";
        }

        public bool VerifyValidity()
        {
            if (Name == null) return false;
            if (Format == null) return false;
            if (Tracks == null) return false;
            return Format.VerifyValidity();
        }
    }
}
