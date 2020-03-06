using System;
using Framework.Utility;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CanEditMultipleObjects]
[CustomEditor(typeof(ScriptableValueBase), true)]
public class ScriptableValueEditor : Editor
{
    private static Type GetFirstImplementer(Type target, Type implements)
    {
        var targetBaseType = target.BaseType;

        if (targetBaseType == implements)
            return target;

        if (targetBaseType != null)
            return GetFirstImplementer(targetBaseType, implements);
        return null;
    }

    public override VisualElement CreateInspectorGUI()
    {
        var container = new VisualElement();

        var iterator = serializedObject.GetIterator();
        if (iterator.NextVisible(true))
        {
            do
            {
                var propertyField = new PropertyField(iterator.Copy()) { name = "PropertyField:" + iterator.propertyPath };

                if (iterator.propertyPath == "m_Script" && serializedObject.targetObject != null)
                    propertyField.SetEnabled(value: false);

                if (iterator.propertyPath == "_value")
                {
                    var type = base.target.GetType();
                    var scriptableValueType = GetFirstImplementer(type, typeof(ScriptableValueBase));
                    Debug.Assert(scriptableValueType.IsConstructedGenericType);
                    var valueType = scriptableValueType.GetGenericArguments()[0];

                    ChangeEventExtensions.RegisterChangeEventCallback(propertyField, valueType, OnTestOnOnCallback);
                }

                container.Add(propertyField);
            }
            while (iterator.NextVisible(false));
        }

        return container;
    }

    private void OnTestOnOnCallback(IChangeEvent evt)
    {
        foreach (var o in targets)
        {
            var casted = (ScriptableValueBase)o;
            casted.ForceNotifyValueChanged();
        }
    }
}