using System;
using UnityEngine;

namespace Framework.Utility
{
    [Serializable, CreateAssetMenu(menuName = "ScriptableValue/TimeSpan", fileName = "TimeSpan")]
    public class ScriptableSpanValue : ScriptableValue<TimeSpan>
    {
    }
}