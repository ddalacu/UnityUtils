using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;

/// <summary>
/// Used to iterate trough a game object's components without allocating memory(it uses a internal pool)
/// </summary>
[StructLayout(LayoutKind.Auto)]
public struct GetComponentQuery<T> where T : class
{
    private readonly GameObject _gameObject;
    private readonly bool _recursive;
    private readonly bool _includeInactive;
    private readonly bool _reverse;

    public GetComponentQuery(GameObject gameObject, bool recursive, bool includeInactive, bool reverse)
    {
        _gameObject = gameObject;
        _recursive = recursive;
        _includeInactive = includeInactive;
        _reverse = reverse;
    }

    public Enumerator GetEnumerator()
    {
        return new Enumerator(_gameObject, _recursive, _includeInactive, _reverse);
    }

    [StructLayout(LayoutKind.Auto)]
    public struct Enumerator : IDisposable
    {
        public static readonly Type Type = typeof(T);
        private List<object> _internalList;
        private int _index;

        public Enumerator(GameObject gameObject, bool recursive, bool includeInactive, bool reverse)
        {
            if (gameObject == null)
                throw new ArgumentNullException(nameof(gameObject));

            _internalList = ObjectListPool.Rent();
            GetComponentUtilities.GetComponents(gameObject, Type, false, recursive, includeInactive, reverse, _internalList);
            _index = -1;
        }

        public T Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [Il2CppSetOption(Option.NullChecks, false), Il2CppSetOption(Option.ArrayBoundsChecks, false)]
            get => Unsafe.As<T>(_internalList[_index]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Il2CppSetOption(Option.NullChecks, false), Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        public bool MoveNext()
        {
            _index++;
            return _internalList.Count > _index;
        }

        [Il2CppSetOption(Option.NullChecks, false), Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        public void Dispose()
        {
            if (_internalList == null)
                return;
            ObjectListPool.Return(_internalList);
            _internalList = null;
        }
    }
}