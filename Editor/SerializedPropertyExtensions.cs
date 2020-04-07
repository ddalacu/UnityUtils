using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class SerializedPropertyExtensions
{

    public static T CreateDelegateFromParentMethod<T>(this SerializedProperty property, string methodName, int targetObjectIndex = 0, BindingFlags flags = BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) where T : class
    {

        object parentObject = GetTargetObjectOfParentProperty(property, targetObjectIndex);

        MethodInfo methodInfo = parentObject.GetType().GetMethod(methodName, flags);

        T method = null;

        if (methodInfo != null)
        {
            try
            {
                if (methodInfo.IsStatic)
                {
                    method = Delegate.CreateDelegate(typeof(T), methodInfo) as T;
                }
                else
                {
                    method = Delegate.CreateDelegate(typeof(T), parentObject, methodInfo) as T;
                }

                return method;
            }
            catch
            {
                return null;
            }
        }

        return null;
    }

    public static object GetTargetObjectOfParentProperty(this SerializedProperty prop, int targetObjectIndex = 0)
    {
        string path = prop.propertyPath.Replace(".Array.data[", "[");
        string[] results = path.Split('.');
        string end = string.Join(".", results, 0, results.Length - 1);

        if (string.IsNullOrEmpty(end) == false)
        {
            end = end.Replace("[", ".Array.data[");
            return prop.serializedObject.FindProperty(end).GetTargetObjectOfProperty(targetObjectIndex);
        }
        else
        {
            return prop.serializedObject.targetObjects[targetObjectIndex];
        }
    }

    public static object GetTargetObjectOfProperty(this SerializedProperty prop, int targetObjectIndex = 0)
    {
        var path = prop.propertyPath.Replace(".Array.data[", "[");
        object obj = prop.serializedObject.targetObjects[targetObjectIndex];
        var elements = path.Split('.');
        foreach (var element in elements)
        {
            if (element.Contains("["))
            {
                var elementName = element.Substring(0, element.IndexOf("["));
                var index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                obj = GetValue_Imp(obj, elementName, index);
            }
            else
            {
                obj = GetValue_Imp(obj, element);
            }
        }
        return obj;
    }

    public static T[] GetTargetObjectsOfProperty<T>(this SerializedProperty prop, int targetObjectIndex = 0)
    {
        int length = prop.serializedObject.targetObjects.Length;

        T[] objects = new T[length];
        for (int i = 0; i < length; i++)
        {
            object result = prop.GetTargetObjectOfProperty(i);
            if (result != null)
            {
                objects[i] = (T)result;
            }
        }

        return objects;
    }


    private static object GetValue_Imp(object source, string name)
    {
        if (source == null)
            return null;
        var type = source.GetType();

        while (type != null)
        {
            var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (f != null)
                return f.GetValue(source);

            var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (p != null)
                return p.GetValue(source, null);

            type = type.BaseType;
        }
        return null;
    }

    private static object GetValue_Imp(object source, string name, int index)
    {
        var enumerable = GetValue_Imp(source, name) as System.Collections.IEnumerable;
        if (enumerable == null) return null;
        var enm = enumerable.GetEnumerator();
        for (int i = 0; i <= index; i++)
        {
            if (!enm.MoveNext()) return null;
        }
        return enm.Current;
    }

}
