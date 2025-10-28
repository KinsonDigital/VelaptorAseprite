using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;

public class SizeJsonConverter : JsonConverter<Size>
{
    public override Size Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;
        
        var w = root.GetProperty("w").GetInt32();
        var h = root.GetProperty("h").GetInt32();
        
        return new Size(w, h);
    }

    public override void Write(Utf8JsonWriter writer, Size value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber("w", value.Width);
        writer.WriteNumber("h", value.Height);
        writer.WriteEndObject();
    }
}
