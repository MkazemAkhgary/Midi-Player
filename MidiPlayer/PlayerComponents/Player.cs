using System;
using MidiStream;

namespace Midi.PlayerComponents
{
    using Commands;
    using PlaybackComponents;

    /// <summary>
    /// Controls playback of a sound from a .mid file.
    /// </summary>
    public sealed class Player : IDisposable
    {
        private readonly PlayerControl _control;
        public PlayerVM Context { get; }
        public string GetMidiOutputDeviceInfo => _control.GetOutputCapabilities.ToString();

        public bool IsLoaded { get; private set; }

        public Player()
        {
            var data = new PlaybackData();
            Context = new PlayerVM(data);
            _control = new PlayerControl(data);

            Context.Toggle = DelegateCommand.Create(_control.Toggle);
            Context.SeekTo = DelegateCommand.Create<double>(_control.SeekTo);
        }

        public void Stop()
        {
            _control.Stop();
        }

        public void Start()
        {
            _control.Start();
        }

        public void Open(string path)
        {
            using (var reader = new MidiStreamReader(path))
            {
                _control?.Initialize(reader.GetStream());
                IsLoaded = true;
            }
        }

        public void Close()
        {
            _control?.Close();
            IsLoaded = false;
        }

        public void Dispose()
        {
            _control.Dispose();
        }
    }
}
