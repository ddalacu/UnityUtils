using Framework.Utility;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class ScriptableValueBoolsTestBehaviour : MonoBehaviour
{
    [UnityEngine.SerializeField]
    private ScriptableBoolValue[] _values;

    [UnityEngine.SerializeField]
    private UnityEvent _allEqualsEvent;

    [UnityEngine.SerializeField]
    private UnityEvent _notEqualsEvent;

    [UnityEngine.SerializeField]
    private bool testAtAwake;

    [UnityEngine.SerializeField]
    private bool awakeValue;

    private void Awake()
    {
        if (testAtAwake) Test(awakeValue);
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