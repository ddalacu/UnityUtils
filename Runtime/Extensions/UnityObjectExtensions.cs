using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.Utility
{

    public struct InstantiateCommand
    {
        private readonly GameObject _prefab;
        private GameObject _instance;

        public InstantiateCommand(GameObject prefab, GameObject instance)
        {
            _prefab = prefab;
            _instance = instance;
        }

        public void Finish()
        {
            if (_prefab != null)
                _prefab.SetActive(true);
            if (_instance != null)
                _instance.SetActive(true);
        }
    }

    public static class UnityObjectExtensions
    {
        private static List<FieldInfo> GetAllFields(Type type, BindingFlags bindingFlags)
        {
            List<FieldInfo> result = new List<FieldInfo>();

            while (type != typeof(object))
            {
                FieldInfo[] fieldsInfos = type.GetFields(bindingFlags);

                int length = fieldsInfos.Length;
                for (int i = 0; i < length; i++)
                {
                    FieldInfo fieldInfo = result.Find(info => fieldsInfos[i].Name == info.Name && fieldsInfos[i].DeclaringType == info.DeclaringType);

                    if (fieldInfo == null)
                    {
                        result.Add(fieldsInfos[i]);
                    }
                }


                type = type.BaseType;
            }

            return result;
        }

        public delegate ScriptableObject CloneDelegate(ScriptableObject scriptableObject);

        private static void CloneScriptableSubAssets(Dictionary<int, Tuple<object, bool>> remap, object o, CloneDelegate clone)
        {
            int hashCode = RuntimeHelpers.GetHashCode(o);

            if (remap.ContainsKey(hashCode) == false)
                remap.Add(hashCode, new Tuple<object, bool>(o, false));

            List<FieldInfo> fieldsInfos = GetAllFields(o.GetType(), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (FieldInfo fieldInfo in fieldsInfos)
            {
                Type fieldType = fieldInfo.FieldType;

                if (fieldInfo.IsPublic == false)
                {
                    if (fieldInfo.GetCustomAttribute<SerializeField>() == null)
                        continue;
                }
                else
                {
                    if (fieldInfo.GetCustomAttribute<NonSerializedAttribute>() != null)
                        continue;
                }

                if (fieldType.IsArray)
                {
                    Array array = fieldInfo.GetValue(o) as Array;
                    if (array == null)
                        continue;

                    int arrayLength = array.Length;
                    for (int i = 0; i < arrayLength; i++)
                    {
                        object asset = array.GetValue(i);

                        if (asset == null)
                            continue;

                        Type assetType = asset.GetType();

                        if (assetType.IsPrimitive)
                            continue;
                        if (assetType.IsEnum)
                            continue;
                        if (assetType.IsGenericType)
                            continue;
                        if (assetType.IsConstructedGenericType)
                            continue;


                        int instanceId = RuntimeHelpers.GetHashCode(asset);
                        Tuple<object, bool> result;
                        if (remap.TryGetValue(instanceId, out result))
                        {
                            if (result.Item2)//is clone
                                array.SetValue(result.Item1, i);
                        }
                        else
                        {
                            if (asset is ScriptableObject)
                            {
                                ScriptableObject scriptableObject = clone(asset as ScriptableObject);
                                if (scriptableObject != null)
                                {
                                    asset = scriptableObject;
                                    remap.Add(instanceId, new Tuple<object, bool>(scriptableObject, true));
                                }
                                else
                                {
                                    remap.Add(instanceId, new Tuple<object, bool>(asset, false));
                                }
                            }

                            array.SetValue(asset, i);
                            CloneScriptableSubAssets(remap, asset, clone);
                        }
                    }
                }
                else
                if (fieldType.IsGenericType && (fieldType.GetGenericTypeDefinition() == typeof(List<>)))
                {
                    IList list = fieldInfo.GetValue(o) as IList;
                    if (list == null)
                        continue;

                    int count = list.Count;
                    for (var index = 0; index < count; index++)
                    {
                        object asset = list[index];

                        if (asset == null)
                            continue;

                        Type assetType = asset.GetType();

                        if (assetType.IsPrimitive)
                            continue;
                        if (assetType.IsEnum)
                            continue;
                        if (assetType.IsGenericType)
                            continue;
                        if (assetType.IsConstructedGenericType)
                            continue;

                        int instanceId = RuntimeHelpers.GetHashCode(asset);
                        Tuple<object, bool> result;
                        if (remap.TryGetValue(instanceId, out result))
                        {
                            if (result.Item2)//is clone
                                list[index] = result.Item1;
                        }
                        else
                        {
                            if (asset is ScriptableObject)
                            {
                                ScriptableObject scriptableObject = clone(asset as ScriptableObject);
                                if (scriptableObject != null)
                                {
                                    asset = scriptableObject;
                                    remap.Add(instanceId, new Tuple<object, bool>(scriptableObject, true));
                                }
                                else
                                {
                                    remap.Add(instanceId, new Tuple<object, bool>(asset, false));
                                }
                            }

                            list[index] = asset;
                            CloneScriptableSubAssets(remap, asset, clone);
                        }
                    }

                }
                else
                {
                    object asset = fieldInfo.GetValue(o);

                    if (asset != null)
                    {
                        Type assetType = asset.GetType();

                        if (assetType.IsPrimitive)
                            continue;

                        if (assetType.IsEnum)
                            continue;

                        if (assetType.IsGenericType)
                            continue;

                        if (assetType.IsConstructedGenericType)
                            continue;

                        int instanceId = RuntimeHelpers.GetHashCode(asset);
                        Tuple<object, bool> result;
                        if (remap.TryGetValue(instanceId, out result))
                        {
                            if (result.Item2)//is clone
                                fieldInfo.SetValue(o, result.Item1);
                        }
                        else
                        {
                            if (asset is ScriptableObject)
                            {
                                ScriptableObject scriptableObject = clone(asset as ScriptableObject);
                                if (scriptableObject != null)
                                {
                                    asset = scriptableObject;
                                    remap.Add(instanceId, new Tuple<object, bool>(scriptableObject, true));
                                }
                                else
                                {
                                    remap.Add(instanceId, new Tuple<object, bool>(asset, false));
                                }
                            }

                            fieldInfo.SetValue(o, asset);
                            CloneScriptableSubAssets(remap, asset, clone);
                        }

                    }

                }
            }
        }

        public static T DeepCloneScriptableObject<T>(this T scriptableObject, out List<ScriptableObject> instances, CloneDelegate clone) where T : ScriptableObject
        {
            T copy = Object.Instantiate(scriptableObject);
            Dictionary<int, Tuple<object, bool>> objDictionary = new Dictionary<int, Tuple<object, bool>>(128);

            int hashCode = RuntimeHelpers.GetHashCode(scriptableObject);
            objDictionary.Add(hashCode, new Tuple<object, bool>(copy, true));

            CloneScriptableSubAssets(objDictionary, copy, clone);

            instances = new List<ScriptableObject>(128);
            foreach (var o in objDictionary)
            {
                if (o.Value.Item2 == false)//not a clone
                    continue;

                if (o.Value.Item1 is ScriptableObject)
                {
                    instances.Add(o.Value.Item1 as ScriptableObject);
                }
            }

            return copy;
        }

        public static InstantiateCommand BeginInstantiate<T>([NotNull]this T prefab, out T instance, Transform parent = null) where T : Component
        {
            if (prefab == null)
                throw new ArgumentNullException(nameof(prefab));

            var gameObject = prefab.gameObject;

            var deactivated = false;

            if (gameObject.activeSelf)
            {
                deactivated = true;
                gameObject.SetActive(false);
            }

            instance = Object.Instantiate(prefab, parent);

            if (deactivated)
            {
                return new InstantiateCommand(gameObject, instance.gameObject);
            }

            return new InstantiateCommand(null, null);
        }

        public static InstantiateCommand BeginInstantiate([NotNull]this GameObject prefab, out GameObject instance, Transform parent = null)
        {
            if (prefab == null)
                throw new ArgumentNullException(nameof(prefab));


            var deactivated = false;

            if (prefab.activeSelf == false)
            {
                deactivated = true;
                prefab.SetActive(false);
            }

            instance = Object.Instantiate(prefab, parent);

            if (deactivated)
            {
                return new InstantiateCommand(prefab, instance);
            }

            return new InstantiateCommand(null, null);
        }

    }

}
