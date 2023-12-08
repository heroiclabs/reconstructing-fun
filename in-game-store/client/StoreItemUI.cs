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
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StoreItemUI : MonoBehaviour
{
    public event Action<string> OnClick;
    
    [SerializeField] private TMP_Text _categoryLabel;
    [SerializeField] private TMP_Text _nameLabel;
    [SerializeField] private TMP_Text _costLabel;
    [SerializeField] private Image _itemImage;
    [SerializeField] private Image _costImage;
    [SerializeField] private RewardItems _rewardItems;

    private string _itemId;

    public void Init(string category, string itemId, string name, string currencyId, int cost)
    {
        _itemId = itemId;
        
        if (_categoryLabel != null)
        {
            _categoryLabel.text = category.ToUpper();
        }

        _nameLabel.text = name;

        var rewardItem = _rewardItems.Get(itemId);
        if (rewardItem != null)
        {
            _itemImage.sprite = _rewardItems.Get(itemId).sprite;
        }
        else
        {
            Debug.Log($"No sprite found for {itemId}, using default.");
            _itemImage.sprite = _rewardItems.Get("default").sprite;
        }

        _costLabel.text = $"{cost:n0}";
        _costImage.sprite = _rewardItems.Get(currencyId).sprite;
    }

    public void Click()
    {
        OnClick?.Invoke(_itemId);
    }
}
