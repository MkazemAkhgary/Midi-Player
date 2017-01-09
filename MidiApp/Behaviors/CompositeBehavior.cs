using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Markup;

namespace MidiApp.Behaviors
{
    [ContentProperty(nameof(BehaviorCollection))]
    public abstract class CompositeBehavior<T> : Behavior<T>
        where T : DependencyObject
    {
        #region Behavior Collection

        private readonly HashSet<Type> _behaviors = new HashSet<Type>();

        public static readonly DependencyProperty BehaviorCollectionProperty =
            DependencyProperty.Register(
                $"{nameof(CompositeBehavior<T>)}",
                typeof(ObservableCollection<Behavior<T>>),
                typeof(CompositeBehavior<T>),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.NotDataBindable));

        public ObservableCollection<Behavior<T>> BehaviorCollection
        {
            get
            {
                var collection = GetValue(BehaviorCollectionProperty) as ObservableCollection<Behavior<T>>;

                if (collection == null)
                {
                    collection = new ObservableCollection<Behavior<T>>();
                    collection.CollectionChanged += OnCollectionChanged;
                    SetValue(BehaviorCollectionProperty, collection);
                }

                return collection;
            }
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.OldItems != null)
            {
                foreach (var behavior in args.OldItems)
                {
                    if (_behaviors.Remove(behavior.GetType()) == false)
                    {
                        throw new InvalidOperationException($"{behavior.GetType().Name} could not be located.");
                    }
                }
            }

            if (args.NewItems != null)
            {
                foreach (var behavior in args.NewItems)
                {
                    if (_behaviors.Add(behavior.GetType()) == false)
                    {
                        throw new InvalidOperationException($"{behavior.GetType().Name} is set more than once.");
                    }
                }
            }
        }

        #endregion

        protected override void OnAttached()
        {
            foreach (var behavior in BehaviorCollection)
            {
                behavior.Attach(AssociatedObject);
            }
        }

        protected override void OnDetaching()
        {
            foreach (var behavior in BehaviorCollection)
            {
                behavior.Detach();
            }
        }
    }
}
