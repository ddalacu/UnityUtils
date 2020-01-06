using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Used to iterate trough a scene's roots without allocating memory(it uses a internal pool)
/// </summary>
[StructLayout(LayoutKind.Auto)]
public struct RootsIterator
{
    private readonly Scene _scene;

    public RootsIterator(Scene scene)
    {
        _scene = scene;
    }

    public Enumerator GetEnumerator()
    {
        return new Enumerator(_scene);
    }

    [StructLayout(LayoutKind.Auto)]
    public struct Enumerator : IDisposable
    {
        private List<object> _internalList;
        private int _index;

        public Enumerator(Scene scene)
        {
            if (scene.IsValid() == false)
                throw new ArgumentException($"{nameof(scene)} is not valid!");

            if (Application.isPlaying == false && scene.isLoaded == false)
                throw new ArgumentException("The scene is not loaded.");

            _index = -1;
            _internalList = ObjectListPool.Rent();

            if (scene.rootCount == 0)
                return;

            if (_internalList.Capacity < scene.rootCount)
                _internalList.Capacity = scene.rootCount;

            SceneUtilities.GetRootGameObjects(scene.handle, _internalList);
        }

        public GameObject Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [Il2CppSetOption(Option.NullChecks, false), Il2CppSetOption(Option.ArrayBoundsChecks, false)]
            get => Unsafe.As<GameObject>(_internalList[_index]);
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