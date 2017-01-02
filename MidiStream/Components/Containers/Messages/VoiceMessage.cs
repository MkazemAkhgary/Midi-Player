namespace MidiStream.Components.Containers.Messages
{
    using Enums;
    using Properties;

    /// <summary>
    /// Initializes new instance of Midi <see cref="VoiceMessage"/> Message.
    /// </summary>
    public sealed class VoiceMessage : MidiMessage
    {
        /// <summary>
        /// Gets the sub type of voice message.
        /// </summary>
        public VoiceType VoiceType => (VoiceType)(Data[0] & 0xF0);
        
        internal VoiceMessage([NotNull]byte[] data) : base(data)
        {
        }

        /// <summary>
        /// packs voice message into single integer value.
        /// </summary>
        /// <returns></returns>
        public uint PackMessage() => PackMessage(Data);

        public static unsafe uint PackMessage(byte[] data)
        {
            fixed (byte* p = &data[0]) return *(uint*) p;
        }
    }
}