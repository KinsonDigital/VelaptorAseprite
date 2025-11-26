using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;
using AsepriteTesting;
using VelaptorAseprite;
using VelaptorAseprite.Data;

// var frame = new AnimationFrame();
//
// var filePath = Path.Combine(AppContext.BaseDirectory, "TestData", "FrameData.json");
// var jsonData = File.ReadAllText(filePath);
//
//
// var result = JsonSerializer.Deserialize<AsepriteAtlasData>(jsonData, new JsonSerializerOptions
// {
//     PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
// });


// return 0;

// Create a new game instance and run it to start the game
var game = new Game();
game.Show();
