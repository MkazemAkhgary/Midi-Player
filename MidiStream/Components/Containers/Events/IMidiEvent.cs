using System;
using JetBrains.Annotations;

namespace MidiStream.Components.Containers.Events
{
    using Messages;

    #region Non Generic IMidiEvent

    public interface IMidiEvent : IComparable
    {
        long AbsoluteTicks { get; }

        [NotNull]
        object Message { get; }
    }

    #endregion Non Generic IMidiEvent

    #region Generic IMidiEvent

    /// <summary>
    /// <see cref="IMidiEvent{TMessage}"/> which containins data required to sent over MIDI device at a certain time.
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public interface IMidiEvent<out TMessage> : IMidiEvent, IComparable<IMidiEvent>, IEquatable<IMidiEvent> where TMessage : MidiMessage
    {
        [NotNull]
        new TMessage Message { get; }
    }

    #endregion Generic IMidiEvent
}
