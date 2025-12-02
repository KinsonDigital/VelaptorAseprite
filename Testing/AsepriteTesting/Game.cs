// <copyright file="Game.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace AsepriteTesting;

using Velaptor;
using Velaptor.Batching;
using Velaptor.Content;
using Velaptor.Factories;
using Velaptor.Graphics.Renderers;
using Velaptor.UI;
using VelaptorAseprite;
using VelaptorAseprite.Data;

/// <summary>
/// The main game class.
/// </summary>
public class Game : Window
{
    private readonly IContentManager contentManager;
    private readonly ITextureRenderer textureRenderer;
    private readonly IBatcher batcher;
    private IAsepriteAtlasData atlasData;

    public Game()
    {
        this.textureRenderer = RendererFactory.CreateTextureRenderer();
        this.batcher = RendererFactory.CreateBatcher();
        this.contentManager = ContentManager.Create();
    }

    /// <summary>
    /// Load content here.
    /// </summary>
    protected override void OnLoad()
    {
        this.atlasData = this.contentManager.LoadAsepriteAtlasData("bouncing-ball");

        base.OnLoad();
    }

    /// <summary>
    /// Unload content here.
    /// </summary>
    protected override void OnUnload()
    {
        this.contentManager.UnloadAsepriteAtlasData(this.atlasData);

        base.OnUnload();
    }

    /// <summary>
    /// Update size dependent related game objects and more here.
    /// </summary>
    /// <param name="size">The new window size.</param>
    protected override void OnResize(SizeU size)
    {
        base.OnResize(size);
    }

    /// <summary>
    /// Add game logic here.
    /// </summary>
    /// <param name="frameTime">The amount of time that passed for the current game loop frame.</param>
    protected override void OnUpdate(FrameTime frameTime)
    {
        base.OnUpdate(frameTime);
    }

    /// <summary>
    /// Render graphics here.
    /// </summary>
    /// <param name="frameTime">The amount of time that passed for the current game loop frame.</param>
    protected override void OnDraw(FrameTime frameTime)
    {
        this.batcher.Begin();

        this.textureRenderer.Render(this.atlasData.Texture, 400, 400);

        this.batcher.End();

        base.OnDraw(frameTime);
    }
}
