using System;
using JetBrains.Annotations;

namespace MidiStream.Components.Containers.Messages
{
    /// <summary>
    /// Midi Message interfance.
    /// </summary>
    public interface IMidiMessage : IEquatable<IMidiMessage>
    {
        [NotNull]
        byte[] Data { get; }
    }
}
