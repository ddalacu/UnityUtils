using System;
using System.Collections.Generic;
using UnityEngine;

public class HandleList
{
    private readonly List<RefCounter.Handle> _handles;

    public HandleList(int capacity = 0)
    {
        _handles = new List<RefCounter.Handle>(capacity);
    }

    public void Add(RefCounter.Handle handle)
    {
        _handles.Add(handle);
    }

    /// <summary>
    /// Calls <see cref="RefCounter.Handle.Dispose"/> on each added handle and then clear the internal list
    /// </summary>
    public void DisposeHandles()
    {
        var copy = _handles.ToArray();
        _handles.Clear();

        foreach (var handle in copy)
            handle.Dispose();

    }

}

public class HandleList<T>
{
    private readonly List<(RefCounter.Handle, T)> _entries;

    public HandleList(int capacity = 0)
    {
        _entries = new List<(RefCounter.Handle, T)>(capacity);
    }

    public void Add(RefCounter.Handle handle, T value)
    {
        _entries.Add((handle, value));
    }

    public void Dispose(T value, bool removeAll = false)
    {
        var comparer = EqualityComparer<T>.Default;

        if (removeAll)
        {
            var removed = new List<(RefCounter.Handle, T)>();

            for (var i = _entries.Count - 1; i >= 0; i--)
            {
                if (comparer.Equals(_entries[i].Item2, value))
                {
                    removed.Add(_entries[i]);
                    _entries.RemoveAt(i);
                }
            }

            foreach (var entry in removed)
                entry.Item1.Dispose();
        }
        else
        {
            for (var i = _entries.Count - 1; i >= 0; i--)
            {
                if (comparer.Equals(_entries[i].Item2, value))
                {
                    var data = _entries[i].Item1;
                    _entries.RemoveAt(i);
                    data.Dispose();
                }
            }

        }
    }

    /// <summary>
    /// Calls <see cref="RefCounter.Handle.Dispose"/> on each added handle and then clear the internal list
    /// </summary>
    public void DisposeHandles()
    {
        var copy = _entries.ToArray();
        _entries.Clear();

        foreach (var entry in copy)
            entry.Item1.Dispose();

    }

}


public class RefCounter
{
    public class Handle : IDisposable
    {
        private RefCounter _machine;

        public Handle(RefCounter machine)
        {
            _machine = machine;
            _machine._refCount++;
            _machine.CountChanged?.Invoke(_machine, _machine._refCount);
        }

        public void Dispose()
        {
            if (_machine != null)
            {
                _machine._refCount--;
                var count = _machine._refCount;
                var machine = _machine;
                var evt = _machine.CountChanged;
                _machine = null;
                evt?.Invoke(machine, count);
            }
        }
    }

    private uint _refCount;

    public uint RefCount => _refCount;

    public event Action<RefCounter, uint> CountChanged;

    public Handle GetHandle()
    {
        return new Handle(this);
    }

}