using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedParameter.Local

namespace MidiApp.Behaviors.Composite
{
    public sealed partial class SliderCompositeBehavior : CompositeBehavior<Slider>
    {
        protected override void OnSelfAttached()
        {
            Host.Loaded += OnLoaded;
            Host.ValueChanged += OnValueChanged;
            Host.PreviewMouseLeftButtonUp += OnLeftButtonUp;
        }

        protected override void OnSelfDetaching()
        {
            Host.Loaded -= OnLoaded;
            Host.ValueChanged -= OnValueChanged;
            Host.PreviewMouseLeftButtonUp -= OnLeftButtonUp;

            GetTrack(Host).Thumb.DragCompleted -= OnDragCompleted;
        }

        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            GetTrack(Host).Thumb.DragCompleted += OnDragCompleted;
        }

        #region Update Values

        private static void OnSourceValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var soruce = (SliderCompositeBehavior)d;
            var newval = (double)e.NewValue;
            var oldval = (double)e.OldValue;
            var args = new RoutedPropertyChangedEventArgs<double>(oldval, newval);
            soruce.OnSourceValueChanged(soruce, args);
        }

        private void OnSourceValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (GetValueBindsToSource(Host))
            {
                Host.Value = e.NewValue;
            }
        }

        private void OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (GetSourceBindsToValue(Host))
            {
                SourceValue = e.NewValue;
            }
        }

        #endregion

        #region Raise Command
        
        private void OnLeftButtonUp(object sender, MouseButtonEventArgs args)
        {
            RaiseCommand(Host.Value); // in case move to point was enabled.
        }

        private void OnDragCompleted(object sender, DragCompletedEventArgs args)
        {
            if (args.Canceled) return;
            
            if (Math.Abs(args.HorizontalChange) > 0)
            {
                RaiseCommand(Host.Value);
            }
        }

        private void RaiseCommand(double value)
        {
            if (Command?.CanExecute(value) ?? false) Command.Execute(value);
        }

        #endregion
    }
}
