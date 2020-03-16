using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

///<summary>
/// Author Paul Diac
/// Version 0.0a
///</summary>
public static class ListExtensions
{
    private static System.Random rng = new System.Random();

    /// <summary>
    /// Shuffle a list using a internal seed
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    /// <summary>
    /// Shuffle a list using a passed seed
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="seed"></param>
    public static void Shuffle<T>(this IList<T> list, int seed)
    {
        System.Random rng = new System.Random(seed);
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    /// <summary>
    /// Searches trough a sorted list using a comparator (binary search) but returns a list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="element"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static List<T> BinarySearchRange<T>(this List<T> list, T element, IComparer<T> comparer)
    {
        List<T> elements = new List<T>();

        int count = list.Count;
        int index = list.BinarySearch(element, comparer);

        if (index > -1)
        {
            int minIndex = index;
            int maxIndex = index;

            elements.Add(list[index]);

            while (minIndex - 1 >= 0)
            {
                if (comparer.Compare(list[minIndex - 1], element) != 0)
                    break;
                minIndex--;
                elements.Add(list[minIndex]);
            }

            while (maxIndex + 1 < count)
            {
                if (comparer.Compare(list[maxIndex + 1], element) != 0)
                    break;
                maxIndex++;
                elements.Add(list[maxIndex]);
            }
        }

        return elements;
    }

    /// <summary>
    /// Removes a item sorted list using a comparator and a shouldRemove func
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="element"></param>
    /// <param name="comparer"></param>
    /// <param name="shouldRemove">return true to remove the element</param>
    /// <param name="breakAtFirst"></param>
    public static void BinarySearchRemove<T>(this List<T> list, T element, IComparer<T> comparer, Func<T, bool> shouldRemove = null, bool breakAtFirst = false)
    {
        List<int> elements = new List<int>();

        int index = list.BinarySearch(element, comparer);
        int minIndex = index;
        int maxIndex = index;

        if (index > -1)
        {
            int count = list.Count;

            elements.Add(index);
            while (maxIndex + 1 < count)
            {
                if (comparer.Compare(list[maxIndex + 1], element) != 0)
                    break;
                maxIndex++;
                elements.Add(maxIndex);
            }

            while (minIndex - 1 >= 0)
            {
                if (comparer.Compare(list[minIndex - 1], element) != 0)
                    break;
                minIndex--;
                elements.Insert(0, minIndex);
            }

            int length = maxIndex - minIndex;
            for (int i = 0; i <= length; i++)
            {
                if (shouldRemove != null)
                {
                    if (shouldRemove(list[minIndex]))
                    {
                        list.RemoveAt(minIndex);
                        if (breakAtFirst)
                            return;
                    }
                    else
                        minIndex++;
                }
                else
                {
                    list.RemoveAt(minIndex);
                    if (breakAtFirst)
                        return;
                }
            }
        }
    }

    public static (List<T> added, List<T> removed) DiffList<T>(List<T> original, List<T> modified)
    {
        var added = new List<T>();
        var remved = new List<T>();

        var originalCount = original.Count;
        var modifiedCount = modified.Count;

        var comparer = EqualityComparer<T>.Default;

        for (int i = 0; i < modifiedCount; i++)
        {
            var modifiedElement = modified[i];

            var contains = false;

            for (int j = 0; j < originalCount; j++)
            {
                var originalElement = original[j];
                if (comparer.Equals(modifiedElement, originalElement))
                {
                    contains = true;
                    break;
                }
            }

            if (!contains)
            {
                added.Add(modifiedElement);
            }
        }

        for (int j = 0; j < originalCount; j++)
        {
            var originalElement = original[j];
            var contains = false;
            for (int i = 0; i < modifiedCount; i++)
            {
                var modifiedElement = modified[i];

                if (comparer.Equals(modifiedElement, originalElement))
                {
                    contains = true;
                    break;
                }
            }
            if (!contains)
            {
                remved.Add(originalElement);
            }
        }

        return (added, remved);
    }

    public static T GetRandomItem<T>(this IReadOnlyList<T> list)
    {
        var listCount = list.Count;
        if(listCount==0)
            throw new Exception("List size is 0, cant randomize!!");

        return list[Random.Range(0, listCount)];
    }

}
