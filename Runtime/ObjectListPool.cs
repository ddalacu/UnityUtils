using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.IL2CPP.CompilerServices;


[Il2CppSetOption(Option.NullChecks, false), Il2CppSetOption(Option.ArrayBoundsChecks, false)]
public static class ObjectListPool
{
    private static List<object>[] _buffer = new List<object>[32];
    private static int _capacity = 32;
    private static int _count;

    public static void Return([NotNull] List<object> list)
    {
        if (_count == _capacity)
        {
            _capacity *= 2;
            var expanded = new List<object>[_capacity];
            Array.Copy(_buffer, 0, expanded, 0, _count);
            _buffer = expanded;
        }

        list.Clear();
        _buffer[_count] = list;
        _count++;
    }

    public static List<object> Rent()
    {
        if (_count == 0)
            return new List<object>(32);

        _count--;
        var toReturn = _buffer[_count];
        _buffer[_count] = null;
        return toReturn;
    }

    /// <summary>
    /// You might want to call this if you think you allocated to much memory
    /// </summary>
    public static void Clear()
    {
        for (short i = 0; i < _count; i++)
            _buffer[i] = null;

        _count = 0;
    }

}