using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;

/// <summary>
/// Used to iterate trough a game object's components without allocating memory(it uses a internal pool)
/// </summary>
/// <typeparam name="T"></typeparam>
[StructLayout(LayoutKind.Auto)]
public partial struct ComponentsIterator<T> : IDisposable where T : class
{
    public static readonly Type Type = typeof(T);

    private List<object> _internalList;

    public T this[int i]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Il2CppSetOption(Option.NullChecks, false), Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        get => Unsafe.As<T>(_internalList[i]);
    }

    [Il2CppSetOption(Option.NullChecks, false), Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    public void SearchOnGameObject([NotNull] GameObject gameObject, bool includeInactive)
    {
        if (gameObject == null)
            throw new ArgumentNullException(nameof(gameObject));
        if (_internalList == null)
            _internalList = ObjectListPool.Rent();
        GetComponentUtilities.GetComponents(gameObject, Type, false, false, includeInactive, false, _internalList);
    }

    [Il2CppSetOption(Option.NullChecks, false), Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    public void SearchInGameObjectChildren([NotNull] GameObject gameObject, bool includeInactive)
    {
        if (gameObject == null)
            throw new ArgumentNullException(nameof(gameObject));
        if (_internalList == null)
            _internalList = ObjectListPool.Rent();
        GetComponentUtilities.GetComponents(gameObject, Type, false, true, includeInactive, false, _internalList);
    }

    [Il2CppSetOption(Option.NullChecks, false), Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    public void SearchInGameObjectParent([NotNull] GameObject gameObject, bool includeInactive)
    {
        if (gameObject == null)
            throw new ArgumentNullException(nameof(gameObject));
        if (_internalList == null)
            _internalList = ObjectListPool.Rent();
        GetComponentUtilities.GetComponents(gameObject, Type, false, true, includeInactive, true, _internalList);
    }

    [Il2CppSetOption(Option.NullChecks, false), Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    public int GetCount()
    {
        if (_internalList == null)
            return 0;

        return _internalList.Count;
    }

    public void Clear()
    {
        _internalList?.Clear();
    }

    public void Dispose()
    {
        if (_internalList == null)
            return;
        ObjectListPool.Return(_internalList);
        _internalList = null;
    }

    public Inserter GetInserter()
    {
        if (_internalList == null)
            _internalList = ObjectListPool.Rent();

        return new Inserter(_internalList);
    }

}


