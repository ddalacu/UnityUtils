using System;
using System.Collections.Generic;

public class ServiceContainer
{
    public static readonly ServiceContainer Instance = new ServiceContainer();

    readonly Dictionary<Type, object> _serviceMap;

    readonly object _serviceMapLock;

    private ServiceContainer()
    {
        _serviceMap = new Dictionary<Type, object>();
        _serviceMapLock = new object();
    }

    public void RemoveService<TServiceContract>(TServiceContract implementation) where TServiceContract : class
    {
        lock (_serviceMapLock)
        {
            if (_serviceMap.TryGetValue(typeof(TServiceContract), out var result) && result.Equals(implementation))
                _serviceMap.Remove(typeof(TServiceContract));
        }
    }

    public void AddService<TServiceContract>(TServiceContract implementation) where TServiceContract : class
    {
        lock (_serviceMapLock)
        {
            _serviceMap[typeof(TServiceContract)] = implementation;
        }
    }

    public bool FillService<TServiceContract>(out TServiceContract result) where TServiceContract : class
    {
        lock (_serviceMapLock)
        {
            object service;
            if (_serviceMap.TryGetValue(typeof(TServiceContract), out service))
            {
                result = service as TServiceContract;
                return result != null;
            }

            result = null;
            return false;
        }
    }

    public TServiceContract GetService<TServiceContract>()
        where TServiceContract : class
    {
        object service;
        lock (_serviceMapLock)
        {
            _serviceMap.TryGetValue(typeof(TServiceContract), out service);
        }
        return service as TServiceContract;
    }
}