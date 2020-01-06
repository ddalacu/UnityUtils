using System.Runtime.CompilerServices;
using UnityEngine;

public static class ComponentGetComponentsExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static GetComponentQuery<T> ComponentsInChildrenQuery<T>(this Component component, bool includeInactive) where T : class
    {
        return new GetComponentQuery<T>(component.gameObject, true, includeInactive, false);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static GetComponentQuery<T> ComponentsInParentQuery<T>(this Component component, bool includeInactive) where T : class
    {
        return new GetComponentQuery<T>(component.gameObject, true, includeInactive, true);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static GetComponentQuery<T> GetComponentsQuery<T>(this Component component, bool includeInactive) where T : class
    {
        return new GetComponentQuery<T>(component.gameObject, false, includeInactive, false);
    }
}