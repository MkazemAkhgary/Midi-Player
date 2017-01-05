using System;
using System.Windows;

namespace MidiApp.Behaviors
{
    /// <summary>
    /// Provides a dependency property from given target.
    /// </summary>
    /// <typeparam name="TProperty"></typeparam>
    /// <typeparam name="TTarget"></typeparam>
    public sealed class DependencyPropertyProvider<TProperty, TTarget> : DependencyObject
        where TTarget : DependencyObject
    {
        private static readonly DependencyProperty ProvidingProperty =
            DependencyProperty.Register(
                typeof(DependencyPropertyProvider<TProperty, TTarget>).FullName,
                typeof(TProperty),
                typeof(DependencyPropertyProvider<TProperty, TTarget>));

        private readonly Func<TTarget, TProperty> _initializer;
        private readonly TTarget _target;

        public TProperty ProvidedProperty => (TProperty)GetValue(ProvidingProperty);

        private DependencyPropertyProvider(Func<TTarget, TProperty> initializer, TTarget target)
        {
            _initializer = initializer;
            _target = target;
        }
        
        public static DependencyPropertyProvider<TProperty, TTarget> Create(
            TTarget target,
            Func<TTarget, TProperty> initializer)
        {
            return new DependencyPropertyProvider<TProperty, TTarget>(initializer, target);
        }

        public void Initialize(object sender, EventArgs args)
        {
            SetValue(ProvidingProperty, _initializer(_target));
        }
    }
}
