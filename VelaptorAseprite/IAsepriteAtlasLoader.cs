// <copyright file="IAsepriteAtlasLoader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorAseprite;

using Data;
using Velaptor.Content;

/// <summary>
/// Loads Aseprite texture atlas data.
/// </summary>
internal interface IAsepriteAtlasLoader : IUnloader<IAsepriteAtlas>
{
    /// <inheritdoc cref="IAtlasLoader.TotalCachedItems"/>
    int TotalCachedItems { get; }

    /// <summary>
    /// Loads Aseprite atlas data using the given <paramref name="atlasPathOrName" />.
    /// </summary>
    /// <param name="atlasPathOrName">The content name or file path to the atlas data.</param>
    /// <param name="animationName">The name of the animation to load by default.</param>
    /// <returns>The loaded atlas data.</returns>
    IAsepriteAtlas Load(string atlasPathOrName, string? animationName = null);
}
