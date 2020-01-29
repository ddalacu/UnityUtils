using System.Collections.Generic;
using UnityEngine;

public class LifeCycleEvents : MonoBehaviour
{

    private static readonly List<ILifeCycleEvent> _listeners = new List<ILifeCycleEvent>();

    [RuntimeInitializeOnLoadMethod]
    private static void AutoInitialize()
    {
        GameObject go = new GameObject(nameof(LifeCycleEvents));
        go.hideFlags = HideFlags.NotEditable | HideFlags.DontSave;
        DontDestroyOnLoad(go);
        go.AddComponent<LifeCycleEvents>();
    }

    public static void AddListener(ILifeCycleEvent listener)
    {
        if (_listeners.Contains(listener) == false)
            _listeners.Add(listener);
    }

    public static void RemoveListener(ILifeCycleEvent listener)
    {
        _listeners.Remove(listener);
    }

    private void Update()
    {
        for (int i = _listeners.Count - 1; i >= 0; i--)
        {
            if (_listeners[i] is ILifeCycleUpdate lifeCycleUpdate)
                lifeCycleUpdate.DoUpdate();
        }
    }
    private void LateUpdate()
    {
        for (int i = _listeners.Count - 1; i >= 0; i--)
        {
            if (_listeners[i] is ILifeCycleLateUpdate lifeCycleUpdate)
                lifeCycleUpdate.DoLateUpdate();
        }
    }
}
