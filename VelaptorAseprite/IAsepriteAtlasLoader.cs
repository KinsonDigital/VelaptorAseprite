// <copyright file="IAsepriteAtlasLoader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorAseprite;

using Data;
using Velaptor.Content;

/// <summary>
/// Loads Aseprite texture atlas data.
/// </summary>
internal interface IAsepriteAtlasLoader : IUnloader<IAsepriteAtlasData>
{
    /// <inheritdoc cref="IAtlasLoader.TotalCachedItems"/>
    int TotalCachedItems { get; }

    /// <inheritdoc cref="IAtlasLoader.Load"/>
    IAsepriteAtlasData Load(string atlasPathOrName);
}
