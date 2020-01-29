using System.Collections.Generic;

namespace Framework.Utility
{
    public class ObservableValue<TValue> : IObservableValue<TValue>
    {
        private TValue _value;

        public event ObservableValueChangedDelegate<TValue> ValueChanged;

        public TValue Value
        {
            get => _value;
            set
            {
                bool changed = ValueChanged != null && EqualityComparer<TValue>.Default.Equals(_value, value) == false;

                _value = value;

                if (changed)
                {
                    ValueChanged(this);
                }
            }
        }

        public ObservableValue(TValue defaulTValue)
        {
            _value = defaulTValue;
        }

        public ObservableValue()
        {
            
        }
    }
}