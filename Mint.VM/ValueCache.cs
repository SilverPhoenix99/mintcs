using System;

namespace Mint
{
    public class ValueCache
    {
        private Func<object> value;

        public object Value
        {
            get { return value(); }
            set { this.value = () => value; }
        }

        public Func<object> Update { get; set; }

        public Action Destruct { get; set; }

        public ValueCache(Func<object> update, Action destruct = null)
        {
            if(update == null) throw new ArgumentNullException(nameof(update));

            Update = update;
            Destruct = destruct;
            Invalidate();
        }

        public ValueCache(object initialValue, Func<object> update, Action destruct = null)
            : this(update, destruct)
        {
            Value = initialValue;
        }

        ~ValueCache()
        {
            if(Destruct != null)
            {
                Destruct();
            }
        }

        public void Invalidate()
        {
            value = () => Value = Update();
        }
    }
}
