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
    /// The name of the tag.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The starting frame number for the tag.
    /// </summary>
    public int From { get; set; }

    /// <summary>
    /// The ending frame number for the tag.
    /// </summary>
    public int To { get; set; }
}
