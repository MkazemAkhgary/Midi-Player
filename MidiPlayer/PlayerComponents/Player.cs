using System;
using MidiStream;

namespace MidiPlayer.PlayerComponents
{
    using Commands;
    using PlaybackComponents;

    /// <summary>
    /// Controls playback of a sound from a .mid file.
    /// </summary>
    public sealed class Player : IDisposable
    {
        private readonly PlayerControl _control;
        private readonly PlayerVM _context;

        public PlayerVM Context => _context;
        public bool IsLoaded => Context.IsPlaybackLoaded;
        public bool IsPlaying => Context.IsPlaybackPlaying;
        public string GetMidiOutputDeviceInfo => _control.GetOutputCapabilities.ToString();

        public Player()
        {
            var data = new PlaybackData();
            _context = new PlayerVM(data);
            _control = new PlayerControl(data);

            Context.Toggle = DelegateCommand.Create(_control.Toggle, o => o as bool? ?? true);
            Context.SeekTo = DelegateCommand.Create<double>(_control.SeekTo);
        }

        public void Stop()
        {
            if (Context.IsPlaybackLoaded) _control.Stop();
        }

        public void Start()
        {
            if (Context.IsPlaybackLoaded) _control.Start();
        }

        public void Open(string path)
        {
            using (var reader = new MidiStreamReader(path))
            {
                _control?.Initialize(reader.GetStream());
                Context.IsPlaybackLoaded = true;
            }
        }

        public void Close()
        {
            _control?.Close();
            Context.IsPlaybackLoaded = false;
        }

        public void Dispose()
        {
            _context.Dispose();
            _control.Dispose();
            Context.Dispose();
        }
    }
}
