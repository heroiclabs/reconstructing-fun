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
using JetBrains.Annotations;
using ILogger = Nakama.ILogger;

namespace KingsCup.Scripts
{
    public class KingsCupSystem : ObservableSystem<KingsCupSystem>, IInitializeSystem
    {
        public string Name => nameof(KingsCupSystem);
        public bool IsInitialized { get; private set; }
        public IEventLeaderboard EventLeaderboard => _eventLeaderboard;
        public bool CanClaim => _eventLeaderboard != null && _eventLeaderboard.CanClaim;
        public bool CanRoll => _eventLeaderboard != null && _eventLeaderboard.CanRoll;
        public bool IsActive => _eventLeaderboard != null && _eventLeaderboard.IsActive;

        private const string LeaderboardId = "kings_cup";
        private readonly EventLeaderboardsSystem _eventLeaderboardsSystem;
        private IEventLeaderboard _eventLeaderboard;

        public KingsCupSystem([NotNull] ILogger logger, EventLeaderboardsSystem eventLeaderboardsSystem) : base(logger)
        {
            _eventLeaderboardsSystem = eventLeaderboardsSystem;
        }
        
        public Task InitializeAsync()
        {
            if (_eventLeaderboardsSystem == null || !_eventLeaderboardsSystem.IsInitialized)
            {
                throw new InvalidOperationException("Must initialize the EventLeaderboardsSystem first");
            }

            IsInitialized = true;
            return Task.CompletedTask;
        }

        public Task InitializeFailedAsync(ILogger logger, Exception e) => Task.CompletedTask;

        public async Task RefreshAsync()
        {
            _eventLeaderboard = await _eventLeaderboardsSystem.GetEventLeaderboardAsync(LeaderboardId);
            NotifyObservers();
        }
        
        public async Task GetAndRollAsync()
        {
            // Get the event leaderboard for the user
            _eventLeaderboard = await _eventLeaderboardsSystem.GetEventLeaderboardAsync(LeaderboardId);
            
            // If we can roll for the next iteration and we don't have a reward to claim, automatically roll.
            if (_eventLeaderboard.CanRoll && !_eventLeaderboard.CanClaim)
            {
                _eventLeaderboard = await _eventLeaderboardsSystem.RollEventLeaderboardAsync(LeaderboardId);
            }

            NotifyObservers();
        }

        public async Task<IEventLeaderboard> RollAsync()
        {
            _eventLeaderboard = await _eventLeaderboardsSystem.RollEventLeaderboardAsync(LeaderboardId);
            NotifyObservers();

            return _eventLeaderboard;
        }

        public async Task<IEventLeaderboard> SubmitScoreAsync(long score)
        {
            _eventLeaderboard = await _eventLeaderboardsSystem.UpdateEventLeaderboardAsync(LeaderboardId, score);
            NotifyObservers();

            return _eventLeaderboard;
        }

        public async Task<IEventLeaderboard> ClaimAsync()
        {
            _eventLeaderboard = await _eventLeaderboardsSystem.ClaimEventLeaderboardAsync(LeaderboardId);
            NotifyObservers();

            return _eventLeaderboard;
        }
    }
}