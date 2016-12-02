using System;

namespace Mint
{
    public class ValueCache<T>
    {
        private T value;

        public Condition Condition { get; private set; }

        public Func<T> Update { get; }

        public T Value
        {
            get
            {
                return Condition.Valid ? value : (Value = Update());
            }
            set
            {
                this.value = value;
                Condition = new Condition();
            }
        }

        public ValueCache(T initialValue, Func<T> update)
        {
            Value = initialValue;
            Update = update;
        }

        public ValueCache(Func<T> update) : this(default(T), update)
        {
            Condition.Invalidate();
        }
    }
}
