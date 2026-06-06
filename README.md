# BlockAdventure

BlockAdventure is a 2D puzzle game built with Unity. The player drags block shapes onto a 9x9 board and scores points by completing horizontal rows, vertical columns, or 3x3 regions. The project includes a main menu, gameplay scene, best score saving, audio, combo popups, and an existing WebGL build in `WebBuild`.

## Table Of Contents

- [Features](#features)
- [Project Info](#project-info)
- [Requirements](#requirements)
- [Opening The Project](#opening-the-project)
- [Running In The Editor](#running-in-the-editor)
- [Build](#build)
- [Gameplay](#gameplay)
- [Folder Structure](#folder-structure)
- [Scenes](#scenes)
- [Code Architecture](#code-architecture)
- [Data And Assets](#data-and-assets)
- [Internal Debug Keys](#internal-debug-keys)
- [Maintenance Notes](#maintenance-notes)
- [License And Asset Sources](#license-and-asset-sources)

## Features

- 9x9 puzzle board divided into 3x3 regions.
- Random generation of 3 playable shapes per turn.
- Drag-and-drop controls through Unity UI events.
- Automatic validation when a shape is dropped.
- Row, column, and 3x3 region clearing.
- Clear animation using DOTween and a particle prefab.
- Combo popup for consecutive line clears.
- Cheer-up popup and voice line based on combo progress.
- Block color progression based on score thresholds.
- Local best score saving.
- Game over and new best score popup.
- Audio mute toggle.
- Existing WebGL build output in `WebBuild`.

## Project Info

| Item | Value |
| --- | --- |
| Unity Editor | `2022.3.47f1` |
| Product name | `BlockAdventure` |
| Company name | `DefaultCompany` |
| Version | `1.0` |
| Build scenes | `Assets/Scenes/MainMenu.unity`, `Assets/Scenes/Game.unity` |
| Default screen size | `1920x1080` |
| WebGL size | `540x960` |
| Render/UI | Unity UI, TextMeshPro |
| Tweening | Local DOTween plugin |
| MCP package | `com.coplaydev.unity-mcp` |

## Requirements

- Unity Hub.
- Unity `2022.3.47f1`, or a compatible Unity 2022.3 LTS version.
- WebGL Build Support module if you need to build for WebGL.
- Git if you want source control.

Main packages in `Packages/manifest.json`:

- `com.unity.ugui`
- `com.unity.textmeshpro`
- `com.unity.test-framework`
- `com.unity.connect.share`
- `com.coplaydev.unity-mcp`
- DOTween is included directly in `Assets/Plugins/Demigiant/DOTween`.

## Opening The Project

1. Open Unity Hub.
2. Select **Add project from disk**.
3. Select this folder:

   ```text
   F:\Unity Projects\Block-Adventure\BlockAdventure
   ```

4. Open it with Unity `2022.3.47f1`.
5. Wait for Unity to import assets and compile scripts before entering Play Mode.

If the project is cloned to another machine, open the nested `BlockAdventure` folder, not the parent `Block-Adventure` folder.

## Running In The Editor

1. Open `Assets/Scenes/MainMenu.unity`.
2. Press **Play**.
3. Use the main menu play button to load the `Game` scene.

You can also open `Assets/Scenes/Game.unity` directly when testing gameplay. The current Build Settings include:

| Build index | Scene |
| --- | --- |
| `0` | `Assets/Scenes/MainMenu.unity` |
| `1` | `Assets/Scenes/Game.unity` |

## Build

### WebGL

The project already contains a WebGL build at:

```text
WebBuild/
```

To rebuild it:

1. Open **File > Build Settings**.
2. Select **WebGL**.
3. Press **Switch Platform** if needed.
4. Make sure `MainMenu` and `Game` are listed in Scenes In Build.
5. Press **Build** and choose an output folder, for example `WebBuild`.

### Windows/macOS/Linux

1. Open **File > Build Settings**.
2. Select the appropriate Standalone platform.
3. Press **Switch Platform**.
4. Press **Build**.

## Gameplay

1. Each turn gives the player 3 shapes at the bottom of the board.
2. Drag one shape onto the 9x9 board.
3. Drop it onto empty cells.
4. If the hovered cells match the shape cell count, the shape is placed.
5. If placement is invalid, the shape returns to its starting position.
6. Completing a row, column, or 3x3 region clears those cells and awards points.
7. When all 3 shapes have been placed, the game generates 3 new shapes.
8. If no active shape can be placed anywhere on the board, the game shows the game over popup.

The current scoring system awards `9` points for each completed line or 3x3 region. If several lines are completed at once, the total score is `9 * completed_line_count`.

## Folder Structure

```text
BlockAdventure/
|-- Assets/
|   |-- Animation/              # Popup/text animation clips and controllers
|   |-- Fonts/                  # TMP font assets
|   |-- Grahpics/               # Textures, UI sprites, and block art
|   |-- Plugins/Demigiant/      # DOTween
|   |-- Prefabs/                # Grid squares, shapes, popups, particles
|   |-- Resources/              # ShapeData, SquareTextureData, DOTween settings
|   |-- Scenes/                 # MainMenu and Game
|   |-- Scripts/                # Gameplay, UI, data, utility scripts
|   |-- Sound/                  # SFX and voice lines
|   `-- TextMesh Pro/           # TMP resources
|-- Packages/
|   `-- manifest.json
|-- ProjectSettings/
|-- WebBuild/                   # Existing WebGL build output
`-- README.md
```

Note: the folder name `Assets/Grahpics` is misspelled in the project. Keep the name unchanged unless asset references are refactored through the Unity Editor.

## Scenes

### `Assets/Scenes/MainMenu.unity`

Root objects:

- `Main Camera`: contains `Camera`, `AudioListener`, and `MenuButtons`.
- `Canvas`: main menu UI.
- `EventSystem`: UI input.

`MenuButtons.LoadScene(string name)` uses `SceneManager.LoadScene` to switch scenes by name.

### `Assets/Scenes/Game.unity`

Root objects:

- `Main Camera`: UI camera and `MenuButtons`.
- `Canvas`: gameplay UI, grid, score, popups, and shape holder.
- `EventSystem`: UI and drag input.
- `SoundManager`: singleton for SFX and voice lines.

## Code Architecture

### Central Events

`Assets/Scripts/GameEvents.cs` contains static `Action` events used to decouple the grid, shapes, score, popups, and UI:

- `CheckIfShapeCanPlaced`
- `CheckIfAnyLineCanCompeleted`
- `UncheckIfAnyLineCanCompeleted`
- `MoveShapeToStartPosition`
- `RequestNewShapes`
- `SetShapeInactive`
- `AddScores`
- `UpdateBestScoreBar`
- `UpdateSquareColor`
- `ComboActivate`
- `GameOver`

### Grid

`Assets/Scripts/Game/Grid/Grid.cs` manages the 9x9 board:

- Spawns 81 `GridSquare` objects from a prefab.
- Calculates cell positions and 3x3 region spacing.
- Checks whether the currently dragged shape can be placed.
- Places blocks on the board.
- Checks completed rows, columns, and 3x3 regions.
- Clears completed cells, plays effects, and awards points.
- Checks the lose condition and requests the game over popup.
- If remaining shapes cannot be placed but a small shape can fit, the game can replace one shape with a small shape.

`Assets/Scripts/Game/Grid/LineIndicator.cs` defines board index mappings:

- `line_data`: rows and columns of the 9x9 board.
- `square_data`: the 9 3x3 regions.

`Assets/Scripts/Game/Grid/GridSquare.cs` manages each board cell:

- `Selected` and `SquareOccupied` state.
- Hover handling when a shape collider enters a cell.
- Normal, active, and hover sprites.
- Clear animation through DOTween.
- Calls `ExplosionManager` when a cell is cleared.

### Shape

`Assets/Scripts/Shape/Shape.cs` handles drag and drop:

- Builds the visual block shape from `ShapeData`.
- Scales the shape while it is being dragged.
- Moves the shape based on pointer position inside the Canvas.
- Fires `CheckIfShapeCanPlaced` when dropped.
- Returns the shape to the start position when placement is invalid.

`Assets/Scripts/Shape/ShapeStorage.cs` manages available shapes:

- Creates the initial 3 shapes.
- Randomly requests new shapes from `shapeData`.
- Stores the selected shape sprite.
- Requests small shapes when needed.
- Includes debug key `R` to request all small shapes.

`Assets/Scripts/Shape/ShapeData.cs` is a `ScriptableObject` that defines a shape with a boolean matrix using `rows` and `columns`.

### Score And Progression

`Assets/Scripts/Game/Scores.cs`:

- Stores `_currentScore`.
- Updates the score text.
- Saves the best score through `BinaryDataStream`.
- Changes block color when the score reaches the threshold in `SquareTextureData`.

`Assets/Scripts/Game/BestScoreBar.cs` updates the best score UI bar using the current score and saved best score.

`Assets/Scripts/ScriptableObjects/SquareTextureData.cs`:

- Stores the list of block color sprites.
- Stores the current color.
- Rotates color by threshold, starting at `10` points.

### Popups, Audio, And Effects

`Assets/Scripts/TextPopUpManager.cs`:

- Shows `COMBO xN!`.
- Shows a `+1` popup over each cleared cell.
- Shows a cheer-up sprite and plays a voice line based on combo count.

`Assets/Scripts/Game/GameOverPopUp.cs`:

- Shows the game over popup.
- Switches between lose and new best score states.
- Displays the final score.
- Includes debug key `O` to trigger game over.

`Assets/Scripts/SoundManager.cs`:

- Singleton audio manager.
- Mute toggle.
- Plays placement SFX, line completed SFX, and voice lines.

`Assets/Scripts/UI_Effects/ExplosionManager.cs` creates the clear explosion effect for grid cells.

## Data And Assets

### Shape Data

Shapes are stored in:

```text
Assets/Resources/Shapes/
```

Examples:

- `Shape1.asset` through `Shape17.asset`
- `ShapeSmall1.asset` through `ShapeSmall3.asset`
- `ShapeLose.asset`
- `ShapeTest.asset`

Each asset uses `ShapeData` with:

- `columns`
- `rows`
- `board`

To create a new shape:

1. Right-click in the Project window.
2. Select **Create > ShapeData**.
3. Set `columns` and `rows`.
4. Use `CreatNewBoard` in the custom inspector if needed.
5. Tick the cells in the board matrix.
6. Add the asset to `ShapeStorage.shapeData` or `ShapeStorage.shapeSmallList`.

### Block Colors

Block color data is stored in:

```text
Assets/Resources/Active Square Texture Data.asset
```

Color sprites are stored in:

```text
Assets/Grahpics/Squares/
Assets/Grahpics/Bonuses/
```

### UI And Popups

Related prefabs:

- `Assets/Prefabs/GridSquare.prefab`
- `Assets/Prefabs/Shape.prefab`
- `Assets/Prefabs/SquareImage.prefab`
- `Assets/Prefabs/TextPopUp/ComboTextPopUp.prefab`
- `Assets/Prefabs/TextPopUp/GridScorePopUp.prefab`
- `Assets/Prefabs/CheerUpPopUp.prefab`
- `Assets/Prefabs/Particle/ExplodeParticle.prefab`

### Audio

Audio files are stored in:

```text
Assets/Sound/
```

Current files include placement SFX, clear line SFX, and voice lines such as `awesome`, `excellent`, `good`, `nice one`, and `sweet`.

## Internal Debug Keys

Some debug keys are currently implemented directly in scripts:

| Key | Behavior | Script |
| --- | --- | --- |
| `R` | Closes the game over popup and requests all small shapes | `ShapeStorage` |
| `O` | Triggers the game over popup | `GameOverPopUp` |
| `Space` | Tests combo text and cheer-up popup | `TextPopUpManager` |

Wrap these debug keys in `#if UNITY_EDITOR` before a production release.

## Maintenance Notes

- Events in `GameEvents` should be subscribed and unsubscribed symmetrically in `OnEnable` and `OnDisable`.
- Many grid rules are hard-coded for `9x9`; changing the board size requires updates in `LineIndicator`, `Grid.CheckIfAnyLineCompleted`, `Grid.GetAllSquaresCombination`, and scoring.
- `ShapeStorage.RequestSmallShapes` currently checks the same `shapeSmallList[smallShapeIndex]` inside its loop. Adjust the logic if true random small-shape selection is needed.
- `BinaryDataStream` saves best score using key `bsdat`; changing this key will make existing player best scores unavailable.
- DOTween is stored in `Assets/Plugins`; back up the project and retest `GridSquare.PlayExplodeEffect` when upgrading DOTween.
- `WebBuild` is build output, not gameplay source. If the repository needs to stay small, consider moving build output to a separate release process.
- For WebGL hosting, verify compression settings and server headers for `.wasm`, `.data`, and `.framework.js`.

## License And Asset Sources

The project contains an asset license file at:

```text
Assets/Grahpics/License.txt
```

Its current content points to:

```text
https://codeplanstudio.com/file-licenses/
```

Before public or commercial release, verify the license for all image assets, fonts, audio files, and DOTween.

## Verification Status

This README was created from information read through Unity MCP and the local project source:

- Active and build scenes in Unity.
- Package list from Unity Package Manager.
- `MainMenu` and `Game` scene hierarchies.
- Gameplay scripts in `Assets/Scripts`.
- Project settings and on-disk asset structure.
