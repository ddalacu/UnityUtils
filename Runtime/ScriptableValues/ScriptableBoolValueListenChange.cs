using Framework.Utility;
using UnityEngine;
using UnityEngine.Events;

public class ScriptableBoolValueListenChange : MonoBehaviour
{
    [System.Serializable]
    public class MyBoolEvent : UnityEvent<bool>
    {
    }

    [SerializeField]
    private MyBoolEvent _valueChanged;

    [SerializeField]
    private ScriptableBoolValue _value;

    private void Awake()
    {
        _value.ValueChanged += ValueChanged;
        ValueChanged(_value);
    }

    private void OnDestroy()
    {
        _value.ValueChanged -= ValueChanged;
    }

    private void ValueChanged(IObservableValue<bool> observable)
    {
        _valueChanged.Invoke(observable.Value);
    }
}