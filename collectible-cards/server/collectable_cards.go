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

package main

import (
	"context"
	"database/sql"
	"encoding/json"
	"fmt"
	"io"

	"github.com/heroiclabs/hiro"
	"github.com/heroiclabs/nakama-common/runtime"
)

type CollectibleCardsDefinition struct {
	RarityStatsLadder     map[string][]*RarityStats    `json:"rarityStatsLadder"`
	RarityCardCostsLadder map[string][]*RarityCardCost `json:"rarityCardCostsLadder"`
}

type RarityStats struct {
	Points      int `json:"points"`
	Probability int `json:"probability"`
}

type RarityCardCost struct {
	CardCount int `json:"cardCount"`
	Coins     int `json:"coins"`
}

func NewCollectibleCardDefinition(nk runtime.NakamaModule, path string) (*CollectibleCardsDefinition, error) {
	file, err := nk.ReadFile(path)
	if err != nil {
		return nil, err
	}
	defer file.Close()

	bytes, err := io.ReadAll(file)
	if err != nil {
		return nil, err
	}

	definition := &CollectibleCardsDefinition{
		RarityStatsLadder:     make(map[string][]*RarityStats),
		RarityCardCostsLadder: make(map[string][]*RarityCardCost),
	}
	if err = json.Unmarshal(bytes, definition); err != nil {
		return nil, err
	}

	return definition, nil
}

type InventoryCardLevelUpRequest struct {
	ItemID string `json:"itemId"`
}

type InventoryListCards struct {
	CardStats map[string][]*RarityStats      `json:"cardStats"`
	CardCosts map[string][]*RarityCardCost   `json:"cardCosts"`
	Cards     map[string]*hiro.InventoryItem `json:"cards"`
}

func InventoryListCardsRpcFn(systems hiro.Hiro, definitions *CollectibleCardsDefinition) func(context.Context, runtime.Logger, *sql.DB, runtime.NakamaModule, string) (string, error) {
	return func(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, payload string) (string, error) {
		userID, ok := ctx.Value(runtime.RUNTIME_CTX_USER_ID).(string)
		if !ok {
			return "", errNoUserIdFound
		}

		if payload != "" {
			return "", errNoInputAllowed
		}

		inventory, err := systems.GetInventorySystem().ListInventoryItems(ctx, logger, nk, userID, "cards")
		if err != nil {
			logger.Error("failed to list inventory: %v", err)
			return "", errInternal
		}

		// Merge in computed fields.
		for _, item := range inventory.GetItems() {
			if item.GetStringProperties() == nil {
				item.StringProperties = make(map[string]string)
			}
			if item.GetNumericProperties() == nil {
				item.NumericProperties = make(map[string]float64)
			}

			rarity, ok := item.GetStringProperties()["card_rarity"]
			if !ok {
				logger.Error("item %q is incomplete", item.GetId())
				break
			}

			rarityCardCosts, ok := definitions.RarityCardCostsLadder[rarity]
			if !ok {
				logger.Error("item %q has unrecognised rarity %q", item.GetId(), rarity)
				break
			}

			spent, _ := item.GetNumericProperties()["card_spent"]
			spentCards := int(spent)
			for i, cardCost := range rarityCardCosts {
				spentCards -= cardCost.CardCount
				if spentCards <= 0 {
					// Add computed properties to card instance.
					item.GetNumericProperties()["card_level"] = float64(i)

					rarityStats := definitions.RarityStatsLadder[rarity][i]
					item.GetNumericProperties()["card_points"] = float64(rarityStats.Points)
					item.GetNumericProperties()["card_probability"] = float64(rarityStats.Probability)

					break
				}
			}

			// If there are more levels, compute the next upgrade cost
			nextLevel := int(item.GetNumericProperties()["card_level"]) + 1
			if nextLevel < len(rarityCardCosts) {
				item.GetNumericProperties()["card_upgrade_count_cost"] = float64(rarityCardCosts[nextLevel].CardCount)
				item.GetNumericProperties()["card_upgrade_coin_cost"] = float64(rarityCardCosts[nextLevel].Coins)
			}
		}

		inventoryCards := &InventoryListCards{
			CardStats: definitions.RarityStatsLadder,
			CardCosts: definitions.RarityCardCostsLadder,
			Cards:     inventory.GetItems(),
		}

		resp, err := json.Marshal(inventoryCards)
		if err != nil {
			logger.Error("failed to marshal type: %v", err)
			return "", errMarshal
		}

		return string(resp), nil
	}
}

