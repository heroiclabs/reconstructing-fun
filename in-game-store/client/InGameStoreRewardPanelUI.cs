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
using Hiro;
using UnityEngine;

public class InGameStoreRewardPanelUI : MonoBehaviour
{
    [SerializeField] private Transform _rewardContainer;
    [SerializeField] private InGameStoreRewardItemUI _storeRewardItemPrefab;
    
    public void Show(IEnumerable<IReward> rewards)
    {
        // Clear existing reward panel UI.
        foreach (Transform child in _rewardContainer)
        {
            Destroy(child.gameObject);
        }
        
        // Update the reward list.
        foreach (var reward in rewards)
        {
            foreach (var item in reward.Items)
            {
                var rewardItem = Instantiate(_storeRewardItemPrefab, _rewardContainer);
                rewardItem.Init(item.Key, item.Value);
            }

            foreach (var energyModifier in reward.EnergyModifiers)
            {
                var rewardItem = Instantiate(_storeRewardItemPrefab, _rewardContainer);
                var text = energyModifier.Operator == "infinite" ? "∞" : energyModifier.Value.ToString();

                var timespan = TimeSpan.FromSeconds(energyModifier.DurationSec);
                text += $" ({timespan.TotalMinutes}m) ";

                rewardItem.Init(energyModifier.Id, text);
            }

            foreach (var currency in reward.Currencies)
            {
                var rewardItem = Instantiate(_storeRewardItemPrefab, _rewardContainer);
                rewardItem.Init(currency.Key, currency.Value);
            }
        }
        
        // Show the panel.
        gameObject.SetActive(true);
    }
}
