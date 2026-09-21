# RTS Voyage Game - Fixed Starter

Godot 4.7.2 Mono / C# minimal low-poly voyage prototype.

## Fixed
- `Environment` ambiguity fixed with `Godot.Environment`
- Godot 4.7 theme font-size API fixed with `AddThemeFontSizeOverride`
- `run.bat` builds with `dotnet build` before launch
- no C# namespace on `Main.cs`

## Run
1. Check the `GODOT` path in `run.bat`.
2. Run `run.bat`.

Or:

```bat
cd VoyageGame
dotnet build VoyageGame.csproj
```

Then launch the project with Godot 4.7.2 Mono.

## Controls
- WASD: camera movement
- Mouse wheel: zoom
- Right mouse drag: rotate camera
