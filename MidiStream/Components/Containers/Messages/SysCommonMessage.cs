using JetBrains.Annotations;

namespace MidiStream.Components.Containers.Messages
{
    /// <summary>
    /// Initializes new instance of Midi <see cref="SysCommonMessage"/> Message.
    /// </summary>
    public class SysCommonMessage : MidiMessage
    {
        internal SysCommonMessage([NotNull] byte[] data) : base(data)
        {
        }
    }
}