func InventoryCardUpgradeRpcFn(systems hiro.Hiro, definitions *CollectibleCardsDefinition) func(context.Context, runtime.Logger, *sql.DB, runtime.NakamaModule, string) (string, error) {
	return func(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, payload string) (string, error) {
		userID, ok := ctx.Value(runtime.RUNTIME_CTX_USER_ID).(string)
		if !ok {
			return "", errNoUserIdFound
		}

		if payload == "" {
			return "", errNoInputGiven
		}

		req := &InventoryCardLevelUpRequest{}
		if err := json.Unmarshal([]byte(payload), req); err != nil {
			return "", errUnmarshal
		}

		if req.ItemID == "" {
			return "", runtime.NewError("itemId cannot be empty", 3)
		}

		inventory, err := systems.GetInventorySystem().ListInventoryItems(ctx, logger, nk, userID, "cards")
		if err != nil {
			logger.Error("inventory system error: %v", err)
			return "", errInternal
		}

		notFound := true
		for instanceId, item := range inventory.GetItems() {
			if item.GetStringProperties() == nil {
				item.StringProperties = make(map[string]string)
			}
			if item.GetNumericProperties() == nil {
				item.NumericProperties = make(map[string]float64)
			}

			rarity, ok := item.GetStringProperties()["card_rarity"]
			if !ok {
				logger.Error("item definition %q is incomplete", item.GetId())
				break
			}

			rarityCardCosts, ok := definitions.RarityCardCostsLadder[rarity]
			if !ok {
				logger.Error("item definition %q has unrecognised rarity %q", item.GetId(), rarity)
				break
			}

			spent, _ := item.GetNumericProperties()["card_spent"]
			spentCards := int(spent)
			for i, cardCost := range rarityCardCosts {
				spentCards -= cardCost.CardCount
				if spentCards <= 0 {
					// Add computed properties to card instance.
					item.GetNumericProperties()["card_level"] = float64(i)

					rarityStats := definitions.RarityStatsLadder[rarity][i]
					item.GetNumericProperties()["card_points"] = float64(rarityStats.Points)
					item.GetNumericProperties()["card_probability"] = float64(rarityStats.Probability)

					break
				}
			}

			if req.ItemID == item.GetId() {
				nextLevel := int(item.GetNumericProperties()["card_level"]) + 1
				if nextLevel >= len(rarityCardCosts) {
					return "", runtime.NewError(fmt.Sprintf("card %q cannot be upgraded to %q level", req.ItemID, nextLevel), 3)
				}

				cardCosts := rarityCardCosts[nextLevel]

				if item.GetCount() <= int64(cardCosts.CardCount) {
					return "", runtime.NewError(fmt.Sprintf("not enough cards to upgrade %q card", req.ItemID), 3)
				}

				currencies := map[string]int64{
					"coins": int64(cardCosts.Coins) * -1,
				}
				items := map[string]int64{
					item.GetId(): int64(cardCosts.CardCount),
				}

				if _, _, err = systems.GetInventorySystem().ConsumeItems(ctx, logger, nk, userID, items, false); err != nil {
					switch err {
					case hiro.ErrItemsInsufficient:
						return "", runtime.NewError(err.Error(), 3)
					default:
						logger.Error("failed to upgrade %q card: %q", req.ItemID, err.Error())
						return "", errInternal
					}
				}

				if _, err = systems.GetEconomySystem().Grant(ctx, logger, nk, userID, currencies, nil, make([]*hiro.RewardModifier, 0)); err != nil {
					switch err {
					case hiro.ErrCurrencyInsufficient:
						return "", runtime.NewError(err.Error(), 3)
					default:
						logger.Error("failed to upgrade %q card", req.ItemID)
						return "", errInternal
					}
				}

				item.Count -= int64(cardCosts.CardCount)
				item.GetNumericProperties()["card_spent"] += float64(cardCosts.CardCount)

				itemUpdates := map[string]*hiro.InventoryUpdateItemProperties{
					instanceId: {
						NumericProperties: map[string]float64{
							"card_spent": item.GetNumericProperties()["card_spent"],
						},
					},
				}

				if _, err = systems.GetInventorySystem().UpdateItems(ctx, logger, nk, userID, itemUpdates); err != nil {
					logger.Error("failed to update card property: %s", err)
					return "", errInternal
				}

				// Recalculate computed properties after upgrade
				spent, _ := item.GetNumericProperties()["card_spent"]
				spentCards := int(spent)
				for i, cardCost := range rarityCardCosts {
					spentCards -= cardCost.CardCount
					if spentCards <= 0 {
						// Add computed properties to card instance.
						item.GetNumericProperties()["card_level"] = float64(i)

						rarityStats := definitions.RarityStatsLadder[rarity][i]
						item.GetNumericProperties()["card_points"] = float64(rarityStats.Points)
						item.GetNumericProperties()["card_probability"] = float64(rarityStats.Probability)

						break
					}
				}

				notFound = false
			}

			// If there are more levels, compute the next upgrade cost
			nextLevel := int(item.GetNumericProperties()["card_level"]) + 1
			if nextLevel < len(rarityCardCosts) {
				item.GetNumericProperties()["card_upgrade_count_cost"] = float64(rarityCardCosts[nextLevel].CardCount)
				item.GetNumericProperties()["card_upgrade_coin_cost"] = float64(rarityCardCosts[nextLevel].Coins)
			}
		}

		if notFound {
			return "", runtime.NewError(fmt.Sprintf("user does not own %q card", req.ItemID), 3)
		}

		inventoryCards := &InventoryListCards{
			CardStats: definitions.RarityStatsLadder,
			CardCosts: definitions.RarityCardCostsLadder,
			Cards:     inventory.GetItems(),
		}

		resp, err := json.Marshal(inventoryCards)
		if err != nil {
			logger.Error("failed to marshal type: %v", err)
			return "", errMarshal
		}

		return string(resp), nil
	}
}
