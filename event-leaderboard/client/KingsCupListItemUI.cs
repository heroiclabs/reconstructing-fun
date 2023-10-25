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

using Hiro;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KingsCupListItemUI : MonoBehaviour
{
    [SerializeField] private Image rankIcon;
    [SerializeField] private Sprite firstPlaceRankIcon;
    [SerializeField] private Sprite secondPlaceRankIcon;
    [SerializeField] private Sprite thirdPlaceRankIcon;
    [SerializeField] private TMP_Text rankLabel;
    [SerializeField] private TMP_Text usernameLabel;
    [SerializeField] private TMP_Text scoreLabel;
    [SerializeField] private Image chestIcon;
    [SerializeField] private Sprite firstPlaceChestIcon;
    [SerializeField] private Sprite secondPlaceChestIcon;
    [SerializeField] private Sprite thirdPlaceChestIcon;
    [SerializeField] private Sprite otherChestIcon;
    
    public void Init(IEventLeaderboardScore score)
    {
        usernameLabel.text = score.Username;
        scoreLabel.text = score.Score.ToString();

        switch (score.Rank)
        {
            case 1:
                rankIcon.sprite = firstPlaceRankIcon;
                chestIcon.sprite = firstPlaceChestIcon;
                break;
            case 2:
                rankIcon.sprite = secondPlaceRankIcon;
                chestIcon.sprite = secondPlaceChestIcon;
                break;
            case 3:
                rankIcon.sprite = thirdPlaceRankIcon;
                chestIcon.sprite = thirdPlaceChestIcon;
                break;
            default:
                rankIcon.gameObject.SetActive(false);
                rankLabel.text = score.Rank.ToString();
                rankLabel.gameObject.SetActive(true);
                chestIcon.sprite = otherChestIcon;
                chestIcon.gameObject.SetActive(true);
                break;
        }

        if (score.Rank > 10)
        {
            chestIcon.gameObject.SetActive(false);
        }
    }
}