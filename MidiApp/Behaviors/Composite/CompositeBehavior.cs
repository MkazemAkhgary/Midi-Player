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
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.NotDataBindable));

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

        protected static TOut GetValueByRef<TOut>(T host, DependencyProperty dp)
        {
            return (TOut)GetReference(host).GetValue(dp);
        }

        protected static void SetValueByRef<TIn>(T host, DependencyProperty dp, TIn value)
        {
            GetReference(host).SetValue(dp, value);
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
                    const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;

                    var constructor = typeof(BehaviorCollection)
                        .GetConstructor(flags, null, CallingConventions.HasThis, Type.EmptyTypes, null);

                    collection = (BehaviorCollection) constructor.Invoke(null);
                    SetValue(BehaviorCollectionProperty, collection);
                }

                return collection;
            }
        }

        private void VerifyDistinct()
        {
            var hashset = new HashSet<Type>();
            foreach (var behavior in BehaviorCollection)
            {
                if (behavior is Behavior<T> == false)
                {
                    throw new InvalidOperationException($"expected element of type {typeof(Behavior<T>).Name} , actual type is {behavior.GetType().Name}.");
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
            VerifyDistinct();

            Host.SetValue(ReferenceKey, this);
            OnSelfAttached();
            BehaviorCollection.Attach(AssociatedObject);
        }

        protected sealed override void OnDetaching()
        {
            BehaviorCollection.Detach();
            OnSelfDetaching();
            Host.ClearValue(ReferenceKey);
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
