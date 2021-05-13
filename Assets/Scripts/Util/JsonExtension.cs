using Newtonsoft.Json;

/// <summary>
/// Extension for json
/// </summary>
public static class JsonExtension
{
    /// <summary>
    /// JsonExtension constructor 
    /// </summary>
    static JsonExtension()
    {
        Initialize();
    }

    /// <summary>
    /// Initialize default setting
    /// </summary>
    private static void Initialize()
    {
        JsonConvert.DefaultSettings = () => new JsonSerializerSettings 
        {
            Formatting = Formatting.None,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
    }

    /// <summary>
    /// Convert object to json
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string ToJson(this object value)
    {
        if (value == null)
            return string.Empty;
        
        return JsonConvert.SerializeObject(value);
    }

    /// <summary>
    /// Convert json to instance
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="json"></param>
    /// <returns></returns>
    public static T ToInstance<T>(this string json)
    {
        return JsonConvert.DeserializeObject<T>(json);
    }
}