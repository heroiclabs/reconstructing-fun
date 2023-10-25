// Copyright 2023 GameUp Online, Inc. d/b/a Heroic Labs.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Threading.Tasks;
using Hiro;
using Hiro.System;
using Hiro.Unity;
using KingsCup.Scripts;
using Nakama;
using UnityEngine;

public sealed class KingsCupGameCoordinator : HiroCoordinator
{
    private const string PlayerPrefsAuthToken = "nakama.AuthToken";
    private const string PlayerPrefsRefreshToken = "nakama.RefreshToken";
    private const string PlayerPrefsDeviceId = "nakama.DeviceId";

    /// <inheritdoc cref="HiroCoordinator.CreateSystemsAsync"/>
    protected override Task<Systems> CreateSystemsAsync()
    {
        Application.targetFrameRate = 60;

        // Nakama server address.
        const string scheme = "http";
        const string host = "127.0.0.1";
        const int port = 7350;

        var logger = new Hiro.Unity.Logger();
        
        // Set up network connectivity probes.
        var nakamaProbe = new NakamaClientNetworkProbe(TimeSpan.FromSeconds(60));
        var monitor = new NetworkMonitor(InternetReachabilityNetworkProbe.Default, nakamaProbe);
        
        monitor.ConnectivityChanged += (_, args) =>
        {
            Instance.Logger.InfoFormat($"Network is online? {args.Online}");
        };

        // Create the systems object
        var systems = new Systems("KingsCupGameSystems", monitor, logger);
        
        // Create the Nakama system
        var nakamaSystem = new NakamaSystem(logger, scheme, host, port, "defaultkey", Authorizer, nakamaProbe);

        async Task<ISession> Authorizer(IClient c)
        {
            var authToken = PlayerPrefs.GetString(PlayerPrefsAuthToken);
            var refreshToken = PlayerPrefs.GetString(PlayerPrefsRefreshToken);
            var session = Session.Restore(authToken, refreshToken);

            // Add a day so we check whether the token is within a day of expiration to refresh it.
            var expiredDate = DateTime.UtcNow.AddDays(1);
            if (session != null && !session.HasExpired(expiredDate))
            {
                return session;
            }

            var deviceId = PlayerPrefs.GetString(PlayerPrefsDeviceId, SystemInfo.deviceUniqueIdentifier);
            session = await c.AuthenticateDeviceAsync(deviceId);
            PlayerPrefs.SetString(PlayerPrefsAuthToken, session.AuthToken);
            PlayerPrefs.SetString(PlayerPrefsRefreshToken, session.RefreshToken);
            PlayerPrefs.SetString(PlayerPrefsDeviceId, deviceId);

            return session;
        }

        systems.Add(nakamaSystem);
        
        // Add the Inventory system
        var inventorySystem = new InventorySystem(logger, nakamaSystem);
        systems.Add(inventorySystem);
        
        // Add the Economy system
        var economySystem = new EconomySystem(logger, nakamaSystem, EconomyStoreType.Unspecified);
        systems.Add(economySystem);
        
        // Add the Event Leaderboards system
        var eventLeaderboardsSystem = new EventLeaderboardsSystem(logger, nakamaSystem);
        systems.Add(eventLeaderboardsSystem);
        
        // Add the Kings Cup System
        var kingsCupSystem = new KingsCupSystem(logger, eventLeaderboardsSystem);
        systems.Add(kingsCupSystem);

        return Task.FromResult(systems);
    }

    /// <inheritdoc cref="HiroCoordinator.SystemsInitializeCompleted"/>
    protected override async void SystemsInitializeCompleted()
    {
        Debug.Log("The game is initialized!");

        var kingsCupManager = GameObject.FindWithTag("KingsCupManager").GetComponent<KingsCupManager>();
        await kingsCupManager.InitAsync();
    }

    /// <inheritdoc cref="HiroCoordinator.SystemsInitializeFailed"/>
    protected override void SystemsInitializeFailed(Exception e)
    {
        // Useful if you need to show a UI popup when an exception occurs as the <c>Systems</c> are initialized.
        Debug.LogException(e);
    }
}