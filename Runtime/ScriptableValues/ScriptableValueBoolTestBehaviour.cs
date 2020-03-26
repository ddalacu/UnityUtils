using System;
using Framework.Utility;
using UnityEngine;
using UnityEngine.Events;
using MonoBehaviour = UnityEngine.MonoBehaviour;
using ScriptableBoolValue = Framework.Utility.ScriptableBoolValue;
using UnityEvent = UnityEngine.Events.UnityEvent;

public class ScriptableValueBoolTestBehaviour : MonoBehaviour
{
    [SerializeField]
    private ScriptableBoolValue _value;

    [SerializeField]
    private UnityEvent _equalsEvent;

    [SerializeField]
    private UnityEvent _notEqualsEvent;

    [SerializeField]
    private bool testAtAwake;

    [SerializeField]
    private bool awakeValue;

    private void Awake()
    {
        if (testAtAwake) Test(awakeValue);
    }

    public void Test(bool value)
    {
        if (_value.Value == value)
        {
            _equalsEvent.Invoke();
        }
        else
        {
            _notEqualsEvent.Invoke();
        }
    }
}