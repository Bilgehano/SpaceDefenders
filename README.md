# SpaceDefenders

SpaceDefenders is a Unity-based VR tower defense project. The player places towers, starts enemy waves, manages resources, and defends the base while interacting in VR using Unity XR tooling.

The project includes:

- VR interaction with Unity XR Interaction Toolkit
- Wave-based enemy spawning
- Placeable towers and tower shop interactions
- Base health / reactor defense gameplay
- Main menu and in-game VR scene flow

## Repository Structure

This repository is organized into a few important top-level folders:

### `src/`

This is the Unity source project.

The actual Unity project folder is:

`src/SpaceDefenders/SpaceDefenders`

That folder contains the Unity project content such as:

- `Assets/`
- `Packages/`
- `ProjectSettings/`

If you want to open the project in Unity, this is the folder you open.

### `Build/`

This folder contains exported build output for the project.

At the moment it includes:

- `Build/SpaceDefenders.zip`

Use this if you want the already built version instead of opening the source in Unity.

### `Dokumentation/`

This folder is reserved for project documentation and related delivery material.

## What This Project Is

SpaceDefenders is a VR tower defense prototype / game project where the player:

1. enters through a main menu
2. loads into the VR gameplay scene
3. places or grabs tower objects through XR interactions
4. starts waves of enemies
5. prevents enemies from reaching the base

From the current codebase, the project clearly includes:

- wave spawning logic in `WaveManager`
- waypoint-based enemy movement in `EnemyMovement`
- XR-based shop/tower spawning in `TowerShop`
- game scenes in `MainMenu.unity` and `VrRoom.unity`

## Unity Version

This project is set up for:

- Unity `6000.4.1f1`

Using the matching Unity version is strongly recommended to avoid package or serialization issues.

## Main Unity Packages

The project uses Unity packages such as:

- XR Interaction Toolkit
- XR Hands
- OpenXR
- XR Management
- Universal Render Pipeline (URP)
- Input System

## How To Open The Source Project In Unity

1. Install Unity Hub.
2. Install Unity Editor version `6000.4.1f1`.
3. In Unity Hub, choose `Add` or `Open`.
4. Select this folder:

   `src/SpaceDefenders/SpaceDefenders`

5. Let Unity import the project and resolve packages.
6. Open the main scenes from `Assets/Scenes/`.

Recommended scenes:

- `MainMenu.unity`
- `VrRoom.unity`

## How To Use The Build

If you do not want to work with the Unity source project:

1. Open the `Build/` folder.
2. Extract `SpaceDefenders.zip`.
3. Run the exported build from the extracted contents.

## What To Do If You Want To Develop Further

If you want to continue development in Unity:

1. Open the Unity project from `src/SpaceDefenders/SpaceDefenders`.
2. Review scenes in `Assets/Scenes/`.
3. Review gameplay scripts in `Assets/Scripts/`.
4. Check XR settings and OpenXR configuration under project settings and XR assets.
5. Build new player versions from Unity when needed.

## Demo / Live Video

This project is also presented with a demo / live video walkthrough covering:

- the main menu flow
- entering the VR scene
- tower placement and shop interaction
- wave start and enemy progression
- the overall gameplay loop

Add the final public video link here when it is published.

## Notes

- Unity-generated folders such as `Library/`, `Temp/`, and `UserSettings/` are intentionally not tracked in Git.
- Generated Visual Studio / solution files are also ignored because Unity can recreate them.
- Large build artifacts can make the repository heavier over time, so `Build/` should be managed deliberately.