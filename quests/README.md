# Quest System (Hearthstone Style)

In this episode, we'll be reconstructing the quest and achievements system found in games like Hearthstone. Our implementation includes both server-side (Go) and client-side (Unity) code.

[![Creating Quests and Achievements with Nakama and Hiro - Reconstructing Fun Ep. 3](https://img.youtube.com/vi/I7dI7xAlhVY/0.jpg)](https://www.youtube.com/watch?v=I7dI7xAlhVY)

## Overview

The achievements system in Hiro allows players to contribute towards one time and recurring achievements in order to obtain exclusive rewards. Players can make progress towards achievements and then claim their rewards once the achievement is complete.

## Directory Structure

- `/server`: Contains the Go code for the server-side logic.
- `/client`: Contains the Unity scripts for the client-side.

## Setup

### Server

1. Navigate to the `server` directory.
2. Copy the file across to your existing Nakama project. If you have an existing `main.go` file be sure to add the Hiro registration code from `server/main.go` and base system definition JSON files.
3. Run the server.

### Client

1. Navigate to the `client` directory.
2. Copy the scripts across to your Unity project.
3. Be sure you have [installed and configured Hiro](https://heroiclabs.com/docs/hiro/concepts/getting-started/install/index.html) in your Unity project.
4. Initialize the `QuestsManager` as shown in `client/QuestsGameCoordinator.cs`.
5. Run the client.

## Features

- **Quest/Achievement System**: Players can contribute towards and claim quests for exclusive rewards.
- **Recurring Quests**: Quests can be one-time or recurring (e.g. daily or weekly).

## Resources

- [Nakama Documentation](https://heroiclabs.com/docs/nakama/)
- [Hiro Documentation](https://heroiclabs.com/docs/hiro/)

## Feedback and Issues

If you encounter any issues or have feedback on the implementation, please [raise an issue](https://github.com/heroiclabs/reconstructing-fun/issues) in the main repository.
