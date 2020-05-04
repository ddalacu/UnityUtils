using Framework.Utility;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class ScriptableValueBoolsTestBehaviour : MonoBehaviour
{
    [UnityEngine.SerializeField]
    private ScriptableBoolValue[] _values = default;

    [UnityEngine.SerializeField]
    private UnityEvent _allEqualsEvent = default;

    [UnityEngine.SerializeField]
    private UnityEvent _notEqualsEvent = default;

    [FormerlySerializedAs("testAtAwake")] [UnityEngine.SerializeField]
    private bool _testAtAwake = default;

    [FormerlySerializedAs("awakeValue")] [UnityEngine.SerializeField]
    private bool _awakeValue = default;

    private void Awake()
    {
        if (_testAtAwake) Test(_awakeValue);
    }

    public void Test(bool value)
    {
        foreach (var scriptableBoolValue in _values)
        {
            if (scriptableBoolValue.Value != value)
            {
                _notEqualsEvent.Invoke();
                return;
            }
        }

        _allEqualsEvent.Invoke();
    }
}