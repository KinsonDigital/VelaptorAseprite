using System.Drawing;
using System.Text.Json.Serialization;

public class MetaData
{
    public string App { get; set; }

    [JsonPropertyName("version")]
    public string Version { get; set; }

    [JsonPropertyName("image")]
    public string ImageFileName { get; set; }

    public string Format { get; set; }

    [JsonConverter(typeof(SizeJsonConverter))]
    public Size Size { get; set; }

    public string Scale { get; set; }
}
