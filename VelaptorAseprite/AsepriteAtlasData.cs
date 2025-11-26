using Velaptor;
using VelaptorAseprite.Data;

namespace VelaptorAseprite;

using Velaptor.Content;

internal class AsepriteAtlasData : IAsepriteAtlasData
{
    public ITexture Texture { get; internal set;}

    public string Name { get; internal set; }

    public string FilePath { get; internal set; }

    public Dictionary<string, AnimationFrame> Frames { get; set; }

    public MetaData Meta { get; set; }

    public void Update(FrameTime frameTime)
    {
        throw new NotImplementedException();
    }
}
