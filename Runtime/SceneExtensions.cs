using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine.SceneManagement;

public static class SceneExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RootsIterator IterateRoots(this Scene scene)
    {
        return new RootsIterator(scene);
    }

    /// <summary>
    /// Calls onElement for each component of type <see cref="T"/> in the scene
    /// Wont generate garbage but take care if passing lambda methods as that will generate garbage
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="scene"></param>
    /// <param name="onElement"></param>
    /// <param name="includeInactive"></param>
    public static void IterateComponents<T>(this Scene scene, [NotNull] Action<T> onElement, bool includeInactive = true) where T : class
    {
        if (onElement == null)
            throw new ArgumentNullException(nameof(onElement));

        foreach (var root in new RootsIterator(scene))
        {
            foreach (var component in root.ComponentsInChildrenQuery<T>(includeInactive))
            {
                onElement(component);
            }
        }
    }


    /// <summary>
    /// Finds a component of type <see cref="T"/> in the scene
    /// Will not generate garbage but take care if passing lambdas
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="scene"></param>
    /// <param name="filter">return true if this is the passed component is the one you want</param>
    /// <param name="found">the element where the filter method returned true</param>
    /// <param name="includeInactive"></param>
    /// <returns>true if filter method returned true</returns>
    public static bool FindComponent<T>(this Scene scene, [NotNull] Func<T, bool> filter, out T found, bool includeInactive = true) where T : class
    {
        if (filter == null)
            throw new ArgumentNullException(nameof(filter));

        foreach (var root in new RootsIterator(scene))
        {
            foreach (var component in root.ComponentsInChildrenQuery<T>(includeInactive))
            {
                if (filter(component) == false)
                    continue;

                found = component;
                return true;
            }
        }

        found = default;
        return false;
    }
}