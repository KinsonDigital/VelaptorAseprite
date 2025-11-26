// <copyright file="IAsepriteAtlasData.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorAseprite.Data;

using Velaptor;
using Velaptor.Content;

public interface IAsepriteAtlasData : IContent, IUpdatable
{
    ITexture Texture { get; }

    public Dictionary<string, AnimationFrame> Frames { get; set; }

    public MetaData Meta { get; set; }
}
