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
using System.Threading.Tasks;
using Hiro;
using Hiro.System;
using Hiro.Unity;
using TMPro;
using UnityEngine;

public class InGameStoreManager : MonoBehaviour
{
    [SerializeField] private Transform _storeGroupLarge;
    [SerializeField] private Transform _storeGroupSmall;
    [SerializeField] private StoreItemUI _storeItemLargePrefab;
    [SerializeField] private StoreItemUI _storeItemSmallPrefab;
    [SerializeField] private InGameStoreRewardPanelUI _rewardPanel;
    [SerializeField] private GameObject _errorPanel;
    [SerializeField] private TMP_Text _coinsText;
    [SerializeField] private TMP_Text _gemsText;
    
    private NakamaSystem _nakamaSystem;
    private EconomySystem _economySystem;
    
    public async Task InitAsync()
    {
        _nakamaSystem = this.GetSystem<NakamaSystem>();
        _economySystem = this.GetSystem<EconomySystem>();

        SystemObserver<EconomySystem>.Create(_economySystem, OnEconomySystemChanged);
        
        await Task.WhenAll(_nakamaSystem.RefreshAsync(), _economySystem.RefreshAsync());
    }

    private void OnEconomySystemChanged(EconomySystem system)
    {
        // Update coins and gems display.
        _coinsText.text = $"{system.Wallet["coins"]:n0}";
        _gemsText.text = $"{system.Wallet["gems"]:n0}";
        
        // Clear existing shop items from UI.
        foreach (Transform child in _storeGroupLarge)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in _storeGroupSmall)
        {
            Destroy(child.gameObject);
        }
        
        // Update shop items in UI.
        var largeStoreItemCategories = new[] {"cosmetics"};
        foreach (var item in system.StoreItems.OrderBy(x => x.Name))
        {
            var isLargeStoreItem = largeStoreItemCategories.Contains(item.Category);
            var prefab = isLargeStoreItem ? _storeItemLargePrefab : _storeItemSmallPrefab;
            var parent = isLargeStoreItem ? _storeGroupLarge : _storeGroupSmall;
            var storeItem = Instantiate(prefab, parent);

            var cost = item.Cost.Currencies.First();
            storeItem.Init(item.Category, item.Id, item.Name, cost.Key, int.Parse(cost.Value));
            storeItem.OnClick += OnStoreItemClick;
        }
    }

    private async void OnStoreItemClick(string itemId)
    {
        try
        {
            var purchaseAck = await _economySystem.PurchaseStoreItemAsync(itemId);
            await _economySystem.RefreshAsync();
            
            _rewardPanel.Show(new [] { purchaseAck.Reward });
        }
        catch (Exception)
        {
            _errorPanel.SetActive(true);
        }
    }
}
