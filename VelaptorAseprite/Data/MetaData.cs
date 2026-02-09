// <copyright file="MetaData.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorAseprite.Data;

using System.Drawing;
using System.Text.Json.Serialization;

// ReSharper disable UnusedAutoPropertyAccessor.Global

/// <summary>
/// Represents metadata information exported from an Aseprite sprite sheet atlas.
/// </summary>
public sealed class MetaData
{
    /// <summary>
    /// Gets or sets the name of the application that exported the atlas data.
    /// </summary>
    public required string App { get; set; }

    /// <summary>
    /// Gets or sets the version of the application that exported the atlas data.
    /// </summary>
    [JsonPropertyName("version")]
    public required string Version { get; set; }

    /// <summary>
    /// Gets or sets the file name of the sprite sheet image.
    /// </summary>
    [JsonPropertyName("image")]
    public required string ImageFileName { get; set; }

    /// <summary>
    /// Gets or sets the pixel format of the sprite sheet image.
    /// </summary>
    public required string Format { get; set; }

    /// <summary>
    /// Gets or sets the total size of the sprite sheet image in pixels.
    /// </summary>
    [JsonConverter(typeof(SizeJsonConverter))]
    public Size Size { get; set; }

    /// <summary>
    /// Gets or sets the scale factor applied to the sprite sheet during export.
    /// </summary>
    public required string Scale { get; set; }

    /// <summary>
    /// Gets the frame tag meta-data.
    /// </summary>
    [JsonPropertyName("frameTags")]
    public FrameTag[] Tags { get; init; } = [];
}
