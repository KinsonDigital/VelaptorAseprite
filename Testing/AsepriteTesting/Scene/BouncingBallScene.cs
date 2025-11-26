// <copyright file="BouncingBallScene.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace AsepriteTesting.Scene;

using Velaptor;
using Velaptor.Scene;
using System.Drawing;
using System.Numerics;
using System.Linq;
using Velaptor.Content;
using Velaptor.Content.Fonts;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.Graphics.Renderers;
using Velaptor.Input;
using VelaptorAseprite;
using VelaptorAseprite.Data;

/// <summary>
/// Used to test out simple animation behaviors, direction, speed, and more.
/// </summary>
public class BouncingBallScene : SceneBase
{
    private const string FontName = $"TimesNewRoman-{nameof(FontStyle.Regular)}.ttf";
    private const float RenderScale = 3f;
    private const float MaxBallHeight = 350f;
    private readonly IContentManager contentManager;
    private readonly ITextureRenderer textureRenderer;
    private readonly IFontRenderer fontRenderer;
    private readonly ILineRenderer lineRenderer;
    private readonly IAppInput<KeyboardState> keyboard;
    private IAsepriteAtlasData? atlasData;
    private IFont? font;
    private KeyboardState prevKeyboardState;
    private float velocityY = 100f;
    private float ballPosY = MaxBallHeight + 50;
    private float maxBallHeight;
    private bool movingDown = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="BouncingBallScene"/> class.
    /// </summary>
    public  BouncingBallScene()
    {
        this.textureRenderer = RendererFactory.CreateTextureRenderer();
        this.lineRenderer = RendererFactory.CreateLineRenderer();
        this.fontRenderer = RendererFactory.CreateFontRenderer();
        this.keyboard = HardwareFactory.GetKeyboard();

        this.contentManager = ContentManager.Create();
    }

    /// <inheritdoc cref="SceneBase" />
    public override void LoadContent()
    {
        this.atlasData = this.contentManager.LoadAsepriteAtlasData("bouncing-ball-squish");
        this.atlasData.LoopingBehavior = LoopingBehavior.None;
        this.maxBallHeight = this.atlasData.Frames.Max(i => i.Value.Bounds.Height * RenderScale);

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
        ProcessInput();

        var ballHeight = this.maxBallHeight;
        var ballHalfHeight = ballHeight / 2f;
        var ballTop = this.ballPosY - ballHalfHeight;
        var ballBottom = this.ballPosY + ballHalfHeight;

        var wasAnimating = this.atlasData.IsAnimating;

        this.atlasData!.Update(frameTime);

        // Check if animation just finished (was enabled, now disabled)
        if (wasAnimating && !this.atlasData.IsAnimating)
        {
            // Animation just finished, start moving up
            this.velocityY = -Math.Abs(this.velocityY);
            this.movingDown = false;
        }

        var winHeight = WindowSize.Height;

        // Start the squish animation when the ball hits the bottom of the screen
        if (ballBottom >= winHeight && !this.atlasData.IsAnimating && this.movingDown)
        {
            this.atlasData.Play();
        }

        // If animating, keep the ball positioned at the bottom based on current frame height
        if (this.atlasData.IsAnimating)
        {
            var currentFrameBounds = this.atlasData.GetCurrentFrame().Bounds;
            this.ballPosY = winHeight - (currentFrameBounds.Height / 2f * RenderScale);
        }
        else
        {
            // If the ball reaches the top bounce point, make it move down
            if (ballTop < MaxBallHeight && !this.movingDown)
            {
                this.movingDown = true;
                this.velocityY = Math.Abs(this.velocityY);
            }

            // Move the ball using a velocity in pixels/sec scaled by the elapsed seconds
            var delta = this.velocityY * (float)frameTime.ElapsedTime.TotalSeconds;
            this.ballPosY += this.atlasData.Enabled ? delta : 0;
        }

        base.Update(frameTime);
    }

