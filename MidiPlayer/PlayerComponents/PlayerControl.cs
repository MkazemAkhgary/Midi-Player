namespace Midi.PlayerComponents
{
    using PlaybackComponents;
    using System;
    using Timers;
    using MidiStream;

    public sealed class PlayerControl : IDisposable
    {
        private bool _isPlaying;
        private readonly MidiTimer _timer;
        private readonly PlaybackControl _control;

        internal PlayerControl(PlaybackData data)
        {
            _timer = new MidiTimer();
            _control = new PlaybackControl(data);

            _control.OnPlaybackEnds += Stop;

            data.TempoChanged += _timer.SetTempo; // bind tempo
            _timer.TempoChanged += data.SetTempo;

            _timer.Beat += _control.Move;
        }

        internal void Initialize(MidiStream stream)
        {
            _timer.Initialize(stream.Format.TimeDivision);
            _control.Initialize(stream.Tracks);
        }

        /// <summary>
        /// Stops playback if it's playing, otherwise starts.
        /// </summary>
        public void Toggle()
        {
            _isPlaying = !_isPlaying;
            if (_isPlaying) Start();
            else Pause();
        }

        /// <summary>
        /// Playback starts.
        /// </summary>
        public void Start()
        {
            _isPlaying = true;
            _timer?.Start();
            _control.Start();
        }

        /// <summary>
        /// Playback stops, and all sounds turn off.
        /// </summary>
        public void Pause()
        {
            _isPlaying = false;
            _timer?.Stop();
            _control.Pause();
        }

        /// <summary>
        /// Playback stops, all sounds turn off, And cue is reset.
        /// </summary>
        public void Stop()
        {
            _isPlaying = false;
            _timer?.Stop();
            _control?.Stop();
        }

        /// <summary>
        /// Playback stops, then seeks to given time, starts again if it was playing.
        /// </summary>
        /// <param name="time"></param>
        public void SeekTo(double time)
        {
            _timer?.Stop();
            _control?.Reset();
            _control?.Seek(time);
            if(_isPlaying) Start();
        }

        /// <summary>
        /// close playback.
        /// </summary>
        public void Close()
        {
            _isPlaying = false;
            _timer?.Stop();
            _control?.Close();
        }

        public void Dispose()
        {
            _timer.Dispose();
            _control.Dispose();
        }
    }
}
