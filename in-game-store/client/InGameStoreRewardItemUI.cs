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

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameStoreRewardItemUI : MonoBehaviour
{
    [SerializeField] private GameObject highlight;
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text label;
    [SerializeField] private TMP_Text quantity;
    [SerializeField] private RewardItems rewardItems;
    
    public void Init(string id, string text, bool shouldHighlight = false)
    {
        var rewardItem = rewardItems.Get(id);
        if (rewardItem == null)
        {
            rewardItem = rewardItems.Get("default");
        }

        icon.sprite = rewardItem.sprite;
        label.text = rewardItem.name;
        quantity.text = text;
        highlight.SetActive(shouldHighlight);
    }
}
