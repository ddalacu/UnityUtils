using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using UnityEngine;

public partial struct ComponentsIterator<T>
{
    /// <summary>
    /// Used to add more components to a <seealso cref="ComponentsIterator{T}"/>
    /// </summary>
    [StructLayout(LayoutKind.Auto)]
    public struct Inserter : IDisposable
    {
        private List<object> _buffer;
        private List<object> _iterator;

        internal Inserter(List<object> iterator)
        {
            _buffer = ObjectListPool.Rent();
            _iterator = iterator;
        }

        public void Insert([NotNull] GameObject gameObject, bool recursive, bool includeInactive, bool reverse)
        {
            if (gameObject == null)
                throw new ArgumentNullException(nameof(gameObject));

            GetComponentUtilities.GetComponents(gameObject, Type, false, recursive, includeInactive, reverse,
                _buffer);


            var bufferCount = _buffer.Count;
            var requiredCapacity = _iterator.Count + bufferCount;

            if (_iterator.Capacity < requiredCapacity)
                _iterator.Capacity = requiredCapacity;

            for (int i = 0; i < bufferCount; i++)
            {
                var item = _buffer[i];
                if (item != null)
                    _iterator.Add(item);
            }
        }

        public void Insert([NotNull] Component component, bool recursive, bool includeInactive, bool reverse)
        {
            if (component == null)
                throw new ArgumentNullException(nameof(component));
            Insert(component.gameObject, recursive, includeInactive, reverse);
        }

        public void Dispose()
        {
            if (_buffer != null)
            {
                ObjectListPool.Return(_buffer);
                _buffer = null;
            }
        }
    }
}