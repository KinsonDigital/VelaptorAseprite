// <copyright file="IoC.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorAseprite;

using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using SimpleInjector;
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
            var reactableFactory = VelapIoC.Container.GetInstance<IReactableFactory>();

            var pathResolverFactory = VelapIoC.Container.GetInstance<IPathResolverFactory>();
            var atlasDataPathResolver = pathResolverFactory.CreateAtlasPathResolver();

            var imageService = VelapIoC.Container.GetInstance<IImageService>();
            var jsonService = VelapIoC.Container.GetInstance<IJsonService>();
            var directory = VelapIoC.Container.GetInstance<IDirectory>();
            var file = VelapIoC.Container.GetInstance<IFile>();
            var path = VelapIoC.Container.GetInstance<IPath>();

            return new AsepriteAtlasLoader(textureFactory,
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
