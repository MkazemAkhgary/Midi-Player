using System;
using System.Collections.ObjectModel;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Win32;
using Utilities.Presentation.Commands;

namespace MidiApp
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

            OpenOrToggle = DelegateCommand.Create(OpenOrToggleImpl);

            Open = DelegateCommand.Create<string[]>(OpenImpl);
            Close = DelegateCommand.Create(CloseImpl);

            Next = DelegateCommand.Create(NextImpl);
            Previous = DelegateCommand.Create(PreviousImpl);

            PlaybackList = new ObservableCollection<string>();
            MidiPlayer = new MidiPlayer();
        }

        [NotNull] public DelegateCommand OpenOrToggle { get; }

        [NotNull] public DelegateCommand Open { get; }
        [NotNull] public DelegateCommand Close { get; }

        [NotNull] public DelegateCommand Next { get; }
        [NotNull] public DelegateCommand Previous { get; }

        private void OpenOrToggleImpl()
        {
            if (!PlaybackList.Any())
            {
                Open.RaiseCommand();
                return;
            }

            if (MidiPlayer.IsPlaying) MidiPlayer.Pause();
            else MidiPlayer.Start();
        }

        private void OpenImpl(string[] fileNames)
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

            Next.RaiseCommand();
        }

        private void CloseImpl()
        {
            PlaybackList.Clear();
            MidiPlayer.Close();
        }

        private async void NextImpl()
        {
            await MidiPlayer.Open(PlaybackList[++_currentPlayback]);
        }

        private async void PreviousImpl()
        {
            await MidiPlayer.Open(PlaybackList[--_currentPlayback]);
        }

        public void Dispose()
        {
            MidiPlayer.Dispose();
        }
    }
}
