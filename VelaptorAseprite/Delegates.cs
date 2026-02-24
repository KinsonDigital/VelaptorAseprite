// <copyright file="Delegates.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorAseprite;

/// <summary>
/// Invoked when an animation cycle completes.
/// </summary>
/// <remarks>
///     If the <paramref name="animationName"/> is empty, then the animation running is
///     every single frame in the atlas.
/// </remarks>
/// <param name="animationName">The current name of the running animation that completed its cycle.</param>
public delegate void OnCycleComplete(string animationName);

/// <summary>
/// Invoked when a frame changes.
/// </summary>
/// <param name="previousFrameIndex">The previous frame index.</param>
/// <param name="currentFrameIndex">The current frame index.</param>
public delegate void OnFrameChange(int previousFrameIndex, int currentFrameIndex);
