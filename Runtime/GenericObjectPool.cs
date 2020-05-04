using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

public class GenericObjectPool<T> where T : UnityEngine.Object
{
    protected List<Tuple<T, bool>> PoolItems = new List<Tuple<T, bool>>();

    public event Action<T> DisableObject;

    public event Action<T> EnableObject;

    public IEnumerable<T> ActiveElements => PoolItems.Where(tuple => tuple.Item2 && tuple.Item1 != null).Select(tuple => tuple.Item1);

    public bool AnyActive => PoolItems.Any(tuple => tuple.Item2 && tuple.Item1 != null);

    public int GetActiveElementsCount()
    {
        int value = 0;

        var poolItemsCount = PoolItems.Count;
        for (var index = 0; index < poolItemsCount; index++)
        {
            var poolItem = PoolItems[index];
            if (poolItem.Item2 && poolItem.Item1 != null)
                value++;
        }

        return value;
    }

    private void CleanNulls()
    {
        int poolItemsCount = PoolItems.Count;

        for (int i = poolItemsCount - 1; i >= 0; i--)
            if (PoolItems[i].Item1 == null)
                PoolItems.RemoveAt(i);
    }

    // Yeah stupid name but I had to name it like this since it does 2 things... at least it will stop confusing me and others
    public T GetOrCreateObject([NotNull] Func<T> createInstance, Func<T, bool> isOk = null)
    {
        if (createInstance == null)
            throw new ArgumentNullException(nameof(createInstance));

        CleanNulls();

        int poolItemsCount = PoolItems.Count;
        for (var index = 0; index < poolItemsCount; index++)
        {
            var poolItem = PoolItems[index];
            if (poolItem.Item2 == false)
            {
                if (isOk != null && isOk(poolItem.Item1) == false)
                    continue;

                PoolItems[index] = new Tuple<T, bool>(poolItem.Item1, true);
                EnableObject?.Invoke(poolItem.Item1);
                return poolItem.Item1;
            }
        }

        var newItem = createInstance();
        PoolItems.Add(new Tuple<T, bool>(newItem, true));
        EnableObject?.Invoke(newItem);
        return newItem;
    }

    public bool ReleaseObject([NotNull] T obj)
    {
        if (obj == null)
            throw new ArgumentNullException(nameof(obj));

        CleanNulls();

        int poolItemsCount = PoolItems.Count;
        for (var index = 0; index < poolItemsCount; index++)
        {
            T item1 = PoolItems[index].Item1;
            if (EqualityComparer<T>.Default.Equals(obj, item1))
            {
                DisableObject?.Invoke(item1);
                PoolItems[index] = new Tuple<T, bool>(item1, false);
                return true;
            }
        }

        return false;
    }

    public void ReleaseAll()
    {
        CleanNulls();

        int poolItemsCount = PoolItems.Count;
        for (var index = 0; index < poolItemsCount; index++)
        {
            PoolItems[index] = new Tuple<T, bool>(PoolItems[index].Item1, false);
            DisableObject?.Invoke(PoolItems[index].Item1);
        }
    }

}
