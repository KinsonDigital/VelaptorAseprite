// <copyright file="FramesJsonConverter.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorAseprite.Data;

using System.Text.Json;
using System.Text.Json.Serialization;

internal class FramesJsonConverter : JsonConverter<Dictionary<int, AnimationFrame>>
{
    public override Dictionary<int, AnimationFrame>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Expected start of object for dictionary");

        var dict = new Dictionary<int, AnimationFrame>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                return dict;

            if (reader.TokenType != JsonTokenType.PropertyName)
                continue;

            string propName = reader.GetString();
            if (propName == null)
                throw new JsonException("Null property name");

            // find trailing digits
            // var m = _trailingDigits.Match(propName);
            // if (!m.Success)
            //     throw new JsonException($"Property name '{propName}' does not contain a trailing integer key.");

            // int key = int.Parse(m.Groups[1].Value);
            int key = int.Parse(propName);

            // move to the value token and deserialize TValue
            reader.Read();
            var value = JsonSerializer.Deserialize<AnimationFrame>(ref reader, options);
            dict[key] = value;
        }

        throw new JsonException("Unexpected end of JSON while reading dictionary");
    }

    public override void Write(Utf8JsonWriter writer, Dictionary<int, AnimationFrame> value, JsonSerializerOptions options) => throw new NotImplementedException();
}
