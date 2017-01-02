namespace MidiStream.Components.Containers.Messages
{
    using Enums;
    using Properties;

    /// <summary>
    /// Initializes new instance of Midi <see cref="MetaMessage"/> Message.
    /// </summary>
    public sealed class MetaMessage : MidiMessage
    {
        /// <summary>
        /// Gets the sub type of meta message.
        /// </summary>
        public MetaType MetaType => (MetaType) Data[1];

        internal MetaMessage([NotNull]byte[] data) : base(data)
        {
        }
    }
}
