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

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private Transform _inventoryContainer;
    [SerializeField] private InventorySlotUI _slotPrefab;
    [SerializeField] private InventoryItemDetailsPanelUI _itemDetailsPanel;
    [SerializeField] private TMP_Text _coinsText;
    [SerializeField] private TMP_Text _gemsText;
    
    public async Task InitAsync()
    {
        var economySystem = this.GetSystem<EconomySystem>();
        var inventorySystem = this.GetSystem<InventorySystem>();
        
        // Create system observers to update the UI when the economy or inventory system data changes.
        SystemObserver<EconomySystem>.Create(economySystem, OnEconomySystemChanged);
        SystemObserver<InventorySystem>.Create(inventorySystem, OnInventorySystemChanged);
        
        // Refresh the economy system.
        await economySystem.RefreshAsync();
        
        // Refresh the inventory system.
        await inventorySystem.RefreshAsync();

        // Refresh the inventory system whenever an item is consumed.
        _itemDetailsPanel.ItemConsumed += async () =>
        {
            await economySystem.RefreshAsync();
            await inventorySystem.RefreshAsync();
        };
    }

    private void OnEconomySystemChanged(EconomySystem economySystem)
    {
        _coinsText.text = $"{economySystem.Wallet["coins"]:N0}";
        _gemsText.text = $"{economySystem.Wallet["gems"]:N0}";
    }
    
    private void OnInventorySystemChanged(InventorySystem inventorySystem)
    {
        var firstItem = inventorySystem.Items.FirstOrDefault();
        _itemDetailsPanel.SetItem(firstItem);

        // Clear the existing item slots.
        foreach (Transform child in _inventoryContainer)
        {
            Destroy(child.gameObject);
        }
        
        // Add item slots for each inventory item.
        foreach (var item in inventorySystem.Items)
        {
            var slotItem = Instantiate(_slotPrefab, _inventoryContainer);
            slotItem.SetItem(item);
            slotItem.Clicked += i =>
            {
                _itemDetailsPanel.SetItem(i);
            };
        }
    }
}
