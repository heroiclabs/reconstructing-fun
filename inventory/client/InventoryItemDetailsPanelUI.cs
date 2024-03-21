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
using System.Collections.Generic;
using System.Globalization;
using Hiro;
using Hiro.Unity;
using TMPro;
using UnityEngine;

public class InventoryItemDetailsPanelUI : MonoBehaviour
{
    public event Action ItemConsumed;
    
    [SerializeField] private InventorySlotUI _slotItem;
    [SerializeField] private TMP_Text _itemRarity;
    [SerializeField] private TMP_Text _itemCategory;
    [SerializeField] private TMP_Text _itemName;
    [SerializeField] private TMP_Text _itemDescription;
    [SerializeField] private InventoryRewardItemUI _rewardItemPrefab;
    [SerializeField] private Transform _rewardContainer;
    [SerializeField] private Transform _rewardPanel;
    [SerializeField] private GameObject _useButton;

    private IInventoryItem _currentItem;
    
    public void SetItem(IInventoryItem item)
    {
        _currentItem = item;
        
        if (item == null)
        {
            gameObject.SetActive(false);
            return;
        }
        
        // Set UI component visibility.
        gameObject.SetActive(true);
        _useButton.SetActive(item.Consumable);
        
        // Update the item slot UI component.
        _slotItem.SetItem(item);
        
        // Update UI labels.
        var rarity = item.StringProperties["rarity"];
        _itemRarity.text = (rarity switch
        {
            "rare" => "Rare",
            "legendary" => "Legendary",
            _ => "Common"
        }).ToUpperInvariant();
        _itemCategory.text = item.Category.ToUpperInvariant();
        _itemName.text = item.Name;

        // Update description/stats label.
        var description = item.Description + "\n";
        foreach (var (key, value) in item.NumericProperties)
        {
            description += $"\n{ToTitleCase(key)}: {value}";
        }
        
        _itemDescription.text = description;
    }

    public async void ConsumeItem()
    {
        if (_currentItem == null || !_currentItem.Consumable)
        {
            return;
        }

        // Get the Hiro Inventory System.
        var inventorySystem = this.GetSystem<InventorySystem>();
        
        // Consume 1 of the chosen item.
        var itemsToConsume = new Dictionary<string, long>()
        {
            { _currentItem.Id, 1 }
        };
        var instancesToConsume = new Dictionary<string, long>();
        var result = await inventorySystem.ConsumeItemsAsync(itemsToConsume, instancesToConsume, false);
        
        // Show the rewards panel.
        ShowRewardPanel(result);
        
        // Trigger ItemConsumed event.
        ItemConsumed?.Invoke();
    }

    private void ShowRewardPanel(IDictionary<string, IRewardList> rewardLists)
    {
        // Clear the current rewards panel.
        foreach (Transform child in _rewardContainer)
        {
            Destroy(child.gameObject);
        }

        // Iterate through each of the consumption rewards and add them to the rewards panel.
        foreach (var consumeReward in rewardLists.Values)
        {
            foreach (var reward in consumeReward.Rewards)
            {
                // Iterate through the Item rewards and add them to the UI.
                foreach (var item in reward.Items)
                {
                    var rewardItem = Instantiate(_rewardItemPrefab, _rewardContainer);
                    rewardItem.GetComponent<InventoryRewardItemUI>().Init(item.Key, item.Value);
                }
            
                // Iterate through the Energy Modifier rewards and add them to the UI.
                foreach (var energyModifier in reward.EnergyModifiers)
                {
                    var rewardItem = Instantiate(_rewardItemPrefab, _rewardContainer);
                    var text = energyModifier.Operator == "infinite" ? "∞" : energyModifier.Value.ToString();
        
                    var timespan = TimeSpan.FromSeconds(energyModifier.DurationSec);
                    text += $" ({timespan.TotalMinutes}m) ";
                
                    rewardItem.GetComponent<InventoryRewardItemUI>().Init(energyModifier.Id, text);
                }
            
                // Iterate through the Currency rewards and add them to the UI.
                foreach (var currency in reward.Currencies)
                {
                    var rewardItem = Instantiate(_rewardItemPrefab, _rewardContainer);
                    rewardItem.GetComponent<InventoryRewardItemUI>().Init(currency.Key, currency.Value);
                }
                
                // Iterate through the Energy rewards and add them to the UI.
                foreach (var energy in reward.Energies)
                {
                    var rewardItem = Instantiate(_rewardItemPrefab, _rewardContainer);
                    rewardItem.GetComponent<InventoryRewardItemUI>().Init(energy.Key, energy.Value.ToString());
                }
            }
        }
        
        // Show the reward popup.
        _rewardPanel.gameObject.SetActive(true);
    }
    
    public static string ToTitleCase(string input)
    {
        var textInfo = CultureInfo.CurrentCulture.TextInfo;
        var parts = input.Split('_');
        
        for (var i = 0; i < parts.Length; i++)
        {
            parts[i] = textInfo.ToTitleCase(parts[i]);
        }
        
        return string.Join(" ", parts);
    }
}
