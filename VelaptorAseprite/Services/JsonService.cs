// namespace VelaptorAseprite.Services;
//
// using System.Text.Json;
//
// /// <inheritdoc />
// internal sealed class JsonService : IJsonService
// {
//
//
//     /// <inheritdoc />
//     public T? Deserialize<T>(string filePath)
//     {
//         var jsonData = File.ReadAllText(filePath);
//
//         var result = JsonSerializer.Deserialize<T>(jsonData, new JsonSerializerOptions
//         {
//             PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
//         });
//
//         return result;
//     }
// }
