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
using System.Threading.Tasks;
using Hiro;
using Hiro.System;
using Nakama.TinyJson;
using UnityEngine;
using ILogger = Nakama.ILogger;

public class CollectibleCardSystem : ObservableSystem<CollectibleCardSystem>, IInitializeSystem
{
    public string Name => nameof(CollectibleCardSystem);
    public bool IsInitialized { get; private set; }

    public Dictionary<string, List<CardCost>> CardCosts => _cardCosts;
    public Dictionary<string, List<CardStat>> CardStats => _cardStats;
    public Dictionary<string, Card> Cards => _cards;

    private readonly INakamaSystem _nakamaSystem;
    private Dictionary<string, List<CardCost>> _cardCosts = new();
    private Dictionary<string, List<CardStat>> _cardStats = new();
    private Dictionary<string, Card> _cards = new();

    public CollectibleCardSystem(ILogger logger, INakamaSystem nakamaSystem) : base(logger)
    {
        _nakamaSystem = nakamaSystem;
    }
    
    public Task InitializeAsync()
    {
        if (_nakamaSystem == null || !_nakamaSystem.IsInitialized)
        {
            throw new InvalidOperationException("Must initialize the NakamaSystem first");
        }

        IsInitialized = true;
        return Task.CompletedTask;
    }

    public Task InitializeFailedAsync(ILogger logger, Exception e) => Task.CompletedTask;

    public async Task GetCardsAsync()
    {
        var result = await _nakamaSystem.Client.RpcAsync(_nakamaSystem.Session, "InventoryListCards");
        var cardList = result.Payload.FromJson<CardList>();
        
        _cardCosts = cardList.CardCosts;
        _cardStats = cardList.CardStats;
        _cards = cardList.Cards;
        
        NotifyObservers();
    }

    public async Task UpgradeCardAsync(string itemId)
    {
        var payload = new Dictionary<string, string>
        {
            {"itemId", itemId}
        };
        var result = await _nakamaSystem.Client.RpcAsync(_nakamaSystem.Session, "InventoryCardUpgrade", payload.ToJson());
        var cardList = result.Payload.FromJson<CardList>();
        
        _cardCosts = cardList.CardCosts;
        _cardStats = cardList.CardStats;
        _cards = cardList.Cards;
        
        NotifyObservers();
    }
}
