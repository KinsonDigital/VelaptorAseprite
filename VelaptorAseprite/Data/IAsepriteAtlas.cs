// <copyright file="IAsepriteAtlas.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorAseprite.Data;

using Velaptor;
using Velaptor.Content;

/// <summary>
/// Represents atlas data exported from Aseprite, providing animation playback capabilities
/// for sprite sheet textures.
/// </summary>
public interface IAsepriteAtlas : IContent, IUpdatable
{
    /// <summary>
    /// Gets the texture containing the sprite sheet atlas image.
    /// </summary>
    ITexture Texture { get; }

    /// <summary>
    /// Gets or sets the collection of animation frames, keyed by frame index.
    /// </summary>
    /// <remarks>The index is 0-based.</remarks>
    Dictionary<int, AnimationFrame> Frames { get; set; }

    /// <summary>
    /// Gets the metadata associated with the atlas, including image dimensions and format information.
    /// </summary>
    MetaData Meta { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the animation updates are enabled.
    /// </summary>
    /// <remarks>
    /// When set to <c>false</c>, the animation will not progress during <see cref="IUpdatable.Update"/> calls.
    /// </remarks>
    bool Enabled { get; set; }

    /// <summary>
    /// Gets a value indicating whether the animation is currently playing.
    /// </summary>
    bool IsAnimating { get; }

    /// <summary>
    /// Gets the index of the current animation frame being displayed.
    /// </summary>
    uint CurrentFrameIndex { get; }

    /// <summary>
    /// Gets the total number of frames that have ever been run.
    /// </summary>
    uint TotalFramesRan { get; }

    /// <summary>
    /// Gets or sets the looping behavior of the animation.
    /// </summary>
    /// <remarks>
    /// Use <see cref="LoopingBehavior.Infinite"/> for continuous looping,
    /// <see cref="LoopingBehavior.None"/> for single playthrough,
    /// or <see cref="LoopingBehavior.Count"/> to loop a specific number of times set by <see cref="MaxLoops"/>.
    /// </remarks>
    LoopingBehavior LoopingBehavior { get; set; }

    /// <summary>
    /// Gets the name of the currently playing animation.
    /// <remarks>
    /// If the value is <c>string.IsNullOrEmpty</c>, then the entire animation will play.
    /// </remarks>
    /// </summary>
    string AnimationName { get; }

    /// <summary>
    /// Gets the number of times the animation has looped since playback started.
    /// </summary>
    /// <remarks>
    /// This value is reset when <see cref="Play"/> is called with <see cref="LoopingBehavior"/> set to <see cref="VelaptorAseprite.LoopingBehavior.Count"/>.
    /// </remarks>
    uint CurrentLoopCount { get; }

    /// <summary>
    /// Gets or sets the maximum number of loops when <see cref="LoopingBehavior"/> is set to <see cref="LoopingBehavior.Count"/>.
    /// </summary>
    /// <remarks>
    /// The animation will stop after completing this many loops.
    /// </remarks>
    uint MaxLoops { get; set; }

    /// <summary>
    /// Gets the total number of times the animation has looped since the atlas was loaded.
    /// </summary>
    uint TotalLoops { get; }

    /// <summary>
    /// Gets or sets the direction of the animation playback.
    /// </summary>
    /// <remarks>
    /// Use <see cref="AnimationDirection.Forward"/> to play from first to last frame,
    /// or <see cref="AnimationDirection.Backward"/> to play from last to first frame.
    /// </remarks>
    AnimationDirection Direction { get; set; }

    /// <summary>
    /// Gets the current frame's animation data.
    /// </summary>
    /// <returns>The <see cref="AnimationFrame"/> at the current frame index.</returns>
    AnimationFrame GetCurrentFrame();

    /// <summary>
    /// Sets the duration of all animation frames to the specified time.
    /// </summary>
    /// <param name="timeMs">The duration in milliseconds for each frame.</param>
    void SetAnimationSpeed(int timeMs);

    /// <summary>
    /// Starts or resumes the animation playback for a specific animation by the given <paramref name="animationName"/>.
    /// </summary>
    /// <param name="animationName">The name of the animation to play.</param>
    /// <remarks>
    /// When <see cref="LoopingBehavior"/> is set to <see cref="VelaptorAseprite.LoopingBehavior.Count"/>,
    /// calling this method resets the <see cref="CurrentLoopCount"/> to zero.
    /// <br />
    /// If no <paramref name="animationName"/> is provided, then all frames will be played regardless of the animation.
    /// </remarks>
    void Play(string? animationName = null);

    /// <summary>
    /// Stops the animation playback at the current frame.
    /// </summary>
    /// <remarks>
    /// The animation can be resumed by calling <see cref="Play"/>.
    /// To restart from the beginning, call <see cref="Reset"/> first.
    /// </remarks>
    void Stop();

    /// <summary>
    /// Resets the animation to its initial state.
    /// </summary>
    /// <remarks>
    /// This sets the current frame index to 0, clears elapsed time, and stops the animation.
    /// Call <see cref="Play"/> to start the animation from the beginning.
    /// </remarks>
    void Reset();
}
