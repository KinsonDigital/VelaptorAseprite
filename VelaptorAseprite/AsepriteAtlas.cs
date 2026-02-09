// <copyright file="AsepriteAtlas.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorAseprite;

using Velaptor;
using Velaptor.Content;
using System.Text.Json.Serialization;
using Data;

/// <inheritdoc/>
internal class AsepriteAtlas : IAsepriteAtlas
{
    private int currentFrameElapsedMs;
    private LoopingBehavior loopingBehavior = LoopingBehavior.Infinite;
    private int startingIndex;
    private int endingIndex;
    private string? animationNameValue;

    /// <inheritdoc/>
    public ITexture Texture { get; internal set; } = null!;

    /// <inheritdoc/>
    public string Name { get; internal set; } = null!;

    /// <inheritdoc/>
    public string FilePath { get; internal set; } = null!;

    /// <inheritdoc/>
    [JsonConverter(typeof(FramesJsonConverter))]
    public Dictionary<int, AnimationFrame> Frames { get; set; } = [];

    /// <inheritdoc/>
    [JsonInclude]
    public MetaData Meta { get; internal set; } = null!;

    /// <inheritdoc/>
    public bool Enabled { get; set; } = true;

    /// <inheritdoc/>
    public bool IsAnimating { get; internal set; }

    /// <inheritdoc/>
    public int CurrentFrameIndex { get; internal set; }

    /// <inheritdoc/>
    public LoopingBehavior LoopingBehavior
    {
        get => this.loopingBehavior;
        set
        {
            // If we are moving from infinite to a finite looping behavior,
            // reset the loop count so the looping does not stop immediately
            // if it was previously set to 'Count' and the current loop count
            // was >= the max loops.
            if (this.loopingBehavior == LoopingBehavior.Infinite && value == LoopingBehavior.Count)
            {
                CurrentLoopCount = 0;
            }

            this.loopingBehavior = value;
        }
    }

    /// <inheritdoc/>
    public string? AnimationName
    {
        get => this.animationNameValue;
        set
        {
            this.animationNameValue = value;

            SetIndexRange(this.animationNameValue);
        }
    }

    /// <inheritdoc/>
    public uint CurrentLoopCount { get; internal set; }

    /// <inheritdoc/>
    public uint MaxLoops { get; set; }

    /// <inheritdoc/>
    public uint TotalLoops { get; set; }

    /// <inheritdoc/>
    public AnimationDirection Direction { get; set; }

    /// <inheritdoc/>
    public AnimationFrame GetCurrentFrame() => Frames[CurrentFrameIndex];

    /// <inheritdoc/>
    public void SetAnimationSpeed(int timeMs)
    {
        foreach (var frame in Frames)
        {
            frame.Value.Duration = timeMs;
            Frames[frame.Key] = frame.Value;
        }
    }

    /// <inheritdoc/>
    public void Play(string? animationName = null)
    {
        IsAnimating = true;

        if (animationName is not null)
        {
            AnimationName = animationName;
            SetIndexRange(AnimationName);
        }

        if (LoopingBehavior == LoopingBehavior.Count)
        {
            CurrentLoopCount = 0;
        }
    }

    /// <inheritdoc/>
    public void Stop() => IsAnimating = false;

    /// <inheritdoc/>
    public void Reset()
    {
        CurrentFrameIndex = 0;
        this.currentFrameElapsedMs = 0;
        IsAnimating = false;
    }

    /// <inheritdoc/>
    public void Update(FrameTime frameTime)
    {
        if (!Enabled || !IsAnimating)
        {
            return;
        }

        var currentFrameDuration = Frames[CurrentFrameIndex].Duration;

        if (this.currentFrameElapsedMs >= currentFrameDuration)
        {
            this.currentFrameElapsedMs = 0;
            var cycleComplete = IsCycleComplete();

            if (cycleComplete)
            {
                TotalLoops += 1;
            }

            CurrentFrameIndex = CalcNextIndex(cycleComplete);

            if (cycleComplete && LoopingBehavior == LoopingBehavior.Count)
            {
                CurrentLoopCount += 1;
                IsAnimating = CurrentLoopCount < MaxLoops;
            }
            else if (LoopingBehavior == LoopingBehavior.None)
            {
                if (cycleComplete)
                {
                    IsAnimating = false;
                }
            }
        }
        else
        {
            this.currentFrameElapsedMs += (int)frameTime.ElapsedTime.TotalMilliseconds;
        }
    }

    /// <summary>
    /// Sets the starting and ending index based on the animation name.
    /// </summary>
    /// <param name="animationName">The name of the animation to play.</param>
    /// <exception cref="Exception">Thrown when the name of the animation does not exist.</exception>
    private void SetIndexRange(string? animationName)
    {
        if (animationName is not null)
        {
            if (Meta.Tags.Length <= 0 || Meta.Tags.All(x => x.Name != animationName))
            {
                throw new NoTagException($"No Aseprite tag name exists that matches the animation of '{animationName}'.");
            }

            // Set the starting and ending index
            this.startingIndex = Meta.Tags.Single(x => x.Name == animationName).From;
            this.endingIndex = Meta.Tags.Single(x => x.Name == animationName).To;
        }
        else
        {
            this.startingIndex = 0;
            this.endingIndex = Frames.Count - 1;
        }

        CurrentFrameIndex = this.startingIndex;
    }

    /// <summary>
    /// Returns a value indicating whether the animation cycle is complete.
    /// </summary>
    /// <returns><c>True</c>, if the animation cycle is complete; otherwise <c>false</c>.</returns>
    private bool IsCycleComplete()
    {
        if (AnimationName is null)
        {
            return Direction == AnimationDirection.Forward
                ? CurrentFrameIndex >= Frames.Count - 1
                : CurrentFrameIndex <= 0;
        }

        return Direction == AnimationDirection.Forward
            ? CurrentFrameIndex >= this.endingIndex
            : CurrentFrameIndex <= this.startingIndex;
    }

    /// <summary>
    /// Calculates the next index to use based on the direction and whether the cycle is complete.
    /// </summary>
    /// <param name="cycleComplete">A value indicating whether the animation cycle is complete.</param>
    /// <returns>The next frame index.</returns>
    private int CalcNextIndex(bool cycleComplete)
    {
        if (AnimationName is null)
        {
            return Direction switch
            {
                AnimationDirection.Forward => cycleComplete ? 0 : CurrentFrameIndex + 1,
                AnimationDirection.Backward => cycleComplete ? Frames.Count - 1 : CurrentFrameIndex - 1,
                _ => throw new Exception($"Invalid direction value of '{Direction.ToString()}'.")
            };
        }

        return Direction switch
        {
            AnimationDirection.Forward => cycleComplete ? this.startingIndex : CurrentFrameIndex + 1,
            AnimationDirection.Backward => cycleComplete ? this.endingIndex : CurrentFrameIndex - 1,
            _ => throw new Exception($"Invalid direction value of '{Direction.ToString()}'.")
        };
    }
}
