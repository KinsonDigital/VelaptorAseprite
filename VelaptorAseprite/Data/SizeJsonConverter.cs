// <copyright file="SizeJsonConverter.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorAseprite.Data;

using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// Converts the Aseprite 'sourceSize' of each frame and the meta-data 'size' to or from JSON.
/// </summary>
[ExcludeFromCodeCoverage(Justification = $"Cannot test due to direct interaction with '{nameof(JsonConverter<Size>)}'.")]
internal class SizeJsonConverter : JsonConverter<Size>
{
    /// <summary>
    /// Reads Aseprite frame data from JSON to <see cref="Size"/>.
    /// </summary>
    /// <param name="reader">The Utf8JsonReader to read from.</param>
    /// <param name="typeToConvert">The <see cref="System.Type"/> being converted.</param>
    /// <param name="options">The <see cref="JsonSerializerOptions"/> being used.</param>
    /// <returns>The value that was converted.</returns>
    /// <exception cref="JsonException">Thrown if the <see cref="JsonTokenType"/> is not of type <see cref="JsonTokenType.StartObject"/>.</exception>
    public override Size Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        var w = root.GetProperty("w").GetInt32();
        var h = root.GetProperty("h").GetInt32();

        return new Size(w, h);
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
    public override void Write(Utf8JsonWriter writer, Size value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber("w", value.Width);
        writer.WriteNumber("h", value.Height);
        writer.WriteEndObject();
    }
}
