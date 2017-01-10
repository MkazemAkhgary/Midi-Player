using System;

namespace Utilities.Threading
{
    public sealed class BlockingContainer<T> : IDisposable where T : class
    {
        private readonly Lazy<SafeTimer> _timer;
        private volatile bool _isFree = true;
        private readonly T _item;
        private readonly Action<T> _callback;

        private SafeTimer Timer => _timer.Value;
        private bool IsTimerInitialized => _timer.IsValueCreated;

        public bool IsFree
        {
            get { return _isFree; }
            private set { _isFree = value; }
        }

        /// <summary>
        /// blocks access to item for specified amount of time.
        /// if another wait is requested current wait is ignored and runs callback after wait period is elapsed.
        /// </summary>
        /// <param name="item">encapsulating item.</param>
        /// <param name="callback">callback to run after wait period is elapsed.</param>
        public BlockingContainer(T item, Action<T> callback = null)
        {
            _item = item;
            _callback = callback;
            _timer = new Lazy<SafeTimer>(InitializeTimer);
        }

        private SafeTimer InitializeTimer()
        {
            var timer = new SafeTimer
            {
                AutoReset = false
            };

            timer.Elapsed += (o, args) =>
            {
                IsFree = true;
                _callback?.Invoke(_item);
            };

            return timer;
        }


        /// <summary>
        /// returns encapsulated item if container is free.
        /// </summary>
        public T RequestItem()
        {
            return IsFree ? _item : null;
        }

        /// <summary>
        /// reserves the callback after specified amount of time.
        /// </summary>
        public void Reserve(int duration)
        {
            if (IsFree) Block(duration);
        }
        
        private void Block(int duration)
        {
            IsFree = false;
            Timer.Stop();
            Timer.Interval = duration;
            Timer.Start();
        }

        public void Dispose()
        {
            if (IsTimerInitialized) Timer.Dispose();
        }
    }
}
