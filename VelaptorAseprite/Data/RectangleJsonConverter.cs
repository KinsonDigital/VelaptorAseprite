// <copyright file="RectangleJsonConverter.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorAseprite.Data;

using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// Parses the 'spriteSourceSize' and 'frame' JSON properties from Aseprite and creates <see cref="Rectangle"/>'s.
/// </summary>
[ExcludeFromCodeCoverage(Justification = $"Cannot test due to direct interaction with '{nameof(JsonConverter<Rectangle>)}'.")]
internal class RectangleJsonConverter : JsonConverter<Rectangle>
{
    /// <summary>
    /// Read and convert the JSON to <see cref="Rectangle"/>.
    /// </summary>
    /// <remarks>
    /// A converter may throw any Exception, but should throw <cref>JsonException</cref> when the JSON is invalid.
    /// </remarks>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to read from.</param>
    /// <param name="typeToConvert">The <see cref="System.Type"/> being converted.</param>
    /// <param name="options">The <see cref="JsonSerializerOptions"/> being used.</param>
    /// <returns>The value that was converted.</returns>
    /// <remarks>Note that the value of <seealso cref="JsonConverter{T}.HandleNull"/> determines if the converter handles null JSON tokens.</remarks>
    public override Rectangle Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        var x = root.GetProperty("x").GetInt32();
        var y = root.GetProperty("y").GetInt32();
        var w = root.GetProperty("w").GetInt32();
        var h = root.GetProperty("h").GetInt32();

        return new Rectangle(x, y, w, h);
    }

    /// <summary>
    /// Write the value as JSON.
    /// </summary>
    /// <remarks>
    /// A converter may throw any Exception, but should throw <cref>JsonException</cref> when the JSON
    /// cannot be created.
    /// </remarks>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write to.</param>
    /// <param name="value">The value to convert. Note that the value of <seealso cref="JsonConverter{T}.HandleNull"/> determines if the converter handles <see langword="null" /> values.</param>
    /// <param name="options">The <see cref="JsonSerializerOptions"/> being used.</param>
    public override void Write(Utf8JsonWriter writer, Rectangle value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber("x", value.X);
        writer.WriteNumber("y", value.Y);
        writer.WriteNumber("w", value.Width);
        writer.WriteNumber("h", value.Height);
        writer.WriteEndObject();
    }
}
