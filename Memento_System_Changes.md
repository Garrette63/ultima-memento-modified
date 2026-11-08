# Memento System – Change Tracker

Workspace root: `/home/Feesh/World`

This document tracks every file created or edited for the Memento Coin / Artefact Book / Memento Collector feature set, plus later custom-shard changes (such as the Magery-powered magic carpet speed rule). Update it whenever further changes are made.

---

## Files Created

| File (relative to `/home/Feesh/World`) | Kind | Purpose |
|---|---|---|
| `Source/Scripts/Items/Books/ArtefactBook.cs` | C# item + gump | `ArtefactBook` item (displayed as "Tome of Wondrous Artifacts") and `ArtefactBookGump`. Lists every standard artefact (`ArtifactLevel.StandardArtefact`) from `ArtifactQuestList` with no in-book title, 16 per page, prev/next arrows and just the page number at the bottom. Clicking an artefact opens a detail page with the icon on the left page and the stat sheet on the right page (`ItemProps.ItemProperties`), plus a curated "Unique Power" description for artefacts with special code-driven abilities. Uses shared `MementoCoin.MementoHue`. `ArtefactBook.ArtefactEntry` carries `Name`, `TypeName` and `Layer`. |
| `Source/Scripts/Items/Misc/MementoCoin.cs` | C# item | `MementoCoin` stackable currency (graphic `0xEED`). Defines the shared `public const int MementoHue = 2141`. |
| `Source/Scripts/Mobiles/Civilized/Merchants/MementoCollector.cs` | C# NPC | `MementoCollector : BaseNPC` (invulnerable / untargetable). Pays Memento Coins for dropped artefacts, trades 75 coins for an `ArtefactBook`, has a Talk gump, random male/female + name, leather outfit, cloak/sash/boots, lit lantern, idle emotes, random artefact gear swaps and a random one-handed artefact weapon. |
| `Compile-Linux.sh` | Shell script (shortcut) | Linux shortcut in the `World` folder. Resolves the repo base folder (parent of `World`), runs `git pull` there, then `cd`s to `Source/Tools` and runs `compile-world-linux.sh` (Mono `mcs` → `WorldLinux.exe`). Aborts before compiling if the pull fails. Marked executable (`+x`). |
| `Compile-Windows.bat` | Batch script (shortcut) | Windows shortcut in the `World` folder. Resolves the repo base folder (parent of `World`) via `%~dp0..`, runs `git pull` there, then `pushd`s to `Source\Tools` and `call`s `compile-world-win.bat` (.NET 4.0 `csc` → `World.exe`). Aborts before compiling if the pull fails. |

## Files Edited

