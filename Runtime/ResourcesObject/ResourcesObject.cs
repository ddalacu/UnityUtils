using System;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Threading;
[Serializable]
public class ResourcesObject
#if UNITY_EDITOR
    : ISerializationCallbackReceiver
#endif
{
    public const string MoveInResourcesTag = "MoveInResources";

    /// <summary>
    /// Do not call this in OnBeforeSerialize/OnAfterDeserialize or you will crash!
    /// </summary>
    /// <returns></returns>
    public T Load<T>() where T : Object
    {
        if (string.IsNullOrEmpty(_guid))
            return null;

#if UNITY_EDITOR

        var str = $"GlobalObjectId_V1-{_typeId}-{_guid}-{(ulong)_localId}-{(ulong)_prefabId}";

        if (GlobalObjectId.TryParse(str, out var id))
        {
            var obj = GlobalObjectId.GlobalObjectIdentifierToObjectSlow(id);
            Debug.Assert(Type == obj.GetType());
            return obj as T;
        }

        return default;
#else
        if (string.IsNullOrEmpty(_guid))
            return null;
        return Resources.Load<T>(_guid);
#endif
    }

    public Object LoadObject()
    {
        if (string.IsNullOrEmpty(_guid))
            return null;

#if UNITY_EDITOR

        var str = $"GlobalObjectId_V1-{_typeId}-{_guid}-{(ulong)_localId}-{(ulong)_prefabId}";

        if (GlobalObjectId.TryParse(str, out var id))
        {
            var obj = GlobalObjectId.GlobalObjectIdentifierToObjectSlow(id);
            Debug.Assert(Type == obj.GetType());
            return obj;
        }

        return default;
#else
        if (string.IsNullOrEmpty(_guid))
            return null;
        return Resources.Load(_guid);
#endif
    }
    public async Task<T> LoadAsync<T>(CancellationToken token) where T : Object
    {
        if (string.IsNullOrEmpty(_guid))
            return null;

#if UNITY_EDITOR

        var str = $"GlobalObjectId_V1-{_typeId}-{_guid}-{_localId}-{_prefabId}";

        if (GlobalObjectId.TryParse(str, out var id))
        {
            var obj = GlobalObjectId.GlobalObjectIdentifierToObjectSlow(id);
            Debug.Assert(Type == obj.GetType());
            return obj as T;
        }

        return default;
#else
        if (string.IsNullOrEmpty(_guid))
            return null;

        var request = Resources.LoadAsync<T>(_guid);

        while (request.isDone == false)
        {
            await Task.Delay(32).ConfigureAwait(true);
            token.ThrowIfCancellationRequested();
        }

        return request.asset as T;
#endif
    }

    [SerializeField]
    private string _guid;

    [SerializeField]
    private long _localId;

    [SerializeField]
    private long _prefabId;

    [SerializeField]
    private int _typeId;

    [SerializeField]
    private string _objectName;

    [SerializeField]
    private string _assemblyQualifiedTypeName;

    public bool IsValid => string.IsNullOrEmpty(_guid) == false;

    public string Guid => _guid;

    public long LocalId => _localId;

    public long PrefabId => _localId;

    public int TypeId => _typeId;

    public string AssemblyQualifiedTypeName => _assemblyQualifiedTypeName;

    private Type _cachedType;


    public Type Type
    {
        get
        {
#if UNITY_EDITOR
            return Type.GetType(_assemblyQualifiedTypeName);
#else
            if (_cachedType == null && string.IsNullOrEmpty(_assemblyQualifiedTypeName) == false)//we can cache type at runtime because _assemblyQualifiedTypeName will not change
                _cachedType = Type.GetType(_assemblyQualifiedTypeName);

            return _cachedType;
#endif
        }
    }

    public string ObjectName => _objectName;

#if UNITY_EDITOR

    public static bool EditorIsBuilding = false;


    public static System.Collections.Generic.IEnumerable<string> ResourcesObjectsToGuidsEditor(System.Collections.Generic.IEnumerable<ResourcesObject> objects)
    {
        var index = 0;
        foreach (var resourcesObject in objects)
        {
            var guid = resourcesObject.Guid;

            if (string.IsNullOrEmpty(guid))
            {
                Debug.LogError($"Null guid at index {index}!");
                index++;
                continue;
            }

            index++;
            yield return guid;
        }
    }

    public void EditorSet(Object obj)
    {
        if (obj != null)
        {
            var globalObjId = GlobalObjectId.GetGlobalObjectIdSlow(obj);

            _guid = globalObjId.assetGUID.ToString();
            _typeId = globalObjId.identifierType;
            _localId = (long)globalObjId.targetObjectId;
            _prefabId = (long)globalObjId.targetPrefabId;

            _objectName = obj.name;
            _assemblyQualifiedTypeName = obj.GetType().AssemblyQualifiedName;
        }
        else
        {
            _guid = string.Empty;
            _localId = 0;
            _typeId = 0;
            _objectName = string.Empty;
            _assemblyQualifiedTypeName = string.Empty;
        }
    }

    public void OnBeforeSerialize()
    {
        if (string.IsNullOrEmpty(_guid))
            return;

        if (EditorIsBuilding == false) //if we serialize during the build then move the asset to resources
            return;

#if UNITY_EDITOR

        if (Type == typeof(SceneAsset))
            return;

        var path = AssetDatabase.GUIDToAssetPath(_guid);

        if (path.Contains("Resources") == false)
        {
            Debug.LogError($"{_objectName} {_guid} is not in resources, something is bad!");
        }
#endif


    }

    public void OnAfterDeserialize()
    {

    }
#endif
}

///<summary>
/// Author Paul Diac
/// Version 0.0a
///</summary>
[AttributeUsage(AttributeTargets.Field)]
public sealed class ResourcesObjectAttribute : Attribute
{
    public Type Type { get; private set; }
    public string FilterMethod { get; private set; }

    public ResourcesObjectAttribute(Type type, string filterMethod = null)
    {
        Type = type;
        FilterMethod = filterMethod;
    }
}