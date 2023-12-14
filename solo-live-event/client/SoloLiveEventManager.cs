// Copyright 2022 GameUp Online, Inc. d/b/a Heroic Labs.
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

using System.Linq;
using System.Threading.Tasks;
using Hiro;
using Hiro.System;
using Hiro.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SoloLiveEventManager : MonoBehaviour
{
    [SerializeField] private SoloLiveEventPopupUI _soloLiveEventPopup;
    [SerializeField] private Transform _soloLiveEventStagesContainer;
    [SerializeField] private SoloLiveEventStageUI _soloLiveEventStagePrefab;
    [SerializeField] private GameObject _rewardPopup;
    [SerializeField] private Image _rewardPopupImage;
    [SerializeField] private TMP_Text _rewardPopupText;
    [SerializeField] private Slider _stageSlider;
    [SerializeField] private TMP_Text _coinsText;
    [SerializeField] private TMP_Text _gemsText;
    
    private AchievementsSystem _achievementsSystem;
    private EconomySystem _economySystem;
    private IAchievement _eventAchievement;
    
    public async Task InitAsync()
    {
        // Get a reference to the Hiro systems.
        _achievementsSystem = this.GetSystem<AchievementsSystem>();
        _economySystem = this.GetSystem<EconomySystem>();

        // Create observers to update the UI when changes occur.
        SystemObserver<AchievementsSystem>.Create(_achievementsSystem, OnAchievementSystemChanged);
        SystemObserver<EconomySystem>.Create(_economySystem, OnEconomySystemChanged);

        // Grab the latest data from both systems.
        await Task.WhenAll(_achievementsSystem.RefreshAsync(), _economySystem.RefreshAsync());
    }

    private void OnEconomySystemChanged(EconomySystem system)
    {
        // Update the coins and gems display.
        _coinsText.text = $"{system.Wallet["coins"]:n0}";
        _gemsText.text = $"{system.Wallet["gems"]:n0}";
    }

    private void OnAchievementSystemChanged(AchievementsSystem system)
    {
        // Update the UI to show/hide the event popup and set the event achievement private field.
        var availableAchievements = system.GetAvailableRepeatAchievements("super_adventure_event").ToList();
        if (availableAchievements.Any())
        {
            // Cache the event variable.
            _eventAchievement = availableAchievements.First();
            
            // Show the event popup in the UI.
            _soloLiveEventPopup.gameObject.SetActive(true);
            _soloLiveEventPopup.Init(_eventAchievement.Name, _eventAchievement.ResetTimeSec);

            // Clear the event stages container.
            foreach (Transform child in _soloLiveEventStagesContainer)
            {
                Destroy(child.gameObject);
            }
            

            // Keep track of how many stages there are and how many are complete.
            var totalSubAchievements = _eventAchievement.SubAchievements.Count;
            var totalCompletedSubAchievements = 0;
            
            // Populate event stages container.
            foreach (var subAchievement in _eventAchievement.SubAchievements)
            {
                // Add the event stage element to the UI and bind to the StageClaimClicked event.
                var stage = Instantiate(_soloLiveEventStagePrefab, _soloLiveEventStagesContainer);
                stage.Init(subAchievement.Value);
                stage.StageClaimClicked += OnStageClaimClicked;

                // If the sub achievement is complete, the stage is complete.
                if (subAchievement.Value.ClaimTimeSec > 0)
                {
                    totalCompletedSubAchievements++;
                }
            }
            
            // Update the event stages progression slider to show what stage the player has reached.
            _stageSlider.value = (float) totalCompletedSubAchievements / totalSubAchievements;
            _stageSlider.handleRect.GetComponentInChildren<TMP_Text>().text = totalCompletedSubAchievements.ToString();
        }
        else
        {
            // Clear the cached event variable and hide the event popup UI element.
            _eventAchievement = null;
            _soloLiveEventPopup.gameObject.SetActive(false);
        }
    }

    private async void OnStageClaimClicked(string achievementId, string rewardText, Sprite rewardSprite)
    {
        // Claim the specified achievement.
        await _achievementsSystem.ClaimAchievementsAsync(new[] {achievementId});

        // Update and show the reward popup.
        _rewardPopupText.text = rewardText;
        _rewardPopupImage.sprite = rewardSprite;
        _rewardPopup.SetActive(true);
    }

    public async void SimulateEarnPoints(int points = 1000)
    {
        if (_eventAchievement != null)
        {
            // Simulate updating the event achievements' count values by `points` amount.
            var achievementIds = _eventAchievement.SubAchievements.ToList()
                .Select(x => x.Key);
            await _achievementsSystem.UpdateAchievementsAsync(achievementIds, points);
        }
    }
}
