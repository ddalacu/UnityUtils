using System;
using System.Globalization;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Framework.Utility
{
    /// <summary>
    /// Holds data about this build
    /// </summary>
    public class BuildInformations : ScriptableObject
    {
        [SerializeField]
        private string _buildTimeAsString;

        public string BuildTimeAsString
        {
            get
            {
#if UNITY_EDITOR
                if (string.IsNullOrEmpty(_buildTimeAsString))
                    _buildTimeAsString = DateTime.Now.ToString(CultureInfo.InvariantCulture);
#endif

                return _buildTimeAsString;
            }

            set => _buildTimeAsString = value;
        }

        public static BuildInformations LoadDefault()
        {
#if UNITY_EDITOR
            if (AssetDatabase.IsValidFolder("Assets/Resources") == false)
                AssetDatabase.CreateFolder("Assets", "Resources");

            string path = "Assets/Resources/" + nameof(BuildInformations) + ".asset";
            BuildInformations existingInformations = AssetDatabase.LoadAssetAtPath<BuildInformations>(path);

            if (existingInformations != null)
            {
                return existingInformations;
            }

            existingInformations = CreateInstance<BuildInformations>();
            AssetDatabase.CreateAsset(existingInformations, path);
            return existingInformations;
#else
            return Resources.Load<BuildInformations>(nameof(BuildInformations));
#endif
        }
    }
}

