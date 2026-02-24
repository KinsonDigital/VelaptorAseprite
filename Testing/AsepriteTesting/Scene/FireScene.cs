// <copyright file="FireScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace AsepriteTesting.Scene;

using System.Drawing;
using System.Numerics;
using Velaptor;
using Velaptor.Content;
using Velaptor.Content.Fonts;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.Graphics.Renderers;
using Velaptor.Input;
using Velaptor.Scene;
using VelaptorAseprite;
using VelaptorAseprite.Data;

/// <summary>
/// Used to test out multiple animations in a single atlas.
/// </summary>
public class FireScene : SceneBase
{
    private const string FontName = $"TimesNewRoman-{nameof(FontStyle.Regular)}.ttf";
    private const string FireAnimation = "blue-fire";
    private readonly IContentManager contentManager;
    private readonly ITextureRenderer textureRenderer;
    private readonly IFontRenderer fontRenderer;
    private readonly IAppInput<KeyboardState> keyboard;
    private IAsepriteAtlas? atlasData;
    private IFont? font;
    private KeyboardState prevKeyboardState;

    /// <summary>
    /// Initializes a new instance of the <see cref="FireScene"/> class.
    /// </summary>
    public FireScene()
    {
        this.textureRenderer = RendererFactory.CreateTextureRenderer();
        this.fontRenderer = RendererFactory.CreateFontRenderer();
        this.keyboard = HardwareFactory.GetKeyboard();

        this.contentManager = ContentManager.Create();
    }

    /// <inheritdoc cref="SceneBase" />
    public override void LoadContent()
    {
        this.atlasData = this.contentManager.LoadAsepriteAtlas(FireAnimation, "grow");
        this.atlasData.LoopingBehavior = LoopingBehavior.Infinite;

        this.atlasData.OnCycleComplete = _ =>
        {
            this.atlasData.Play("burn");
        };

        this.font = this.contentManager.LoadFont(FontName, 16);

        base.LoadContent();
    }

    /// <inheritdoc cref="SceneBase" />
    public override void UnloadContent()
    {
        if (this.atlasData is not null)
        {
            this.contentManager.UnloadAsepriteAtlas(this.atlasData);
        }

        if (this.font is not null)
        {
            this.contentManager.Unload(this.font);
        }

        base.UnloadContent();
    }

    /// <inheritdoc cref="SceneBase" />
    public override void Update(FrameTime frameTime)
    {
        ProcessInput();

        this.atlasData.Update(frameTime);

        base.Update(frameTime);
    }

    /// <inheritdoc cref="SceneBase" />
    public override void Render()
    {
        if (this.atlasData is null)
        {
            throw new Exception("The atlas data has not been loaded yet.");
        }

        var frame = this.atlasData.GetCurrentFrame();

        var srcRect = frame.Bounds;
        var destRect = new Rectangle(0, 0, (int)this.atlasData.Texture.Width, (int)this.atlasData.Texture.Height);
        destRect.X = WindowCenter.X;
        destRect.Y = WindowCenter.Y;

        this.textureRenderer.Render(
            this.atlasData.Texture,
            srcRect,
            destRect,
            1f,
            0f,
            Color.White,
            RenderEffects.None);

        RenderText();

        base.Render();
    }

    private void RenderText()
    {
        const int verticalSpacing = 15;
        var winWidth = (int)WindowSize.Width;

        var screenCenterX = (int)(WindowSize.Width / 2);
        var screenCenterY = (int)(WindowSize.Height / 2);
        var screenOneEighthHeightY = (int)(WindowSize.Height / 8);

        var animationName = $"Animation Name: {this.atlasData.AnimationName}";
        var animationNameSize = this.font.Measure(animationName);
        var animationNamePosY = screenOneEighthHeightY * 6;

        var animationEnabledText = $"Animation Enabled: {this.atlasData.IsAnimating}";
        var animationEnabledTextSize = this.font.Measure(animationEnabledText);
        var animationTextPosY = animationNamePosY + (int)animationNameSize.Height + verticalSpacing;

        var currentFrameText = $"Current Frame: {this.atlasData.CurrentFrameIndex}";
        var currentFrameTextSize = this.font.Measure(currentFrameText);
        var currentFrameTextPosY = animationTextPosY + (int)animationEnabledTextSize.Height + verticalSpacing;

        var instructionWidths = new[]
        {
            (int)animationNameSize.Width,
            (int)animationEnabledTextSize.Width,
            (int)currentFrameTextSize.Width,
        };

        var largestWidth = instructionWidths.Max();

        // Render instructions
        var instructionsText = "Press space to 'start/stop' animation\n";
        instructionsText += "Press 'up' to speed up movement velocity\n";
        instructionsText += "Press 'down' to slow down movement velocity\n";
        instructionsText += "Press 'left/right' to change animations";

        this.fontRenderer.Render(this.font,
            instructionsText,
            new Vector2(screenCenterX, screenCenterY - (screenOneEighthHeightY * 3f)));

        this.fontRenderer.Render(
            this.font,
            animationName,
            winWidth - ((int)animationNameSize.Width / 2) - largestWidth,
            animationNamePosY);
        this.fontRenderer.Render(
            this.font,
            animationEnabledText,
            winWidth - ((int)animationEnabledTextSize.Width / 2) - largestWidth,
            animationTextPosY);
        this.fontRenderer.Render(
            this.font,
            currentFrameText,
            winWidth - ((int)currentFrameTextSize.Width / 2) - largestWidth,
            currentFrameTextPosY);
    }

    /// <summary>
    /// Processes keyboard input.
    /// </summary>
    private void ProcessInput()
    {
        var currentKeyboardState = this.keyboard.GetState();

        if (currentKeyboardState.IsKeyUp(KeyCode.Space) && this.prevKeyboardState.IsKeyDown(KeyCode.Space))
        {
            if (this.atlasData.IsAnimating)
            {
                this.atlasData.Stop();
            }
            else
            {
                this.atlasData.Play(this.atlasData.AnimationName);
            }
        }

        this.prevKeyboardState = currentKeyboardState;
    }
}
