// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable InconsistentNaming

namespace MidiStream.Components.Containers.Messages
{
    /// <summary>
    /// Initializes new instance of Midi <see cref="SysexMessage"/> Message.
    /// </summary>
    public class SysexMessage : MidiMessage
    {
        internal SysexMessage(byte[] data) : base(data)
        {
        }
    }
}
