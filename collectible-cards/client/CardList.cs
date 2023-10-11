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
using System.Runtime.Serialization;
using Hiro;

[Serializable]
public class CardList
{
    [DataMember(Name = "cardStats"), Preserve]
    public Dictionary<string, List<CardStat>> CardStats { get; set; }
    
    [DataMember(Name = "cardCosts"), Preserve]
    public Dictionary<string, List<CardCost>> CardCosts { get; set; }
    
    [DataMember(Name = "cards"), Preserve]
    public Dictionary<string, Card> Cards { get; set; }
}

[Serializable]
public class CardCost
{
    [DataMember(Name = "cardCount"), Preserve]
    public int CardCount { get; set; }
    
    [DataMember(Name = "coins"), Preserve]
    public long Coins { get; set; }
}

[Serializable]
public class CardStat
{
    [DataMember(Name = "points"), Preserve]
    public int Points { get; set; }
    
    [DataMember(Name = "probability"), Preserve]
    public int Probability { get; set; }
}

[Serializable]
public class Card
{
    [DataMember(Name = "id"), Preserve]
    public string Id { get; set; }
    
    [DataMember(Name = "name"), Preserve]
    public string Name { get; set; }
    
    [DataMember(Name = "count"), Preserve]
    public int Count { get; set; }
    
    [DataMember(Name = "string_properties"), Preserve]
    public Dictionary<string, string> StringProperties { get; set; }
    
    [DataMember(Name = "numeric_properties"), Preserve]
    public Dictionary<string, double> NumericProperties { get; set; }
}