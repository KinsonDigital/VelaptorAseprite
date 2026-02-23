<div align="center">

# VelaptorAseprite

**Seamless Aseprite integration for the _Velaptor_ 2D game framework**

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-9.0-purple.svg)](https://dotnet.microsoft.com/)

[Features](#-features) • [Installation](#-installation) • [Quick Start](#-quick-start) • [Examples](#-examples)

</div>

---

## 📖 Overview

**VelaptorAseprite** is a powerful NuGet package that brings native [Aseprite](https://www.aseprite.org/) sheet support to [Velaptor](https://github.com/KinsonDigital/Velaptor) 2D games. Load your Aseprite-exported sprite sheets and JSON data seamlessly, with full support for frame-by-frame animations, looping behaviors, and playback control.

Perfect for indie game developers who want to use **Aseprite's** excellent pixel art tools with the **Velaptor** game framework.

---

## ✨ Features

- **Direct Aseprite Integration** - Load sprite sheets exported from Aseprite with their JSON data
- **Full Animation Control** - Play, pause, stop, and reset animations with ease
- **Flexible Looping** - Support for infinite loops, single play through, or custom loop counts
- **Directional Playback** - Play animations forward or backward
- **Speed Control** - Adjust animation speed dynamically
- **Simple API** - Extension methods on `IContentManager` for familiar content loading
- **Frame-Accurate** - Maintains Aseprite's original frame timing and metadata

---

## 📦 Installation

Install via NuGet Package Manager:

```bash
dotnet add package KinsonDigital.VelaptorAseprite
```

Or via Package Manager Console:

```powershell
Install-Package KinsonDigital.VelaptorAseprite
```

---

## 🚀 Quick Start

### 1. Export from Aseprite

Export your sprite sheet from Aseprite:
- **File** → **Export Sprite Sheet**
- **Output**: JSON Data format
- Ensure both `.png` and `.json` files are saved to your Content folder

### 2. Load and Animate

```csharp
using VelaptorAseprite;
using Velaptor.Content;

public class MyScene : SceneBase
{
    private IContentManager contentManager;
    private IAsepriteAtlas? atlasData;

    public override void LoadContent()
    {
        contentManager = ContentManager.Create();

        // Load the Aseprite atlas data
        atlasData = contentManager.LoadAsepriteAtlas("my-character");

        // Start the animation
        atlasData.Play();
    }

    public override void Update(FrameTime frameTime)
    {
        // Update the animation each frame
        atlasData?.Update(frameTime);
    }

    public override void Render()
    {
        if (atlasData is null)
        {
            return;
        }

        var frame = this.atlasData.GetCurrentFrame();

        var srcRect = frame.Bounds;
        var destRect = new Rectangle(0, 0, (int)(this.atlasData.Texture.Width ?? 0), (int)(this.atlasData.Texture.Height ?? 0));
        destRect.X = WindowCenter.X;
        destRect.Y = WindowCenter.Y;

        this.textureRenderer.Render(
            this.atlasData.Texture,
            srcRect,
            destRect,
            RenderScale,
            0f,
            Color.White,
            RenderEffects.None);
    }

    public override void UnloadContent()
    {
        if (atlasData is not null)
        {
            contentManager.UnloadAsepriteAtlas(atlasData);
        }
    }
}
```

---

## 💡 Examples

### Example 1: Simple Character Animation

```csharp
// Load and play a character walk cycle
var walkCycle = contentManager.LoadAsepriteAtlas("character-walk");
walkCycle.LoopingBehavior = LoopingBehavior.Infinite;
walkCycle.Play();

// In your update loop
walkCycle.Update(frameTime);
```

### Example 2: Play Animation X Times

```csharp
// Play a spell effect 3 times then stop
var spellEffect = contentManager.LoadAsepriteAtlas("spell-cast");
spellEffect.LoopingBehavior = LoopingBehavior.Count;
spellEffect.MaxLoops = 3;
spellEffect.Play();
```

### Example 3: Reverse Animation

```csharp
// Play an animation backward
var rewindAnim = contentManager.LoadAsepriteAtlas("door-open");
rewindAnim.Direction = AnimationDirection.Backward;
rewindAnim.Play();
```

### Example 4: Speed Control

```csharp
// Make animation play twice as fast
var fastAnim = contentManager.LoadAsepriteAtlas("fast-action");
fastAnim.SetAnimationSpeed(50); // 50ms per frame instead of exported Aseprite default
fastAnim.Play();
```

### Example 5: Animation State Machine

```csharp
public class Player
{
    private IAsepriteAtlas playerAtlasData;
    private readonly IAppInput<KeyboardState> keyboard;
    private KeyboardState prevKeyState;

    public Player() => this.keyboard = HardwareFactory.GetKeyboard();

    public override void LoadContent()
    {
        // Load the main player animation atlas
        this.playerAtlasData = contentManager.LoadAsepriteAtlas("main-player", "player-idle");

        // Set the initial animation
        this.playerAtlasData.Play();
        this.playerAtlasData.LoopingBehavior = LoopingBehavior.Infinite;
    }

    public void Update(FrameTime frameTime)
    {
        var currentKeyState = this.keyboard.GetState();

        if (currentKeyState.IsKeyDown(KeyCode.Left) || currentKeyState.IsKeyDown(KeyCode.Right))
        {
            this.playerAtlasData.LoopingBehavior = LoopingBehavior.Infinite;
            this.playerAtlasData.AnimationName = "player-walk";
        }

        if (currentKeyState.IsKeyUp(KeyCode.Space) && this.prevKeyState.IsKeyDown(KeyCode.Space)) 
        {
            this.playerAtlasData.LoopingBehavior = LoopingBehavior.None;
            this.playerAtlasData.AnimationName = "player-jump";
        }

        this.prevKeyState = currentKeyState;
        this.playerAtlasData.Update(frameTime);
    }
    
    public override void Render()
    {
        // ... render code here
    }
}
```

## 🤝 Contributing

Contributions are welcome! Feel free to open issues or submit pull requests.

---

## 🙏 Acknowledgments

- [Velaptor](https://github.com/KinsonDigital/Velaptor) – The 2D game framework this library extends
- [Aseprite](https://www.aseprite.org/) – The amazing pixel art and animation tool
- Built by [KinsonDigital](https://github.com/KinsonDigital) (Creator of Velaptor)

---

<div align="center">

**Made with ❤️ for indie game developers**

</div>
