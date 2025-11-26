// <copyright file="AsepriteAtlasData.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorAseprite;

using Velaptor;
using Velaptor.Content;
using System.Text.Json.Serialization;
using Data;

/// <inheritdoc/>
internal class AsepriteAtlasData : IAsepriteAtlasData
{
    private int currentFrameElapsedMs;
    private LoopingBehavior loopingBehavior = LoopingBehavior.Infinite;

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
    public void Play()
    {
        IsAnimating = true;

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
            var cycleComplete = Direction == AnimationDirection.Forward
                ? CurrentFrameIndex >= Frames.Count - 1
                : CurrentFrameIndex <= 0;

            if (cycleComplete)
            {
                TotalLoops += 1;
            }

            CurrentFrameIndex = Direction switch
            {
                AnimationDirection.Forward => cycleComplete ? 0 : CurrentFrameIndex + 1,
                AnimationDirection.Backward => cycleComplete ? Frames.Count - 1 : CurrentFrameIndex - 1,
                _ => CurrentFrameIndex
            };

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
}
