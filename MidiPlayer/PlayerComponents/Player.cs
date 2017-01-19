using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

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

        [NotNull]
        public PlayerVM Context { get; }

        public bool IsLoaded => Context.IsPlaybackLoaded;
        public bool IsPlaying => Context.IsPlaybackPlaying;
        public string GetMidiOutputDeviceInfo => _control.GetOutputCapabilities.ToString();

        public Player()
        {
            var data = new PlaybackData();
            Context = new PlayerVM(data);
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

        public async Task<bool> Open([NotNull] string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(path));

            using (var reader = new MidiStreamReader(path))
            {
                if(IsLoaded) Close();
                var stream = await reader.GetStreamAsync();
                return OpenStream(stream);
            }
        }

        private bool OpenStream([NotNull] MidiStream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

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
