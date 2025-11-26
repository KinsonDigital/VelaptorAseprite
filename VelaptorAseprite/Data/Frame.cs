using System.Drawing;
using System.Text.Json.Serialization;

namespace VelaptorAseprite.Data;

public class AnimationFrame
{
    [JsonConverter(typeof(RectangleJsonConverter))]
    public Rectangle Frame { get; set; }

    public bool Rotated { get; set; }

    public bool Trimmed { get; set; }

    [JsonConverter(typeof(RectangleJsonConverter))]
    public Rectangle SpriteSourceSize { get; set; }

    [JsonConverter(typeof(SizeJsonConverter))]
    public Size SourceSize { get; set; }

    public int Duration { get; set; }
}
