using System;
using Framework.Utility;
using UnityEngine;

namespace GraphVariables
{
    [Serializable, CreateAssetMenu(menuName = "ScriptableValue/GameObject", fileName = "GameObjectValue")]
    public class ScriptableGameObjectValue : ScriptableValue<GameObject>
    {
    }
}
