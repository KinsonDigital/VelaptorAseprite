// <copyright file="CompassScene.cs" company="KinsonDigital">
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
/// Used to test out various animation behaviors, direction, speed, and more.
/// </summary>
public class CompassScene : SceneBase
{
    private const string FontName = $"TimesNewRoman-{nameof(FontStyle.Regular)}.ttf";
    private const float RenderScale = 3;
    private readonly IContentManager contentManager;
    private readonly ITextureRenderer textureRenderer;
    private readonly IFontRenderer fontRenderer;
    private readonly IAppInput<KeyboardState> keyboard;
    private IAsepriteAtlasData? atlasData;
    private IFont? font;
    private KeyboardState prevKeyboardState;
    private int animationSpeedMs = 100;

    /// <summary>
    /// Initializes a new instance of the <see cref="CompassScene"/> class.
    /// </summary>
    public CompassScene()
    {
        this.contentManager = ContentManager.Create();
        this.textureRenderer = RendererFactory.CreateTextureRenderer();
        this.fontRenderer = RendererFactory.CreateFontRenderer();

        this.keyboard = HardwareFactory.GetKeyboard();
    }

    /// <inheritdoc cref="SceneBase" />
    public override void LoadContent()
    {
        this.atlasData = this.contentManager.LoadAsepriteAtlasData("compass");
        this.atlasData.MaxLoops = 4;
        this.atlasData.LoopingBehavior = LoopingBehavior.None;

        this.font = this.contentManager.LoadFont(FontName, 16);

        base.LoadContent();
    }

