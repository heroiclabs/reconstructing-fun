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
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestItemUI : MonoBehaviour
{
    public event Action ClaimClicked;

    [SerializeField] private GameObject claimButton;
    [SerializeField] private Image claimIcon;
    [SerializeField] private Image repeatIcon;
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text descriptionLabel;
    [SerializeField] private TMP_Text counterLabel;

    [Header("Sprites")]
    
    [SerializeField] private Sprite countSprite;
    [SerializeField] private Sprite healthSprite;
    [SerializeField] private Sprite damageSprite;
    [SerializeField] private Sprite winsSprite;
    [SerializeField] private Sprite skullSprite;
    [SerializeField] private Sprite manaSprite;
    
    public void Init(string description, long count, long maxCount, string iconName, bool isRepeat, bool claimed)
    {
        var canClaim = count == maxCount && !claimed;
        descriptionLabel.text = description;
        counterLabel.text = claimed ? "Claimed" : canClaim ? "Claim!" : $"{count}/{maxCount}";
        repeatIcon.gameObject.SetActive(isRepeat);
        claimIcon.gameObject.SetActive(canClaim);
        claimButton.gameObject.SetActive(canClaim);

        if (canClaim)
        {
            counterLabel.color = Color.green;
        }
        
        switch (iconName)
        {
            case "health":
                icon.sprite = healthSprite;
                break;
            case "damage":
                icon.sprite = damageSprite;
                break;
            case "wins":
                icon.sprite = winsSprite;
                break;
            case "skull":
                icon.sprite = skullSprite;
                break;
            case "mana":
                icon.sprite = manaSprite;
                break;
            default:
                icon.sprite = countSprite;
                break;
        }
    }

    public void Claim()
    {
        ClaimClicked?.Invoke();
    }
}
