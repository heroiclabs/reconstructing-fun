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

using System;
using System.Linq;
using Hiro;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SoloLiveEventStageUI : MonoBehaviour
{
    public event Action<string, string, Sprite> StageClaimClicked;
    
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _descriptionText;
    [SerializeField] private TMP_Text _progressText;
    [SerializeField] private Slider _progressSlider;
    [SerializeField] private GameObject _rewardContainer;
    [SerializeField] private Image _rewardImage;
    [SerializeField] private TMP_Text _rewardCountText;
    [SerializeField] private GameObject _claimedIcon;
    [SerializeField] private Sprite _coinSprite;
    [SerializeField] private Sprite _gemSprite;
    [SerializeField] private Button _claimButton;
    
    private ISubAchievement _achievement;
    private string _rewardText;
    private Sprite _rewardSprite;
    
    public void Init(ISubAchievement achievement)
    {
        if (achievement == null)
        {
            Destroy(gameObject);
            return;
        }
        
        _achievement = achievement;

        _titleText.text = achievement.Name;
        _descriptionText.text = achievement.Description;
        _progressText.text = $"{achievement.Count:n0}/{achievement.MaxCount:n0}";
        _progressSlider.value = (float) achievement.Count / achievement.MaxCount;
        _claimedIcon.SetActive(achievement.ClaimTimeSec > 0);

        var reward = achievement.AvailableRewards.Guaranteed.Currencies.FirstOrDefault();
        if (reward.Value != null)
        {
            var value = reward.Value.Count.Min;

            _rewardText = $"{value:n0}";
            _rewardSprite = reward.Key == "coins" ? _coinSprite : _gemSprite;
            _rewardCountText.text = _rewardText;
            _rewardImage.sprite = _rewardSprite;
        }

        if (achievement.Count == achievement.MaxCount && achievement.ClaimTimeSec <= 0)
        {
            _rewardContainer.SetActive(false);
            _claimButton.gameObject.SetActive(true);
        }
        else
        {
            _rewardContainer.SetActive(true);
            _claimButton.gameObject.SetActive(false);
        }
    }

    public void Clicked()
    {
        if (_achievement == null)
        {
            return;
        }
        
        StageClaimClicked?.Invoke(_achievement.Id, _rewardText, _rewardSprite);
    }
}
