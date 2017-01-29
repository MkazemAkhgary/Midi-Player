using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Win32;
using Utilities.Presentation.Commands;

namespace MidiApp.ViewModel
{
    using MidiPlayer.PlayerComponents;

    public class Player : IDisposable
    {
        private readonly OpenFileDialog _openFileDialog;

        private int _currentPlayback = -1;

        [NotNull, ItemNotNull]
        public ObservableCollection<string> PlaybackList { get; }

        [NotNull]
        public MidiPlayer MidiPlayer { get; }

        public Player()
        {
            _openFileDialog = new OpenFileDialog
            {
                Filter = "Midi(*.mid)|*.mid|All Files(*.*)|*.*",
                DefaultExt = "*.mid",
                Multiselect = true,
            };

            OpenOrToggle = DelegateCommand.CreateAsyncCommand(OpenOrToggleImpl);
            Stop = DelegateCommand.CreateCommand(StopImpl);

            Open = DelegateCommand.CreateAsyncCommand<string[]>(OpenImpl);
            Close = DelegateCommand.CreateCommand(CloseImpl);

            Next = DelegateCommand.CreateAsyncCommand(NextImpl);
            Previous = DelegateCommand.CreateAsyncCommand(PreviousImpl);

            PlaybackList = new ObservableCollection<string>();
            MidiPlayer = new MidiPlayer();
        }

        [NotNull] public DelegateCommand OpenOrToggle { get; }
        [NotNull] public DelegateCommand Stop { get; }

        [NotNull] public DelegateCommand Open { get; }
        [NotNull] public DelegateCommand Close { get; }

        [NotNull] public DelegateCommand Next { get; }
        [NotNull] public DelegateCommand Previous { get; }

        private async Task OpenOrToggleImpl()
        {
            if (!PlaybackList.Any())
            {
                await Open.RaiseCommandAsync();
            }

            if (MidiPlayer.IsPlaying) MidiPlayer.Pause();
            else MidiPlayer.Start();
        }

        private void StopImpl()
        {
            MidiPlayer.Stop();
        }

        private async Task OpenImpl(string[] fileNames)
        {
            if (fileNames == null)
            {
                if (_openFileDialog.ShowDialog() == true)
                {
                    fileNames = _openFileDialog.FileNames;
                }
                else
                {
                    return;
                }
            }

            foreach (var fileName in fileNames)
            {
                PlaybackList.Add(fileName);
            }

            await Next.RaiseCommandAsync();
        }

        private void CloseImpl()
        {
            PlaybackList.Clear();
            MidiPlayer.Close();
        }

        private async Task NextImpl()
        {
            await MidiPlayer.Open(PlaybackList[++_currentPlayback]);
        }

        private async Task PreviousImpl()
        {
            await MidiPlayer.Open(PlaybackList[--_currentPlayback]);
        }

        public void Dispose()
        {
            MidiPlayer.Dispose();
        }
    }
}
