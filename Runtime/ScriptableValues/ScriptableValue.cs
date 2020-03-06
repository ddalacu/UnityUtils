using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Framework.Utility
{
    public abstract class ScriptableValueBase : ScriptableObject
    {
        [Multiline]
        public string Description;//the value description, for what is this value used

        public abstract void ForceNotifyValueChanged();
    }


    /// <summary>
    /// Used to share values using scriptable objects
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ScriptableValue<T> : ScriptableValueBase, IObservableValue<T>, IEquatable<ScriptableValue<T>>, ISerializationCallbackReceiver
    {
        [SerializeField, FormerlySerializedAs("Value")]
        private T _value;//the serialized value

        [SerializeField]
        private bool _setToDefaultWhenDeserialized;//the serialized value

        [SerializeField]
        protected T _defaultValue;//the serialized value

        public T DefaultValue => _defaultValue;

        public event ObservableValueChangedDelegate<T> ValueChanged;

        public T Value
        {
            get => _value;
            set
            {
                var changed = ValueChanged != null && EqualityComparer<T>.Default.Equals(_value, value) == false;

                _value = value;

                if (changed)
                {
                    ValueChanged(this);
                }
            }
        }

        public override void ForceNotifyValueChanged()
        {
            ValueChanged?.Invoke(this);
        }

        public bool Equals(ScriptableValue<T> other)
        {
            if (other == null)
                return false;

            return Value.Equals(other.Value);
        }

        public override bool Equals(object other)
        {
            if (ReferenceEquals(other, null))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (other.GetType() != GetType())
                return false;

            var othherI = other as ScriptableValue<T>;

            if (Value == null)
            {
                if (othherI.Value == null)
                    return true;
                return false;
            }
            else
            {
                if (othherI.Value == null)
                    return false;

                return Value.Equals(other);

            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(ScriptableValue<T> a, ScriptableValue<T> b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
            {
                return false;
            }

            if (a.Value == null)
            {
                if (b.Value == null)
                    return true;
                return false;
            }

            return a.Value.Equals(b.Value);
        }

        public static bool operator !=(ScriptableValue<T> a, ScriptableValue<T> b)
        {
            return !(a == b);
        }

        public virtual void OnBeforeSerialize()
        {

        }

        public virtual void OnAfterDeserialize()
        {
            if (_setToDefaultWhenDeserialized)
                _value = _defaultValue;
        }
    }
}
