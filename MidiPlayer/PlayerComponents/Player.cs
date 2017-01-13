using System;

namespace MidiPlayer.PlayerComponents
{
    using MidiStream;
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

        public bool Open(string path)
        {
            using (var reader = new MidiStreamReader(path))
            {
                if(IsLoaded) Close();
                var stream = reader.GetStream();
                return OpenStream(stream);
            }
        }

        private bool OpenStream(MidiStream stream)
        {
            if(!stream.VerifyValidity()) return false;

            _control?.Initialize(stream);
            return true;
        }

        public void Close()
        {
            _control?.Close();
        }

        public void Dispose()
        {
            _control.Dispose();
        }
    }
}
