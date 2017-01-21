using System;
using System.Diagnostics;
using System.Timers;
using JetBrains.Annotations;

namespace Utilities.Threading.Timers
{
    public sealed class SafeTimer : IDisposable
    {
        public event ElapsedEventHandler Elapsed;

        [NotNull] private readonly Timer _timer;

        private volatile bool _gaurd;

        public SafeTimer()
        {
            _timer = new Timer();
            _timer.Elapsed += OnElapsed;
        }

        public double Interval
        {
            get { return _timer.Interval; }
            set { _timer.Interval = value; }
        }

        public bool AutoReset
        {
            get { return _timer.AutoReset; }
            set { _timer.AutoReset = value; }
        }

        public bool Enabled
        {
            get { return _timer.Enabled; }
            set { _timer.Enabled = value; }
        }

        private void OnElapsed(object sender, ElapsedEventArgs e)
        {
            if (_gaurd) return;
            lock (this)
            {
                _gaurd = true;

                Elapsed?.Invoke(this, e);

                _gaurd = false;
            }
        }
        
        public void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
            lock (this) { } // this ensures timer is fully stopped.
        }

        public void Dispose()
        {
            _timer.Stop();
            _timer.Dispose();
        }
    }
}
