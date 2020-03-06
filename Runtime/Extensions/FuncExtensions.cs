using System;
using System.Collections.Generic;

public static class FuncExtensions
{
    public static IEnumerable<TReturn> InvokeEach<TReturn>(this Func<TReturn> func)
    {
        if (func != null)
            foreach (var del in func.GetInvocationList())
                yield return ((Func<TReturn>)del)();
    }

    public static IEnumerable<TReturn> InvokeEach<T1, TReturn>(this Func<T1, TReturn> func, T1 param1)
    {
        if (func != null)
            foreach (var del in func.GetInvocationList())
                yield return ((Func<T1, TReturn>)del)(param1);
    }

    public static IEnumerable<TReturn> InvokeEach<T1, T2, TReturn>(this Func<T1, T2, TReturn> func, T1 param1, T2 param2)
    {
        if (func != null)
            foreach (var del in func.GetInvocationList())
                yield return ((Func<T1, T2, TReturn>)del)(param1, param2);
    }

    public static bool AllTrue(this Func<bool> func)
    {
        if (func != null)
            foreach (var del in func.GetInvocationList())
            {
                var result = ((Func<bool>)del)();
                if (result == false)
                    return false;
            }
        return true;
    }

    public static bool AllFalse(this Func<bool> func)
    {
        if (func != null)
            foreach (var del in func.GetInvocationList())
            {
                var result = ((Func<bool>)del)();
                if (result)
                    return false;
            }
        return true;
    }

    public static bool AllTrue<T1>(this Func<T1, bool> func, T1 param1)
    {
        if (func != null)
            foreach (var del in func.GetInvocationList())
            {
                var result = ((Func<T1, bool>)del)(param1);
                if (result == false)
                    return false;
            }
        return true;
    }

    public static bool AllFalse<T1>(this Func<bool> func, T1 param1)
    {
        if (func != null)
            foreach (var del in func.GetInvocationList())
            {
                var result = ((Func<T1, bool>)del)(param1);
                if (result)
                    return false;
            }
        return true;
    }

    public static bool AllTrue<T1, T2>(this Func<T1, bool> func, T1 param1, T2 param2)
    {
        if (func != null)
            foreach (var del in func.GetInvocationList())
            {
                var result = ((Func<T1, T2, bool>)del)(param1, param2);
                if (result == false)
                    return false;
            }
        return true;
    }

    public static bool AllFalse<T1, T2>(this Func<bool> func, T1 param1, T2 param2)
    {
        if (func != null)
            foreach (var del in func.GetInvocationList())
            {
                var result = ((Func<T1, T2, bool>)del)(param1, param2);
                if (result)
                    return false;
            }
        return true;
    }

}