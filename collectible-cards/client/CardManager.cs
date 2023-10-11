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
using System.Linq;
using System.Threading.Tasks;
using Hiro;
using Hiro.System;
using TMPro;
using UnityEngine;

[Serializable]
public struct CardSprite
{
    public string name;
    public Sprite sprite;
}

public class CardManager : MonoBehaviour
{
    [SerializeField] private GameObject cardGrid;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private TMP_Text coinsLabel;
    [SerializeField] private GameObject loadingSpinner;
    [SerializeField] private List<CardSprite> cardSprites;

    private EconomySystem _economySystem;
    private CollectibleCardSystem _collectibleCardSystem;
    private readonly Dictionary<string, Sprite> _cardSprites = new();
    private long _coins;

    private void Start()
    {
        // Cache card sprites in a dictionary for easy lookup
        foreach (var cardSprite in cardSprites)
        {
            _cardSprites.Add(cardSprite.name, cardSprite.sprite);
        }
    }

    public async Task InitAsync(EconomySystem economySystem, CollectibleCardSystem collectibleCardSystem)
    {
        _economySystem = economySystem;
        _collectibleCardSystem = collectibleCardSystem;

        SystemObserver<EconomySystem>.Create(economySystem, OnEconomySystemChanged);
        SystemObserver<CollectibleCardSystem>.Create(collectibleCardSystem, OnCollectibleCardSystemChanged);

        // Fetch first data from collectible card system
        await _collectibleCardSystem.GetCardsAsync();
    }

    private void OnEconomySystemChanged(EconomySystem system)
    {
        // Update the coins display
        _coins = system.Wallet["coins"];
        
        UpdateUI();
    }

    private void OnCollectibleCardSystemChanged(CollectibleCardSystem system)
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        // Update coins display
        coinsLabel.text = _coins.ToString();
        
        // Clear the existing card grid
        foreach (Transform t in cardGrid.transform)
        {
            Destroy(t.gameObject);
        }
        
        // Sort cards by name
        var sortedCards = _collectibleCardSystem.Cards.Select(x => x.Value).OrderBy(x => x.Name);

        // Hide the loading spinner
        loadingSpinner.SetActive(false);
        
        // Add cards to the grid
        foreach (var card in sortedCards)
        {
            var newCard = Instantiate(cardPrefab, cardGrid.transform);
            var collectibleCardUI = newCard.GetComponent<CollectibleCardUI>();
            
            collectibleCardUI.Clicked += () => OnCardClicked(card);
            collectibleCardUI.SetCard(card, _cardSprites[card.Name], _coins);
        }
    }
    
    private async void OnCardClicked(Card card)
    {
        await _collectibleCardSystem.UpgradeCardAsync(card.Id);
        await _economySystem.RefreshAsync();
    }
}
