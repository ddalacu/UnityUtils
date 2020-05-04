using Framework.Utility;
using UnityEngine;
using UnityEngine.Events;

public class ScriptableIntValueListenChange : MonoBehaviour
{
    [System.Serializable]
    public class MyIntEvent : UnityEvent<int>
    {
    }

    [SerializeField]
    private MyIntEvent _valueChanged = default;

    [SerializeField]
    private ScriptableIntValue _value = default;

    private void Awake()
    {
        _value.ValueChanged += ValueChanged;
        ValueChanged(_value);
    }

    private void OnDestroy()
    {
        _value.ValueChanged -= ValueChanged;
    }

    private void ValueChanged(IObservableValue<int> observable)
    {
        _valueChanged.Invoke(observable.Value);
    }
}