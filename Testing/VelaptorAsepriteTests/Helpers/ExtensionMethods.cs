// <copyright file="ExtensionMethods.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorAsepriteTests.Helpers;

using Velaptor;

/// <summary>
/// Provides extension methods for various types.
/// </summary>
public static class ExtensionMethods
{
    /// <summary>
    /// Sets the milliseconds portion of the <see cref="FrameTime"/> to the specified value.
    /// </summary>
    /// <param name="value">The original <see cref="FrameTime"/> value.</param>
    /// <param name="ms">The milliseconds to set.</param>
    /// <returns>A new <see cref="FrameTime"/> with the updated milliseconds.</returns>
    public static FrameTime SetMs(this FrameTime value, int ms)
    {
        return new FrameTime
        {
            ElapsedTime = new TimeSpan(
                value.TotalTime.Days,
                value.TotalTime.Hours,
                value.TotalTime.Minutes,
                value.TotalTime.Seconds,
                ms),
        };
    }
}
