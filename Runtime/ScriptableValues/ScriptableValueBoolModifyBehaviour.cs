using Framework.Utility;
using UnityEngine;

public class ScriptableValueBoolModifyBehaviour : MonoBehaviour
{
    [SerializeField]
    private ScriptableBoolValue _value = default;

    public void SetValue(bool value)
    {
        _value.Value = value;
    }
}