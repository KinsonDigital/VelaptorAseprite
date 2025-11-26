// <copyright file="ExtensionMethods.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorAsepriteTests.Helpers;

using Velaptor;

public static class ExtensionMethods
{
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
