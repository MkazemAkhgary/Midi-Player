﻿using System;
using System.Diagnostics;
using System.Timers;
using JetBrains.Annotations;
using MidiStream.Components.Header;
using Utilities.Threading.Timers;

namespace MidiPlayer.Timers
{
    using Extensions;

    /// <summary>
    /// Timer for sequencing midi tracks. this class is thread safe.
    /// </summary>
    internal sealed class MidiTimer : IMidiTimer, IDisposable
    {
        #region Fields

        private TimeDivision _division;
        private readonly SafeTimer _timer;
        private readonly Stopwatch _stopwatch;

        private double _intervalunit;

        #endregion Fields

        public MidiTimer()
        {
            _stopwatch = new Stopwatch();
            _timer = new SafeTimer();
            _timer.Elapsed += OnTimerTick;
            Initialize(TimeDivision.Default);
        }

        public void Initialize([NotNull] TimeDivision division)
        {
            if (division == null) throw new ArgumentNullException(nameof(division));

            if(_timer.Enabled) Stop();
            _division = division;
        }

        #region Events

        private void OnTimerTick(object sender, ElapsedEventArgs e)
        {
            _stopwatch.Stop();
            var runtime = _stopwatch.ElapsedMilliseconds;
            var fixedtime = runtime * _intervalunit;

            Beat?.Invoke(fixedtime, runtime);
            _stopwatch.Restart();
        }

        /// <summary>
        /// event that fires at every interval.
        /// </summary>
        public event EventArgs<double, double> Beat;
        /// <summary>
        /// event that fires when tempo is changed.
        /// </summary>
        public event EventArgs<double> TempoChanged;

        /// <summary>
        /// Sets new tempo and fires <see cref="TempoChanged"/> event. passes new calculated interval to this event.
        /// </summary>
        /// <param name="tempo">new tempo to set.</param>
        public void SetTempo(double tempo)
        {
            if (tempo <= 0) throw new ArgumentOutOfRangeException(nameof(tempo));

            var interval = _division.GetResolution(tempo) / 1000;
            _timer.Interval = interval;
            _intervalunit = 1 / interval;
            TempoChanged?.Invoke(interval);
        }

        #endregion Events

        /// <summary>
        /// start the timer.
        /// </summary>
        public void Start()
        {
            _stopwatch.Restart();
            _timer.Start();
        }

        /// <summary>
        /// stop the timer.
        /// </summary>
        public void Stop()
        {
            _stopwatch.Stop();
            _stopwatch.Reset();
            _timer.Stop();
        }

        public void Dispose()
        {
            _stopwatch.Stop();
            _timer.Dispose();
        }
    }
}
