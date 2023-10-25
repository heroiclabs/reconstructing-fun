# Event Leaderboard System (Royal Match King's Cup Style)

In this episode, we'll be reconstructing the event leaderboard system found in games like Royal Match. Our implementation includes both server-side (Go) and client-side (Unity) code.

[![Creating Event Leaderboards with Nakama and Hiro - Reconstructing Fun Ep. 2](https://img.youtube.com/vi/DRFr3VBeCcQ/0.jpg)](https://www.youtube.com/watch?v=DRFr3VBeCcQ)

## Overview

The event leaderboard system allows players to participate in a recurring scheduled event in order to compete for prizes at the top of the leaderboard. Players are bucketed into cohorts of 50 to keep the leaderboards fresh and engaging, with rewards that vary based on the player's final rank within the cohort.

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
4. Initialize the `KingsCupSystem` as shown in `client/KingsCupGameCoordinator.cs`.
5. Run the client.

## Features

- **Recurring Event Leaderboard**: Players can compete regularly to earn a top spot and exclusive rewards.
- **Bucketing**: Players are placed into cohorts/buckets of 50 each iteration.
- **Tiers**: Player's can optionally move up and down tiers each iteration within the event leaderboard to compete for progressively better rewards.

## Resources

- [Nakama Documentation](https://heroiclabs.com/docs/nakama/)
- [Hiro Documentation](https://heroiclabs.com/docs/hiro/)

## Feedback and Issues

If you encounter any issues or have feedback on the implementation, please [raise an issue](https://github.com/heroiclabs/reconstructing-fun/issues) in the main repository.
