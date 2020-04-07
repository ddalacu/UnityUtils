using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.Utility
{
    [CustomPropertyDrawer(typeof(ObjectPath))]
    public class ObjectPathDrawer : PropertyDrawer
    {
        private delegate bool FilterDelegate(Object obj);

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            object[] attributes = fieldInfo?.GetCustomAttributes(typeof(ObjectPathTypeAttribute), false);

            if (attributes == null || attributes.Length == 0)
            {
                EditorGUI.LabelField(position, "Add ObjectPathType on " + label.text);
            }
            else
            {
                ObjectPathTypeAttribute objectPathTypeAttribute = attributes[0] as ObjectPathTypeAttribute;
                if (objectPathTypeAttribute == null)
                {
                    throw new ArgumentNullException("objectPathType");
                }

                SerializedProperty objectProperty = property.FindPropertyRelative("_value");
                Object o = objectProperty.objectReferenceValue;

                EditorGUI.BeginChangeCheck();
                EditorGUI.ObjectField(position, objectProperty, objectPathTypeAttribute.Type, label);

                if (EditorGUI.EndChangeCheck() && string.IsNullOrEmpty(objectPathTypeAttribute.FilterMethod) == false)
                {
                    FilterDelegate method = property.CreateDelegateFromParentMethod<FilterDelegate>(objectPathTypeAttribute.FilterMethod);
                    if (method != null && method(objectProperty.objectReferenceValue) == false)
                    {
                        objectProperty.objectReferenceValue = o;
                    }
                }

            }
        }
    }
}