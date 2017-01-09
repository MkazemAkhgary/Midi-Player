﻿using System;
using System.Collections.Generic;
using System.Reflection;
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
                            null, Type.EmptyTypes, null);

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
