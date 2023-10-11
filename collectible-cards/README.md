# Collectible Card System (Clash Royale Style)

In this episode, we'll be reconstructing the collectible card system found in games like Clash Royale. Our implementation includes both server-side (Go) and client-side (Unity) code.

[![Creating Collectible Card Mechanics with Nakama and Hiro - Reconstructing Fun Ep. 1](https://img.youtube.com/vi/Jv80QJ01bDk/0.jpg)](https://www.youtube.com/watch?v=Jv80QJ01bDk)

## Overview

The collectible card system allows players to collect, upgrade, and use cards in battles. Cards can be obtained through various means, and their attributes can be enhanced as they are upgraded.

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
4. Initialize the Collectible Card System as shown in `client/CollectibleCardGameCoordinator.cs`.
5. Run the client.

## Features

- **Card Collection**: Players can collect a variety of cards in their inventory.
- **Card Upgrades**: Enhance card attributes by upgrading them using in-game resources.

## Resources

- [Nakama Documentation](https://heroiclabs.com/docs/nakama/)
- [Hiro Documentation](https://heroiclabs.com/docs/hiro/)

## Feedback and Issues

If you encounter any issues or have feedback on the implementation, please [raise an issue](https://github.com/heroiclabs/reconstructing-fun/issues) in the main repository.
