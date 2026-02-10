// <copyright file="FrameTag.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorAseprite.Data;

/// <summary>
/// A frame tag with tag meta-data.
/// </summary>
public sealed class FrameTag
{
    /// <summary>
    /// Gets the name of the tag.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Gets the starting frame number for the tag.
    /// </summary>
    public int From { get; init; }

    /// <summary>
    /// Gets the ending frame number for the tag.
    /// </summary>
    public int To { get; init; }
}
