using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Markup;
// ReSharper disable StaticMemberInGenericType

namespace MidiApp.Behaviors.Composite
{
    [ContentProperty(nameof(BehaviorCollection))]
    public abstract class CompositeBehavior<T> : Behavior<T>
        where T : DependencyObject
    {
        protected T Host => AssociatedObject;

        #region Reference Holder

        private static readonly DependencyPropertyKey ReferenceKey =
            DependencyProperty.RegisterReadOnly(
                $"{nameof(CompositeBehavior<T>)}<{typeof(T).Name}>.Reference",
                typeof(CompositeBehavior<T>),
                typeof(CompositeBehavior<T>),
                new PropertyMetadata(null));

        private static readonly DependencyProperty ReferenceProperty = ReferenceKey.DependencyProperty;

        protected static CompositeBehavior<T> GetReference(T host)
        {
            var reference = (CompositeBehavior<T>)host.GetValue(ReferenceProperty);

            if (reference == null)
            {
                reference = new DefaultCompositeBehavior();
                host.SetValue(ReferenceKey, reference);
            }

            return reference;
        }

        #endregion
        
        #region Behavior Collection

        public static readonly DependencyProperty BehaviorCollectionProperty =
            DependencyProperty.Register(
                $"{nameof(CompositeBehavior<T>)}<{typeof(T).Name}>",
                typeof(BehaviorCollection),
                typeof(CompositeBehavior<T>),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.NotDataBindable));

        public BehaviorCollection BehaviorCollection
        {
            get
            {
                var collection = GetValue(BehaviorCollectionProperty) as BehaviorCollection;

                if (collection == null)
                {
                    var constructor = typeof(BehaviorCollection)
                        .GetConstructor(
                            BindingFlags.NonPublic | BindingFlags.Instance,
                            null, CallingConventions.HasThis,
                            Type.EmptyTypes, null);

                    collection = (BehaviorCollection) constructor.Invoke(null);
                    collection.Changed += OnCollectionChanged;
                    SetValue(BehaviorCollectionProperty, collection);
                }

                return collection;
            }
        }

        private void OnCollectionChanged(object sender, EventArgs args)
        {
            var hashset = new HashSet<Type>();

            foreach (var behavior in BehaviorCollection)
            {
                if (behavior is Behavior<T> == false)
                {
                    throw new InvalidOperationException($"{behavior.GetType().Name} does not inherit from {typeof(Behavior<T>).Name}.");
                }
                if (hashset.Add(behavior.GetType()) == false)
                {
                    throw new InvalidOperationException($"{behavior.GetType().Name} is set more than once.");
                }
            }
        }

        #endregion

        protected sealed override void OnAttached()
        {
            Host.SetValue(ReferenceKey, this);

            OnSelfAttached();

            foreach (var behavior in BehaviorCollection)
            {
                behavior.Attach(Host);
            }
        }

        protected sealed override void OnDetaching()
        {
            Host.ClearValue(ReferenceKey);

            OnSelfDetaching();

            foreach (var behavior in BehaviorCollection)
            {
                behavior.Detach();
            }
        }

        protected virtual void OnSelfAttached()
        {
        }

        protected virtual void OnSelfDetaching()
        {
        }

        private sealed class DefaultCompositeBehavior : CompositeBehavior<T>
        {
        }
    }
}
