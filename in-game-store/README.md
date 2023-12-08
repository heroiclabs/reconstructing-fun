# In-Game Store System (Fall Guys Style)

In this episode, we'll be reconstructing the in-game store system found in games like Fall Guys. Our implementation includes both server-side (Go) and client-side (Unity) code.

[![Creating an In-Game Store with Nakama and Hiro - Reconstructing Fun Ep. 4](https://img.youtube.com/vi/HIVNPzk9NMk/0.jpg)](https://www.youtube.com/watch?v=HIVNPzk9NMk)

## Overview

The economy system in Hiro allows game designers to create exciting and rewarding virtual stores using a configuration driven approach. Players can purchase items from these virtual stores using both virtual and fiat currencies, allowing them to boost their experience by buying things such as in-game skins, loot boxes and more.

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
4. Initialize the `InGameStoreManager` as shown in `client/InGameStoreGameCoordinator.cs`.
5. Run the client.

## Features

- **In-Game Shop System**: Players can purchase virtual store items for rewards.

## Resources

- [Nakama Documentation](https://heroiclabs.com/docs/nakama/)
- [Hiro Documentation](https://heroiclabs.com/docs/hiro/)

## Feedback and Issues

If you encounter any issues or have feedback on the implementation, please [raise an issue](https://github.com/heroiclabs/reconstructing-fun/issues) in the main repository.
