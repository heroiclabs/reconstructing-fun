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
using System.Collections.Generic;
using System.Threading.Tasks;
using Hiro;
using Hiro.System;
using Hiro.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class QuestsManager : MonoBehaviour
{
    [SerializeField] private QuestItemUI questItemPrefab;
    [SerializeField] private QuestsRewardItemUI rewardItemPrefab;
    [SerializeField] private Transform onceGrid;
    [SerializeField] private Transform dailyGrid;
    [SerializeField] private Transform weeklyGrid;
    [SerializeField] private Transform rewardPanel;
    [SerializeField] private Transform rewardGrid;
    [SerializeField] private TMP_Text coinsCounter;
    [SerializeField] private TMP_Text gemsCounter;

    private EconomySystem _economySystem;
    private AchievementsSystem _achievementsSystem;

    private const string LoginAchievementId = "daily_login";
    private const string FirstVictoryAchievementId = "first_victory";
    private const string AppleLinkAchievementId = "apple_link";
    private const string Play1GameAchievementId = "play_1_game";
    private const string Play5GamesAchievementId = "play_5_games";
    private const string Deal10DamageAchievementId = "deal_10_damage";
    private const string Spend100ManaAchievementId = "spend_100_mana";
    private const string Spend500ManaAchievementId = "spend_500_mana";
    
    public async Task InitAsync()
    {
        // Get the Economy system and listen for updates.
        _economySystem = this.GetSystem<EconomySystem>();
        SystemObserver<EconomySystem>.Create(_economySystem, OnEconomySystemUpdated);
        
        // Get the Achievements system and listen for updates.
        _achievementsSystem = this.GetSystem<AchievementsSystem>();
        SystemObserver<AchievementsSystem>.Create(_achievementsSystem, OnAchievementsSystemUpdated);

        // Refresh both systems to get the latest data.
        await Task.WhenAll(_economySystem.RefreshAsync(), _achievementsSystem.RefreshAsync());
    }

    private void OnEconomySystemUpdated(EconomySystem economySystem)
    {
        coinsCounter.text = _economySystem.Wallet["coins"].ToString("N0"); 
        gemsCounter.text = _economySystem.Wallet["gems"].ToString("N0");
    }

    private void OnAchievementsSystemUpdated(AchievementsSystem achievementsSystem)
    {
        // Populate the once achievements
        UpdateQuestGrid(onceGrid, _achievementsSystem.GetAvailableAchievements("once"), false);

        // Populate the daily achievements
        UpdateQuestGrid(dailyGrid, _achievementsSystem.GetAvailableRepeatAchievements("daily"), true);
        
        // Populate the weekly achievements
        UpdateQuestGrid(weeklyGrid, _achievementsSystem.GetAvailableRepeatAchievements("weekly"), true);
    }
    
    private void UpdateQuestGrid(Transform grid, IEnumerable<IAchievement> achievements, bool repeating)
    {
        // Clear existing items from the UI.
        foreach (Transform child in grid)
        {
            Destroy(child.gameObject);
        }

        // Iterate through each achievement and update the UI.
        foreach (var achievement in achievements)
        {
            // Determine if the Achievement has been claimed.
            var claimed = achievement.ClaimTimeSec > 0;
            
            // Get the icon name from the Achievement's AdditionalProperties metadata if possible.
            var iconName = "count";
            if (achievement.AdditionalProperties.TryGetValue("icon_name", out var n))
            {
                iconName = n;
            }
            
            // Instantiate a Quest Item UI element and initialize it with the appropriate display data.
            var questItem = Instantiate(questItemPrefab, grid);
            questItem.Init(achievement.Description, achievement.Count, achievement.MaxCount, iconName, repeating, claimed);
            
            // Listen for click events and claim the Achievement when it is clicked.
            questItem.ClaimClicked += async () =>
            {
                // Claim the Achievement.
                var achievementsUpdateAck = await _achievementsSystem.ClaimAchievementsAsync(new[] {achievement.Id});
                
                // Refresh the Economy system so that we can update the UI appropriately after claiming.
                await _economySystem.RefreshAsync();
                
                // Show the Reward popup panel.
                ShowClaimRewardPanel(achievementsUpdateAck);
            };
        }
    }

    private void ShowClaimRewardPanel(IAchievementsUpdateAck ack)
    {
        // Inline function to update the Reward popup panel UI for an Achievement's rewards.
        void AddAchievementRewardToRewardPanel(IAchievement achievement)
        {
            // Iterate through the Item rewards and add them to the UI.
            foreach (var item in achievement.Reward.Items)
            {
                var rewardItem = Instantiate(rewardItemPrefab, rewardGrid);
                rewardItem.GetComponent<QuestsRewardItemUI>().Init(item.Key, item.Value);
            }
            
            // Iterate through the Energy Modifier rewards and add them to the UI.
            foreach (var energyModifier in achievement.Reward.EnergyModifiers)
            {
                var rewardItem = Instantiate(rewardItemPrefab, rewardGrid);
                var text = energyModifier.Operator == "infinite" ? "∞" : energyModifier.Value.ToString();
        
                var timespan = TimeSpan.FromSeconds(energyModifier.DurationSec);
                text += $" ({timespan.TotalMinutes}m) ";
                
                rewardItem.GetComponent<QuestsRewardItemUI>().Init(energyModifier.Id, text);
            }
            
            // Iterate through the Currency rewards and add them to the UI.
            foreach (var currency in achievement.Reward.Currencies)
            {
                var rewardItem = Instantiate(rewardItemPrefab, rewardGrid);
                rewardItem.GetComponent<QuestsRewardItemUI>().Init(currency.Key, currency.Value);
            }
        }
        
        // Clear existing rewards from the reward panel.
        foreach (Transform child in rewardGrid)
        {
            Destroy(child.gameObject);
        }
        
        // Add rewards from normal achievements.
        foreach (var achievement in ack.Achievements)
        {
            AddAchievementRewardToRewardPanel(achievement.Value);
        }
        
        // Add rewards from repeat achievements.
        foreach (var achievement in ack.RepeatAchievements)
        {
            AddAchievementRewardToRewardPanel(achievement.Value);
        }

        // Show the reward popup.
        rewardPanel.gameObject.SetActive(true);
    }

    public void HideRewardPanel()
    {
        rewardPanel.gameObject.SetActive(false);
    }
    
    
    public async void SimulateGamePlayed()
    {
        var achievementIds = new[] {FirstVictoryAchievementId, Play1GameAchievementId, Play5GamesAchievementId};
        await _achievementsSystem.UpdateAchievementsAsync(achievementIds, 1);
    }

    public async void SimulateDamage()
    {
        var achievementIds = new[] {Deal10DamageAchievementId};
        await _achievementsSystem.UpdateAchievementsAsync(achievementIds, 5);
    }
    
    public async void SimulateLinkIOS()
    {
        var achievementIds = new[] {AppleLinkAchievementId};
        await _achievementsSystem.UpdateAchievementsAsync(achievementIds, 1);
    }
    
    public async void SimulateLogin()
    {
        var achievementIds = new[] {LoginAchievementId};
        await _achievementsSystem.UpdateAchievementsAsync(achievementIds, 1);
    }
    
    public async void SimulateSpendMana()
    {
        var achievementIds = new[] {Spend100ManaAchievementId, Spend500ManaAchievementId};
        await _achievementsSystem.UpdateAchievementsAsync(achievementIds, 100);
    }
}
