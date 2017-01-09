using Utilities.Properties;

namespace MidiStream.Components.Containers.Events
{
    using Messages;

    /// <summary>
    /// Initializes a new instance of <see cref="MidiEvent{TMessage}"/> which containins data required to sent over MIDI device at a certain time.
    /// </summary>
    public class MidiEvent<TMessage> : IMidiEvent<TMessage> where TMessage : MidiMessage
    {

        internal MidiEvent(long ticks, [NotNull]TMessage message)
        {
            AbsoluteTicks = ticks;
            Message = message;
        }

        #region Properties

        /// <summary>
        /// Gets the time this event should occur in ticks./>
        /// </summary>
        public long AbsoluteTicks { get; }

        /// <summary>
        /// Message that must be dispatched to midi device when this event occurs.
        /// </summary>
        public TMessage Message { get; }
        object IMidiEvent.Message => Message;

        #endregion Properties

        #region Implemented Methods

        public bool Equals(IMidiEvent obj)
        {
            if (obj == null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (AbsoluteTicks != obj.AbsoluteTicks) return false;
            return Message.Equals(obj.Message);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IMidiEvent);
        }

        public override int GetHashCode()
        {
            return -AbsoluteTicks.GetHashCode() ^ ~Message.GetHashCode();
        }

        public int CompareTo(IMidiEvent other)
        {
            return AbsoluteTicks.CompareTo(other.AbsoluteTicks);
        }

        public int CompareTo(object obj)
        {
            if (!(obj is IMidiEvent)) return 1;
            return AbsoluteTicks.CompareTo(((IMidiEvent) obj).AbsoluteTicks);
        }

        #endregion Implemented Methods

        public static MidiEvent<TMessage> CreateEmpty(long ticks)
        {
            return new MidiEvent<TMessage>(ticks, null);
        }
    }
}
