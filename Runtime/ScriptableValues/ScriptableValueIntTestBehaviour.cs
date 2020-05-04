using Framework.Utility;
using UnityEngine;
using UnityEngine.Events;

public class ScriptableValueIntTestBehaviour : MonoBehaviour
{
    [SerializeField]
    private ScriptableIntValue _value = default;

    [SerializeField]
    private UnityEvent _equalsEvent = default;

    [SerializeField]
    private UnityEvent _smallerEvent = default;

    [SerializeField]
    private UnityEvent _biggerEvent = default;


    public void Test(int value)
    {
        if (_value.Value == value)
        {
            _equalsEvent.Invoke();
        }
        else
        if (_value.Value < value)
        {
            _smallerEvent.Invoke();
        }
        else
        if (_value.Value > value)
        {
            _biggerEvent.Invoke();
        }
    }
}