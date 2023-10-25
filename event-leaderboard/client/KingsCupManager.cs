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
using TMPro;
using UnityEngine;
using Random = System.Random;

public class KingsCupManager : MonoBehaviour
{
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject rankingPanel;
    [SerializeField] private GameObject rewardPanel;
    [SerializeField] private KingsCupRewardItemUI rewardItemPrefab;
    [SerializeField] private Transform rewardItemsContainer;
    [SerializeField] private GameObject scoreListItemPrefab;
    [SerializeField] private KingsCupListItemUI ownListItem;
    [SerializeField] private Transform scoresContainer;
    [SerializeField] private GameObject claimButton;
    [SerializeField] private GameObject activeTime;
    [SerializeField] private TMP_Text activeTimeRemaining;

    private long _endTimeSeconds;
    private long _remainingSeconds;
    private KingsCupSystem _kingsCupSystem;
    private INakamaSystem _nakamaSystem;

    public async Task InitAsync()
    {
        _nakamaSystem = this.GetSystem<NakamaSystem>();
        _kingsCupSystem = this.GetSystem<KingsCupSystem>();
        SystemObserver<KingsCupSystem>.Create(_kingsCupSystem, OnKingsCupSystemChanged);

        // Get the user's current event leaderboard
        await _kingsCupSystem.GetAndRollAsync();

        GoToEventScreen();
    }
    
    private void OnKingsCupSystemChanged(KingsCupSystem system)
    {
        if (system.EventLeaderboard == null)
        {
            return;
        }

        // Update the end time seconds value
        _endTimeSeconds = system.EventLeaderboard.EndTimeSec;
        
        // Display the active time or the claim button
        if (system.CanClaim)
        {
            activeTime.SetActive(false);
            claimButton.SetActive(true);
        }
        else
        {
            activeTime.SetActive(true);
            claimButton.SetActive(false);
        }
        
        // Clear the current scores list
        foreach (Transform child in scoresContainer)
        {
            Destroy(child.gameObject);
        }
        
        // Refresh the scores list
        foreach (var score in system.EventLeaderboard.Scores)
        {
            if (score.Username == _nakamaSystem.Account.User.Username)
            {
                ownListItem.Init(score);
            }
            
            var listItem = Instantiate(scoreListItemPrefab, scoresContainer);
            listItem.GetComponent<KingsCupListItemUI>().Init(score);
        }
    }

    public async void SubmitScore()
    {
        try
        {
            await _kingsCupSystem.RefreshAsync();

            // Only submit a score if the event is still active
            if (_kingsCupSystem.IsActive)
            {
                var score = new Random().Next(1, 10);
                await _kingsCupSystem.SubmitScoreAsync(score);
            }
        }
        catch (ApiResponseException)
        {
            Debug.Log("Unable to submit score, event leaderboard is likely inactive.");
        }
        
        GoToEventScreen();
    }

    // These functions have a void return type so we can hook this up to the Claim button in the Unity inspector
    public async void Claim()
    {
        // Claim the reward
        await _kingsCupSystem.RefreshAsync();

        if (_kingsCupSystem.CanClaim)
        {
            var result = await _kingsCupSystem.ClaimAsync();
            rewardPanel.SetActive(true);

            // Clear the current reward list
            foreach (Transform child in rewardItemsContainer)
            {
                Destroy(child.gameObject);
            }
            
            // Update the reward list
            foreach (var item in result.Reward.Items)
            {
                var rewardItem = Instantiate(rewardItemPrefab, rewardItemsContainer);
                rewardItem.GetComponent<KingsCupRewardItemUI>().Init(item.Key, item.Value);
            }
            
            foreach (var energyModifier in result.Reward.EnergyModifiers)
            {
                var rewardItem = Instantiate(rewardItemPrefab, rewardItemsContainer);
                var text = energyModifier.Operator == "infinite" ? "∞" : energyModifier.Value.ToString();

                var timespan = TimeSpan.FromSeconds(energyModifier.DurationSec);
                text += $" ({timespan.TotalMinutes}m) ";
                
                rewardItem.GetComponent<KingsCupRewardItemUI>().Init(energyModifier.Id, text);
            }
            
            foreach (var currency in result.Reward.Currencies)
            {
                var rewardItem = Instantiate(rewardItemPrefab, rewardItemsContainer);
                rewardItem.GetComponent<KingsCupRewardItemUI>().Init(currency.Key, currency.Value);
            }
        }
        
        // Re-join the event if we can
        if (_kingsCupSystem.CanRoll)
        {
            await _kingsCupSystem.RollAsync();
        }
    }
    
    public void GoToGameScreen()
    {
        // Show the appropriate panel
        gamePanel.SetActive(true);
        rankingPanel.SetActive(false);
        rewardPanel.SetActive(false);
    }
    public void GoToEventScreen()
    {
        // Show the appropriate panel
        gamePanel.SetActive(false);
        rankingPanel.SetActive(true);
        rewardPanel.SetActive(false);
    }
    private void Update()
    {
        if (_endTimeSeconds <= 0) return;

        var previousRemainingSeconds = _remainingSeconds;
        _remainingSeconds = _endTimeSeconds - new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
        var timespan = TimeSpan.FromSeconds(_remainingSeconds);
        activeTimeRemaining.text = $"{(int) timespan.TotalHours}h {timespan.Minutes}m";

        // Show the claim button if the remaining time ran out
        if (previousRemainingSeconds > 0 && _remainingSeconds <= 0)
        {
            activeTime.SetActive(false);
            claimButton.SetActive(true);
        }
    }
}
