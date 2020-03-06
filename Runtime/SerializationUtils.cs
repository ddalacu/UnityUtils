using System.IO;
using Newtonsoft.Json;

public class SerializationUtils
{
    public static void SerializeToStream(object value, Stream s, JsonSerializer serializer)
    {
        using (StreamWriter writer = new StreamWriter(s))
        using (JsonTextWriter jsonWriter = new JsonTextWriter(writer))
        {
            serializer.Serialize(jsonWriter, value);
            jsonWriter.Flush();
        }
    }

    public static T DeserializeFromStream<T>(Stream s, JsonSerializer serializer)
    {
        using (StreamReader reader = new StreamReader(s))
        using (JsonTextReader jsonReader = new JsonTextReader(reader))
        {
            return serializer.Deserialize<T>(jsonReader);
        }
    }

}