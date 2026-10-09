# Chapters 4 and 5 — The First Survivor Game

Sample project for **Chapters 4 and 5** of [*High Performance Unity Game Development (Using data-oriented design)*](https://www.manning.com/books/high-performance-unity-game-development) by Nitzan Wilnai (Manning).

This is the first version of the survivor game that the book keeps building on. You are a circle in the middle of the screen. Enemies start in a ring around you and walk toward you. The run ends when one of them reaches you. A timer counts how long you survive. The game is small on purpose. It is here to show the basic data-oriented layout that the later samples keep.

## What it shows

- Data and logic are separate. `Balance` and `GameData` only hold fields. `Logic` holds functions and no state.
- Every function in `Logic` is static. It takes the data it needs as parameters and changes `GameData`.
- Enemy positions are one `Vector2[]` array, allocated once in `Logic.AllocateGameData`.
- Enemy GameObjects are created once in `Board.Init` and reused. Nothing is instantiated or destroyed while you play.
- The MonoBehaviours are a thin layer. `Board` reads input, calls `Logic.Tick`, then copies the positions from `GameData` to the transforms.
- `Logic` never touches a GameObject or a Transform.

## How the code is organized

All the code is in `Assets/Scripts`.

Data

- `Balance.cs` — values that do not change during play: number of enemies, speeds, radii.
- `GameData.cs` — values that change during play: `EnemyPosition`, `PlayerDirection`, `GameTime`, `BestTime`, `MenuState`.

Logic

- `Logic.cs` — the whole simulation. `StartGame` places the enemies. `Tick` moves them, respawns the ones that are too far away, pushes overlapping enemies apart, applies the player's movement, and checks for game over.

The MonoBehaviour side

- `Game.cs` — the entry point. Owns the `GameData` and the `Balance`, shows and hides the menus, and calls `Board.Tick` from `Update`.
- `Board.cs` — owns the pool of enemy GameObjects, reads the input, calls `Logic.Tick`, and updates what is on screen.
- `Tools/Singleton.cs` — a small MonoBehaviour singleton base class, used by `Game`.

Two things worth reading in `Logic.cs`:

- The player never moves. `movePlayer` shifts every enemy the opposite way instead.
- `doEemyToEnemyCollision` tests every pair of enemies. It is a plain nested loop over the position array.

## Running it

1. Open the project in Unity **6000.3.22f1** (Unity 6.3 LTS) or newer.
2. Open `Assets/Scenes/MainGameScene.unity` and press **Play**.
3. Click **Start**. Hold the left mouse button and drag to move. A joystick appears where you pressed, and you move in the direction you drag. Release to stop.
4. When an enemy reaches you the game is over. Click **Retry** to play again.
5. Press **S** to save a screenshot (`screenshot0.png`, `screenshot1.png`, and so on).

To change the game, select the `Game` object in the scene and edit the `Balance` values in the Inspector. `NumEnemies` is 500 in the scene.

Mouse input is used in the Editor. In a build, `Board.handleInput` reads the first touch instead.

## More samples

All sample projects for the book: https://github.com/Data-Oriented-Design-for-Games

Next: [Chapter-7-And-8](https://github.com/Data-Oriented-Design-for-Games/Chapter-7-And-8)

Chapter 4 also has a separate sample: [Chapter-4-List-Allocations](https://github.com/Data-Oriented-Design-for-Games/Chapter-4-List-Allocations)
