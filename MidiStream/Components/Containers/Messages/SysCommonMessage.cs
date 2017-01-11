using JetBrains.Annotations;

namespace MidiStream.Components.Containers.Messages
{
    public class SysCommonMessage : MidiMessage
    {
        internal SysCommonMessage([NotNull] byte[] data) : base(data)
        {
        }
    }
}
