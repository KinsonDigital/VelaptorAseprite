// <copyright file="IJsonService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorAseprite.Services;

/// <summary>
/// Deserializes JSON data.
/// </summary>
public interface IJsonService
{
    /// <summary>
    /// Parses the text representing a single JSON value into a <typeparamref name="T"/> value.
    /// </summary>
    /// <param name="jsonData">The JSON data to deserialize.</param>
    /// <typeparam name="T">The type to deserialize the JSON value into.</typeparam>
    /// <returns>The deserialized object.</returns>
    T? Deserialize<T>(string jsonData);
}
