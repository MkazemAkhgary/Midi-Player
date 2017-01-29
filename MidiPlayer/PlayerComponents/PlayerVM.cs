using System;
using System.Windows.Input;
using JetBrains.Annotations;
using Utilities.Presentation.NotifyPropertyChanged;

// ReSharper disable InconsistentNaming

namespace MidiPlayer.PlayerComponents
{
    using PlaybackComponents;

    public class PlayerVM : NotifyPropertyChanged
    {
        internal PlayerVM([NotNull] PlaybackData data) : base(
            null,
            canResetToDefaults: false,
            enableAutoPropertyChangedNotification: false)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            Data = data;
            Data.PropertyChanged += OnPropertyChanged;
        }

        private PlaybackData Data { get; }

        public bool IsPlaybackLoaded
        {
            get { return Data.IsLoaded; }
            set { Data.IsLoaded = value; }
        }

        public bool IsPlaybackPlaying
        {
            get { return Data.IsPlaying; }
            set { Data.IsPlaying = value; }
        }

        public double StaticDuration
        {
            get { return Data.StaticDuration; }
            set { Data.StaticDuration = value; }
        }

        public double StaticPosition
        {
            get { return Data.StaticPosition; }
            set { Data.StaticPosition = value; }
        }

        public double RuntimeDuration
        {
            get { return Data.RuntimeDuration; }
            set { Data.RuntimeDuration = value; }
        }

        public double RuntimePosition
        {
            get { return Data.RuntimePosition; }
            set { Data.RuntimePosition = value; }
        }

        public double PlaybackSpeed
        {
            get { return Data.PlaybackSpeed; }
            set { Data.PlaybackSpeed = value; }
        }

        [NotNull]
        public ICommand SeekTo { get; internal set; }
    }
}
