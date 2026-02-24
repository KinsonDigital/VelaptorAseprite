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
    private uint currentFrameElapsedMs;
    private LoopingBehavior loopingBehavior = LoopingBehavior.Infinite;
    private uint startingIndex;
    private uint endingIndex;
    private uint prevFrameIndex;
    private string animationName = string.Empty;

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
    public uint CurrentFrameIndex { get; internal set; }

    /// <inheritdoc/>
    public uint TotalFramesRan { get; private set; }

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
    public string AnimationName
    {
        get => this.animationName;
        internal set => this.animationName = string.IsNullOrEmpty(value) ? string.Empty : value;
    }

    /// <inheritdoc/>
    public uint CurrentLoopCount { get; internal set; }

    /// <inheritdoc/>
    public uint MaxLoops { get; set; }

    /// <inheritdoc/>
    public uint TotalLoops { get; private set; }

    /// <inheritdoc/>
    public AnimationDirection Direction { get; set; }

    /// <inheritdoc/>
    public OnCycleComplete? OnCycleComplete { get; set; }

    /// <inheritdoc/>
    public OnFrameChange? OnFrameChange { get; set; }

    /// <inheritdoc/>
    public AnimationFrame GetCurrentFrame() => Frames[(int)CurrentFrameIndex];

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
    public void Play(string? nameOfAnimation = null)
    {
        IsAnimating = true;

        AnimationName = string.IsNullOrEmpty(nameOfAnimation) ? string.Empty : nameOfAnimation;
        SetIndexRange(AnimationName);
        CurrentFrameIndex = CalcNextIndex(IsCycleComplete());

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

        var currentFrameDuration = Frames[(int)CurrentFrameIndex].Duration;

        if (this.currentFrameElapsedMs >= currentFrameDuration)
        {
            this.currentFrameElapsedMs = 0;
            TotalFramesRan += 1;

            var cycleComplete = IsCycleComplete();

            if (cycleComplete)
            {
                TotalLoops += 1;

                // If the on cycle complete event handler is set, invoke it
                OnCycleComplete?.Invoke(AnimationName);
            }

            this.prevFrameIndex = CurrentFrameIndex;
            CurrentFrameIndex = CalcNextIndex(cycleComplete);

            OnFrameChange?.Invoke((int)this.prevFrameIndex, (int)CurrentFrameIndex);

            if (cycleComplete && LoopingBehavior == LoopingBehavior.Count)
            {
                CurrentLoopCount += 1;
                IsAnimating = CurrentLoopCount < MaxLoops;
            }
            else if (LoopingBehavior == LoopingBehavior.None && cycleComplete)
            {
                IsAnimating = false;
            }
        }
        else
        {
            this.currentFrameElapsedMs += (uint)frameTime.ElapsedTime.TotalMilliseconds;
        }
    }

    /// <summary>
    /// Sets the starting and ending index based on the animation name.
    /// </summary>
    /// <param name="nameOfAnimation">The name of the animation to play.</param>
    /// <exception cref="Exception">Thrown when the name of the animation does not exist.</exception>
    private void SetIndexRange(string? nameOfAnimation)
    {
        if (string.IsNullOrEmpty(nameOfAnimation))
        {
            this.startingIndex = 0;
            this.endingIndex = (uint)(Frames.Count - 1u);
        }
        else
        {
            if (Meta.Tags.Length <= 0 || Meta.Tags.All(x => x.Name != nameOfAnimation))
            {
                throw new NoTagException($"No Aseprite tag name exists that matches the animation of '{nameOfAnimation}'.");
            }

            // Set the starting and ending index
            this.startingIndex = (uint)Meta.Tags.Single(x => x.Name == nameOfAnimation).From;
            this.endingIndex = (uint)Meta.Tags.Single(x => x.Name == nameOfAnimation).To;
        }
    }

    /// <summary>
    /// Returns a value indicating whether the animation cycle is complete.
    /// </summary>
    /// <returns><c>True</c>, if the animation cycle is complete; otherwise <c>false</c>.</returns>
    private bool IsCycleComplete()
    {
        if (string.IsNullOrEmpty(AnimationName))
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
    private uint CalcNextIndex(bool cycleComplete)
    {
        if (string.IsNullOrEmpty(AnimationName))
        {
            return Direction switch
            {
                AnimationDirection.Forward => cycleComplete ? 0 : CurrentFrameIndex + 1,
                AnimationDirection.Backward => cycleComplete ? (uint)(Frames.Count - 1u) : CurrentFrameIndex - 1,
                _ => throw new InvalidOperationException($"Invalid direction value of '{Direction}'.")
            };
        }

        return Direction switch
        {
            AnimationDirection.Forward => cycleComplete ? this.startingIndex : CurrentFrameIndex + 1,
            AnimationDirection.Backward => cycleComplete ? this.endingIndex : CurrentFrameIndex - 1,
            _ => throw new InvalidOperationException($"Invalid direction value of '{Direction}'.")
        };
    }
}
