using System;

namespace MidiStream.Components.Containers.Messages
{
    /// <summary>
    /// Midi Message interfance.
    /// </summary>
    public interface IMidiMessage : IEquatable<IMidiMessage>
    {
        byte[] Data { get; }
    }
}
