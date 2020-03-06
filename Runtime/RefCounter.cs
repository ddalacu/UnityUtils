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