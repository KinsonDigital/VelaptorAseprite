// <copyright file="ContentManagerExtensions.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorAseprite;

using Velaptor.Content;
using Data;

public static class ContentManagerExtensions
{
    public static IAsepriteAtlasData LoadAsepriteAtlasData(this IContentManager _, string pathOrNanme)
    {
        var atlasLoader = IoC.Container.GetInstance<IAsepriteAtlasLoader>();

        var atlasData = atlasLoader.Load(pathOrNanme);

        return atlasData;
    }
}
