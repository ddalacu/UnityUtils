using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.Utility
{
    [CustomPropertyDrawer(typeof(ResourcesObject))]
    public class ResourcesObjectDrawer : PropertyDrawer
    {
        private delegate bool FilterDelegate(Object obj);

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var resourcesObjectAttribute = fieldInfo?.GetCustomAttribute<ResourcesObjectAttribute>(false);

            if (resourcesObjectAttribute == null)
            {
                EditorGUI.HelpBox(position, $"Add {nameof(ResourcesObjectAttribute)} on {label.text}", MessageType.Error);
            }
            else
            {
                var guidProperty = property.FindPropertyRelative("_guid");
                var localIdProperty = property.FindPropertyRelative("_localId");
                var typeIdProperty = property.FindPropertyRelative("_typeId");
                var prefabIdProperty = property.FindPropertyRelative("_prefabId");
                var objectNameProperty = property.FindPropertyRelative("_objectName");
                var assemblyQalifiedProperty = property.FindPropertyRelative("_assemblyQualifiedTypeName");


                var assetGuid = guidProperty.stringValue;

                Object value = null;

                if (string.IsNullOrEmpty(assetGuid) == false)
                {
                    var assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
                    value = AssetDatabase.LoadMainAssetAtPath(assetPath);
                }

                EditorGUI.BeginChangeCheck();

                Object result;
                //EditorGUIUtility.labelWidth = GUI.skin.label.CalcSize(label).x + 2;

                if (property.hasMultipleDifferentValues)
                {
                    EditorGUI.showMixedValue = true;
                    result = EditorGUI.ObjectField(position, property.displayName, value,
                        resourcesObjectAttribute.Type, false);
                    EditorGUI.showMixedValue = false;
                }
                else
                {
                    result = EditorGUI.ObjectField(position, property.displayName, value,
                        resourcesObjectAttribute.Type, false);
                    EditorGUI.showMixedValue = false;
                }

                if (EditorGUI.EndChangeCheck())
                {
                    if (result != null)
                    {
                        var asset = AssetDatabase.GetAssetPath(result);

                        var reload = AssetDatabase.LoadAssetAtPath(asset, resourcesObjectAttribute.Type);

                        if (result != reload)
                        {
                            Debug.LogError($"{result} is not a main asset, this field accepts only main assets!");
                            result = null;
                        }

                        if (string.IsNullOrEmpty(resourcesObjectAttribute.FilterMethod) == false)
                        {
                            FilterDelegate method = property.CreateDelegateFromParentMethod<FilterDelegate>(resourcesObjectAttribute.FilterMethod);
                            if (method != null && method(result) == false)
                            {
                                result = value;
                            }
                        }
                    }

                    if (result != null)
                    {
                        var globalObjId = GlobalObjectId.GetGlobalObjectIdSlow(result);

                        guidProperty.stringValue = globalObjId.assetGUID.ToString();
                        localIdProperty.longValue = (long)globalObjId.targetObjectId;
                        prefabIdProperty.longValue = (long) globalObjId.targetPrefabId;
                        typeIdProperty.intValue = globalObjId.identifierType;
                        objectNameProperty.stringValue = result.name;
                        assemblyQalifiedProperty.stringValue = result.GetType().AssemblyQualifiedName;
                    }
                    else
                    {
                        guidProperty.stringValue = string.Empty;
                        localIdProperty.longValue = 0;
                        prefabIdProperty.longValue = 0;
                        typeIdProperty.intValue = 0;
                        objectNameProperty.stringValue = string.Empty;
                    }
                }

            }
        }
    }
}