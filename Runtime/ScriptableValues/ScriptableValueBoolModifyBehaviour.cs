using Framework.Utility;
using UnityEngine;

public class ScriptableValueBoolModifyBehaviour : MonoBehaviour
{
    [SerializeField]
    private ScriptableBoolValue _value;

    public void SetValue(bool value)
    {
        _value.Value = value;
    }
}