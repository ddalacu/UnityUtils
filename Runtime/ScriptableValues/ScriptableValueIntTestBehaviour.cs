using Framework.Utility;
using UnityEngine;
using UnityEngine.Events;

public class ScriptableValueIntTestBehaviour : MonoBehaviour
{
    [SerializeField]
    private ScriptableIntValue _value;

    [SerializeField]
    private UnityEvent _equalsEvent;

    [SerializeField]
    private UnityEvent _smallerEvent;

    [SerializeField]
    private UnityEvent _biggerEvent;


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