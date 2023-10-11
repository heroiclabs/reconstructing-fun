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
using System.Collections;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CollectibleCardUI : MonoBehaviour
{
    public event Action Clicked;

    [SerializeField] private float zoomSpeed = 10f;
    [SerializeField] private float zoomTargetScale = 1.25f;
    [SerializeField] private TMP_Text cardLevelLabel;
    [SerializeField] private TMP_Text cardCountLabel;
    [SerializeField] private TMP_Text cardUpgradeCostLabel;
    [SerializeField] private GameObject cardUpgradeIcon;
    [SerializeField] private GameObject cardUpgradeCoinsIcon;
    [SerializeField] private GameObject cardMaxedIcon;
    [SerializeField] private Image cardImage;
    [SerializeField] private TMP_Text cardName;
    [SerializeField] private Slider costSlider;
    [SerializeField] private Image costSliderFill;
    [SerializeField] private Sprite costSliderInProgressSprite;
    [SerializeField] private Sprite costSliderFullSprite;
    [SerializeField] private Image cardBackground;
    [SerializeField] private Sprite cardCommonBackground;
    [SerializeField] private Sprite cardRareBackground;
    [SerializeField] private Sprite cardEpicBackground;
    [SerializeField] private Sprite cardLegendaryBackground;

    public void SetCard(Card card, Sprite sprite, long currentCoins)
    {
        // Set card name and sprite
        cardName.text = card.Name;
        cardImage.sprite = sprite;
        
        // Set card rarity background
        var background = card.StringProperties["card_rarity"] switch
        {
            "rare" => cardRareBackground,
            "epic" => cardEpicBackground,
            "legendary" => cardLegendaryBackground,
            _ => cardCommonBackground
        };

        cardBackground.sprite = background;

        // If card isn't at max level, show the upgrade cost
        if (card.NumericProperties.ContainsKey("card_upgrade_count_cost"))
        {
            var upgradeCostCardCount = card.NumericProperties["card_upgrade_count_cost"];
            var upgradeCostCoinsCount = card.NumericProperties["card_upgrade_coin_cost"];
            
            // Set card count label
            cardCountLabel.text = $"{card.Count - 1}/{upgradeCostCardCount}";
            cardCountLabel.color = card.Count - 1 >= upgradeCostCardCount ? Color.green : Color.white;
        
            // NOTE: We use card.Count - 1 in upgrade calculations as the player needs to keep at least 1 of the card
            // Set card upgrade slider value
            costSlider.maxValue = (float) upgradeCostCardCount;
            costSlider.value = card.Count - 1;
            costSliderFill.sprite =
                card.Count - 1 >= upgradeCostCardCount ? costSliderFullSprite : costSliderInProgressSprite;
            
            // Hide max icon
            cardMaxedIcon.gameObject.SetActive(false);

            var canAffordUpgrade = currentCoins >= upgradeCostCoinsCount;
            
            // Set card upgrade cost label
            cardUpgradeCostLabel.gameObject.SetActive(true);
            cardUpgradeCoinsIcon.gameObject.SetActive(true);
            cardUpgradeCostLabel.text = upgradeCostCoinsCount.ToString();
            cardUpgradeCostLabel.color = canAffordUpgrade ? Color.green : Color.red;
            
            // Set card upgrade icon visibility
            if (canAffordUpgrade && card.Count - 1 >= upgradeCostCardCount)
            {
                cardUpgradeIcon.SetActive(true);
            }
            else
            {
                cardUpgradeIcon.SetActive(false);
            }
        }
        else
        {
            // Set card count label
            cardCountLabel.text = $"MAX";
            cardCountLabel.color = Color.white;
            
            // Set card upgrade slider value
            costSlider.maxValue = 1;
            costSlider.value = 1;
            costSliderFill.sprite = costSliderFullSprite;
            
            // Show max icon
            cardMaxedIcon.gameObject.SetActive(true);
            
            // Hide card upgrade cost label and icons
            cardUpgradeCostLabel.gameObject.SetActive(false);
            cardUpgradeCoinsIcon.gameObject.SetActive(false);
            cardUpgradeIcon.SetActive(false);
        }

        // Set card level label
        double level = 0;
        if (card.NumericProperties.TryGetValue("card_level", out var cardLevel))
        {
            level = cardLevel + 1;
        }

        cardLevelLabel.text = level.ToString(CultureInfo.InvariantCulture);
    }

    public void OnClick()
    {
        Clicked?.Invoke();
    }

    public void OnEnter()
    {
        StartCoroutine(nameof(ZoomInCoroutine));
    }

    public void OnExit()
    {
        StartCoroutine(nameof(ZoomOutCoroutine));
    }

    private IEnumerator ZoomInCoroutine()
    {
        var originalScale = transform.localScale;

        var progress = 0.0f;
        while (progress < 1.0f)
        {
            progress += Time.deltaTime * zoomSpeed;
            transform.localScale = Vector3.Lerp(originalScale, Vector3.one * zoomTargetScale, progress);
            yield return null;
        }

        transform.localScale = Vector3.one * zoomTargetScale;
    }

    private IEnumerator ZoomOutCoroutine()
    {
        var originalScale = transform.localScale;

        var progress = 0.0f;
        while (progress < 1.0f)
        {
            progress += Time.deltaTime * zoomSpeed;
            transform.localScale = Vector3.Lerp(originalScale, Vector3.one, progress);
            yield return null;
        }

        transform.localScale = Vector3.one;
    }
}
