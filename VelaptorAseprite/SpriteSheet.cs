using VelaptorAseprite.Data;

namespace VelaptorAseprite;

public class SpriteSheet
{
    public Dictionary<string, AnimationFrame> Frames { get; set; }

    public MetaData Meta { get; set; }
}
