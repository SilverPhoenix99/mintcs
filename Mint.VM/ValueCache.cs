using System;

namespace Mint
{
    public class ValueCache
    {
        private Func<object> value;
        private Func<object> update;


        public ValueCache(Func<object> update)
        {
            Update = update;
            Invalidate();
        }


        public ValueCache(object initialValue, Func<object> update)
            : this(update)
        {
            Value = initialValue;
        }


        public object Value
        {
            get => value();
            set => this.value = () => value;
        }


        public Func<object> Update
        {
            get => update;
            set => update = value ?? throw new ArgumentNullException(nameof(Update));
        }


        public void Invalidate()
        {
            value = () => Value = Update();
        }
    }
}
