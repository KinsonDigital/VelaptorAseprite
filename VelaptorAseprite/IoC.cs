// <copyright file="IoC.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorAseprite;

using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using SimpleInjector;
using Velaptor.Content;
using Velaptor.Content.Factories;
using Velaptor.Factories;
using Velaptor.Services;
using VelapIoC = Velaptor.IoC;

/// <summary>
/// Provides dependency injection for the application.
/// </summary>
[ExcludeFromCodeCoverage(Justification = $"Cannot test due to direct interaction with the '{nameof(SimpleInjector)}' library.")]
internal static class IoC
{
    private static readonly Container IoCContainer = new ();
    private static bool isInitialized;

    /// <summary>
    /// Gets the inversion of control container used to get instances of objects.
    /// </summary>
    public static Container Container
    {
        get
        {
            if (!isInitialized)
            {
                SetupContainer();
            }

            return IoCContainer;
        }
    }

    /// <summary>
    /// Sets up the IoC container.
    /// </summary>
    private static void SetupContainer()
    {
        IoCContainer.Register<IAsepriteAtlasLoader>(() =>
        {
            var textureFactory = VelapIoC.Container.GetInstance<ITextureFactory>();
            var atlasDataFactory = VelapIoC.Container.GetInstance<IAtlasDataFactory>();
            var reactableFactory = VelapIoC.Container.GetInstance<IReactableFactory>();
            var atlasDataPathResolver = VelapIoC.Container.GetInstance<IContentPathResolver>();
            var imageService = VelapIoC.Container.GetInstance<IImageService>();
            var jsonService = VelapIoC.Container.GetInstance<IJsonService>();
            var directory = VelapIoC.Container.GetInstance<IDirectory>();
            var file = VelapIoC.Container.GetInstance<IFile>();
            var path = VelapIoC.Container.GetInstance<IPath>();

            return new AsepriteAtlasLoader(textureFactory,
                atlasDataFactory,
                reactableFactory,
                atlasDataPathResolver,
                imageService,
                jsonService,
                directory,
                file,
                path);
        }, Lifestyle.Singleton);

        isInitialized = true;
    }
}
