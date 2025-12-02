// <copyright file="Game.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace AsepriteTesting;

using System.Drawing;
using Velaptor;
using Velaptor.Batching;
using Velaptor.Content;
using Velaptor.Factories;
using Velaptor.Graphics;
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

        var frame = this.atlasData.Frames[0];

        var srcRect = frame.Bounds;
        var destRect = new Rectangle(0, 0, (int)this.atlasData.Texture.Width, (int)this.atlasData.Texture.Height);
        destRect.X = 400;
        destRect.Y = 400;

        this.textureRenderer.Render(
            this.atlasData.Texture,
            srcRect,
            destRect,
            3f,
            0f,
            Color.White,
            RenderEffects.None);

        this.batcher.End();

        base.OnDraw(frameTime);
    }
}
