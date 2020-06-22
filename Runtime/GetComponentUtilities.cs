using System;
using UnityEngine;
using System.Runtime.CompilerServices;


#if UNITY_EDITOR || !ENABLE_IL2CPP
using System.Reflection;

public static class GetComponentUtilities
{

    private delegate Array GetComponentsInternalDelegate(GameObject target, Type type, bool useSearchTypeAsArrayReturnType, bool recursive, bool includeInactive, bool reverse, object resultList);

    private static readonly GetComponentsInternalDelegate GetComponentsInternal;

    public static bool CreateDelegate<T>(this Type type, string name, BindingFlags flags, out T var) where T : Delegate
    {
        var methodInfo = type.GetMethod(name, flags);

        if (methodInfo == null)
        {
            Debug.LogError($"Could not find method {name} on type {type} with form {typeof(T)}");
            var = null;
            return false;
        }

        var = (T)Delegate.CreateDelegate(typeof(T), methodInfo);
        return true;
    }

    public static bool InstanceMethodCallCreateDelegate<T>(string name, out T var) where T : Delegate
    {
        var parameters = typeof(T).GetMethod("Invoke")?.GetParameters();

        if (parameters == null || parameters.Length == 0)
        {
            var = null;
            return false;
        }

        return CreateDelegate(parameters[0].ParameterType, name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, out var);
    }

    static GetComponentUtilities()
    {
        InstanceMethodCallCreateDelegate("GetComponentsInternal", out GetComponentsInternal);
    }

    /// <summary>
    /// Calls the unity internal method using a delegate created by reflection
    /// </summary>
    /// <param name="instance">the instance on which we call the method</param>
    /// <param name="type">the type of object we are searching</param>
    /// <param name="useSearchTypeAsArrayReturnType">if set to false will return a Component[] else will return Type[]</param>
    /// <param name="recursive">if set to false will only search on same go</param>
    /// <param name="includeInactive">if to include inactive behaviours</param>
    /// <param name="reverse">if the search should go up in hierarchy</param>
    /// <param name="resultList">use this to store results instead of returning a array</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Array GetComponents(GameObject instance, Type type, bool useSearchTypeAsArrayReturnType, bool recursive, bool includeInactive, bool reverse, object resultList)
    {
        return GetComponentsInternal(instance, type, useSearchTypeAsArrayReturnType, recursive, includeInactive, reverse, resultList);
    }
}

#else
using Unity.IL2CPP.CompilerServices;

public static class GetComponentUtilities
{
    [InstanceMethodProxy("GetComponentsInternal")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Il2CppSetOption(Option.NullChecks, false)]
    public static Array GetComponents(GameObject instance, Type type, bool useSearchTypeAsArrayReturnType, bool recursive, bool includeInactive, bool reverse, object resultList)
    {
        throw new Exception("GetComponents should of been patched!");
    }
}
#endif