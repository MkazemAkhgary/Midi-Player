using System;

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
            Type = type;
            TimeDivision = new TimeDivision(timedivision);
        }

        public bool VerifyValidity()
        {
            return Enum.IsDefined(typeof(MidiType), Type) && TimeDivision.VerifyValidity();
        }
    }
}
