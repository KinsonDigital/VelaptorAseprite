// <copyright file="Game.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace AsepriteTesting;

using Scene;
using Velaptor;
using Velaptor.Factories;
using Velaptor.Input;
using Velaptor.UI;

/// <summary>
/// The main game class.
/// </summary>
public class Game : Window
{
    private readonly IAppInput<KeyboardState> keyboard;
    private KeyboardState prevKeyboardState;

    /// <summary>
    /// Initializes a new instance of the <see cref="Game"/> class.
    /// </summary>
    public Game()
    {
        this.keyboard = HardwareFactory.GetKeyboard();

        // var bouncingBallScene = new BouncingBallScene { Name = "Bouncing Ball" };
        // var compassScene = new CompassScene { Name = "Compass" };
        // var multipleScene = new MultipleScene { Name = "Multiple Animations" };
        var fireScene = new FireScene { Name = "Fire" };

        // SceneManager.AddScene(bouncingBallScene, true);
        // SceneManager.AddScene(compassScene);
        // SceneManager.AddScene(multipleScene);
        SceneManager.AddScene(fireScene);
    }

    /// <summary>
    /// Add game logic here.
    /// </summary>
    /// <param name="frameTime">The amount of time that passed for the current game loop frame.</param>
    protected override void OnUpdate(FrameTime frameTime)
    {
        var currentKeyboardState = this.keyboard.GetState();

        if (currentKeyboardState.IsKeyUp(KeyCode.PageDown) && this.prevKeyboardState.IsKeyDown(KeyCode.PageDown))
        {
            SceneManager.NextScene();
            Title = SceneManager.CurrentScene.Name;
        }

        if (currentKeyboardState.IsKeyUp(KeyCode.PageUp) && this.prevKeyboardState.IsKeyDown(KeyCode.PageUp))
        {
            SceneManager.PreviousScene();
            Title = SceneManager.CurrentScene.Name;
        }

        this.prevKeyboardState = currentKeyboardState;

        base.OnUpdate(frameTime);
    }
}
