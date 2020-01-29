#if UNITY_EDITOR

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;
using Object = UnityEngine.Object;

[Serializable]
public class ObjReference
{
    public Object Obj;

    public static string GetJsonFromData(string guid, long localId, long typeId)
    {
        var jObj = new JObject
        {
            {nameof(Obj), new JObject
                {
                    { "fileID",new JValue(localId) },
                    { "guid",new JValue(guid) },
                    { "type",new JValue(typeId) },
                }
            }
        };
        return jObj.ToString(Formatting.None);
    }

    public static Object GetObjectFromData(string guid, long localId, long typeId)
    {
        if (string.IsNullOrEmpty(guid))
            return null;

        var json = GetJsonFromData(guid, localId, typeId);
        var instance = new ObjReference();
        EditorJsonUtility.FromJsonOverwrite(json, instance);
        return instance.Obj;
    }

    public static bool GetDataFromObject(Object obj, out string guid, out long localId, out long typeId)
    {
        guid = string.Empty;
        localId = 0;
        typeId = 0;

        if (obj == null)
            return false;

        var json = EditorJsonUtility.ToJson(new ObjReference
        {
            Obj = obj
        });

        var jObject = JObject.Parse(json);


        if (jObject.TryGetValue(nameof(Obj), out var token) && token is JObject data)
        {
            if (data.TryGetValue("fileID", out var parsedLocalId))
                localId = parsedLocalId.Value<long>();
            else
                return false;

            if (data.TryGetValue("guid", out var parsedGuid))
                guid = parsedGuid.Value<string>();
            else
                return false;

            if (data.TryGetValue("type", out var parsedType))
                typeId = parsedType.Value<long>();
            else
                return false;

        }

        return true;
    }

}

#endif