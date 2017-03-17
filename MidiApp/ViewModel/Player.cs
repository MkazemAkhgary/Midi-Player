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

        private int _currentPlayback = -1;
        private bool _repeatTrack = false;
        private bool _repeatList = false;

        private TrackInfo _current;

        private object _synchronizingObject; // object used to check if execution of async method must continue or not.

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

        [NotNull]
        public DelegateCommand OpenOrToggle { get; }

        [NotNull]
        public DelegateCommand Stop { get; }

        [NotNull]
        public DelegateCommand Open { get; }

        [NotNull]
        public DelegateCommand Select { get; }

        [NotNull]
        public DelegateCommand Close { get; }

        [NotNull]
        public DelegateCommand Next { get; }

        [NotNull]
        public DelegateCommand Previous { get; }

        private async Task OpenOrToggleImpl()
        {
            if (!PlaybackList.Any())
            {
                await Open.RaiseCommandAsync();

                if (MidiPlayer.IsLoaded) MidiPlayer.Start();
                return;
            }

            if (!MidiPlayer.IsPlaying) MidiPlayer.Start();
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
                    MidiPlayer.Context.IsPlaybackPlaying = false;
                    return;
                }
            }

            object sync = new object();
            _synchronizingObject = sync;

            // reset play list
            PlaybackList.Clear();
            _currentPlayback = -1;

            foreach (var fileName in fileNames.OrderBy(x => x))
            {
                var trackInfo = new TrackInfo(fileName);
                PlaybackList.Add(trackInfo);
            }

            _current = PlaybackList.First();
            _currentPlayback = 0;
            await _current.Initialize().ConfigureAwait(false);
            await Play(_current);

            foreach (var trackInfo in PlaybackList.Skip(1))
            {
                var stat = TrackInfo.TrackStatus.Ready;
                try
                {
                    await trackInfo.Initialize().ConfigureAwait(false);
                }
                catch (Exception)
                {
                    stat = TrackInfo.TrackStatus.Error;
                }

                if (sync != _synchronizingObject) break;

                trackInfo.Status = stat;
            }
        }

        private async Task SelectImpl(TrackInfo track)
        {
            _currentPlayback = PlaybackList.IndexOf(track);
            await Play(track);
        }

        private void CloseImpl()
        {
            PlaybackList.Clear();
            MidiPlayer.Close();
        }

        private async Task NextImpl()
        {
            if (PlaybackList.Count == 0) return;

            _currentPlayback++;
            if (_currentPlayback == PlaybackList.Count) _currentPlayback = 0;

            await Play(PlaybackList[_currentPlayback]);
        }

        private async Task PreviousImpl()
        {
            if (PlaybackList.Count == 0) return;

            // if passed 2 seconds repeat o.w go to previous song.
            if (MidiPlayer.Context.RuntimePosition < 2000D)
            {
                _currentPlayback--;
            }

            if (_currentPlayback < 0) _currentPlayback = PlaybackList.Count - 1;

            await Play(PlaybackList[_currentPlayback]);
        }

        private async void OnPlaybackEnds(object sender, EventArgs e)
        {
            if (_repeatTrack) _currentPlayback--;

            if (_currentPlayback + 1 == PlaybackList.Count && !_repeatList) return;

            await Next.RaiseCommandAsync();
        }

        private async Task Play(TrackInfo info)
        {
            if(_current.Status == TrackInfo.TrackStatus.Error) return;

            _current.Status = TrackInfo.TrackStatus.Ready;
            info.Status = TrackInfo.TrackStatus.Playing;

            try
            {
                await MidiPlayer.Open(info);
            }
            catch (Exception)
            {
                info.Status = TrackInfo.TrackStatus.Error;
                return;
            }

            MidiPlayer.Start();
            _current = info;
        }

        public void Dispose()
        {
            MidiPlayer.Dispose();
        }
    }
}