| File (relative to `/home/Feesh/World`) | Change |
|---|---|
| `Source/Scripts/System/Misc/Talk.cs` | Added a `SpeechFunctions.SpeechText` branch for `sConversation == "MementoCollector"` (around line 202) explaining the coin payouts and the 75-coin → Artefact Book exchange. |
| `Info/Spawns/Merchant.map` | Added one `MementoCollector` spawner beside each of the 34 custom-merchant stalls (same x/y/z/map as the merchant, home/walk 2, count 1). Applied on the next `[buildworld`. Original backed up as `Merchant.map.bak`. |
| `Source/Scripts/Items/Boats/BaseBoat.cs` | `ApplySpeedBoost` selects the speed skill per vessel: `SkillName.Seafaring` for sailing vessels (Magery has no effect) and `SkillName.Magery` for magic carpets, detected with the existing `isCarpet( this )` (Seafaring has no effect). Same 50/75/100/125 tiers (1/2/3/4 boosts). `MAX_SPEED_BOOSTS` raised to 6 (Avatar "Fast Seaman" can reach 6); magic carpets are additionally clamped to `MAX_CARPET_SPEED_BOOSTS = 4`. Backed up as `Backup Files/BaseBoat.cs.2026-11-07-140056.bak` (first edit) and `Backup Files/BaseBoat.cs.2026-11-07-141352.bak` (this edit). |
| `Compile-Linux.sh` | Double-click support: prepends Homebrew/Linuxbrew `bin` + the standard bin dirs to `PATH` (GUI launches don't read `~/.bashrc`), and re-execs itself in `konsole -e bash …` when launched without a TTY so the build output is visible and the window closes automatically when the build finishes. Console (TTY) runs are unaffected. Backed up as `Backups/Compile-Linux.sh.2026-11-07-193557.bak` (initial) and `Backups/Compile-Linux.sh.2026-11-07-193951.bak` (auto-close). |
| `~/.config/kiorc` (per-user, not in repo) | Added `[Executable scripts] behaviourOnLaunch=execute` so KDE/KIO **executes** executable scripts on double-click instead of opening them in the text editor (this is the `KIO::WidgetsOpenOrExecuteFileHandler` memo, default `alwaysAsk`). |
| `Source/Scripts/System/Regions/Core/World.cs` | Fixed `GetDungeonListing(int)` cases 61/62 to use the canonical names `"the Dungeon of the Lich King"` / `"the Dungeon of the Mad Archmage"` (leading "the") so they match `GetAreaEntrance`'s `zone == "the …"` entries — previously those two guide rows resolved to location (0,0) and opened the map at the world origin. Also added `public const int DungeonCount = 85;` to the `Worlds` class. Backed up as `Backups/World.cs.2026-11-07-222342.bak`. |
| `Source/Scripts/Items/Games/DandD/DungeonMastersGuide.cs` | `DMGuideGump` loop bound now uses `Server.Misc.Worlds.DungeonCount` instead of the literal `85`; `OnResponse` now checks `Sextants.HasSextant( from )` before opening `Sextants.MapGump` and sends "You need a sextant to locate this dungeon." otherwise (prevents the empty gump window). Backed up as `Backups/DungeonMastersGuide.cs.2026-11-07-222342.bak`. |
| `Source/Scripts/System/Chat/General/General.cs` | Added the missing `EventSink.ChatRequest` subscription (`OnChatRequest`) in `Initialize()`, so the client's chat button (packet `0xB5`) now behaves exactly like `[c`: it routes through the Public channel (`Channel.GetByType(typeof(Public)).OnChat(m, "")`) to join/set the current channel and open the channel chat view (falls back to `General.List(m, 0)` only if Public is missing). Backed up as `Backups/General.cs.2026-11-07-230824.bak` (first edit) and `Backups/General.cs.2026-11-07-232515.bak` (button routes via Public). |
| `Source/Scripts/System/Chat/Channels/Channel.cs` | `Channel.Load()` now calls `PredefinedChannels()` in its `catch`, so a missing/stale `Channels.bin` can never leave chat with zero channels (which would also mean the `[c`/`[chat` commands never register). Backed up as `Backups/Channel.cs.2026-11-07-231000.bak`. |
| `Source/Scripts/Mobiles/Civilized/Merchants/CustomHairstylist.cs` | `HairstylistBuyGump` (the barber/hair-stylist service menu opened by the `BarberEntry` context option) previously built rows **only for services the player could currently afford** (`balance >= Price`), so a character with less than the cheapest price (50g) — e.g. a fresh/GM character — saw an empty gump. It now lists all applicable services (still hiding facial-hair options for female characters); clicking an unaffordable one shows the existing "You cannot afford my services for that style." message. Backed up as `Backups/CustomHairstylist.cs.2026-11-07-235505.bak`. |

No other files were modified.

---

## Change History (newest last)

1. **Artefact Book picker** — created `ArtefactBook.cs`: a book that opens a paginated gump listing every standard artefact and lets the player claim one. Default `Charges = 1` (negative = unlimited).
2. **Memento Collector + currency** — created `MementoCoin.cs` and `MementoCollector.cs`. The NPC pays coins by artefact tier and the coin is a stackable item.
3. **75 coins → Artefact Book + Talk dialogue** — `MementoCollector.OnDragDrop` now handles `MementoCoin` drops (75+ coins → `ArtefactBook`, extras refunded); added `TalkGumpTitle`/`TalkGumpSubject` and the `Talk.cs` dialogue branch.
4. **Random name** — the NPC now takes a standard `NameList.RandomName(...)` name; title stays `"the artefact collector"`.
5. **Gender / outfit pass** — random male or female (matching body + name); added a lit lantern; replaced robe/shoes with leather armour; added cloak, sash and boots; female bustiers used instead of the leather chest; no arm armour at that stage.
6. **Hue + idle actions + random gear** — shared hue changed to `2141` for the book, coins, cloak, sash and lantern; leather cap removed; leather armour dyed dark gray (`0x83B`); added adventurer-style idle emotes (yawn, sigh, etc.); equipment randomly swapped for standard artefacts on spawn.
7. **Cap/sleeves, wandering, no bustier rule, wieldable artefact** — re-added the leather cap and leather sleeves (`LeatherArms`); removed `CantWalk` so the NPC wanders; removed the female bustier special-case (everyone wears `LeatherChest`); the NPC now wields a random one-handed standard artefact weapon alongside the lantern.
8. **Artefact detail page** — clicking an artefact in the `ArtefactBook` list no longer grants it immediately. It now opens a detail page showing the artefact's icon and full stats/powers (via `ItemProps.ItemProperties`), with Back and Claim buttons; Claim performs the actual grant and charge consumption.
9. **Detail layout + unique powers** — the detail page now places the artefact picture on the left page and all information on the right page, and prepends a "UNIQUE POWER" description for artefacts with special behaviours (e.g., Merlin's Mystical Staff calls a dragon on double-click, Hammer of Thor casts lightning, Rod of Resurrection revives the fallen, etc.).
10. **Detail title placement** — the artefact name is now centred at the top of the left page, directly over the artefact picture (instead of spanning the whole book).
11. **Cloth hat** — the collector now wears a floppy cloth hat dyed the Memento hue (2141) to match the cloak and sash, replacing the dark-gray leather cap.
12. **Random hat type** — the collector now picks any of several cloth hat styles (floppy, wide-brim, cap, skull cap, bandana, straw, tall straw, wizard, feathered, tricorne, pirate, hood, witch, bonnet), all dyed hue 2141.
13. **Vendor team** — the collector is explicitly placed on the neutral team `0` used by town vendors, and re-asserts it in `OnAfterSpawn` so a spawner cannot move it to a different team.
14. **Custom merchant spawns** — added one `MementoCollector` spawner per merchant/wagon stall in `Info/Spawns/Merchant.map` (34 total), so a collector stands beside every custom merchant. Applies on the next `[buildworld`.
15. **Coin singular/plural** — a single `MementoCoin` now displays as "Memento Coin"; stacks of two or more display as "Memento Coins". The internal `Name` stays plural so stacks still merge.
16. **Merchant name colour** — the collector's name now uses the innocent/vendor blue (`Notoriety.GetHue( Notoriety.Innocent )`) instead of the guard white (`1154`), matching the other merchants.
17. **Vendor vulnerability** — removed the transient `Blessed` flag (which `OnThink` was clearing anyway) and turned off `AlwaysAttackable`; `NameHue` is back to the default so notoriety drives the colour. The collector is now vulnerable and Innocent exactly like a regular vendor.
18. **Invulnerable / untargetable** — reverted the `BaseCreature` edit; `MementoCollector` now derives from `BaseNPC` (already in the always-invulnerable set), so its `Blessed` flag persists and it cannot be targeted or harmed. `Deserialize` re-asserts `Blessed`, and `NameHue` keeps the merchant blue.
19. **Tome rename + page number** — the book is now named "Tome of Wondrous Artifacts"; the "STANDARD ARTEFACTS" heading inside the book was removed, and the page indicator is now just the page number at the bottom next to the arrows. The NPC dialogue/messages were updated to say "Tome of Wondrous Artifacts".
20. **Page indicator position** — the page counter is now right-aligned as "Page X of Y" just left of the next-page arrow (bottom right).
21. **Detail Claim button** — on the artefact detail page the Claim action now uses a checkmark-box button (`0x837`/`0x838`) in the arrow's place, with the "Claim" label right-aligned next to it.
22. **Magery-powered magic carpets** — `BaseBoat.ApplySpeedBoost` now selects `SkillName.Magery` instead of `SkillName.Seafaring` when the boat is a magic carpet (`isCarpet( this )`); sailing vessels still use Seafaring. The 50/75/100/125 thresholds still grant 1/2/3/4 boosts and the maximum remains capped at 4 by `MAX_SPEED_BOOSTS`, so carpets can never gain more than 4 bonus speed steps. The Avatar "Fast Seaman" boat-speed upgrade still applies to both vessel types, but the combined result is clamped to 4.
23. **Speed caps 6 / carpet 4 + explicit skill isolation** — `MAX_SPEED_BOOSTS` raised from 4 to 6, so the Avatar "Fast Seaman" upgrade (via `Constants.BOAT_SPEED_MAX_LEVEL`) can now reach 6 on regular boats; added `MAX_CARPET_SPEED_BOOSTS = 4` and a carpet-only clamp in `ApplySpeedBoost`, so magic carpets never exceed 4 bonus speed steps regardless of Magery or Avatar values. Boats are driven only by Seafaring (Magery has no effect) and carpets only by Magery (Seafaring has no effect).
24. **Compile shortcuts (Linux + Windows)** — added `Compile-Linux.sh` and `Compile-Windows.bat` in the `World` folder. Each computes the repository base folder (the parent of `World`), runs `git pull` there, then runs the matching compiler from `Source/Tools` (`compile-world-linux.sh` / `compile-world-win.bat`). If `git pull` fails the shortcut stops before compiling so stale/conflicting code is never built. The Linux shortcut is executable; `git pull` is best run with a clean working tree.
25. **Linux double-click fix (desktop launcher + GUI PATH)** — double-clicking the `.sh` in KDE Dolphin opened it in a text editor, and when run from the GUI `mcs`/`mono`/`csc` were missing from `PATH` (they live in Linuxbrew, added only by `~/.bashrc`). Added `World/Compile-Linux.desktop`: a `Terminal=true` KDE launcher that runs the script in Konsole and keeps the window open for the result (executable + trusted, also registered in the app menu as "Compile World (Linux)"). Hardened `Compile-Linux.sh` to prepend Homebrew/Linuxbrew `bin` and the standard bin dirs to `PATH`, and to pause on exit when launched from the desktop (`COMPILE_SHORTCUT_PAUSE=1`). Verified the full `git pull` → `mcs` compile flow under a stripped GUI-like environment (`env -i`, minimal `PATH`) with exit 0. `Compile-Linux.sh` backed up as `Backups/Compile-Linux.sh.2026-11-07-192511.bak`.
26. **Reverted the `.desktop` launcher (scripts only)** — per request, removed `World/Compile-Linux.desktop` and its `~/.local/share/applications` registration, and restored `Compile-Linux.sh` to the plain git-pull → compile script (dropping the `.desktop`-only `PATH`/pause additions). The shortcuts are again plain scripts: `Compile-Linux.sh` (Linux) and `Compile-Windows.bat` (Windows).
27. **Double-click now runs the `.sh` (KIO execute + Konsole relaunch)** — on KDE Plasma 6 a `.sh` opened in the editor on double-click because KIO's executable-script memo was set to open-in-application. Set `~/.config/kiorc` `[Executable scripts] behaviourOnLaunch=execute` (confirmed from KIO's `WidgetsOpenOrExecuteFileHandler`) so Dolphin executes executable scripts. Because KIO executes without a terminal (and a session `PATH` lacking Linuxbrew), `Compile-Linux.sh` now adds Homebrew/Linuxbrew/standard bin dirs to `PATH` and, when run without a TTY, re-execs itself via `konsole --hold -e bash …` so the output is visible and the window stays open. Verified: the fake-konsole relaunch wiring and a full build under a GUI-like minimal `PATH` (`env -i`, `PATH=/usr/bin:/bin`) compiled cleanly (exit 0). `Compile-Linux.sh` backed up as `Backups/Compile-Linux.sh.2026-11-07-193557.bak`.
28. **Konsole window auto-closes when the build finishes** — dropped `--hold` from the double-click relaunch (`konsole -e bash …`), so the window closes itself at the end instead of staying open. Console runs are unaffected. `Compile-Linux.sh` backed up as `Backups/Compile-Linux.sh.2026-11-07-193951.bak`.
29. **Dungeon Master's Guide fixes** — (a) `Worlds.GetDungeonListing(int)` cases 61/62 were `"Dungeon of the Lich King"`/`"Dungeon of the Mad Archmage"` (no leading "the") while `GetAreaEntrance` matches `"the Dungeon of …"`, so those two book rows resolved to location (0,0) and opened the map at the world origin — added the `the ` prefix in `World.cs`. (b) Added `Worlds.DungeonCount = 85` and switched the gump loop from the hardcoded `85` to it. (c) `OnResponse` now guards the map with `Sextants.HasSextant( from )` and messages the player instead of opening an empty `Sextants.MapGump` when they carry no sextant. Verified: every listing name now has a matching `GetAreaEntrance` zone; both edited files pass `mcs` syntax parsing. Backups: `Backups/World.cs.2026-11-07-222342.bak`, `Backups/DungeonMastersGuide.cs.2026-11-07-222342.bak`.
30. **Knives Chat3 "not functional" — chat button + load robustness** — the client chat button sends packet `0xB5` → core `ChatRequest` → `EventSink.InvokeChatRequest`, but Knives Chat3 had **no `EventSink.ChatRequest` subscriber** (`General.Initialize()` only wired Speech/Login/CharacterCreated), so the chat button did nothing (the in-game book even says "the CHAT button does not work… type `[c` instead"). Added the subscription + `OnChatRequest` → `General.List(m, 0)`. The command entries (`[c`/`[chat` via the Public channel, `[Va`/`[ViewAll`, etc.) were already wired and still work. Also hardened `Channel.Load()` to fall back to `PredefinedChannels()` on any load error, so a stale/corrupt `Channels.bin` can never leave chat with zero channels (which would also make `[c` unregistered). Verified all chat→core symbols exist (no compile issue) and both files pass `mcs` syntax parsing. Backups: `Backups/General.cs.2026-11-07-230824.bak`, `Backups/Channel.cs.2026-11-07-231000.bak`.
31. **Chat button matches `[c`** — the ChatRequest handler opened `General.List(m, 0)` = `ListPage.All` (a "View All" player list), which showed up as an empty Channels gump with no channels and no create option. Changed it to route through the Public channel exactly like `[c` (`Channel.GetByType(typeof(Public)).OnChat(m, "")`), so the button sets the current channel and opens the real channel chat view (falls back to `List(m,0)` only if Public is missing). `General.cs` backed up as `Backups/General.cs.2026-11-07-232515.bak`. Verified via `mcs` syntax parsing.
32. **Hair stylist "hire"/service gump shows no options** — the `CustomHairstylist` barber menu (`HairstylistBuyGump`, opened from the context option) filtered its rows by affordability, so with less than 50 gold it opened with zero buttons. Changed it to always list the applicable services (facial-hair entries still hidden for females) and size the background by the shown count; affordability is enforced on click via the existing "You cannot afford my services for that style." message. Verified via `mcs` syntax parsing. Backed up as `Backups/CustomHairstylist.cs.2026-11-07-235505.bak`.

---

## Current Behaviour

### Artefact Book (`ArtefactBook`)
- Item: `Tome of Wondrous Artifacts`, graphic `0x22C5`, hue `MementoCoin.MementoHue` (2141).
- `Charges` default `1`; set negative for unlimited.
- Catalogue = `ArtifactQuestList` entries filtered to `ArtefactLevel.StandardArtefact` (about 317 items), cached in `ArtefactBook.Entries`.
- Gump: 16 artefacts per page, two columns, no in-book title, prev/next arrows with a right-aligned "Page X of Y" next to the next arrow. Clicking an artefact opens a detail page with the icon on the left page and, on the right page, a "UNIQUE POWER" description (for artefacts with special abilities) plus the full stat sheet (`ItemProps.ItemProperties`); Claim (a checkmark-box button with its label right-aligned beside it) grants the item and consumes a charge, Back returns to the list.

### Memento Coin (`MementoCoin`)
- Stackable, hue `2141`, `DefaultWeight 0.02`.
- Displays as "Memento Coin" when the stack size is 1, and "Memento Coins" (with the amount) when 2 or more.

### Memento Collector (`MementoCollector`)
- Random male/female with matching `NameList` name; title `"the artefact collector"`. Name forced to the merchant blue (`Notoriety.GetHue(Notoriety.Innocent)`).
- Team `0` (the same neutral team town vendors use), re-asserted in `OnAfterSpawn`.
- Invulnerable / untargetable: derives from `BaseNPC` (always-invulnerable), so `Blessed` is kept; `Deserialize` re-asserts it. Cannot be targeted or harmed.
- Wanders (`CantWalk` not set).
- Wears dark-gray leather (`0x83B`): gorget, arms/sleeves, gloves, legs and chest; a random cloth hat (floppy, wide-brim, cap, skull cap, bandana, straw, tall straw, wizard, feathered, tricorne, pirate, hood, witch or bonnet), cloak and sash in hue `2141`; dark boots; a lit lantern (hue `2141`) in hand.
- On spawn each armour slot has a 50% chance to be replaced by a random standard artefact of the same `Layer` (at least one swap is guaranteed; gender-specific pieces are skipped). It also wields a random one-handed standard artefact weapon (`Layer.OneHanded`), which coexists with the lantern (`Layer.TwoHanded`).
- Idle emotes (yawn, sigh, cough, etc.) play when a player is within 12 tiles, on a 20–45s cooldown.
- Coin payouts by artefact tier: Legendary `15`, Standard `11`, Artifact `7`, Decorative `3`; non-artefacts are refused.
- Spawns: one collector per custom-merchant stall via `Info/Spawns/Merchant.map` (34 entries), loaded on `[buildworld` when `MySettings.S_CustomMerchant` is true.

### Talk / trade rules
- Dropping an artefact: deletes it and gives the tier payout in `MementoCoin`s.
- Dropping 75+ `MementoCoin`s: consumes 75 (refunds the remainder) and gives an `ArtefactBook`.

### Boat / Magic Carpet speed (`BaseBoat.ApplySpeedBoost`)
- Sailing vessels: driven only by `SkillName.Seafaring` (Magery has no effect) — 50/75/100/125 skill gives 1/2/3/4 boosts (25 ms per boost off the 250 ms fast interval).
- Magic carpets (`MagicCarpetA`–`MagicCarpetI`, detected by `BaseBoat.isCarpet`): driven only by `SkillName.Magery` (Seafaring has no effect); the same 50/75/100/125 tier table applies to the carpet owner's Magery.
- Overall maximum is `BaseBoat.MAX_SPEED_BOOSTS = 6` (also `Constants.BOAT_SPEED_MAX_LEVEL`), so the Avatar "Fast Seaman" upgrade and other sources can reach 6 on sailing vessels.
- Magic carpets are additionally clamped to `BaseBoat.MAX_CARPET_SPEED_BOOSTS = 4`, so a carpet can never exceed 4 bonus speed steps no matter what the Magery or Avatar values are.
- Slow movement and slow drift are unchanged (no skill boost applied).

### Compile shortcuts (`Compile-Linux.sh` / `Compile-Windows.bat`)
- Both live directly in the `World` folder, next to `Data`, `Info`, `Saves`, `Source`, etc.
- Base folder = the parent of `World` (the git repository root). Each shortcut runs `git pull` there first, then runs the compiler from `Source/Tools`.
  - Linux: `Compile-Linux.sh` → `bash Source/Tools/compile-world-linux.sh` (Mono `mcs`, output `World/WorldLinux.exe`). Run with `./Compile-Linux.sh`, `bash Compile-Linux.sh`, or double-click.
  - Windows: `Compile-Windows.bat` → `call Source\Tools\compile-world-win.bat` (.NET 4.0 `csc`, output `World\World.exe`). Double-click to run.
- Flow: `git pull` (base folder) → if it fails, abort with a message → otherwise compile from `Source/Tools` → report done.
- Both compilers use paths relative to `Source/Tools`, hence the shortcuts `cd`/`pushd` into that folder before invoking them.
- **Double-click on Linux:** Dolphin **executes** `Compile-Linux.sh` on double-click because `~/.config/kiorc` has `[Executable scripts] behaviourOnLaunch=execute`. That launch has no terminal, so the script re-opens itself in `konsole -e bash …` (build output visible; the window closes automatically when the build finishes) and sets up `PATH` (Homebrew/Linuxbrew) first. Console runs already have a TTY and skip the Konsole relaunch.
- The double-click behaviour is a per-user KIO setting (`~/.config/kiorc`, not part of the repo); set `behaviourOnLaunch=alwaysAsk` there to get the "execute or open?" prompt back instead.

### Dungeon Master's Guide (`DungeonMastersGuide`)
- Item graphic `0x3046`, name "Dungeon Masters Guide", weight 1.0; obtainable via `Loot.cs` and required for the `GygaxStatue` quest.
- Double-click opens a two-column gump listing all dungeons from `Worlds.GetDungeonListing` (row count = `Worlds.DungeonCount`, currently 85); each row shows world / dungeon name / sextant coordinate, plus a close button (button ID 0).
- Clicking a dungeon opens `Sextants.MapGump` at its entrance **only if the player carries a Sextant or MagicSextant**; otherwise the player is messaged "You need a sextant to locate this dungeon." (the raw `MapGump` renders empty without one).
- Fixed rows: "the Dungeon of the Lich King" and "the Dungeon of the Mad Archmage" now resolve to their real entrances instead of (0,0).

### Chat (Knives Chat3)
- Open with the client **chat button** (wired via `EventSink.ChatRequest` → routes through the Public channel exactly like `[c`) or the commands `[c` / `[chat` (Public channel), `[Va`/`[ViewAll`, `[Fri`/`[Friends`, `[Ma`/`[Mail`, `[pm`/`[msg`.
- Channels (`Public`, `Staff`, `Guild`, `Alliance`, `Faction`, `Irc`, `Multi`) load from `Saves/ChatBeta8/Channels.bin`; if that file is missing or fails to load, `PredefinedChannels()` is now a fallback so chat still works.
- `[c` dispatches to the Public channel's `OnChat` (empty text opens the list); chat gumps build via `GumpPlus` (deferred to a `Timer.DelayCall`).
- Known cosmetic leftover: `General.LoadHelpFile()` only clears the help table, so in-chat "Help" topics report "Help Error" (help contents aren't loaded from a file).

### Hair stylist / barber (`CustomHairstylist`)
- The context option (`BarberEntry`) opens `HairstylistBuyGump`, listing services: change hairstyle (500), facial hair (500), and several hair-color palettes (50–1000).
- The list now always shows the applicable services regardless of gold (facial-hair options are hidden for female characters). Choosing one charges the price from the backpack and opens the hairstyle/hue picker; if you can't afford it, the vendor says "You cannot afford my services for that style."


