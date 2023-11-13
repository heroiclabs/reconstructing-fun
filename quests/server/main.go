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
	"errors"
	"fmt"

	"github.com/heroiclabs/hiro"
	"github.com/heroiclabs/nakama-common/runtime"
)

var (
	errBadInput        = runtime.NewError("input contained invalid data", 3) // INVALID_ARGUMENT
	errInternal        = runtime.NewError("internal server error", 13)       // INTERNAL
	errMarshal         = runtime.NewError("cannot marshal type", 13)         // INTERNAL
	errNoInputAllowed  = runtime.NewError("no input allowed", 3)             // INVALID_ARGUMENT
	errNoInputGiven    = runtime.NewError("no input was given", 3)           // INVALID_ARGUMENT
	errNoUserIdFound   = runtime.NewError("no user ID in context", 3)        // INVALID_ARGUMENT
	errNoUsernameFound = runtime.NewError("no username in context", 3)       // INVALID_ARGUMENT
	errUnmarshal       = runtime.NewError("cannot unmarshal type", 13)       // INTERNAL
)

func InitModule(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, initializer runtime.Initializer) error {
	props, ok := ctx.Value(runtime.RUNTIME_CTX_ENV).(map[string]string)
	if !ok {
		return errors.New("invalid context runtime env")
	}

	env, ok := props["ENV"]
	if !ok || env == "" {
		return errors.New("'ENV' key missing or invalid in env")
	}

	hiroLicense, ok := props["HIRO_LICENSE"]
	if !ok || hiroLicense == "" {
		return errors.New("'HIRO_LICENSE' key missing or invalid in env")
	}

	binPath := "hiro.bin"
	systems, err := hiro.Init(ctx, logger, nk, initializer, binPath, hiroLicense,
		hiro.WithAchievementsSystem(fmt.Sprintf("base-achievements-%s.json", env), true),
		hiro.WithEconomySystem(fmt.Sprintf("base-economy-%s.json", env), true),
		hiro.WithInventorySystem(fmt.Sprintf("base-inventory-%s.json", env), true))
	if err != nil {
		return err
	}

	return nil
}
