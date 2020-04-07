using System;
using UnityEngine;

public class AppState : MonoBehaviour
{
    public static bool IsQuitting;

    private static bool _internetNotReachable;

    public static bool IsInternetReachable => _internetNotReachable == false;

    public static event Action<bool> ConnectionStatusChanged;

    [RuntimeInitializeOnLoadMethod]
    private static void Initialize()
    {
        var go = new GameObject(nameof(AppState));
        go.AddComponent<AppState>();
        DontDestroyOnLoad(go);
        go.hideFlags = HideFlags.NotEditable | HideFlags.DontSave;
    }

    public void Awake()
    {
        IsQuitting = true;
        Application.quitting += ApplicationQuitting;
    }

    private void OnDestroy()
    {
        Application.quitting -= ApplicationQuitting;
    }

    private void Update()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            if (_internetNotReachable == false)
            {
                _internetNotReachable = true;
                ConnectionStatusChanged?.Invoke(false);
            }
        }
        else
        {
            if (_internetNotReachable)
            {
                _internetNotReachable = false;
                ConnectionStatusChanged?.Invoke(true);
            }
        }
    }

    private void ApplicationQuitting()
    {
        IsQuitting = true;
    }
}
