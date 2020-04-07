using System;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Framework.Utility
{

    ///<summary>
    /// Author Paul Diac
    /// Version 0.0a
    ///</summary>
    [Serializable]
    public sealed class ObjectPath
#if UNITY_EDITOR
        : ISerializationCallbackReceiver
#endif
    {
        [SerializeField]
        private string _objectPath = "";

        [SerializeField]
        private string _objectName = "";

        public string Path
        {
            get
            {
#if UNITY_EDITOR
                SetValueEditor(_value);
#endif
                return _objectPath;
            }
        }

        public string Name
        {
            get
            {
#if UNITY_EDITOR
                SetValueEditor(_value);
#endif
                return _objectName;
            }
        }

        public bool IsAssigned => string.IsNullOrEmpty(Path) == false;

#if UNITY_EDITOR
        [SerializeField]
        private Object _value = null;

        public void SetValueEditor(Object value)
        {
            if (value != null)
            {
                _objectName = value.name;
                string path = AssetDatabase.GetAssetPath(value);//instance assets have null paths, but if we use instantiate we still need the original path
                if (string.IsNullOrEmpty(path) == false)
                {
                    _objectPath = path;
                }
            }
            else
            {
                _objectPath = "";
            }
        }

        public void OnBeforeSerialize()
        {
            SetValueEditor(_value);
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
    public sealed class ObjectPathTypeAttribute : Attribute
    {
        public Type Type { get; private set; }
        public string FilterMethod { get; private set; }

        public ObjectPathTypeAttribute(Type type, string filterMethod = null)
        {
            Type = type;
            FilterMethod = filterMethod;
        }
    }
}