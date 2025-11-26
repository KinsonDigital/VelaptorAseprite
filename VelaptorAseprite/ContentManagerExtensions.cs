// <copyright file="ContentManagerExtensions.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorAseprite;

using Velaptor.Content;
using Data;
using Services;
using Velaptor;

public static class ContentManagerExtensions
{
    public static IAsepriteAtlasData LoadAsepriteAtlasData(this IContentManager contentManager, string pathOrNanme)
    {
        var atlasLoader = IoC.Container.GetInstance<IAsepriteAtlasLoader>();

        return null;
    }
}
