// <copyright file="MultipleScene.cs" company="KinsonDigital">
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
public class MultipleScene : SceneBase
{
    private const string FontName = $"TimesNewRoman-{nameof(FontStyle.Regular)}.ttf";
    private const float RenderScale = 3f;
    private const string FireAnimation = "fire";
    private const string SparkAnimation = "spark";
    private readonly IContentManager contentManager;
    private readonly ITextureRenderer textureRenderer;
    private readonly ITextRenderer textRenderer;
    private readonly IAppInput<KeyboardState> keyboard;
    private IAsepriteAtlas? atlasData;
    private IFont? font;
    private KeyboardState prevKeyboardState;
    private string currentAnimation = FireAnimation;
    private float velocityY = 100f;
    private bool movingDown = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="MultipleScene"/> class.
    /// </summary>
    public  MultipleScene()
    {
        this.textureRenderer = RendererFactory.CreateTextureRenderer();
        this.textRenderer = RendererFactory.CreateTextRenderer();
        this.keyboard = HardwareFactory.GetKeyboard();

        this.contentManager = ContentManager.Create();
    }

    /// <inheritdoc cref="SceneBase" />
    public override void LoadContent()
    {
        this.atlasData = this.contentManager.LoadAsepriteAtlas(FireAnimation, SparkAnimation);
        this.atlasData.LoopingBehavior = LoopingBehavior.Infinite;

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

        var wasAnimating = this.atlasData.IsAnimating;

        this.atlasData.Update(frameTime);

        // Check if animation just finished (was enabled, now disabled)
        if (wasAnimating && !this.atlasData.IsAnimating)
        {
            // Animation just finished, start moving up
            this.velocityY = -Math.Abs(this.velocityY);
            this.movingDown = false;
        }

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
            RenderScale,
            0f,
            Color.White,
            RenderEffects.None);

        RenderText();

        base.Render();
    }

    private void RenderText()
    {
        const int verticalSpacing = 25;
        var winWidth = (int)WindowSize.Width;

        var screenCenterX = (int)(WindowSize.Width / 2);
        var screenBottomY = (int)WindowSize.Height;

        var currentFrameText = $"Current Frame: {this.atlasData.CurrentFrameIndex}";
        var currentFrameTextSize = this.font.Measure(currentFrameText);
        var currentFrameTextPosY = screenBottomY - (int)(currentFrameTextSize.Height / 2f) - verticalSpacing;

        var movingDownText = $"Moving Down: {this.movingDown}";
        var movingDownTextSize = this.font.Measure(movingDownText);
        var movingDownTextPosY = currentFrameTextPosY - (int)(movingDownTextSize.Height / 2f) - verticalSpacing;

        var animationEnabledText = $"Animation Enabled: {this.atlasData.IsAnimating}";
        var animationEnabledTextSize = this.font.Measure(animationEnabledText);
        var animationTextPosY = movingDownTextPosY - (int)(animationEnabledTextSize.Height / 2f) - verticalSpacing;

        var velocityText = $"Velocity: {this.velocityY}";
        var velocityTextSize = this.font.Measure(velocityText);
        var velocityTextPosY = animationTextPosY - (int)(animationEnabledTextSize.Height / 2f) - verticalSpacing;

        var animationName = $"Animation Name: {this.currentAnimation}";
        var animationNameSize = this.font.Measure(animationName);
        var animationNamePosY = velocityTextPosY - (int)(animationNameSize.Height / 2f) - verticalSpacing;

        var instructionWidths = new[]
        {
            (int)animationNameSize.Width,
            (int)velocityTextSize.Width, (int)animationEnabledTextSize.Width,
            (int)movingDownTextSize.Width, (int)currentFrameTextSize.Width,
        };

        var largestWidth = instructionWidths.Max();

        // Render instructions
        var instructionsText = "Press space to 'start/stop' animation\n";
        instructionsText += "Press 'up' to speed up movement velocity\n";
        instructionsText += "Press 'down' to slow down movement velocity\n";
        instructionsText += "Press 'left/right' to change animations";
        var instructionsTextSize = this.font.Measure(instructionsText);

        this.textRenderer.Render(this.font,
            instructionsText,
            new Vector2(screenCenterX, (instructionsTextSize.Height / 2f) + verticalSpacing));

        this.textRenderer.Render(
            this.font,
            animationName,
            winWidth - ((int)animationNameSize.Width / 2) - largestWidth,
            animationNamePosY);
        this.textRenderer.Render(
            this.font,
            velocityText,
            winWidth - ((int)velocityTextSize.Width / 2) - largestWidth,
            velocityTextPosY);
        this.textRenderer.Render(
            this.font,
            animationEnabledText,
            winWidth - ((int)animationEnabledTextSize.Width / 2) - largestWidth,
            animationTextPosY);
        this.textRenderer.Render(
            this.font,
            movingDownText,
            winWidth - ((int)movingDownTextSize.Width / 2) - largestWidth,
            movingDownTextPosY);
        this.textRenderer.Render(
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
            this.atlasData.LoopingBehavior = this.currentAnimation == FireAnimation
                ? LoopingBehavior.Infinite
                : LoopingBehavior.None;

            if (this.atlasData.IsAnimating)
            {
                this.atlasData.Stop();
            }
            else
            {
                this.atlasData.Play(this.currentAnimation);
            }
        }

        if (currentKeyboardState.IsKeyUp(KeyCode.Left) && this.prevKeyboardState.IsKeyDown(KeyCode.Left))
        {
            SwitchAnimation();
        }

        if (currentKeyboardState.IsKeyUp(KeyCode.Right) && this.prevKeyboardState.IsKeyDown(KeyCode.Right))
        {
            SwitchAnimation();
        }

        this.prevKeyboardState = currentKeyboardState;
    }

    /// <summary>
    /// Switches the animation being played.
    /// </summary>
    private void SwitchAnimation()
    {
        this.currentAnimation = this.currentAnimation switch
        {
            FireAnimation => SparkAnimation,
            SparkAnimation => FireAnimation,
            _ => this.atlasData.AnimationName
        };

        this.atlasData.Play(this.currentAnimation);
    }
}
