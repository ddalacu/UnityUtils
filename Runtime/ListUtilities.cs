using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Unity.Collections.LowLevel.Unsafe;
using Unity.IL2CPP.CompilerServices;

public static class ListUtilities
{
    public static readonly int ItemsFieldOffset;
    public static readonly int SizeFieldOffset;

    static ListUtilities()
    {
        var type = typeof(List<>);
        var itemsMember = type.GetField("_items", BindingFlags.Instance | BindingFlags.NonPublic);
        var sizeMember = type.GetField("_size", BindingFlags.Instance | BindingFlags.NonPublic);

        ItemsFieldOffset = UnsafeUtility.GetFieldOffset(itemsMember);
        SizeFieldOffset = UnsafeUtility.GetFieldOffset(sizeMember);
    }

    /// <summary>
    /// Creates a list of T then set its internal array to the passed array, this method will only allocate the list and not the internal buffer of the list
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    [Il2CppSetOption(Option.NullChecks, false)]
    public static List<TMember> CreateFromBuffer<TMember>([NotNull] TMember[] buffer)
    {
        if (buffer == null)
            throw new ArgumentNullException(nameof(buffer));

        var result = new List<TMember>(0);
        UseBuffer(result, buffer);
        return result;
    }

    /// <summary>
    /// Changes the internal array in the passed list
    /// </summary>
    /// <param name="list"></param>
    /// <param name="buffer"></param>
    [Il2CppSetOption(Option.NullChecks, false)]
    public static unsafe void UseBuffer<TMember>([NotNull] List<TMember> list, TMember[] buffer)
    {
        if (list == null)
            throw new ArgumentNullException(nameof(list));

        var ptr = (byte*)UnsafeUtility.PinGCObjectAndGetAddress(list, out var handle);
        UnsafeUtility.CopyObjectAddressToPtr(buffer, ptr + ItemsFieldOffset);
        *(int*)(ptr + SizeFieldOffset) = buffer.Length;
        UnsafeUtility.ReleaseGCObject(handle);
    }

    /// <summary>
    /// Returns the internal array used by a list
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    [Il2CppSetOption(Option.NullChecks, false)]
    public static unsafe TMember[] ExtractBuffer<TMember>([NotNull] List<TMember> list)
    {
        //We could use UnityEngine.NoAllocHelpers.ExtractArrayFromList that method is faster but internal :(
        if (list == null)
            throw new ArgumentNullException(nameof(list));

        var ptr = (byte*)UnsafeUtility.PinGCObjectAndGetAddress(list, out var handle);
        TMember[] objectContainer = null;
        Unsafe.Copy(ref objectContainer, ptr + ItemsFieldOffset);
        UnsafeUtility.ReleaseGCObject(handle);
        return objectContainer;
    }

}
