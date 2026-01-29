// <copyright file="ContentManagerExtensions.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorAseprite;

using System.Diagnostics.CodeAnalysis;
using Velaptor.Content;
using Data;

/// <summary>
/// Provides extension methods for the <see cref="IContentManager"/>.
/// </summary>
[ExcludeFromCodeCoverage(Justification = $"Cannot test due to direct interaction with the '{nameof(SimpleInjector)}' library.")]
public static class ContentManagerExtensions
{
#pragma warning disable IDE0060 // Remove unused parameter
    /// <summary>
    /// Loads an aseprite atlas data file.
    /// </summary>
    /// <param name="value">The content manager.</param>
    /// <param name="pathOrName">The full qualified path or name of the atlas content to load.</param>
    /// <returns>The Aseprite atlas data.</returns>
    [SuppressMessage("ReSharper", "UnusedParameter.Global", Justification = "Only used to apply extension method.")]
    public static IAsepriteAtlasData LoadAsepriteAtlasData(this IContentManager value, string pathOrName)
    {
        var atlasLoader = IoC.Container.GetInstance<IAsepriteAtlasLoader>();

        var atlasData = atlasLoader.Load(pathOrName);

        return atlasData;
    }

    /// <summary>
    /// Unloads the given <paramref name="atlasData"/>.
    /// </summary>
    /// <param name="value">The content manager.</param>
    /// <param name="atlasData">The Aseprite atlas data to unload.</param>
    [SuppressMessage("ReSharper", "UnusedParameter.Global", Justification = "Only used to apply extension method.")]
    public static void UnloadAsepriteAtlasData(this IContentManager value, IAsepriteAtlasData atlasData)
    {
        var atlasLoader = IoC.Container.GetInstance<IAsepriteAtlasLoader>();

        atlasLoader.Unload(atlasData);
    }
#pragma warning restore IDE0060 // Remove unused parameter
}
