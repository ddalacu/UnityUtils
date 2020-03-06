using System;
using Random = UnityEngine.Random;

public static class ArrayExtensions
{

    public static T[] RemoveAt<T>(this T[] arr, int index)
    {
        int newSize = arr.Length - 1;
        for (int a = index; a < newSize; a++)
        {
            arr[a] = arr[a + 1];
        }

        T[] copy = new T[newSize];
        for (int i = 0; i < newSize; i++)
        {
            copy[i] = arr[i];
        }
        return copy;
    }

    public static T[] AddRange<T>(T[] arr, T[] toAdd)
    {
        int length = arr.Length;
        int addLength = toAdd.Length;
        int newLength = length + addLength;

        T[] copy = new T[newLength];
        for (int i = 0; i < length; i++)
        {
            copy[i] = arr[i];
        }

        for (int i = length; i < newLength; i++)
        {
            copy[i] = toAdd[i - length];
        }

        return copy;
    }

    public static T[] Add<T>(this T[] arr, T toAdd)
    {
        int length = arr.Length;
        int newLength = length + 1;

        T[] copy = new T[newLength];
        for (int i = 0; i < length; i++)
        {
            copy[i] = arr[i];
        }
        copy[length] = toAdd;
        return copy;
    }

    public static T[] Insert<T>(T[] arr, T toAdd, int index)
    {
        int length = arr.Length;
        int newLength = length + 1;
        int copyFromIndex = 0;

        bool added = false;
        T[] copy = new T[newLength];

        for (int i = 0; i < length; i++)
        {
            if (i == index)
            {
                copy[i] = toAdd;
                copyFromIndex++;
                added = true;
                continue;
            }
            copy[i] = arr[copyFromIndex];
            copyFromIndex++;
        }

        if (!added) copy[length] = toAdd;
        return copy;
    }

    public static T GetRandomElement<T>(this T[] arr, out int randomIndex, int indexToIgnore = -1)
    {
        var arrLength = arr.Length;
        randomIndex = Random.Range(0, arrLength);

        if (randomIndex == indexToIgnore)
        {
            randomIndex++;
            if (randomIndex == arrLength)
                randomIndex = 0;
        }

        return arr[randomIndex];
    }

}
