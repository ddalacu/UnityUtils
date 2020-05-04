using Framework.Utility;
using UnityEngine;

public class ScriptableValueIntModifyBehaviour : MonoBehaviour
{
    [SerializeField]
    private ScriptableIntValue _value = default;

    public void AddValue(int value)
    {
        _value.Value += value;
    }
}
