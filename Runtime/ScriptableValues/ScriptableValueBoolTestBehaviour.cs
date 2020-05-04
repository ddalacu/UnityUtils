using System;
using Framework.Utility;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using MonoBehaviour = UnityEngine.MonoBehaviour;
using ScriptableBoolValue = Framework.Utility.ScriptableBoolValue;
using UnityEvent = UnityEngine.Events.UnityEvent;

public class ScriptableValueBoolTestBehaviour : MonoBehaviour
{
    [SerializeField]
    private ScriptableBoolValue _value = default;

    [SerializeField]
    private UnityEvent _equalsEvent = default;

    [SerializeField]
    private UnityEvent _notEqualsEvent = default;

    [FormerlySerializedAs("testAtAwake")]
    [SerializeField]
    private bool _testAtAwake = default;

    [FormerlySerializedAs("awakeValue")]
    [SerializeField]
    private bool _awakeValue = default;

    private void Awake()
    {
        if (_testAtAwake) Test(_awakeValue);
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