using System.Runtime.CompilerServices;
using UnityEngine;

public static class GameObjectGetComponentsExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static GetComponentQuery<T> ComponentsInChildrenQuery<T>(this GameObject gameObject, bool includeInactive) where T : class
    {
        return new GetComponentQuery<T>(gameObject, true, includeInactive, false);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static GetComponentQuery<T> ComponentsInParentQuery<T>(this GameObject gameObject, bool includeInactive) where T : class
    {
        return new GetComponentQuery<T>(gameObject, true, includeInactive, true);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static GetComponentQuery<T> GetComponentsQuery<T>(this GameObject gameObject, bool includeInactive) where T : class
    {
        return new GetComponentQuery<T>(gameObject, false, includeInactive, false);
    }

    public static void SetComponentsInChildrenEnabledState<T>(this GameObject gameObject, bool enabled, bool includeInactive = true) where T : Behaviour
    {
        var internalList = ObjectListPool.Rent();
        GetComponentUtilities.GetComponents(gameObject, typeof(T), false, true, includeInactive, false, internalList);

        var count = internalList.Count;
        for (int i = 0; i < count; i++)
        {
            var behaviour = Unsafe.As<T>(internalList[i]);
            behaviour.enabled = enabled;
        }

        ObjectListPool.Return(internalList);
    }
}