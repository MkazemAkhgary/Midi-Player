using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MidiStream.Exceptions
{
    using Enums;
    using static Helpers.Extension.Enumerations;
    [Serializable]
    public sealed class MidiStreamException : Exception
    {
#if DEBUG
        private static string Format(
            MidiException exception,
            string path = null,
            string caller = null,
            int? line = null)
        {
            return $@"{GetAttribute<DescriptionAttribute>(exception).Description}.
Exception occured at ""{caller}"" in ""{path}"" within line ""{line}""";
        }

        internal MidiStreamException(
            MidiException exception,
            [CallerFilePath] string path = null,
            [CallerMemberName] string caller = null,
            [CallerLineNumber] int line = -1
        ) : base(Format(exception, path, caller, line))
        {
        }
#else
        internal MidiStreamException(MidiException exception) : base(GetAttribute<DescriptionAttribute>(exception).Description)
        {
        }
#endif
    }
}
