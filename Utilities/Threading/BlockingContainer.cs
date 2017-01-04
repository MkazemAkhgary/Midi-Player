using System;

namespace Utilities.Threading
{
    public sealed class BlockingContainer<T> : IDisposable where T : class
    {
        private readonly Lazy<SafeTimer> _timer;
        private volatile bool _isFree = true;
        private readonly T _item;

        private SafeTimer Timer => _timer.Value;
        private bool IsTimerInitialized => _timer.IsValueCreated;

        public bool IsFree
        {
            get { return _isFree; }
            private set { _isFree = value; }
        }

        public BlockingContainer(T item)
        {
            _item = item;
            _timer = new Lazy<SafeTimer>(InitializeTimer);
        }

        private SafeTimer InitializeTimer()
        {
            var timer = new SafeTimer
            {
                AutoReset = false
            };

            timer.Elapsed += (o, args) => IsFree = true;

            return timer;
        }

        public T GetItem()
        {
            return IsFree ? _item : null;
        }

        public T GetItem(int duration)
        {
            if (IsFree)
            {
                Block(duration);
                return _item;
            }

            return null;
        }

        public void Block(int duration)
        {
            IsFree = false;
            Timer.Stop();
            Timer.Interval = duration;
            Timer.Start();
        }

        public T ForceGetItem()
        {
            return _item;
        }

        public T FreeItem()
        {
            if (IsTimerInitialized)
            {
                Timer.Stop();
                IsFree = true;
            }

            return _item;
        }

        public void Dispose()
        {
            if (IsTimerInitialized) Timer.Dispose();
        }
    }
}
