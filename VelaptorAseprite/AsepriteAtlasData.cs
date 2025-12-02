using Velaptor;
using VelaptorAseprite.Data;

namespace VelaptorAseprite;

using System.Text.Json.Serialization;
using Velaptor.Content;

internal class AsepriteAtlasData : IAsepriteAtlasData
{
    public ITexture Texture { get; internal set; }

    public string Name { get; internal set; }

    public string FilePath { get; internal set; }

    [JsonConverter(typeof(FramesJsonConverter))]
    public Dictionary<int, AnimationFrame> Frames { get; set; }

    public MetaData Meta { get; set; }

    public AnimationFrame GetCurrentFrame()
    {
        throw new NotImplementedException();
    }

    public void Update(FrameTime frameTime)
    {
    }
}
