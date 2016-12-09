using System;

namespace Mint
{
    public class ValueCache
    {
        private Func<object> value;
        private Func<object> update;

        public object Value
        {
            get { return value(); }
            set { this.value = () => value; }
        }

        public Func<object> Update
        {
            get { return update; }
            set
            {
                if(value == null) throw new ArgumentNullException(nameof(Update));
                update = value;
            }
        }
        
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

        public void Invalidate()
        {
            value = () => Value = Update();
        }
    }
}
