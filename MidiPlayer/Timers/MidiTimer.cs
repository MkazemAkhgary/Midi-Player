﻿using System;
using System.Diagnostics;
using System.Timers;
using MidiStream.Components.Header;

namespace Midi.Timers
{
    using Extensions;

    /// <summary>
    /// Timer for sequencing midi tracks. this class is thread safe.
    /// </summary>
    internal sealed class MidiTimer : IMidiTimer, IDisposable
    {
        public double Interval => _timer.Interval;

        #region Fields

        private TimeDivision _division;
        private readonly Timer _timer;
        private readonly Stopwatch _stopwatch;

        private volatile bool _gaurd;

        private double _intervalunit;

        private double ElapsedStaticTime => _stopwatch.ElapsedMilliseconds*_intervalunit;
        private double ElapsedRunTime => _stopwatch.ElapsedMilliseconds;

        #endregion Fields

        public MidiTimer()
        {
            _stopwatch = new Stopwatch();
            _timer = new Timer();
            _timer.Elapsed += OnTimerTick;
            Initialize(TimeDivision.Default);
        }

        public void Initialize(TimeDivision division)
        {
            _division = division;
        }

        #region Events

        private void OnTimerTick(object sender, ElapsedEventArgs e)
        {
            if (_gaurd) return;
            lock (this)
            {
                if (!_timer.Enabled) return;
                _gaurd = true;

                Beat?.Invoke(ElapsedStaticTime, ElapsedRunTime);

                _stopwatch.Restart();
                _gaurd = false;
            }
        }

        public event EventArgs<double, double> Beat;
        public event EventArgs<double> TempoChanged; 

        public void SetTempo(double tempo)
        {
            var interval = _division.GetResolution(tempo) / 1000;
            _timer.Interval = interval;
            _intervalunit = 1 / interval;
            TempoChanged?.Invoke(interval);
        }

        #endregion Events

        public void Start()
        {
            _stopwatch.Restart();
            _timer.Start();
        }

        public void Stop()
        {
            _stopwatch.Stop();
            _timer.Stop();
            lock (this) { }
        }

        public void Dispose()
        {
            _timer.Dispose();
            _stopwatch.Stop();
        }
    }
}
