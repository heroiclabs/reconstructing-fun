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
using UnityEngine;

[CreateAssetMenu(menuName = "Reconstructing Fun/Kings Cup/Reward Items")]
public class RewardItems : ScriptableObject
{
    public List<RewardItem> items = new();

    public RewardItem Get(string id)
    {
        return items.FirstOrDefault(item => item.id == id);
    }
}

[Serializable]
public class RewardItem
{
    public string id;
    public string name;
    public Sprite sprite;
}
