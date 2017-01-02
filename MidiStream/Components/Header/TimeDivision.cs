using System;

namespace MidiStream.Components.Header
{
    using Enums;

    /// <summary>
    /// Initializes a new instance of <see cref="TimeDivision"/>
    /// </summary>
    public class TimeDivision
    {
        private const int DefaultMicroSecondsPerBeat = 500000;

        #region PPQ
            public int MicroSecondsPerBeat { get; private set; }
            public int TicksPerBeat { get; }
        #endregion PPQ

        #region FPS
            public int FramesPerSecond { get; }
            public int TicksPerFrame { get; }
        #endregion FPS

        #region Properties and Methods

        public static TimeDivision Default { get; } = new TimeDivision(60);

        public DivisionType Type { get; }

        private Func<double, double> TickLength { get; }

        /// <summary>
        /// returns tick length in microseconds
        /// </summary>
        /// <param name="microSecondsPerBeat">
        /// length of quarter note in microseconds.
        /// this will be ignored for FPS division formats.
        /// default length is 500000.</param>
        public double GetResolution(double microSecondsPerBeat = DefaultMicroSecondsPerBeat)
        {
            MicroSecondsPerBeat = (int)microSecondsPerBeat;
            return TickLength(microSecondsPerBeat);
        }

        #endregion Properties and Methods

        #region Constructors

        internal TimeDivision(int timedivision, int mspb = DefaultMicroSecondsPerBeat)
        {
            MicroSecondsPerBeat = mspb;
            var value = timedivision;

            // set strategy.
            if ((value & 0x8000) != 0)
            {
                Type = DivisionType.FPS;
                FramesPerSecond = (value & 0x7FFF) >> 8;
                TicksPerFrame = value & 0xFF;
                TickLength = _ => 1E6 / (FramesPerSecond * TicksPerFrame);
            }
            else
            {
                Type = DivisionType.PPQN;
                TicksPerBeat = value;
                TickLength = mpb => mpb / TicksPerBeat;
            }
        }

        #endregion Constructors
    }
}
