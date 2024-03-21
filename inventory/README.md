# Inventory System (RPG Style)

In this episode, we'll be reconstructing an RPG-style inventory system found in games like Zelda, The Witcher, and World of Warcraft. Our implementation includes both server-side (Go) and client-side (Unity) code.

[![Creating an RPG Inventory System with Nakama and Hiro - Reconstructing Fun Ep. 6](https://img.youtube.com/vi/w8CZxEZU-zI/0.jpg)](https://www.youtube.com/watch?v=w8CZxEZU-zI)

## Overview

The inventory system in Hiro allows players to collect, manage, use, and consume items within your game. In this episode, we use the inventory system to create an RPG-style player inventory, making use of items stats and consumables to provide players with rewards.

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
4. Initialize the `InventoryManager` as shown in `client/InventoryGameCoordinator.cs`.
5. Run the client.

## Features

- **Inventory System**: Players can collect, manage and consume items.

## Resources

- [Nakama Documentation](https://heroiclabs.com/docs/nakama/)
- [Hiro Documentation](https://heroiclabs.com/docs/hiro/)

## Feedback and Issues

If you encounter any issues or have feedback on the implementation, please [raise an issue](https://github.com/heroiclabs/reconstructing-fun/issues) in the main repository.
