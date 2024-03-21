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
using Hiro;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    public event Action<IInventoryItem> Clicked;
    
    [SerializeField] private Image _slotFrame;
    [SerializeField] private Image _slotItem;
    [SerializeField] private TMP_Text _slotCount;
    [SerializeField] private Sprite _commonFrame;
    [SerializeField] private Sprite _rareFrame;
    [SerializeField] private Sprite _legendaryFrame;
    [SerializeField] private RewardItems _rewardItems;

    private IInventoryItem _currentItem;
    
    public void SetItem(IInventoryItem item)
    {
        _currentItem = item;
        
        var rewardItem = _rewardItems.Get(item.Id);
        if (rewardItem == null)
        {
            Debug.Log($"Could not find data for item: {item.Id}");
            return;
        }

        var rarity = item.StringProperties["rarity"];
        _slotFrame.sprite = rarity switch
        {
            "rare" => _rareFrame,
            "legendary" => _legendaryFrame,
            _ => _commonFrame
        };
        _slotCount.text = item.Count.ToString("N0");
        _slotItem.sprite = rewardItem.sprite;
    }

    public void OnClicked()
    {
        if (_currentItem == null)
        {
            return;
        }
        
        Clicked?.Invoke(_currentItem);
    }
}
