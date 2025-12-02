// <copyright file="JsonService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorAseprite.Services;

using System.Text.Json;

/// <inheritdoc />
internal sealed class JsonService : IJsonService
{
    /// <inheritdoc />
    public T? Deserialize<T>(string jsonData)
    {
        var result = JsonSerializer.Deserialize<T>(jsonData, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        });

        return result;
    }
}
