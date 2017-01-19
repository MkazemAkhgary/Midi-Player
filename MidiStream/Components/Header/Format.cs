using System;
using System.ComponentModel;

namespace MidiStream.Components.Header
{
    using Enums;

    /// <summary>
    /// Initializes a new instance of <see cref="MidiFormat"/>
    /// </summary>
    public class MidiFormat
    {
        #region Properties
        /// <summary>
        /// defines type of midi format.
        /// </summary>
        public MidiType Type { get; }

        /// <summary>
        /// gets the time division of midi format.
        /// </summary>
        public TimeDivision TimeDivision { get; }

        #endregion Properties

        internal MidiFormat(MidiType type, int timedivision)
        {
            if (!Enum.IsDefined(typeof(MidiType), type))
                throw new InvalidEnumArgumentException(nameof(type), (int) type, typeof(MidiType));
            if (timedivision <= 0) throw new ArgumentOutOfRangeException(nameof(timedivision));

            Type = type;
            TimeDivision = new TimeDivision(timedivision);
        }
    }
}
