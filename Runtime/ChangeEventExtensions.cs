using System;
using UnityEngine.UIElements;

public static class ChangeEventExtensions
{
    public static void RegisterChangeEventCallback(CallbackEventHandler eventHandler, Type type, EventCallback<IChangeEvent> callback, TrickleDown useTrickleDown = TrickleDown.NoTrickleDown)
    {
        foreach (var method in eventHandler.GetType().GetMethods())
        {
            if (method.Name != nameof(CallbackEventHandler.RegisterCallback))
                continue;

            var genericArguments = method.GetGenericArguments();
            if (genericArguments.Length == 1)
            {
                var constructedGenericType = typeof(ChangeEvent<>).MakeGenericType(type);
                var typedMethod = method.MakeGenericMethod(constructedGenericType);

                typedMethod.Invoke(eventHandler, new object[]
                {
                    callback,
                    useTrickleDown
                });

                return;
            }
        }

        throw new NotImplementedException();
    }

    public static void UnregisterChangeEventCallback(CallbackEventHandler eventHandler, Type type, EventCallback<IChangeEvent> callback, TrickleDown useTrickleDown = TrickleDown.NoTrickleDown)
    {
        foreach (var method in eventHandler.GetType().GetMethods())
        {
            if (method.Name != nameof(CallbackEventHandler.UnregisterCallback))
                continue;

            var genericArguments = method.GetGenericArguments();
            if (genericArguments.Length == 1)
            {
                var constructedGenericType = typeof(ChangeEvent<>).MakeGenericType(type);
                var typedMethod = method.MakeGenericMethod(constructedGenericType);

                typedMethod.Invoke(eventHandler, new object[]
                {
                    callback,
                    useTrickleDown
                });

                return;
            }
        }

        throw new NotImplementedException();
    }

}