# Solo Live Event System (Fortnite Style)

In this episode, we'll be reconstructing a solo live event system found in games like Fortnite. Our implementation includes both server-side (Go) and client-side (Unity) code.

[![Creating a Solo Live Event with Nakama and Hiro - Reconstructing Fun Ep. 5](https://img.youtube.com/vi/RBrnk0Xe2dI/0.jpg)](https://www.youtube.com/watch?v=RBrnk0Xe2dI)

## Overview

The achievements system in Hiro allows players to contribute towards one time and recurring achievements in order to obtain exclusive rewards. In this episode, we use the achievements system to create a solo live event, making use of sub achievements to map player progression during the event.

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
4. Initialize the `SoloLiveEventManager` as shown in `client/SoloLiveEventGameCoordinator.cs`.
5. Run the client.

## Features

- **Solo Live Event System**: Players can progress through a solo live event.

## Resources

- [Nakama Documentation](https://heroiclabs.com/docs/nakama/)
- [Hiro Documentation](https://heroiclabs.com/docs/hiro/)

## Feedback and Issues

If you encounter any issues or have feedback on the implementation, please [raise an issue](https://github.com/heroiclabs/reconstructing-fun/issues) in the main repository.