    /// <inheritdoc cref="SceneBase" />
    public override void Render()
    {
        var frame = this.atlasData!.GetCurrentFrame();

        var srcRect = frame.Bounds;
        var destRect = new Rectangle(0, 0, (int)this.atlasData!.Texture.Width, (int)this.atlasData!.Texture.Height);
        destRect.X = 400;
        destRect.Y = (int)this.ballPosY;

        this.textureRenderer.Render(
            this.atlasData!.Texture,
            srcRect,
            destRect,
            RenderScale,
            0f,
            Color.White,
            RenderEffects.None);

        const int verticalSpacing = 15;
        var winWidth = (int)WindowSize.Width;
        var line = new Line(new Vector2(0, MaxBallHeight), new Vector2(winWidth, MaxBallHeight), Color.CornflowerBlue);
        this.lineRenderer.Render(line);

        var screenCenterX = (int)(WindowSize.Width / 2);
        var screenCenterY = (int)(WindowSize.Height / 2);
        var screenOneEighthHeightY = (int)(WindowSize.Height / 8);

        var velocityText = $"Velocity: {this.velocityY}";
        var velocityTextSize = this.font.Measure(velocityText);
        var velocityTextPosY = screenOneEighthHeightY * 5;

        var animationEnabledText = $"Animation Enabled: {this.atlasData.IsAnimating}";
        var animationEnabledTextSize = this.font.Measure(animationEnabledText);
        var animationTextPosY = velocityTextPosY + (int)velocityTextSize.Height + verticalSpacing;

        var movingDownText = $"Moving Down: {this.movingDown}";
        var movingDownTextSize = this.font.Measure(movingDownText);
        var movingDownTextPosY = animationTextPosY + (int)animationEnabledTextSize.Height + verticalSpacing;

        var currentFrameText = $"Current Frame: {this.atlasData.CurrentFrameIndex}";
        var currentFrameTextSize = this.font.Measure(currentFrameText);
        var currentFrameTextPosY = movingDownTextPosY + (int)movingDownTextSize.Height + verticalSpacing;

        var instructionWidths = new[]
        {
            (int)velocityTextSize.Width, (int)animationEnabledTextSize.Width, (int)movingDownTextSize.Width, (int)currentFrameTextSize.Width,
        };

        var largestWidth = instructionWidths.Max();

        // Render instructions
        var instructionsText = "Press space to 'start/stop' animation\n";
        instructionsText += "Press 'up' to speed up movement velocity\n";
        instructionsText += "Press 'up' to slow down movement velocity\n";

        this.fontRenderer.Render(this.font,
            instructionsText,
            new Vector2(screenCenterX, screenCenterY - (screenOneEighthHeightY * 3)));

        this.fontRenderer.Render(
            this.font,
            velocityText,
            winWidth - ((int)velocityTextSize.Width / 2) - largestWidth,
            velocityTextPosY);
        this.fontRenderer.Render(
            this.font,
            animationEnabledText,
            winWidth - ((int)animationEnabledTextSize.Width / 2) - largestWidth,
            animationTextPosY);
        this.fontRenderer.Render(
            this.font,
            movingDownText,
            winWidth - ((int)movingDownTextSize.Width / 2) - largestWidth,
            movingDownTextPosY);
        this.fontRenderer.Render(
            this.font,
            currentFrameText,
            winWidth - ((int)currentFrameTextSize.Width / 2) - largestWidth,
            currentFrameTextPosY);

        base.Render();
    }

    /// <summary>
    /// Processes keyboard input.
    /// </summary>
    private void ProcessInput()
    {
        var currentKeyboardState = this.keyboard.GetState();

        if (currentKeyboardState.IsKeyUp(KeyCode.Space) && this.prevKeyboardState.IsKeyDown(KeyCode.Space))
        {
            this.atlasData.Enabled = !this.atlasData.Enabled;
        }

        if (currentKeyboardState.IsKeyUp(KeyCode.Up) && this.prevKeyboardState.IsKeyDown(KeyCode.Up))
        {
            this.velocityY = this.velocityY < 0
                ? this.velocityY + -5
                : this.velocityY + 5;
        }

        if (currentKeyboardState.IsKeyUp(KeyCode.Down) && this.prevKeyboardState.IsKeyDown(KeyCode.Down))
        {
            this.velocityY = this.velocityY < 0
                ? this.velocityY - -5
                : this.velocityY - 5;
        }

        this.prevKeyboardState = currentKeyboardState;
    }
}
