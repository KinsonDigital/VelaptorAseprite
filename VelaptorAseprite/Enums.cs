// <copyright file="Enums.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorAseprite;

/// <summary>
/// Represents the direction of an animation.
/// </summary>
public enum AnimationDirection
{
    /// <summary>
    /// Represents an animation running forward.
    /// </summary>
    Forward,

    /// <summary>
    /// Represents an animation running backward.
    /// </summary>
    Backward,
}

/// <summary>
/// Represents the looping behavior of an animation.
/// </summary>
public enum LoopingBehavior
{
    /// <summary>
    /// Represents an animation that will loop infinitely.
    /// </summary>
    Infinite,

    /// <summary>
    /// Represents an animation that will not loop.
    /// </summary>
    None,

    /// <summary>
    /// Represents an animation that loops a finite number of times.
    /// </summary>
    Count,
}
