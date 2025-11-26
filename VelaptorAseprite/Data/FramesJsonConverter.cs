// <copyright file="FramesJsonConverter.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorAseprite.Data;

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// Converts the Asesprite frame data or value to or from JSON.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "JSON processing is not possible to test.")]
internal class FramesJsonConverter : JsonConverter<Dictionary<int, AnimationFrame>>
{
    /// <summary>
    /// Reads Aseprite frame data from JSON to <see cref="Dictionary{TKey, TValue}"/>.
    /// </summary>
    /// <param name="reader">The Utf8JsonReader to read from.</param>
    /// <param name="typeToConvert">The <see cref="System.Type"/> being converted.</param>
    /// <param name="options">The <see cref="JsonSerializerOptions"/> being used.</param>
    /// <returns>The value that was converted.</returns>
    /// <exception cref="JsonException">Thrown if the <see cref="JsonTokenType"/> is not of type <see cref="JsonTokenType.StartObject"/>.</exception>
    public override Dictionary<int, AnimationFrame> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException("Expected start of an object for dictionary.");
        }

        var dict = new Dictionary<int, AnimationFrame>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return dict;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                continue;
            }

            var propName = reader.GetString();

            if (propName == null)
            {
                throw new JsonException("Null property name");
            }

            var key = int.Parse(propName);

            // move to the value token and deserialize TValue
            reader.Read();
            var value = JsonSerializer.Deserialize<AnimationFrame>(ref reader, options);

            dict[key] = value ?? throw new JsonException("Deserialized value was null.");
        }

        throw new JsonException("Unexpected end of JSON while reading dictionary");
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
    public override void Write(Utf8JsonWriter writer, Dictionary<int, AnimationFrame> value, JsonSerializerOptions options) => throw new NotImplementedException();
}
