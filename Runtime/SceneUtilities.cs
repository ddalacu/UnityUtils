using System;
using UnityEngine;
using System.Runtime.CompilerServices;
using UnityEngine.SceneManagement;


#if UNITY_EDITOR || !ENABLE_IL2CPP
using System.Reflection;

public static class SceneUtilities
{

    private delegate void GetRootGameObjectsInternalDelegate(int sceneHandle, object resultRootList);

    private static readonly GetRootGameObjectsInternalDelegate GetRootGameObjectsInternal;

    static SceneUtilities()
    {
        var methodInfo = typeof(Scene).GetMethod("GetRootGameObjectsInternal", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

        if (methodInfo == null)
        {
            Debug.LogError($"Could not find method GetRootGameObjectsInternal on type {typeof(Scene)} with form {typeof(GetRootGameObjectsInternalDelegate)}");
            return;
        }

        GetRootGameObjectsInternal = (GetRootGameObjectsInternalDelegate)Delegate.CreateDelegate(typeof(GetRootGameObjectsInternalDelegate), methodInfo);
    }

    /// <summary>
    /// Calls GetRootGameObjectsInternal on scene
    /// </summary>
    /// <param name="sceneHandle"></param>
    /// <param name="resultRootList"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void GetRootGameObjects(int sceneHandle, object resultRootList)
    { 
        GetRootGameObjectsInternal(sceneHandle, resultRootList);
    }
}

#else
using Unity.IL2CPP.CompilerServices;

public static class SceneUtilities
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Il2CppSetOption(Option.NullChecks, false)]
    [StaticMethodProxy(typeof(Scene), "GetRootGameObjectsInternal")]
    public static void GetRootGameObjects(int sceneHandle, object resultRootList)
    {
        throw new NotImplementedException("GetRootGameObjects should have been patched!");
    }
}
#endif