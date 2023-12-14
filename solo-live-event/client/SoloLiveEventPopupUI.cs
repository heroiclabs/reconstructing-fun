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
using UnityEngine;

public class SoloLiveEventPopupUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _timerText;

    private long _expiryTimeSec;
    
    public void Init(string name, long expiryTimeSec)
    {
        _titleText.text = name;
        _expiryTimeSec = expiryTimeSec;
    }

    private void Update()
    {
        if (_expiryTimeSec <= 0)
        {
            return;
        }

        var timeNow = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
        var timeRemaining = TimeSpan.FromSeconds(_expiryTimeSec - timeNow);

        _timerText.text = $"{timeRemaining.Days}d {timeRemaining.Hours}h {timeRemaining.Minutes}m";
    }
}
