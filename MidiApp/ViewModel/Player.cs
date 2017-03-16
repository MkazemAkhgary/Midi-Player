using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Win32;
using MidiPlayer;
using Utilities.Presentation.Commands;

namespace MidiApp.ViewModel
{
    /// <summary>
    /// provides control over <see cref="MidiPlayer"/>.
    /// </summary>
    public class Player : IDisposable
    {
        private readonly OpenFileDialog _openFileDialog;

        public int CurrentPlayback = -1;
        private bool _repeatTrack = false;
        private bool _repeatList = false;

        [NotNull, ItemNotNull]
        public ObservableCollection<TrackInfo> PlaybackList { get; }

        [NotNull]
        public MidiPlayer.MidiPlayer MidiPlayer { get; }

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
            Select = DelegateCommand.CreateAsyncCommand<TrackInfo>(SelectImpl);
            Close = DelegateCommand.CreateCommand(CloseImpl);

            Next = DelegateCommand.CreateAsyncCommand(NextImpl);
            Previous = DelegateCommand.CreateAsyncCommand(PreviousImpl);

            PlaybackList = new ObservableCollection<TrackInfo>();
            MidiPlayer = new MidiPlayer.MidiPlayer();

            MidiPlayer.PlaybackEnds += OnPlaybackEnds;
        }

        [NotNull] public DelegateCommand OpenOrToggle { get; }
        [NotNull] public DelegateCommand Stop { get; }

        [NotNull] public DelegateCommand Open { get; }
        [NotNull] public DelegateCommand Select { get; }
        [NotNull] public DelegateCommand Close { get; }

        [NotNull] public DelegateCommand Next { get; }
        [NotNull] public DelegateCommand Previous { get; }

        private async Task OpenOrToggleImpl()
        {
            if (!PlaybackList.Any())
            {
                await Open.RaiseCommandAsync();
                
                if(MidiPlayer.IsLoaded) MidiPlayer.Start();
            }

            // due to two way data binding this condition is inverted.
            if (MidiPlayer.IsPlaying) MidiPlayer.Start(); 
            else MidiPlayer.Pause();
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

            PlaybackList.Clear();

            foreach (var fileName in fileNames.OrderBy(x => x))
            {
                var trackInfo = new TrackInfo(fileName);
                PlaybackList.Add(trackInfo);
            }

            await Next.RaiseCommandAsync();

            foreach (var trackInfo in PlaybackList.ToList()) // todo Tolist used to allow re-opening files.
            {
                await trackInfo.Initialize().ConfigureAwait(false);
            }
        }

        private async Task SelectImpl(TrackInfo track)
        {
            CurrentPlayback = PlaybackList.IndexOf(track) - 1;
            await Next.RaiseCommandAsync();
        }

        private void CloseImpl()
        {
            PlaybackList.Clear();
            MidiPlayer.Close();
        }

        private async Task NextImpl()
        {
            if(PlaybackList.Count == 0) return;

            CurrentPlayback++;
            if (CurrentPlayback == PlaybackList.Count) CurrentPlayback = 0;
            await MidiPlayer.Open(PlaybackList[CurrentPlayback].Path);

            MidiPlayer.Start();
        }

        private async Task PreviousImpl()
        {
            if (PlaybackList.Count == 0) return;

            // if passed 2 seconds repeat o.w go to previous song.
            if (MidiPlayer.Context.RuntimePosition < 2000D)
            {
                CurrentPlayback--;
            }

            if (CurrentPlayback < 0) CurrentPlayback = PlaybackList.Count - 1;
            await MidiPlayer.Open(PlaybackList[CurrentPlayback].Path);

            MidiPlayer.Start();
        }

        private async void OnPlaybackEnds(object sender, EventArgs e)
        {
            await Task.Delay(500); // bug, slider reaches end when next track plays. this delay fixes bug temporary.
            if (_repeatTrack) CurrentPlayback--;

            if (CurrentPlayback + 1 == PlaybackList.Count && !_repeatList) return;

            await Next.RaiseCommandAsync();
        }

        public void Dispose()
        {
            MidiPlayer.Dispose();
        }
    }
}
