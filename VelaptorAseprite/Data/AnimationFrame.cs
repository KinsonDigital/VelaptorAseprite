// <copyright file="AnimationFrame.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorAseprite.Data;

using System.Drawing;
using System.Text.Json.Serialization;

/// <summary>
/// Represents a single frame of exported animation.
/// </summary>
public sealed class AnimationFrame
{
    /// <summary>
    /// Gets the bounding rectangle of the frame within the sprite sheet atlas.
    /// </summary>
    [JsonConverter(typeof(RectangleJsonConverter))]
    [JsonPropertyName("frame")]
    public Rectangle Bounds { get; init; }

    /// <summary>
    /// Gets a value indicating whether the frame is rotated in the sprite sheet.
    /// </summary>
    public bool Rotated { get; init; }

    /// <summary>
    /// Gets a value indicating whether the frame has been trimmed of transparent pixels.
    /// </summary>
    public bool Trimmed { get; init; }

    /// <summary>
    /// Gets the position and size of the trimmed sprite within the original source frame.
    /// </summary>
    [JsonConverter(typeof(RectangleJsonConverter))]
    public Rectangle SpriteSourceSize { get; init; }

    /// <summary>
    /// Gets the original size of the frame.
    /// </summary>
    [JsonConverter(typeof(SizeJsonConverter))]
    public Size SourceSize { get; init; }

    /// <summary>
    /// Gets or sets the display duration of the frame in milliseconds.
    /// </summary>
    public int Duration { get; set; }
}
