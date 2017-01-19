using System;
using System.ComponentModel;
using System.Diagnostics;
using JetBrains.Annotations;

// ReSharper disable MemberCanBePrivate.Global

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
            public int FramesPerSecond { get; } = 24;
            public int TicksPerFrame { get; }
        #endregion FPS

        #region Properties and Methods

        [NotNull]
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
            if (microSecondsPerBeat <= 0) throw new ArgumentOutOfRangeException(nameof(microSecondsPerBeat));

            MicroSecondsPerBeat = (int)microSecondsPerBeat;
            return TickLength(microSecondsPerBeat);
        }

        #endregion Properties and Methods

        #region Constructors

        internal TimeDivision(int timedivision, int mspb = DefaultMicroSecondsPerBeat)
        {
            if (timedivision <= 0) throw new ArgumentOutOfRangeException(nameof(timedivision));
            if (mspb <= 0) throw new ArgumentOutOfRangeException(nameof(mspb));

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

            if (!Enum.IsDefined(typeof(FramesPerSecond), (FramesPerSecond)FramesPerSecond))
                throw new InvalidEnumArgumentException(nameof(FramesPerSecond), FramesPerSecond, typeof(FramesPerSecond));

            Debug.Assert(Enum.IsDefined(typeof(DivisionType), Type), "Enum.IsDefined(typeof(DivisionType), Type)");
        }

        #endregion Constructors
    }
}