    /// <inheritdoc cref="SceneBase" />
    public override void UnloadContent()
    {
        if (this.atlasData is not null)
        {
            this.contentManager.UnloadAsepriteAtlasData(this.atlasData);
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
        this.atlasData.Update(frameTime);
        ProcessInput();

        base.Update(frameTime);
    }

    /// <inheritdoc cref="SceneBase" />
    public override void Render()
    {
        var frame = this.atlasData!.GetCurrentFrame();
        var srcRect = frame.Bounds;
        var destRect = new Rectangle(0, 0, (int)this.atlasData!.Texture.Width, (int)this.atlasData!.Texture.Height);

        destRect.X = (int)(WindowSize.Width / 2);
        destRect.Y = (int)(WindowSize.Height / 2);

        this.textureRenderer.Render(
            this.atlasData!.Texture,
            srcRect,
            destRect,
            RenderScale,
            0f,
            Color.White,
            RenderEffects.None);

        RenderText();

        base.Render();
    }

    /// <summary>
    /// Processes keyboard input.
    /// </summary>
    private void ProcessInput()
    {
        var currentKeyboardState = this.keyboard.GetState();

        // Start/Stop animation
        if (currentKeyboardState.IsKeyUp(KeyCode.Space) && this.prevKeyboardState.IsKeyDown(KeyCode.Space))
        {
            if (this.atlasData.IsAnimating)
            {
                this.atlasData.Stop();
            }
            else
            {
                this.atlasData.Play();
            }
        }

        // Animate forwards
        if (currentKeyboardState.IsKeyUp(KeyCode.Right) && this.prevKeyboardState.IsKeyDown(KeyCode.Right))
        {
            this.atlasData.Direction = AnimationDirection.Forward;
        }

        // Animate backwards
        if (currentKeyboardState.IsKeyUp(KeyCode.Left) && this.prevKeyboardState.IsKeyDown(KeyCode.Left))
        {
            this.atlasData.Direction = AnimationDirection.Backward;
        }

        // Speed animation up
        if (currentKeyboardState.IsKeyUp(KeyCode.Up) && this.prevKeyboardState.IsKeyDown(KeyCode.Up))
        {
            this.animationSpeedMs -= 25;
            this.animationSpeedMs = this.animationSpeedMs < 0 ? 0 : this.animationSpeedMs;
            this.atlasData.SetAnimationSpeed(this.animationSpeedMs);
        }

        // Slow animation down
        if (currentKeyboardState.IsKeyUp(KeyCode.Down) && this.prevKeyboardState.IsKeyDown(KeyCode.Down))
        {
            this.animationSpeedMs += 25;
            this.atlasData.SetAnimationSpeed(this.animationSpeedMs);
        }

        // Reset animation
        if (currentKeyboardState.IsKeyUp(KeyCode.Enter) && this.prevKeyboardState.IsKeyDown(KeyCode.Enter))
        {
            this.atlasData.Reset();
        }

        // Change looping behavior
        if (currentKeyboardState.IsKeyUp(KeyCode.L) && this.prevKeyboardState.IsKeyDown(KeyCode.L))
        {
            var loopBehaviors = Enum.GetValues<LoopingBehavior>();
            var maxValue = loopBehaviors.Max();

            var nextLoopBehavior = this.atlasData.LoopingBehavior + 1;
            nextLoopBehavior = nextLoopBehavior > maxValue ? 0 : nextLoopBehavior;

            this.atlasData.LoopingBehavior = nextLoopBehavior;
        }

        this.prevKeyboardState = currentKeyboardState;
    }

    /// <summary>
    /// Renders the text.
    /// </summary>
    private void RenderText()
    {
        const int offsetY = 15;
        var screenCenterX = (int)(WindowSize.Width / 2);
        var screenCenterY = (int)(WindowSize.Height / 2);
        var screenOneEighthHeightY = (int)(WindowSize.Height / 8);
        var nextTextPosY = 0f;

        // Render instructions
        var instructionsText = "Press space to 'start/stop' animation\n";
        instructionsText += "Press 'left/right' to change the animation direction\n";
        instructionsText += "Press 'up/down' to change the animation speed\n";
        instructionsText += "Press 'enter' to reset the animation\n";
        instructionsText += "Press 'l' to change the looping behavior";

        this.fontRenderer.Render(this.font,
            instructionsText,
            new Vector2(screenCenterX, screenCenterY - (screenOneEighthHeightY * 3)));

        // Render current frame index
        var currentFrameIndexText = $"Current Frame: {this.atlasData.CurrentFrameIndex}";
        var currentFrameIndexTextSize = this.font.Measure(currentFrameIndexText);
        nextTextPosY = (screenOneEighthHeightY * 5) + offsetY;
        this.fontRenderer.Render(
            this.font,
            currentFrameIndexText,
            new Vector2(screenCenterX, nextTextPosY));

        // Render animation speed
        var animationSpeedText = $"Animation Speed: {this.animationSpeedMs}ms";
        var animationSpeedTextSize = this.font.Measure(animationSpeedText);
        nextTextPosY += currentFrameIndexTextSize.Height + offsetY;
        this.fontRenderer.Render(
            this.font,
            animationSpeedText,
            new Vector2(screenCenterX, nextTextPosY));

        var loopingBehaviorText = $"Looping Behavior: {this.atlasData.LoopingBehavior}";
        var loopingBehaviorTextSize = this.font.Measure(loopingBehaviorText);
        nextTextPosY += animationSpeedTextSize.Height + offsetY;
        this.fontRenderer.Render(
            this.font,
            loopingBehaviorText,
            new Vector2(screenCenterX, nextTextPosY));

        var currentLoopCountText = $"Current Loop Count: {this.atlasData.CurrentLoopCount}";
        var currentLoopCountTextSize = this.font.Measure(currentLoopCountText);
        nextTextPosY += loopingBehaviorTextSize.Height + offsetY;
        this.fontRenderer.Render(
            this.font,
            currentLoopCountText,
            new Vector2(screenCenterX, nextTextPosY));

        var totalLoopsText = $"Total Loops: {this.atlasData.TotalLoops}";
        nextTextPosY += currentLoopCountTextSize.Height + offsetY;
        this.fontRenderer.Render(
            this.font,
            totalLoopsText,
            new Vector2(screenCenterX, nextTextPosY));
    }
}
