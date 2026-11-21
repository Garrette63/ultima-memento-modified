# Memento Codebase Map

Generated 2026-11-21 from the `World/` tree. Covers **5298** `.cs` files in **555** source folders, plus every non-source folder.

## How to use this map

- Find a file: `grep -n "VendorValue.cs" Memento_Codebase_Map.md` or `grep -n "Items/Weapons/Axes" Memento_Codebase_Map.md`.
- Find a class: `grep -n "class BrownBear" ...` (entries show `Type : Base`).
- Browse by area: the folder index lists every folder with its direct file count and contained types.
- Entry format: `File.cs` — **PrimaryType : Base** — purpose (`(+N types)` = extra types in that file; brackets = attributes such as `Constructable`/`CorpseName`).
- Descriptions are extracted from XML summaries, `DefaultDescription`, command registrations, AI/name literals and class/base heuristics; generic classes report their public-member count.

## Repository at a glance

| Path | What it is |
|---|---|
| `World/Source/System/` | RunUO/Memento core: Item, Mobile, Map, network, persistence. Built into `WorldLinux.exe`. |
| `World/Source/Scripts/` | Shard script assembly: items, mobiles, engines, commands, skills (~5,172 files). Recompiled on startup. |
| `World/Source/Tools/` | `compile-world-linux.sh` (mcs); output `World/WorldLinux.exe`. |
| `World/Info/Scripts/` | Startup settings projects: `Settings.cs`, `Settings.override.cs`, `Merchant.cs`. |
| `World/Info/` | Spawn maps, articles, decoration data. |
| `World/Data/` | Runtime data: `Files/` (mul), `System/` (config/cliloc), `Spawns/`, `Decoration/`, `Logs/`, `RichVeins/`. |
| `World/Saves/` | Serialized accounts/items/mobiles/guilds/player data. Do not hand-edit. |
| `World/Documentation/` | Design/showcase docs; not loaded by the server. |
| `World/Backups/` | Timestamped `.bak` copies of edited files (git-ignored). |

## Cross-cutting systems

| Topic | Start here |
|---|---|
| Tooltip worth/tiers | `World/Source/Scripts/System/Misc/VendorValue.cs`, `World/Source/Scripts/System/Misc/ItemRank.cs` |
| Core->script hooks | `World/Source/System/Item.cs` (`InvokeItemTier`, `InvokeColoredName`, `InvokeNameHue`, `InvokeSuppressColorText3`, `InvokeLabelHue`) |
| Vendor pricing | `World/Source/Scripts/System/Misc/ItemSales.cs`, `World/Source/Scripts/Mobiles/Base/GenericSell.cs`, `World/Source/Scripts/Mobiles/Base/BaseVendor.cs` |
| Preferences/settings | `World/Source/Scripts/Mobiles/Base/PlayerPreferenceContext.cs`, `World/Source/Scripts/System/Help/HelpGump*.cs` |
| Relic loot/ID | `World/Source/Scripts/Items/Relics/*` (`DDRelic*`, `HighSeasRelic`, `NotIdentified`, `RelicFunctions`) |
| Loot generation | `World/Source/Scripts/Items/Containers/LootPack.cs`, `World/Source/Scripts/Mobiles/Base/BaseCreature.cs` |
| Commands | `World/Source/Scripts/System/Commands/`, `World/Source/Scripts/System/Commands/LootTierCommands.cs` |
| Random equipment names | `World/Source/Scripts/System/Misc/RandomThings.cs` (`MagicItemName`, `SpecialName`, `SkipMagicName`) |
| Spawns/regions | `World/Source/Scripts/System/Spawns/`, `World/Source/Scripts/System/Regions/`, `World/Info/Spawns/` |
| Persistence | `World/Source/System/Persistence/`, `World/Saves/` |

### Common tasks

| Want to... | Look at |
|---|---|
| Add/modify a weapon or armor | `Scripts/Items/Weapons/<family>`, `Scripts/Items/Armor/<family>`, base class in same folder |
| Add a creature | `Scripts/Mobiles/<type>/<family>`, `Mobiles/Base/BaseCreature.cs` |
| Add a command | `Scripts/System/Commands/`, follow `Initialize()` + `CommandSystem.Register` |
| Add a spell | `Scripts/Engines and Systems/Magic/<school>/Spells`, base `Spell` |
| Add quest content | `Scripts/Engines and Systems/Quests/<quest>` |
| Tune crafting/trades | `Scripts/Engines and Systems/Trades/`, `Scripts/Items/Trades/` |
| Tune loot/relics | `Scripts/Items/Containers/LootPack.cs`, `Scripts/Items/Relics/` |
| Tune prices/vendors | `Scripts/System/Misc/ItemSales.cs`, `Scripts/Mobiles/Base/{BaseVendor,GenericSell}.cs` |
| Change tooltips/prefs | `Scripts/System/Misc/{VendorValue,ItemRank}.cs`, `Scripts/System/Help/HelpGump*.cs`, `Mobiles/Base/PlayerPreferenceContext.cs` |
| Add a gump | `Scripts/System/Gumps/`, derive from `Gump` |

### Conventions

- File name = main class name. One feature per folder; base classes sit beside their concrete items.
- Items: `[Constructable]` ctor + `(Serial)` ctor + `Serialize`/`Deserialize`; mobiles: `[CorpseName]`, stats via `SetStr`/`SetSkill`.
- Commands: static `Initialize()` with `CommandSystem.Register`; gumps: `Gump` subclasses.
- Core never references script types directly: `World/Source/System/Item.cs` resolves `Server.ItemRank` / `Server.VendorValue` by reflection (`Invoke*` hooks). Keep hook signatures stable.
- Scripts compile at startup; core changes require rebuilding `World/WorldLinux.exe`.
- Record source changes in `Memento_System_Changes.md`; backups go to `World/Backups/<File>.<timestamp>.bak`.

### Maintenance

- This map is generated from source. Regenerate with `python3 World/Documentation/Generate-CodebaseMap.py`; hand-edits are overwritten.
- Keep the counts in the header and the folder index in sync when files move; `Memento_System_Changes.md` remains the authoritative change log.


## Folder index

One line per source folder (direct `.cs` counts). Curated purposes for major folders, type-derived otherwise.

- **Scripts** — 0 files — Shard script assembly; gameplay/content code compiled at startup.
- **Scripts/Engines and Systems** — 1 file — Gameplay engines and content systems (magic, quests, trades, champs, UI systems).
- **Scripts/Engines and Systems/Achievments** — 4 files — Achievement definitions, tracking, gumps and rewards.
- **Scripts/Engines and Systems/Achievments/AchieveTypes** — 8 files — Achieve types. Types: BaseAchievement, DiscoverLandAchievement, DiscoveryAchievement, ExplicitAchievement, ....
- **Scripts/Engines and Systems/Achievments/Gumps** — 2 files — Gumps. Types: AchievementGump, AchievementObtainedGump.
- **Scripts/Engines and Systems/Avatar** — 9 files — Avatar progression, coins, rewards, starter templates and safety-deposit items.
- **Scripts/Engines and Systems/Avatar/Core** — 3 files — Core. Types: CoinRewardCalculator, CoinRewardCalculatorLegacy, IAvatarOnlyItem.
- **Scripts/Engines and Systems/Avatar/Items** — 1 file — Items. Types: SafetyDepositBox.
- **Scripts/Engines and Systems/Avatar/Reward** — 12 files — Reward. Types: ActionReward, AvatarShopGump, RewardFactory, RewardFactory, ....
- **Scripts/Engines and Systems/Champs** — 10 files — Champion spawn altars, bosses, minions and rewards.
- **Scripts/Engines and Systems/Champs/Items** — 1 file — Items. Types: UnholyBone.
- **Scripts/Engines and Systems/Champs/Mobiles** — 2 files — Mobiles. Types: GreaterMongbat, SerpentineDragon.
- **Scripts/Engines and Systems/Champs/Mobiles/Bosses** — 8 files — Bosses. Types: Barracoon, BaseChampion, LordOaks, Mephitis, ....
- **Scripts/Engines and Systems/Jako Pets** — 0 files — Jako pet attribute system and pet items.
- **Scripts/Engines and Systems/Jako Pets/Attributes** — 10 files — Attributes. Types: JakoAttributes, JakoBaseAttribute, JakoHitPointsAttribute, JakoManaPointsAttribute, ....
- **Scripts/Engines and Systems/Magic** — 0 files — Spell schools, spellbooks, reagents, summons, spell mechanics.
- **Scripts/Engines and Systems/Magic/Bard** — 5 files — Magic/spell system: Bard. Types: BardSongProvider, SongBook, SongBookGump, CastSongSpells, ....
- **Scripts/Engines and Systems/Magic/Bard/Scrolls** — 16 files — Magic/spell system: Scrolls. Types: ArmysPaeonScroll, EnchantingEtudeScroll, EnergyCarolScroll, EnergyThrenodyScroll, ....
- **Scripts/Engines and Systems/Magic/Bard/Spells** — 16 files — Magic/spell system: Spells. Types: ArmysPaeonSong, EnchantingEtudeSong, EnergyCarolSong, EnergyThrenodySong, ....
- **Scripts/Engines and Systems/Magic/Base** — 16 files — Magic/spell system: Base. Types: CastSpellCommand, DisturbType, ForgetfulGem, Initializer, ....
- **Scripts/Engines and Systems/Magic/Bushido** — 9 files — Magic/spell system: Bushido. Types: BookOfBushido, Confidence, CounterAttack, Evasion, ....
- **Scripts/Engines and Systems/Magic/Death Knight** — 8 files — Magic/spell system: Death knight. Types: DeathKnightCommands, DeathKnightSpell, DeathKnightSpellbook, DeathKnightSpellbookGump, ....
- **Scripts/Engines and Systems/Magic/Death Knight/Spells** — 14 files — Magic/spell system: Spells. Types: BanishSpell, DemonicTouchSpell, DevilPactSpell, GrimReaperSpell, ....
- **Scripts/Engines and Systems/Magic/Druidism** — 4 files — Magic/spell system: Druidism. Types: BookDruidBrewing, DruidPouch, ShieldOfEarthPotion, HerbalistSpell.
- **Scripts/Engines and Systems/Magic/Druidism/Effects** — 16 files — Magic/spell system: Effects. Types: BlendWithForestSpell, DruidicRuneSpell, FireflySpell, GraspingRootsSpell, ....
- **Scripts/Engines and Systems/Magic/Elementalism** — 8 files — Magic/spell system: Elementalism. Types: ElementalCommands, ElementalEffect, ElementalExit, Elemental_Armor_Scroll, ....
- **Scripts/Engines and Systems/Magic/Elementalism/Mobiles** — 21 files — Magic/spell system: Mobiles. Types: ElementalCalledAir, ElementalCalledEarth, ElementalCalledFire, ElementalCalledWater, ....
- **Scripts/Engines and Systems/Magic/Elementalism/Sphere 1** — 4 files — Magic/spell system: Sphere 1. Types: Elemental_Armor_Spell, Elemental_Bolt_Spell, Elemental_Mend_Spell, Elemental_Sanctuary_Spell.
- **Scripts/Engines and Systems/Magic/Elementalism/Sphere 2** — 4 files — Magic/spell system: Sphere 2. Types: Elemental_Pain_Spell, Elemental_Protection_Spell, Elemental_Purge_Spell, Elemental_Steed_Spell.
- **Scripts/Engines and Systems/Magic/Elementalism/Sphere 3** — 4 files — Magic/spell system: Sphere 3. Types: Elemental_Call_Spell, Elemental_Force_Spell, Elemental_Wall_Spell, Elemental_Warp_Spell.
- **Scripts/Engines and Systems/Magic/Elementalism/Sphere 4** — 4 files — Magic/spell system: Sphere 4. Types: Elemental_Field_Spell, Elemental_Restoration_Spell, Elemental_Strike_Spell, Elemental_Void_Spell.
- **Scripts/Engines and Systems/Magic/Elementalism/Sphere 5** — 4 files — Magic/spell system: Sphere 5. Types: Elemental_Blast_Spell, Elemental_Echo_Spell, Elemental_Fiend_Spell, Elemental_Hold_Spell.
- **Scripts/Engines and Systems/Magic/Elementalism/Sphere 6** — 4 files — Magic/spell system: Sphere 6. Types: Elemental_Barrage_Spell, Elemental_Rune_Spell, Elemental_Storm_Spell, Elemental_Summon_Spell.
- **Scripts/Engines and Systems/Magic/Elementalism/Sphere 7** — 4 files — Magic/spell system: Sphere 7. Types: Elemental_Devastation_Spell, Elemental_Fall_Spell, Elemental_Gate_Spell, Elemental_Havoc_Spell.
- **Scripts/Engines and Systems/Magic/Elementalism/Sphere 8** — 4 files — Magic/spell system: Sphere 8. Types: Elemental_Apocalypse_Spell, Elemental_Lord_Spell, Elemental_Soul_Spell, Elemental_Spirit_Spell.
- **Scripts/Engines and Systems/Magic/Holy Man** — 8 files — Magic/spell system: Holy man. Types: HolyManCommands, HolyManSpell, HolyManSpellbook, HolyManSpellbookGump, ....
- **Scripts/Engines and Systems/Magic/Holy Man/Spells** — 14 files — Magic/spell system: Spells. Types: BanishEvilSpell, DampenSpiritSpell, EnchantSpell, HammerOfFaithSpell, ....
- **Scripts/Engines and Systems/Magic/Jedi** — 6 files — Magic/spell system: Jedi. Types: CastJediSpells, JediDatacron01, JediSpell, JediSpellbookGump, ....
- **Scripts/Engines and Systems/Magic/Jedi/Spells** — 10 files — Magic/spell system: Spells. Types: Celerity, Deflection, ForceGrip, MindsEye, ....
- **Scripts/Engines and Systems/Magic/Jester** — 5 files — Magic/spell system: Jester. Types: BagOfTricks, CastJesterSpells, JesterSpell, SummonedJoke, ....
- **Scripts/Engines and Systems/Magic/Jester/Spells** — 10 files — Magic/spell system: Spells. Types: CanOfSnakes, Clowns, FlowerPower, Hilarity, ....
- **Scripts/Engines and Systems/Magic/Knight** — 12 files — Magic/spell system: Knight. Types: BookOfChivalry, CleanseByFireSpell, CloseWoundsSpell, ConsecrateWeaponSpell, ....
- **Scripts/Engines and Systems/Magic/Magery** — 2 files — Magic/spell system: Magery. Types: MagerySpell, Spellbook.
- **Scripts/Engines and Systems/Magic/Magery/Scrolls** — 1 file — Magic/spell system: Scrolls. Types: SpellScroll.
- **Scripts/Engines and Systems/Magic/Magery/Scrolls/Eighth Circle** — 8 files — Magic/spell system: Eighth circle. Types: EarthquakeScroll, EnergyVortexScroll, ResurrectionScroll, SummonAirElementalScroll, ....
- **Scripts/Engines and Systems/Magic/Magery/Scrolls/Fifth Circle** — 8 files — Magic/spell system: Fifth circle. Types: BladeSpiritsScroll, DispelFieldScroll, IncognitoScroll, MagicReflectScroll, ....
- **Scripts/Engines and Systems/Magic/Magery/Scrolls/First Circle** — 8 files — Magic/spell system: First circle. Types: ClumsyScroll, CreateFoodScroll, FeeblemindScroll, HealScroll, ....
- **Scripts/Engines and Systems/Magic/Magery/Scrolls/Fourth Circle** — 8 files — Magic/spell system: Fourth circle. Types: ArchProtectionScroll, ArchCureScroll, CurseScroll, FireFieldScroll, ....
- **Scripts/Engines and Systems/Magic/Magery/Scrolls/Second Circle** — 8 files — Magic/spell system: Second circle. Types: AgilityScroll, CunningScroll, CureScroll, HarmScroll, ....
- **Scripts/Engines and Systems/Magic/Magery/Scrolls/Seventh Circle** — 8 files — Magic/spell system: Seventh circle. Types: ChainLightningScroll, EnergyFieldScroll, FlamestrikeScroll, GateTravelScroll, ....
- **Scripts/Engines and Systems/Magic/Magery/Scrolls/Sixth Circle** — 8 files — Magic/spell system: Sixth circle. Types: DispelScroll, EnergyBoltScroll, ExplosionScroll, InvisibilityScroll, ....
- **Scripts/Engines and Systems/Magic/Magery/Scrolls/Third Circle** — 8 files — Magic/spell system: Third circle. Types: BlessScroll, FireballScroll, MagicLockScroll, PoisonScroll, ....
- **Scripts/Engines and Systems/Magic/Magery/Spells** — 0 files — No direct scripts; groups subfolders.
- **Scripts/Engines and Systems/Magic/Magery/Spells/Magery 1st** — 8 files — Magic/spell system: Magery 1st. Types: ClumsySpell, CreateFoodSpell, FeeblemindSpell, HealSpell, ....
- **Scripts/Engines and Systems/Magic/Magery/Spells/Magery 2nd** — 8 files — Magic/spell system: Magery 2nd. Types: AgilitySpell, CunningSpell, CureSpell, HarmSpell, ....
- **Scripts/Engines and Systems/Magic/Magery/Spells/Magery 3rd** — 8 files — Magic/spell system: Magery 3rd. Types: BlessSpell, FireballSpell, MagicLockSpell, PoisonSpell, ....
- **Scripts/Engines and Systems/Magic/Magery/Spells/Magery 4th** — 8 files — Magic/spell system: Magery 4th. Types: ArchCureSpell, ArchProtectionSpell, CurseSpell, FireFieldSpell, ....
- **Scripts/Engines and Systems/Magic/Magery/Spells/Magery 5th** — 8 files — Magic/spell system: Magery 5th. Types: BladeSpiritsSpell, DispelFieldSpell, IncognitoSpell, MagicReflectSpell, ....
- **Scripts/Engines and Systems/Magic/Magery/Spells/Magery 6th** — 8 files — Magic/spell system: Magery 6th. Types: DispelSpell, EnergyBoltSpell, ExplosionSpell, InvisibilitySpell, ....
- **Scripts/Engines and Systems/Magic/Magery/Spells/Magery 7th** — 8 files — Magic/spell system: Magery 7th. Types: ChainLightningSpell, EnergyFieldSpell, FlameStrikeSpell, GateTravelSpell, ....
- **Scripts/Engines and Systems/Magic/Magery/Spells/Magery 8th** — 8 files — Magic/spell system: Magery 8th. Types: AirElementalSpell, EarthElementalSpell, EarthquakeSpell, EnergyVortexSpell, ....
- **Scripts/Engines and Systems/Magic/Misc** — 9 files — Magic/spell system: Misc. Types: AttackSpells, IdentifySpell, MagicRuneBag, MagicalSpell, ....
- **Scripts/Engines and Systems/Magic/Mystic** — 6 files — Magic/spell system: Mystic. Types: CastMonkSpells, MysticMonkRobe, MysticPack, MysticSpell, ....
- **Scripts/Engines and Systems/Magic/Mystic/Scrolls** — 10 files — Magic/spell system: Scrolls. Types: AstralProjectionScroll, AstralTravelScroll, CreateRobeScroll, GentleTouchScroll, ....
- **Scripts/Engines and Systems/Magic/Mystic/Spells** — 10 files — Magic/spell system: Spells. Types: AstralProjection, AstralTravel, CreateRobe, GentleTouch, ....
- **Scripts/Engines and Systems/Magic/Necromancy** — 20 files — Magic/spell system: Necromancy. Types: AnimateDeadSpell, BloodOathSpell, CorpseSkinSpell, CurseWeaponSpell, ....
- **Scripts/Engines and Systems/Magic/Necromancy/Scrolls** — 17 files — Magic/spell system: Scrolls. Types: AnimateDeadScroll, BloodOathScroll, CorpseSkinScroll, CurseWeaponScroll, ....
- **Scripts/Engines and Systems/Magic/Ninjitsu** — 11 files — Magic/spell system: Ninjitsu. Types: AnimalForm, Backstab, BookOfNinjitsu, DeathStrike, ....
- **Scripts/Engines and Systems/Magic/Research** — 6 files — Magic/spell system: Research. Types: AncientSpellbook, AncientSpellbookGump, ResearchBag, ResearchCommands, ....
- **Scripts/Engines and Systems/Magic/Research/Spells** — 0 files — No direct scripts; groups subfolders.
- **Scripts/Engines and Systems/Magic/Research/Spells/Conjuration** — 8 files — Magic/spell system: Conjuration. Types: ResearchAerialServant, ResearchClone, ResearchConjure, ResearchCreateGold, ....
- **Scripts/Engines and Systems/Magic/Research/Spells/Death** — 8 files — Magic/spell system: Death. Types: ResearchCreateGolem, ResearchDeathSpeak, ResearchGrantPeace, ResearchMaskofDeath, ....
- **Scripts/Engines and Systems/Magic/Research/Spells/Enchanting** — 8 files — Magic/spell system: Enchanting. Types: ResearchCauseFear, ResearchCharm, ResearchEnchant, ResearchMassMight, ....
- **Scripts/Engines and Systems/Magic/Research/Spells/Sorcery** — 8 files — Magic/spell system: Sorcery. Types: ResearchConflagration, ResearchCreateFire, ResearchEndureCold, ResearchEndureHeat, ....
- **Scripts/Engines and Systems/Magic/Research/Spells/Summoning** — 8 files — Magic/spell system: Summoning. Types: ResearchSummonAcidElemental, ResearchSummonBloodElemental, ResearchSummonElectricalElemental, ResearchSummonGemElemental, ....
- **Scripts/Engines and Systems/Magic/Research/Spells/Thaumaturgy** — 8 files — Magic/spell system: Thaumaturgy. Types: ResearchBanishDaemon, ResearchCallDestruction, ResearchConfusionBlast, ResearchDevastation, ....
- **Scripts/Engines and Systems/Magic/Research/Spells/Theurgy** — 8 files — Magic/spell system: Theurgy. Types: ResearchAirWalk, ResearchDivination, ResearchFadefromSight, ResearchHealingTouch, ....
- **Scripts/Engines and Systems/Magic/Research/Spells/Wizardry** — 8 files — Magic/spell system: Wizardry. Types: ResearchAvalanche, ResearchFrostField, ResearchFrostStrike, ResearchGasCloud, ....
- **Scripts/Engines and Systems/Magic/Shinobi** — 3 files — Magic/spell system: Shinobi. Types: CastShinobiSpells, ShinobiScroll, ShinobiSpell.
- **Scripts/Engines and Systems/Magic/Shinobi/Spells** — 8 files — Magic/spell system: Spells. Types: CheetahPaws, Deception, EagleEye, Espionage, ....
- **Scripts/Engines and Systems/Magic/Syth** — 6 files — Magic/spell system: Syth. Types: HellShard, CastSythSpells, SythDatacron01, SythSpell, ....
- **Scripts/Engines and Systems/Magic/Syth/Spells** — 10 files — Magic/spell system: Spells. Types: Absorption, CloneBody, DeathGrip, DrainLife, ....
- **Scripts/Engines and Systems/Magic/Witch** — 4 files — Magic/spell system: Witch. Types: BookWitchBrewing, UndeadSpell, BloodPactScroll, WitchPouch.
- **Scripts/Engines and Systems/Magic/Witch/Effects** — 16 files — Magic/spell system: Effects. Types: UnBloodlessSpecPactSpell, UndeadCurePoisonSpell, GhostPhaseSpell, GhostlyImagesSpell, ....
- **Scripts/Engines and Systems/Messaging** — 3 files — Messaging/social system.
- **Scripts/Engines and Systems/MobileEnhancement** — 2 files — Mobile enhancement tweaks and helpers.
- **Scripts/Engines and Systems/MobileEnhancement/Abstractions** — 6 files — Abstractions. Types: EnhancementDecorator, EnhancementRecipientDecorator, EnhancementSourceBase, IEnhancement, ....
- **Scripts/Engines and Systems/MobileEnhancement/Enhancements** — 1 file — Enhancements. Types: OwnerDependentEnhancement.
- **Scripts/Engines and Systems/MobileEnhancement/Recipients** — 2 files — Recipients. Types: RangeDependentRecipient, TimeDependentRecipient.
- **Scripts/Engines and Systems/ModernSkill** — 2 files — Modern skill UI/logic.
- **Scripts/Engines and Systems/PuzzleChest** — 5 files — Puzzle chest mini-system.
- **Scripts/Engines and Systems/Quests** — 12 files — Quest framework and content (standard, fishing, assassin, museum, runes).
- **Scripts/Engines and Systems/Quests/Assassin** — 2 files — Quest content: Assassin. Types: BoxOfAtonement, AssassinFunctions.
- **Scripts/Engines and Systems/Quests/Bards Tale** — 1 file — Quest content: Bards tale. Types: MangarRobe.
- **Scripts/Engines and Systems/Quests/Codex** — 5 files — Quest content: Codex. Types: ApproachVoid, CodexWisdom, CubeOnCorpse, DoorCodex, ....
- **Scripts/Engines and Systems/Quests/Core** — 10 files — Quest content: Core. Types: IDeserializable, IQuestGiver, MLQuest, MLQuestContext, ....
- **Scripts/Engines and Systems/Quests/Core/Definitions** — 5 files — Quest content: Definitions. Types: BlacksmithRecipeQuest, BlacksmithBasicsQuest, ArtifactRumor, StealingThePastQuest, ....
- **Scripts/Engines and Systems/Quests/Core/Gumps** — 9 files — Quest content: Gumps. Types: BaseQuestGump, InfoNPCGump, QuestCancelConfirmGump, QuestConversationGump, ....
- **Scripts/Engines and Systems/Quests/Core/Objectives** — 7 files — Quest content: Objectives. Types: BaseObjective, CollectObjective, CraftObjective, DeliverObjective, ....
- **Scripts/Engines and Systems/Quests/Core/Rewards** — 5 files — Quest content: Rewards. Types: BaseReward, ConstructibleItemReward, DummyReward, FameReward, ....
- **Scripts/Engines and Systems/Quests/Core/Utilities** — 1 file — Quest content: Utilities. Types: DeliveryQuestUtilities.
- **Scripts/Engines and Systems/Quests/Epic** — 2 files — Quest content: Epic. Types: CourierMail, EpicGump.
- **Scripts/Engines and Systems/Quests/Fishing** — 2 files — Quest content: Fishing. Types: FishingQuestBoard, FishingQuestFunctions.
- **Scripts/Engines and Systems/Quests/Frankenstein** — 14 files — Quest content: Frankenstein. Types: EmbalmingFluid, FrankenArmLeft, FrankenArmRight, FrankenBrain, ....
- **Scripts/Engines and Systems/Quests/Golems** — 5 files — Quest content: Golems. Types: BottleOil, GolemFighter, GolemManual, GolemPorter, ....
- **Scripts/Engines and Systems/Quests/Hoard** — 2 files — Quest content: Hoard. Types: HoardPiles, HoardTile.
- **Scripts/Engines and Systems/Quests/Jester** — 1 file — Quest content: Jester. Types: JokeBook.
- **Scripts/Engines and Systems/Quests/Magic Pools** — 1 file — Quest content: Magic pools. Types: MagicPool.
- **Scripts/Engines and Systems/Quests/Major** — 2 files — Quest content: Major. Types: QuestTake, QuestTome.
- **Scripts/Engines and Systems/Quests/Museum** — 3 files — Quest content: Museum. Types: Museums, MuseumBook, MuseumBookGump.
- **Scripts/Engines and Systems/Quests/Pagan** — 7 files — Quest content: Pagan. Types: ApproachObsidian, ObeliskOnCorpse, ObeliskTip, ObsidianGate, ....
- **Scripts/Engines and Systems/Quests/Prisoners** — 1 file — Quest content: Prisoners. Types: Prisoner.
- **Scripts/Engines and Systems/Quests/Robots** — 11 files — Quest content: Robots. Types: Robot, RobotBatteries, RobotBolt, RobotCircuitBoard, ....
- **Scripts/Engines and Systems/Quests/Runes** — 3 files — Quest content: Runes. Types: RuneBox, RunesBase, RunesBaseEmpty.
- **Scripts/Engines and Systems/Quests/Search** — 1 file — Quest content: Search. Types: SearchBase.
- **Scripts/Engines and Systems/Quests/Serpents** — 7 files — Quest content: Serpents. Types: BaneBase, BaneBaseEmpty, BlackrockSerpentOrder, LanternOfDiscipline, ....
- **Scripts/Engines and Systems/Quests/Shadowlords** — 10 files — Quest content: Shadowlords. Types: BalinorTeleporter, BellOfCourage, BookOfTruth, CandleOfLove, ....
- **Scripts/Engines and Systems/Quests/Standard** — 2 files — Quest content: Standard. Types: StandardQuestBoard, StandardQuestFunctions.
- **Scripts/Engines and Systems/Quests/Summon** — 5 files — Quest content: Summon. Types: SummonQuests, SummonChest, SummonItems, SummonPrison, ....
- **Scripts/Engines and Systems/Quests/Thief** — 9 files — Quest content: Thief. Types: Coffer, HayCrate, HollowStump, StealBag, ....
- **Scripts/Engines and Systems/Quests/Underworld** — 3 files — Quest content: Underworld. Types: RuneStoneGate, SkullOfBaronAlmric, UnderworldTeleporter.
- **Scripts/Engines and Systems/Spell Bars** — 4 files — Spell-bar UI and commands.
- **Scripts/Engines and Systems/Spell Bars/Core** — 12 files — Core. Types: ISpellBarConfiguration, ISpellBarDescriptor, ISpellSchool, SpellBarDefinition, ....
- **Scripts/Engines and Systems/Spell Bars/Schools** — 9 files — Schools. Types: AncientSpellSchool, BardSpellSchool, DeathKnightSpellSchool, ElementalSpellSchool, ....
- **Scripts/Engines and Systems/Temptations** — 5 files — Temptation/choice event system.
- **Scripts/Engines and Systems/Trades** — 0 files — Crafting/trade engines: bulk orders, harvesting, shoppes, gardening, tinkering.
- **Scripts/Engines and Systems/Trades/Apiculture** — 6 files — Trade/craft content: Apiculture. Types: apiBeeHive, apiBeeHiveMainGump, apiBeeHiveHelpGump, Beeswax, ....
- **Scripts/Engines and Systems/Trades/Apiculture/Craft** — 7 files — Trade/craft content: Craft. Types: CandleReligious, ColorCandleLong, ColorCandleShort, JarsOfWaxMetal, ....
- **Scripts/Engines and Systems/Trades/Apiculture/Items** — 5 files — Trade/craft content: Items. Types: HiveTool, apiLargeWaxPot, RawBeeswax, Slumgum, ....
- **Scripts/Engines and Systems/Trades/Bulk Orders** — 0 files — No direct scripts; groups subfolders.
- **Scripts/Engines and Systems/Trades/Bulk Orders/Rewards** — 3 files — Trade/craft content: Rewards. Types: ColoredAnvil, LeatherGlovesOfMining, PowderOfTemperament.
- **Scripts/Engines and Systems/Trades/BulkCraft** — 4 files — Trade/craft content: Bulk craft. Types: BulkCraft, BulkCraftContext, BulkCraftGump, BulkCraftTimer.
- **Scripts/Engines and Systems/Trades/Core** — 25 files — Trade/craft content: Core. Types: BreakDown, CraftAmountPrompt, CraftContext, CraftGroup, ....
- **Scripts/Engines and Systems/Trades/Crafting** — 22 files — Trade/craft content: Crafting. Types: DefAlchemy, DefApothecary, DefBlacksmithy, DefBonecrafting, ....
- **Scripts/Engines and Systems/Trades/Gardening** — 13 files — Trade/craft content: Gardening. Types: EmptyTheBowlGump, MainPlantGump, PlantBowl, PlantHue, ....
- **Scripts/Engines and Systems/Trades/Gardening/MiscItems** — 4 files — Trade/craft content: Misc items. Types: FertileDirt, GreenThorns, OrangePetals, RedLeaves.
- **Scripts/Engines and Systems/Trades/Gardening/Network** — 1 file — Trade/craft content: Network. Types: DisplayHelpTopic.
- **Scripts/Engines and Systems/Trades/Global Shoppe** — 6 files — Trade/craft content: Global shoppe. Types: RewardType, ShoppeCommands, ShoppeConstants, ShoppeEngine, ....
- **Scripts/Engines and Systems/Trades/Global Shoppe/Abstractions** — 6 files — Trade/craft content: Abstractions. Types: ICustomerShoppe, IExceptionalItem, IGemTypeItem, IOrderContext, ....
- **Scripts/Engines and Systems/Trades/Global Shoppe/Calculators** — 13 files — Trade/craft content: Calculators. Types: AlchemyRewardCalculator, BaseCraftRewardCalculator, BaseRewardCalculator, BlacksmithRewardCalculator, ....
- **Scripts/Engines and Systems/Trades/Global Shoppe/Core** — 5 files — Trade/craft content: Core. Types: BasicCustomerOrderShoppe, CustomerOrderShoppe, CustomerShoppe, OrderFactory, ....
- **Scripts/Engines and Systems/Trades/Global Shoppe/Diagnostics** — 2 files — Trade/craft content: Diagnostics. Types: IDiagnosticOrderShoppe, ShoppeCraftSystem.
- **Scripts/Engines and Systems/Trades/Global Shoppe/Gumps** — 5 files — Trade/craft content: Gumps. Types: ExplainShoppeGump, OrderGump, RewardSelectionGump, ShoppeGump, ....
- **Scripts/Engines and Systems/Trades/Global Shoppe/Models** — 6 files — Trade/craft content: Models. Types: CustomerContext, EquipmentOrderContext, OrderContext, PlayerContext, ....
- **Scripts/Engines and Systems/Trades/Global Shoppe/Rewards** — 6 files — Trade/craft content: Rewards. Types: AdvancedSkinningKnife, AncientCraftingGloves, ArboristTool, DyeTubTempBase, ....
- **Scripts/Engines and Systems/Trades/Global Shoppe/Shoppes** — 11 files — Trade/craft content: Shoppes. Types: AlchemistShoppe, BakerShoppe, BlacksmithShoppe, BowyerShoppe, ....
- **Scripts/Engines and Systems/Trades/Guild** — 8 files — Trade/craft content: Guild. Types: AttributeHandler, EnhancementGump, GuildCraftingProcess, GuildCarpentry, ....
- **Scripts/Engines and Systems/Trades/Harvest** — 15 files — Trade/craft content: Harvest. Types: BonusHarvestResource, Fishing, GraveRobbing, HarvestArrow, ....
- **Scripts/Engines and Systems/Trades/Harvest/RichLumberjacking** — 2 files — Trade/craft content: Rich lumberjacking. Types: RichLumberjacking, RichLumberjackingSparkle.
- **Scripts/Engines and Systems/Trades/Harvest/RichLumberjacking/Volatile** — 2 files — Trade/craft content: Volatile. Types: VolatileHarvestBank, VolatileHarvestDefinition.
- **Scripts/Engines and Systems/Trades/Harvest/RichVeins** — 7 files — Trade/craft content: Rich veins. Types: LootCommands, RichVeinCommands, RichVeinConfig, RichVeinEngine, ....
- **Scripts/Engines and Systems/Trades/Harvest/RichVeins/Items** — 3 files — Trade/craft content: Items. Types: RichVeinMineable, RichVeinSpawner, RichVeinSpawnerPersistent.
- **Scripts/Engines and Systems/Trades/Shoppes** — 2 files — Trade/craft content: Shoppes. Types: DungeoneerCrate, MerchantCrate.
- **Scripts/Engines and Systems/Trades/Stone** — 6 files — Trade/craft content: Stone. Types: StatueGateGuardian, BaseStatue, BaseStatueDeed, StoneChairs, ....
- **Scripts/Engines and Systems/Trades/Taxidermy** — 6 files — Trade/craft content: Taxidermy. Types: Skulls, MountedTrophyHead, MountingBase, StuffedBear, ....
- **Scripts/Items** — 0 files — Item definitions: equipment, consumables, containers, relics, housing, games, tech.
- **Scripts/Items/Abstractions** — 2 files — Items: Abstractions. Types: ICommodity, IHarvestTool.
- **Scripts/Items/Armor** — 4 files — Armor and shield item classes.
- **Scripts/Items/Armor/Bone** — 12 files — Armor/shield items: Bone. Types: BoneArms, BoneChest, BoneGloves, BoneHelm, ....
- **Scripts/Items/Armor/Chain** — 5 files — Armor/shield items: Chain. Types: ChainChest, ChainCoif, ChainHatsuburi, ChainLegs, ....
- **Scripts/Items/Armor/Glasses** — 13 files — Armor/shield items: Glasses. Types: AnthropomorphistGlasses, ArtsGlasses, ElvenGlasses, FoldedSteelGlasses, ....
- **Scripts/Items/Armor/Helmets** — 6 files — Armor/shield items: Helmets. Types: Bascinet, CloseHelm, DreadHelm, Helmet, ....
- **Scripts/Items/Armor/Leather** — 30 files — Armor/shield items: Leather. Types: FemaleLeafChest, FemaleLeatherChest, HideChest, HikingBoots, ....
- **Scripts/Items/Armor/Plate** — 20 files — Armor/shield items: Plate. Types: DecorativePlateKabuto, FemalePlateChest, HeavyPlateJingasa, LightPlateJingasa, ....
- **Scripts/Items/Armor/Ranger** — 5 files — Armor/shield items: Ranger. Types: RangerArms, RangerChest, RangerGloves, RangerGorget, ....
- **Scripts/Items/Armor/Ring** — 5 files — Armor/shield items: Ring. Types: RingmailArms, RingmailChest, RingmailGloves, RingmailLegs, ....
- **Scripts/Items/Armor/Royal** — 7 files — Armor/shield items: Royal. Types: RoyalArms, RoyalBoots, RoyalChest, RoyalGloves, ....
- **Scripts/Items/Armor/Scaled** — 25 files — Armor/shield items: Scaled. Types: DragonArms, DragonChest, DragonGloves, DragonHelm, ....
- **Scripts/Items/Armor/Shields** — 19 files — Armor/shield items: Shields. Types: BaseShield, BronzeShield, Buckler, ChampionShield, ....
- **Scripts/Items/Armor/Studded** — 15 files — Armor/shield items: Studded. Types: BearCap, FemaleStuddedChest, StuddedArms, StuddedBustierArms, ....
- **Scripts/Items/Boats** — 29 files — Items: Boats. Types: BaseBoat, BaseBoatDeed, BaseDockedBoat, BoatBuild, ....
- **Scripts/Items/Books** — 13 files — Items: Books. Types: ArtefactBook, BardsTaleNote, BaseBook, BeginnerBook, ....
- **Scripts/Items/Books/BulletinBoards** — 10 files — Items: Bulletin boards. Types: AdminBoard, AssassinNote, BoardGump, GuardNote, ....
- **Scripts/Items/Books/PowerScrolls** — 2 files — Items: Power scrolls. Types: PowerScroll, PowerGump.
- **Scripts/Items/Books/Trades** — 7 files — Items: Trades. Types: LearnGraniteBook, LearnLeatherBook, LearnMetalBook, LearnMiscBook, ....
- **Scripts/Items/Clothing** — 14 files — Clothing, hats, shoes and wearable deco.
- **Scripts/Items/Clothing/Suits** — 5 files — Clothing items: Suits. Types: BaseSuit, DeathShroud, DupreSuit, LordBlackthorneSuit, ....
- **Scripts/Items/Containers** — 44 files — Containers, loot packs, treasure/paragon/sunken chests, corpses.
- **Scripts/Items/Deeds** — 7 files — Items: Deeds. Types: BarkeepContract, ClothingBlessDeed, CommodityDeed, DragonBardingDeed, ....
- **Scripts/Items/Explorers** — 8 files — Items: Explorers. Types: Bedroll, BedrolledOut, CamperTentEffect, Campfire, ....
- **Scripts/Items/Food** — 13 files — Items: Food. Types: Wasabi, BakedBread, BeverageType, BloodyDrink, ....
- **Scripts/Items/Food/Farming** — 13 files — Items: Farming. Types: FarmableCabbage, FarmableCarrot, FarmableCorn, FarmableCotton, ....
- **Scripts/Items/Games** — 15 files — Items: Games. Types: Backgammon, BaseBoard, BasePiece, CEOBlackJack, ....
- **Scripts/Items/Games/DandD** — 9 files — Items: Dand d. Types: Dice10, Dice12, Dice20, Dice4, ....
- **Scripts/Items/Games/LiarsDice** — 8 files — Items: Liars dice. Types: CallBluffGump, MobileDiceStstatus, DiceCommand, ExitDiceGump, ....
- **Scripts/Items/Games/Mahjong** — 11 files — Items: Mahjong. Types: MahjongDealerIndicator, MahjongDices, MahjongPieceDirection, MahjongGame, ....
- **Scripts/Items/Gems** — 12 files — Items: Gems. Types: Amber, Amethyst, Citrine, Crystals, ....
- **Scripts/Items/Houses** — 25 files — Housing, addons, decoration, remodeling and house tools.
- **Scripts/Items/Houses/Carpets** — 9 files — Items: Carpets. Types: MagicCarpetA, MagicCarpetB, MagicCarpetC, MagicCarpetD, ....
- **Scripts/Items/Houses/Construction** — 2 files — Items: Construction. Types: Ankhs, MerchantWagonSouth.
- **Scripts/Items/Houses/Construction/Addons** — 94 files — Items: Addons. Types: AbbatoirAddon, AddonComponent, AddonContainerComponent, AlchemistTableEastAddon, ....
- **Scripts/Items/Houses/Construction/Chairs** — 4 files — Items: Chairs. Types: WoodenBench, FancyWoodenChairCushion, Stool, Throne.
- **Scripts/Items/Houses/Construction/Decorative** — 4 files — Items: Decorative. Types: DecorativeShield1, DecorativeBowWest, LargePainting, Tapestry1N.
- **Scripts/Items/Houses/Construction/Floors** — 1 file — Items: Floors. Types: BaseFloor.
- **Scripts/Items/Houses/Construction/Lights** — 35 files — Items: Lights. Types: BaseEquipableLight, BaseLight, Brazier, BrazierTall, ....
- **Scripts/Items/Houses/Construction/Misc** — 9 files — Items: Misc. Types: BarrelLid, Easle, MeltedWax, TallMusicStand, ....
- **Scripts/Items/Houses/Construction/Ruined** — 1 file — Items: Ruined. Types: RuinedFallenChairA.
- **Scripts/Items/Houses/Construction/Signs** — 3 files — Items: Signs. Types: BaseSign, LocalizedSign, Sign.
- **Scripts/Items/Houses/Construction/Tables** — 2 files — Items: Tables. Types: ElegantLowTable, WritingTable.
- **Scripts/Items/Houses/Construction/Treasure Piles** — 8 files — Items: Treasure piles. Types: TreasurePile01Addon, TreasurePile02Addon, TreasurePile03Addon, TreasurePile04Addon, ....
- **Scripts/Items/Houses/Construction/Trees** — 4 files — Items: Trees. Types: DarkBrownTreeAddon, GreyTreeAddon, LightBrownTreeAddon, PearTreeAddon.
- **Scripts/Items/Houses/Construction/Walls** — 6 files — Items: Walls. Types: BaseWall, DarkWoodWall, ThickGrayStoneWall, ThinBrickWall, ....
- **Scripts/Items/Houses/Construction/Wells** — 6 files — Items: Wells. Types: BlackWellAddon, BrownWellAddon, MarbleWellAddon, RedWellAddon, ....
- **Scripts/Items/Houses/Decorations** — 7 files — Items: Decorations. Types: DecoStatueDeed, EvilItems, GwennoGraveAddon, house_sign_sign_post_a, ....
- **Scripts/Items/Houses/Decorations/Artifacts** — 3 files — Items: Artifacts. Types: BaseDecorationArtifact, BackpackArtifact, StealableArtifactsSpawner.
- **Scripts/Items/Houses/Decorations/Artifacts/Decorative** — 14 files — Items: Decorative. Types: ArtifactLargeVase, ArtifactVase, DemonSkull, DirtPatch, ....
- **Scripts/Items/Houses/Doors** — 25 files — Items: Doors. Types: ArgentrockTeleporter, BaseDoor, BasementDoor, BasementDoorway, ....
- **Scripts/Items/Houses/Monopoly** — 2 files — Items: Monopoly. Types: Monopoly, RUOVersion.
- **Scripts/Items/Houses/Monopoly/Gumps** — 0 files — No direct scripts; groups subfolders.
- **Scripts/Items/Houses/Monopoly/Gumps/Error Reporting** — 3 files — Items: Error reporting. Types: Errors, ErrorsGump, ErrorsNotifyGump.
- **Scripts/Items/Houses/Monopoly/Gumps/Gumps Plus Light** — 6 files — Items: Gumps plus light. Types: BackgroundPlus, ButtonPlus, GumpPlusLight, HTML, ....
- **Scripts/Items/Houses/Monopoly/Gumps/TownHouse Gumps** — 5 files — Items: Town house gumps. Types: ContractConfirmGump, ContractSetupGump, TownHouseConfirmGump, TownHouseSetupGump, ....
- **Scripts/Items/Houses/Monopoly/Items** — 6 files — Items: Items. Types: RentalContract, RentalContractCopy, RentalLicense, SignHammer, ....
- **Scripts/Items/Houses/Monopoly/Misc** — 4 files — Items: Misc. Types: CommandInfo, DecoreItemInfo, General, GumpResponse.
- **Scripts/Items/Houses/Remodeling** — 22 files — Items: Remodeling. Types: StairRefundEntry, LawnGate, LawnGump, LawnItem, ....
- **Scripts/Items/Instruments** — 12 files — Items: Instruments. Types: BambooFlute, BaseInstrument, Drums, Fiddle, ....
- **Scripts/Items/Magical** — 12 files — Magical items: artifacts, quivers, wands, gift/quest rewards.
- **Scripts/Items/Magical/Artifacts** — 1 file — Artifact item definitions grouped by slot/set (armor, clothing, jewelry, weapons, books, quivers).
- **Scripts/Items/Magical/Artifacts/Armor** — 42 files — Artifact items: Armor. Types: Artifact_AbysmalGloves, Artifact_AegisOfGrace, Artifact_AngelicEmbrace, Artifact_ArmsOfToxicity, ....
- **Scripts/Items/Magical/Artifacts/Armor/Aegis** — 6 files — Artifact items: Aegis. Types: Artifact_ArmsOfAegis, Artifact_GlovesOfAegis, Artifact_GorgetOfAegis, Artifact_HelmOfAegis, ....
- **Scripts/Items/Magical/Artifacts/Armor/Arcane** — 6 files — Artifact items: Arcane. Types: Artifact_ArcaneArms, Artifact_ArcaneCap, Artifact_ArcaneGloves, Artifact_ArcaneGorget, ....
- **Scripts/Items/Magical/Artifacts/Armor/Bane** — 3 files — Artifact items: Bane. Types: Artifact_CoifOfBane, Artifact_LeggingsOfBane, Artifact_TunicOfBane.
- **Scripts/Items/Magical/Artifacts/Armor/Divine** — 6 files — Artifact items: Divine. Types: Artifact_DivineArms, Artifact_DivineCountenance, Artifact_DivineGloves, Artifact_DivineGorget, ....
- **Scripts/Items/Magical/Artifacts/Armor/FallenKing** — 6 files — Artifact items: Fallen king. Types: Artifact_ArmsOfTheFallenKing, Artifact_CapOfTheFallenKing, Artifact_GlovesOfTheFallenKing, Artifact_LegsOfTheFallenKing, ....
- **Scripts/Items/Magical/Artifacts/Armor/Fire** — 3 files — Artifact items: Fire. Types: Artifact_CoifOfFire, Artifact_LeggingsOfFire, Artifact_TunicOfFire.
- **Scripts/Items/Magical/Artifacts/Armor/Fortune** — 6 files — Artifact items: Fortune. Types: Artifact_ArmorOfFortune, Artifact_ArmsOfFortune, Artifact_CapOfFortune, Artifact_GlovesOfFortune, ....
- **Scripts/Items/Magical/Artifacts/Armor/Harrower** — 5 files — Artifact items: Harrower. Types: Artifact_ArmsOfTheHarrower, Artifact_GlovesOfTheHarrower, Artifact_LegsOfTheHarrower, Artifact_OrnateCrownOfTheHarrower, ....
- **Scripts/Items/Magical/Artifacts/Armor/HolyKnight** — 6 files — Artifact items: Holy knight. Types: Artifact_HolyKnightsArmPlates, Artifact_HolyKnightsBreastplate, Artifact_HolyKnightsGloves, Artifact_HolyKnightsGorget, ....
- **Scripts/Items/Magical/Artifacts/Armor/Hunters** — 5 files — Artifact items: Hunters. Types: Artifact_HuntersArms, Artifact_HuntersGloves, Artifact_HuntersGorget, Artifact_HuntersLeggings, ....
- **Scripts/Items/Magical/Artifacts/Armor/Inquisitor** — 6 files — Artifact items: Inquisitor. Types: Artifact_InquisitorsArms, Artifact_InquisitorsGorget, Artifact_InquisitorsHelm, Artifact_InquisitorsLeggings, ....
- **Scripts/Items/Magical/Artifacts/Armor/Insight** — 6 files — Artifact items: Insight. Types: Artifact_ArmorOfInsight, Artifact_ArmsOfInsight, Artifact_GlovesOfInsight, Artifact_GorgetOfInsight, ....
- **Scripts/Items/Magical/Artifacts/Armor/Jackals** — 6 files — Artifact items: Jackals. Types: Artifact_JackalsArms, Artifact_JackalsCollar, Artifact_JackalsGloves, Artifact_JackalsHelm, ....
- **Scripts/Items/Magical/Artifacts/Armor/Midnight** — 5 files — Artifact items: Midnight. Types: Artifact_MidnightBracers, Artifact_MidnightGloves, Artifact_MidnightHelm, Artifact_MidnightLegs, ....
- **Scripts/Items/Magical/Artifacts/Armor/Nobility** — 4 files — Artifact items: Nobility. Types: Artifact_ArmorOfNobility, Artifact_ArmsOfNobility, Artifact_GauntletsOfNobility, Artifact_LegsOfNobility.
- **Scripts/Items/Magical/Artifacts/Armor/ShadowDancer** — 6 files — Artifact items: Shadow dancer. Types: Artifact_ShadowDancerArms, Artifact_ShadowDancerCap, Artifact_ShadowDancerGloves, Artifact_ShadowDancerGorget, ....
- **Scripts/Items/Magical/Artifacts/Armor/Totem** — 6 files — Artifact items: Totem. Types: Artifact_SpiritOfTheTotem, Artifact_TotemArms, Artifact_TotemGloves, Artifact_TotemGorget, ....
- **Scripts/Items/Magical/Artifacts/Books** — 6 files — Artifact items: Books. Types: Artifact_BookOfKnowledge, Artifact_HydrosLexicon, Artifact_LithosTome, Artifact_OssianGrimoire, ....
- **Scripts/Items/Magical/Artifacts/Clothing** — 0 files — No direct scripts; groups subfolders.
- **Scripts/Items/Magical/Artifacts/Clothing/Back** — 4 files — Artifact items: Back. Types: Artifact_EmbroideredOakLeafCloak, Artifact_FurCapeOfTheSorceress, Artifact_GrayMouserCloak, Artifact_PolarBearCape.
- **Scripts/Items/Magical/Artifacts/Clothing/Chest** — 2 files — Artifact items: Chest. Types: Artifact_JinBaoriOfGoodFortune, Artifact_LieutenantOfTheBritannianRoyalGuard.
- **Scripts/Items/Magical/Artifacts/Clothing/Foot** — 9 files — Artifact items: Foot. Types: Artifact_BootsofHermes, Artifact_BootsofHydros, Artifact_BootsofLithos, Artifact_BootsofPyros, ....
- **Scripts/Items/Magical/Artifacts/Clothing/Head** — 16 files — Artifact items: Head. Types: Artifact_BurglarsBandana, Artifact_CaptainJohnsHat, Artifact_CrownOfTalKeesh, Artifact_DreadPirateHat, ....
- **Scripts/Items/Magical/Artifacts/Clothing/Outer** — 18 files — Artifact items: Outer. Types: Artifact_ANecromancerShroud, Artifact_AcidProofRobe, Artifact_ArcanicRobe, Artifact_BeggarsRobe, ....
- **Scripts/Items/Magical/Artifacts/Clothing/Waist** — 4 files — Artifact items: Waist. Types: Artifact_BeltofHercules, Artifact_ConansLoinCloth, Artifact_CrimsonCincture, Artifact_GeishasObi.
- **Scripts/Items/Magical/Artifacts/Instruments** — 3 files — Artifact items: Instruments. Types: Artifact_GwennosHarp, Artifact_HornOfKingTriton, Artifact_IolosLute.
- **Scripts/Items/Magical/Artifacts/Jewelry** — 22 files — Artifact items: Jewelry. Types: Artifact_AlchemistsBauble, Artifact_BraceletOfHealth, Artifact_BraceletOfTheElements, Artifact_BraceletOfTheVile, ....
- **Scripts/Items/Magical/Artifacts/Minor** — 12 files — Artifact items: Minor. Types: AdmiralsHeartyRum, CandelabraOfSouls, ColoringBook, EverlastingBottle, ....
- **Scripts/Items/Magical/Artifacts/Offhands** — 8 files — Artifact items: Offhands. Types: Artifact_ArcticBeacon, Artifact_AuraOfShadows, Artifact_CandleCold, Artifact_EternalFlame, ....
- **Scripts/Items/Magical/Artifacts/Quivers** — 8 files — Artifact items: Quivers. Types: Artifact_Pestilence, Artifact_QuiverOfBlight, Artifact_QuiverOfElements, Artifact_QuiverOfFire, ....
- **Scripts/Items/Magical/Artifacts/Shields** — 6 files — Artifact items: Shields. Types: Artifact_AchillesShield, Artifact_Aegis, Artifact_ArcaneShield, Artifact_DupresShield, ....
- **Scripts/Items/Magical/Artifacts/Trinkets** — 3 files — Artifact items: Trinkets. Types: Artifact_BloodwoodSpirit, Artifact_ShimmeringTalisman, Artifact_TotemOfVoid.
- **Scripts/Items/Magical/Artifacts/Weapons** — 1 file — Artifact items: Weapons. Types: Artifact_RodOfResurrection.
- **Scripts/Items/Magical/Artifacts/Weapons/Axes** — 11 files — Artifact items: Axes. Types: Artifact_Annihilation, Artifact_AxeOfTheHeavens, Artifact_AxeoftheMinotaur, Artifact_BlazeOfDeath, ....
- **Scripts/Items/Magical/Artifacts/Weapons/Bludgeoning** — 15 files — Artifact items: Bludgeoning. Types: Artifact_ArcticDeathDealer, Artifact_BelmontWhip, Artifact_BoneCrusher, Artifact_Bonesmasher, ....
- **Scripts/Items/Magical/Artifacts/Weapons/Bows** — 16 files — Artifact items: Bows. Types: Artifact_AilricsLongbow, Artifact_BlightGrippedLongbow, Artifact_BowOfTheJukaKing, Artifact_BowofthePhoenix, ....
- **Scripts/Items/Magical/Artifacts/Weapons/Fencing** — 19 files — Artifact items: Fencing. Types: Artifact_AchillesSpear, Artifact_DaggerOfVenom, Artifact_DarkLordsPitchfork, Artifact_EnchantedTitanLegBone, ....
- **Scripts/Items/Magical/Artifacts/Weapons/Staffs** — 7 files — Artifact items: Staffs. Types: Artifact_Boomstick, Artifact_GandalfsStaff, Artifact_PhantomStaff, Artifact_StaffOfPower, ....
- **Scripts/Items/Magical/Artifacts/Weapons/Swordsmanship** — 31 files — Artifact items: Swordsmanship. Types: Artifact_AngeroftheGods, Artifact_BladeDance, Artifact_BladeOfInsanity, Artifact_BladeOfTheRighteous, ....
- **Scripts/Items/Magical/Gifts** — 3 files — Gift/reward item bases and variants.
- **Scripts/Items/Magical/Gifts/Armor** — 2 files — Magical items: Armor. Types: BaseGiftArmor, GiftWoodenPlateLegs.
- **Scripts/Items/Magical/Gifts/Armor/Bone** — 4 files — Magical items: Bone. Types: GiftBoneArms, GiftBoneChest, GiftBoneGloves, GiftBoneLegs.
- **Scripts/Items/Magical/Gifts/Armor/Chain** — 3 files — Magical items: Chain. Types: GiftChainChest, GiftChainHatsuburi, GiftChainLegs.
- **Scripts/Items/Magical/Gifts/Armor/Dragon** — 5 files — Magical items: Dragon. Types: GiftDragonArms, GiftDragonChest, GiftDragonGloves, GiftDragonHelm, ....
- **Scripts/Items/Magical/Gifts/Armor/Helmets** — 13 files — Magical items: Helmets. Types: GiftBascinet, GiftBoneHelm, GiftChainCoif, GiftCirclet, ....
- **Scripts/Items/Magical/Gifts/Armor/Leather** — 26 files — Magical items: Leather. Types: GiftFemaleLeatherChest, GiftLeatherArms, GiftLeatherBustierArms, GiftLeatherChest, ....
- **Scripts/Items/Magical/Gifts/Armor/Plate** — 18 files — Magical items: Plate. Types: GiftDecorativePlateKabuto, GiftFemalePlateChest, GiftHeavyPlateJingasa, GiftLightPlateJingasa, ....
- **Scripts/Items/Magical/Gifts/Armor/Ring** — 4 files — Magical items: Ring. Types: GiftRingmailArms, GiftRingmailChest, GiftRingmailGloves, GiftRingmailLegs.
- **Scripts/Items/Magical/Gifts/Armor/Royal** — 6 files — Magical items: Royal. Types: GiftRoyalArms, GiftRoyalBoots, GiftRoyalChest, GiftRoyalGloves, ....
- **Scripts/Items/Magical/Gifts/Armor/Studded** — 12 files — Magical items: Studded. Types: GiftFemaleStuddedChest, GiftStuddedArms, GiftStuddedBustierArms, GiftStuddedChest, ....
- **Scripts/Items/Magical/Gifts/Clothing** — 11 files — Magical items: Clothing. Types: BaseGiftClothing, BaseGiftCloak, BaseGiftHat, BaseGiftMiddleTorso, ....
- **Scripts/Items/Magical/Gifts/Jewels** — 11 files — Magical items: Jewels. Types: BaseGiftJewel, BaseGiftBracelet, BaseGiftEarrings, GiftNecklace, ....
- **Scripts/Items/Magical/Gifts/Shields** — 20 files — Magical items: Shields. Types: BaseGiftShield, GiftBronzeShield, GiftBuckler, GiftChampionShield, ....
- **Scripts/Items/Magical/Gifts/Weapons** — 4 files — Magical items: Weapons. Types: GiftHarpoon, GiftPugilistGloves, GiftStave, GiftThrowingGloves.
- **Scripts/Items/Magical/Gifts/Weapons/Axes** — 10 files — Magical items: Axes. Types: BaseGiftAxe, GiftAxe, GiftBattleAxe, GiftDoubleAxe, ....
- **Scripts/Items/Magical/Gifts/Weapons/Knives** — 5 files — Magical items: Knives. Types: BaseGiftKnife, GiftButcherKnife, GiftCleaver, GiftDagger, ....
- **Scripts/Items/Magical/Gifts/Weapons/ML Weapons** — 12 files — Magical items: Ml weapons. Types: GiftAssassinSpike, GiftDiamondMace, GiftElvenCompositeLongbow, GiftElvenMachete, ....
- **Scripts/Items/Magical/Gifts/Weapons/Maces** — 12 files — Magical items: Maces. Types: BaseGiftBashing, BaseGiftWhip, GiftClub, GiftHammerPick, ....
- **Scripts/Items/Magical/Gifts/Weapons/PoleArms** — 4 files — Magical items: Pole arms. Types: BaseGiftPoleArm, GiftBardiche, GiftHalberd, GiftScythe.
- **Scripts/Items/Magical/Gifts/Weapons/Ranged** — 6 files — Magical items: Ranged. Types: BaseGiftRanged, GiftBow, GiftCompositeBow, GiftCrossbow, ....
- **Scripts/Items/Magical/Gifts/Weapons/SE Weapons** — 12 files — Magical items: Se weapons. Types: GiftBokuto, GiftDaisho, GiftKama, GiftLajatang, ....
- **Scripts/Items/Magical/Gifts/Weapons/SpearsAndForks** — 9 files — Magical items: Spears and forks. Types: BaseGiftSpear, GiftBladedStaff, GiftDoubleBladedStaff, GiftPike, ....
- **Scripts/Items/Magical/Gifts/Weapons/Staves** — 5 files — Magical items: Staves. Types: BaseGiftStaff, GiftBlackStaff, GiftGnarledStaff, GiftQuarterStaff, ....
- **Scripts/Items/Magical/Gifts/Weapons/Swords** — 15 files — Magical items: Swords. Types: BaseGiftSword, GiftBoneHarvester, GiftBroadsword, GiftClaymore, ....
- **Scripts/Items/Magical/God** — 9 files — Magical items: God. Types: ILevelable, ItemExperienceToken, LegendaryArtifactRename, LegendsBook, ....
- **Scripts/Items/Magical/God/Armor** — 2 files — Magical items: Armor. Types: BaseLevelArmor, LevelWoodenPlateLegs.
- **Scripts/Items/Magical/God/Armor/Bone** — 4 files — Magical items: Bone. Types: LevelBoneArms, LevelBoneChest, LevelBoneGloves, LevelBoneLegs.
- **Scripts/Items/Magical/God/Armor/Chain** — 3 files — Magical items: Chain. Types: LevelChainChest, LevelChainHatsuburi, LevelChainLegs.
- **Scripts/Items/Magical/God/Armor/Dragon** — 5 files — Magical items: Dragon. Types: LevelDragonArms, LevelDragonChest, LevelDragonGloves, LevelDragonHelm, ....
- **Scripts/Items/Magical/God/Armor/Helmets** — 13 files — Magical items: Helmets. Types: LevelBascinet, LevelBoneHelm, LevelChainCoif, LevelCirclet, ....
- **Scripts/Items/Magical/God/Armor/Leather** — 26 files — Magical items: Leather. Types: LevelFemaleLeatherChest, LevelLeatherArms, LevelLeatherBustierArms, LevelLeatherChest, ....
- **Scripts/Items/Magical/God/Armor/Plate** — 18 files — Magical items: Plate. Types: LevelDecorativePlateKabuto, LevelFemalePlateChest, LevelHeavyPlateJingasa, LevelLightPlateJingasa, ....
- **Scripts/Items/Magical/God/Armor/Ring** — 4 files — Magical items: Ring. Types: LevelRingmailArms, LevelRingmailChest, LevelRingmailGloves, LevelRingmailLegs.
- **Scripts/Items/Magical/God/Armor/Royal** — 6 files — Magical items: Royal. Types: LevelRoyalArms, LevelRoyalBoots, LevelRoyalChest, LevelRoyalGloves, ....
- **Scripts/Items/Magical/God/Armor/Studded** — 12 files — Magical items: Studded. Types: LevelFemaleStuddedChest, LevelStuddedArms, LevelStuddedBustierArms, LevelStuddedChest, ....
- **Scripts/Items/Magical/God/Clothing** — 11 files — Magical items: Clothing. Types: BaseLevelClothing, BaseLevelCloak, BaseLevelHat, BaseLevelMiddleTorso, ....
- **Scripts/Items/Magical/God/Commands** — 1 file — Magical items: Commands. Types: ixpCommand.
- **Scripts/Items/Magical/God/Gumps** — 3 files — Magical items: Gumps. Types: AwaitingSmithApprovalGump, ItemExperienceGump, LevelUpAcceptGump.
- **Scripts/Items/Magical/God/Jewels** — 11 files — Magical items: Jewels. Types: BaseLevelJewel, BaseLevelBracelet, BaseLevelEarrings, LevelNecklace, ....
- **Scripts/Items/Magical/God/Shields** — 20 files — Magical items: Shields. Types: BaseLevelShield, LevelBronzeShield, LevelBuckler, LevelChampionShield, ....
- **Scripts/Items/Magical/God/Weapons** — 4 files — Magical items: Weapons. Types: LevelHarpoon, LevelPugilistGloves, LevelStave, LevelThrowingGloves.
- **Scripts/Items/Magical/God/Weapons/Axes** — 10 files — Magical items: Axes. Types: BaseLevelAxe, LevelAxe, LevelBattleAxe, LevelDoubleAxe, ....
- **Scripts/Items/Magical/God/Weapons/Knives** — 5 files — Magical items: Knives. Types: BaseLevelKnife, LevelButcherKnife, LevelCleaver, LevelDagger, ....
- **Scripts/Items/Magical/God/Weapons/ML Weapons** — 12 files — Magical items: Ml weapons. Types: LevelAssassinSpike, LevelDiamondMace, LevelElvenCompositeLongbow, LevelElvenMachete, ....
- **Scripts/Items/Magical/God/Weapons/Maces** — 12 files — Magical items: Maces. Types: BaseLevelBashing, BaseLevelWhip, LevelClub, LevelHammerPick, ....
- **Scripts/Items/Magical/God/Weapons/PoleArms** — 4 files — Magical items: Pole arms. Types: BaseLevelPoleArm, LevelBardiche, LevelHalberd, LevelScythe.
- **Scripts/Items/Magical/God/Weapons/Ranged** — 6 files — Magical items: Ranged. Types: BaseLevelRanged, LevelBow, LevelCompositeBow, LevelCrossbow, ....
- **Scripts/Items/Magical/God/Weapons/SE Weapons** — 12 files — Magical items: Se weapons. Types: LevelBokuto, LevelDaisho, LevelKama, LevelLajatang, ....
- **Scripts/Items/Magical/God/Weapons/SpearsAndForks** — 9 files — Magical items: Spears and forks. Types: BaseLevelSpear, LevelBladedStaff, LevelDoubleBladedStaff, LevelPike, ....
- **Scripts/Items/Magical/God/Weapons/Staves** — 5 files — Magical items: Staves. Types: BaseLevelStaff, LevelBlackStaff, LevelGnarledStaff, LevelQuarterStaff, ....
- **Scripts/Items/Magical/God/Weapons/Swords** — 17 files — Magical items: Swords. Types: BaseLevelSword, LevelBoneHarvester, LevelBroadsword, LevelClaymore, ....
- **Scripts/Items/Misc** — 60 files — Miscellaneous items (dyes, tools, bodies, fragments, currency).
- **Scripts/Items/Misc/Bodies** — 9 files — Items: Bodies. Types: BodyPart, BonePile, Head, LeftArm, ....
- **Scripts/Items/Misc/Bodies/Corpses** — 4 files — Items: Corpses. Types: Corpse, CorpseNameAttribute, DecayedCorpse, CorpseEquip.
- **Scripts/Items/Misc/Bodies/LivingDead** — 3 files — Items: Living dead. Types: BookofDead, DarkHeart, SummonCorpse.
- **Scripts/Items/Misc/Christmas** — 21 files — Items: Christmas. Types: ChristmasRobe, GiftBox, GingerBreadHouseDeed, HolidayBell, ....
- **Scripts/Items/Misc/Crystal** — 8 files — Items: Crystal. Types: ECrystalAltarComponent, ECrystalBeggarStatueComponent, ECrystalBrazierComponent, ECrystalBullComponent, ....
- **Scripts/Items/Misc/Dyes** — 23 files — Items: Dyes. Types: AllDyeTubsArmor, AllDyeTubsBookRune, AllDyeTubsBookSpell, AllDyeTubsFurniture, ....
- **Scripts/Items/Misc/Dyes/Essence** — 3 files — Items: Essence. Types: EssenceBase, EssenceBaseEmpty, EssenceOrb.
- **Scripts/Items/Misc/Evil** — 11 files — Items: Evil. Types: EGlobeOfSosariaComponent, EObsidianPillarComponent, EObsidianRockComponent, EShadowAltarComponent, ....
- **Scripts/Items/Misc/Facial** — 2 files — Items: Facial. Types: Beard, Hair.
- **Scripts/Items/Misc/Halloween** — 43 files — Items: Halloween. Types: AppleBobbingBarrel, BloodPentagramDeed, BloodyTableAddon, CarvedPumpkin, ....
- **Scripts/Items/Misc/Market** — 34 files — Items: Market. Types: AppleTrunkAddon, BlueDecorativeRugAddon, BlueFancyRugAddon, BluePlainRugAddon, ....
- **Scripts/Items/Misc/Mounts** — 5 files — Items: Mounts. Types: DaemonMount, DeathKnightWarhorse, NecroHorse, PaladinWarhorse, ....
- **Scripts/Items/Misc/Rugs** — 4 files — Items: Rugs. Types: RoseEastLargeAddon, RoseEastSmallAddon, RoseSouthLargeAddon, RoseSouthSmallAddon.
- **Scripts/Items/Misc/Scrolls** — 3 files — Items: Scrolls. Types: ScrollofAlacrity, ScrollofTranscendence, SpecialScroll.
- **Scripts/Items/Misc/Translocation** — 5 files — Items: Translocation. Types: BagOfSending, BallOfSummoning, BraceletOfBinding, MessageHelper, ....
- **Scripts/Items/Potions** — 1 file — Items: Potions. Types: BasePotion.
- **Scripts/Items/Potions/Elixirs** — 2 files — Items: Elixirs. Types: BaseElixir, ElixirAlchemy.
- **Scripts/Items/Potions/Mixtures** — 13 files — Items: Mixtures. Types: AlchemicSlime, BaseLiquid, BaseMixture, LiquidFire, ....
- **Scripts/Items/Potions/Special** — 23 files — Items: Special. Types: AutoResPotion, BottleOfAcid, BrewCauldron, CanopicJar, ....
- **Scripts/Items/Potions/Standard** — 1 file — Items: Standard. Types: NightSightPotion.
- **Scripts/Items/Potions/Standard/Agility Potions** — 3 files — Items: Agility potions. Types: AgilityPotion, BaseAgilityPotion, GreaterAgilityPotion.
- **Scripts/Items/Potions/Standard/Conflagration Potions** — 3 files — Items: Conflagration potions. Types: BaseConflagrationPotion, ConflagrationPotion, GreaterConflagrationPotion.
- **Scripts/Items/Potions/Standard/Confusion Blast Potions** — 3 files — Items: Confusion blast potions. Types: BaseConfusionBlastPotion, ConfusionBlastPotion, GreaterConfusionBlastPotion.
- **Scripts/Items/Potions/Standard/Cure Potions** — 4 files — Items: Cure potions. Types: BaseCurePotion, CurePotion, GreaterCurePotion, LesserCurePotion.
- **Scripts/Items/Potions/Standard/Explosion Potions** — 4 files — Items: Explosion potions. Types: BaseExplosionPotion, ExplosionPotion, GreaterExplosionPotion, LesserExplosionPotion.
- **Scripts/Items/Potions/Standard/Frostbite Potions** — 3 files — Items: Frostbite potions. Types: BaseFrostbitePotion, FrostbitePotion, GreaterFrostbitePotion.
- **Scripts/Items/Potions/Standard/Heal Potions** — 4 files — Items: Heal potions. Types: BaseHealPotion, GreaterHealPotion, HealPotion, LesserHealPotion.
- **Scripts/Items/Potions/Standard/Poison Potions** — 7 files — Items: Poison potions. Types: BasePoisonPotion, DeadlyPoisonPotion, GreaterPoisonPotion, LesserPoisonPotion, ....
- **Scripts/Items/Potions/Standard/Refresh Potions** — 3 files — Items: Refresh potions. Types: BaseRefreshPotion, RefreshPotion, TotalRefreshPotion.
- **Scripts/Items/Potions/Standard/Strength Potions** — 3 files — Items: Strength potions. Types: BaseStrengthPotion, GreaterStrengthPotion, StrengthPotion.
- **Scripts/Items/Potions/Unique** — 11 files — Items: Unique. Types: BaseManaRefreshPotion, BaseRejuvenatePotion, GreaterInvisibilityPotion, GreaterManaPotion, ....
- **Scripts/Items/Quivers** — 3 files — Items: Quivers. Types: ArcherQuiver, BaseQuiver, ElvenQuiver.
- **Scripts/Items/Relics** — 28 files — Relic loot family, identification flow and legacy worth text.
- **Scripts/Items/Sharpening** — 3 files — Items: Sharpening. Types: AddDamageItemBase, ConsecrateItemBase, SplitElementalItemBase.
- **Scripts/Items/Sharpening/BowStrings** — 6 files — Items: Bow strings. Types: ConsecratedBowString, DamageIncreaseBowStringBase, DenseBowString, ElementalBowString, ....
- **Scripts/Items/Sharpening/SharpeningStones** — 7 files — Items: Sharpening stones. Types: ConsecratedSharpeningStone, DamageIncreaseSharpeningStoneBase, DenseSharpeningStone, ElementalSharpeningStone, ....
- **Scripts/Items/Sharpening/WeightingStones** — 7 files — Items: Weighting stones. Types: ConsecratedWeightingStone, DamageIncreaseWeightingStoneBase, DenseWeightingStone, ElementalWeightingStone, ....
- **Scripts/Items/Special** — 29 files — Special/quest/event items and one-off systems.
- **Scripts/Items/Special/Broken Furniture Collection** — 8 files — Items: Broken furniture collection. Types: BrokenArmoireComponent, BrokenBedAddon, BrokenBookcaseComponent, BrokenChestOfDrawersComponent, ....
- **Scripts/Items/Special/Evil Home Decor Collection** — 15 files — Items: Evil home decor collection. Types: AwesomeDisturbingPortraitComponent, BedOfNailsComponent, BoneCouchComponent, BoneTableAddon, ....
- **Scripts/Items/Special/Items** — 10 files — Items: Items. Types: Banner, BloodyPentagramComponent, RewardBrazier, DecorativeShield, ....
- **Scripts/Items/Special/Rares** — 0 files — No direct scripts; groups subfolders.
- **Scripts/Items/Special/Rares/Containers** — 7 files — Items: Containers. Types: BaseWaterContainer, Bucket, ClosedBarrel, HugeWaterTub, ....
- **Scripts/Items/Special/Rares/Daily** — 4 files — Items: Daily. Types: DecoRock, DecoRock2, DecoRocks, DecoRocks2.
- **Scripts/Items/Special/Rares/Flowers** — 5 files — Items: Flowers. Types: DecoFlower, DecoFlower2, DecoRoseOfTrinsic, DecoRoseOfTrinsic2, ....
- **Scripts/Items/Special/Rares/Food** — 3 files — Items: Food. Types: DecoBottlesOfLiquor, DecoTray2, DecoTray.
- **Scripts/Items/Special/Rares/Furniture** — 1 file — Items: Furniture. Types: BrokenChair.
- **Scripts/Items/Special/Rares/GamePieces** — 5 files — Items: Game pieces. Types: Checkers, Checkers2, Chessmen, Chessmen2, ....
- **Scripts/Items/Special/Rares/Ingots** — 21 files — Items: Ingots. Types: DecoGoldIngot, DecoGoldIngot2, DecoGoldIngots, DecoGoldIngots2, ....
- **Scripts/Items/Special/Rares/Jars** — 3 files — Items: Jars. Types: EmptyJars, DecoFullJar, HalfEmptyJars.
- **Scripts/Items/Special/Rares/Magic** — 2 files — Items: Magic. Types: DecoCrystalBall, DecoMagicalCrystal.
- **Scripts/Items/Special/Rares/Misc** — 1 file — Items: Misc. Types: DecoSpittoon.
- **Scripts/Items/Special/Rares/PaganReagents** — 25 files — Items: Pagan reagents. Types: DecoBlackmoor, DecoBloodspawn, DecoBrimstone, DecoDragonsBlood, ....
- **Scripts/Items/Special/Rares/PlayingCards** — 7 files — Items: Playing cards. Types: Cards, Cards2, Cards3, Cards4, ....
- **Scripts/Items/Special/Rares/Stables** — 5 files — Items: Stables. Types: DecoBridle, DecoBridle2, DecoHay, DecoHay2, ....
- **Scripts/Items/Special/Rares/TarotCards** — 9 files — Items: Tarot cards. Types: DecoDeckOfTarot, DecoDeckOfTarot2, DecoTarot, DecoTarot2, ....
- **Scripts/Items/Special/Rares/Tinker** — 6 files — Items: Tinker. Types: DecoArrowShafts, CrossbowBolts, EmptyToolKit, EmptyToolKit2, ....
- **Scripts/Items/Technology** — 26 files — Sci-fi/technology items (ZorkPad, DataPads, devices).
- **Scripts/Items/Trades** — 6 files — Crafting tools, resources and trade skill items.
- **Scripts/Items/Trades/Alchemy** — 7 files — Items: Alchemy. Types: AlchemyTub, GodBrewing, Bottle, CrystallineJar, ....
- **Scripts/Items/Trades/Blacksmithing** — 16 files — Items: Blacksmithing. Types: Anvil, BaseBlocks, DwarvenForge, FireGiantForge, ....
- **Scripts/Items/Trades/Bowcraft** — 7 files — Items: Bowcraft. Types: Arrow, ArrowsAndBolts, Bolt, Feather, ....
- **Scripts/Items/Trades/Carpentry** — 5 files — Items: Carpentry. Types: Board, CarpenterTools, CarpenterToolsRunicI, TaxidermyKit, ....
- **Scripts/Items/Trades/Cartography** — 1 file — Items: Cartography. Types: MapmakersPen.
- **Scripts/Items/Trades/Cartography/Maps** — 12 files — Items: Maps. Types: BlankMap, CityMap, IndecipherableMap, LocalMap, ....
- **Scripts/Items/Trades/Cooking** — 1 file — Items: Cooking. Types: CulinarySet.
- **Scripts/Items/Trades/Druid** — 1 file — Items: Druid. Types: DruidCauldron.
- **Scripts/Items/Trades/Fishing** — 16 files — Items: Fishing. Types: BigFish, FabledFishingNet, Fish, FishBarrel, ....
- **Scripts/Items/Trades/Fishing/Misc** — 5 files — Items: Misc. Types: MagicSextant, MessageInABottle, SOS, Sextant, ....
- **Scripts/Items/Trades/Fishing/Sea Artifacts** — 14 files — Items: Sea artifacts. Types: AquariumEastAddon, AquariumSouthAddon, DolphinEastLargeAddon, DolphinEastSmallAddon, ....
- **Scripts/Items/Trades/Forensics** — 6 files — Items: Forensics. Types: Bones, GraveSpade, PolishBoneBrush, BaseSkeletal, ....
- **Scripts/Items/Trades/Glass Stone** — 8 files — Items: Glass stone. Types: Blowpipe, GlassblowingBook, Granite, MalletAndChisel, ....
- **Scripts/Items/Trades/Inscription** — 4 files — Items: Inscription. Types: Monocle, ScribesPen, ScribesPenRunicI, TomeOfWands.
- **Scripts/Items/Trades/Lumberjack** — 3 files — Items: Lumberjack. Types: Log, SawMillEastAddon, SawMillSouthAddon.
- **Scripts/Items/Trades/Magical** — 4 files — Items: Magical. Types: ArcaneScroll, BlankScroll, Moongate, RecallRune.
- **Scripts/Items/Trades/Magical/Tools** — 9 files — Items: Tools. Types: BaseRunicTool, RunicFletching, RunicHammer, RunicLeatherKit, ....
- **Scripts/Items/Trades/Misc** — 2 files — Items: Misc. Types: Bandage, FireHorn.
- **Scripts/Items/Trades/Ninjitsu** — 7 files — Items: Ninjitsu. Types: EggBomb, Fukiya, FukiyaDarts, LeatherNinjaBelt, ....
- **Scripts/Items/Trades/Plants** — 0 files — No direct scripts; groups subfolders.
- **Scripts/Items/Trades/Plants/Potted** — 4 files — Items: Potted. Types: SmallEmptyPot, PottedCactus, PottedPlant, PottedTree.
- **Scripts/Items/Trades/Reagents** — 6 files — Items: Reagents. Types: BagOfAlchemicReagents, BagOfAllReagents, BagOfNecroReagents, BagOfReagents, ....
- **Scripts/Items/Trades/Reagents/Alchemy** — 12 files — Items: Alchemy. Types: BeetleShell, Brimstone, ButterflyWings, EyeOfToad, ....
- **Scripts/Items/Trades/Reagents/Common** — 8 files — Items: Common. Types: BlackPearl, Bloodmoss, Garlic, Ginseng, ....
- **Scripts/Items/Trades/Reagents/Farm** — 4 files — Items: Farm. Types: FarmableGarlic, FarmableGinseng, FarmableMandrakeRoot, FarmableNightshade.
- **Scripts/Items/Trades/Reagents/Necromancy** — 5 files — Items: Necromancy. Types: BatWing, DaemonBlood, GraveDust, NoxCrystal, ....
- **Scripts/Items/Trades/Reagents/Unique** — 13 files — Items: Unique. Types: DemigodBlood, DemonClaw, DragonBlood, DragonTooth, ....
- **Scripts/Items/Trades/Reagents/Witch** — 9 files — Items: Witch. Types: BitterRoot, BlackSand, BloodRose, DriedToad, ....
- **Scripts/Items/Trades/Tailoring** — 17 files — Items: Tailoring. Types: Bone, Cotton, Dressform, Dyes, ....
- **Scripts/Items/Trades/Thieving** — 8 files — Items: Thieving. Types: DisguiseKit, DisguisePersistance, ILockpickable, MagicSkeltonsKey, ....
- **Scripts/Items/Trades/Tinkering** — 13 files — Items: Tinkering. Types: Axle, AxleGears, ClockFrame, ClockParts, ....
- **Scripts/Items/Trades/Witch** — 1 file — Items: Witch. Types: WitchCauldron.
- **Scripts/Items/Traps** — 29 files — Items: Traps. Types: BaseTrap, StopBlock, BookBox, BrokenGear, ....
- **Scripts/Items/Trinkets** — 14 files — Items: Trinkets. Types: BaseTrinket, GuildRing, JewelryBracelet, JewelryCirclet, ....
- **Scripts/Items/Weapons** — 7 files — Weapon item classes grouped by family.
- **Scripts/Items/Weapons/Axes** — 10 files — Weapon items: Axes. Types: Axe, BaseAxe, BattleAxe, DoubleAxe, ....
- **Scripts/Items/Weapons/Bows** — 9 files — Weapon items: Bows. Types: BaseRanged, Bow, CompositeBow, Crossbow, ....
- **Scripts/Items/Weapons/Hands** — 3 files — Weapon items: Hands. Types: Fists, PugilistGlove, PugilistMits.
- **Scripts/Items/Weapons/Knives** — 9 files — Weapon items: Knives. Types: AssassinSpike, BaseKnife, ButcherKnife, Cleaver, ....
- **Scripts/Items/Weapons/Maces** — 14 files — Weapon items: Maces. Types: BaseBashing, BaseWhip, Club, DiamondMace, ....
- **Scripts/Items/Weapons/Marksman** — 5 files — Weapon items: Marksman. Types: Harpoon, MonsterGloves, ThrowingGloves, ThrowingWeapon, ....
- **Scripts/Items/Weapons/Oriental** — 12 files — Weapon items: Oriental. Types: Bokuto, Daisho, Kama, Lajatang, ....
- **Scripts/Items/Weapons/PoleArms** — 4 files — Weapon items: Pole arms. Types: Bardiche, BasePoleArm, Halberd, Scythe.
- **Scripts/Items/Weapons/SpearsAndForks** — 10 files — Weapon items: Spears and forks. Types: BaseSpear, BladedStaff, DoubleBladedStaff, Pike, ....
- **Scripts/Items/Weapons/Staves** — 7 files — Weapon items: Staves. Types: BaseStaff, BlackStaff, GlacialStaff, GnarledStaff, ....
- **Scripts/Items/Weapons/Swords** — 19 files — Weapon items: Swords. Types: BaseSword, BoneHarvester, Broadsword, Claymore, ....
- **Scripts/Mobiles** — 0 files — NPC and creature definitions.
- **Scripts/Mobiles/Animals** — 0 files — No direct scripts; groups subfolders.
- **Scripts/Mobiles/Animals/Bears** — 17 files — Creatures/NPCs: Bears. Types: BlackBear, BrownBear, CaveBearRiding, DireBear, ....
- **Scripts/Mobiles/Animals/Birds** — 12 files — Creatures/NPCs: Birds. Types: AxeBeakRiding, Bird, Chicken, Crane, ....
- **Scripts/Mobiles/Animals/Canines** — 14 files — Creatures/NPCs: Canines. Types: BlackWolf, Dog, Fox, GreyWolf, ....
- **Scripts/Mobiles/Animals/Cows** — 3 files — Creatures/NPCs: Cows. Types: Bull, BullradonRiding, Cow.
- **Scripts/Mobiles/Animals/Felines** — 18 files — Creatures/NPCs: Felines. Types: BlackCat, Bobcat, Cat, Cheetah, ....
- **Scripts/Mobiles/Animals/Misc** — 32 files — Creatures/NPCs: Misc. Types: Antelope, Ape, Boar, DireBoar, ....
- **Scripts/Mobiles/Animals/Mounts** — 16 files — Creatures/NPCs: Mounts. Types: DesertOstard, EtherealMount, EvilMount, ForestOstard, ....
- **Scripts/Mobiles/Animals/Rodents** — 19 files — Creatures/NPCs: Rodents. Types: AlbinoBat, Bat, Critter, DiseasedRat, ....
- **Scripts/Mobiles/Base** — 26 files — Base mobile classes, AI/behavior helpers, vendors, player preferences.
- **Scripts/Mobiles/Civilized** — 13 files — Town NPCs, vendors, guards, citizens and services.
- **Scripts/Mobiles/Civilized/Citizens** — 26 files — Creatures/NPCs: Citizens. Types: AdventurerEast, Citizens, DragonRider, HouseVisitor, ....
- **Scripts/Mobiles/Civilized/Comrades** — 11 files — Creatures/NPCs: Comrades. Types: HenchHorse, HenchmanArcher, HenchmanArcherItem, HenchmanFighter, ....
- **Scripts/Mobiles/Civilized/Familiars** — 9 files — Creatures/NPCs: Familiars. Types: DarkWolfFamiliar, DeathAdder, HenchmanFamiliar, HenchmanFamiliarItem, ....
- **Scripts/Mobiles/Civilized/Guilds** — 22 files — Creatures/NPCs: Guilds. Types: AlchemistGuildmaster, ArcherGuildmaster, AssassinGuildmaster, BardGuildmaster, ....
- **Scripts/Mobiles/Civilized/Healers** — 3 files — Creatures/NPCs: Healers. Types: EvilHealer, Healer, WanderingHealer.
- **Scripts/Mobiles/Civilized/Merchants** — 68 files — Creatures/NPCs: Merchants. Types: Alchemist, AnimalTrainer, Architect, Armorer, ....
- **Scripts/Mobiles/Civilized/Porters** — 2 files — Creatures/NPCs: Porters. Types: PackBeast, PackBeastItem.
- **Scripts/Mobiles/Civilized/Special** — 10 files — Creatures/NPCs: Special. Types: Courier, Devon, EpicCharacter, EpicPet, ....
- **Scripts/Mobiles/Civilized/Teachers** — 5 files — Creatures/NPCs: Teachers. Types: ExaltedDealer, LegendaryDealer, MythicalDealer, PowerDealer, ....
- **Scripts/Mobiles/Constructs** — 1 file — Creatures/NPCs: Constructs. Types: WaxSculpture.
- **Scripts/Mobiles/Constructs/Alien** — 7 files — Creatures/NPCs: Alien. Types: BattleDroid, CombatDroid, ExcavationDroid, MaintenanceDroid, ....
- **Scripts/Mobiles/Constructs/Golems** — 14 files — Creatures/NPCs: Golems. Types: AncientFleshGolem, BoneGolem, CaddelliteGolem, FleshGolem, ....
- **Scripts/Mobiles/Constructs/Statues** — 9 files — Creatures/NPCs: Statues. Types: AnyStatue, LivingBronzeStatue, LivingGoldStatue, LivingIronStatue, ....
- **Scripts/Mobiles/Demons** — 32 files — Creatures/NPCs: Demons. Types: AbysmalDaemon, Afreet, Archfiend, Balron, ....
- **Scripts/Mobiles/Dragons** — 3 files — Creatures/NPCs: Dragons. Types: DragonTurtle, Dragonogre, DrakkhenRed.
- **Scripts/Mobiles/Dragons/Dragons** — 7 files — Creatures/NPCs: Dragons. Types: AsianDragon, DragonGolem, Dragons, Dragoon, ....
- **Scripts/Mobiles/Dragons/Drakes** — 5 files — Creatures/NPCs: Drakes. Types: AbysmalDrake, AncientDrake, Drake, SeaDrake, ....
- **Scripts/Mobiles/Dragons/Great Dragons** — 8 files — Creatures/NPCs: Great dragons. Types: AshDragon, BottleDragon, CaddelliteDragon, CrystalDragon, ....
- **Scripts/Mobiles/Dragons/Hydras** — 3 files — Creatures/NPCs: Hydras. Types: EnergyHydra, Hydra, Wyvra.
- **Scripts/Mobiles/Dragons/Primeval** — 16 files — Creatures/NPCs: Primeval. Types: PrimevalAbysmalDragon, PrimevalAmberDragon, PrimevalBlackDragon, PrimevalDragon, ....
- **Scripts/Mobiles/Dragons/Wyrms** — 5 files — Creatures/NPCs: Wyrms. Types: AncientWyrm, ShadowWyrm, VolcanicDragon, Wyrm, ....
- **Scripts/Mobiles/Dragons/Wyverns** — 2 files — Creatures/NPCs: Wyverns. Types: AncientWyvern, Wyverns.
- **Scripts/Mobiles/Dragons/Young** — 3 files — Creatures/NPCs: Young. Types: BabyDragon, FireWyrmling, YoungDragon.
- **Scripts/Mobiles/Elementals** — 10 files — Creatures/NPCs: Elementals. Types: CrystalGoliath, Efreet, IceColossus, MudMan, ....
- **Scripts/Mobiles/Elementals/Elementals** — 29 files — Creatures/NPCs: Elementals. Types: AirElemental, AnyElemental, AnyGemElemental, BloodElemental, ....
- **Scripts/Mobiles/Elementals/Gemmed** — 14 files — Creatures/NPCs: Gemmed. Types: CaddelliteElemental, CrystalElemental, DilithiumElemental, GarnetElemental, ....
- **Scripts/Mobiles/Elementals/Lesser** — 2 files — Creatures/NPCs: Lesser. Types: BloodSpawn, WaterSpawn.
- **Scripts/Mobiles/Elementals/Ore Elementals** — 11 files — Creatures/NPCs: Ore elementals. Types: AgapiteElemental, AnimatedRocks, BronzeElemental, CopperElemental, ....
- **Scripts/Mobiles/Gargoyles** — 15 files — Creatures/NPCs: Gargoyles. Types: AncientGargoyle, CosmicGargoyle, FireGargoyle, Gargoyle, ....
- **Scripts/Mobiles/Goliaths** — 2 files — Creatures/NPCs: Goliaths. Types: OrkDemigod, ZornTheBlacksmith.
- **Scripts/Mobiles/Goliaths/Cyclops** — 3 files — Creatures/NPCs: Cyclops. Types: AncientCyclops, Cyclops, ShamanicCyclops.
- **Scripts/Mobiles/Goliaths/Ettins** — 4 files — Creatures/NPCs: Ettins. Types: AncientEttin, ArcticEttin, Ettin, EttinShaman.
- **Scripts/Mobiles/Goliaths/Giants** — 18 files — Creatures/NPCs: Giants. Types: AbyssGiant, CloudGiant, DeepSeaGiant, FireGiant, ....
- **Scripts/Mobiles/Goliaths/Ogres** — 6 files — Creatures/NPCs: Ogres. Types: AbysmalOgre, ArcticOgreLord, Ogre, OgreLord, ....
- **Scripts/Mobiles/Goliaths/Titans** — 2 files — Creatures/NPCs: Titans. Types: ElderTitan, Titan.
- **Scripts/Mobiles/Goliaths/Trolls** — 6 files — Creatures/NPCs: Trolls. Types: FrostTroll, FrostTrollShaman, SeaTroll, SwampTroll, ....
- **Scripts/Mobiles/Hellish** — 13 files — Creatures/NPCs: Hellish. Types: AncientNightmareRiding, Cerberus, Chimera, DarkHound, ....
- **Scripts/Mobiles/Humanoids** — 15 files — Creatures/NPCs: Humanoids. Types: Bugbear, Drider, DriderWizard, Durgar, ....
- **Scripts/Mobiles/Humanoids/Aliens** — 5 files — Creatures/NPCs: Aliens. Types: BombWorshipper, Jedi, Psionicist, SavageAlien, ....
- **Scripts/Mobiles/Humanoids/Ambroz** — 3 files — Creatures/NPCs: Ambroz. Types: AbrozChieftain, AbrozShaman, AbrozWarrior.
- **Scripts/Mobiles/Humanoids/Drakkul** — 3 files — Creatures/NPCs: Drakkul. Types: Drakkul, DrakkulChief, DrakkulMage.
- **Scripts/Mobiles/Humanoids/Elves** — 5 files — Creatures/NPCs: Elves. Types: ElfBerserker, ElfMage, ElfMinstrel, ElfMonks, ....
- **Scripts/Mobiles/Humanoids/Gnome** — 3 files — Creatures/NPCs: Gnome. Types: Gnome, GnomeMage, GnomeWarrior.
- **Scripts/Mobiles/Humanoids/Harpies** — 5 files — Creatures/NPCs: Harpies. Types: Harpy, HarpyElder, HarpyHen, SnowHarpy, ....
- **Scripts/Mobiles/Humanoids/Humans** — 14 files — Creatures/NPCs: Humans. Types: Adventurers, Archmage, Bandit, Berserker, ....
- **Scripts/Mobiles/Humanoids/Kilrathi** — 2 files — Creatures/NPCs: Kilrathi. Types: Kilrathi, KilrathiGunner.
- **Scripts/Mobiles/Humanoids/Lizardmen** — 14 files — Creatures/NPCs: Lizardmen. Types: Grathek, Kobold, KoboldMage, Lizardman, ....
- **Scripts/Mobiles/Humanoids/Minotaurs** — 5 files — Creatures/NPCs: Minotaurs. Types: Minotaur, MinotaurCaptain, MinotaurScout, MinotaurSmall, ....
- **Scripts/Mobiles/Humanoids/Orcs** — 16 files — Creatures/NPCs: Orcs. Types: Orc, OrcBomber, OrcCaptain, OrcishLord, ....
- **Scripts/Mobiles/Humanoids/Ratmen** — 3 files — Creatures/NPCs: Ratmen. Types: Ratman, RatmanArcher, RatmanMage.
- **Scripts/Mobiles/Humanoids/Sailors** — 3 files — Creatures/NPCs: Sailors. Types: BaseSailor, BoatPirateArcher, BoatSailorArcher.
- **Scripts/Mobiles/Humanoids/Sailors/Galleons** — 35 files — Creatures/NPCs: Galleons. Types: BasePirate, PirateCult, PirateCyclops, PirateDaemon, ....
- **Scripts/Mobiles/Humanoids/Sailors/Pirates** — 11 files — Creatures/NPCs: Pirates. Types: ElfPirateCaptain, ElfPirateCrew, ElfPirateCrewBow, ElfPirateCrewMage, ....
- **Scripts/Mobiles/Humanoids/Savages** — 10 files — Creatures/NPCs: Savages. Types: Native, NativeArcher, NativeWitchDoctor, Savage, ....
- **Scripts/Mobiles/Humanoids/Sea** — 10 files — Creatures/NPCs: Sea. Types: Dagon, Krakoa, Lobstran, Locathah, ....
- **Scripts/Mobiles/Humanoids/Serpents** — 15 files — Creatures/NPCs: Serpents. Types: FireNaga, FireSalamander, Naga, OphidianArchmage, ....
- **Scripts/Mobiles/Humanoids/Terathan** — 4 files — Creatures/NPCs: Terathan. Types: TerathanAvenger, TerathanDrone, TerathanMatriarch, TerathanWarrior.
- **Scripts/Mobiles/Insects** — 5 files — Creatures/NPCs: Insects. Types: Anhkheg, DeadlyScorpion, Lavapede, Mantis, ....
- **Scripts/Mobiles/Insects/Aliens** — 4 files — Creatures/NPCs: Aliens. Types: Alien, AlienSmall, AlienSpider, Shaclaw.
- **Scripts/Mobiles/Insects/Ants** — 5 files — Creatures/NPCs: Ants. Types: AntLion, AntaurKing, AntaurProgenitor, AntaurSoldier, ....
- **Scripts/Mobiles/Insects/Beetles** — 12 files — Creatures/NPCs: Beetles. Types: Beetle, DeathwatchBeetle, DeathwatchBeetleHatchling, DuneBeetle, ....
- **Scripts/Mobiles/Insects/Spiders** — 15 files — Creatures/NPCs: Spiders. Types: AbyssCrawler, Arachnar, CaveFisher, DreadSpider, ....
- **Scripts/Mobiles/Mystical** — 33 files — Creatures/NPCs: Mystical. Types: AncientSphinx, Angel, Archangel, Centaur, ....
- **Scripts/Mobiles/Omni AI** — 9 files — OmniAI behaviour framework used by custom creatures.
- **Scripts/Mobiles/Plants** — 21 files — Creatures/NPCs: Plants. Types: AncientEnt, AncientReaper, BloodLotus, BogThing, ....
- **Scripts/Mobiles/Races** — 3 files — Creatures/NPCs: Races. Types: BaseRace, NPCRace, RacePotions.
- **Scripts/Mobiles/Reptilian** — 21 files — Creatures/NPCs: Reptilian. Types: Alligator, BasiliskRiding, CaveLizard, Crocodile, ....
- **Scripts/Mobiles/Reptilian/Dinosaurs** — 12 files — Creatures/NPCs: Dinosaurs. Types: Basilosaurus, Brontosaur, GorceratopsRiding, Iguanodon, ....
- **Scripts/Mobiles/Reptilian/Fish** — 5 files — Creatures/NPCs: Fish. Types: GreatWhite, Lochasaur, Megalodon, SeaHorses, ....
- **Scripts/Mobiles/Reptilian/Frogs** — 7 files — Creatures/NPCs: Frogs. Types: BullFrog, FireToad, Frog, GiantToad, ....
- **Scripts/Mobiles/Reptilian/Sea** — 10 files — Creatures/NPCs: Sea. Types: Calamari, GiantCrab, GiantEel, GiantLamprey, ....
- **Scripts/Mobiles/Reptilian/Snakes** — 21 files — Creatures/NPCs: Snakes. Types: BloodSnake, DeepSeaSerpent, GiantAdder, GiantIceWorm, ....
- **Scripts/Mobiles/Slimes** — 15 files — Creatures/NPCs: Slimes. Types: AcidPuddle, BlackPudding, BloodWorm, CarcassWorm, ....
- **Scripts/Mobiles/Summoned** — 26 files — Creatures/NPCs: Summoned. Types: BladeSpirits, DeathVortex, EnergyVortex, EvilBladeSpirits, ....
- **Scripts/Mobiles/Undead** — 90 files — Creatures/NPCs: Undead. Types: AncientLich, AquaticGhoul, Bodak, BoneKnight, ....
- **Scripts/Mobiles/Unique** — 20 files — Creatures/NPCs: Unique. Types: BaneOfAnarchy, BaneOfInsanity, BaneOfWantoness, BaronAlmric, ....
- **Scripts/Mobiles/Unusual** — 21 files — Creatures/NPCs: Unusual. Types: Beholder, BloodGodTentacles, Cronosaurus, ElderGazer, ....
- **Scripts/System** — 0 files — Support systems: commands, gumps, help, networking, skills, spawns, regions.
- **Scripts/System/Cache** — 2 files — Support system: Cache. Types: ItemSnapshot, ItemSnapshotCache.
- **Scripts/System/Chat** — 2 files — Support system: Chat. Types: ChatJail, RUOVersion.
- **Scripts/System/Chat/Channels** — 8 files — Support system: Channels. Types: Alliance, Channel, Faction, Guild, ....
- **Scripts/System/Chat/Commands** — 6 files — Support system: Commands. Types: All, Friends, HelpContents, Mail, ....
- **Scripts/System/Chat/General** — 15 files — Support system: General. Types: Avatar, Chat3Guild, Chat3Party, CommandInfo, ....
- **Scripts/System/Chat/Gumps** — 0 files — No direct scripts; groups subfolders.
- **Scripts/System/Chat/Gumps/1.0 Skin** — 2 files — Support system: 1.0 skin. Types: ListGump10, PmNotifyGump10.
- **Scripts/System/Chat/Gumps/2.0 Skin** — 2 files — Support system: 2.0 skin. Types: ListGump20, PmNotifyGump20.
- **Scripts/System/Chat/Gumps/3.0 Skin** — 18 files — Support system: 3.0 skin. Types: ChannelGump, ColorsGump, EditNotGump, FilterGump, ....
- **Scripts/System/Chat/Gumps/Error Reporting** — 3 files — Support system: Error reporting. Types: Errors, ErrorsGump, ErrorsNotifyGump.
- **Scripts/System/Chat/Gumps/Gumps Plus** — 7 files — Support system: Gumps plus. Types: BackgroundPlus, ButtonPlus, GumpInfo, GumpPlus, ....
- **Scripts/System/Commands** — 44 files — GM/player commands and command frameworks.
- **Scripts/System/Commands/Commands** — 3 files — Command modules: Commands. Types: BaseCommand, TargetCommands, InterfaceCommand.
- **Scripts/System/Commands/Extensions** — 5 files — Command modules: Extensions. Types: BaseExtension, DistinctExtension, LimitExtension, SortExtension, ....
- **Scripts/System/Commands/Extensions/Compilers** — 3 files — Command modules: Compilers. Types: ConditionalCompiler, DistinctCompiler, SortCompiler.
- **Scripts/System/Commands/Implementors** — 11 files — Command modules: Implementors. Types: AreaCommandImplementor, BaseCommandImplementor, ContainedCommandImplementor, GlobalCommandImplementor, ....
- **Scripts/System/Commands/Player** — 39 files — Command modules: Player. Types: AFK, AutoAttack, AutoSheatheWeapon, BandSelf, ....
- **Scripts/System/Gumps** — 39 files — Server-side gump UIs.
- **Scripts/System/Gumps/Go** — 4 files — Gump UI: Go. Types: ChildNode, GoGump, LocationTree, ParentNode.
- **Scripts/System/Gumps/Guilds** — 26 files — Gump UI: Guilds. Types: DeclareFealtyGump, GrantGuildTitleGump, GuildAbbrvPrompt, GuildAcceptWarGump, ....
- **Scripts/System/Gumps/Guilds/New Guild System** — 11 files — Gump UI: New guild system. Types: GuildAdvancedSearchGump, BaseGuildGump, BaseGuildListGump, CreateGuildGump, ....
- **Scripts/System/Gumps/Properties** — 11 files — Gump UI: Properties. Types: PropsConfig, PropertiesGump, SetBodyGump, SetCustomEnumGump, ....
- **Scripts/System/Help** — 12 files — Help gump, settings UI and player preference pages.
- **Scripts/System/Misc** — 81 files — Cross-cutting systems (tooltips/worth/tiers, item sales, settings, random things).
- **Scripts/System/Network** — 1 file — Packets, packet handlers, networking extensions.
- **Scripts/System/Regions** — 32 files — Custom regions and region logic.
- **Scripts/System/Regions/Core** — 5 files — Support system: Core. Types: BaseRegion, HouseRegion, Jail, RegionMusic, ....
- **Scripts/System/Regions/Spawning** — 3 files — Support system: Spawning. Types: SpawnDefinition, SpawnEntry, SpawnPersistence.
- **Scripts/System/Skills** — 26 files — Skill systems and skill-specific mechanics.
- **Scripts/System/Skills/Weapon Abilities** — 37 files — Skill system: Weapon abilities. Types: WeaponAbilityBook, ArmorIgnore, ArmorPierce, Bladeweave, ....
- **Scripts/System/Skills/Weapon Abilities/Extra** — 25 files — Skill system: Extra. Types: AchillesStrike, ConsecratedStrike, DeathBlow, DevastatingBlow, ....
- **Scripts/System/Spawns** — 9 files — Spawners and spawn management.
- **Scripts/System/Tasks** — 5 files — Support system: Tasks. Types: TaskManager150Min, TaskManager200Min, TaskManager250Min, TaskManager60Min, ....
- **Scripts/Systems** — 0 files — No direct scripts; groups subfolders.
- **Scripts/Systems/Transmogrify** — 8 files — Transmogrify. Types: TransmogrifyCatalog, TransmogrifyFees, TransmogrifyGem, TransmogrifyGump, ....
- **Scripts/Timers** — 2 files — Timers. Types: GoodiesTimer, RepeatableAction.
- **Scripts/Utilities** — 9 files — Shared helpers, constants and UTC utilities.
- **Scripts/Utilities/Constants** — 1 file — Constants. Types: EmoteHues.
- **Core** — 51 files — RunUO/Memento core compiled into WorldLinux.exe.
- **Core/ContextMenus** — 4 files — Context menu primitives.
- **Core/Diagnostics** — 5 files — Diagnostics and logging.
- **Core/Gumps** — 23 files — Core gump primitives.
- **Core/Items** — 5 files — Core item/container primitives.
- **Core/Menus** — 3 files — Menu primitives.
- **Core/Network** — 14 files — Core packet definitions, handlers, NetState and encryption.
- **Core/Persistence** — 12 files — World/account/timer persistence.
- **Core/Properties** — 0 files — Property/attribute plumbing.
- **Core/Targeting** — 6 files — Targeting primitives.
- **Info scripts** — 3 files — Startup settings projects compiled separately from shard scripts.

## Non-source folders

| Path | Purpose |
|---|---|
| `World/Backups` | Timestamped .bak copies of edited source files; git-ignored. |
| `World/Backups/Automatic` | Automatic periodic backups. |
| `World/Backups/Crashed` | Copies captured after a crash. |
| `World/Data` | Runtime data root (client mul files, config, spawns, logs). |
| `World/Data/Decoration` | Decoration item data. |
| `World/Data/Decoration/Monopoly` | Monopoly board/deed decoration data by land. |
| `World/Data/Decoration/Monopoly/IslesOfDread` | Runtime data: isles of dread. |
| `World/Data/Decoration/Monopoly/Lodor` | Runtime data: lodor. |
| `World/Data/Decoration/Monopoly/Savage` | Runtime data: savage. |
| `World/Data/Decoration/Monopoly/Serpent` | Runtime data: serpent. |
| `World/Data/Decoration/Monopoly/Sosaria` | Runtime data: sosaria. |
| `World/Data/Decoration/Monopoly/Underworld` | Runtime data: underworld. |
| `World/Data/Files` | Client mul data: maps, statics, multi, Cliloc.enu. |
| `World/Data/Logs` | Server/runtime logs. |
| `World/Data/Logs/Commands` | Command audit logs by access level. |
| `World/Data/Logs/Commands/Owner` | Runtime data: owner. |
| `World/Data/RichVeins` | Rich vein harvesting data. |
| `World/Data/Spawns` | Spawn data files. |
| `World/Data/System` | Config and startup data. |
| `World/Data/System/Bulk Orders` | Bulk-order reward data (blacksmith, tailoring). |
| `World/Data/System/Bulk Orders/Blacksmith` | Runtime data: blacksmith. |
| `World/Data/System/Bulk Orders/Tailoring` | Runtime data: tailoring. |
| `World/Data/System/CFG` | Config files including `cliloc.cfg` (cliloc text/colour markup). |
| `World/Data/System/Components` | Component definitions used by systems. |
| `World/Data/System/XML` | XML settings/data used at startup. |
| `World/Documentation` | Prose/design documentation; not loaded by the server. |
| `World/Documentation/Development` | Developer notes/design docs. |
| `World/Documentation/Showcase` | Showcase material. |
| `World/Info` | Startup configuration and content data. |
| `World/Info/Articles` | Newspaper/article definitions. |
| `World/Info/Decorations` | Decoration definitions. |
| `World/Info/Spawns` | Spawn map definitions. |
| `World/Saves` | Serialized world state. |
| `World/Saves/Accounts` | accounts.xml login/character data. |
| `World/Saves/Achievements` | Achievement save data. |
| `World/Saves/ChatBeta8` | Chat system save data. |
| `World/Saves/Craft` | Crafting save data. |
| `World/Saves/Data` | Misc persisted data. |
| `World/Saves/Guilds` | Guild save data. |
| `World/Saves/Items` | World item serialization (Items.bin/idx/tdb). |
| `World/Saves/Mobiles` | World mobile serialization (Mobiles.bin/idx/tdb). |
| `World/Saves/Player` | Per-player persisted data. |
| `World/Saves/Transmogrify` | Transmogrify system save data. |
| `World/Source/Tools` | Build helper scripts (Linux mcs build). |

Timestamped snapshots inside `World/Backups/Automatic` are copies and are not listed individually.

## Script catalogs

Format: `File.cs` — **PrimaryType : Base** — purpose. `(+N types)` = extra types in the file. Item attributes appear in brackets.

### Scripts

#### Scripts/Engines and Systems — Gameplay engines and content systems (magic, quests, trades, champs, UI systems).

- `FastPlayer.cs` — **FastPlayer** — class (~7 public members).

#### Scripts/Engines and Systems/Achievments — Achievement definitions, tracking, gumps and rewards.

- `Achievement.cs` — **AchieveData** — Data holder.
- `AchievementEntry.cs` — **AchievementMenuEntry : ContextMenuEntry** — If you want to add this context entry you will need to add a cliloc in the 300**** range, or use an existin...
- `AchievementSystem.cs` — **AchievementSystem** — Commands: [achievement, [achievements, [feats (+1 type)
- `FeatsTarget.cs` — **FeatsTarget : Target** — Targeting prompt.

#### Scripts/Engines and Systems/Achievments/AchieveTypes — Achieve types. Types: BaseAchievement, DiscoverLandAchievement, DiscoveryAchievement, ExplicitAchievement, ....

- `BaseAchievement.cs` — **BaseAchievement** — Base Achievement Class.
- `DiscoverLandAchievement.cs` — **DiscoverLandAchievement : BaseAchievement** — class (~3 public members).
- `DiscoveryAchievement.cs` — **DiscoveryAchievement : BaseAchievement** — class (~3 public members).
- `ExplicitAchievement.cs` — **ExplicitAchievement : BaseAchievement** — class (~5 public members). (+1 type)
- `HarvestAchievement.cs` — **HarvestAchievement : BaseAchievement** — class (~2 public members).
- `HunterAchievement.cs` — **HunterAchievement : BaseAchievement** — class (~3 public members).
- `ItemCraftedAchievement.cs` — **ItemCraftedAchievement : BaseAchievement** — class (~2 public members).
- `SkillProgressAchievement.cs` — **SkillProgressAchievement : BaseAchievement** — class (~1 public members).

#### Scripts/Engines and Systems/Achievments/Gumps — Gumps. Types: AchievementGump, AchievementObtainedGump.

- `AchievementGump.cs` — **AchievementGump : Gump** — Gump UI.
- `AchievementObtainedGump.cs` — **AchievementObtainedGump : Gump** — Gump UI.

#### Scripts/Engines and Systems/Avatar — Avatar progression, coins, rewards, starter templates and safety-deposit items.

- `AvatarBook.cs` — **AvatarBook : Item** — Item ("The Avatar's Ascent"). [Constructable]
- `AvatarCommand.cs` — **AvatarCommand** — Commands: [avatar-draft-enable, [avatar-enable, [avatar-migrate--game-time, [avatar-shop (+1 type)
- `AvatarEngine.cs` — **AvatarEngine** — class (~10 public members).
- `AvatarStarterTemplates.cs` — **AvatarStarterTemplates** — Enum with about 19 members.
- `Constants.cs` — **Constants** — class (~30 public members).
- `DeathContext.cs` — **DeathContext** — Context/state holder. (+1 type)
- `PlayerContext.cs` — **PlayerContext** — Context/state holder. (+3 types)
- `PlayerContext_Draft.cs` — **PlayerContext** — Context/state holder.
- `SkillArchive.cs` — **SkillArchive** — class (~66 public members).

#### Scripts/Engines and Systems/Avatar/Core — Core. Types: CoinRewardCalculator, CoinRewardCalculatorLegacy, IAvatarOnlyItem.

- `CoinRewardCalculator.cs` — **CoinRewardCalculator** — class (~2 public members).
- `CoinRewardCalculatorLegacy.cs` — **CoinRewardCalculatorLegacy** — class (~2 public members).
- `IAvatarOnlyItem.cs` — **IAvatarOnlyItem** — Interface contract.

#### Scripts/Engines and Systems/Avatar/Items — Items. Types: SafetyDepositBox.

- `SafetyDepositBox.cs` — **SafetyDepositBox : Container** — Container ("safety deposit box").

#### Scripts/Engines and Systems/Avatar/Reward — Reward. Types: ActionReward, AvatarShopGump, RewardFactory, RewardFactory, ....

- `ActionReward.cs` — **ActionReward : IReward** — class (~16 public members).
- `AvatarShopGump.cs` — **AvatarShopGump : Gump** — Gump UI. (+2 types)
- `AvatarShopGumpRewards.cs` — **RewardFactory** — Factory/service.
- `AvatarShopGumpRewards_Ascensions.cs` — **RewardFactory** — Factory/service.
- `AvatarShopGumpRewards_Draft.cs` — **RewardFactory** — Factory/service.
- `AvatarShopGumpRewards_Items.cs` — **RewardFactory** — Factory/service.
- `AvatarShopGumpRewards_SkillArchive.cs` — **RewardFactory** — Factory/service.
- `AvatarShopGumpRewards_SkillBoost.cs` — **RewardFactory** — Factory/service.
- `AvatarShopGumpRewards_Templates.cs` — **RewardFactory** — Factory/service.
- `Categories.cs` — **Categories** — Enum with about 9 members.
- `IReward.cs` — **IReward** — Interface contract.
- `ItemReward.cs` — **ItemReward : IReward** — class (~15 public members).

#### Scripts/Engines and Systems/Champs — Champion spawn altars, bosses, minions and rewards.

- `ChampionAltar.cs` — **ChampionAltar : PentagramAddon** — class (~6 public members).
- `ChampionIdolTarget.cs` — **ChampionIdolTarget : Target** — Targeting prompt.
- `ChampionPlatform.cs` — **ChampionPlatform : BaseAddon** — House addon.
- `ChampionRewards.cs` — **ChampionRewards** — class (~6 public members).
- `ChampionSkull.cs` — **ChampionSkull : Item** — Item ("A skull of summoning"). [Constructable]
- `ChampionSkullGump.cs` — **ChampionSkullGump : Gump** — Gump UI. (+1 type)
- `ChampionSpawn.cs` — **ChampionSpawn : Item** — Item. [Constructable] (+2 types)
- `ChampionSpawnInfoGump.cs` — **ChampionSpawnInfoGump : Gump** — Gump UI. (+1 type)
- `ChampionSpawnType.cs` — **ChampionSpawnType** — Enum with about 6 members. (+1 type)
- `SliceTimer.cs` — **SliceTimer : Timer** — Timer.

#### Scripts/Engines and Systems/Champs/Items — Items. Types: UnholyBone.

- `UnholyBone.cs` — **UnholyBone : Item** — Item. [Constructable] (+1 type)

#### Scripts/Engines and Systems/Champs/Mobiles — Mobiles. Types: GreaterMongbat, SerpentineDragon.

- `GreaterMongbat.cs` — **GreaterMongbat : BaseCreature** — Creature ("a greater mongbat", AI_Melee, Closest). [Constructable, CorpseName]
- `SerpentineDragon.cs` — **SerpentineDragon : BaseCreature** — Creature ("a serpentine dragon", AI_Mage, Evil). [Constructable, CorpseName]

#### Scripts/Engines and Systems/Champs/Mobiles/Bosses — Bosses. Types: Barracoon, BaseChampion, LordOaks, Mephitis, ....

- `Barracoon.cs` — **Barracoon : BaseChampion** — class (~23 public members). [Constructable] (+1 type)
- `BaseChampion.cs` — **BaseChampion : BaseCreature** — Creature (Closest). (+1 type)
- `LordOaks.cs` — **LordOaks : BaseChampion** — class (~23 public members). [Constructable]
- `Mephitis.cs` — **Mephitis : BaseChampion** — class (~11 public members). [Constructable]
- `Neira.cs` — **Neira : BaseChampion** — class (~31 public members). [Constructable] (+3 types)
- `Rikktor.cs` — **Rikktor : BaseChampion** — class (~18 public members). [Constructable]
- `Semidar.cs` — **Semidar : BaseChampion** — class (~15 public members). [Constructable]
- `Silvani.cs` — **Silvani : BaseCreature** — Creature ("Silvani", AI_Mage, Evil). [Constructable]

#### Scripts/Engines and Systems/Jako Pets/Attributes — Attributes. Types: JakoAttributes, JakoBaseAttribute, JakoHitPointsAttribute, JakoManaPointsAttribute, ....

- `JakoAttributes.cs` — **JakoAttributes** — class (~17 public members). (+1 type)
- `JakoBaseAttribute.cs` — **JakoBaseAttribute : IComparable** — Abstract class (~18 public members).
- `JakoHitPointsAttribute.cs` — **JakoHitPointsAttribute : JakoBaseAttribute** — class (~7 public members).
- `JakoManaPointsAttribute.cs` — **JakoManaPointsAttribute : JakoBaseAttribute** — class (~6 public members).
- `JakoResistColdAttribute.cs` — **JakoResistColdAttribute : JakoBaseAttribute** — class (~6 public members).
- `JakoResistEnergyAttribute.cs` — **JakoResistEnergyAttribute : JakoBaseAttribute** — class (~6 public members).
- `JakoResistFireAttribute.cs` — **JakoResistFireAttribute : JakoBaseAttribute** — class (~6 public members).
- `JakoResistPhysicalAttribute.cs` — **JakoResistPhysicalAttribute : JakoBaseAttribute** — class (~6 public members).
- `JakoResistPoisonAttribute.cs` — **JakoResistPoisonAttribute : JakoBaseAttribute** — class (~6 public members).
- `JakoStaminaPointsAttribute.cs` — **JakoStaminaPointsAttribute : JakoBaseAttribute** — class (~6 public members).

#### Scripts/Engines and Systems/Magic/Bard — Magic/spell system: Bard. Types: BardSongProvider, SongBook, SongBookGump, CastSongSpells, ....

- `BardSongProvider.cs` — **BardSongProvider** — Provider/service.
- `SongBook.cs` — **SongBook : Spellbook** — class (~12 public members). [Constructable, Flipable]
- `SongBookGump.cs` — **SongBookGump : Gump** — Gump UI.
- `SongCommandList.cs` — **CastSongSpells** — Commands: [Spells_Bard
- `SongSpells.cs` — **BardFunctions** — Abstract class (~18 public members). (+1 type)

#### Scripts/Engines and Systems/Magic/Bard/Scrolls — Magic/spell system: Scrolls. Types: ArmysPaeonScroll, EnchantingEtudeScroll, EnergyCarolScroll, EnergyThrenodyScroll, ....

- `ArmysPaeon.cs` — **ArmysPaeonScroll : SpellScroll** — class (~8 public members). [Constructable]
- `EnchantingEtude.cs` — **EnchantingEtudeScroll : SpellScroll** — class (~8 public members). [Constructable]
- `EnergyCarol.cs` — **EnergyCarolScroll : SpellScroll** — class (~8 public members). [Constructable]
- `EnergyThrenody.cs` — **EnergyThrenodyScroll : SpellScroll** — class (~8 public members). [Constructable]
- `FireCarol.cs` — **FireCarolScroll : SpellScroll** — class (~8 public members). [Constructable]
- `FireThrenody.cs` — **FireThrenodyScroll : SpellScroll** — class (~8 public members). [Constructable]
- `FoeRequiem.cs` — **FoeRequiemScroll : SpellScroll** — class (~8 public members). [Constructable]
- `IceCarol.cs` — **IceCarolScroll : SpellScroll** — class (~8 public members). [Constructable]
- `IceThrenody.cs` — **IceThrenodyScroll : SpellScroll** — class (~8 public members). [Constructable]
- `KnightsMinne.cs` — **KnightsMinneScroll : SpellScroll** — class (~8 public members). [Constructable]
- `MagesBallad.cs` — **MagesBalladScroll : SpellScroll** — class (~8 public members). [Constructable]
- `MagicFinale.cs` — **MagicFinaleScroll : SpellScroll** — class (~8 public members). [Constructable]
- `PoisonCarol.cs` — **PoisonCarolScroll : SpellScroll** — class (~8 public members). [Constructable]
- `PoisonThrenody.cs` — **PoisonThrenodyScroll : SpellScroll** — class (~8 public members). [Constructable]
- `SheepfoeMambo.cs` — **SheepfoeMamboScroll : SpellScroll** — class (~8 public members). [Constructable]
- `SinewyEtude.cs` — **SinewyEtudeScroll : SpellScroll** — class (~8 public members). [Constructable]

#### Scripts/Engines and Systems/Magic/Bard/Spells — Magic/spell system: Spells. Types: ArmysPaeonSong, EnchantingEtudeSong, EnergyCarolSong, EnergyThrenodySong, ....

- `ArmysPaeonSong.cs` — **ArmysPaeonSong : Song** — class (~9 public members). (+1 type)
- `EnchantingEtudeSong.cs` — **EnchantingEtudeSong : Song** — class (~8 public members). (+1 type)
- `EnergyCarolSong.cs` — **EnergyCarolSong : Song** — class (~8 public members). (+1 type)
- `EnergyThrenodySong.cs` — **EnergyThrenodySong : Song** — class (~11 public members). (+2 types)
- `FireCarolSong.cs` — **FireCarolSong : Song** — class (~8 public members). (+1 type)
- `FireThrenodySong.cs` — **FireThrenodySong : Song** — class (~11 public members). (+2 types)
- `FoeRequiemSong.cs` — **FoeRequiemSong : Song** — class (~14 public members). (+1 type)
- `IceCarolSong.cs` — **IceCarolSong : Song** — class (~8 public members). (+1 type)
- `IceThrenodySong.cs` — **IceThrenodySong : Song** — class (~11 public members). (+2 types)
- `KnightsMinneSong.cs` — **KnightsMinneSong : Song** — class (~8 public members). (+1 type)
- `MagesBalladSong.cs` — **MagesBalladSong : Song** — class (~9 public members). (+1 type)
- `MagicFinaleSong.cs` — **MagicFinaleSong : Song** — class (~7 public members).
- `PoisonCarolSong.cs` — **PoisonCarolSong : Song** — class (~8 public members). (+1 type)
- `PoisonThrenodySong.cs` — **PoisonThrenodySong : Song** — class (~11 public members). (+2 types)
- `SheepfoeMamboSong.cs` — **SheepfoeMamboSong : Song** — class (~8 public members). (+1 type)
- `SinewyEtudeSong.cs` — **SinewyEtudeSong : Song** — class (~8 public members). (+1 type)

#### Scripts/Engines and Systems/Magic/Base — Magic/spell system: Base. Types: CastSpellCommand, DisturbType, ForgetfulGem, Initializer, ....

- `CastSpellCommand.cs` — **CastSpellCommand** — Command implementation.
- `DisturbType.cs` — **DisturbType** — Enum with about 6 members.
- `ForgetfulGem.cs` — **ForgetfulGem : Item** — Item ("magical crystal"). [Constructable] (+1 type)
- `Initializer.cs` — **Initializer** — class (~3 public members).
- `Reagent.cs` — **Reagent** — class (~48 public members).
- `Runebook.cs` — **Runebook : Item** — Item. [Constructable] (+1 type)
- `SpecialMove.cs` — **SpecialMove** — Abstract class (~36 public members). (+2 types)
- `Spell.cs` — **Spell : ISpell** — Abstract class (~88 public members). (+3 types)
- `SpellCircle.cs` — **SpellCircle** — Enum with about 8 members.
- `SpellDefinition.cs` — **SpellDefinition** — class (~10 public members).
- `SpellHelper.cs` — **SpellHelper** — Helper utilities. (+9 types)
- `SpellInfo.cs` — **SpellInfo** — Info/data container.
- `SpellItemInfo.cs` — **SpellItemInfo** — Info/data container. (+1 type)
- `SpellRegistry.cs` — **SpellRegistry** — class (~11 public members).
- `SpellState.cs` — **SpellState** — Enum with about 5 members.
- `UnsummonTimer.cs` — **UnsummonTimer : Timer** — Timer.

#### Scripts/Engines and Systems/Magic/Bushido — Magic/spell system: Bushido. Types: BookOfBushido, Confidence, CounterAttack, Evasion, ....

- `BookOfBushido.cs` — **BookOfBushido : Spellbook** — class (~10 public members). [Constructable]
- `Confidence.cs` — **Confidence : SamuraiSpell** — class (~15 public members). (+2 types)
- `CounterAttack.cs` — **CounterAttack : SamuraiSpell** — class (~12 public members). (+1 type)
- `Evasion.cs` — **Evasion : SamuraiSpell** — class (~16 public members). (+1 type)
- `HonorableExecution.cs` — **HonorableExecution : SamuraiMove** — class (~21 public members). (+1 type)
- `LightningStrike.cs` — **LightningStrike : SamuraiMove** — class (~14 public members).
- `MomentumStrike.cs` — **MomentumStrike : SamuraiMove** — class (~7 public members).
- `SamuraiMove.cs` — **SamuraiMove : SpecialMove** — class (~3 public members).
- `SamuraiSpell.cs` — **SamuraiSpell : Spell** — Spell.

#### Scripts/Engines and Systems/Magic/Death Knight — Magic/spell system: Death knight. Types: DeathKnightCommands, DeathKnightSpell, DeathKnightSpellbook, DeathKnightSpellbookGump, ....

- `DeathKnightCommands.cs` — **DeathKnightCommands** — Commands: [Spells_DeathKnight
- `DeathKnightSpell.cs` — **DeathKnightSpell : Spell** — Spell.
- `DeathKnightSpellBook.cs` — **DeathKnightSpellbook : Spellbook** — class (~14 public members). [Constructable, Flipable]
- `DeathKnightSpellBookGump.cs` — **DeathKnightSpellbookGump : Gump** — Gump UI.
- `DeathKnightSpellProvider.cs` — **DeathKnightSpellProvider** — Provider/service.
- `DeathSkulls.cs` — **DeathKnightSkull750 : SpellScroll** — //////////////////////////////////////////////////////////////////////////////////////////// [Constructable] (+13 types)
- `DevilPact.cs` — **DevilPact : BaseCreature** — Creature (AI_Melee, Closest). [Constructable, CorpseName]
- `SoulLantern.cs` — **SoulLantern : TrinketLantern** — class (~11 public members). [Constructable]

#### Scripts/Engines and Systems/Magic/Death Knight/Spells — Magic/spell system: Spells. Types: BanishSpell, DemonicTouchSpell, DevilPactSpell, GrimReaperSpell, ....

- `BanishSpell.cs` — **BanishSpell : DeathKnightSpell** — class (~10 public members). (+1 type)
- `DemonicTouchSpell.cs` — **DemonicTouchSpell : DeathKnightSpell** — class (~10 public members). (+1 type)
- `DevilPactSpell.cs` — **DevilPactSpell : DeathKnightSpell** — class (~11 public members). (+1 type)
- `GrimReaperSpell.cs` — **GrimReaperSpell : DeathKnightSpell** — class (~9 public members).
- `HagHandSpell.cs` — **HagHandSpell : DeathKnightSpell** — class (~11 public members). (+1 type)
- `HellfireSpell.cs` — **HellfireSpell : DeathKnightSpell** — class (~15 public members). (+2 types)
- `LucifersBoltSpell.cs` — **LucifersBoltSpell : DeathKnightSpell** — class (~11 public members). (+1 type)
- `OrbOfOrcusSpell.cs` — **OrbOfOrcusSpell : DeathKnightSpell** — class (~9 public members).
- `ShieldOfHateSpell.cs` — **ShieldOfHateSpell : DeathKnightSpell** — class (~14 public members). (+2 types)
- `SoulReaperSpell.cs` — **SoulReaperSpell : DeathKnightSpell** — class (~13 public members). (+2 types)
- `StrengthOfSteelSpell.cs` — **StrengthOfSteelSpell : DeathKnightSpell** — class (~10 public members). (+1 type)
- `StrikeSpell.cs` — **StrikeSpell : DeathKnightSpell** — class (~10 public members). (+1 type)
- `SuccubusSkinSpell.cs` — **SuccubusSkinSpell : DeathKnightSpell** — class (~13 public members). (+2 types)
- `WrathSpell.cs` — **WrathSpell : DeathKnightSpell** — class (~11 public members). (+1 type)

#### Scripts/Engines and Systems/Magic/Druidism — Magic/spell system: Druidism. Types: BookDruidBrewing, DruidPouch, ShieldOfEarthPotion, HerbalistSpell.

- `BookDruidBrewing.cs` — **BookDruidBrewing : Item** — Item ("Druidic Herbalism"). [Constructable] (+1 type)
- `DruidPouch.cs` — **DruidPouch : Bag** — class (~60 public members). [Constructable] (+3 types)
- `HerbalistPotions.cs` — **ShieldOfEarthPotion : SpellScroll** — /////////////////////////////////////////////////////////////////////////////////////////////////////// [Constructable] (+15 types)
- `HerbalistSpell.cs` — **HerbalistSpell : Spell** — Spell.

#### Scripts/Engines and Systems/Magic/Druidism/Effects — Magic/spell system: Effects. Types: BlendWithForestSpell, DruidicRuneSpell, FireflySpell, GraspingRootsSpell, ....

- `BlendWithForrestSpell.cs` — **BlendWithForestSpell : HerbalistSpell** — class (~18 public members). (+2 types)
- `DruidicRuneSpell.cs` — **DruidicRuneSpell : HerbalistSpell** — class (~11 public members). (+1 type)
- `FireflySpell.cs` — **FireflySpell : HerbalistSpell** — class (~13 public members). (+1 type)
- `GraspingRootsSpell.cs` — **GraspingRootsSpell : HerbalistSpell** — class (~16 public members). (+3 types)
- `HerbalHealingSpell.cs` — **HerbalHealingSpell : HerbalistSpell** — class (~10 public members). (+1 type)
- `LureStoneSpell.cs` — **LureStoneSpell : HerbalistSpell** — class (~34 public members). (+6 types)
- `MushroomGatewaySpell.cs` — **MushroomGatewaySpell : HerbalistSpell** — class (~17 public members). (+3 types)
- `NaturesPassageSpell.cs` — **NaturesPassageSpell : HerbalistSpell** — class (~12 public members). (+1 type)
- `ProtectiveFairySpell.cs` — **ProtectiveFairySpell : HerbalistSpell** — class (~16 public members). [Constructable, CorpseName] (+1 type)
- `RestorativeSoilSpell.cs` — **RestorativeSoilSpell : HerbalistSpell** — class (~11 public members). (+1 type)
- `ShieldOfEarthSpell.cs` — **ShieldOfEarthSpell : HerbalistSpell** — class (~17 public members). (+3 types)
- `StoneCircleSpell.cs` — **StoneCircleSpell : HerbalistSpell** — class (~18 public members). (+3 types)
- `SwarmofInsectsSpell.cs` — **SwarmOfInsectsSpell : HerbalistSpell** — class (~14 public members). (+2 types)
- `TreefellowSpell.cs` — **TreefellowSpell : HerbalistSpell** — class (~24 public members). [Constructable, CorpseName] (+2 types)
- `VolcanicEruption.cs` — **VolcanicEruptionSpell : HerbalistSpell** — class (~10 public members). (+1 type)
- `WoodlandProtectionSpell.cs` — **WoodlandProtectionSpell : HerbalistSpell** — class (~11 public members). (+1 type)

#### Scripts/Engines and Systems/Magic/Elementalism — Magic/spell system: Elementalism. Types: ElementalCommands, ElementalEffect, ElementalExit, Elemental_Armor_Scroll, ....

- `ElementalCommands.cs` — **ElementalCommands** — Casts Elemental Armor
- `ElementalEffect.cs` — **ElementalEffect : Item** — Item. [Constructable] (+1 type)
- `ElementalExit.cs` — **ElementalExit : Item** — Item ("lyceum exit"). [Constructable]
- `ElementalScrolls.cs` — **Elemental_Armor_Scroll : SpellScroll** — class (~224 public members). [Constructable] (+31 types)
- `ElementalShrine.cs` — **ElementalShrine : Item** — Item ("shrine"). [Constructable]
- `ElementalSpell.cs` — **ElementalSpell : Spell** — Spell ("water").
- `ElementalSpellbook.cs` — **ElementalSpellbook : Spellbook** — class (~17 public members). [Constructable]
- `ElementalSpellbookGump.cs` — **ElementalSpellbookGump : Gump** — Gump UI. (+1 type)

#### Scripts/Engines and Systems/Magic/Elementalism/Mobiles — Magic/spell system: Mobiles. Types: ElementalCalledAir, ElementalCalledEarth, ElementalCalledFire, ElementalCalledWater, ....

- `ElementalCalledAir.cs` — **ElementalCalledAir : BaseCreature** — Creature ("an air elemental", AI_Mage, Closest). [Constructable, CorpseName]
- `ElementalCalledEarth.cs` — **ElementalCalledEarth : BaseCreature** — Creature ("an earth elemental", AI_Mage, Closest). [Constructable, CorpseName]
- `ElementalCalledFire.cs` — **ElementalCalledFire : BaseCreature** — Creature ("a fire elemental", AI_Mage, Closest). [Constructable, CorpseName]
- `ElementalCalledWater.cs` — **ElementalCalledWater : BaseCreature** — Creature ("a water elemental", AI_Mage, Closest). [Constructable, CorpseName]
- `ElementalFiendAir.cs` — **ElementalFiendAir : BaseCreature** — Creature ("a starlight fiend", AI_Melee, Closest). [Constructable, CorpseName]
- `ElementalFiendEarth.cs` — **ElementalFiendEarth : BaseCreature** — Creature ("a corpser fiend", AI_Melee, Closest). [Constructable, CorpseName]
- `ElementalFiendFire.cs` — **ElementalFiendFire : BaseCreature** — Creature ("a lava fiend", AI_Melee, Closest). [Constructable, CorpseName]
- `ElementalFiendWater.cs` — **ElementalFiendWater : BaseCreature** — Creature ("a watery fiend", AI_Melee, Closest). [Constructable, CorpseName]
- `ElementalLordAir.cs` — **ElementalLordAir : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName]
- `ElementalLordEarth.cs` — **ElementalLordEarth : BaseCreature** — Creature (AI_Melee, Closest). [Constructable, CorpseName]
- `ElementalLordFire.cs` — **ElementalLordFire : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName]
- `ElementalLordWater.cs` — **ElementalLordWater : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName]
- `ElementalSpiritAir.cs` — **ElementalSpiritAir : BaseCreature** — Creature ("a cloud spirit", AI_Melee, Closest). [Constructable, CorpseName]
- `ElementalSpiritEarth.cs` — **ElementalSpiritEarth : BaseCreature** — Creature ("an earth spirit", AI_Melee, Closest). [Constructable, CorpseName]
- `ElementalSpiritFire.cs` — **ElementalSpiritFire : BaseCreature** — Creature ("a lava spirit", AI_Melee, Closest). [Constructable, CorpseName]
- `ElementalSpiritWater.cs` — **ElementalSpiritWater : BaseCreature** — Creature ("a water spirit", AI_Melee, Closest). [Constructable, CorpseName]
- `ElementalSteed.cs` — **ElementalSteed : BaseMount** — Mount. [Constructable]
- `ElementalSummonEnt.cs` — **ElementalSummonEnt : BaseCreature** — Creature ("an ent", AI_Melee, Closest). [Constructable, CorpseName]
- `ElementalSummonIce.cs` — **ElementalSummonIce : BaseCreature** — Creature ("an ice elemental", AI_Melee, Closest). [Constructable, CorpseName]
- `ElementalSummonLightning.cs` — **ElementalSummonLightning : BaseCreature** — Creature ("a lightning elemental", AI_Melee, Closest). [Constructable, CorpseName]
- `ElementalSummonMagma.cs` — **ElementalSummonMagma : BaseCreature** — Creature ("a magma elemental", AI_Melee, Closest). [Constructable, CorpseName]

#### Scripts/Engines and Systems/Magic/Elementalism/Sphere 1 — Magic/spell system: Sphere 1. Types: Elemental_Armor_Spell, Elemental_Bolt_Spell, Elemental_Mend_Spell, Elemental_Sanctuary_Spell.

- `Elemental_Armor.cs` — **Elemental_Armor_Spell : ElementalSpell** — class (~6 public members).
- `Elemental_Bolt.cs` — **Elemental_Bolt_Spell : ElementalSpell** — class (~8 public members). (+1 type)
- `Elemental_Mend.cs` — **Elemental_Mend_Spell : ElementalSpell** — class (~7 public members). (+1 type)
- `Elemental_Sanctuary.cs` — **Elemental_Sanctuary_Spell : ElementalSpell** — class (~5 public members).

#### Scripts/Engines and Systems/Magic/Elementalism/Sphere 2 — Magic/spell system: Sphere 2. Types: Elemental_Pain_Spell, Elemental_Protection_Spell, Elemental_Purge_Spell, Elemental_Steed_Spell.

- `Elemental_Pain.cs` — **Elemental_Pain_Spell : ElementalSpell** — class (~7 public members). (+1 type)
- `Elemental_Protection.cs` — **Elemental_Protection_Spell : ElementalSpell** — class (~8 public members).
- `Elemental_Purge.cs` — **Elemental_Purge_Spell : ElementalSpell** — class (~7 public members). (+1 type)
- `Elemental_Steed.cs` — **Elemental_Steed_Spell : ElementalSpell** — class (~6 public members).

#### Scripts/Engines and Systems/Magic/Elementalism/Sphere 3 — Magic/spell system: Sphere 3. Types: Elemental_Call_Spell, Elemental_Force_Spell, Elemental_Wall_Spell, Elemental_Warp_Spell.

- `Elemental_Call.cs` — **Elemental_Call_Spell : ElementalSpell** — class (~6 public members).
- `Elemental_Force.cs` — **Elemental_Force_Spell : ElementalSpell** — class (~7 public members). (+1 type)
- `Elemental_Wall.cs` — **Elemental_Wall_Spell : ElementalSpell** — class (~14 public members). (+3 types)
- `Elemental_Warp.cs` — **Elemental_Warp_Spell : ElementalSpell** — class (~8 public members). (+1 type)

#### Scripts/Engines and Systems/Magic/Elementalism/Sphere 4 — Magic/spell system: Sphere 4. Types: Elemental_Field_Spell, Elemental_Restoration_Spell, Elemental_Strike_Spell, Elemental_Void_Spell.

- `Elemental_Field.cs` — **Elemental_Field_Spell : ElementalSpell** — class (~16 public members). (+3 types)
- `Elemental_Restoration.cs` — **Elemental_Restoration_Spell : ElementalSpell** — class (~7 public members). (+1 type)
- `Elemental_Strike.cs` — **Elemental_Strike_Spell : ElementalSpell** — class (~7 public members). (+1 type)
- `Elemental_Void.cs` — **Elemental_Void_Spell : ElementalSpell** — class (~9 public members). (+1 type)

#### Scripts/Engines and Systems/Magic/Elementalism/Sphere 5 — Magic/spell system: Sphere 5. Types: Elemental_Blast_Spell, Elemental_Echo_Spell, Elemental_Fiend_Spell, Elemental_Hold_Spell.

- `Elemental_Blast.cs` — **Elemental_Blast_Spell : ElementalSpell** — class (~7 public members). (+1 type)
- `Elemental_Echo.cs` — **Elemental_Echo_Spell : ElementalSpell** — class (~5 public members).
- `Elemental_Fiend.cs` — **Elemental_Fiend_Spell : ElementalSpell** — class (~8 public members). (+1 type)
- `Elemental_Hold.cs` — **Elemental_Hold_Spell : ElementalSpell** — class (~7 public members). (+1 type)

#### Scripts/Engines and Systems/Magic/Elementalism/Sphere 6 — Magic/spell system: Sphere 6. Types: Elemental_Barrage_Spell, Elemental_Rune_Spell, Elemental_Storm_Spell, Elemental_Summon_Spell.

- `Elemental_Barrage.cs` — **Elemental_Barrage_Spell : ElementalSpell** — class (~7 public members). (+1 type)
- `Elemental_Rune.cs` — **Elemental_Rune_Spell : ElementalSpell** — class (~7 public members). (+1 type)
- `Elemental_Storm.cs` — **Elemental_Storm_Spell : ElementalSpell** — class (~9 public members). (+2 types)
- `Elemental_Summon.cs` — **Elemental_Summon_Spell : ElementalSpell** — class (~6 public members).

#### Scripts/Engines and Systems/Magic/Elementalism/Sphere 7 — Magic/spell system: Sphere 7. Types: Elemental_Devastation_Spell, Elemental_Fall_Spell, Elemental_Gate_Spell, Elemental_Havoc_Spell.

- `Elemental_Devastation.cs` — **Elemental_Devastation_Spell : ElementalSpell** — class (~7 public members). (+1 type)
- `Elemental_Fall.cs` — **Elemental_Fall_Spell : ElementalSpell** — class (~7 public members). (+1 type)
- `Elemental_Gate.cs` — **Elemental_Gate_Spell : ElementalSpell** — class (~14 public members). (+3 types)
- `Elemental_Havoc.cs` — **Elemental_Havoc_Spell : ElementalSpell** — class (~7 public members). (+1 type)

#### Scripts/Engines and Systems/Magic/Elementalism/Sphere 8 — Magic/spell system: Sphere 8. Types: Elemental_Apocalypse_Spell, Elemental_Lord_Spell, Elemental_Soul_Spell, Elemental_Spirit_Spell.

- `Elemental_Apocalypse.cs` — **Elemental_Apocalypse_Spell : ElementalSpell** — class (~5 public members).
- `Elemental_Lord.cs` — **Elemental_Lord_Spell : ElementalSpell** — class (~6 public members).
- `Elemental_Soul.cs` — **Elemental_Soul_Spell : ElementalSpell** — class (~7 public members). (+1 type)
- `Elemental_Spirit.cs` — **Elemental_Spirit_Spell : ElementalSpell** — class (~7 public members). (+1 type)

#### Scripts/Engines and Systems/Magic/Holy Man — Magic/spell system: Holy man. Types: HolyManCommands, HolyManSpell, HolyManSpellbook, HolyManSpellbookGump, ....

- `HolyManCommands.cs` — **HolyManCommands** — Commands: [Spells_HolyMan
- `HolyManSpell.cs` — **HolyManSpell : Spell** — Spell.
- `HolyManSpellBook.cs` — **HolyManSpellbook : Spellbook** — class (~14 public members). [Constructable, Flipable]
- `HolyManSpellBookGump.cs` — **HolyManSpellbookGump : Gump** — Gump UI.
- `HolyManSpellProvider.cs` — **HolyManSpellProvider** — Provider/service.
- `HolySymbol.cs` — **HolySymbol : TrinketTalisman** — class (~12 public members). [Constructable]
- `HolySymbols.cs` — **HolyManSymbol770 : SpellScroll** — //////////////////////////////////////////////////////////////////////////////////////////// [Constructable] (+13 types)
- `MalletStake.cs` — **MalletStake : Item** — Item ("wooden mallet and stake"). [Constructable] (+2 types)

#### Scripts/Engines and Systems/Magic/Holy Man/Spells — Magic/spell system: Spells. Types: BanishEvilSpell, DampenSpiritSpell, EnchantSpell, HammerOfFaithSpell, ....

- `BanishEvilSpell.cs` — **BanishEvilSpell : HolyManSpell** — class (~11 public members). (+2 types)
- `DampenSpiritSpell.cs` — **DampenSpiritSpell : HolyManSpell** — class (~10 public members). (+1 type)
- `EnchantSpell.cs` — **EnchantSpell : HolyManSpell** — class (~34 public members). (+4 types)
- `HammerOfFaithSpell.cs` — **HammerOfFaithSpell : HolyManSpell** — class (~20 public members). [Constructable] (+2 types)
- `HeavenlyLightSpell.cs` — **HeavenlyLightSpell : HolyManSpell** — class (~9 public members). (+1 type)
- `NourishSpell.cs` — **NourishSpell : HolyManSpell** — class (~11 public members). (+1 type)
- `PurgeSpell.cs` — **PurgeSpell : HolyManSpell** — class (~10 public members). (+1 type)
- `RebirthSpell.cs` — **RebirthSpell : HolyManSpell** — class (~11 public members). (+1 type)
- `SacredBoonSpell.cs` — **SacredBoonSpell : HolyManSpell** — class (~13 public members). (+2 types)
- `SanctifySpell.cs` — **SanctifySpell : HolyManSpell** — class (~12 public members). (+1 type)
- `SeanceSpell.cs` — **SeanceSpell : HolyManSpell** — class (~11 public members). (+1 type)
- `SmiteSpell.cs` — **SmiteSpell : HolyManSpell** — class (~10 public members). (+1 type)
- `TouchOfLifeSpell.cs` — **TouchOfLifeSpell : HolyManSpell** — class (~10 public members). (+1 type)
- `TrialByFireSpell.cs` — **TrialByFireSpell : HolyManSpell** — class (~9 public members).

#### Scripts/Engines and Systems/Magic/Jedi — Magic/spell system: Jedi. Types: CastJediSpells, JediDatacron01, JediSpell, JediSpellbookGump, ....

- `JediCommandList.cs` — **CastJediSpells** — Casts Force Grip
- `JediDatacrons.cs` — **JediDatacron01 : SpellScroll** — //////////////////////////////////////////////////////////////////////////////////////////// [Constructable] (+9 types)
- `JediSpell.cs` — **JediSpell : Spell** — Spell.
- `JediSpellBookGump.cs` — **JediSpellbookGump : Gump** — Gump UI.
- `JediSpellbook.cs` — **JediSpellbook : Spellbook** — class (~32 public members). [Constructable] (+2 types)
- `KaranCrystal.cs` — **KaranCrystal : BaseReagent** — class (~6 public members). [Constructable]

#### Scripts/Engines and Systems/Magic/Jedi/Spells — Magic/spell system: Spells. Types: Celerity, Deflection, ForceGrip, MindsEye, ....

- `Celerity.cs` — **Celerity : JediSpell** — class (~13 public members).
- `Deflection.cs` — **Deflection : JediSpell** — class (~11 public members).
- `ForceGrip.cs` — **ForceGrip : JediSpell** — class (~15 public members). (+1 type)
- `MindsEye.cs` — **MindsEye : JediSpell** — class (~12 public members). (+1 type)
- `Mirage.cs` — **Mirage : JediSpell** — class (~23 public members). (+1 type)
- `PsychicAura.cs` — **PsychicAura : JediSpell** — class (~11 public members).
- `Replicate.cs` — **Replicate : JediSpell** — class (~10 public members).
- `SoothingTouch.cs` — **SoothingTouch : JediSpell** — class (~13 public members). (+1 type)
- `StasisField.cs` — **StasisField : JediSpell** — class (~13 public members). (+1 type)
- `ThrowSabre.cs` — **ThrowSabre : JediSpell** — class (~15 public members). (+1 type)

#### Scripts/Engines and Systems/Magic/Jester — Magic/spell system: Jester. Types: BagOfTricks, CastJesterSpells, JesterSpell, SummonedJoke, ....

- `BagOfTricks.cs` — **BagOfTricks : Item** — Item ("bag of tricks"). [Constructable] (+6 types)
- `JesterCommandList.cs` — **CastJesterSpells** — Casts Can Of Nuts
- `JesterSpell.cs` — **JesterSpell : Spell** — Spell.
- `SummonedJoke.cs` — **SummonedJoke : BaseCreature** — Creature ("a prank", AI_Melee, Closest). [Constructable, CorpseName]
- `SummonedPrank.cs` — **SummonedPrank : BaseCreature** — Creature ("a prank", AI_Melee, Closest). [Constructable]

#### Scripts/Engines and Systems/Magic/Jester/Spells — Magic/spell system: Spells. Types: CanOfSnakes, Clowns, FlowerPower, Hilarity, ....

- `CanOfSnakes.cs` — **CanOfSnakes : JesterSpell** — class (~7 public members).
- `Clowns.cs` — **Clowns : JesterSpell** — class (~23 public members). (+2 types)
- `FlowerPower.cs` — **FlowerPower : JesterSpell** — class (~9 public members). (+1 type)
- `Hilarity.cs` — **Hilarity : JesterSpell** — class (~11 public members). (+1 type)
- `Insult.cs` — **Insult : JesterSpell** — class (~12 public members). (+2 types)
- `JumpAround.cs` — **JumpAround : JesterSpell** — class (~10 public members). (+1 type)
- `PoppingBalloon.cs` — **PoppingBalloon : JesterSpell** — class (~7 public members).
- `RabbitInAHat.cs` — **RabbitInAHat : JesterSpell** — class (~7 public members).
- `SeltzerBottle.cs` — **SeltzerBottle : JesterSpell** — class (~9 public members). (+1 type)
- `SurpriseGift.cs` — **SurpriseGift : JesterSpell** — class (~7 public members).

#### Scripts/Engines and Systems/Magic/Knight — Magic/spell system: Knight. Types: BookOfChivalry, CleanseByFireSpell, CloseWoundsSpell, ConsecrateWeaponSpell, ....

- `BookOfChivalry.cs` — **BookOfChivalry : Spellbook** — class (~10 public members). [Constructable]
- `CleanseByFire.cs` — **CleanseByFireSpell : PaladinSpell** — class (~11 public members). (+1 type)
- `CloseWounds.cs` — **CloseWoundsSpell : PaladinSpell** — class (~11 public members). (+1 type)
- `ConsecrateWeapon.cs` — **ConsecrateWeaponSpell : PaladinSpell** — class (~15 public members). (+1 type)
- `DispelEvil.cs` — **DispelEvilSpell : PaladinSpell** — class (~12 public members).
- `DivineFury.cs` — **DivineFurySpell : PaladinSpell** — class (~11 public members).
- `EnemyOfOne.cs` — **EnemyOfOneSpell : PaladinSpell** — class (~10 public members).
- `HolyLight.cs` — **HolyLightSpell : PaladinSpell** — class (~11 public members).
- `NobleSacrifice.cs` — **NobleSacrificeSpell : PaladinSpell** — class (~10 public members).
- `PaladinSpell.cs` — **PaladinSpell : Spell** — Spell.
- `RemoveCurse.cs` — **RemoveCurseSpell : PaladinSpell** — class (~13 public members). (+1 type)
- `SacredJourney.cs` — **SacredJourneySpell : PaladinSpell** — class (~14 public members). (+1 type)

#### Scripts/Engines and Systems/Magic/Magery — Magic/spell system: Magery. Types: MagerySpell, Spellbook.

- `MagerySpell.cs` — **MagerySpell : Spell** — Spell.
- `Spellbook.cs` — **Spellbook : Item** — Commands: [AllSpells (+1 type)

#### Scripts/Engines and Systems/Magic/Magery/Scrolls — Magic/spell system: Scrolls. Types: SpellScroll.

- `SpellScroll.cs` — **SpellScroll : Item** — Item. [Constructable]

#### Scripts/Engines and Systems/Magic/Magery/Scrolls/Eighth Circle — Magic/spell system: Eighth circle. Types: EarthquakeScroll, EnergyVortexScroll, ResurrectionScroll, SummonAirElementalScroll, ....

- `EarthquakeScroll.cs` — **EarthquakeScroll : SpellScroll** — class (~6 public members). [Constructable]
- `EnergyVortexScroll.cs` — **EnergyVortexScroll : SpellScroll** — class (~6 public members). [Constructable]
- `ResurrectionScroll.cs` — **ResurrectionScroll : SpellScroll** — class (~6 public members). [Constructable]
- `SummonAirElementalScroll.cs` — **SummonAirElementalScroll : SpellScroll** — class (~6 public members). [Constructable]
- `SummonDaemonScroll.cs` — **SummonDaemonScroll : SpellScroll** — class (~6 public members). [Constructable]
- `SummonEarthElementalScroll.cs` — **SummonEarthElementalScroll : SpellScroll** — class (~6 public members). [Constructable]
- `SummonFireElementalScroll.cs` — **SummonFireElementalScroll : SpellScroll** — class (~6 public members). [Constructable]
- `SummonWaterElementalScroll.cs` — **SummonWaterElementalScroll : SpellScroll** — class (~6 public members). [Constructable]

#### Scripts/Engines and Systems/Magic/Magery/Scrolls/Fifth Circle — Magic/spell system: Fifth circle. Types: BladeSpiritsScroll, DispelFieldScroll, IncognitoScroll, MagicReflectScroll, ....

- `BladeSpiritsScroll.cs` — **BladeSpiritsScroll : SpellScroll** — class (~6 public members). [Constructable]
- `DispelFieldScroll.cs` — **DispelFieldScroll : SpellScroll** — class (~6 public members). [Constructable]
- `IncognitoScroll.cs` — **IncognitoScroll : SpellScroll** — class (~6 public members). [Constructable]
- `MagicReflectScroll.cs` — **MagicReflectScroll : SpellScroll** — class (~6 public members). [Constructable]
- `MindBlastScroll.cs` — **MindBlastScroll : SpellScroll** — class (~6 public members). [Constructable]
- `ParalyzeScroll.cs` — **ParalyzeScroll : SpellScroll** — class (~6 public members). [Constructable]
- `PoisonFieldScroll.cs` — **PoisonFieldScroll : SpellScroll** — class (~6 public members). [Constructable]
- `SummonCreatureScroll.cs` — **SummonCreatureScroll : SpellScroll** — class (~6 public members). [Constructable]

#### Scripts/Engines and Systems/Magic/Magery/Scrolls/First Circle — Magic/spell system: First circle. Types: ClumsyScroll, CreateFoodScroll, FeeblemindScroll, HealScroll, ....

- `ClumsyScroll.cs` — **ClumsyScroll : SpellScroll** — class (~6 public members). [Constructable]
- `CreateFoodScroll.cs` — **CreateFoodScroll : SpellScroll** — class (~6 public members). [Constructable]
- `FeeblemindScroll.cs` — **FeeblemindScroll : SpellScroll** — class (~6 public members). [Constructable]
- `HealScroll.cs` — **HealScroll : SpellScroll** — class (~6 public members). [Constructable]
- `MagicArrowScroll.cs` — **MagicArrowScroll : SpellScroll** — class (~6 public members). [Constructable]
- `NightSightScroll.cs` — **NightSightScroll : SpellScroll** — class (~6 public members). [Constructable]
- `ReactiveArmorScroll.cs` — **ReactiveArmorScroll : SpellScroll** — class (~6 public members). [Constructable]
- `WeakenScroll.cs` — **WeakenScroll : SpellScroll** — class (~6 public members). [Constructable]

#### Scripts/Engines and Systems/Magic/Magery/Scrolls/Fourth Circle — Magic/spell system: Fourth circle. Types: ArchProtectionScroll, ArchCureScroll, CurseScroll, FireFieldScroll, ....

- `ArchProtectionScroll.cs` — **ArchProtectionScroll : SpellScroll** — class (~6 public members). [Constructable]
- `ArchcureScroll.cs` — **ArchCureScroll : SpellScroll** — class (~6 public members). [Constructable]
- `CurseScroll.cs` — **CurseScroll : SpellScroll** — class (~6 public members). [Constructable]
- `FireFieldScroll.cs` — **FireFieldScroll : SpellScroll** — class (~6 public members). [Constructable]
- `GreaterHealScroll.cs` — **GreaterHealScroll : SpellScroll** — class (~6 public members). [Constructable]
- `LightningScroll.cs` — **LightningScroll : SpellScroll** — class (~6 public members). [Constructable]
- `ManaDrainScroll.cs` — **ManaDrainScroll : SpellScroll** — class (~6 public members). [Constructable]
- `RecallScroll.cs` — **RecallScroll : SpellScroll** — class (~6 public members). [Constructable]

#### Scripts/Engines and Systems/Magic/Magery/Scrolls/Second Circle — Magic/spell system: Second circle. Types: AgilityScroll, CunningScroll, CureScroll, HarmScroll, ....

- `AgilityScroll.cs` — **AgilityScroll : SpellScroll** — class (~6 public members). [Constructable]
- `CunningScroll.cs` — **CunningScroll : SpellScroll** — class (~6 public members). [Constructable]
- `CureScroll.cs` — **CureScroll : SpellScroll** — class (~6 public members). [Constructable]
- `HarmScroll.cs` — **HarmScroll : SpellScroll** — class (~6 public members). [Constructable]
- `MagicTrapScroll.cs` — **MagicTrapScroll : SpellScroll** — class (~6 public members). [Constructable]
- `MagicUnTrapScroll.cs` — **MagicUnTrapScroll : SpellScroll** — class (~6 public members). [Constructable]
- `ProtectionScroll.cs` — **ProtectionScroll : SpellScroll** — class (~6 public members). [Constructable]
- `StrengthScroll.cs` — **StrengthScroll : SpellScroll** — class (~6 public members). [Constructable]

#### Scripts/Engines and Systems/Magic/Magery/Scrolls/Seventh Circle — Magic/spell system: Seventh circle. Types: ChainLightningScroll, EnergyFieldScroll, FlamestrikeScroll, GateTravelScroll, ....

- `ChainLightningScroll.cs` — **ChainLightningScroll : SpellScroll** — class (~6 public members). [Constructable]
- `EnergyFieldScroll.cs` — **EnergyFieldScroll : SpellScroll** — class (~6 public members). [Constructable]
- `FlamestrikeScroll.cs` — **FlamestrikeScroll : SpellScroll** — class (~6 public members). [Constructable]
- `GateTravelScroll.cs` — **GateTravelScroll : SpellScroll** — class (~6 public members). [Constructable]
- `ManaVampireScroll.cs` — **ManaVampireScroll : SpellScroll** — class (~6 public members). [Constructable]
- `MassDispelScroll.cs` — **MassDispelScroll : SpellScroll** — class (~6 public members). [Constructable]
- `MeteorStormScroll.cs` — **MeteorSwarmScroll : SpellScroll** — class (~6 public members). [Constructable]
- `PolymorphScroll.cs` — **PolymorphScroll : SpellScroll** — class (~6 public members). [Constructable]

#### Scripts/Engines and Systems/Magic/Magery/Scrolls/Sixth Circle — Magic/spell system: Sixth circle. Types: DispelScroll, EnergyBoltScroll, ExplosionScroll, InvisibilityScroll, ....

- `DispelScroll.cs` — **DispelScroll : SpellScroll** — class (~6 public members). [Constructable]
- `EnergyBoltScroll.cs` — **EnergyBoltScroll : SpellScroll** — class (~6 public members). [Constructable]
- `ExplosionScroll.cs` — **ExplosionScroll : SpellScroll** — class (~6 public members). [Constructable]
- `InvisibilityScroll.cs` — **InvisibilityScroll : SpellScroll** — class (~6 public members). [Constructable]
- `MarkScroll.cs` — **MarkScroll : SpellScroll** — class (~6 public members). [Constructable]
- `MassCurseScroll.cs` — **MassCurseScroll : SpellScroll** — class (~6 public members). [Constructable]
- `ParalyzeFieldSpell.cs` — **ParalyzeFieldScroll : SpellScroll** — class (~6 public members). [Constructable]
- `RevealScroll.cs` — **RevealScroll : SpellScroll** — class (~6 public members). [Constructable]

#### Scripts/Engines and Systems/Magic/Magery/Scrolls/Third Circle — Magic/spell system: Third circle. Types: BlessScroll, FireballScroll, MagicLockScroll, PoisonScroll, ....

- `BlessScroll.cs` — **BlessScroll : SpellScroll** — class (~6 public members). [Constructable]
- `FireballScroll.cs` — **FireballScroll : SpellScroll** — class (~6 public members). [Constructable]
- `MagicLockScroll.cs` — **MagicLockScroll : SpellScroll** — class (~6 public members). [Constructable]
- `PoisonScroll.cs` — **PoisonScroll : SpellScroll** — class (~6 public members). [Constructable]
- `TelekinisisScroll.cs` — **TelekinisisScroll : SpellScroll** — class (~6 public members). [Constructable]
- `TeleportScroll.cs` — **TeleportScroll : SpellScroll** — class (~6 public members). [Constructable]
- `UnlockScroll.cs` — **UnlockScroll : SpellScroll** — class (~6 public members). [Constructable]
- `WallOfStoneScroll.cs` — **WallOfStoneScroll : SpellScroll** — class (~6 public members). [Constructable]

#### Scripts/Engines and Systems/Magic/Magery/Spells/Magery 1st — Magic/spell system: Magery 1st. Types: ClumsySpell, CreateFoodSpell, FeeblemindSpell, HealSpell, ....

- `Clumsy.cs` — **ClumsySpell : MagerySpell** — class (~6 public members). (+1 type)
- `CreateFood.cs` — **CreateFoodSpell : MagerySpell** — class (~9 public members). (+1 type)
- `Feeblemind.cs` — **FeeblemindSpell : MagerySpell** — class (~6 public members). (+1 type)
- `Heal.cs` — **HealSpell : MagerySpell** — class (~7 public members). (+1 type)
- `MagicArrow.cs` — **MagicArrowSpell : MagerySpell** — class (~8 public members). (+1 type)
- `NightSight.cs` — **NightSightSpell : MagerySpell** — class (~5 public members). (+1 type)
- `ReactiveArmor.cs` — **ReactiveArmorSpell : MagerySpell** — class (~6 public members).
- `Weaken.cs` — **WeakenSpell : MagerySpell** — class (~7 public members). (+1 type)

#### Scripts/Engines and Systems/Magic/Magery/Spells/Magery 2nd — Magic/spell system: Magery 2nd. Types: AgilitySpell, CunningSpell, CureSpell, HarmSpell, ....

- `Agility.cs` — **AgilitySpell : MagerySpell** — class (~6 public members). (+1 type)
- `Cunning.cs` — **CunningSpell : MagerySpell** — class (~6 public members). (+1 type)
- `Cure.cs` — **CureSpell : MagerySpell** — class (~7 public members). (+1 type)
- `Harm.cs` — **HarmSpell : MagerySpell** — class (~8 public members). (+1 type)
- `MagicTrap.cs` — **MagicTrapSpell : MagerySpell** — class (~7 public members). (+1 type)
- `Protection.cs` — **ProtectionSpell : MagerySpell** — class (~8 public members).
- `RemoveTrap.cs` — **RemoveTrapSpell : MagerySpell** — class (~6 public members). (+1 type)
- `Strength.cs` — **StrengthSpell : MagerySpell** — class (~6 public members). (+1 type)

#### Scripts/Engines and Systems/Magic/Magery/Spells/Magery 3rd — Magic/spell system: Magery 3rd. Types: BlessSpell, FireballSpell, MagicLockSpell, PoisonSpell, ....

- `Bless.cs` — **BlessSpell : MagerySpell** — class (~6 public members). (+1 type)
- `Fireball.cs` — **FireballSpell : MagerySpell** — class (~7 public members). (+1 type)
- `MagicLock.cs` — **MagicLockSpell : MagerySpell** — class (~116 public members). (+5 types)
- `Poison.cs` — **PoisonSpell : MagerySpell** — class (~6 public members). (+1 type)
- `Telekinesis.cs` — **TelekinesisSpell : MagerySpell** — class (~10 public members). (+2 types)
- `Teleport.cs` — **TeleportSpell : MagerySpell** — class (~8 public members). (+1 type)
- `Unlock.cs` — **UnlockSpell : MagerySpell** — class (~5 public members). (+1 type)
- `WallOfStone.cs` — **WallOfStoneSpell : MagerySpell** — class (~14 public members). (+3 types)

#### Scripts/Engines and Systems/Magic/Magery/Spells/Magery 4th — Magic/spell system: Magery 4th. Types: ArchCureSpell, ArchProtectionSpell, CurseSpell, FireFieldSpell, ....

- `ArchCure.cs` — **ArchCureSpell : MagerySpell** — class (~7 public members). (+1 type)
- `ArchProtection.cs` — **ArchProtectionSpell : MagerySpell** — class (~6 public members). (+1 type)
- `Curse.cs` — **CurseSpell : MagerySpell** — class (~8 public members). (+1 type)
- `FireField.cs` — **FireFieldSpell : MagerySpell** — class (~16 public members). (+3 types)
- `GreaterHeal.cs` — **GreaterHealSpell : MagerySpell** — class (~7 public members). (+1 type)
- `Lightning.cs` — **LightningSpell : MagerySpell** — class (~7 public members). (+1 type)
- `ManaDrain.cs` — **ManaDrainSpell : MagerySpell** — class (~7 public members). (+1 type)
- `Recall.cs` — **RecallSpell : MagerySpell** — class (~9 public members). (+1 type)

#### Scripts/Engines and Systems/Magic/Magery/Spells/Magery 5th — Magic/spell system: Magery 5th. Types: BladeSpiritsSpell, DispelFieldSpell, IncognitoSpell, MagicReflectSpell, ....

- `BladeSpirits.cs` — **BladeSpiritsSpell : MagerySpell** — class (~8 public members). (+1 type)
- `DispelField.cs` — **DispelFieldSpell : MagerySpell** — class (~6 public members). (+1 type)
- `Incognito.cs` — **IncognitoSpell : MagerySpell** — class (~7 public members). (+1 type)
- `MagicReflect.cs` — **MagicReflectSpell : MagerySpell** — class (~8 public members).
- `MindBlast.cs` — **MindBlastSpell : MagerySpell** — class (~7 public members). (+1 type)
- `Paralyze.cs` — **ParalyzeSpell : MagerySpell** — class (~7 public members). (+1 type)
- `PoisonField.cs` — **PoisonFieldSpell : MagerySpell** — class (~16 public members). (+3 types)
- `SummonCreature.cs` — **SummonCreatureSpell : MagerySpell** — class (~7 public members).

#### Scripts/Engines and Systems/Magic/Magery/Spells/Magery 6th — Magic/spell system: Magery 6th. Types: DispelSpell, EnergyBoltSpell, ExplosionSpell, InvisibilitySpell, ....

- `Dispel.cs` — **DispelSpell : MagerySpell** — class (~6 public members). (+1 type)
- `EnergyBolt.cs` — **EnergyBoltSpell : MagerySpell** — class (~7 public members). (+1 type)
- `Explosion.cs` — **ExplosionSpell : MagerySpell** — class (~9 public members). (+2 types)
- `Invisibility.cs` — **InvisibilitySpell : MagerySpell** — class (~10 public members). (+2 types)
- `Mark.cs` — **MarkSpell : MagerySpell** — class (~7 public members). (+1 type)
- `MassCurse.cs` — **MassCurseSpell : MagerySpell** — class (~6 public members). (+1 type)
- `ParalyzeField.cs` — **ParalyzeFieldSpell : MagerySpell** — class (~15 public members). (+3 types)
- `Reveal.cs` — **RevealSpell : MagerySpell** — class (~6 public members). (+1 type)

#### Scripts/Engines and Systems/Magic/Magery/Spells/Magery 7th — Magic/spell system: Magery 7th. Types: ChainLightningSpell, EnergyFieldSpell, FlameStrikeSpell, GateTravelSpell, ....

- `ChainLightning.cs` — **ChainLightningSpell : MagerySpell** — class (~7 public members). (+1 type)
- `EnergyField.cs` — **EnergyFieldSpell : MagerySpell** — class (~14 public members). (+3 types)
- `FlameStrike.cs` — **FlameStrikeSpell : MagerySpell** — class (~7 public members). (+1 type)
- `GateTravel.cs` — **GateTravelSpell : MagerySpell** — class (~14 public members). (+3 types)
- `ManaVampire.cs` — **ManaVampireSpell : MagerySpell** — class (~7 public members). (+1 type)
- `MassDispel.cs` — **MassDispelSpell : MagerySpell** — class (~6 public members). (+1 type)
- `MeteorSwarm.cs` — **MeteorSwarmSpell : MagerySpell** — class (~7 public members). (+1 type)
- `Polymorph.cs` — **PolymorphSpell : MagerySpell** — class (~8 public members). (+1 type)

#### Scripts/Engines and Systems/Magic/Magery/Spells/Magery 8th — Magic/spell system: Magery 8th. Types: AirElementalSpell, EarthElementalSpell, EarthquakeSpell, EnergyVortexSpell, ....

- `AirElemental.cs` — **AirElementalSpell : MagerySpell** — class (~5 public members).
- `EarthElemental.cs` — **EarthElementalSpell : MagerySpell** — class (~5 public members).
- `Earthquake.cs` — **EarthquakeSpell : MagerySpell** — class (~5 public members).
- `EnergyVortex.cs` — **EnergyVortexSpell : MagerySpell** — class (~7 public members). (+1 type)
- `FireElemental.cs` — **FireElementalSpell : MagerySpell** — class (~5 public members).
- `Resurrection.cs` — **ResurrectionSpell : MagerySpell** — class (~7 public members). (+1 type)
- `SummonDaemon.cs` — **SummonDaemonSpell : MagerySpell** — class (~5 public members).
- `WaterElemental.cs` — **WaterElementalSpell : MagerySpell** — class (~5 public members).

#### Scripts/Engines and Systems/Magic/Misc — Magic/spell system: Misc. Types: AttackSpells, IdentifySpell, MagicRuneBag, MagicalSpell, ....

- `AttackSpells.cs` — **AttackSpells : MagicalSpell** — class (~13 public members). (+1 type)
- `IdentifySpell.cs` — **IdentifySpell : MagicalSpell** — class (~9 public members). (+1 type)
- `MagicRuneBag.cs` — **MagicRuneBag : Item** — Item. [Constructable] (+28 types)
- `MagicalSpell.cs` — **MagicalSpell : Spell** — Spell.
- `SummonDragonSpell.cs` — **SummonDragonSpell : MagicalSpell** — class (~22 public members). [Constructable, CorpseName] (+2 types)
- `SummonSkeleton.cs` — **SummonSkeleton : BaseCreature** — Creature ("a skeleton", AI_Melee, Closest). [Constructable, CorpseName] (+1 type)
- `SummonSnakesSpell.cs` — **SummonSnakesSpell : MagicalSpell** — class (~21 public members). [Constructable, CorpseName] (+2 types)
- `ThorLightningSpell.cs` — **ThorLightningSpell : MagicalSpell** — class (~10 public members). (+1 type)
- `TravelSpell.cs` — **TravelSpell : MagicalSpell** — class (~14 public members). (+1 type)

#### Scripts/Engines and Systems/Magic/Mystic — Magic/spell system: Mystic. Types: CastMonkSpells, MysticMonkRobe, MysticPack, MysticSpell, ....

- `MysticCommandList.cs` — **CastMonkSpells** — Casts AstralProjection
- `MysticMonkRobe.cs` — **MysticMonkRobe : BaseGiftOuterTorso** — class (~8 public members). [Constructable]
- `MysticPack.cs` — **MysticPack : LargeSack** — class (~13 public members). [Constructable, Flipable]
- `MysticSpell.cs` — **MysticSpell : Spell** — Spell.
- `MysticSpellBook.cs` — **MysticSpellbook : Spellbook** — class (~200 public members). [Constructable, Flipable]
- `MysticSpellBookGump.cs` — **MysticSpellbookGump : Gump** — Gump UI.

#### Scripts/Engines and Systems/Magic/Mystic/Scrolls — Magic/spell system: Scrolls. Types: AstralProjectionScroll, AstralTravelScroll, CreateRobeScroll, GentleTouchScroll, ....

- `AstralProjectionScroll.cs` — **AstralProjectionScroll : SpellScroll** — class (~11 public members). [Constructable]
- `AstralTravelScroll.cs` — **AstralTravelScroll : SpellScroll** — class (~11 public members). [Constructable]
- `CreateRobeScroll.cs` — **CreateRobeScroll : SpellScroll** — class (~11 public members). [Constructable]
- `GentleTouchScroll.cs` — **GentleTouchScroll : SpellScroll** — class (~11 public members). [Constructable]
- `LeapScroll.cs` — **LeapScroll : SpellScroll** — class (~11 public members). [Constructable]
- `PsionicBlastScroll.cs` — **PsionicBlastScroll : SpellScroll** — class (~11 public members). [Constructable]
- `PsychicWallScroll.cs` — **PsychicWallScroll : SpellScroll** — class (~11 public members). [Constructable]
- `PurityOfBodyScroll.cs` — **PurityOfBodyScroll : SpellScroll** — class (~11 public members). [Constructable]
- `QuiveringPalmScroll.cs` — **QuiveringPalmScroll : SpellScroll** — class (~11 public members). [Constructable]
- `WindRunnerScroll.cs` — **WindRunnerScroll : SpellScroll** — class (~11 public members). [Constructable]

#### Scripts/Engines and Systems/Magic/Mystic/Spells — Magic/spell system: Spells. Types: AstralProjection, AstralTravel, CreateRobe, GentleTouch, ....

- `AstralProjection.cs` — **AstralProjection : MysticSpell** — class (~11 public members). (+1 type)
- `AstralTravel.cs` — **AstralTravel : MysticSpell** — class (~13 public members). (+1 type)
- `CreateRobe.cs` — **CreateRobe : MysticSpell** — class (~8 public members).
- `GentleTouch.cs` — **GentleTouch : MysticSpell** — class (~11 public members). (+1 type)
- `Leap.cs` — **Leap : MysticSpell** — class (~12 public members). (+1 type)
- `PsionicBlast.cs` — **PsionicBlast : MysticSpell** — class (~12 public members). (+1 type)
- `PsychicWall.cs` — **PsychicWall : MysticSpell** — class (~9 public members).
- `PurityOfBody.cs` — **PurityOfBody : MysticSpell** — class (~8 public members).
- `QuiveringPalm.cs` — **QuiveringPalm : MysticSpell** — class (~10 public members). (+1 type)
- `WindRunner.cs` — **WindRunner : MysticSpell** — class (~11 public members).

#### Scripts/Engines and Systems/Magic/Necromancy — Magic/spell system: Necromancy. Types: AnimateDeadSpell, BloodOathSpell, CorpseSkinSpell, CurseWeaponSpell, ....

- `AnimateDeadSpell.cs` — **AnimateDeadSpell : NecromancerSpell** — class (~8 public members). (+1 type)
- `BloodOathSpell.cs` — **BloodOathSpell : NecromancerSpell** — class (~12 public members). (+2 types)
- `CorpseSkin.cs` — **CorpseSkinSpell : NecromancerSpell** — class (~11 public members). (+2 types)
- `CurseWeapon.cs` — **CurseWeaponSpell : NecromancerSpell** — class (~8 public members). (+2 types)
- `EvilOmen.cs` — **EvilOmenSpell : NecromancerSpell** — class (~9 public members). (+1 type)
- `Exorcism.cs` — **ExorcismSpell : NecromancerSpell** — class (~10 public members). (+2 types)
- `HorrificBeast.cs` — **HorrificBeastSpell : TransformationSpell** — class (~9 public members).
- `LichForm.cs` — **LichFormSpell : TransformationSpell** — class (~13 public members).
- `MindRot.cs` — **MindRotSpell : NecromancerSpell** — class (~20 public members). (+3 types)
- `NecromancerSpell.cs` — **NecromancerSpell : Spell** — Spell.
- `NecromancerSpellbook.cs` — **NecromancerSpellbook : Spellbook** — class (~11 public members). [Constructable]
- `PainSpike.cs` — **PainSpikeSpell : NecromancerSpell** — class (~10 public members). (+2 types)
- `PoisonStrike.cs` — **PoisonStrikeSpell : NecromancerSpell** — class (~9 public members). (+1 type)
- `Strangle.cs` — **StrangleSpell : NecromancerSpell** — class (~10 public members). (+2 types)
- `SummonFamiliar.cs` — **SummonFamiliarSpell : NecromancerSpell** — class (~18 public members). (+2 types)
- `TransformationSpell.cs` — **TransformationSpell : NecromancerSpell** — Abstract class (~16 public members).
- `VampiricEmbrace.cs` — **VampiricEmbraceSpell : TransformationSpell** — class (~10 public members).
- `VengefulSpirit.cs` — **VengefulSpiritSpell : NecromancerSpell** — class (~9 public members). (+1 type)
- `Wither.cs` — **WitherSpell : NecromancerSpell** — class (~7 public members).
- `WraithForm.cs` — **WraithFormSpell : TransformationSpell** — class (~15 public members).

#### Scripts/Engines and Systems/Magic/Necromancy/Scrolls — Magic/spell system: Scrolls. Types: AnimateDeadScroll, BloodOathScroll, CorpseSkinScroll, CurseWeaponScroll, ....

- `AnimateDeadScroll.cs` — **AnimateDeadScroll : SpellScroll** — class (~7 public members). [Constructable]
- `BloodOathScroll.cs` — **BloodOathScroll : SpellScroll** — class (~7 public members). [Constructable]
- `CorpseSkinScroll.cs` — **CorpseSkinScroll : SpellScroll** — class (~7 public members). [Constructable]
- `CurseWeaponScroll.cs` — **CurseWeaponScroll : SpellScroll** — class (~7 public members). [Constructable]
- `EvilOmenScroll.cs` — **EvilOmenScroll : SpellScroll** — class (~7 public members). [Constructable]
- `ExorcismScroll.cs` — **ExorcismScroll : SpellScroll** — class (~7 public members). [Constructable]
- `HorrificBeastScroll.cs` — **HorrificBeastScroll : SpellScroll** — class (~7 public members). [Constructable]
- `LichFormScroll.cs` — **LichFormScroll : SpellScroll** — class (~7 public members). [Constructable]
- `MindRotScroll.cs` — **MindRotScroll : SpellScroll** — class (~7 public members). [Constructable]
- `PainSpikeScroll.cs` — **PainSpikeScroll : SpellScroll** — class (~7 public members). [Constructable]
- `PoisonStrikeScroll.cs` — **PoisonStrikeScroll : SpellScroll** — class (~7 public members). [Constructable]
- `StrangleScroll.cs` — **StrangleScroll : SpellScroll** — class (~7 public members). [Constructable]
- `SummonFamiliarScroll.cs` — **SummonFamiliarScroll : SpellScroll** — class (~7 public members). [Constructable]
- `VampiricEmbraceScroll.cs` — **VampiricEmbraceScroll : SpellScroll** — class (~7 public members). [Constructable]
- `VengefulSpiritScroll.cs` — **VengefulSpiritScroll : SpellScroll** — class (~7 public members). [Constructable]
- `WitherScroll.cs` — **WitherScroll : SpellScroll** — class (~7 public members). [Constructable]
- `WraithFormScroll.cs` — **WraithFormScroll : SpellScroll** — class (~7 public members). [Constructable]

#### Scripts/Engines and Systems/Magic/Ninjitsu — Magic/spell system: Ninjitsu. Types: AnimalForm, Backstab, BookOfNinjitsu, DeathStrike, ....

- `AnimalForm.cs` — **AnimalForm : NinjaSpell** — class (~49 public members). (+5 types)
- `Backstab.cs` — **Backstab : NinjaMove** — class (~12 public members).
- `BookOfNinjitsu.cs` — **BookOfNinjitsu : Spellbook** — class (~10 public members). [Constructable]
- `DeathStrike.cs` — **DeathStrike : NinjaMove** — class (~15 public members). (+1 type)
- `FocusAttack.cs` — **FocusAttack : NinjaMove** — class (~10 public members).
- `KiAttack.cs` — **KiAttack : NinjaMove** — class (~15 public members). (+1 type)
- `MirrorImage.cs` — **MirrorImage : NinjaSpell** — class (~28 public members). (+2 types)
- `NinjaMove.cs` — **NinjaMove : SpecialMove** — class (~3 public members).
- `NinjaSpell.cs` — **NinjaSpell : Spell** — Spell.
- `ShadowJump.cs` — **Shadowjump : NinjaSpell** — class (~11 public members). (+1 type)
- `SurpriseAttack.cs` — **SurpriseAttack : NinjaMove** — class (~15 public members). (+1 type)

#### Scripts/Engines and Systems/Magic/Research — Magic/spell system: Research. Types: AncientSpellbook, AncientSpellbookGump, ResearchBag, ResearchCommands, ....

- `AncientSpellBook.cs` — **AncientSpellbook : Spellbook** — class (~20 public members). [Constructable, Flipable]
- `AncientSpellBookGump.cs` — **AncientSpellbookGump : Gump** — Gump UI.
- `ResearchBag.cs` — **ResearchBag : Item** — Item ("research pack"). [Constructable, Flipable] (+2 types)
- `ResearchCommands.cs` — **ResearchCommands** — Casts the Ancient Researched Spell
- `ResearchFunctions.cs` — **ResearchSettings** — Settings holder. (+1 type)
- `ResearchSpell.cs` — **ResearchSpell : Spell** — Spell.

#### Scripts/Engines and Systems/Magic/Research/Spells/Conjuration — Magic/spell system: Conjuration. Types: ResearchAerialServant, ResearchClone, ResearchConjure, ResearchCreateGold, ....

- `ResearchAerialServant.cs` — **ResearchAerialServant : ResearchSpell** — class (~32 public members). [Constructable, CorpseName] (+1 type)
- `ResearchClone.cs` — **ResearchClone : ResearchSpell** — class (~14 public members).
- `ResearchConjure.cs` — **ResearchConjure : ResearchSpell** — class (~10 public members).
- `ResearchCreateGold.cs` — **ResearchCreateGold : ResearchSpell** — class (~12 public members). (+1 type)
- `ResearchDeathVortex.cs` — **ResearchDeathVortex : ResearchSpell** — class (~12 public members). (+1 type)
- `ResearchExtinguish.cs` — **ResearchExtinguish : ResearchSpell** — class (~12 public members). (+1 type)
- `ResearchMagicSteed.cs` — **ResearchMagicSteed : ResearchSpell** — class (~10 public members).
- `ResearchSwarm.cs` — **ResearchSwarm : ResearchSpell** — class (~12 public members). (+1 type)

#### Scripts/Engines and Systems/Magic/Research/Spells/Death — Magic/spell system: Death. Types: ResearchCreateGolem, ResearchDeathSpeak, ResearchGrantPeace, ResearchMaskofDeath, ....

- `ResearchCreateGolem.cs` — **ResearchCreateGolem : ResearchSpell** — class (~10 public members).
- `ResearchDeathSpeak.cs` — **ResearchDeathSpeak : ResearchSpell** — class (~9 public members).
- `ResearchGrantPeace.cs` — **ResearchGrantPeace : ResearchSpell** — class (~12 public members). (+2 types)
- `ResearchMaskofDeath.cs` — **ResearchMaskofDeath : ResearchSpell** — class (~23 public members). [Constructable] (+2 types)
- `ResearchOpenGround.cs` — **ResearchOpenGround : ResearchSpell** — class (~23 public members). (+2 types)
- `ResearchRockFlesh.cs` — **ResearchRockFlesh : ResearchSpell** — class (~14 public members). (+1 type)
- `ResearchSummonDead.cs` — **ResearchSummonDead : ResearchSpell** — class (~10 public members).
- `ResearchWithstandDeath.cs` — **ResearchWithstandDeath : ResearchSpell** — class (~22 public members). [Constructable] (+3 types)

#### Scripts/Engines and Systems/Magic/Research/Spells/Enchanting — Magic/spell system: Enchanting. Types: ResearchCauseFear, ResearchCharm, ResearchEnchant, ResearchMassMight, ....

- `ResearchCauseFear.cs` — **ResearchCauseFear : ResearchSpell** — class (~12 public members). (+1 type)
- `ResearchCharm.cs` — **ResearchCharm : ResearchSpell** — class (~14 public members). (+2 types)
- `ResearchEnchant.cs` — **ResearchEnchant : ResearchSpell** — class (~32 public members). (+4 types)
- `ResearchMassMight.cs` — **ResearchMassMight : ResearchSpell** — class (~9 public members).
- `ResearchMassSleep.cs` — **ResearchMassSleep : ResearchSpell** — class (~15 public members). (+2 types)
- `ResearchSleep.cs` — **ResearchSleep : ResearchSpell** — class (~14 public members). (+2 types)
- `ResearchSleepField.cs` — **ResearchSleepField : ResearchSpell** — class (~22 public members). (+4 types)
- `ResearchSneak.cs` — **ResearchSneak : ResearchSpell** — class (~15 public members). (+2 types)

#### Scripts/Engines and Systems/Magic/Research/Spells/Sorcery — Magic/spell system: Sorcery. Types: ResearchConflagration, ResearchCreateFire, ResearchEndureCold, ResearchEndureHeat, ....

- `ResearchConflagration.cs` — **ResearchConflagration : ResearchSpell** — class (~21 public members). (+3 types)
- `ResearchCreateFire.cs` — **ResearchCreateFire : ResearchSpell** — class (~26 public members). [Constructable] (+2 types)
- `ResearchEndureCold.cs` — **ResearchEndureCold : ResearchSpell** — class (~11 public members). (+1 type)
- `ResearchEndureHeat.cs` — **ResearchEndureHeat : ResearchSpell** — class (~11 public members). (+1 type)
- `ResearchExplosion.cs` — **ResearchExplosion : ResearchSpell** — class (~12 public members). (+1 type)
- `ResearchFlameBolt.cs` — **ResearchFlameBolt : ResearchSpell** — class (~13 public members). (+1 type)
- `ResearchIgnite.cs` — **ResearchIgnite : ResearchSpell** — class (~12 public members). (+1 type)
- `ResearchRingofFire.cs` — **ResearchRingofFire : ResearchSpell** — class (~22 public members). (+3 types)

#### Scripts/Engines and Systems/Magic/Research/Spells/Summoning — Magic/spell system: Summoning. Types: ResearchSummonAcidElemental, ResearchSummonBloodElemental, ResearchSummonElectricalElemental, ResearchSummonGemElemental, ....

- `ResearchSummonAcidElemental.cs` — **ResearchSummonAcidElemental : ResearchSpell** — class (~10 public members).
- `ResearchSummonBloodElemental.cs` — **ResearchSummonBloodElemental : ResearchSpell** — class (~10 public members).
- `ResearchSummonElectricalElemental.cs` — **ResearchSummonElectricalElemental : ResearchSpell** — class (~10 public members).
- `ResearchSummonGemElemental.cs` — **ResearchSummonGemElemental : ResearchSpell** — class (~10 public members).
- `ResearchSummonIceElemental.cs` — **ResearchSummonIceElemental : ResearchSpell** — class (~10 public members).
- `ResearchSummonMudElemental.cs` — **ResearchSummonMudElemental : ResearchSpell** — class (~10 public members).
- `ResearchSummonPoisonElemental.cs` — **ResearchSummonPoisonElemental : ResearchSpell** — class (~10 public members).
- `ResearchSummonWeedElemental.cs` — **ResearchSummonWeedElemental : ResearchSpell** — class (~10 public members).

#### Scripts/Engines and Systems/Magic/Research/Spells/Thaumaturgy — Magic/spell system: Thaumaturgy. Types: ResearchBanishDaemon, ResearchCallDestruction, ResearchConfusionBlast, ResearchDevastation, ....

- `ResearchBanishDaemon.cs` — **ResearchBanishDaemon : ResearchSpell** — class (~12 public members). (+2 types)
- `ResearchCallDestruction.cs` — **ResearchCallDestruction : ResearchSpell** — class (~13 public members). (+1 type)
- `ResearchConfusionBlast.cs` — **ResearchConfusionBlast : ResearchSpell** — class (~12 public members). (+1 type)
- `ResearchDevastation.cs` — **ResearchDevastation : ResearchSpell** — class (~14 public members). (+2 types)
- `ResearchEtherealTravel.cs` — **ResearchEtherealTravel : ResearchSpell** — class (~14 public members). (+1 type)
- `ResearchMeteorShower.cs` — **ResearchMeteorShower : ResearchSpell** — class (~12 public members). (+1 type)
- `ResearchSummonCreature.cs` — **ResearchSummonCreature : ResearchSpell** — class (~10 public members).
- `ResearchSummonDevil.cs` — **ResearchSummonDevil : ResearchSpell** — class (~10 public members).

#### Scripts/Engines and Systems/Magic/Research/Spells/Theurgy — Magic/spell system: Theurgy. Types: ResearchAirWalk, ResearchDivination, ResearchFadefromSight, ResearchHealingTouch, ....

- `ResearchAirWalk.cs` — **ResearchAirWalk : ResearchSpell** — class (~12 public members). (+1 type)
- `ResearchDivination.cs` — **ResearchDivination : ResearchSpell** — class (~11 public members). (+1 type)
- `ResearchFadefromSight.cs` — **ResearchFadefromSight : ResearchSpell** — class (~12 public members). (+1 type)
- `ResearchHealingTouch.cs` — **ResearchHealingTouch : ResearchSpell** — class (~12 public members). (+1 type)
- `ResearchIntervention.cs` — **ResearchIntervention : ResearchSpell** — class (~11 public members). (+1 type)
- `ResearchRestoration.cs` — **ResearchRestoration : ResearchSpell** — class (~12 public members). (+1 type)
- `ResearchSeeTruth.cs` — **ResearchSeeTruth : ResearchSpell** — class (~10 public members). (+1 type)
- `ResearchWizardEye.cs` — **ResearchWizardEye : ResearchSpell** — class (~10 public members). (+1 type)

#### Scripts/Engines and Systems/Magic/Research/Spells/Wizardry — Magic/spell system: Wizardry. Types: ResearchAvalanche, ResearchFrostField, ResearchFrostStrike, ResearchGasCloud, ....

- `ResearchAvalanche.cs` — **ResearchAvalanche : ResearchSpell** — class (~12 public members). (+1 type)
- `ResearchFrostField.cs` — **ResearchFrostField : ResearchSpell** — class (~21 public members). (+3 types)
- `ResearchFrostStrike.cs` — **ResearchFrostStrike : ResearchSpell** — class (~11 public members). (+1 type)
- `ResearchGasCloud.cs` — **ResearchGasCloud : ResearchSpell** — class (~12 public members). (+1 type)
- `ResearchHailStorm.cs` — **ResearchHailStorm : ResearchSpell** — class (~14 public members). (+2 types)
- `ResearchIcicle.cs` — **ResearchIcicle : ResearchSpell** — class (~13 public members). (+1 type)
- `ResearchMassDeath.cs` — **ResearchMassDeath : ResearchSpell** — class (~13 public members). (+1 type)
- `ResearchSnowBall.cs` — **ResearchSnowBall : ResearchSpell** — class (~12 public members). (+1 type)

#### Scripts/Engines and Systems/Magic/Shinobi — Magic/spell system: Shinobi. Types: CastShinobiSpells, ShinobiScroll, ShinobiSpell.

- `ShinobiCommandList.cs` — **CastShinobiSpells** — Casts Cheetah Paw
- `ShinobiScroll.cs` — **ShinobiScroll : Item** — Item ("shinobi scroll"). [Constructable] (+3 types)
- `ShinobiSpell.cs` — **ShinobiSpell : Spell** — Spell.

#### Scripts/Engines and Systems/Magic/Shinobi/Spells — Magic/spell system: Spells. Types: CheetahPaws, Deception, EagleEye, Espionage, ....

- `CheetahPaws.cs` — **CheetahPaws : ShinobiSpell** — class (~11 public members).
- `Deception.cs` — **Deception : ShinobiSpell** — class (~11 public members). (+1 type)
- `EagleEye.cs` — **EagleEye : ShinobiSpell** — class (~10 public members). (+1 type)
- `Espionage.cs` — **Espionage : ShinobiSpell** — class (~9 public members). (+1 type)
- `FerretFlee.cs` — **FerretFlee : ShinobiSpell** — class (~8 public members).
- `MonkeyLeap.cs` — **MonkeyLeap : ShinobiSpell** — class (~12 public members). (+1 type)
- `MysticShuriken.cs` — **MysticShuriken : ShinobiSpell** — class (~11 public members). (+1 type)
- `TigerStrength.cs` — **TigerStrength : ShinobiSpell** — class (~9 public members).

#### Scripts/Engines and Systems/Magic/Syth — Magic/spell system: Syth. Types: HellShard, CastSythSpells, SythDatacron01, SythSpell, ....

- `HellShard.cs` — **HellShard : BaseReagent** — class (~6 public members). [Constructable]
- `SythCommandList.cs` — **CastSythSpells** — Casts Psychokinesis
- `SythDatacrons.cs` — **SythDatacron01 : SpellScroll** — //////////////////////////////////////////////////////////////////////////////////////////// [Constructable] (+9 types)
- `SythSpell.cs` — **SythSpell : Spell** — Spell.
- `SythSpellBookGump.cs` — **SythSpellbookGump : Gump** — Gump UI.
- `SythSpellbook.cs` — **SythSpellbook : Spellbook** — class (~32 public members). [Constructable] (+2 types)

#### Scripts/Engines and Systems/Magic/Syth/Spells — Magic/spell system: Spells. Types: Absorption, CloneBody, DeathGrip, DrainLife, ....

- `Absorption.cs` — **Absorption : SythSpell** — class (~11 public members).
- `Clone.cs` — **CloneBody : SythSpell** — class (~10 public members).
- `DeathGrip.cs` — **DeathGrip : SythSpell** — class (~15 public members). (+2 types)
- `DrainLife.cs` — **DrainLife : SythSpell** — class (~15 public members). (+2 types)
- `Projection.cs` — **Projection : SythSpell** — class (~23 public members). (+1 type)
- `PsychicBlast.cs` — **PsychicBlast : SythSpell** — class (~14 public members). (+1 type)
- `Psychokinesis.cs` — **Psychokinesis : SythSpell** — class (~15 public members). (+1 type)
- `Speed.cs` — **SythSpeed : SythSpell** — class (~13 public members).
- `SythLightning.cs` — **SythLightning : SythSpell** — class (~12 public members). (+1 type)
- `ThrowSword.cs` — **ThrowSword : SythSpell** — class (~15 public members). (+1 type)

#### Scripts/Engines and Systems/Magic/Witch — Magic/spell system: Witch. Types: BookWitchBrewing, UndeadSpell, BloodPactScroll, WitchPouch.

- `BookWitchBrewing.cs` — **BookWitchBrewing : Item** — Item ("The Witch's Brew"). [Constructable] (+1 type)
- `UndeadSpell.cs` — **UndeadSpell : Spell** — Spell.
- `WitchBrews.cs` — **BloodPactScroll : SpellScroll** — class (~128 public members). [Constructable] (+15 types)
- `WitchPouch.cs` — **WitchPouch : Bag** — class (~60 public members). [Constructable] (+3 types)

#### Scripts/Engines and Systems/Magic/Witch/Effects — Magic/spell system: Effects. Types: UnBloodlessSpecPactSpell, UndeadCurePoisonSpell, GhostPhaseSpell, GhostlyImagesSpell, ....

- `BloodPact.cs` — **UnBloodlessSpecPactSpell : UndeadSpell** — class (~9 public members). (+1 type)
- `CurePoison.cs` — **UndeadCurePoisonSpell : UndeadSpell** — class (~9 public members). (+1 type)
- `GhostPhase.cs` — **GhostPhaseSpell : UndeadSpell** — class (~10 public members). (+1 type)
- `GhostlyImages.cs` — **GhostlyImagesSpell : UndeadSpell** — class (~18 public members). (+2 types)
- `GraveyardGateway.cs` — **UndeadGraveyardGatewaySpell : UndeadSpell** — class (~16 public members). (+3 types)
- `HellsBrand.cs` — **HellsBrandSpell : UndeadSpell** — class (~9 public members). (+1 type)
- `HellsGate.cs` — **HellsGateSpell : UndeadSpell** — class (~11 public members). (+1 type)
- `ManaLeech.cs` — **ManaLeechSpell : UndeadSpell** — class (~11 public members). (+1 type)
- `NecroPoison.cs` — **NecroPoisonSpell : UndeadSpell** — class (~11 public members). (+1 type)
- `NecroUnlock.cs` — **NecroUnlockSpell : UndeadSpell** — class (~7 public members). (+1 type)
- `Phantasm.cs` — **PhantasmSpell : UndeadSpell** — class (~8 public members). (+1 type)
- `RetchedAir.cs` — **RetchedAirSpell : UndeadSpell** — class (~9 public members). (+1 type)
- `SpectreShadow.cs` — **SpectreShadowSpell : UndeadSpell** — class (~12 public members). (+2 types)
- `UndeadEyes.cs` — **UndeadEyesSpell : UndeadSpell** — class (~9 public members). (+2 types)
- `VampireGift.cs` — **VampireGiftSpell : UndeadSpell** — class (~9 public members). (+1 type)
- `WallOfSpikes.cs` — **UndeadWallOfSpikesSpell : UndeadSpell** — class (~15 public members). (+3 types)

#### Scripts/Engines and Systems/Messaging — Messaging/social system.

- `Icons.cs` — **Icons** — class (~10 public members).
- `MessageLogger.cs` — **MessageLogger** — class (~5 public members).
- `NotificationService.cs` — **NotificationService** — class (~5 public members).

#### Scripts/Engines and Systems/MobileEnhancement — Mobile enhancement tweaks and helpers.

- `Engine.cs` — **Engine** — Central engine for mobile enhancements (e.g. bard songs). Holds a context per mobile, runs a periodic valid...
- `MobileContext.cs` — **MobileContext** — Per-mobile container of active enhancements. Tracks each enhancement and runs Validate() to expire or resch... (+1 type)

#### Scripts/Engines and Systems/MobileEnhancement/Abstractions — Abstractions. Types: EnhancementDecorator, EnhancementRecipientDecorator, EnhancementSourceBase, IEnhancement, ....

- `EnhancementDecorator.cs` — **EnhancementDecorator : IEnhancement** — class (~11 public members).
- `EnhancementRecipientDecorator.cs` — **EnhancementRecipientDecorator** — class (~13 public members).
- `EnhancementSourceBase.cs` — **EnhancementSourceBase** — Abstract class (~1 public members).
- `IEnhancement.cs` — **IEnhancement** — Interface contract.
- `IEnhancementRecipient.cs` — **IEnhancementRecipient** — Interface contract.
- `Recipient.cs` — **Recipient** — Base for recipient enhancements that are removed only when the Source removes them. Effectively persistent...

#### Scripts/Engines and Systems/MobileEnhancement/Enhancements — Enhancements. Types: OwnerDependentEnhancement.

- `OwnerDependentEnhancement.cs` — **OwnerDependentEnhancement** — Abstract class (~3 public members).

#### Scripts/Engines and Systems/MobileEnhancement/Recipients — Recipients. Types: RangeDependentRecipient, TimeDependentRecipient.

- `RangeDependentRecipient.cs` — **RangeDependentRecipient** — Tracks the Source, validates range, and allows a grace period after the target goes out of range before the...
- `TimeDependentRecipient.cs` — **TimeDependentRecipient** — Base for a single recipient of an enhancement. Tracks the Duration and removes when time is up.

#### Scripts/Engines and Systems/ModernSkill — Modern skill UI/logic.

- `LockpickAndRemoveTrapGump.cs` — **LockpickAndRemoveTrapGump : Gump** — Gump UI. (+1 type)
- `ModernSkillSetupGump.cs` — **ModernSkillSetupGump : Gump** — Gump UI. (+1 type)

#### Scripts/Engines and Systems/PuzzleChest — Puzzle chest mini-system.

- `PuzzleChest.cs` — **PuzzleChest** — class (~17 public members).
- `PuzzleChestCylinder.cs` — **PuzzleChestCylinder** — Enum with about 9 members.
- `PuzzleChestSolution.cs` — **PuzzleChestSolution** — class (~15 public members).
- `PuzzleChestSolutionAndTime.cs` — **PuzzleChestSolutionAndTime : PuzzleChestSolution** — class (~5 public members).
- `PuzzleGump.cs` — **PuzzleGump : Gump** — Gump UI. (+2 types)

#### Scripts/Engines and Systems/Quests — Quest framework and content (standard, fishing, assassin, museum, runes).

- `GygaxStatue.cs` — **GygaxStatue : Item** — Item ("scroll of Gygax"). [Constructable] (+3 types)
- `HelpMessage.cs` — **HelpMessage : Item** — Item ("help"). [Constructable]
- `NoticeClue.cs` — **NoticeClue : Item** — Item ("clue"). [Constructable]
- `QuestChests.cs` — **BardHarkynBox : Item** — ///////////////////////////////////////////////////////////////////////////////////////////////////////////... [Constructable] (+10 types)
- `QuestGlow.cs` — **QuestGlow : Item** — Item ("a strange glow"). [Constructable] (+1 type)
- `QuestStories.cs` — **QuestCharacters** — Quest object.
- `QuestTeleporter.cs` — **QuestTeleporter : Item** — Item ("Quest Teleporter"). [Constructable]
- `QuestTransporter.cs` — **QuestTransporter : Teleporter** — Quest object.
- `Quests.cs` — **Quests** — Commands: [quests
- `ScrollClue.cs` — **ScrollClue : Item** — Item ("a parchment"). [Constructable] (+1 type)
- `SomeRandomNote.cs` — **SomeRandomNote : Item** — Item ("an old parchment"). [Constructable] (+1 type)
- `TriggerTile.cs` — **TriggerTile : Item** — Item ("trigger"). [Constructable]

#### Scripts/Engines and Systems/Quests/Assassin — Quest content: Assassin. Types: BoxOfAtonement, AssassinFunctions.

- `AssassinBox.cs` — **BoxOfAtonement : Item** — Item ("Box of Atonement"). [Constructable]
- `AssassinFunctions.cs` — **AssassinFunctions** — Quest object.

#### Scripts/Engines and Systems/Quests/Bards Tale — Quest content: Bards tale. Types: MangarRobe.

- `MangarsRewards.cs` — **MangarRobe : Robe** — Quest object ("Mangar's Robe").

#### Scripts/Engines and Systems/Quests/Codex — Quest content: Codex. Types: ApproachVoid, CodexWisdom, CubeOnCorpse, DoorCodex, ....

- `ApproachVoid.cs` — **ApproachVoid : Item** — Item ("floor"). [Constructable]
- `CodexWisdom.cs` — **CodexWisdom : TrinketTalisman** — Quest object ("Codex of Ultimate Wisdom").
- `CubeOnCorpse.cs` — **CubeOnCorpse : Item** — Item ("vortex cube"). [Constructable]
- `DoorCodex.cs` — **DoorCodex : Item** — Item ("chamber door"). [Constructable, Flipable]
- `VortexCube.cs` — **VortexCube : Item** — Item ("vortex cube"). (+1 type)

#### Scripts/Engines and Systems/Quests/Core — Quest content: Core. Types: IDeserializable, IQuestGiver, MLQuest, MLQuestContext, ....

- `IDeserializable.cs` — **IDeserializable** — Interface contract.
- `IQuestGiver.cs` — **IQuestGiver** — Interface contract.
- `MLQuest.cs` — **MLQuest** — Quest object.
- `MLQuestContext.cs` — **MLQuestContext** — Context/state holder. (+2 types)
- `MLQuestEntry.cs` — **MLQuestInstanceFlags : byte** — Enum with about 4 members. (+1 type)
- `MLQuestPersistence.cs` — **MLQuestPersistence : Item** — Item.
- `MLQuestSystem.cs` — **MLQuestSystem** — Commands: [InvalidQuestItems, [MLQuestsClearSpawners, [MLQuestsGenerate, [MLQuestsInfo, [SaveAllQuests, [SaveQuest (+3 types)
- `QuestArea.cs` — **QuestArea** — Quest object.
- `QuesterNameAttribute.cs` — **QuesterNameAttribute : Attribute** — Quest object.
- `ValidationQueue.cs` — **ValidationQueue** — Static system module (Initialize). (+1 type)

#### Scripts/Engines and Systems/Quests/Core/Definitions — Quest content: Definitions. Types: BlacksmithRecipeQuest, BlacksmithBasicsQuest, ArtifactRumor, StealingThePastQuest, ....

- `BlacksmithRecipes.cs` — **BlacksmithRecipeQuest : MLQuest** — Quest object ("Ringmail Armor Recipes").
- `BlacksmithTraining.cs` — **BlacksmithBasicsQuest : MLQuest** — Quest object ("training shovel").
- `SageArtifact.cs` — **ArtifactRumor : MLQuest** — Quest object ("highly reliable legend for ").
- `StealingThePast.cs` — **StealingThePastQuest : MLQuest** — Quest object.
- `TheWar.cs` — **SupportingTheWarQuest : MLQuest** — Quest object ("Weapon Recipes").

#### Scripts/Engines and Systems/Quests/Core/Gumps — Quest content: Gumps. Types: BaseQuestGump, InfoNPCGump, QuestCancelConfirmGump, QuestConversationGump, ....

- `BaseQuestGump.cs` — **BaseQuestGump : Gump** — Gump UI. (+3 types)
- `InfoNPCGump.cs` — **InfoNPCGump : BaseQuestGump** — Quest object.
- `QuestCancelConfirmGump.cs` — **QuestCancelConfirmGump : Gump** — Gump UI.
- `QuestConversationGump.cs` — **QuestConversationGump : BaseQuestGump** — Quest object.
- `QuestLogDetailedGump.cs` — **QuestLogDetailedGump : BaseQuestGump** — Quest object.
- `QuestLogGump.cs` — **QuestLogGump : BaseQuestGump** — Quest object.
- `QuestOfferGump.cs` — **QuestOfferGump : BaseQuestGump** — Quest object.
- `QuestReportBackGump.cs` — **QuestReportBackGump : BaseQuestGump** — Quest object.
- `QuestRewardGump.cs` — **QuestRewardGump : BaseQuestGump** — Quest object.

#### Scripts/Engines and Systems/Quests/Core/Objectives — Quest content: Objectives. Types: BaseObjective, CollectObjective, CraftObjective, DeliverObjective, ....

- `BaseObjective.cs` — **BaseObjective** — Quest object.
- `CollectObjective.cs` — **CollectObjective : BaseObjective** — Quest object.
- `CraftObjective.cs` — **CraftObjective : CollectObjective** — Basically a clone of CollectObjective, just used for better messaging (+2 types)
- `DeliverObjective.cs` — **DeliverObjective : BaseObjective** — Quest object.
- `DummyObjective.cs` — **DummyObjective : BaseObjective** — Quest object.
- `GainSkillObjective.cs` — **GainSkillObjective : BaseObjective** — Quest object.
- `KillObjective.cs` — **KillObjective : BaseObjective** — Quest object.

#### Scripts/Engines and Systems/Quests/Core/Rewards — Quest content: Rewards. Types: BaseReward, ConstructibleItemReward, DummyReward, FameReward, ....

- `BaseReward.cs` — **BaseReward** — Quest object.
- `ConstructibleItemReward.cs` — **ConstructibleItemReward : BaseReward** — Quest object.
- `DummyReward.cs` — **DummyReward : BaseReward** — Quest object.
- `FameReward.cs` — **FameReward : BaseReward** — Quest object.
- `ItemReward.cs` — **ItemReward : BaseReward** — Quest object.

#### Scripts/Engines and Systems/Quests/Core/Utilities — Quest content: Utilities. Types: DeliveryQuestUtilities.

- `DeliveryQuestUtilities.cs` — **DeliveryQuestUtilities** — Quest object.

#### Scripts/Engines and Systems/Quests/Epic — Quest content: Epic. Types: CourierMail, EpicGump.

- `CourierMail.cs` — **CourierMail : Item** — Item ("Message for "). [Constructable] (+1 type)
- `EpicGump.cs` — **EpicGump : Gump** — Gump UI.

#### Scripts/Engines and Systems/Quests/Fishing — Quest content: Fishing. Types: FishingQuestBoard, FishingQuestFunctions.

- `FishingQuestBoard.cs` — **FishingQuestBoard : Item** — Item ("Seeking Brave Sailors"). [Constructable, Flipable] (+3 types)
- `FishingQuestFunctions.cs` — **FishingQuestFunctions** — Quest object.

#### Scripts/Engines and Systems/Quests/Frankenstein — Quest content: Frankenstein. Types: EmbalmingFluid, FrankenArmLeft, FrankenArmRight, FrankenBrain, ....

- `EmbalmingFluid.cs` — **EmbalmingFluid : Item** — Item ("embalming fluid"). [Constructable] (+1 type)
- `FrankenArmLeft.cs` — **FrankenArmLeft : FrankenItem** — Quest object ("severed left arm").
- `FrankenArmRight.cs` — **FrankenArmRight : FrankenItem** — Quest object ("severed right arm").
- `FrankenBrain.cs` — **FrankenBrain : FrankenItem** — Quest object ("fresh brain").
- `FrankenFighter.cs` — **FrankenFighter : BaseCreature** — Creature ("a reanimation", AI_Melee, Closest). [Constructable, CorpseName]
- `FrankenHead.cs` — **FrankenHead : FrankenItem** — Quest object ("severed head").
- `FrankenItem.cs` — **FrankenItem : Item** — Item ("body part"). [Constructable] (+1 type)
- `FrankenJournal.cs` — **FrankenJournal : Item** — Item ("Frankenstein's Journal"). [Constructable] (+1 type)
- `FrankenLegLeft.cs` — **FrankenLegLeft : FrankenItem** — Quest object ("severed left leg").
- `FrankenLegRight.cs` — **FrankenLegRight : FrankenItem** — Quest object ("severed right leg").
- `FrankenPorter.cs` — **FrankenPorter : BaseCreature** — Creature ("a reanimation", AI_Animal, Aggressor). [Constructable, CorpseName]
- `FrankenPorterItem.cs` — **FrankenPorterItem : Item** — Item ("a reanimation"). [Constructable]
- `FrankenTorso.cs` — **FrankenTorso : FrankenItem** — Quest object ("severed torso").
- `PowerCoil.cs` — **PowerCoil : Item** — Item ("power coil"). [Constructable]

#### Scripts/Engines and Systems/Quests/Golems — Quest content: Golems. Types: BottleOil, GolemFighter, GolemManual, GolemPorter, ....

- `BottleOil.cs` — **BottleOil : Item** — Item ("technomancer oil"). [Constructable]
- `GolemFighter.cs` — **GolemFighter : BaseCreature** — Creature ("a golem", AI_Melee, Closest). [Constructable, CorpseName]
- `GolemManual.cs` — **GolemManual : Item** — Item ("Manual of Golems"). [Constructable] (+1 type)
- `GolemPorter.cs` — **GolemPorter : BaseCreature** — Creature ("a golem", AI_Animal, Aggressor). [Constructable, CorpseName]
- `GolemPorterItem.cs` — **GolemPorterItem : Item** — Item ("a golem"). [Constructable]

#### Scripts/Engines and Systems/Quests/Hoard — Quest content: Hoard. Types: HoardPiles, HoardTile.

- `HoardPile.cs` — **HoardPiles : Item** — Item ("treasure hoard"). [Constructable]
- `HoardSpawner.cs` — **HoardTile : Item** — Item ("hoard tile"). [Constructable] (+1 type)

#### Scripts/Engines and Systems/Quests/Jester — Quest content: Jester. Types: JokeBook.

- `JokeBook.cs` — **JokeBook : Item** — Item. [Constructable]

#### Scripts/Engines and Systems/Quests/Magic Pools — Quest content: Magic pools. Types: MagicPool.

- `MagicPool.cs` — **MagicPool : BaseAddon** — House addon ("mystical chest from "). (+1 type)

#### Scripts/Engines and Systems/Quests/Major — Quest content: Major. Types: QuestTake, QuestTome.

- `QuestTake.cs` — **QuestTake : Item** — Item ("Journal of "). [Constructable] (+2 types)
- `QuestTome.cs` — **QuestTome : Item** — Item ("lost journal"). (+1 type)

#### Scripts/Engines and Systems/Quests/Museum — Quest content: Museum. Types: Museums, MuseumBook, MuseumBookGump.

- `Museum.cs` — **Museums : Item** — Item ("antique"). [Constructable]
- `MuseumBook.cs` — **MuseumBook : Item** — Item ("Tome of Antiques"). [Constructable]
- `MuseumBookGump.cs` — **MuseumBookGump : Gump** — Gump UI.

#### Scripts/Engines and Systems/Quests/Pagan — Quest content: Pagan. Types: ApproachObsidian, ObeliskOnCorpse, ObeliskTip, ObsidianGate, ....

- `ApproachObsidian.cs` — **ApproachObsidian : Item** — Item ("floor"). [Constructable]
- `ObeliskOnCorpse.cs` — **ObeliskOnCorpse : Item** — Item ("obelisk tip"). [Constructable]
- `ObeliskTip.cs` — **ObeliskTip : Item** — Item ("obelisk tip"). [Constructable] (+1 type)
- `ObsidianGate.cs` — **ObsidianGate : Item** — Item ("Blackrock Gate"). [Constructable, Flipable]
- `PaganArtifact.cs` — **PaganArtifact : Item** — Item ("Pagan Artifact"). [Constructable] (+1 type)
- `PaganBase.cs` — **PaganBase : BaseAddon** — House addon.
- `PaganBaseEmpty.cs` — **PaganBaseEmpty : Item** — Item ("Pedestal"). [Constructable]

#### Scripts/Engines and Systems/Quests/Prisoners — Quest content: Prisoners. Types: Prisoner.

- `Prisoner.cs` — **Prisoner : Item** — Item ("caged creature"). [Constructable] (+1 type)

#### Scripts/Engines and Systems/Quests/Robots — Quest content: Robots. Types: Robot, RobotBatteries, RobotBolt, RobotCircuitBoard, ....

- `Robot.cs` — **Robot : BaseCreature** — Creature ("a robot", AI_Melee, Closest). [Constructable, CorpseName]
- `RobotBatteries.cs` — **RobotBatteries : Item** — Item. [Constructable] (+1 type)
- `RobotBolt.cs` — **RobotBolt : Item** — Item ("robot bolt"). [Constructable]
- `RobotCircuitBoard.cs` — **RobotCircuitBoard : Item** — Item ("robot circuit board"). [Constructable]
- `RobotEngineParts.cs` — **RobotEngineParts : Item** — Item ("robot engine parts"). [Constructable]
- `RobotGears.cs` — **RobotGears : Item** — Item ("robot gears"). [Constructable]
- `RobotItem.cs` — **RobotItem : Item** — Item ("a robot"). [Constructable]
- `RobotOil.cs` — **RobotOil : Item** — Item ("robot oil"). [Constructable]
- `RobotSchematics.cs` — **RobotSchematics : Item** — Item ("Robot Schematics"). [Constructable] (+1 type)
- `RobotSheetMetal.cs` — **RobotSheetMetal : Item** — Item ("robot metal"). [Constructable]
- `RobotTransistors.cs` — **RobotTransistor : Item** — Item ("robot transistor"). [Constructable]

#### Scripts/Engines and Systems/Quests/Runes — Quest content: Runes. Types: RuneBox, RunesBase, RunesBaseEmpty.

- `RuneBox.cs` — **RuneBox : Item** — Item ("chest of virtue"). [Constructable] (+1 type)
- `RunesBase.cs` — **RunesBase : BaseAddon** — House addon.
- `RunesBaseEmpty.cs` — **RunesBaseEmpty : Item** — Item ("Pedestal"). [Constructable]

#### Scripts/Engines and Systems/Quests/Search — Quest content: Search. Types: SearchBase.

- `SearchBase.cs` — **SearchBase : BaseAddon** — House addon ("Fake "). [Constructable]

#### Scripts/Engines and Systems/Quests/Serpents — Quest content: Serpents. Types: BaneBase, BaneBaseEmpty, BlackrockSerpentOrder, LanternOfDiscipline, ....

- `BaneBase.cs` — **BaneBase : BaseAddon** — House addon.
- `BaneBaseEmpty.cs` — **BaneBaseEmpty : Item** — Item ("Pedestal"). [Constructable]
- `BlackrockSerpents.cs` — **BlackrockSerpentOrder : Item** — Item ("Blackrock Serpent"). [Constructable] (+5 types)
- `LanternOfDiscipline.cs` — **LanternOfDiscipline : Item** — Item ("Lantern of Discipline"). [Constructable]
- `OrbOfLogic.cs` — **OrbOfLogic : Item** — Item ("Orb of Logic"). [Constructable]
- `ScalesOfEthicality.cs` — **ScalesOfEthicality : Item** — Item ("Scales of Ethicality"). [Constructable]
- `SerpentSpawners.cs` — **SerpentSpawnerOrder : Item** — Item ("Serpent of Order"). [Constructable] (+3 types)

#### Scripts/Engines and Systems/Quests/Shadowlords — Quest content: Shadowlords. Types: BalinorTeleporter, BellOfCourage, BookOfTruth, CandleOfLove, ....

- `BalinorTeleporter.cs` — **BalinorTeleporter : Item** — Item ("balinor teleporter"). [Constructable]
- `BellOfCourage.cs` — **BellOfCourage : Item** — Item ("Bell of Courage"). [Constructable]
- `BookOfTruth.cs` — **BookOfTruth : Item** — Item ("Book of Truth"). [Constructable]
- `CandleOfLove.cs` — **CandleOfLove : Item** — Item ("Candle of Love"). [Constructable]
- `FlamesBase.cs` — **FlamesBase : BaseAddon** — House addon.
- `FlamesBaseEmpty.cs` — **FlamesBaseEmpty : Item** — Item ("Pedestal"). [Constructable]
- `GemImmortality.cs` — **GemImmortality : Item** — Item ("gem of immortality"). [Constructable] (+3 types)
- `ShardOfCowardice.cs` — **ShardOfCowardice : Item** — Item ("Shard of Cowardice"). [Constructable]
- `ShardOfFalsehood.cs` — **ShardOfFalsehood : Item** — Item ("Shard of Falsehood"). [Constructable]
- `ShardOfHatred.cs` — **ShardOfHatred : Item** — Item ("Shard of Hatred"). [Constructable]

#### Scripts/Engines and Systems/Quests/Standard — Quest content: Standard. Types: StandardQuestBoard, StandardQuestFunctions.

- `StandardQuestBoard.cs` — **StandardQuestBoard : Item** — Item ("Seeking Brave Adventurers"). [Constructable, Flipable]
- `StandardQuestFunctions.cs` — **StandardQuestFunctions** — Quest object.

#### Scripts/Engines and Systems/Quests/Summon — Quest content: Summon. Types: SummonQuests, SummonChest, SummonItems, SummonPrison, ....

- `SummonCarriers.cs` — **SummonQuests** — Quest object ("heart of ash").
- `SummonChest.cs` — **SummonChest : MetalGoldenChest** — Quest object ("Metal Chest").
- `SummonItems.cs` — **SummonItems : Item** — Item ("item"). [Constructable] (+1 type)
- `SummonPrison.cs` — **SummonPrison : Item** — Item ("Magical Prison"). [Constructable] (+1 type)
- `SummonTutorial.cs` — **SummonTutorial : Gump** — Gump UI.

#### Scripts/Engines and Systems/Quests/Thief — Quest content: Thief. Types: Coffer, HayCrate, HollowStump, StealBag, ....

- `Coffer.cs` — **Coffer : Container** — Container ("coffer"). [Constructable, Flipable]
- `HayCrate.cs` — **HayCrate : Item** — Item ("hay"). [Constructable]
- `HollowStump.cs` — **HollowStump : Item** — Item ("hollow stump"). [Constructable, Flipable]
- `StealBag.cs` — **StealBag : Bag** — Quest object.
- `StealBase.cs` — **StealBase : BaseAddon** — House addon.
- `StealBaseEmpty.cs` — **StealBaseEmpty : Item** — Item ("Pedestal"). [Constructable]
- `StealBox.cs` — **StealBox : WoodenBox** — Quest object.
- `StealMetalBox.cs` — **StealMetalBox : WoodenBox** — Quest object.
- `ThiefNote.cs` — **ThiefNote : Item** — Item ("secret note"). [Constructable] (+1 type)

#### Scripts/Engines and Systems/Quests/Underworld — Quest content: Underworld. Types: RuneStoneGate, SkullOfBaronAlmric, UnderworldTeleporter.

- `RuneStoneGate.cs` — **RuneStoneGate : Item** — Item ("runic doorway"). [Constructable]
- `SkullOfBaron Almric.cs` — **SkullOfBaronAlmric : Item** — Item ("skull of Baron Almric"). [Constructable]
- `UnderworldTeleporter.cs` — **UnderworldTeleporter : Teleporter** — Quest object ("underworld teleporter").

#### Scripts/Engines and Systems/Spell Bars — Spell-bar UI and commands.

- `SpellBarId.cs` — **SpellBarId** — Enum with about 22 members.
- `SpellBarRegistry.cs` — **SpellBarRegistry** — class (~3 public members).
- `SpellBarSchool.cs` — **SpellBarSchool** — Enum with about 9 members.
- `SpellBarsCommands.cs` — **AncientBook** — Commands: [ancient

#### Scripts/Engines and Systems/Spell Bars/Core — Core. Types: ISpellBarConfiguration, ISpellBarDescriptor, ISpellSchool, SpellBarDefinition, ....

- `ISpellBarConfiguration.cs` — **ISpellBarConfiguration** — Interface contract.
- `ISpellBarDescriptor.cs` — **ISpellBarDescriptor** — Interface contract.
- `ISpellSchool.cs` — **ISpellSchool** — Interface contract.
- `SpellBarDefinition.cs` — **SpellBarDefinition : ISpellBarDescriptor** — class (~10 public members).
- `SpellBarRegistry.cs` — **SpellBarRegistry** — class (~15 public members).
- `SpellBarSetupGump.cs` — **SpellBarSetupGump : SpellBarSetupGumpBase** — class (~4 public members).
- `SpellBarSetupGumpBase.cs` — **SpellBarSetupGumpBase : Gump** — Gump UI.
- `SpellBarSetupPagedGump.cs` — **SpellBarSetupPagedGump : SpellBarSetupGumpBase** — class (~4 public members).
- `SpellBarState.cs` — **SpellBarState** — State holder.
- `SpellBarToolbarGump.cs` — **SpellBarToolbarGump : Gump** — Gump UI.
- `SpellBarsContext.cs` — **SpellBarsContext** — Context/state holder.
- `ToolBarSpellBarConfiguration.cs` — **ToolBarSpellBarConfiguration : ISpellBarConfiguration** — class (~10 public members).

#### Scripts/Engines and Systems/Spell Bars/Schools — Schools. Types: AncientSpellSchool, BardSpellSchool, DeathKnightSpellSchool, ElementalSpellSchool, ....

- `Ancient.cs` — **AncientSpellSchool : ISpellSchool** — class (~25 public members). (+8 types)
- `Bard.cs` — **BardSpellSchool : ISpellSchool** — class (~17 public members). (+4 types)
- `DeathKnight.cs` — **DeathKnightSpellSchool : ISpellSchool** — class (~17 public members). (+4 types)
- `Elemental.cs` — **ElementalSpellSchool : ISpellSchool** — class (~18 public members). (+4 types)
- `Knight.cs` — **KnightSpellSchool : ISpellSchool** — class (~17 public members). (+4 types)
- `Magery.cs` — **MagerySpellSchool : ISpellSchool** — class (~25 public members). (+8 types)
- `Monk.cs` — **MonkSpellSchool : ISpellSchool** — class (~17 public members). (+4 types)
- `Necromancy.cs` — **NecromancySpellSchool : ISpellSchool** — class (~17 public members). (+4 types)
- `Priest.cs` — **PriestSpellSchool : ISpellSchool** — class (~17 public members). (+4 types)

#### Scripts/Engines and Systems/Temptations — Temptation/choice event system.

- `PlayerContext.cs` — **PlayerContext** — Context/state holder.
- `TemptationCommands.cs` — **TemptationsCommand : BaseCommand** — Command implementation.
- `TemptationEngine.cs` — **TemptationEngine** — class (~9 public members).
- `TemptationFlags.cs` — **TemptationFlags : byte** — Enum with about 9 members.
- `TemptationGump.cs` — **TemptationGump : Gump** — Gump UI. (+1 type)

#### Scripts/Engines and Systems/Trades/Apiculture — Trade/craft content: Apiculture. Types: apiBeeHive, apiBeeHiveMainGump, apiBeeHiveHelpGump, Beeswax, ....

- `BeeHiveAddon.cs` — **apiBeeHive : BaseAddon** — House addon ("beehive"). (+3 types)
- `BeeHiveGumps.cs` — **apiBeeHiveMainGump : Gump** — Gump UI. (+5 types)
- `BeeHiveHelp.cs` — **apiBeeHiveHelpGump : Gump** — Gump UI.
- `Beeswax.cs` — **Beeswax : Item** — Item. [Constructable]
- `WaxCrafting.cs` — **WaxingPot : BaseTool** — Tool ("wax crafting pot"). [Constructable]
- `beehivehelper.cs` — **HiveHealth** — Enum with about 4 members. (+4 types)

#### Scripts/Engines and Systems/Trades/Apiculture/Craft — Trade/craft content: Craft. Types: CandleReligious, ColorCandleLong, ColorCandleShort, JarsOfWaxMetal, ....

- `CandleReligious.cs` — **CandleReligious : BaseLight** — Light source ("religious candle"). [Constructable, Flipable]
- `ColorCandleLong.cs` — **ColorCandleLong : BaseLight** — Light source ("colored candle"). [Constructable]
- `ColorCandleShort.cs` — **ColorCandleShort : BaseLight** — Light source ("colored candle"). [Constructable]
- `JarsOfWax.cs` — **JarsOfWaxMetal : Item** — ///////////////////////////////////////////////////////////////////////////////////////////////////////////... [Constructable] (+5 types)
- `PaintCanvas.cs` — **PaintCanvas : Item** — Item ("painting canvas"). [Constructable]
- `WaxPaintings.cs` — **WaxPaintingA : WaxPainting** — class (~55 public members). [Constructable] (+8 types)
- `WaxSculptors.cs` — **WaxSculptors : Item** — Item ("wax sculptor"). [Constructable, Flipable] (+6 types)

#### Scripts/Engines and Systems/Trades/Apiculture/Items — Trade/craft content: Items. Types: HiveTool, apiLargeWaxPot, RawBeeswax, Slumgum, ....

- `HiveTool.cs` — **HiveTool : Item** — Item ("Hive Tool"). [Constructable]
- `LargeWaxPot.cs` — **apiLargeWaxPot : Item** — Item ("Large Wax Pot"). [Constructable] (+1 type)
- `RawBeeswax.cs` — **RawBeeswax : Item** — Item ("Raw Beeswax"). [Constructable]
- `Slumgum.cs` — **Slumgum : Item** — Item ("Slumgum"). [Constructable]
- `SmallWaxPot.cs` — **apiSmallWaxPot : Item** — Item ("Small Wax Pot"). [Constructable] (+3 types)

#### Scripts/Engines and Systems/Trades/Bulk Orders/Rewards — Trade/craft content: Rewards. Types: ColoredAnvil, LeatherGlovesOfMining, PowderOfTemperament.

- `ColoredAnvil.cs` — **ColoredAnvil : Item** — Item ("anvil"). [Constructable] (+1 type)
- `GlovesOfMining.cs` — **LeatherGlovesOfMining : BaseGlovesOfMining** — Abstract class (~64 public members). [Constructable, Flipable] (+3 types)
- `PowderOfTemperament.cs` — **PowderOfTemperament : Item** — Item. [Constructable] (+1 type)

#### Scripts/Engines and Systems/Trades/BulkCraft — Trade/craft content: Bulk craft. Types: BulkCraft, BulkCraftContext, BulkCraftGump, BulkCraftTimer.

- `BulkCraft.cs` — **BulkCraft** — class (~5 public members).
- `BulkCraftContext.cs` — **BulkCraftContext** — Context/state holder.
- `BulkCraftGump.cs` — **BulkCraftGump : Gump** — Gump UI.
- `BulkCraftTimer.cs` — **BulkCraftTimer : Timer** — Timer.

#### Scripts/Engines and Systems/Trades/Core — Trade/craft content: Core. Types: BreakDown, CraftAmountPrompt, CraftContext, CraftGroup, ....

- `BreakDown.cs` — **BreakDown** — class (~7 public members). (+2 types)
- `CraftAmountPrompt.cs` — **CraftAmountPrompt : Prompt** — class (~5 public members). (+1 type)
- `CraftContext.cs` — **CraftContext** — Context/state holder.
- `CraftGroup.cs` — **CraftGroup** — class (~6 public members).
- `CraftGroupCol.cs` — **CraftGroupCol : CollectionBase** — class (~6 public members).
- `CraftGump.cs` — **CraftGump : Gump** — Gump UI. (+1 type)
- `CraftGumpItem.cs` — **CraftGumpItem : Gump** — Gump UI.
- `CraftItem.cs` — **CraftItem** — class (~51 public members). (+4 types)
- `CraftItemCol.cs` — **CraftItemCol : CollectionBase** — class (~7 public members).
- `CraftItemIDAttribute.cs` — **CraftItemIDAttribute : Attribute** — class (~3 public members).
- `CraftItemSkillListGump.cs` — **CraftItemSkillListGump : Gump** — Gump UI.
- `CraftOrSkillGroup.cs` — **CraftOrSkillGroup** — class (~11 public members). (+1 type)
- `CraftRes.cs` — **CraftRes** — class (~10 public members).
- `CraftResCol.cs` — **CraftResCol : CollectionBase** — class (~5 public members).
- `CraftSearchPrompt.cs` — **CraftSearchPrompt : Prompt** — class (~4 public members).
- `CraftSkill.cs` — **CraftSkill** — class (~5 public members).
- `CraftSkillCol.cs` — **CraftSkillCol : CollectionBase** — class (~5 public members).
- `CraftSubRes.cs` — **CraftSubRes** — class (~9 public members).
- `CraftSubResCol.cs` — **CraftSubResCol : CollectionBase** — class (~10 public members).
- `CraftSystem.cs` — **CraftSystem** — Abstract class (~73 public members). (+1 type)
- `CustomCraft.cs` — **CustomCraft** — Abstract class (~10 public members).
- `Enhance.cs` — **Enhance** — class (~6 public members). (+2 types)
- `RecipeScroll.cs` — **RecipeScroll : Item** — Item. [Constructable]
- `Recipes.cs` — **Recipe** — Commands: [ForgetAllRecipes, [LearnAllRecipes
- `Repair.cs` — **Repair** — class (~6 public members). (+1 type)

#### Scripts/Engines and Systems/Trades/Crafting — Trade/craft content: Crafting. Types: DefAlchemy, DefApothecary, DefBlacksmithy, DefBonecrafting, ....

- `DefAlchemy.cs` — **DefAlchemy : CraftSystem** — class (~15 public members).
- `DefApothecary.cs` — **DefApothecary : CraftSystem** — class (~14 public members).
- `DefBlacksmithy.cs` — **DefBlacksmithy : CraftSystem** — class (~24 public members). (+2 types)
- `DefBonecrafting.cs` — **DefBonecrafting : CraftSystem** — class (~14 public members).
- `DefBowFletching.cs` — **DefBowFletching : CraftSystem** — class (~14 public members).
- `DefCarpentry.cs` — **DefCarpentry : CraftSystem** — Data entry/record.
- `DefCartography.cs` — **DefCartography : CraftSystem** — class (~11 public members).
- `DefCooking.cs` — **DefCooking : CraftSystem** — class (~12 public members).
- `DefDraconic.cs` — **DefDraconic : CraftSystem** — class (~17 public members).
- `DefDruidism.cs` — **DefDruidism : CraftSystem** — class (~12 public members).
- `DefGlassblowing.cs` — **DefGlassblowing : CraftSystem** — class (~11 public members).
- `DefInscription.cs` — **DefInscription : CraftSystem** — class (~13 public members). (+1 type)
- `DefLapidary.cs` — **DefLapidary : CraftSystem** — class (~17 public members).
- `DefLeatherworking.cs` — **DefLeatherworking : CraftSystem** — class (~16 public members).
- `DefMasonry.cs` — **DefMasonry : CraftSystem** — class (~12 public members).
- `DefShelves.cs` — **DefShelves : CraftSystem** — class (~12 public members).
- `DefStitching.cs` — **DefStitching : CraftSystem** — class (~15 public members).
- `DefTailoring.cs` — **DefTailoring : CraftSystem** — class (~17 public members).
- `DefTinkering.cs` — **DefTinkering : CraftSystem** — Abstract class (~29 public members). (+5 types)
- `DefWands.cs` — **DefWands : CraftSystem** — class (~14 public members).
- `DefWaxCrafting.cs` — **DefWaxingPot : CraftSystem** — class (~13 public members).
- `DefWitchery.cs` — **DefWitchery : CraftSystem** — class (~12 public members).

#### Scripts/Engines and Systems/Trades/Gardening — Trade/craft content: Gardening. Types: EmptyTheBowlGump, MainPlantGump, PlantBowl, PlantHue, ....

- `EmptyTheBowlGump.cs` — **EmptyTheBowlGump : Gump** — Gump UI.
- `MainPlantGump.cs` — **MainPlantGump : Gump** — Gump UI.
- `PlantBowl.cs` — **PlantBowl : Item** — Item. [Constructable] (+1 type)
- `PlantHue.cs` — **PlantHue** — Enum with about 22 members. (+1 type)
- `PlantItem.cs` — **PlantItem : Item** — Item. [Constructable] (+1 type)
- `PlantPourTarget.cs` — **PlantPourTarget : Target** — Targeting prompt.
- `PlantResources.cs` — **PlantResourceInfo** — Info/data container.
- `PlantSystem.cs` — **PlantSystem** — class (~43 public members). (+2 types)
- `PlantType.cs` — **PlantType** — Enum with about 42 members. (+1 type)
- `PollinateTarget.cs` — **PollinateTarget : Target** — Targeting prompt.
- `ReproductionGump.cs` — **ReproductionGump : Gump** — Gump UI.
- `Seed.cs` — **Seed : Item** — Item. [Constructable] (+1 type)
- `SetToDecorativeGump.cs` — **SetToDecorativeGump : Gump** — Gump UI.

#### Scripts/Engines and Systems/Trades/Gardening/MiscItems — Trade/craft content: Misc items. Types: FertileDirt, GreenThorns, OrangePetals, RedLeaves.

- `FertileDirt.cs` — **FertileDirt : Item** — Item. [Constructable]
- `GreenThorns.cs` — **GreenThorns : Item** — Item. [Constructable] (+12 types)
- `OrangePetals.cs` — **OrangePetals : Item** — Item. [Constructable] (+2 types)
- `RedLeaves.cs` — **RedLeaves : Item** — Item. [Constructable] (+1 type)

#### Scripts/Engines and Systems/Trades/Gardening/Network — Trade/craft content: Network. Types: DisplayHelpTopic.

- `DisplayHelpTopic.cs` — **DisplayHelpTopic : Packet** — Network packet.

#### Scripts/Engines and Systems/Trades/Global Shoppe — Trade/craft content: Global shoppe. Types: RewardType, ShoppeCommands, ShoppeConstants, ShoppeEngine, ....

- `RewardType.cs` — **RewardType** — Enum with about 4 members.
- `ShoppeCommands.cs` — **ShoppeCommands** — Commands: [Shoppe-Disable, [Shoppe-Enable, [Shoppe-GetContext, [Shoppe-Order-Export, [Shoppe-Order-Export-All, [Shoppe-Status (+1 type)
- `ShoppeConstants.cs` — **ShoppeConstants** — class (~12 public members).
- `ShoppeEngine.cs` — **ShoppeEngine** — class (~8 public members). (+1 type)
- `ShoppeOrderConstants.cs` — **ShoppeOrderConstants** — class (~37 public members). (+3 types)
- `ShoppeType.cs` — **ShoppeType** — Enum with about 11 members.

#### Scripts/Engines and Systems/Trades/Global Shoppe/Abstractions — Trade/craft content: Abstractions. Types: ICustomerShoppe, IExceptionalItem, IGemTypeItem, IOrderContext, ....

- `ICustomerShoppe.cs` — **ICustomerShoppe** — Interface contract.
- `IExceptionalItem.cs` — **IExceptionalItem** — Interface contract.
- `IGemTypeItem.cs` — **IGemTypeItem** — Interface contract.
- `IOrderContext.cs` — **IOrderContext** — Interface contract.
- `IOrderShoppe.cs` — **IOrderShoppe** — Interface contract.
- `IResourceItem.cs` — **IResourceItem** — Interface contract.

#### Scripts/Engines and Systems/Trades/Global Shoppe/Calculators — Trade/craft content: Calculators. Types: AlchemyRewardCalculator, BaseCraftRewardCalculator, BaseRewardCalculator, BlacksmithRewardCalculator, ....

- `AlchemyRewardCalculator.cs` — **AlchemyRewardCalculator : ResourceSellPriceRewardCalculator** — class (~2 public members).
- `BaseCraftRewardCalculator.cs` — **BaseCraftRewardCalculator** — Abstract class (~2 public members).
- `BaseRewardCalculator.cs` — **BaseRewardCalculator** — Abstract class (~1 public members).
- `BlacksmithRewardCalculator.cs` — **BlacksmithRewardCalculator : BaseCraftRewardCalculator** — class (~2 public members).
- `BowcraftRewardCalculator.cs` — **BowcraftRewardCalculator : BaseCraftRewardCalculator** — class (~2 public members).
- `CarpenterRewardCalculator.cs` — **CarpenterRewardCalculator : BaseCraftRewardCalculator** — class (~2 public members).
- `CookingRewardCalculator.cs` — **CookingRewardCalculator : ResourceSellPriceRewardCalculator** — class (~2 public members).
- `HerbalistRewardCalculator.cs` — **HerbalistRewardCalculator : ResourceSellPriceRewardCalculator** — class (~2 public members).
- `LibrarianRewardCalculator.cs` — **LibrarianRewardCalculator : ResourceSellPriceRewardCalculator** — class (~2 public members).
- `MorticianRewardCalculator.cs` — **MorticianRewardCalculator : ResourceSellPriceRewardCalculator** — class (~2 public members).
- `ResourceSellPriceRewardCalculator.cs` — **ResourceSellPriceRewardCalculator : BaseRewardCalculator** — Abstract class (~2 public members).
- `TailorRewardCalculator.cs` — **TailorRewardCalculator : BaseCraftRewardCalculator** — class (~2 public members).
- `TinkerRewardCalculator.cs` — **TinkerRewardCalculator : BaseCraftRewardCalculator** — class (~2 public members).

#### Scripts/Engines and Systems/Trades/Global Shoppe/Core — Trade/craft content: Core. Types: BasicCustomerOrderShoppe, CustomerOrderShoppe, CustomerShoppe, OrderFactory, ....

- `BasicCustomerOrderShoppe.cs` — **BasicCustomerOrderShoppe : CustomerOrderShoppe** — Abstract class (~1 public members).
- `CustomerOrderShoppe.cs` — **CustomerOrderShoppe** — Abstract class (~9 public members).
- `CustomerShoppe.cs` — **CustomerShoppe : ShoppeBase** — Abstract class (~8 public members).
- `OrderFactory.cs` — **OrderFactory** — Factory/service.
- `ShoppeBase.cs` — **ShoppeBase : Item** — Item. (+1 type)

#### Scripts/Engines and Systems/Trades/Global Shoppe/Diagnostics — Trade/craft content: Diagnostics. Types: IDiagnosticOrderShoppe, ShoppeCraftSystem.

- `IDiagnosticOrderShoppe.cs` — **IDiagnosticOrderShoppe** — Interface contract.
- `ShoppeCraftSystem.cs` — **ShoppeCraftSystem : CraftSystem** — This is a stub CraftSystem that is used to export the craft items from a CraftSystem. (+1 type)

#### Scripts/Engines and Systems/Trades/Global Shoppe/Gumps — Trade/craft content: Gumps. Types: ExplainShoppeGump, OrderGump, RewardSelectionGump, ShoppeGump, ....

- `ExplainShoppeGump.cs` — **ExplainShoppeGump : Gump** — Gump UI.
- `OrderGump.cs` — **OrderGump : Gump** — Gump UI. (+1 type)
- `RewardSelectionGump.cs` — **RewardSelectionGump : Gump** — Gump UI. (+1 type)
- `ShoppeGump.cs` — **ShoppeGump : Gump** — Gump UI. (+1 type)
- `ShoppeRewardGump.cs` — **ShoppeRewardGump : Gump** — Gump UI. (+2 types)

#### Scripts/Engines and Systems/Trades/Global Shoppe/Models — Trade/craft content: Models. Types: CustomerContext, EquipmentOrderContext, OrderContext, PlayerContext, ....

- `CustomerContext.cs` — **CustomerContext** — Context/state holder.
- `EquipmentOrderContext.cs` — **EquipmentOrderContext : IOrderContext** — Context/state holder.
- `OrderContext.cs` — **OrderContext : IOrderContext** — Context/state holder.
- `PlayerContext.cs` — **PlayerContext** — Context/state holder.
- `TinkerOrderContext.cs` — **TinkerOrderContext : IOrderContext** — Context/state holder.
- `TradeSkillContext.cs` — **TradeSkillContext** — Context/state holder.

#### Scripts/Engines and Systems/Trades/Global Shoppe/Rewards — Trade/craft content: Rewards. Types: AdvancedSkinningKnife, AncientCraftingGloves, ArboristTool, DyeTubTempBase, ....

- `AdvancedSkinningKnife.cs` — **AdvancedSkinningKnife : SkinningKnifeTool** — class (~8 public members). [Constructable]
- `AncientCraftingGloves.cs` — **AncientCraftingGloves : BaseArmor** — These gloves have been imbued with ancient magic, but their power diminishes with use. [Constructable]
- `ArboristTool.cs` — **ArboristTool : Item** — Item ("Arborist Tool"). [Constructable] (+1 type)
- `DyeTubTemp.cs` — **DyeTubTempBase : Item** — Item ("Dye tub (Leather)"). (+4 types)
- `HueVacuumTube.cs` — **HueVacuumTube : Item** — Item ("Dye Extraction Serum"). [Constructable] (+1 type)
- `SkillBonusGloves.cs` — **SkillBonusGloves : BaseArmor** — Armor ("Gloves of "). [Constructable]

#### Scripts/Engines and Systems/Trades/Global Shoppe/Shoppes — Trade/craft content: Shoppes. Types: AlchemistShoppe, BakerShoppe, BlacksmithShoppe, BowyerShoppe, ....

- `AlchemistShoppe.cs` — **AlchemistShoppe : CustomerOrderShoppe** — class (~10 public members). [Constructable, Flipable]
- `BakerShoppe.cs` — **BakerShoppe : CustomerOrderShoppe** — class (~10 public members). [Constructable, Flipable]
- `BlacksmithShoppe.cs` — **BlacksmithShoppe : BasicCustomerOrderShoppe** — class (~10 public members). [Constructable, Flipable]
- `BowyerShoppe.cs` — **BowyerShoppe : BasicCustomerOrderShoppe** — class (~10 public members). [Constructable, Flipable]
- `CarpentryShoppe.cs` — **CarpentryShoppe : BasicCustomerOrderShoppe** — class (~10 public members). [Constructable, Flipable]
- `CartographyShoppe.cs` — **CartographyShoppe : CustomerShoppe** — class (~8 public members). [Constructable, Flipable]
- `HerbalistShoppe.cs` — **HerbalistShoppe : CustomerOrderShoppe** — class (~10 public members). [Constructable, Flipable]
- `LibrarianShoppe.cs` — **LibrarianShoppe : CustomerOrderShoppe** — class (~10 public members). [Constructable, Flipable]
- `MorticianShoppe.cs` — **MorticianShoppe : CustomerOrderShoppe** — class (~10 public members). [Constructable, Flipable]
- `TailorShoppe.cs` — **TailorShoppe : BasicCustomerOrderShoppe** — class (~10 public members). [Constructable, Flipable]
- `TinkerShoppe.cs` — **TinkerShoppe : CustomerOrderShoppe** — class (~10 public members). [Constructable, Flipable]

#### Scripts/Engines and Systems/Trades/Guild — Trade/craft content: Guild. Types: AttributeHandler, EnhancementGump, GuildCraftingProcess, GuildCarpentry, ....

- `AttributeHandler.cs` — **AttributeHandler** — Handler. (+1 type)
- `EnhancementGump.cs` — **EnhancementGump : Gump** — Gump UI.
- `EnhancementStoneProcess.cs` — **GuildCraftingProcess** — class (~10 public members).
- `GuildCarpentry.cs` — **GuildCarpentry : Item** — Item ("Extraordinary Woodworking Tools"). [Constructable]
- `GuildFletching.cs` — **GuildFletching : Item** — Item ("Extraordinary Bowcrafting Tools"). [Constructable]
- `GuildHammer.cs` — **GuildHammer : Item** — Item ("Extraordinary Smithing Tools"). [Constructable]
- `GuildSewing.cs` — **GuildSewing : Item** — Item ("Extraordinary Sewing Kit"). [Constructable]
- `GuildTinkering.cs` — **GuildTinkering : Item** — Item ("Extraordinary Tinkers Tools"). [Constructable]

#### Scripts/Engines and Systems/Trades/Harvest — Trade/craft content: Harvest. Types: BonusHarvestResource, Fishing, GraveRobbing, HarvestArrow, ....

- `BonusHarvestResource.cs` — **BonusHarvestResource** — class (~7 public members).
- `Fishing.cs` — **Fishing : HarvestSystem** — class (~28 public members). (+1 type)
- `GraveRobbing.cs` — **GraveRobbing : HarvestSystem** — class (~10 public members).
- `HarvestArrow.cs` — **HarvestArrow : QuestArrow** — class (~6 public members). (+1 type)
- `HarvestBank.cs` — **HarvestBank** — class (~10 public members).
- `HarvestDefinition.cs` — **HarvestDefinition** — class (~38 public members).
- `HarvestResource.cs` — **HarvestResource** — class (~8 public members).
- `HarvestSoundTimer.cs` — **HarvestSoundTimer : Timer** — Timer.
- `HarvestSystem.cs` — **HarvestSystem** — Abstract class (~39 public members). (+2 types)
- `HarvestTarget.cs` — **HarvestTarget : Target** — Targeting prompt.
- `HarvestTimer.cs` — **HarvestTimer : Timer** — Timer.
- `HarvestVein.cs` — **HarvestVein** — class (~6 public members).
- `Librarian.cs` — **Librarian : HarvestSystem** — class (~10 public members).
- `Lumberjacking.cs` — **Lumberjacking : HarvestSystem** — class (~11 public members). (+1 type)
- `Mining.cs` — **Mining : HarvestSystem** — class (~13 public members).

#### Scripts/Engines and Systems/Trades/Harvest/RichLumberjacking — Trade/craft content: Rich lumberjacking. Types: RichLumberjacking, RichLumberjackingSparkle.

- `RichLumberjacking.cs` — **RichLumberjacking : Lumberjacking** — class (~4 public members).
- `RichLumberjackingSparkle.cs` — **RichLumberjackingSparkle : Item** — This should only be programatically created

#### Scripts/Engines and Systems/Trades/Harvest/RichLumberjacking/Volatile — Trade/craft content: Volatile. Types: VolatileHarvestBank, VolatileHarvestDefinition.

- `VolatileHarvestBank.cs` — **VolatileHarvestBank : HarvestBank** — class (~7 public members).
- `VolatileHarvestDefinition.cs` — **VolatileHarvestDefinition : HarvestDefinition** — class (~6 public members).

#### Scripts/Engines and Systems/Trades/Harvest/RichVeins — Trade/craft content: Rich veins. Types: LootCommands, RichVeinCommands, RichVeinConfig, RichVeinEngine, ....

- `LootCommands.cs` — **LootCommands** — Commands: [Loot-AddAverage, [Loot-AddFilthyRich, [Loot-AddMeager, [Loot-AddPoor, [Loot-AddRich, [Loot-AddUltraRich (+2 types)
- `RichVeinCommands.cs` — **RichVeinCommands** — Commands: [OreVein, [OreVein-Build, [OreVein-Clear, [OreVein-DebugMe, [OreVein-ExportData, [OreVein-ExportMarkers (+2)
- `RichVeinConfig.cs` — **RichVeinConfig** — class (~5 public members).
- `RichVeinEngine.cs` — **RichVeinEngine** — class (~9 public members). (+1 type)
- `RichVeinGump.cs` — **RichVeinGump : Gump** — Gump UI. (+1 type)
- `RichVeinMining.cs` — **RichVeinMining : Mining** — class (~9 public members).
- `RichVeinUtilities.cs` — **RichVeinUtilities** — class (~9 public members).

#### Scripts/Engines and Systems/Trades/Harvest/RichVeins/Items — Trade/craft content: Items. Types: RichVeinMineable, RichVeinSpawner, RichVeinSpawnerPersistent.

- `RichVeinMineable.cs` — **RichVeinMineable : Item** — Item ("A glistening ore vein").
- `RichVeinSpawner.cs` — **RichVeinSpawner : Item** — Item ("Ore Vein Spawner").
- `RichVeinSpawnerPersistent.cs` — **RichVeinSpawnerPersistent : RichVeinSpawner** — class (~5 public members). [Constructable]

#### Scripts/Engines and Systems/Trades/Shoppes — Trade/craft content: Shoppes. Types: DungeoneerCrate, MerchantCrate.

- `DungeoneerCrate.cs` — **DungeoneerCrate : Container** — Container ("dungeoneer crate"). [Constructable, Flipable] (+4 types)
- `MerchantCrate.cs` — **MerchantCrate : Container** — These crates are commonly used by merchants to store wares. [Constructable, Flipable] (+1 type)

#### Scripts/Engines and Systems/Trades/Stone — Trade/craft content: Stone. Types: StatueGateGuardian, BaseStatue, BaseStatueDeed, StoneChairs, ....

- `Assemblies.cs` — **StatueGateGuardian : BaseStatueDeed** — class (~154 public members). [Constructable] (+21 types)
- `BaseStatue.cs` — **BaseStatue : Item** — Item. (+1 type)
- `BaseStatueDeed.cs` — **BaseStatueDeed : BaseAddonDeed** — Addon deed ("statue"). [Constructable] (+2 types)
- `Furniture.cs` — **StoneChairs : BaseStatue** — class (~154 public members). [Constructable, Flipable] (+21 types)
- `Gravestones.cs` — **StoneTombStoneA : BaseStatue** — class (~140 public members). [Constructable, Flipable] (+19 types)
- `Statues.cs` — **StatueElvenKnight : BaseStatue** — class (~245 public members). [Constructable, Flipable] (+34 types)

#### Scripts/Engines and Systems/Trades/Taxidermy — Trade/craft content: Taxidermy. Types: Skulls, MountedTrophyHead, MountingBase, StuffedBear, ....

- `MagicSkulls.cs` — **Skulls** — class (~131 public members). (+13 types)
- `MountedTrophyHead.cs` — **MountedTrophyHead : Item** — Item ("mounted head"). [Constructable]
- `MountingBase.cs` — **MountingBase : Item** — Item ("mounting base"). [Constructable] (+2 types)
- `StuffedBear.cs` — **StuffedBear : Item** — Item ("stuffed bear"). [Constructable]
- `StuffingBasket.cs` — **StuffingBasket : Item** — Item ("stuffing basket"). [Constructable] (+2 types)
- `TrophyBase.cs` — **TrophyBase : Item** — Item ("trophy base"). [Constructable, Flipable] (+2 types)

#### Scripts/Items/Abstractions — Items: Abstractions. Types: ICommodity, IHarvestTool.

- `ICommodity.cs` — **ICommodity** — Interface contract.
- `IHarvestTool.cs` — **IHarvestTool** — Interface contract.

#### Scripts/Items/Armor — Armor and shield item classes.

- `ArmorEnums.cs` — **ArmorQuality** — Enum with about 3 members. (+5 types)
- `BaseArmor.cs` — **BaseArmor : Item** — Base armor: identity, name/property building, resistances, durability.
- `HorseArmor.cs` — **HorseArmor : Item** — Item ("horse barding"). [Constructable] (+1 type)
- `WoodenArmor.cs` — **WoodenPlateLegs : BaseArmor ///////////////////////////////////////////////////////** — Armor item ("wooden leggings").

#### Scripts/Items/Armor/Bone — Armor/shield items: Bone. Types: BoneArms, BoneChest, BoneGloves, BoneHelm, ....

- `BoneArms.cs` — **BoneArms : BaseArmor** — Armor ("bone bracers"). [Constructable, Flipable]
- `BoneChest.cs` — **BoneChest : BaseArmor** — Armor ("bone tunic"). [Constructable, Flipable]
- `BoneGloves.cs` — **BoneGloves : BaseArmor** — Armor ("bone gauntlets"). [Constructable, Flipable]
- `BoneHelm.cs` — **BoneHelm : BaseArmor** — Armor ("bone helm"). [Constructable, Flipable]
- `BoneLegs.cs` — **BoneLegs : BaseArmor** — Armor ("bone greaves"). [Constructable, Flipable]
- `BoneSkirt.cs` — **BoneSkirt : BaseArmor** — Armor ("bone skirt"). [Constructable, Flipable]
- `OrcHelm.cs` — **OrcHelm : BaseArmor** — Armor ("horned helm"). [Constructable]
- `SavageArms.cs` — **SavageArms : BaseArmor** — Armor ("skeletal bracers"). [Constructable]
- `SavageChest.cs` — **SavageChest : BaseArmor** — Armor ("skeletal tunic"). [Constructable]
- `SavageGloves.cs` — **SavageGloves : BaseArmor** — Armor ("skeletal gauntlets"). [Constructable]
- `SavageHelm.cs` — **SavageHelm : BaseArmor** — Armor ("skeletal helm"). [Constructable]
- `SavageLegs.cs` — **SavageLegs : BaseArmor** — Armor ("skeletal greaves"). [Constructable]

#### Scripts/Items/Armor/Chain — Armor/shield items: Chain. Types: ChainChest, ChainCoif, ChainHatsuburi, ChainLegs, ....

- `ChainChest.cs` — **ChainChest : BaseArmor** — Armor ("chainmail tunic"). [Constructable]
- `ChainCoif.cs` — **ChainCoif : BaseArmor** — Armor ("chainmail coif"). [Constructable]
- `ChainHatsuburi.cs` — **ChainHatsuburi : BaseArmor** — Armor. [Constructable]
- `ChainLegs.cs` — **ChainLegs : BaseArmor** — Armor ("chainmail leggings"). [Constructable]
- `ChainSkirt.cs` — **ChainSkirt : BaseArmor** — Armor ("chainmail skirt"). [Constructable]

#### Scripts/Items/Armor/Glasses — Armor/shield items: Glasses. Types: AnthropomorphistGlasses, ArtsGlasses, ElvenGlasses, FoldedSteelGlasses, ....

- `AnthropomorphistGlasses.cs` — **AnthropomorphistGlasses : ElvenGlasses** — Armor item.
- `ArtsGlasses.cs` — **ArtsGlasses : ElvenGlasses** — Armor item.
- `ElvenGlasses.cs` — **ElvenGlasses : BaseArmor** — Armor. [Constructable] (+1 type)
- `FoldedSteelGlasses.cs` — **FoldedSteelGlasses : ElvenGlasses** — Armor item.
- `LightOfWayGlasses.cs` — **LightOfWayGlasses : ElvenGlasses** — Armor item.
- `LyricalGlasses.cs` — **LyricalGlasses : ElvenGlasses** — Armor item.
- `MaceShieldGlasses.cs` — **MaceShieldGlasses : ElvenGlasses** — Armor item.
- `MaritimeGlasses.cs` — **MaritimeGlasses : ElvenGlasses** — Armor item.
- `NecromanticGlasses.cs` — **NecromanticGlasses : ElvenGlasses** — Armor item.
- `PoisonedGlasses.cs` — **PoisonedGlasses : ElvenGlasses** — Armor item.
- `TradeGlasses.cs` — **TradeGlasses : ElvenGlasses** — Armor item.
- `TreasureTrinketGlasses.cs` — **TreasureTrinketGlasses : ElvenGlasses** — Armor item.
- `WizardsGlasses.cs` — **WizardsGlasses : ElvenGlasses** — Armor item.

#### Scripts/Items/Armor/Helmets — Armor/shield items: Helmets. Types: Bascinet, CloseHelm, DreadHelm, Helmet, ....

- `Bascinet.cs` — **Bascinet : BaseArmor** — Armor ("bascinet"). [Constructable]
- `CloseHelm.cs` — **CloseHelm : BaseArmor** — Armor. [Constructable]
- `DreadHelm.cs` — **DreadHelm : BaseArmor** — Armor ("dread helm"). [Constructable]
- `Helmet.cs` — **Helmet : BaseArmor** — Armor. [Constructable]
- `NorseHelm.cs` — **NorseHelm : BaseArmor** — Armor ("norse helm"). [Constructable]
- `OniwabanHood.cs` — **OniwabanHood : LeatherCap** — Armor item ("oniwaban hood").

#### Scripts/Items/Armor/Leather — Armor/shield items: Leather. Types: FemaleLeafChest, FemaleLeatherChest, HideChest, HikingBoots, ....

- `FemaleLeafChest.cs` — **FemaleLeafChest : BaseArmor** — Armor. [Constructable, Flipable]
- `FemaleLeatherChest.cs` — **FemaleLeatherChest : BaseArmor** — Armor. [Constructable, Flipable]
- `HideChest.cs` — **HideChest : BaseArmor** — Armor ("hide tunic"). [Constructable]
- `HikingBoots.cs` — **HikingBoots : BaseShoes** — Armor item ("hiking boots").
- `LeatherArms.cs` — **LeatherArms : BaseArmor** — Armor. [Constructable, Flipable]
- `LeatherBoots.cs` — **LeatherBoots : BaseArmor** — Armor ("leather sandals"). [Constructable, Flipable] (+4 types)
- `LeatherBustierArms.cs` — **LeatherBustierArms : BaseArmor** — Armor. [Constructable, Flipable]
- `LeatherCap.cs` — **LeatherCap : BaseArmor** — Armor. [Constructable, Flipable]
- `LeatherChest.cs` — **LeatherChest : BaseArmor** — Armor ("leather tunic"). [Constructable]
- `LeatherDo.cs` — **LeatherDo : BaseArmor** — Armor. [Constructable]
- `LeatherGarb.cs` — **LeatherCloak : BaseArmor** — Armor ("leather cloak"). [Constructable, Flipable] (+1 type)
- `LeatherGloves.cs` — **LeatherGloves : BaseArmor** — Armor. [Constructable, Flipable]
- `LeatherGorget.cs` — **LeatherGorget : BaseArmor** — Armor. [Constructable]
- `LeatherHaidate.cs` — **LeatherHaidate : BaseArmor** — Armor. [Constructable]
- `LeatherHiroSode.cs` — **LeatherHiroSode : BaseArmor** — Armor. [Constructable]
- `LeatherJingasa.cs` — **LeatherJingasa : BaseArmor** — Armor. [Constructable]
- `LeatherLegs.cs` — **LeatherLegs : BaseArmor** — Armor. [Constructable, Flipable]
- `LeatherMempo.cs` — **LeatherMempo : BaseArmor** — Armor. [Constructable]
- `LeatherNinjaHood.cs` — **LeatherNinjaHood : BaseArmor** — Armor. [Constructable]
- `LeatherNinjaJacket.cs` — **LeatherNinjaJacket : BaseArmor** — Armor. [Constructable]
- `LeatherNinjaMitts.cs` — **LeatherNinjaMitts : BaseArmor** — Armor. [Constructable]
- `LeatherNinjaPants.cs` — **LeatherNinjaPants : BaseArmor** — Armor. [Constructable]
- `LeatherShorts.cs` — **LeatherShorts : BaseArmor** — Armor. [Constructable, Flipable]
- `LeatherSkirt.cs` — **LeatherSkirt : BaseArmor** — Armor. [Constructable, Flipable]
- `LeatherSuneate.cs` — **LeatherSuneate : BaseArmor** — Armor. [Constructable]
- `OniwabanBoots.cs` — **OniwabanBoots : LeatherSoftBoots** — Armor item ("oniwaban boots").
- `OniwabanGloves.cs` — **OniwabanGloves : LeatherGloves** — Armor item ("oniwaban gloves").
- `OniwabanLeggings.cs` — **OniwabanLeggings : LeatherLegs** — Armor item ("oniwaban leggings").
- `OniwabanTunic.cs` — **OniwabanTunic : LeatherChest** — Armor item ("oniwaban tunic").
- `ShinobiGarb.cs` — **ShinobiRobe : BaseArmor** — Armor ("leather shinobi robe"). [Constructable] (+3 types)

#### Scripts/Items/Armor/Plate — Armor/shield items: Plate. Types: DecorativePlateKabuto, FemalePlateChest, HeavyPlateJingasa, LightPlateJingasa, ....

- `DecorativePlateKabuto.cs` — **DecorativePlateKabuto : BaseArmor** — Armor. [Constructable]
- `FemalePlateChest.cs` — **FemalePlateChest : BaseArmor** — Armor. [Constructable, Flipable]
- `HeavyPlateJingasa.cs` — **HeavyPlateJingasa : BaseArmor** — Armor. [Constructable]
- `LightPlateJingasa.cs` — **LightPlateJingasa : BaseArmor** — Armor. [Constructable]
- `PlateArms.cs` — **PlateArms : BaseArmor** — Armor ("platemail arms"). [Constructable]
- `PlateBattleKabuto.cs` — **PlateBattleKabuto : BaseArmor** — Armor. [Constructable]
- `PlateChest.cs` — **PlateChest : BaseArmor** — Armor ("platemail"). [Constructable]
- `PlateDo.cs` — **PlateDo : BaseArmor** — Armor. [Constructable]
- `PlateGloves.cs` — **PlateGloves : BaseArmor** — Armor. [Constructable, Flipable]
- `PlateGorget.cs` — **PlateGorget : BaseArmor** — Armor. [Constructable]
- `PlateHaidate.cs` — **PlateHaidate : BaseArmor** — Armor. [Constructable]
- `PlateHatsuburi.cs` — **PlateHatsuburi : BaseArmor** — Armor. [Constructable]
- `PlateHelm.cs` — **PlateHelm : BaseArmor** — Armor ("plate helm"). [Constructable]
- `PlateHiroSode.cs` — **PlateHiroSode : BaseArmor** — Armor. [Constructable]
- `PlateLegs.cs` — **PlateLegs : BaseArmor** — Armor ("platemail legs"). [Constructable]
- `PlateMempo.cs` — **PlateMempo : BaseArmor** — Armor. [Constructable]
- `PlateSkirt.cs` — **PlateSkirt : BaseArmor** — Armor ("platemail skirt"). [Constructable, Flipable]
- `PlateSuneate.cs` — **PlateSuneate : BaseArmor** — Armor. [Constructable]
- `SmallPlateJingasa.cs` — **SmallPlateJingasa : BaseArmor** — Armor. [Constructable]
- `StandardPlateKabuto.cs` — **StandardPlateKabuto : BaseArmor** — Armor. [Constructable]

#### Scripts/Items/Armor/Ranger — Armor/shield items: Ranger. Types: RangerArms, RangerChest, RangerGloves, RangerGorget, ....

- `RangerArms.cs` — **RangerArms : BaseArmor** — Armor. [Constructable, Flipable]
- `RangerChest.cs` — **RangerChest : BaseArmor** — Armor. [Constructable, Flipable]
- `RangerGloves.cs` — **RangerGloves : BaseArmor** — Armor. [Constructable, Flipable]
- `RangerGorget.cs` — **RangerGorget : BaseArmor** — Armor. [Constructable]
- `RangerLegs.cs` — **RangerLegs : BaseArmor** — Armor. [Constructable, Flipable]

#### Scripts/Items/Armor/Ring — Armor/shield items: Ring. Types: RingmailArms, RingmailChest, RingmailGloves, RingmailLegs, ....

- `RingmailArms.cs` — **RingmailArms : BaseArmor** — Armor. [Constructable, Flipable]
- `RingmailChest.cs` — **RingmailChest : BaseArmor** — Armor. [Constructable, Flipable]
- `RingmailGloves.cs` — **RingmailGloves : BaseArmor** — Armor. [Constructable, Flipable]
- `RingmailLegs.cs` — **RingmailLegs : BaseArmor** — Armor. [Constructable, Flipable]
- `RingmailSkirt.cs` — **RingmailSkirt : BaseArmor** — Armor ("ringmail skirt"). [Constructable]

#### Scripts/Items/Armor/Royal — Armor/shield items: Royal. Types: RoyalArms, RoyalBoots, RoyalChest, RoyalGloves, ....

- `RoyalArms.cs` — **RoyalArms : PlateArms** — Armor item ("royal mantle").
- `RoyalBoots.cs` — **RoyalBoots : PlateGorget** — Armor item ("royal boots").
- `RoyalChest.cs` — **RoyalChest : PlateChest** — Armor item ("royal tunic").
- `RoyalGloves.cs` — **RoyalGloves : PlateGloves** — Armor item ("royal bracers").
- `RoyalGorget.cs` — **RoyalGorget : PlateGorget** — Armor item ("royal gorget").
- `RoyalHelm.cs` — **RoyalHelm : PlateHelm** — Armor item ("royal helm").
- `RoyalLegs.cs` — **RoyalsLegs : PlateLegs** — Armor item ("royal leggings").

#### Scripts/Items/Armor/Scaled — Armor/shield items: Scaled. Types: DragonArms, DragonChest, DragonGloves, DragonHelm, ....

- `DragonArms.cs` — **DragonArms : BaseArmor** — Armor ("scalemail arms"). [Constructable, Flipable]
- `DragonChest.cs` — **DragonChest : BaseArmor** — Armor ("scalemail tunic"). [Constructable, Flipable]
- `DragonGloves.cs` — **DragonGloves : BaseArmor** — Armor ("scalemail gloves"). [Constructable, Flipable]
- `DragonHelm.cs` — **DragonHelm : BaseArmor** — Armor ("scalemail helm"). [Constructable, Flipable]
- `DragonLegs.cs` — **DragonLegs : BaseArmor** — Armor ("scalemail leggings"). [Constructable, Flipable]
- `DrakboneBracers.cs` — **DrakboneBracers : BaseArmor** — Armor ("drakbone bracers"). [Constructable]
- `DrakboneGreaves.cs` — **DrakboneGreaves : BaseArmor** — Armor ("drakbone greaves"). [Constructable]
- `DrakboneGuantlets.cs` — **DrakboneGuantlets : BaseArmor** — Armor ("drakbone gauntlets"). [Constructable]
- `DrakboneHelm.cs` — **DrakboneHelm : BaseArmor** — Armor ("drakbone helm"). [Constructable]
- `DrakboneTunic.cs` — **DrakboneTunic : BaseArmor** — Armor ("drakbone tunic"). [Constructable]
- `ScaledArms.cs` — **ScaledArms : BaseArmor** — Armor ("scaled arms"). [Constructable]
- `ScaledChest.cs` — **ScaledChest : BaseArmor** — Armor ("scaled chest"). [Constructable]
- `ScaledGloves.cs` — **ScaledGloves : BaseArmor** — Armor ("scaled gloves"). [Constructable]
- `ScaledGorget.cs` — **ScaledGorget : BaseArmor** — Armor ("scaled gorget"). [Constructable]
- `ScaledHelm.cs` — **ScaledHelm : BaseArmor** — Armor ("scaled helm"). [Constructable]
- `ScaledLegs.cs` — **ScaledLegs : BaseArmor** — Armor ("scaled legs"). [Constructable]
- `ScaledShield.cs` — **ScaledShield : BaseShield** — Shield ("scaled shield"). [Constructable]
- `ScalemailShield.cs` — **ScalemailShield : BaseShield** — Shield ("scalemail shield"). [Constructable]
- `ScalyArms.cs` — **ScalyArms : BaseArmor** — Armor ("scaly sleeves"). [Constructable]
- `ScalyBoots.cs` — **ScalyBoots : BaseArmor** — Armor ("scaly boots"). [Constructable]
- `ScalyChest.cs` — **ScalyChest : BaseArmor** — Armor ("scaly tunic"). [Constructable]
- `ScalyGloves.cs` — **ScalyGloves : BaseArmor** — Armor ("scaly gloves"). [Constructable]
- `ScalyGorget.cs` — **ScalyGorget : BaseArmor** — Armor ("scaly gorget"). [Constructable]
- `ScalyHelm.cs` — **ScalyHelm : BaseArmor** — Armor ("scaly helm"). [Constructable]
- `ScalyLegs.cs` — **ScalyLegs : BaseArmor** — Armor ("scaly leggings"). [Constructable]

#### Scripts/Items/Armor/Shields — Armor/shield items: Shields. Types: BaseShield, BronzeShield, Buckler, ChampionShield, ....

- `BaseShield.cs` — **BaseShield : BaseArmor** — Armor.
- `BronzeShield.cs` — **BronzeShield : BaseShield** — Shield ("large shield"). [Constructable]
- `Buckler.cs` — **Buckler : BaseShield** — Shield. [Constructable]
- `ChampionShield.cs` — **ChampionShield : BaseShield** — Shield ("champion shield"). [Constructable, Flipable]
- `ChaosShield.cs` — **ChaosShield : BaseShield** — Shield. [Constructable]
- `CrestedShield.cs` — **CrestedShield : BaseShield** — Shield ("crested shield"). [Constructable, Flipable]
- `DarkShield.cs` — **DarkShield : BaseShield** — Shield ("dark shield"). [Constructable, Flipable]
- `ElvenShield.cs` — **ElvenShield : BaseShield** — Shield ("elven shield"). [Constructable, Flipable]
- `GuardsmanShield.cs` — **GuardsmanShield : BaseShield** — Shield ("guardsman shield"). [Constructable, Flipable]
- `HeaterShield.cs` — **HeaterShield : BaseShield** — Shield. [Constructable]
- `JeweledShield.cs` — **JeweledShield : BaseShield** — Shield ("jeweled shield"). [Constructable, Flipable]
- `MetalKiteShield.cs` — **MetalKiteShield : BaseShield** — Shield. [Constructable]
- `MetalShield.cs` — **MetalShield : BaseShield** — Shield. [Constructable]
- `OrderShield.cs` — **OrderShield : BaseShield** — Shield. [Constructable]
- `RoyalShield.cs` — **RoyalShield : HeaterShield** — Armor item ("royal shield").
- `SunShield.cs` — **SunShield : BaseShield** — Shield ("sun shield"). [Constructable]
- `VirtueShield.cs` — **VirtueShield : BaseShield** — Shield ("virtue shield"). [Constructable]
- `WoodenKiteShield.cs` — **WoodenKiteShield : BaseShield** — Shield. [Constructable]
- `WoodenShield.cs` — **WoodenShield : BaseShield** — Shield. [Constructable]

#### Scripts/Items/Armor/Studded — Armor/shield items: Studded. Types: BearCap, FemaleStuddedChest, StuddedArms, StuddedBustierArms, ....

- `AnimalCaps.cs` — **BearCap : BaseArmor** — Armor ("bearskin cap"). [Constructable] (+3 types)
- `FemaleStuddedChest.cs` — **FemaleStuddedChest : BaseArmor** — Armor. [Constructable, Flipable]
- `StuddedArms.cs` — **StuddedArms : BaseArmor** — Armor. [Constructable, Flipable]
- `StuddedBustierArms.cs` — **StuddedBustierArms : BaseArmor** — Armor. [Constructable, Flipable]
- `StuddedChest.cs` — **StuddedChest : BaseArmor** — Armor. [Constructable, Flipable]
- `StuddedDo.cs` — **StuddedDo : BaseArmor** — Armor. [Constructable]
- `StuddedGloves.cs` — **StuddedGloves : BaseArmor** — Armor. [Constructable, Flipable]
- `StuddedGorget.cs` — **StuddedGorget : BaseArmor** — Armor. [Constructable]
- `StuddedHaidate.cs` — **StuddedHaidate : BaseArmor** — Armor. [Constructable]
- `StuddedHideChest.cs` — **StuddedHideChest : BaseArmor** — Armor ("studded hide tunic"). [Constructable]
- `StuddedHiroSode.cs` — **StuddedHiroSode : BaseArmor** — Armor. [Constructable]
- `StuddedLegs.cs` — **StuddedLegs : BaseArmor** — Armor. [Constructable, Flipable]
- `StuddedMempo.cs` — **StuddedMempo : BaseArmor** — Armor. [Constructable]
- `StuddedSkirt.cs` — **StuddedSkirt : BaseArmor** — Armor ("studded skirt"). [Constructable, Flipable]
- `StuddedSuneate.cs` — **StuddedSuneate : BaseArmor** — Armor. [Constructable]

#### Scripts/Items/Boats — Items: Boats. Types: BaseBoat, BaseBoatDeed, BaseDockedBoat, BoatBuild, ....

- `BaseBoat.cs` — **BaseBoat : BaseMulti** — Multi ("a ship key"). (+8 types)
- `BaseBoatDeed.cs` — **BaseBoatDeed : Item** — Item ("magic carpet"). (+1 type)
- `BaseDockedBoat.cs` — **BaseDockedBoat : Item** — Item ("magic carpet"). (+1 type)
- `BoatBuild.cs` — **BoatBuild : Item** — Item ("Schematics of a Small Ship"). [Constructable]
- `BoatDoor.cs` — **BoatDoor : Item** — Item ("cabin hatch"). [Constructable, Flipable] (+1 type)
- `BoatModel.cs` — **BoatModel : BaseAddon** — House addon ("boat model"). [Constructable] (+1 type)
- `BoatStain.cs` — **BoatStain : Item** — Item ("boat stain"). [Constructable] (+1 type)
- `Cargo.cs` — **Cargo : Item** — Item ("wooden barrel"). [Constructable] (+1 type)
- `CarpetBuild.cs` — **CarpetBuild : Item** — Item ("The Carpet of Aladdin"). [Constructable] (+1 type)
- `ConfirmDryDockGump.cs` — **ConfirmDryDockGump : Gump** — Gump UI.
- `DockingLantern.cs` — **DockingLantern : Item** — Item ("docking lantern"). [Constructable] (+1 type)
- `FindboatGump.cs` — **FindboatGump : Gump** — Commands: [FindBoat
- `Galleons.cs` — **GalleonBarbarian : BaseBoat** — Boat. [Constructable] (+10 types)
- `GrapplingHook.cs` — **GrapplingHook : Item** — Item ("grappling hook"). [Constructable] (+1 type)
- `Hold.cs` — **Hold : Container** — Container ("magic bag").
- `LargeBoat.cs` — **LargeBoat : BaseBoat** — Boat ("large sized ship"). [Constructable] (+2 types)
- `LargeDragonBoat.cs` — **LargeDragonBoat : BaseBoat** — Boat ("large sized dragon ship"). [Constructable] (+2 types)
- `MediumBoat.cs` — **MediumBoat : BaseBoat** — Boat ("medium sized ship"). [Constructable] (+2 types)
- `MediumDragonBoat.cs` — **MediumDragonBoat : BaseBoat** — Boat ("medium sized dragon ship"). [Constructable] (+2 types)
- `PirateBounty.cs` — **PirateBounty : Item** — Item ("Pirate Bounty"). [Constructable] (+1 type)
- `Plank.cs` — **Plank : Item** — Item ("magic carpet"). (+3 types)
- `RenameBoatPrompt.cs` — **RenameBoatPrompt : Prompt** — Item.
- `SmallBoat.cs` — **SmallBoat : BaseBoat** — Boat ("small sized ship"). [Constructable] (+2 types)
- `SmallDragonBoat.cs` — **SmallDragonBoat : BaseBoat** — Boat ("small sized dragon ship"). [Constructable] (+2 types)
- `Strandedness.cs` — **Strandedness** — Item.
- `TillerMan.cs` — **TillerMan : Item** — Item ("magic lamp").
- `TillerManGump.cs` — **TillerManGump : Gump** — Gump UI.
- `TinyBoat.cs` — **TinyBoat : BaseBoat** — Boat. [Constructable]
- `Vessels.cs` — **VesselsNS : BaseMulti** — Multi. [Constructable] (+5 types)

#### Scripts/Items/Books — Items: Books. Types: ArtefactBook, BardsTaleNote, BaseBook, BeginnerBook, ....

- `ArtefactBook.cs` — **ArtefactBook : Item** — Item ("Tome of Wondrous Artifacts"). [Constructable] (+2 types)
- `BardsTaleNote.cs` — **BardsTaleNote : Item** — Item ("an old parchment"). [Constructable] (+1 type)
- `BaseBook.cs` — **BaseBook : Item** — Item. [Constructable] (+4 types)
- `BeginnerBook.cs` — **BeginnerBook : Item** — Item ("The Journey Begins"). [Constructable]
- `BookContent.cs` — **BookContent** — Item.
- `DoomFlayerNote.cs` — **DoomFlayerNote : Item** — Item ("a dusty scroll"). [Constructable] (+1 type)
- `DynamicBook.cs` — **DynamicBook : Item** — Item. [Constructable] (+26 types)
- `LearnReagents.cs` — **LearnReagentsBook : Item** — Item ("Scroll of Various Reagents"). [Constructable] (+1 type)
- `LearnStealing.cs` — **LearnStealingBook : Item** — Item ("The Art of Thievery"). [Constructable] (+1 type)
- `LoreBook.cs` — **LoreBook : DynamicBook** — Item ("Staff of Five Parts").
- `SwordsAndShackles.cs` — **SwordsAndShackles : Item** — Item ("Skulls and Shackles"). [Constructable] (+1 type)
- `TitleChangeDeed.cs` — **TitleChangeDeed : Item** — Item ("a title change deed"). [Constructable] (+1 type)
- `WritingBook.cs` — **WritingBook : BaseBook** — Book. [Constructable]

#### Scripts/Items/Books/BulletinBoards — Items: Bulletin boards. Types: AdminBoard, AssassinNote, BoardGump, GuardNote, ....

- `AdminBoard.cs` — **AdminBoard : Item** — Item ("News From The Nobles"). [Constructable, Flipable] (+2 types)
- `AssassinNote.cs` — **AssassinNote : Item** — Item ("a letter"). [Constructable] (+1 type)
- `BoardGump.cs` — **BoardGump : Gump** — Gump UI.
- `GuardNote.cs` — **GuardNote : Item** — Item ("a note"). [Constructable] (+1 type)
- `GuildBoard.cs` — **GuildBoard : Item** — Item ("Local Guilds"). [Constructable, Flipable] (+2 types)
- `PatchBoard.cs` — **PatchBoard : Item** — Item ("The Changing World"). [Constructable, Flipable] (+1 type)
- `RulesBoard.cs` — **RulesBoard : Item** — Item ("Laws of the Land"). [Constructable, Flipable]
- `StatusBoard.cs` — **StatusBoard : Item** — Item ("Status Board"). [Constructable, Flipable] (+1 type)
- `StealingBoard.cs` — **StealingBoard : Item** — Item ("Stealing the Past"). [Constructable, Flipable]
- `WantedMangar.cs` — **WantedMangar : Item** — Item ("Wanted!"). [Constructable, Flipable] (+1 type)

#### Scripts/Items/Books/PowerScrolls — Items: Power scrolls. Types: PowerScroll, PowerGump.

- `PowerScroll.cs` — **PowerScroll : SpecialScroll** — Item.
- `PowerScrollBuy.cs` — **PowerGump : Gump** — Gump UI.

#### Scripts/Items/Books/Trades — Items: Trades. Types: LearnGraniteBook, LearnLeatherBook, LearnMetalBook, LearnMiscBook, ....

- `LearnGranite.cs` — **LearnGraniteBook : Item** — Item ("Sand & Stone Crafts"). [Constructable] (+1 type)
- `LearnLeather.cs` — **LearnLeatherBook : Item** — Item ("Leather & Bone Crafts"). [Constructable] (+1 type)
- `LearnMetal.cs` — **LearnMetalBook : Item** — Item ("Metal Smithing & Tinkering"). [Constructable] (+1 type)
- `LearnMisc.cs` — **LearnMiscBook : Item** — Item ("Skinning & Carving"). [Constructable] (+1 type)
- `LearnScales.cs` — **LearnScalesBook : Item** — Item ("Reptile Scale Crafts"). [Constructable] (+1 type)
- `LearnTailor.cs` — **LearnTailorBook : Item** — Item ("Tailoring the Cloth"). [Constructable] (+1 type)
- `LearnWood.cs` — **LearnWoodBook : Item** — Item ("Wooden Carvings"). [Constructable] (+1 type)

#### Scripts/Items/Clothing — Clothing, hats, shoes and wearable deco.

- `BaseClothing.cs` — **BaseClothing : Item** — Base clothing: name/properties, quality and wearables.
- `Cloaks.cs` — **BaseCloak : BaseClothing** — Clothing. [Constructable, Flipable] (+1 type)
- `Hats.cs` — **BaseHat : BaseClothing** — Clothing ("cloth hood"). [Constructable, Flipable] (+27 types)
- `LoinCloth.cs` — **LoinCloth : BaseWaist** — Clothing item ("loin cloth").
- `MiddleTorso.cs` — **BaseMiddleTorso : BaseClothing** — Clothing ("royal shirt"). [Constructable, Flipable] (+10 types)
- `OuterLegs.cs` — **BaseOuterLegs : BaseClothing** — Clothing ("royal skirt"). [Constructable, Flipable] (+5 types)
- `OuterTorso.cs` — **BaseOuterTorso : BaseClothing** — Clothing ("gilded dress"). [Constructable] (+11 types)
- `Pants.cs` — **BasePants : BaseClothing** — Clothing ("short pants"). [Constructable, Flipable] (+5 types)
- `ReaperHoods.cs` — **ReaperHood : BaseHat** — Hat ("reaper hood"). [Constructable] (+1 type)
- `Robes.cs` — **JokerRobe : BaseOuterTorso** — Clothing item ("jester coat").
- `RoyalCloak.cs` — **RoyalCape : BaseCloak** — Clothing item ("royal cloak").
- `Shirts.cs` — **BaseShirt : BaseClothing** — Clothing ("royal coat"). [Constructable, Flipable] (+10 types)
- `Shoes.cs` — **Shoes : BaseShoes** — Clothing item ("barbarian boots").
- `Waist.cs` — **BaseWaist : BaseClothing** — Clothing ("belt"). [Constructable, Flipable] (+3 types)

#### Scripts/Items/Clothing/Suits — Clothing items: Suits. Types: BaseSuit, DeathShroud, DupreSuit, LordBlackthorneSuit, ....

- `BaseSuit.cs` — **BaseSuit : Item** — Item.
- `DeathShroud.cs` — **DeathShroud : BaseSuit** — Clothing item.
- `DupreSuit.cs` — **DupreSuit : BaseSuit** — Clothing item.
- `LordBlackthorneSuit.cs` — **LordBlackthorneSuit : BaseSuit** — Clothing item.
- `LordBritishSuit.cs` — **LordBritishSuit : BaseSuit** — Clothing item.

#### Scripts/Items/Containers — Containers, loot packs, treasure/paragon/sunken chests, corpses.

- `AlchemistPouch.cs` — **AlchemistPouch : Bag** — Container item ("alchemist's belt pouch").
- `AlchemyPouch.cs` — **AlchemyPouch : LargeSack** — Container item ("alchemy rucksack").
- `AnimalCages.cs` — **BaseCaged : Item** — Item ("cage rabbit"). [Constructable] (+76 types)
- `BagOfHolding.cs` — **BagOfHolding : LargeSack** — Container item ("magical backpack").
- `BankChest.cs` — **BankChest : Item** — Item ("Home Bank Vault"). [Constructable, Flipable]
- `BankChestAny.cs` — **BankChestAny : Item** — Item ("Bank Vault"). [Constructable, Flipable]
- `BaseTreasureChest.cs` — **BaseTreasureChest : LockableContainer** — Container item.
- `BoneContainer.cs` — **BoneContainer : Bag** — Container item ("a strange pile of bones").
- `BuriedBody.cs` — **BuriedBody : LockableContainer** — Container item.
- `BuriedChest.cs` — **BuriedChest : LockableContainer** — Container item ("Chest").
- `Container.cs` — **BaseContainer : Container** — Container ("small hollow book"). (+91 types)
- `ContainerFunctions.cs` — **ContainerFunctions** — Container item ("Wooden Chest").
- `CorpseChest.cs` — **CorpseChest : LockableContainer** — Container item.
- `CorpseSailor.cs` — **CorpseSailor : LockableContainer** — Container item.
- `DungeonChest.cs` — **DungeonChest : LockableContainer** — ///////////////////////////////////////////////////////////////////////////////////////////////////////////... [Constructable] (+1 type)
- `DustPile.cs` — **DustPile : Item** — Item. [Constructable]
- `FoodChest.cs` — **FoodChest : Item** — Item ("Food Chest"). [Constructable, Flipable]
- `FurnitureContainer.cs` — **TallCabinet : BaseContainer** — ///////////////////////////////////////////////////////////////////////////////////// [Constructable, Flipable] (+22 types)
- `GraveChest.cs` — **GraveChest : SkullChest** — Container item ("graveyard chest").
- `GypsyShelf.cs` — **GypsyShelf : Item** — Item ("book shelf"). [Constructable]
- `HiddenBox.cs` — **HiddenBox : HiddenContainer** — Container item ("trap door").
- `HiddenChest.cs` — **HiddenChest : Item** — Item ("a hidden chest"). [Constructable]
- `InnChest.cs` — **InnChest : Item** — Item ("inn chest"). [Constructable, Flipable]
- `LandChest.cs` — **LandChest : LockableContainer** — Container item ("Corpse").
- `LockableContainer.cs` — **LockableContainer : TrapableContainer** — Container item.
- `Loot.cs` — **Loot** — Container item.
- `LootBag.cs` — **LootBag : LockableContainer** — Container item ("chest").
- `LootChest.cs` — **LootChest : LockableContainer** — Container item ("chest").
- `LootPack.cs` — **LootPack** — Loot generation: pack tiers, loot drops, relic/artifact placement.
- `MovingBox.cs` — **MovingBox : LargeCrate** — Container item ("housing crate").
- `ParagonChest.cs` — **ParagonChest : LockableContainer** — Container item ("cursed chest").
- `PirateChest.cs` — **PirateChest : SkullChest** — Container item ("pirate chest").
- `SackOfHolding.cs` — **SackOfHolding : LargeSack** — Container item ("pack of holding").
- `Safe.cs` — **Safe : Item** — Item ("safe"). [Constructable, Flipable]
- `Shelves.cs` — **SailorShelf : BaseContainer** — Container ("shelf"). [Constructable, Flipable] (+184 types)
- `Strongbox.cs` — **StrongBox : BaseContainer** — Container. [Flipable]
- `SunkenBag.cs` — **SunkenBag : LockableContainer** — Container item ("bag").
- `SunkenChest.cs` — **SunkenChest : LockableContainer** — Container item ("sunken chest").
- `SunkenShip.cs` — **SunkenShip : LockableContainer** — Container item ("sunken ship").
- `TrapableContainer.cs` — **TrapableContainer : BaseContainer** — Container. (+2 types)
- `TreasureChest.cs` — **WoodenTreasureChest : BaseTreasureChest** — Container item.
- `TreasureMapChest.cs` — **TreasureMapChest : LockableContainer** — Container item ("Treasure Chest").
- `WaterChest.cs` — **WaterChest : LockableContainer** — Container item ("Boat").
- `WeightReductionContainer.cs` — **WeightReductionContainer : Container** — Container ("bag of holding"). [Constructable] (+3 types)

#### Scripts/Items/Deeds — Items: Deeds. Types: BarkeepContract, ClothingBlessDeed, CommodityDeed, DragonBardingDeed, ....

- `BarkeepContract.cs` — **BarkeepContract : Item** — Item. [Constructable]
- `ClothingBlessDeed.cs` — **ClothingBlessDeed : Item // Create the item class which is derived from the base item class** — Item.
- `CommodityDeed.cs` — **CommodityDeed : Item** — Item. [Constructable] (+1 type)
- `DragonBardingDeed.cs` — **DragonBardingDeed : Item** — Item. [TypeAlias]
- `HairRestylingDeed.cs` — **HairRestylingDeed : Item** — Item. [Constructable] (+1 type)
- `NameChangeDeed.cs` — **NameChangeDeed : Item** — Item. [Constructable]
- `VendorRentalContract.cs` — **VendorRentalContract : Item** — Item. [Constructable] (+3 types)

#### Scripts/Items/Explorers — Items: Explorers. Types: Bedroll, BedrolledOut, CamperTentEffect, Campfire, ....

- `Bedroll.cs` — **Bedroll : Item** — Item. [Constructable, Flipable]
- `BedrolledOut.cs` — **BedrolledOut : Item** — Item.
- `CamperTent.cs` — **CamperTentEffect** — Enum with about 1 members. [Constructable] (+3 types)
- `Campfire.cs` — **Campfire : Item** — Item. (+1 type)
- `Kindling.cs` — **Kindling : Item** — Item. [Constructable]
- `SmallTent.cs` — **SmallTent : Item** — Item ("small tent"). [Constructable] (+3 types)
- `Spyglass.cs` — **Spyglass : Item** — Using these binoculars will give you a bonus to tracking for a short time, allowing you to search for your... [Constructable] (+2 types)
- `StableStone.cs` — **StableStone : Item** — Item ("hitching post"). [Constructable] (+5 types)

#### Scripts/Items/Food — Items: Food. Types: Wasabi, BakedBread, BeverageType, BloodyDrink, ....

- `Asian.cs` — **Wasabi : Item** — Item. [Constructable] (+11 types)
- `BakedBread.cs` — **BakedBread : Item** — Item ("bread"). [Constructable]
- `Beverage.cs` — **BeverageType** — Enum with about 6 members. [Constructable, TypeAlias] (+16 types)
- `BloodDrink.cs` — **BloodyDrink : Item** — Item ("fresh blood"). [Constructable]
- `Bowls.cs` — **EmptyWoodenBowl : Item** — Item. [Constructable] (+14 types)
- `CookableFood.cs` — **CookableFood : Item** — Item ("raw pig"). (+22 types)
- `Cooking.cs` — **UtilityItem** — Food/drink item.
- `Food.cs` — **Food : Item** — Item ("beef jerky"). (+43 types)
- `FreshBrain.cs` — **FreshBrain : Item** — Item ("fresh brain"). [Constructable]
- `Fruits.cs` — **FruitBasket : Food** — Food/drink item.
- `Hunger.cs` — **MyHunger** — Commands: [mhgr, [myhunger
- `TastyHeart.cs` — **TastyHeart : Item** — Item. [Constructable]
- `Vegetables.cs` — **Carrot : Food** — Food/drink item ("large pumpkin").

#### Scripts/Items/Food/Farming — Items: Farming. Types: FarmableCabbage, FarmableCarrot, FarmableCorn, FarmableCotton, ....

- `FarmableCabbage.cs` — **FarmableCabbage : FarmableCrop** — Food/drink item ("cabbage").
- `FarmableCarrot.cs` — **FarmableCarrot : FarmableCrop** — Food/drink item ("carrot").
- `FarmableCorn.cs` — **FarmableCorn : FarmableCrop** — Food/drink item ("corn").
- `FarmableCotton.cs` — **FarmableCotton : FarmableCrop** — Food/drink item ("cotton").
- `FarmableCrop.cs` — **FarmableCrop : Item** — Item.
- `FarmableFlax.cs` — **FarmableFlax : FarmableCrop** — Food/drink item ("flax").
- `FarmableLettuce.cs` — **FarmableLettuce : FarmableCrop** — Food/drink item ("lettuce").
- `FarmableOnion.cs` — **FarmableOnion : FarmableCrop** — Food/drink item ("onion").
- `FarmablePumpkin.cs` — **FarmablePumpkin : FarmableCrop** — Food/drink item ("pumpkin").
- `FarmableTomato.cs` — **FarmableTomato : FarmableCrop** — Food/drink item ("tomato").
- `FarmableTurnip.cs` — **FarmableTurnip : FarmableCrop** — Food/drink item ("turnip").
- `FarmableWatermelon.cs` — **FarmableWatermelon : FarmableCrop** — Food/drink item ("watermelon").
- `FarmableWheat.cs` — **FarmableWheat : FarmableCrop** — Food/drink item ("wheat").

#### Scripts/Items/Games — Items: Games. Types: Backgammon, BaseBoard, BasePiece, CEOBlackJack, ....

- `Backgammon.cs` — **Backgammon : BaseBoard** — Item.
- `BaseBoard.cs` — **BaseBoard : Container** — Container. (+1 type)
- `BasePiece.cs` — **BasePiece : Item** — Item.
- `BlackJack.cs` — **CEOBlackJack : Item** — Item. [Flipable] (+12 types)
- `CardDeck.cs` — **CardDeck** — Item.
- `CasinoToken.cs` — **CasinoToken : Item   // Free plays on Casino games that are aware of Tokens** — Item ("Casino Token").
- `CheckerBoard.cs` — **CheckerBoard : BaseBoard** — Item.
- `CheckersPieces.cs` — **PieceWhiteChecker : BasePiece** — Item.
- `ChessPieces.cs` — **PieceWhiteKing : BasePiece** — Item.
- `Chessboard.cs` — **Chessboard : BaseBoard** — Item.
- `Dices.cs` — **Dices : Item** — Item. [Constructable]
- `HiLoCards.cs` — **HiLoCards : Item** — Item ("High-Low Card Game"). [Flipable] (+2 types)
- `PuzzleCube.cs` — **PuzzleCube : Item** — Item ("puzzle cube"). [Constructable]
- `Tarot.cs` — **TarotDeck : Item** — Item ("tarot deck"). [Constructable, Flipable] (+2 types)
- `TarotPoker.cs` — **tarotpoker : Item** — Item ("Deck of Tarot Poker Cards"). [Constructable, Flipable] (+1 type)

#### Scripts/Items/Games/DandD — Items: Dand d. Types: Dice10, Dice12, Dice20, Dice4, ....

- `Dice10.cs` — **Dice10 : Item** — Item ("dice"). [Constructable]
- `Dice12.cs` — **Dice12 : Item** — Item ("dice"). [Constructable]
- `Dice20.cs` — **Dice20 : Item** — Item ("dice"). [Constructable]
- `Dice4.cs` — **Dice4 : Item** — Item ("dice"). [Constructable]
- `Dice6.cs` — **Dice6 : Item** — Item ("dice"). [Constructable]
- `Dice8.cs` — **Dice8 : Item** — Item ("dice"). [Constructable]
- `DungeonMastersGuide.cs` — **DungeonMastersGuide : Item** — Item ("Dungeon Masters Guide"). [Constructable] (+1 type)
- `MonsterManual.cs` — **MonsterManual : Item** — Item ("Monster Manual"). [Constructable] (+1 type)
- `PlayersHandbook.cs` — **PlayersHandbook : Item** — Item ("Players Handbook"). [Constructable] (+1 type)

#### Scripts/Items/Games/LiarsDice — Items: Liars dice. Types: CallBluffGump, MobileDiceStstatus, DiceCommand, ExitDiceGump, ....

- `CallBluffGump.cs` — **CallBluffGump : Gump** — Gump UI.
- `DiceChannel.cs` — **MobileDiceStstatus** — Item.
- `DiceCommand.cs` — **DiceCommand** — Commands: [mex
- `ExitDiceGump.cs` — **ExitDiceGump : Gump** — Gump UI.
- `GameDiceGump.cs` — **GameDiceGump : Gump** — Gump UI.
- `LiarsDice.cs` — **LiarsDice : Item** — Item ("Liar's Dice Game"). [Constructable]
- `NewDiceGameGump.cs` — **NewDiceGameGump : Gump** — Gump UI.
- `StatusDiceGump.cs` — **StatusDiceGump : Gump** — Gump UI.

#### Scripts/Items/Games/Mahjong — Items: Mahjong. Types: MahjongDealerIndicator, MahjongDices, MahjongPieceDirection, MahjongGame, ....

- `MahjongDealerIndicator.cs` — **MahjongDealerIndicator** — Item.
- `MahjongDices.cs` — **MahjongDices** — Item.
- `MahjongEnums.cs` — **MahjongPieceDirection** — Enum with about 4 members. (+2 types)
- `MahjongGame.cs` — **MahjongGame : Item** — Item. [Constructable] (+1 type)
- `MahjongPacketHandlers.cs` — **MahjongPacketHandlers** — Item.
- `MahjongPieceDim.cs` — **MahjongPieceDim** — Item.
- `MahjongPlayers.cs` — **MahjongPlayers** — Item.
- `MahjongTile.cs` — **MahjongTile** — Item.
- `MahjongTileTypeGenerator.cs` — **MahjongTileTypeGenerator** — Item.
- `MahjongWallBreakIndicator.cs` — **MahjongWallBreakIndicator** — Item.
- `Packets.cs` — **MahjongJoinGame : Packet** — Network packet. (+5 types)

#### Scripts/Items/Gems — Items: Gems. Types: Amber, Amethyst, Citrine, Crystals, ....

- `Amber.cs` — **Amber : Item** — Item. [Constructable]
- `Amethyst.cs` — **Amethyst : Item** — Item. [Constructable]
- `Citrine.cs` — **Citrine : Item** — Item. [Constructable]
- `Crystals.cs` — **Crystals : Item** — Item ("crystals"). [Constructable]
- `Diamond.cs` — **Diamond : Item** — Item. [Constructable]
- `Emerald.cs` — **Emerald : Item** — Item. [Constructable]
- `LargeCrystal.cs` — **LargeCrystal : Item** — Item ("crystal"). [Constructable]
- `Oyster.cs` — **Oyster : Item** — Item ("pearl"). [Constructable]
- `Ruby.cs` — **Ruby : Item** — Item. [Constructable]
- `Sapphire.cs` — **Sapphire : Item** — Item. [Constructable]
- `StarSapphire.cs` — **StarSapphire : Item** — Item. [Constructable]
- `Tourmaline.cs` — **Tourmaline : Item** — Item. [Constructable]

#### Scripts/Items/Houses — Housing, addons, decoration, remodeling and house tools.

- `AdvertiserVendor.cs` — **AdvertiserVendor : Item** — Item ("The Merchant Advertiser"). [Constructable, Flipable] (+1 type)
- `BaseHouse.cs` — **BaseHouse : BaseMulti** — Multi. (+18 types)
- `CircusTentsEast.cs` — **MyCircusTentEastAddon : BaseAddon** — House addon ("Circus Tent (East Door)"). [Constructable] (+2 types)
- `CircusTentsSouth.cs` — **MyCircusTentSouthAddon : BaseAddon** — House addon ("Circus Tent (South Door)"). [Constructable] (+2 types)
- `ComponentVerification.cs` — **ComponentVerification** — Item.
- `Deeds.cs` — **HousePlacementTarget : MultiTarget** — Item.
- `GypsyCampAddon.cs` — **GypsyCampAddon : BaseAddon** — House addon ("GypsyCamp"). (+1 type)
- `HouseFoundation.cs` — **HouseFoundation : BaseHouse** — Commands: [DesignInsert (+10 types)
- `HousePlacement.cs` — **HousePlacement** — Item.
- `HousePlacementTool.cs` — **HousePlacementTool : Item** — Item ("construction contract"). [Constructable] (+4 types)
- `HouseSign.cs` — **HouseSign : Item** — Item. (+2 types)
- `HouseTeleporter.cs` — **HouseTeleporter : Item** — Item. [Constructable] (+2 types)
- `Houses.cs` — **SmallOldHouse : BaseHouse** — House. (+12 types)
- `InteriorDecorator.cs` — **InteriorDecorator : Item** — Item ("Homeowner Tools"). [Constructable] (+3 types)
- `MagicalRope.cs` — **MagicalRope** — Source file (no top-level type declaration detected).
- `Mailbox.cs` — **Mailbox : BaseContainer** — Container. [Constructable, Flipable]
- `MovingCrate.cs` — **MovingCrate : Container** — Container. (+2 types)
- `NewHouses.cs` — **Pyramid : BaseHouse** — /////////////////////////////////////////////////////////////////////////////////////////// (+48 types)
- `PlayersHouseTeleporter.cs` — **PlayersHouseTeleporter : Item** — Item ("House Teleporter"). [Constructable]
- `PlayersZTeleporter.cs` — **PlayersZTeleporter : Item** — Item ("House High Teleporter"). [Constructable]
- `PreviewHouse.cs` — **PreviewHouse : BaseMulti** — Multi. (+1 type)
- `SmallTents.cs` — **BlueTentDeed : HouseDeed** — Item.
- `TavernTable.cs` — **TavernTable : Item** — Item ("tavern table"). [Constructable, Flipable] (+1 type)
- `TentsEast.cs` — **MyTentEastAddon : BaseAddon** — House addon ("Tent (East Door)"). [Constructable] (+1 type)
- `TentsSouth.cs` — **MyTentSouthAddon : BaseAddon** — House addon ("Tent (South Door)"). [Constructable] (+1 type)

#### Scripts/Items/Houses/Carpets — Items: Carpets. Types: MagicCarpetA, MagicCarpetB, MagicCarpetC, MagicCarpetD, ....

- `MagicCarpetA.cs` — **MagicCarpetA : BaseBoat** — Boat ("magic carpet"). [Constructable] (+2 types)
- `MagicCarpetB.cs` — **MagicCarpetB : BaseBoat** — Boat ("magic carpet"). [Constructable] (+2 types)
- `MagicCarpetC.cs` — **MagicCarpetC : BaseBoat** — Boat ("magic carpet"). [Constructable] (+2 types)
- `MagicCarpetD.cs` — **MagicCarpetD : BaseBoat** — Boat ("magic carpet"). [Constructable] (+2 types)
- `MagicCarpetE.cs` — **MagicCarpetE : BaseBoat** — Boat ("magic carpet"). [Constructable] (+2 types)
- `MagicCarpetF.cs` — **MagicCarpetF : BaseBoat** — Boat ("magic carpet"). [Constructable] (+2 types)
- `MagicCarpetG.cs` — **MagicCarpetG : BaseBoat** — Boat ("magic carpet"). [Constructable] (+2 types)
- `MagicCarpetH.cs` — **MagicCarpetH : BaseBoat** — Boat ("magic carpet"). [Constructable] (+2 types)
- `MagicCarpetI.cs` — **MagicCarpetI : BaseBoat** — Boat ("magic carpet"). [Constructable] (+2 types)

#### Scripts/Items/Houses/Construction — Items: Construction. Types: Ankhs, MerchantWagonSouth.

- `Ankhs.cs` — **Ankhs** — Item.
- `MerchantWagon.cs` — **MerchantWagonSouth : BaseAddon** — House addon. (+1 type)

#### Scripts/Items/Houses/Construction/Addons — Items: Addons. Types: AbbatoirAddon, AddonComponent, AddonContainerComponent, AlchemistTableEastAddon, ....

- `AbbatoirAddon.cs` — **AbbatoirAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `AddonComponent.cs` — **AddonComponent : Item** — Item. [Constructable] (+4 types)
- `AddonContainerComponent.cs` — **AddonContainerComponent : Item** — Item. [Constructable] (+1 type)
- `AlchemistTableEastAddon.cs` — **AlchemistTableEastAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `AlchemistTableSouthAddon.cs` — **AlchemistTableSouthAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `AnvilEastAddon.cs` — **AnvilEastAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `AnvilSouthAddon.cs` — **AnvilSouthAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `ArcaneBookshelfEastAddon.cs` — **ArcaneBookshelfEastAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `ArcaneBookshelfSouthAddon.cs` — **ArcaneBookshelfSouthAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `ArcaneCircleAddon.cs` — **ArcaneCircleAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `ArcanistStatueEastAddon.cs` — **ArcanistStatueEastAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `ArcanistStatueSouthAddon.cs` — **ArcanistStatueSouthAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `ArcheryButteAddon.cs` — **ArcheryButteAddon : BaseAddon** — House addon. [Constructable, Flipable] (+3 types)
- `BallotBox.cs` — **BallotBox : AddonComponent** — Item.
- `BaseAddon.cs` — **BaseAddon : Item** — Item. (+2 types)
- `BaseAddonContainer.cs` — **BaseAddonContainer : BaseContainer** — Container.
- `BaseAddonContainerDeed.cs` — **BaseAddonContainerDeed : Item** — Item. [Flipable] (+1 type)
- `BaseAddonDeed.cs` — **BaseAddonDeed : Item** — Item. [Flipable] (+1 type)
- `BearRugs.cs` — **BrownBearRugEastAddon : BaseAddon** — House addon. [Constructable] (+7 types)
- `BloodPentagram.cs` — **BloodPentagram : BaseAddon** — House addon. [Constructable]
- `DartBoard.cs` — **DartBoard : AddonComponent** — Item.
- `ElvenBedEastAddon.cs` — **ElvenBedEastAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `ElvenBedSouthAddon.cs` — **ElvenBedSouthAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `ElvenDresserEastAddon.cs` — **ElvenDresserEastAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `ElvenDresserSouthAddon.cs` — **ElvenDresserSouthAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `ElvenForgeAddon.cs` — **ElvenForgeAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `ElvenLoveseatEastAddon.cs` — **ElvenLoveseatEastAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `ElvenLoveseatSouthAddon.cs` — **ElvenLoveseatSouthAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `ElvenSpinningwheelEastAddon.cs` — **ElvenSpinningwheelEastAddon : BaseAddon** — House addon. [Constructable] (+2 types)
- `ElvenSpinningwheelSouthAddon.cs` — **ElvenSpinningwheelSouthAddon : BaseAddon** — House addon. [Constructable] (+2 types)
- `ElvenStoveEastAddon.cs` — **ElvenStoveEastAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `ElvenStoveSouthAddon.cs` — **ElvenStoveSouthAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `ElvenWashbasinEastAddon.cs` — **ElvenWashBasinEastAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `ElvenWashbasinSouthAddon.cs` — **ElvenWashBasinSouthAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `FancyElvenTableEastAddon.cs` — **FancyElvenTableEastAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `FancyElvenTableSouthAddon.cs` — **FancyElvenTableSouthAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `FireColumnAddon.cs` — **FireColumnAddon : BaseAddon** — House addon. [Constructable]
- `FlourMillEastAddon.cs` — **FlourMillEastAddon : BaseAddon** — House addon. [Constructable] (+3 types)
- `FlourMillSouthAddon.cs` — **FlourMillSouthAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `FlowerTapestries.cs` — **LightFlowerTapestryEastAddon : BaseAddon** — House addon. [Constructable] (+7 types)
- `GiantWebs.cs` — **GiantWeb1 : BaseAddon** — House addon. [Constructable] (+5 types)
- `GozaMats.cs` — **GozaMatEastAddon : BaseAddon** — House addon. [Constructable] (+15 types)
- `GrayBrickFireplaceEastAddon.cs` — **GrayBrickFireplaceEastAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `GrayBrickFireplaceSouthAddon.cs` — **GrayBrickFireplaceSouthAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `JackOLantern.cs` — **JackOLantern : BaseAddon** — House addon ("jack-o-latern"). [Constructable]
- `LargeBedEastAddon.cs` — **LargeBedEastAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `LargeBedSouthAddon.cs` — **LargeBedSouthAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `LargeForgeEastAddon.cs` — **LargeForgeEastAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `LargeForgeSouthAddon.cs` — **LargeForgeSouthAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `LargeStoneTableEastAddon.cs` — **LargeStoneTableEastAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `LargeStoneTableSouthAddon.cs` — **LargeStoneTableSouthAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `LoomEastAddon.cs` — **LoomEastAddon : BaseAddon** — House addon. [Constructable] (+2 types)
- `LoomSouthAddon.cs` — **LoomSouthAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `MediumStoneTableEastAddon.cs` — **MediumStoneTableEastAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `MediumStoneTableSouthAddon.cs` — **MediumStoneTableSouthAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `OrnateElvenTableEastAddon.cs` — **OrnateElvenTableEastAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `OrnateElvenTableSouthAddon.cs` — **OrnateElvenTableSouthAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `ParrotPerchAddon.cs` — **ParrotPerchAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `PentagramAddon.cs` — **PentagramAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `PickpocketDips.cs` — **PickpocketDip : AddonComponent** — Item.
- `PyramidAddon.cs` — **PyramidAddon : BaseAddon** — House addon. [Constructable]
- `RejuvinationAnkhs.cs` — **RejuvinationAddonComponent : AddonComponent** — Item.
- `SHTeleporter.cs` — **SHTeleporter : BaseAddon** — Commands: [SHTelGen [Constructable] (+2 types)
- `SandstoneFireplaceEastAddon.cs` — **SandstoneFireplaceEastAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `SandstoneFireplaceSouthAddon.cs` — **SandstoneFireplaceSouthAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `SandstoneFountainAddon.cs` — **SandstoneFountainAddon : BaseAddon** — House addon. [Constructable]
- `SerpentPillarAddon.cs` — **SerpentPillarAddon : BaseAddon** — House addon. [Constructable]
- `ShrineOfWisdomAddon.cs` — **ShrineOfWisdomAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `SkullPileAddon.cs` — **SkullPileAddon : BaseAddon** — House addon. [Constructable]
- `SmallBedEastAddon.cs` — **SmallBedEastAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `SmallBedSouthAddon.cs` — **SmallBedSouthAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `SmallForgeAddon.cs` — **SmallForgeAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `SpinningwheelEastAddon.cs` — **SpinningwheelEastAddon : BaseAddon** — House addon. [Constructable] (+3 types)
- `SpinningwheelSouthAddon.cs` — **SpinningwheelSouthAddon : BaseAddon** — House addon. [Constructable] (+2 types)
- `SquirrelStatueEastAddon.cs` — **SquirrelStatueEastAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `SquirrelStatueSouthAddon.cs` — **SquirrelStatueSouthAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `StoneFireplaceEastAddon.cs` — **StoneFireplaceEastAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `StoneFireplaceSouthAddon.cs` — **StoneFireplaceSouthAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `StoneFountainAddon.cs` — **StoneFountainAddon : BaseAddon** — House addon. [Constructable]
- `StoneOvenEastAddon.cs` — **StoneOvenEastAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `StoneOvenSouthAddon.cs` — **StoneOvenSouthAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `StretchedHides.cs` — **SmallStretchedHideEastAddon : BaseAddon** — House addon. [Constructable] (+7 types)
- `TallElvenBedEastAddon.cs` — **TallElvenBedEastAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `TallElvenBedSouthAddon.cs` — **TallElvenBedSouthAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `Telescope.cs` — **Telescope : BaseAddon** — House addon. [Constructable]
- `TigerRug.cs` — **TigerRugEastAddon : BaseAddon** — House addon ("tiger rug (east)"). [Constructable] (+7 types)
- `TrainingDaemons.cs` — **TrainingDaemon : AddonComponent** — Item ("training daemon").
- `TrainingDummies.cs` — **TrainingDummy : AddonComponent** — Item.
- `WagonAddon.cs` — **WagonAddon : BaseAddon** — House addon ("wagon deed"). (+1 type)
- `WarriorStatueEastAddon.cs` — **WarriorStatueEastAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `WarriorStatueSouthAddon.cs` — **WarriorStatueSouthAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `WaterTroughEastAddon.cs` — **WaterTroughEastAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `WaterTroughSouthAddon.cs` — **WaterTroughSouthAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `WaterVat.cs` — **WaterVatEast : BaseAddon** — House addon. [Constructable] (+1 type)

#### Scripts/Items/Houses/Construction/Chairs — Items: Chairs. Types: WoodenBench, FancyWoodenChairCushion, Stool, Throne.

- `Benchs.cs` — **WoodenBench : Item** — Item. [Constructable, Flipable]
- `Chairs.cs` — **FancyWoodenChairCushion : Item** — Item. [Constructable, Flipable] (+7 types)
- `Stools.cs` — **Stool : Item** — Item. [Constructable] (+1 type)
- `Thrones.cs` — **Throne : Item** — Item ("ornate throne"). [Constructable, Flipable] (+1 type)

#### Scripts/Items/Houses/Construction/Decorative — Items: Decorative. Types: DecorativeShield1, DecorativeBowWest, LargePainting, Tapestry1N.

- `DecorativeShield.cs` — **DecorativeShield1 : Item** — Item. [Constructable, Flipable] (+14 types)
- `DecorativeWeapon.cs` — **DecorativeBowWest : Item** — Item. [Constructable, Flipable] (+11 types)
- `PaintingPortraits.cs` — **LargePainting : Item** — Item. [Constructable, Flipable] (+6 types)
- `Tapestry.cs` — **Tapestry1N : Item** — Item. [Constructable] (+21 types)

#### Scripts/Items/Houses/Construction/Floors — Items: Floors. Types: BaseFloor.

- `Floors.cs` — **BaseFloor : Item** — Item. [Constructable] (+23 types)

#### Scripts/Items/Houses/Construction/Lights — Items: Lights. Types: BaseEquipableLight, BaseLight, Brazier, BrazierTall, ....

- `BaseEquipableLight.cs` — **BaseEquipableLight : BaseLight** — Light source. [Constructable]
- `BaseLight.cs` — **BaseLight : Item** — Item. [Constructable] (+1 type)
- `Brazier.cs` — **Brazier : BaseLight** — Light source. [Constructable]
- `BrazierTall.cs` — **BrazierTall : BaseLight** — Light source. [Constructable]
- `BurningScarecrow.cs` — **BurningScarecrowA : BaseLight** — Light source ("burning scarecrow"). [Constructable] (+1 type)
- `Candelabra.cs` — **Candelabra : BaseLight** — Light source ("candelabra"). [Constructable]
- `CandelabraStand.cs` — **CandelabraStand : BaseLight** — Light source ("candelabra"). [Constructable]
- `Candle.cs` — **Candle : BaseEquipableLight** — Item.
- `CandleLarge.cs` — **CandleLarge : BaseLight** — Light source ("candle"). [Constructable]
- `CandleLong.cs` — **CandleLong : BaseLight** — Light source. [Constructable]
- `CandleShort.cs` — **CandleShort : BaseLight** — Light source. [Constructable]
- `CandleSkull.cs` — **CandleSkull : BaseLight** — Light source. [Constructable]
- `ColoredWallTorch.cs` — **ColoredWallTorch : BaseLight** — Light source ("wall torch"). [Constructable, Flipable]
- `DarkSource.cs` — **DarkSource : Item** — Item. [Constructable]
- `DragonLamp.cs` — **DragonLamp : BaseLight** — Light source ("dragon lamp"). [Constructable]
- `GlowingLight.cs` — **GlowingLight : Item** — Item ("glowing light"). [Constructable]
- `GothicCandelabra.cs` — **GothicCandelabraA : BaseLight** — Light source ("gothic candelabra"). [Constructable] (+1 type)
- `HangingLantern.cs` — **HangingLantern : BaseLight** — Light source. [Constructable]
- `HeatingStand.cs` — **HeatingStand : BaseLight** — Light source. [Constructable]
- `LampPost1.cs` — **LampPost1 : BaseLight** — Light source. [Constructable]
- `LampPost2.cs` — **LampPost2 : BaseLight** — Light source. [Constructable]
- `LampPost3.cs` — **LampPost3 : BaseLight** — Light source. [Constructable]
- `Lantern.cs` — **Lantern : BaseEquipableLight** — Item.
- `LightSource.cs` — **LightSource : Item** — Item. [Constructable] (+2 types)
- `PaperLantern.cs` — **PaperLantern : BaseLight** — Light source. [Constructable, Flipable]
- `RedHangingLantern.cs` — **RedHangingLantern : BaseLight** — Light source. [Constructable, Flipable]
- `RoundPaperLantern.cs` — **RoundPaperLantern : BaseLight** — Light source. [Constructable, Flipable]
- `ShojiLantern.cs` — **ShojiLantern : BaseLight** — Light source. [Constructable, Flipable]
- `StrangeGlow.cs` — **StrangeGlow : Item** — Item ("strange glow"). [Constructable] (+1 type)
- `Torch.cs` — **Torch : BaseEquipableLight** — Item.
- `TorchAndDragonBrazier.cs` — **light_dragon_brazier : BaseLight** — Light source ("dragon brazier"). [Constructable, Flipable] (+1 type)
- `TowerLantern.cs` — **TowerLantern : BaseLight** — Light source ("tower lantern"). [Constructable]
- `WallSconce.cs` — **WallSconce : BaseLight** — Light source. [Constructable, Flipable]
- `WallTorch.cs` — **WallTorch : BaseLight** — Light source. [Constructable, Flipable]
- `WhiteHangingLantern.cs` — **WhiteHangingLantern : BaseLight** — Light source. [Constructable, Flipable]

#### Scripts/Items/Houses/Construction/Misc — Items: Misc. Types: BarrelLid, Easle, MeltedWax, TallMusicStand, ....

- `BarrelParts.cs` — **BarrelLid : Item** — Item. [Constructable, Flipable] (+3 types)
- `Easle.cs` — **Easle : Item** — Item. [Constructable, Flipable]
- `MeltedWax.cs` — **MeltedWax : Item** — Item. [Constructable]
- `MusicStand.cs` — **TallMusicStand : Item** — Item. [Constructable, Flipable] (+1 type)
- `Obelisk.cs` — **Obelisk : Item** — Item. [Constructable]
- `Screens.cs` — **BambooScreen : Item** — Item. [Constructable, Flipable] (+1 type)
- `Statues.cs` — **StatueSouth : Item** — Item. [Constructable] (+11 types)
- `Vase.cs` — **Vase : Item** — Item. [Constructable] (+2 types)
- `Vines.cs` — **Vines : Item** — Item. [Constructable]

#### Scripts/Items/Houses/Construction/Ruined — Items: Ruined. Types: RuinedFallenChairA.

- `RuinedItemSingle.cs` — **RuinedFallenChairA : Item** — Item. [Constructable, Flipable] (+10 types)

#### Scripts/Items/Houses/Construction/Signs — Items: Signs. Types: BaseSign, LocalizedSign, Sign.

- `BaseSign.cs` — **BaseSign : Item** — Item.
- `LocalizedSign.cs` — **LocalizedSign : Sign** — Item.
- `Sign.cs` — **Sign : BaseSign** — Item.

#### Scripts/Items/Houses/Construction/Tables — Items: Tables. Types: ElegantLowTable, WritingTable.

- `Tables.cs` — **ElegantLowTable : Item** — Item. [Constructable, Flipable] (+4 types)
- `WritingTable.cs` — **WritingTable : Item** — Item. [Constructable, Flipable]

#### Scripts/Items/Houses/Construction/Treasure Piles — Items: Treasure piles. Types: TreasurePile01Addon, TreasurePile02Addon, TreasurePile03Addon, TreasurePile04Addon, ....

- `TreasurePile01Addon.cs` — **TreasurePile01Addon : BaseAddon** — House addon ("Chest of Decorative Treasure"). [Constructable] (+1 type)
- `TreasurePile02Addon.cs` — **TreasurePile02Addon : BaseAddon** — House addon ("Chest of Decorative Treasure"). [Constructable] (+1 type)
- `TreasurePile03Addon.cs` — **TreasurePile03Addon : BaseAddon** — House addon ("Chest of Decorative Treasure"). [Constructable] (+1 type)
- `TreasurePile04Addon.cs` — **TreasurePile04Addon : BaseAddon** — House addon ("Chest of Decorative Treasure"). [Constructable] (+1 type)
- `TreasurePile05Addon.cs` — **TreasurePile05Addon : BaseAddon** — House addon ("Chest of Decorative Treasure"). [Constructable] (+1 type)
- `TreasurePile2Addon.cs` — **TreasurePile2Addon : BaseAddon** — House addon ("Chest of Decorative Treasure"). [Constructable] (+1 type)
- `TreasurePile3Addon.cs` — **TreasurePile3Addon : BaseAddon** — House addon ("Chest of Decorative Treasure"). [Constructable] (+1 type)
- `TreasurePileAddon.cs` — **TreasurePileAddon : BaseAddon** — House addon ("Chest of Decorative Treasure"). [Constructable] (+1 type)

#### Scripts/Items/Houses/Construction/Trees — Items: Trees. Types: DarkBrownTreeAddon, GreyTreeAddon, LightBrownTreeAddon, PearTreeAddon.

- `DarkBrownTreeAddon.cs` — **DarkBrownTreeAddon : BaseAddon** — House addon ("Dark Brown Tree"). [Constructable] (+1 type)
- `GreyTreeAddon.cs` — **GreyTreeAddon : BaseAddon** — House addon ("Grey Tree"). [Constructable] (+1 type)
- `LightBrownTreeAddon.cs` — **LightBrownTreeAddon : BaseAddon** — House addon ("Light Brown Tree"). [Constructable] (+1 type)
- `PearTreeAddon.cs` — **PearTreeAddon : BaseFruitTreeAddon** — Item.

#### Scripts/Items/Houses/Construction/Walls — Items: Walls. Types: BaseWall, DarkWoodWall, ThickGrayStoneWall, ThinBrickWall, ....

- `BaseWall.cs` — **BaseWall : Item** — Item.
- `DarkWoodWall.cs` — **DarkWoodWall : BaseWall** — Item.
- `ThickGrayStoneWall.cs` — **ThickGrayStoneWall : BaseWall** — Item.
- `ThinBrickWall.cs` — **ThinBrickWall : BaseWall** — Item.
- `ThinStoneWall.cs` — **ThinStoneWall : BaseWall** — Item.
- `WhiteStoneWall.cs` — **WhiteStoneWall : BaseWall** — Item.

#### Scripts/Items/Houses/Construction/Wells — Items: Wells. Types: BlackWellAddon, BrownWellAddon, MarbleWellAddon, RedWellAddon, ....

- `blackwell.cs` — **BlackWellAddon : BaseAddon** — House addon ("Well Digging Spade"). [Constructable] (+2 types)
- `brownwell.cs` — **BrownWellAddon : BaseAddon** — House addon ("Well Digging Spade"). [Constructable] (+2 types)
- `marblewell.cs` — **MarbleWellAddon : BaseAddon** — House addon ("Well Digging Spade"). [Constructable] (+2 types)
- `redwell.cs` — **RedWellAddon : BaseAddon** — House addon ("Well Digging Spade"). [Constructable] (+2 types)
- `stonewell.cs` — **StoneWellAddon : BaseAddon** — House addon ("Well Digging Spade"). [Constructable] (+2 types)
- `woodwell.cs` — **WoodWellAddon : BaseAddon** — House addon ("Well Digging Spade"). [Constructable] (+2 types)

#### Scripts/Items/Houses/Decorations — Items: Decorations. Types: DecoStatueDeed, EvilItems, GwennoGraveAddon, house_sign_sign_post_a, ....

- `DecoIngotDeed.cs` — **DecoStatueDeed : Item** — Item. [Constructable]
- `EvilItems.cs` — **EvilItems : Item** — Item ("stuffed nightmare"). [Constructable, Flipable] (+3 types)
- `GwennoGraveAddon.cs` — **GwennoGraveAddon : BaseAddon** — House addon ("Gwenno Monument Deed"). [Constructable] (+1 type)
- `HouseSign.cs` — **house_sign_sign_post_a : Item** — ///////////////////////////////////////////////////////////////////// [Constructable, Flipable] (+45 types)
- `MonsterStatueDeed.cs` — **MonsterStatueDeed : Item** — Item. [Constructable]
- `OrientalItems.cs` — **OrientalItems : Item** — Item ("zen rock garden"). [Constructable]
- `Pillows.cs` — **Pillows : Item** — Item ("pillow"). [Constructable]

#### Scripts/Items/Houses/Decorations/Artifacts — Items: Artifacts. Types: BaseDecorationArtifact, BackpackArtifact, StealableArtifactsSpawner.

- `BaseDecorationArtifact.cs` — **BaseDecorationArtifact : Item** — Item. (+1 type)
- `DoomDecorationArtifacts.cs` — **BackpackArtifact : BaseDecorationContainerArtifact** — Item ("ancient backpack").
- `StealableArtifactsSpawner.cs` — **StealableArtifactsSpawner : Item** — Commands: [GenStealArties, [RemoveStealArties (+2 types)

#### Scripts/Items/Houses/Decorations/Artifacts/Decorative — Items: Decorative. Types: ArtifactLargeVase, ArtifactVase, DemonSkull, DirtPatch, ....

- `ArtifactLargeVase.cs` — **ArtifactLargeVase : Item** — Item. [Constructable]
- `ArtifactVase.cs` — **ArtifactVase : Item** — Item. [Constructable]
- `DemonSkull.cs` — **DemonSkull : Item** — Item. [Constructable]
- `DirtPatch.cs` — **DirtPatch : Item** — Item. [Constructable]
- `EvilIdolSkull.cs` — **EvilIdolSkull : Item** — Item. [Constructable]
- `Futon.cs` — **Futon : Item** — Item. [Constructable, Flipable]
- `LavaTile.cs` — **LavaTile : Item** — Item. [Constructable]
- `Pier.cs` — **Pier : Item** — Item. [Constructable]
- `SkullPole.cs` — **SkullPole : Item** — Item. [Constructable]
- `SwampTile.cs` — **SwampTile : Item** — Item. [Constructable]
- `TatteredAncientMummyWrapping.cs` — **TatteredAncientMummyWrapping : Item** — Item. [Constructable]
- `WallBlood.cs` — **WallBlood : Item** — Item. [Constructable]
- `WaterTile.cs` — **WaterTile : Item** — Item. [Constructable]
- `Web.cs` — **Web : Item** — Item. [Constructable]

#### Scripts/Items/Houses/Doors — Items: Doors. Types: ArgentrockTeleporter, BaseDoor, BasementDoor, BasementDoorway, ....

- `ArgentrockTeleporter.cs` — **ArgentrockTeleporter : Item** — Item ("maze teleporter"). [Constructable]
- `BaseDoor.cs` — **BaseDoor : Item** — Commands: [ChainLink, [Link (+2 types)
- `BasementDoor.cs` — **BasementDoor : Item** — Item ("basement trapdoor"). [Constructable, Flipable]
- `BasementDoorway.cs` — **BasementDoorway : Item** — Item ("door"). [Constructable]
- `DoorOpener.cs` — **DoorOpener : Item** — Item ("a door opener"). [Constructable]
- `DoorStuck.cs` — **DoorStuck : Item** — Item ("door"). [Constructable, Flipable]
- `DoorSwitch.cs` — **DoorSwitch : Item** — Item ("door"). [Constructable] (+1 type)
- `Doors.cs` — **DoorFacing** — Enum with about 12 members. [Constructable] (+17 types)
- `GateMoon.cs` — **GateMoon : Item** — Item ("moongate"). [Constructable] (+1 type)
- `HouseDoors.cs` — **MetalHouseDoor : BaseHouseDoor** — Item.
- `KeywordDoor.cs` — **KeywordDoor : BaseDoor** — Door. [Constructable] (+2 types)
- `MagicMirror.cs` — **MagicMirror : Item** — Item ("mirror"). [Constructable, Flipable]
- `Moongates.cs` — **moongates : Item** — Item. [Constructable] (+2 types)
- `PickableDoor.cs` — **PickableDoor : DarkWoodDoor** — Lockpickable Door v1.0 Original script created b Carding. Modified by PitHelvit [Constructable]
- `Portcullis.cs` — **PortcullisNS : BaseDoor** — Door. [Constructable] (+1 type)
- `PrisonExit.cs` — **PrisonExit : Item** — Item ("teleporter"). [Constructable]
- `RaceTeleporter.cs` — **RaceTeleporter : Item** — Item ("teleporter"). [Constructable]
- `RandomExit.cs` — **RandomExit : Item** — Item ("chasm exit"). [Constructable]
- `SecretDoors.cs` — **SecretStoneDoor1 : BaseDoor** — Door. [Constructable] (+5 types)
- `SerpentPillars.cs` — **SerpentPillars : Item** — Item ("serpent pillar"). [Constructable]
- `SkullGate.cs` — **SkullGate : Item** — Item ("skull gate"). [Constructable]
- `StrangePortal.cs` — **StrangePortal : Item** — Item ("portal"). [Constructable] (+1 type)
- `TeleportTile.cs` — **TeleportTile : Item** — Item ("teleport"). [Constructable]
- `Teleporter.cs` — **Teleporter : Item** — Item. [Constructable] (+2 types)
- `ThruDoor.cs` — **ThruDoor : BaseDoor** — Door ("door"). [Constructable]

#### Scripts/Items/Houses/Monopoly — Items: Monopoly. Types: Monopoly, RUOVersion.

- `Decorate.cs` — **Monopoly** — Commands: [Monopoly (+2 types)
- `RUOVersion.cs` — **RUOVersion** — Item.

#### Scripts/Items/Houses/Monopoly/Gumps/Error Reporting — Items: Error reporting. Types: Errors, ErrorsGump, ErrorsNotifyGump.

- `Errors.cs` — **Errors** — Item.
- `ErrorsGump.cs` — **ErrorsGump : GumpPlusLight** — Item.
- `ErrorsNotifyGump.cs` — **ErrorsNotifyGump : GumpPlusLight** — Item.

#### Scripts/Items/Houses/Monopoly/Gumps/Gumps Plus Light — Items: Gumps plus light. Types: BackgroundPlus, ButtonPlus, GumpPlusLight, HTML, ....

- `BackgroundPlus.cs` — **BackgroundPlus : GumpBackground** — Item.
- `ButtonPlus.cs` — **ButtonPlus : GumpButton** — Item.
- `GumpPlusLight.cs` — **GumpPlusLight : Gump** — Gump UI.
- `HTML.cs` — **HTML** — Item.
- `HtmlPlus.cs` — **HtmlPlus : GumpHtml** — Item.
- `InfoGump.cs` — **InfoGump : GumpPlusLight** — Item.

#### Scripts/Items/Houses/Monopoly/Gumps/TownHouse Gumps — Items: Town house gumps. Types: ContractConfirmGump, ContractSetupGump, TownHouseConfirmGump, TownHouseSetupGump, ....

- `ContractConfirmGump.cs` — **ContractConfirmGump : GumpPlusLight** — Item.
- `ContractSetupGump.cs` — **ContractSetupGump : GumpPlusLight** — Item.
- `TownHouseConfirmGump.cs` — **TownHouseConfirmGump : GumpPlusLight** — Item.
- `TownHouseSetupGump.cs` — **TownHouseSetupGump : GumpPlusLight** — Item.
- `TownHousesGump.cs` — **TownHousesGump : GumpPlusLight** — Item.

#### Scripts/Items/Houses/Monopoly/Items — Items: Items. Types: RentalContract, RentalContractCopy, RentalLicense, SignHammer, ....

- `RentalContract.cs` — **RentalContract : TownHouseSign** — Item.
- `RentalContractCopy.cs` — **RentalContractCopy : Item** — Item ("rental contract copy").
- `RentalLicense.cs` — **RentalLicense : Item** — Item.
- `SignHammer.cs` — **SignHammer : Item** — Item ("Sign Hammer"). (+2 types)
- `TownHouse.cs` — **TownHouse : VersionHouse** — Item.
- `TownHouseSign.cs` — **TownHouseSign : Item** — Item ("This building is for sale or rent!"). [Flipable] (+1 type)

#### Scripts/Items/Houses/Monopoly/Misc — Items: Misc. Types: CommandInfo, DecoreItemInfo, General, GumpResponse.

- `CommandInfo.cs` — **CommandInfo** — Info/data container.
- `DecoreItemInfo.cs` — **DecoreItemInfo** — Info/data container.
- `General.cs` — **General** — Item.
- `GumpResponse.cs` — **GumpResponse** — Item.

#### Scripts/Items/Houses/Remodeling — Items: Remodeling. Types: StairRefundEntry, LawnGate, LawnGump, LawnItem, ....

- `ContextMenuEntries.cs` — **StairRefundEntry : ContextMenuEntry** — Context-menu entry. (+5 types)
- `LawnGate.cs` — **LawnGate : BaseDoor** — Door.
- `LawnGump.cs` — **LawnGump : Gump** — Gump UI. (+1 type)
- `LawnItem.cs` — **LawnItem : LawnPiece** — Item.
- `LawnRegistry.cs` — **LawnRegistry** — Item.
- `LawnRemoveTarget.cs` — **LawnRemoveTarget : Target** — Targeting prompt.
- `LawnSecurityGump.cs` — **LawnSecurityGump : Gump** — Gump UI.
- `LawnStair.cs` — **LawnStair : LawnItem** — Item.
- `LawnSystem.cs` — **LawnSystem** — Item.
- `LawnTarget.cs` — **LawnTarget : Target** — Targeting prompt.
- `LawnTools.cs` — **LawnTools : Item** — Item ("lawn tools"). [Constructable]
- `Remodeling.cs` — **Remodeling** — Item.
- `ShantyDoor.cs` — **ShantyDoor : BaseDoor** — Door.
- `ShantyGump.cs` — **ShantyGump : Gump** — Gump UI. (+1 type)
- `ShantyItem.cs` — **ShantyItem : ShantyPiece** — Item.
- `ShantyRegistry.cs` — **ShantyRegistry** — Item.
- `ShantyRemoveTarget.cs` — **ShantyRemoveTarget : Target** — Targeting prompt.
- `ShantySecurityGump.cs` — **ShantySecurityGump : Gump** — Gump UI.
- `ShantyStair.cs` — **ShantyStair : ShantyItem** — Item.
- `ShantySystem.cs` — **ShantySystem** — Item.
- `ShantyTarget.cs` — **ShantyTarget : Target** — Targeting prompt.
- `ShantyTools.cs` — **ShantyTools : Item** — Item ("remodeling tools"). [Constructable]

#### Scripts/Items/Instruments — Items: Instruments. Types: BambooFlute, BaseInstrument, Drums, Fiddle, ....

- `BambooFlute.cs` — **BambooFlute : BaseInstrument** — Instrument ("flute"). [Constructable]
- `BaseInstrument.cs` — **BaseInstrument : Item** — Item. (+2 types)
- `Drums.cs` — **Drums : BaseInstrument** — Instrument ("drum"). [Constructable]
- `Fiddle.cs` — **Fiddle : BaseInstrument** — Instrument ("fiddle"). [Constructable]
- `Harp.cs` — **Harp : BaseInstrument** — Instrument ("harp"). [Constructable]
- `LapHarp.cs` — **LapHarp : BaseInstrument** — Instrument ("harp"). [Constructable]
- `Lute.cs` — **Lute : BaseInstrument** — Instrument ("lute"). [Constructable]
- `MagicHorn.cs` — **MagicHorn : BaseInstrument** — Instrument ("magic horn"). [Constructable]
- `Pipes.cs` — **Pipes : BaseInstrument** — Instrument ("pipes"). [Constructable]
- `Tambourine.cs` — **Tambourine : BaseInstrument** — Instrument ("tambourine"). [Constructable]
- `TambourineTassel.cs` — **TambourineTassel : BaseInstrument** — Instrument ("tambourine"). [Constructable]
- `Trumpet.cs` — **Trumpet : BaseInstrument** — Instrument ("trumpet"). [Constructable]

#### Scripts/Items/Magical — Magical items: artifacts, quivers, wands, gift/quest rewards.

- `ArtifactManual.cs` — **ArtifactManual : Item** — Item ("Encyclopedia of Rarities"). [Constructable] (+2 types)
- `DraculaSword.cs` — **DraculaSword : RoyalSword** — Magical item ("Dracula's Sword").
- `HydraTooth.cs` — **HydraTooth : SpellScroll** — Magical item ("hydra tooth").
- `LuckyHorseShoes.cs` — **LuckyHorseShoes : Item** — Item ("lucky horse shoes"). [Constructable] (+1 type)
- `MagicQuiver.cs` — **MagicQuiver : BaseQuiver** — Quiver ("quiver"). [Constructable]
- `ManualOfItems.cs` — **ManualOfItems : Item** — Item ("Mystical Relic Chest"). [Constructable] (+1 type)
- `Moonstone.cs` — **MoonStone : Item** — Item ("Moonstone"). [Constructable] (+1 type)
- `RuneOfVirtue.cs` — **RuneOfVirtue : LevelTalismanHoly** — Magical item ("rune").
- `SlayerDeed.cs` — **SlayerDeed : Item** — Item ("a slayer stone"). [Constructable] (+1 type)
- `SoulOrb.cs` — **SoulOrb : Item** — Item ("soul orb"). [Constructable] (+2 types)
- `StaffOfFiveParts.cs` — **StaffFiveParts : QuarterStaff** — //////////////////////////////////////////////////////////////////////// [Constructable] (+5 types)
- `WeaponRenamingTool.cs` — **WeaponRenamingTool : Item** — Item ("Weapon Renaming Tool"). [Constructable] (+3 types)

#### Scripts/Items/Magical/Artifacts — Artifact item definitions grouped by slot/set (armor, clothing, jewelry, weapons, books, quivers).

- `Arty_Setup.cs` — **Arty** — Artifact item.

#### Scripts/Items/Magical/Artifacts/Armor — Artifact items: Armor. Types: Artifact_AbysmalGloves, Artifact_AegisOfGrace, Artifact_AngelicEmbrace, Artifact_ArmsOfToxicity, ....

- `Artifact_AbysmalGloves.cs` — **Artifact_AbysmalGloves : GiftLeatherGloves** — Artifact item ("Abysmal Gloves").
- `Artifact_AegisOfGrace.cs` — **Artifact_AegisOfGrace : GiftDragonHelm** — Artifact item ("Aegis of Grace").
- `Artifact_AngelicEmbrace.cs` — **Artifact_AngelicEmbrace : GiftPlateArms** — Artifact item ("Angelic Embrace").
- `Artifact_ArmsOfToxicity.cs` — **Artifact_ArmsOfToxicity : GiftLeatherArms** — Artifact item ("Arms Of Toxicity").
- `Artifact_BrambleCoat.cs` — **Artifact_BrambleCoat : GiftLeatherChest** — Artifact item ("Bramble Coat").
- `Artifact_CircletOfTheSorceress.cs` — **Artifact_CircletOfTheSorceress : GiftLeatherCap** — Artifact item ("Circlet Of The Sorceress").
- `Artifact_ConansHelm.cs` — **Artifact_ConansHelm : GiftPlateHelm** — Artifact item ("Helm of the Cimmerian").
- `Artifact_DarkGuardiansChest.cs` — **Artifact_DarkGuardiansChest : GiftPlateChest** — Artifact item ("Dark Guardian's Chest").
- `Artifact_DarkNeck.cs` — **Artifact_DarkNeck : GiftPlateGorget** — Artifact item ("Dark Neck").
- `Artifact_DeathsMask.cs` — **Artifact_DeathsMask : GiftBoneHelm** — Artifact item ("Mask of Death").
- `Artifact_DupresCollar.cs` — **Artifact_DupresCollar : GiftPlateGorget** — Artifact item ("Dupre's Collar").
- `Artifact_EvilMageGloves.cs` — **Artifact_EvilMageGloves : GiftLeatherGloves** — Artifact item ("Evil Mage Gloves").
- `Artifact_FeyLeggings.cs` — **Artifact_FeyLeggings : GiftChainLegs** — Artifact item ("Fey Leggings").
- `Artifact_FortifiedArms.cs` — **Artifact_Fortifiedarms : GiftBoneArms** — Artifact item ("Fortified Arms").
- `Artifact_GladiatorsCollar.cs` — **Artifact_GladiatorsCollar : GiftPlateGorget** — Artifact item ("Gladiator's Collar").
- `Artifact_GlovesOfCorruption.cs` — **Artifact_GlovesOfCorruption : GiftLeatherGloves** — Artifact item ("Gloves Of Corruption").
- `Artifact_GlovesOfDexterity.cs` — **Artifact_GlovesOfDexterity : GiftLeatherGloves** — Artifact item ("Gloves of Dexterity").
- `Artifact_GlovesOfRegeneration.cs` — **Artifact_GlovesOfRegeneration : GiftLeatherGloves** — Artifact item ("Gloves Of Regeneration").
- `Artifact_GlovesOfThePugilist.cs` — **Artifact_GlovesOfThePugilist : GiftPugilistGloves** — Artifact item ("gloves of the pugilist").
- `Artifact_GuantletsOfAnger.cs` — **Artifact_GuantletsOfAnger : GiftPlateGloves** — Artifact item ("Gauntlets of Anger").
- `Artifact_HeartOfTheLion.cs` — **Artifact_HeartOfTheLion : GiftPlateChest** — Artifact item ("Heart of the Lion").
- `Artifact_HellForgedArms.cs` — **Artifact_HellForgedArms : GiftPlateArms** — Artifact item ("Hell Forged Arms").
- `Artifact_HelmOfBrilliance.cs` — **Artifact_HelmOfBrilliance : GiftNorseHelm** — Artifact item ("Helm of Brilliance").
- `Artifact_HelmOfSwiftness.cs` — **Artifact_HelmOfSwiftness : GiftNorseHelm** — Artifact item ("Helm of Swiftness").
- `Artifact_Indecency.cs` — **Artifact_Indecency : GiftStuddedChest** — Artifact item ("Indecency").
- `Artifact_IronwoodCrown.cs` — **Artifact_IronwoodCrown : GiftPlateHelm** — Artifact item ("Ironwood Crown").
- `Artifact_LeggingsOfDeceit.cs` — **Artifact_LeggingsOfDeceit : GiftChainLegs** — Artifact item ("Leggings Of Deceit").
- `Artifact_LeggingsOfEmbers.cs` — **Artifact_LeggingsOfEmbers : GiftPlateLegs** — Artifact item ("Royal Leggings of Embers").
- `Artifact_LeggingsOfEnlightenment.cs` — **Artifact_LeggingsOfEnlightenment : GiftLeatherLegs** — Artifact item ("Leggings Of Enlightenment").
- `Artifact_MagiciansMempo.cs` — **Artifact_MagiciansMempo : GiftPlateMempo** — Artifact item ("Magician's Mempo").
- `Artifact_OrcChieftainHelm.cs` — **Artifact_OrcChieftainHelm : GiftOrcHelm** — Artifact item ("Orc Chieftain Helm").
- `Artifact_OrcishVisage.cs` — **Artifact_OrcishVisage : GiftOrcHelm** — Artifact item ("Orcish Visage").
- `Artifact_RoyalGuardsChestplate.cs` — **Artifact_RoyalGuardsChestplate : GiftPlateChest** — Artifact item ("Royal Guard's Chest Plate").
- `Artifact_RoyalGuardsGorget.cs` — **Artifact_RoyalGuardsGorget : GiftPlateGorget** — Artifact item ("Royal Guardian's Gorget").
- `Artifact_SamuraiHelm.cs` — **Artifact_SamuraiHelm : GiftPlateBattleKabuto** — Artifact item ("Ancient Samurai Helm").
- `Artifact_ShroudOfDeceit.cs` — **Artifact_ShroudOfDeciet : GiftBoneChest** — Artifact item ("Shroud of Deceit").
- `Artifact_SongWovenMantle.cs` — **Artifact_SongWovenMantle : GiftLeatherArms** — Artifact item ("Song Woven Mantle").
- `Artifact_SpellWovenBritches.cs` — **Artifact_SpellWovenBritches : GiftLeatherLegs** — Artifact item ("Spell Woven Britches").
- `Artifact_StitchersMittens.cs` — **Artifact_StitchersMittens : GiftLeatherGloves** — Artifact item ("Stitcher's Mittens").
- `Artifact_VioletCourage.cs` — **Artifact_VioletCourage : GiftFemalePlateChest** — Artifact item ("Violet Courage").
- `Artifact_WizardsPants.cs` — **Artifact_WizardsPants : GiftLeatherLegs** — Artifact item ("Wizard's Pants").
- `Artifact_YashimotosHatsuburi.cs` — **Artifact_YashimotosHatsuburi : GiftChainHatsuburi** — Artifact item ("Yashimoto's Hatsuburi").

#### Scripts/Items/Magical/Artifacts/Armor/Aegis — Artifact items: Aegis. Types: Artifact_ArmsOfAegis, Artifact_GlovesOfAegis, Artifact_GorgetOfAegis, Artifact_HelmOfAegis, ....

- `Artifact_ArmsOfAegis.cs` — **Artifact_ArmsOfAegis : GiftPlateArms** — Artifact item ("Arms of Aegis").
- `Artifact_GlovesOfAegis.cs` — **Artifact_GlovesOfAegis : GiftPlateGloves** — Artifact item ("Gloves of Aegis").
- `Artifact_GorgetOfAegis.cs` — **Artifact_GorgetOfAegis : GiftPlateGorget** — Artifact item ("Gorget of Aegis").
- `Artifact_HelmOfAegis.cs` — **Artifact_HelmOfAegis : GiftPlateHelm** — Artifact item ("Helm of Aegis").
- `Artifact_LegsOfAegis.cs` — **Artifact_LeggingsOfAegis : GiftPlateLegs** — Artifact item ("Leggings of Aegis").
- `Artifact_TunicOfAegis.cs` — **Artifact_TunicOfAegis : GiftPlateChest** — Artifact item ("Tunic of Aegis").

#### Scripts/Items/Magical/Artifacts/Armor/Arcane — Artifact items: Arcane. Types: Artifact_ArcaneArms, Artifact_ArcaneCap, Artifact_ArcaneGloves, Artifact_ArcaneGorget, ....

- `Artifact_ArcaneArms.cs` — **Artifact_ArcaneArms : GiftLeatherArms** — Artifact item ("Arcane Arms").
- `Artifact_ArcaneCap.cs` — **Artifact_ArcaneCap : GiftLeatherCap** — Artifact item ("Arcane Cap").
- `Artifact_ArcaneGloves.cs` — **Artifact_ArcaneGloves : GiftLeatherGloves** — Artifact item ("Arcane Gloves").
- `Artifact_ArcaneGorget.cs` — **Artifact_ArcaneGorget : GiftLeatherGorget** — Artifact item ("Arcane Gorget").
- `Artifact_ArcaneLeggings.cs` — **Artifact_ArcaneLeggings : GiftLeatherLegs** — Artifact item ("Arcane Leggings").
- `Artifact_ArcaneTunic.cs` — **Artifact_ArcaneTunic : GiftLeatherChest** — Artifact item ("Arcane Tunic").

#### Scripts/Items/Magical/Artifacts/Armor/Bane — Artifact items: Bane. Types: Artifact_CoifOfBane, Artifact_LeggingsOfBane, Artifact_TunicOfBane.

- `Artifact_CoifOfBane.cs` — **Artifact_CoifOfBane : GiftChainCoif** — Artifact item ("Coif of Bane").
- `Artifact_LeggingsOfBane.cs` — **Artifact_LeggingsOfBane : GiftChainLegs** — Artifact item ("Leggings of Bane").
- `Artifact_TunicOfBane.cs` — **Artifact_TunicOfBane : GiftChainChest** — Artifact item ("Tunic of Bane").

#### Scripts/Items/Magical/Artifacts/Armor/Divine — Artifact items: Divine. Types: Artifact_DivineArms, Artifact_DivineCountenance, Artifact_DivineGloves, Artifact_DivineGorget, ....

- `Artifact_DivineArms.cs` — **Artifact_DivineArms : GiftLeatherArms** — Artifact item ("Divine Arms").
- `Artifact_DivineCountenance.cs` — **Artifact_DivineCountenance : GiftHornedTribalMask** — Artifact item ("Divine Countenance").
- `Artifact_DivineGloves.cs` — **Artifact_DivineGloves : GiftLeatherGloves** — Artifact item ("Divine Gloves").
- `Artifact_DivineGorget.cs` — **Artifact_DivineGorget : GiftLeatherGorget** — Artifact item ("Divine Gorget").
- `Artifact_DivineLeggings.cs` — **Artifact_DivineLeggings : GiftLeatherLegs** — Artifact item ("Divine Leggings").
- `Artifact_DivineTunic.cs` — **Artifact_DivineTunic : GiftLeatherChest** — Artifact item ("Divine Tunic").

#### Scripts/Items/Magical/Artifacts/Armor/FallenKing — Artifact items: Fallen king. Types: Artifact_ArmsOfTheFallenKing, Artifact_CapOfTheFallenKing, Artifact_GlovesOfTheFallenKing, Artifact_LegsOfTheFallenKing, ....

- `Artifact_ArmsOfTheFallenKing.cs` — **Artifact_ArmsOfTheFallenKing : GiftLeatherArms** — Artifact item ("Arms of the Fallen King").
- `Artifact_CapOfTheFallenKing.cs` — **Artifact_CapOfTheFallenKing : GiftLeatherCap** — Artifact item ("Cap of the Fallen King").
- `Artifact_GlovesOfTheFallenKing.cs` — **Artifact_GlovesOfTheFallenKing : GiftLeatherGloves** — Artifact item ("Gloves of the Fallen King").
- `Artifact_LegsOfTheFallenKing.cs` — **Artifact_LegsOfTheFallenKing : GiftLeatherLegs** — Artifact item ("Leggings of the Fallen King").
- `Artifact_TunicOfTheFallenKing.cs` — **Artifact_TunicOfTheFallenKing : GiftLeatherChest** — Artifact item ("Tunic of the Fallen King").
- `Artifact_VoiceOfTheFallenKing.cs` — **Artifact_VoiceOfTheFallenKing : GiftLeatherGorget** — Artifact item ("Voice of the Fallen King").

#### Scripts/Items/Magical/Artifacts/Armor/Fire — Artifact items: Fire. Types: Artifact_CoifOfFire, Artifact_LeggingsOfFire, Artifact_TunicOfFire.

- `Artifact_CoifOfFire.cs` — **Artifact_CoifOfFire : GiftChainCoif** — Artifact item ("Coif of Fire").
- `Artifact_LeggingsOfFire.cs` — **Artifact_LeggingsOfFire : GiftChainLegs** — Artifact item ("Leggings of Fire").
- `Artifact_TunicOfFire.cs` — **Artifact_TunicOfFire : GiftChainChest** — Artifact item ("Tunic of Fire").

#### Scripts/Items/Magical/Artifacts/Armor/Fortune — Artifact items: Fortune. Types: Artifact_ArmorOfFortune, Artifact_ArmsOfFortune, Artifact_CapOfFortune, Artifact_GlovesOfFortune, ....

- `Artifact_ArmorOfFortune.cs` — **Artifact_ArmorOfFortune : GiftStuddedChest** — Artifact item ("Armor of Fortune").
- `Artifact_ArmsOfFortune.cs` — **Artifact_ArmsOfFortune : GiftStuddedArms** — Artifact item ("Arms of Fortune").
- `Artifact_CapOfFortune.cs` — **Artifact_CapOfFortune : GiftLeatherCap** — Artifact item ("Cap of Fortune").
- `Artifact_GlovesOfFortune.cs` — **Artifact_GlovesOfFortune : GiftStuddedGloves** — Artifact item ("Gloves of Fortune").
- `Artifact_GorgetOfFortune.cs` — **Artifact_GorgetOfFortune : GiftStuddedGorget** — Artifact item ("Gorget of Fortune").
- `Artifact_LegsOfFortune.cs` — **Artifact_LegsOfFortune : GiftStuddedLegs** — Artifact item ("Legging of Fortune").

#### Scripts/Items/Magical/Artifacts/Armor/Harrower — Artifact items: Harrower. Types: Artifact_ArmsOfTheHarrower, Artifact_GlovesOfTheHarrower, Artifact_LegsOfTheHarrower, Artifact_OrnateCrownOfTheHarrower, ....

- `Artifact_ArmsOfTheHarrower.cs` — **Artifact_ArmsOfTheHarrower : GiftBoneArms** — Artifact item ("Arms of the Harrower").
- `Artifact_GlovesOfTheHarrower.cs` — **Artifact_GlovesOfTheHarrower : GiftBoneGloves** — Artifact item ("Gloves of the Harrower").
- `Artifact_LegsOfTheHarrower.cs` — **Artifact_LegsOfTheHarrower : GiftBoneLegs** — Artifact item ("Leggings of the Harrower").
- `Artifact_OrnateCrownOfTheHarrower.cs` — **Artifact_OrnateCrownOfTheHarrower : GiftBoneHelm** — Artifact item ("Ornate Crown of the Harrower").
- `Artifact_TunicOfTheHarrower.cs` — **Artifact_TunicOfTheHarrower : GiftBoneChest** — Artifact item ("Tunic of the Harrower").

#### Scripts/Items/Magical/Artifacts/Armor/HolyKnight — Artifact items: Holy knight. Types: Artifact_HolyKnightsArmPlates, Artifact_HolyKnightsBreastplate, Artifact_HolyKnightsGloves, Artifact_HolyKnightsGorget, ....

- `Artifact_HolyKnightsArms.cs` — **Artifact_HolyKnightsArmPlates : GiftRoyalArms** — Artifact item ("Holy Knight's Arm Plates").
- `Artifact_HolyKnightsBreastplate.cs` — **Artifact_HolyKnightsBreastplate : GiftRoyalChest** — Artifact item ("Holy Knight's Breastplate").
- `Artifact_HolyKnightsGloves.cs` — **Artifact_HolyKnightsGloves : GiftRoyalGloves** — Artifact item ("Holy Knight's Gloves").
- `Artifact_HolyKnightsGorget.cs` — **Artifact_HolyKnightsGorget : GiftRoyalGorget** — Artifact item ("Holy Knight's Gorget").
- `Artifact_HolyKnightsHelm.cs` — **Artifact_HolyKnightsPlateHelm : GiftRoyalHelm** — Artifact item ("Holy Knight's Plate Helm").
- `Artifact_HolyKnightsLegging.cs` — **Artifact_HolyKnightsLegging : GiftRoyalsLegs** — Artifact item ("Holy Knight's Legging").

#### Scripts/Items/Magical/Artifacts/Armor/Hunters — Artifact items: Hunters. Types: Artifact_HuntersArms, Artifact_HuntersGloves, Artifact_HuntersGorget, Artifact_HuntersLeggings, ....

- `Artifact_HuntersArms.cs` — **Artifact_HuntersArms : GiftLeatherArms** — Artifact item ("Hunter's Arms").
- `Artifact_HuntersGloves.cs` — **Artifact_HuntersGloves : GiftLeatherGloves** — Artifact item ("Hunter's Gloves").
- `Artifact_HuntersGorget.cs` — **Artifact_HuntersGorget : GiftLeatherGorget** — Artifact item ("Hunter's Gorget").
- `Artifact_HuntersLeggings.cs` — **Artifact_HuntersLeggings : GiftLeatherLegs** — Artifact item ("Hunter's Leggings").
- `Artifact_HuntersTunic.cs` — **Artifact_HuntersTunic : GiftLeatherChest** — Artifact item ("Hunter's Tunic").

#### Scripts/Items/Magical/Artifacts/Armor/Inquisitor — Artifact items: Inquisitor. Types: Artifact_InquisitorsArms, Artifact_InquisitorsGorget, Artifact_InquisitorsHelm, Artifact_InquisitorsLeggings, ....

- `Artifact_InquisitorsArms.cs` — **Artifact_InquisitorsArms : GiftPlateArms** — Artifact item ("Inquisitor's Arms").
- `Artifact_InquisitorsGorget.cs` — **Artifact_InquisitorsGorget : GiftPlateGorget** — Artifact item ("Inquisitor's Gorget").
- `Artifact_InquisitorsHelm.cs` — **Artifact_InquisitorsHelm : GiftPlateHelm** — Artifact item ("Inquisitor's Helm").
- `Artifact_InquisitorsLegs.cs` — **Artifact_InquisitorsLeggings : GiftPlateLegs** — Artifact item ("Inquisitor's Leggings").
- `Artifact_InquisitorsResolution.cs` — **Artifact_InquisitorsResolution : GiftPlateGloves** — Artifact item ("Inquisitor's Resolution").
- `Artifact_InquisitorsTunic.cs` — **Artifact_InquisitorsTunic : GiftPlateChest** — Artifact item ("Inquisitor's Tunic").

#### Scripts/Items/Magical/Artifacts/Armor/Insight — Artifact items: Insight. Types: Artifact_ArmorOfInsight, Artifact_ArmsOfInsight, Artifact_GlovesOfInsight, Artifact_GorgetOfInsight, ....

- `Artifact_ArmorOfInsight.cs` — **Artifact_ArmorOfInsight : GiftPlateChest** — Artifact item ("Armor of Insight").
- `Artifact_ArmsOfInsight.cs` — **Artifact_ArmsOfInsight : GiftPlateArms** — Artifact item ("Arms of Insight").
- `Artifact_GlovesOfInsight.cs` — **Artifact_GlovesOfInsight : GiftPlateGloves** — Artifact item ("Gloves of Insight").
- `Artifact_GorgetOfInsight.cs` — **Artifact_GorgetOfInsight : GiftPlateGorget** — Artifact item ("Gorget of Insight").
- `Artifact_HelmOfInsight.cs` — **Artifact_HelmOfInsight : GiftPlateHelm** — Artifact item ("Helm of Insight").
- `Artifact_LegsOfInsight.cs` — **Artifact_LegsOfInsight : GiftPlateLegs** — Artifact item ("Legging of Insight").

#### Scripts/Items/Magical/Artifacts/Armor/Jackals — Artifact items: Jackals. Types: Artifact_JackalsArms, Artifact_JackalsCollar, Artifact_JackalsGloves, Artifact_JackalsHelm, ....

- `Artifact_JackalsArms.cs` — **Artifact_JackalsArms : GiftPlateArms** — Artifact item ("Jackal's Arms").
- `Artifact_JackalsCollar.cs` — **Artifact_JackalsCollar : GiftPlateGorget** — Artifact item ("Jackal's Collar").
- `Artifact_JackalsGloves.cs` — **Artifact_JackalsGloves : GiftPlateGloves** — Artifact item ("Jackal's Gloves").
- `Artifact_JackalsHelm.cs` — **Artifact_JackalsHelm : GiftPlateHelm** — Artifact item ("Jackal's Helm").
- `Artifact_JackalsLegging.cs` — **Artifact_JackalsLeggings : GiftPlateLegs** — Artifact item ("Jackal's Leggings").
- `Artifact_JackalsTunic.cs` — **Artifact_JackalsTunic : GiftPlateChest** — Artifact item ("Jackal's Tunic").

#### Scripts/Items/Magical/Artifacts/Armor/Midnight — Artifact items: Midnight. Types: Artifact_MidnightBracers, Artifact_MidnightGloves, Artifact_MidnightHelm, Artifact_MidnightLegs, ....

- `Artifact_MidnightBracers.cs` — **Artifact_MidnightBracers : GiftBoneArms** — Artifact item ("Midnight Bracers").
- `Artifact_MidnightGloves.cs` — **Artifact_MidnightGloves : GiftBoneGloves** — Artifact item ("Midnight Gloves").
- `Artifact_MidnightHelm.cs` — **Artifact_MidnightHelm : GiftBoneHelm** — Artifact item ("Midnight Helm").
- `Artifact_MidnightLegs.cs` — **Artifact_MidnightLegs : GiftBoneLegs** — Artifact item ("Midnight Leggings").
- `Artifact_MidnightTunic.cs` — **Artifact_MidnightTunic : GiftBoneChest** — Artifact item ("Midnight Tunic").

#### Scripts/Items/Magical/Artifacts/Armor/Nobility — Artifact items: Nobility. Types: Artifact_ArmorOfNobility, Artifact_ArmsOfNobility, Artifact_GauntletsOfNobility, Artifact_LegsOfNobility.

- `Artifact_ArmorOfNobility.cs` — **Artifact_ArmorOfNobility : GiftRingmailChest** — Artifact item ("Armor of Nobility").
- `Artifact_ArmsOfNobility.cs` — **Artifact_ArmsOfNobility : GiftRingmailArms** — Artifact item ("Arms of Nobility").
- `Artifact_GauntletsOfNobility.cs` — **Artifact_GauntletsOfNobility : GiftRingmailGloves** — Artifact item ("Gauntlets of Nobility").
- `Artifact_LegsOfNobility.cs` — **Artifact_LegsOfNobility : GiftRingmailLegs** — Artifact item ("Legs of Nobility").

#### Scripts/Items/Magical/Artifacts/Armor/ShadowDancer — Artifact items: Shadow dancer. Types: Artifact_ShadowDancerArms, Artifact_ShadowDancerCap, Artifact_ShadowDancerGloves, Artifact_ShadowDancerGorget, ....

- `Artifact_ShadowDancerArms.cs` — **Artifact_ShadowDancerArms : GiftLeatherArms** — Artifact item ("Shadow Dancer Arms").
- `Artifact_ShadowDancerCap.cs` — **Artifact_ShadowDancerCap : GiftLeatherCap** — Artifact item ("Shadow Dancer Cap").
- `Artifact_ShadowDancerGloves.cs` — **Artifact_ShadowDancerGloves : GiftLeatherGloves** — Artifact item ("Shadow Dancer Gloves").
- `Artifact_ShadowDancerGorget.cs` — **Artifact_ShadowDancerGorget : GiftLeatherGorget** — Artifact item ("Shadow Dancer Gorget").
- `Artifact_ShadowDancerLeggings.cs` — **Artifact_ShadowDancerLeggings : GiftLeatherLegs** — Artifact item ("Shadow Dancer Leggings").
- `Artifact_ShadowDancerTunic.cs` — **Artifact_ShadowDancerTunic : GiftLeatherChest** — Artifact item ("Shadow Dancer Tunic").

#### Scripts/Items/Magical/Artifacts/Armor/Totem — Artifact items: Totem. Types: Artifact_SpiritOfTheTotem, Artifact_TotemArms, Artifact_TotemGloves, Artifact_TotemGorget, ....

- `Artifact_SpiritOfTheTotem.cs` — **Artifact_SpiritOfTheTotem : GiftBearMask** — Artifact item ("Spirit of the Totem").
- `Artifact_TotemArms.cs` — **Artifact_TotemArms : GiftLeatherArms** — Artifact item ("Totem Arms").
- `Artifact_TotemGloves.cs` — **Artifact_TotemGloves : GiftLeatherGloves** — Artifact item ("Totem Gloves").
- `Artifact_TotemGorget.cs` — **Artifact_TotemGorget : GiftLeatherGorget** — Artifact item ("Totem Gorget").
- `Artifact_TotemLeggings.cs` — **Artifact_TotemLeggings : GiftLeatherLegs** — Artifact item ("Totem Leggings").
- `Artifact_TotemTunic.cs` — **Artifact_TotemTunic : GiftLeatherChest** — Artifact item ("Totem Tunic").

#### Scripts/Items/Magical/Artifacts/Books — Artifact items: Books. Types: Artifact_BookOfKnowledge, Artifact_HydrosLexicon, Artifact_LithosTome, Artifact_OssianGrimoire, ....

- `Artifact_BookOfKnowledge.cs` — **Artifact_BookOfKnowledge : Spellbook** — Artifact item ("Book Of Knowledge").
- `Artifact_HydrosLexicon.cs` — **Artifact_HydrosLexicon : ElementalSpellbook** — Artifact item ("Lexicon of the Lurker").
- `Artifact_LithosTome.cs` — **Artifact_LithosTome : ElementalSpellbook** — Artifact item ("Tome of the Mountain King").
- `Artifact_OssianGrimoire.cs` — **Artifact_OssianGrimoire : NecromancerSpellbook** — Artifact item ("Ossian Grimoire").
- `Artifact_PyrosGrimoire.cs` — **Artifact_PyrosGrimoire : ElementalSpellbook** — Artifact item ("Grimoire of the Daemon King").
- `Artifact_StratosManual.cs` — **Artifact_StratosManual : ElementalSpellbook** — Artifact item ("Manual of the Mystic Voice").

#### Scripts/Items/Magical/Artifacts/Clothing/Back — Artifact items: Back. Types: Artifact_EmbroideredOakLeafCloak, Artifact_FurCapeOfTheSorceress, Artifact_GrayMouserCloak, Artifact_PolarBearCape.

- `Artifact_EmbroideredOakLeafCloak.cs` — **Artifact_EmbroideredOakLeafCloak : GiftCloak** — Artifact item ("Embroidered Oak Leaf Cloak").
- `Artifact_FurCapeOfTheSorceress.cs` — **Artifact_FurCapeOfTheSorceress : GiftCloak** — Artifact item ("Fur Cape Of The Sorceress").
- `Artifact_GrayMouserCloak.cs` — **Artifact_GrayMouserCloak : GiftCloak** — Artifact item ("Cloak of the Rogue").
- `Artifact_PolarBearCloak.cs` — **Artifact_PolarBearCape : GiftFurCape** — Artifact item ("Polar Bear Cape").

#### Scripts/Items/Magical/Artifacts/Clothing/Chest — Artifact items: Chest. Types: Artifact_JinBaoriOfGoodFortune, Artifact_LieutenantOfTheBritannianRoyalGuard.

- `Artifact_JinBaoriOfGoodfFortune.cs` — **Artifact_JinBaoriOfGoodFortune : GiftJinBaori** — Artifact item ("Jin-Baori Of Good Fortune").
- `Artifact_LieutenantOfTheBritannianRoyalGuard.cs` — **Artifact_LieutenantOfTheBritannianRoyalGuard : GiftSash** — Artifact item ("Royal Guard Sash").

#### Scripts/Items/Magical/Artifacts/Clothing/Foot — Artifact items: Foot. Types: Artifact_BootsofHermes, Artifact_BootsofHydros, Artifact_BootsofLithos, Artifact_BootsofPyros, ....

- `Artifact_BootsofHermes.cs` — **Artifact_BootsofHermes : GiftBoots** — Artifact item ("Boots of Hermes").
- `Artifact_BootsofHydros.cs` — **Artifact_BootsofHydros : GiftBoots** — Artifact item ("Boots of the Lurker").
- `Artifact_BootsofLithos.cs` — **Artifact_BootsofLithos : GiftBoots** — Artifact item ("Boots of the Mountain King").
- `Artifact_BootsofPyros.cs` — **Artifact_BootsofPyros : GiftBoots** — Artifact item ("Boots of the Daemon King").
- `Artifact_BootsofStratos.cs` — **Artifact_BootsofStratos : GiftBoots** — Artifact item ("Boots of the Mystic Voice").
- `Artifact_DetectiveBoots.cs` — **Artifact_DetectiveBoots : GiftBoots** — Artifact item ("Detective Boots of the Royal Guard").
- `Artifact_PadsOfTheCuSidhe.cs` — **Artifact_PadsOfTheCuSidhe : GiftFurBoots** — Artifact item ("Pads of the Cu Sidhe").
- `Artifact_PolarBearBoots.cs` — **Artifact_PolarBearBoots : GiftFurBoots** — Artifact item ("Polar Bear Boots").
- `Artifact_SprintersSandals.cs` — **Artifact_SprintersSandals : GiftSandals** — Artifact item ("Sprinter's Sandals").

#### Scripts/Items/Magical/Artifacts/Clothing/Head — Artifact items: Head. Types: Artifact_BurglarsBandana, Artifact_CaptainJohnsHat, Artifact_CrownOfTalKeesh, Artifact_DreadPirateHat, ....

- `Artifact_BurglarsBandana.cs` — **Artifact_BurglarsBandana : GiftBandana** — Artifact item ("Burglar's Bandana").
- `Artifact_CaptainJohnsHat.cs` — **Artifact_CaptainJohnsHat : GiftTricorneHat** — Artifact item ("Captain John's Hat").
- `Artifact_CrownOfTalKeesh.cs` — **Artifact_CrownOfTalKeesh : GiftBandana** — Artifact item ("Crown of Tal'Keesh").
- `Artifact_DreadPirateHat.cs` — **Artifact_DreadPirateHat : GiftTricorneHat** — Artifact item ("Dread Pirate Hat").
- `Artifact_GandalfsHat.cs` — **Artifact_GandalfsHat : GiftWizardsHat** — Artifact item ("Merlin's Mystical Hat").
- `Artifact_GrimReapersMask.cs` — **Artifact_GrimReapersMask : GiftWizardsHat** — Artifact item ("Grim Reaper's Mask").
- `Artifact_HatOfTheMagi.cs` — **Artifact_HatOfTheMagi : GiftWizardsHat** — Artifact item ("Hat of the Magi").
- `Artifact_HuntersHeaddress.cs` — **Artifact_HuntersHeaddress : GiftDeerMask** — Artifact item ("Hunter's Headdress").
- `Artifact_JesterHatofChuckles.cs` — **Artifact_JesterHatofChuckles : GiftJesterHat** — Artifact item ("Jester Hat of Chuckles").
- `Artifact_KodiakBearMask.cs` — **Artifact_KodiakBearMask : GiftBearMask** — Artifact item ("Kodiak Bear Mask").
- `Artifact_MantleofHydros.cs` — **Artifact_MantleofHydros : GiftWizardsHat** — Artifact item ("Mantle of the Lurker").
- `Artifact_MantleofLithos.cs` — **Artifact_MantleofLithos : GiftWizardsHat** — Artifact item ("Mantle of the Mountain King").
- `Artifact_MantleofPyros.cs` — **Artifact_MantleofPyros : GiftWizardsHat** — Artifact item ("Mantle of the Daemon King").
- `Artifact_MantleofStratos.cs` — **Artifact_MantleofStratos : GiftWizardsHat** — Artifact item ("Mantle of the Mystic Voice").
- `Artifact_PolarBearMask.cs` — **Artifact_PolarBearMask : GiftBearMask** — Artifact item ("Spirit of the Polar Bear").
- `Artifact_RobinHoodsFeatheredHat.cs` — **Artifact_RobinHoodsFeatheredHat : GiftFeatheredHat** — Artifact item ("Robin Hood's Feathered Hat").

#### Scripts/Items/Magical/Artifacts/Clothing/Outer — Artifact items: Outer. Types: Artifact_ANecromancerShroud, Artifact_AcidProofRobe, Artifact_ArcanicRobe, Artifact_BeggarsRobe, ....

- `Artifact_ANecromancerShroud.cs` — **Artifact_ANecromancerShroud : GiftRobe** — Artifact item ("Necromancer Shroud").
- `Artifact_AcidProofRobe.cs` — **Artifact_AcidProofRobe : GiftRobe** — Artifact item ("Acidic Robe").
- `Artifact_ArcanicRobe.cs` — **Artifact_ArcanicRobe : GiftRobe** — Artifact item ("Arcanic Robe").
- `Artifact_BeggarsRobe.cs` — **Artifact_BeggarsRobe : GiftRobe** — Artifact item ("Beggar's Robe").
- `Artifact_GandalfsRobe.cs` — **Artifact_GandalfsRobe : GiftRobe** — Artifact item ("Merlin's Mystical Robe").
- `Artifact_GrimReapersRobe.cs` — **Artifact_GrimReapersRobe : GiftRobe** — Artifact item ("Grim Reaper's Robe").
- `Artifact_HoodedShroudOfShadows.cs` — **Artifact_HoodedShroudOfShadows : GiftRobe** — Artifact item ("Hooded Shroud of Shadows").
- `Artifact_RobeOfTeleportation.cs` — **Artifact_RobeOfTeleportation : GiftRobe** — Artifact item ("Robe Of Teleportation").
- `Artifact_RobeOfTheEclipse.cs` — **Artifact_RobeOfTheEclipse : GiftRobe** — Artifact item ("Robe of the Eclipse").
- `Artifact_RobeOfTheEquinox.cs` — **Artifact_RobeOfTheEquinox : GiftRobe** — Artifact item ("Robe of the Equinox").
- `Artifact_RobeOfTreason.cs` — **Artifact_RobeOfTreason : GiftRobe** — Artifact item ("Robe Of Treason").
- `Artifact_RobeofHydros.cs` — **Artifact_RobeofHydros : GiftRobe** — Artifact item ("Robe of the Lurker").
- `Artifact_RobeofLithos.cs` — **Artifact_RobeofLithos : GiftRobe** — Artifact item ("Robe of the Mountain King").
- `Artifact_RobeofPyros.cs` — **Artifact_RobeofPyros : GiftRobe** — Artifact item ("Robe of the Daemon King").
- `Artifact_RobeofStratos.cs` — **Artifact_RobeofStratos : GiftRobe** — Artifact item ("Robe of the Mystic Voice").
- `Artifact_SamaritanRobe.cs` — **Artifact_SamaritanRobe : GiftRobe** — Artifact item ("Good Samaritan Robe").
- `Artifact_TheRobeOfBritanniaAri.cs` — **Artifact_TheRobeOfBritanniaAri : GiftRobe** — Artifact item ("Robe of Sosaria").
- `Artifact_VampiresRobe.cs` — **Artifact_VampiresRobe : GiftRobe** — Artifact item ("Nosferatu's Robe").

#### Scripts/Items/Magical/Artifacts/Clothing/Waist — Artifact items: Waist. Types: Artifact_BeltofHercules, Artifact_ConansLoinCloth, Artifact_CrimsonCincture, Artifact_GeishasObi.

- `Artifact_BeltofHercules.cs` — **Artifact_BeltofHercules : GiftBelt** — Artifact item ("Belt of Hercules").
- `Artifact_ConansLoinCloth.cs` — **Artifact_ConansLoinCloth : GiftBelt** — Artifact item ("Loin Cloth of the Cimmerian").
- `Artifact_CrimsonCincture.cs` — **Artifact_CrimsonCincture : GiftHalfApron** — Artifact item ("Crimson Cincture").
- `Artifact_GeishasObi.cs` — **Artifact_GeishasObi : GiftObi** — Artifact item ("Geishas Obi").

#### Scripts/Items/Magical/Artifacts/Instruments — Artifact items: Instruments. Types: Artifact_GwennosHarp, Artifact_HornOfKingTriton, Artifact_IolosLute.

- `Artifact_GwennosHarp.cs` — **Artifact_GwennosHarp : BaseInstrument** — Instrument ("Gwenno's Harp"). [Constructable]
- `Artifact_HornOfKingTriton.cs` — **Artifact_HornOfKingTriton : BaseInstrument** — Instrument ("Horn of King Triton"). [Constructable]
- `Artifact_IolosLute.cs` — **Artifact_IolosLute : BaseInstrument** — Instrument ("Iolo's Lute"). [Constructable]

#### Scripts/Items/Magical/Artifacts/Jewelry — Artifact items: Jewelry. Types: Artifact_AlchemistsBauble, Artifact_BraceletOfHealth, Artifact_BraceletOfTheElements, Artifact_BraceletOfTheVile, ....

- `Artifact_AlchemistsBauble.cs` — **Artifact_AlchemistsBauble : GiftTalismanLeather** — Artifact item ("Alchemist's Bauble").
- `Artifact_BraceletOfHealth.cs` — **Artifact_BraceletOfHealth : GiftGoldBracelet** — Artifact item ("Bracelet of Health").
- `Artifact_BraceletOfTheElements.cs` — **Artifact_BraceletOfTheElements : GiftGoldBracelet** — Artifact item ("Bracelet of the Elements").
- `Artifact_BraceletOfTheVile.cs` — **Artifact_BraceletOfTheVile : GiftGoldBracelet** — Artifact item ("Bracelet of the Vile").
- `Artifact_DjinnisRing.cs` — **Artifact_DjinnisRing : GiftSilverRing** — Artifact item ("Djinni's Ring").
- `Artifact_EarringsOfHealth.cs` — **Artifact_EarringsOfHealth : GiftGoldEarrings** — Artifact item ("Earrings of Health").
- `Artifact_EarringsOfProtection.cs` — **Artifact_RingOfProtection : GiftGoldRing** — Artifact item ("Ring of Protection").
- `Artifact_EarringsOfTheElements.cs` — **Artifact_EarringsOfTheElements : GiftGoldEarrings** — Artifact item ("Earrings of the Elements").
- `Artifact_EarringsOfTheMagician.cs` — **Artifact_EarringsOfTheMagician : GiftGoldEarrings** — Artifact item ("Earrings of the Magician").
- `Artifact_EarringsOfTheVile.cs` — **Artifact_EarringsOfTheVile : GiftGoldEarrings** — Artifact item ("Earrings of the Vile").
- `Artifact_EssenceOfBattle.cs` — **Artifact_EssenceOfBattle : GiftGoldRing** — Artifact item ("Essence of Battle").
- `Artifact_LuckyEarrings.cs` — **Artifact_LuckyEarrings : GiftGoldEarrings** — Standard artifact earrings: Luck 150, regen 3, attack/defend +5, weapon speed +5, ArtifactLevel set and ArtySetup applied.
- `Artifact_LuckyNecklace.cs` — **Artifact_LuckyNecklace : GiftGoldNecklace** — Artifact item ("Lucky Necklace").
- `Artifact_MagesBand.cs` — **Artifact_MagesBand : GiftGoldRing** — Artifact item ("Mage's Band").
- `Artifact_OrnamentOfTheMagician.cs` — **Artifact_OrnamentOfTheMagician : GiftGoldBracelet** — Artifact item ("Ornament of the Magician").
- `Artifact_PendantOfTheMagi.cs` — **Artifact_PendantOfTheMagi : GiftGoldNecklace** — Artifact item ("Pendant of the Magi").
- `Artifact_ResillientBracer.cs` — **Artifact_ResilientBracer : GiftGoldBracelet** — Artifact item ("Resillient Bracer").
- `Artifact_RingOfHealth.cs` — **Artifact_RingOfHealth : GiftGoldRing** — Artifact item ("Ring of Health").
- `Artifact_RingOfTheElements.cs` — **Artifact_RingOfTheElements : GiftGoldRing** — Artifact item ("Ring of the Elements").
- `Artifact_RingOfTheMagician.cs` — **Artifact_RingOfTheMagician : GiftGoldRing** — Artifact item ("Ring of the Magician").
- `Artifact_RingOfTheVile.cs` — **Artifact_RingOfTheVile : GiftGoldRing** — Artifact item ("Ring of the Vile").
- `Artifact_WarriorsClasp.cs` — **Artifact_WarriorsClasp : GiftGoldBracelet** — Artifact item ("Warrior's Clasp").

#### Scripts/Items/Magical/Artifacts/Minor — Artifact items: Minor. Types: AdmiralsHeartyRum, CandelabraOfSouls, ColoringBook, EverlastingBottle, ....

- `AdmiralsHeartyRum.cs` — **AdmiralsHeartyRum : BeverageBottle** — Artifact item.
- `CandelabraOfSouls.cs` — **CandelabraOfSouls : CandelabraStand** — Artifact item ("Candelabra of Souls").
- `ColoringBook.cs` — **ColoringBook : Item** — Item ("Book of Prismatic Magic"). [Constructable] (+2 types)
- `EverlastingBottle.cs` — **EverlastingBottle : Item** — Item ("Everlasting Bottle"). [Constructable]
- `EverlastingLoaf.cs` — **EverlastingLoaf : Item** — Item ("Everlasting Loaf"). [Constructable]
- `GemOfSeeing.cs` — **GemOfSeeing : Item** — Item ("Gem of Seeing"). [Constructable] (+2 types)
- `GhostShipAnchor.cs` — **GhostShipAnchor : Item** — Item. [Constructable]
- `GoldBricks.cs` — **GoldBricks : Item** — Item. [Constructable]
- `PandorasBox.cs` — **PandorasBox : Item** — Item ("Pandora's Box"). [Constructable] (+1 type)
- `PhillipsWoodenSteed.cs` — **PhillipsWoodenSteed : MonsterStatuette** — Artifact item.
- `SeahorseStatuette.cs` — **SeahorseStatuette : MonsterStatuette** — Artifact item.
- `ShipModelOfTheHMSCape.cs` — **ShipModelOfTheHMSCape : Item** — Item. [Constructable]

#### Scripts/Items/Magical/Artifacts/Offhands — Artifact items: Offhands. Types: Artifact_ArcticBeacon, Artifact_AuraOfShadows, Artifact_CandleCold, Artifact_EternalFlame, ....

- `Artifact_ArcticBeacon.cs` — **Artifact_ArcticBeacon : GiftLantern** — Artifact item ("Winter Beacon").
- `Artifact_AuraOfShadows.cs` — **Artifact_AuraOfShadows : GiftLantern** — Artifact item ("Aura Of Shadows").
- `Artifact_Candles.cs` — **Artifact_CandleCold : GiftCandle** — //////////////////////////////////////////////////////////////////////////////////////////// [Constructable] (+5 types)
- `Artifact_EternalFlame.cs` — **Artifact_EternalFlame : GiftLantern** — Artifact item ("Eternal Flame").
- `Artifact_GrimReapersLantern.cs` — **Artifact_GrimReapersLantern : GiftLantern** — Artifact item ("Grim Reaper's Lantern").
- `Artifact_NoxNightlight.cs` — **Artifact_NoxNightlight : GiftLantern** — Artifact item ("Nox Nightlight").
- `Artifact_PowerSurge.cs` — **Artifact_PowerSurge : GiftLantern** — Artifact item ("Lantern of Power").
- `Artifact_TorchOfTrapFinding.cs` — **Artifact_TorchOfTrapFinding : GiftTorch** — Artifact item ("Torch of Trap Burning").

#### Scripts/Items/Magical/Artifacts/Quivers — Artifact items: Quivers. Types: Artifact_Pestilence, Artifact_QuiverOfBlight, Artifact_QuiverOfElements, Artifact_QuiverOfFire, ....

- `Artifact_Pestilence.cs` — **Artifact_Pestilence : BaseQuiver** — Quiver ("Pestilence"). [Constructable]
- `Artifact_QuiverOfBlight.cs` — **Artifact_QuiverOfBlight : ElvenQuiver** — Artifact item ("Quiver of Blight").
- `Artifact_QuiverOfElements.cs` — **Artifact_QuiverOfElements : BaseQuiver** — Quiver ("Quiver of the Elements"). [Constructable]
- `Artifact_QuiverOfFire.cs` — **Artifact_QuiverOfFire : ElvenQuiver** — Artifact item ("Quiver of Fire").
- `Artifact_QuiverOfIce.cs` — **Artifact_QuiverOfIce : ElvenQuiver** — Artifact item ("Quiver of Ice").
- `Artifact_QuiverOfInfinity.cs` — **Artifact_QuiverOfInfinity : BaseQuiver** — Quiver ("Quiver of Infinity"). [Constructable]
- `Artifact_QuiverOfLightning.cs` — **Artifact_QuiverOfLightning : ElvenQuiver** — Artifact item ("Quiver of Lightning").
- `Artifact_QuiverOfRage.cs` — **Artifact_QuiverOfRage : BaseQuiver** — Quiver ("Quiver of Rage"). [Constructable]

#### Scripts/Items/Magical/Artifacts/Shields — Artifact items: Shields. Types: Artifact_AchillesShield, Artifact_Aegis, Artifact_ArcaneShield, Artifact_DupresShield, ....

- `Artifact_AchillesShield.cs` — **Artifact_AchillesShield : GiftBronzeShield** — Artifact item ("Achille's Shield").
- `Artifact_Aegis.cs` — **Artifact_Aegis : GiftHeaterShield** — Artifact item ("Aegis").
- `Artifact_ArcaneShield.cs` — **Artifact_ArcaneShield : GiftWoodenKiteShield** — Artifact item ("Arcane Shield").
- `Artifact_DupresShield.cs` — **Artifact_DupresShield : GiftOrderShield** — Artifact item ("Dupre�s Shield").
- `Artifact_MarbleShield.cs` — **Artifact_MarbleShield : GiftHeaterShield** — Artifact item ("Gargoyle Shield").
- `Artifact_ShieldOfInvulnerability.cs` — **Artifact_ShieldOfInvulnerability : GiftOrderShield** — Artifact item ("Shield of Invulnerability").

#### Scripts/Items/Magical/Artifacts/Trinkets — Artifact items: Trinkets. Types: Artifact_BloodwoodSpirit, Artifact_ShimmeringTalisman, Artifact_TotemOfVoid.

- `Artifact_BloodwoodSpirit.cs` — **Artifact_BloodwoodSpirit : GiftTalismanLeather** — Artifact item ("Bloodwood Spirit").
- `Artifact_ShimmeringTalisman.cs` — **Artifact_ShimmeringTalisman : GiftTalismanLeather** — Artifact item ("Shimmering Talisman").
- `Artifact_TotemOfVoid.cs` — **Artifact_TotemOfVoid : GiftTalismanLeather** — Artifact item ("Totem of the Void").

#### Scripts/Items/Magical/Artifacts/Weapons — Artifact items: Weapons. Types: Artifact_RodOfResurrection.

- `Artifact_RodOfResurrection.cs` — **Artifact_RodOfResurrection : GiftScepter** — Artifact item ("Rod Of Resurrection").

#### Scripts/Items/Magical/Artifacts/Weapons/Axes — Artifact items: Axes. Types: Artifact_Annihilation, Artifact_AxeOfTheHeavens, Artifact_AxeoftheMinotaur, Artifact_BlazeOfDeath, ....

- `Artifact_Annihilation.cs` — **Artifact_Annihilation : GiftBardiche** — Artifact item ("Annihilation").
- `Artifact_AxeOfTheHeavens.cs` — **Artifact_AxeOfTheHeavens : GiftDoubleAxe** — Artifact item ("Axe of the Heavens").
- `Artifact_AxeoftheMinotaur.cs` — **Artifact_AxeoftheMinotaur : GiftLargeBattleAxe** — Artifact item ("Axe of the Minotaur").
- `Artifact_BlazeOfDeath.cs` — **Artifact_BlazeOfDeath : GiftHalberd** — Artifact item ("Blaze of Death").
- `Artifact_Calm.cs` — **Artifact_Calm : GiftHalberd** — Artifact item ("Calm").
- `Artifact_KamiNarisIndestructableDoubleAxe.cs` — **Artifact_KamiNarisIndestructableDoubleAxe : GiftDoubleAxe** — Artifact item ("Kami-Naris Indestructable Axe").
- `Artifact_LegacyOfTheDreadLord.cs` — **Artifact_LegacyOfTheDreadLord : GiftBardiche** — Artifact item ("Legacy of the Dread Lord").
- `Artifact_MadmansHatchet.cs` — **Artifact_MadmansHatchet : GiftHatchet** — Artifact item ("Madman's Hatchet").
- `Artifact_Quell.cs` — **Artifact_Quell : GiftBardiche** — Artifact item ("Quell").
- `Artifact_TalonBite.cs` — **Artifact_TalonBite : GiftOrnateAxe** — Artifact item ("Talon Bite").
- `Artifact_ZyronicClaw.cs` — **Artifact_ZyronicClaw : GiftExecutionersAxe** — Artifact item ("Zyronic Claw").

#### Scripts/Items/Magical/Artifacts/Weapons/Bludgeoning — Artifact items: Bludgeoning. Types: Artifact_ArcticDeathDealer, Artifact_BelmontWhip, Artifact_BoneCrusher, Artifact_Bonesmasher, ....

- `Artifact_ArcticDeathDealer.cs` — **Artifact_ArcticDeathDealer : GiftWarMace** — Artifact item ("Arctic Death Dealer").
- `Artifact_BelmontWhip.cs` — **Artifact_BelmontWhip : GiftWhips** — Artifact item ("Vampire Killer").
- `Artifact_BoneCrusher.cs` — **Artifact_BoneCrusher : GiftWarMace** — Artifact item ("Bone Crusher").
- `Artifact_Bonesmasher.cs` — **Artifact_Bonesmasher : GiftDiamondMace** — Artifact item ("Bonesmasher").
- `Artifact_CavortingClub.cs` — **Artifact_CavortingClub : GiftClub** — Artifact item ("Cavorting Club").
- `Artifact_FalseGodsScepter.cs` — **Artifact_FalseGodsScepter : GiftScepter** — Artifact item ("Scepter Of The False Goddess").
- `Artifact_GiantBlackjack.cs` — **Artifact_GiantBlackjack : GiftClub** — Artifact item ("Giant Blackjack").
- `Artifact_HammerofThor.cs` — **Artifact_HammerofThor : GiftWarHammer** — Artifact item ("Hammer of Thor").
- `Artifact_MaulOfTheTitans.cs` — **Artifact_MaulOfTheTitans : GiftMaul** — Artifact item ("Maul of the Titans").
- `Artifact_MauloftheBeast.cs` — **Artifact_MauloftheBeast : GiftMaul** — Artifact item ("Maul of the Beast").
- `Artifact_MelisandesCorrodedHatchet.cs` — **Artifact_MelisandesCorrodedHatchet : GiftHatchet** — Artifact item ("Melisande's Corroded Hatchet").
- `Artifact_PixieSwatter.cs` — **Artifact_PixieSwatter : GiftScepter** — Artifact item ("Pixie Swatter").
- `Artifact_ShardTrasher.cs` — **Artifact_ShardThrasher : GiftDiamondMace** — Artifact item ("Shard Thrasher").
- `Artifact_TheBeserkersMaul.cs` — **Artifact_TheBeserkersMaul : GiftMaul** — Artifact item ("Berserker's Maul").
- `Artifact_TitansHammer.cs` — **Artifact_TitansHammer : GiftWarHammer** — Artifact item ("Titan's Hammer").

#### Scripts/Items/Magical/Artifacts/Weapons/Bows — Artifact items: Bows. Types: Artifact_AilricsLongbow, Artifact_BlightGrippedLongbow, Artifact_BowOfTheJukaKing, Artifact_BowofthePhoenix, ....

- `Artifact_AilricsLongbow.cs` — **Artifact_AilricsLongbow : GiftElvenCompositeLongbow** — Artifact item ("Ailric's Longbow").
- `Artifact_BlightGrippedLongbow.cs` — **Artifact_BlightGrippedLongbow : GiftElvenCompositeLongbow** — Artifact item ("Blight Gripped Longbow").
- `Artifact_BowOfTheJukaKing.cs` — **Artifact_BowOfTheJukaKing : GiftBow** — Artifact item ("Bow of the Juka King").
- `Artifact_BowofthePhoenix.cs` — **Artifact_BowofthePhoenix : GiftElvenCompositeLongbow** — Artifact item ("Bow of the Phoenix").
- `Artifact_Frostbringer.cs` — **Artifact_Frostbringer : GiftBow** — Artifact item ("Frostbringer").
- `Artifact_LongShot.cs` — **Artifact_LongShot : GiftCompositeBow** — Artifact item ("Long Shot").
- `Artifact_NoxBow.cs` — **Artifact_NoxBow : GiftHeavyCrossbow** — Artifact item ("Nox Bow").
- `Artifact_NoxRangersHeavyCrossbow.cs` — **Artifact_NoxRangersHeavyCrossbow : GiftHeavyCrossbow** — Artifact item ("Nox Ranger's Heavy Crossbow").
- `Artifact_RobinHoodsBow.cs` — **Artifact_RobinHoodsBow : GiftCompositeBow** — Artifact item ("Robin Hood's Bow").
- `Artifact_RoyalArchersBow.cs` — **Artifact_RoyalArchersBow : GiftBow** — Artifact item ("Royal Archer's Bow").
- `Artifact_ShaminoCrossbow.cs` — **Artifact_ShaMontorrossbow : GiftRepeatingCrossbow** — Artifact item ("Shamino�s Crossbow").
- `Artifact_SilvanisFeywoodBow.cs` — **Artifact_SilvanisFeywoodBow : GiftElvenCompositeLongbow** — Artifact item ("Silvani's Feywood Bow").
- `Artifact_TheDryadBow.cs` — **Artifact_TheDryadBow : GiftBow** — Artifact item ("Dryad Bow").
- `Artifact_TheNightReaper.cs` — **Artifact_TheNightReaper : GiftRepeatingCrossbow** — Artifact item ("Night Reaper").
- `Artifact_WildfireBow.cs` — **Artifact_WildfireBow : GiftElvenCompositeLongbow** — Artifact item ("Wildfire Bow").
- `Artifact_Windsong.cs` — **Artifact_Windsong : GiftMagicalShortbow** — Artifact item ("Windsong").

#### Scripts/Items/Magical/Artifacts/Weapons/Fencing — Artifact items: Fencing. Types: Artifact_AchillesSpear, Artifact_DaggerOfVenom, Artifact_DarkLordsPitchfork, Artifact_EnchantedTitanLegBone, ....

- `Artifact_AchillesSpear.cs` — **Artifact_AchillesSpear : GiftSpear** — Artifact item ("Achille's Spear").
- `Artifact_DaggerOfVenom.cs` — **Artifact_DaggerOfVenom : GiftDagger** — Artifact item ("Dagger of Venom").
- `Artifact_DarkLordsPitchfork.cs` — **Artifact_DarkLordsPitchfork : GiftPitchfork** — Artifact item ("Dark Lord's PitchFork").
- `Artifact_EnchantedTitanLegBone.cs` — **Artifact_EnchantedTitanLegBone : GiftShortSpear** — Artifact item ("Enchanted Pirate Rapier").
- `Artifact_FangOfRactus.cs` — **Artifact_FangOfRactus : GiftKryss** — Artifact item ("Fang of Ractus").
- `Artifact_FesteringWound.cs` — **Artifact_FesteringWound : GiftKryss** — Artifact item ("Festering Wound").
- `Artifact_LunaLance.cs` — **Artifact_LunaLance : GiftLance** — Artifact item ("Holy Lance").
- `Artifact_MagiciansIllusion.cs` — **Artifact_MagiciansIllusion : GiftDoubleBladedStaff** — Artifact item ("Magician's Illusion").
- `Artifact_NightsKiss.cs` — **Artifact_NightsKiss : GiftDagger** — Artifact item ("Night's Kiss").
- `Artifact_OblivionsNeedle.cs` — **Artifact_OblivionsNeedle : GiftDagger** — Artifact item ("Oblivion Needle").
- `Artifact_Pacify.cs` — **Artifact_Pacify : GiftPike** — Artifact item ("Pacify").
- `Artifact_RaedsGlory.cs` — **Artifact_RaedsGlory : GiftWarCleaver** — Artifact item ("Raed's Glory").
- `Artifact_RamusNecromanticScalpel.cs` — **Artifact_RamusNecromanticScalpel : GiftDagger** — Artifact item ("Ramus' Necromantic Scalpel").
- `Artifact_Retort.cs` — **Artifact_Retort : GiftWarFork** — Artifact item ("Retort").
- `Artifact_RoyalGuardSurvivalKnife.cs` — **Artifact_RoyalGuardSurvivalKnife : GiftDagger** — Artifact item ("Royal Guard Survival Knife").
- `Artifact_RuneCarvingKnife.cs` — **Artifact_RuneCarvingKnife : GiftAssassinSpike** — Artifact item ("Rune Carving Knife").
- `Artifact_SerpentsFang.cs` — **Artifact_SerpentsFang : GiftKryss** — Artifact item ("Serpent's Fang").
- `Artifact_TheDragonSlayer.cs` — **Artifact_TheDragonSlayer : GiftLance** — Artifact item ("Slayer of Dragons").
- `Artifact_TheTaskmaster.cs` — **Artifact_TheTaskmaster : GiftWarFork** — Artifact item ("Taskmaster").

#### Scripts/Items/Magical/Artifacts/Weapons/Staffs — Artifact items: Staffs. Types: Artifact_Boomstick, Artifact_GandalfsStaff, Artifact_PhantomStaff, Artifact_StaffOfPower, ....

- `Artifact_Boomstick.cs` — **Artifact_Boomstick : GiftWildStaff** — Artifact item ("Boomstick").
- `Artifact_GandalfsStaff.cs` — **Artifact_GandalfsStaff : GiftQuarterStaff** — Artifact item ("Merlin's Mystical Staff").
- `Artifact_PhantomStaff.cs` — **Artifact_PhantomStaff : GiftWildStaff** — Artifact item ("Phantom Staff").
- `Artifact_StaffOfPower.cs` — **Artifact_StaffOfPower : GiftBlackStaff** — Artifact item ("Staff of Power").
- `Artifact_StaffOfTheMagi.cs` — **Artifact_StaffOfTheMagi : GiftBlackStaff** — Artifact item ("Staff of the Magi").
- `Artifact_StaffofSnakes.cs` — **Artifact_StaffofSnakes : GiftQuarterStaff** — Artifact item ("Staff of the Serpent").
- `Artifact_WrathOfTheDryad.cs` — **Artifact_WrathOfTheDryad : GiftGnarledStaff** — Artifact item ("Wrath of the Dryad").

#### Scripts/Items/Magical/Artifacts/Weapons/Swordsmanship — Artifact items: Swordsmanship. Types: Artifact_AngeroftheGods, Artifact_BladeDance, Artifact_BladeOfInsanity, Artifact_BladeOfTheRighteous, ....

- `Artifact_AngerOfTheGods.cs` — **Artifact_AngeroftheGods : GiftBroadsword** — Artifact item ("Anger of the Gods").
- `Artifact_BladeDance.cs` — **Artifact_BladeDance : GiftRuneBlade** — Artifact item ("Blade Dance").
- `Artifact_BladeOfInsanity.cs` — **Artifact_BladeOfInsanity : GiftKatana** — Artifact item ("Blade of Insanity").
- `Artifact_BladeOfTheRighteous.cs` — **Artifact_BladeOfTheRighteous : GiftLongsword** — Artifact item ("Blade of the Righteous").
- `Artifact_BraveKnightOfTheBritannia.cs` — **Artifact_BraveKnightOfTheBritannia : GiftKatana** — Artifact item ("Brave Knight of Sosaria").
- `Artifact_BreathOfTheDead.cs` — **Artifact_BreathOfTheDead : GiftBoneHarvester** — Artifact item ("Breath of the Dead").
- `Artifact_CaptainQuacklebushsCutlass.cs` — **Artifact_CaptainQuacklebushsCutlass : GiftCutlass** — Artifact item ("Captain Quacklebush's Cutlass").
- `Artifact_ColdBlood.cs` — **Artifact_ColdBlood : GiftCleaver** — Artifact item ("Cold Blood").
- `Artifact_ColdForgedBlade.cs` — **Artifact_ColdForgedBlade : GiftElvenSpellblade** — Artifact item ("Cold Forged Blade").
- `Artifact_ConansSword.cs` — **Artifact_ConansSword : GiftClaymore** — Artifact item ("Blade of the Cimmerian").
- `Artifact_DragonSlayer.cs` — **Artifact_NordicVikingSword : GiftClaymore** — Artifact item ("Nordic Dragon Blade").
- `Artifact_Excalibur.cs` — **Artifact_Excalibur : GiftClaymore** — Artifact item ("Excalibur").
- `Artifact_FleshRipper.cs` — **Artifact_FleshRipper : GiftAssassinSpike** — Artifact item ("Flesh Ripper").
- `Artifact_FortunateBlades.cs` — **Artifact_FortunateBlades : GiftDaisho** — Artifact item ("Fortunate Blades").
- `Artifact_Fury.cs` — **Artifact_Fury : GiftKatana** — Artifact item ("Fury").
- `Artifact_GlassSword.cs` — **Artifact_GlassSword : GiftVikingSword** — Artifact item ("Sword of Shattered Hopes").
- `Artifact_GrimReapersScythe.cs` — **Artifact_GrimReapersScythe : GiftScythe** — Artifact item ("Grim Reaper's Scythe").
- `Artifact_HolySword.cs` — **Artifact_HolySword : GiftLongsword** — Artifact item ("Holy Sword").
- `Artifact_JadeScimitar.cs` — **Artifact_JadeScimitar : GiftScimitar** — Artifact item ("Jade Scimitar").
- `Artifact_LuminousRuneBlade.cs` — **Artifact_LuminousRuneBlade : GiftRuneBlade** — Artifact item ("Luminous Rune Blade").
- `Artifact_MinersPickaxe.cs` — **Artifact_MinersPickaxe : GiftPickaxe** — Artifact item ("Miner's Pickaxe").
- `Artifact_OverseerSunderedBlade.cs` — **Artifact_OverseerSunderedBlade : GiftRadiantScimitar** — Artifact item ("Overseer Sundered Blade").
- `Artifact_RighteousAnger.cs` — **Artifact_RighteousAnger : GiftElvenMachete** — Artifact item ("Righteous Anger").
- `Artifact_Shadowblade.cs` — **Artifact_ShadowBlade : GiftLongsword** — Artifact item ("Blade of the Shadows").
- `Artifact_SinbadsSword.cs` — **Artifact_SinbadsSword : GiftCutlass** — Artifact item ("Sword of Sinbad").
- `Artifact_SoulSeeker.cs` — **Artifact_SoulSeeker : GiftRadiantScimitar** — Artifact item ("Soul Seeker").
- `Artifact_Stormbringer.cs` — **Artifact_Stormbringer : GiftVikingSword** — Artifact item ("Stormbringer").
- `Artifact_Subdue.cs` — **Artifact_Subdue : GiftScythe** — Artifact item ("Subdue").
- `Artifact_SwiftStrike.cs` — **Artifact_SwiftStrike : GiftWakizashi** — Artifact item ("Swift Strike").
- `Artifact_TownGuardsHalberd.cs` — **Artifact_TownGuardsHalberd : GiftHalberd** — Artifact item ("Guardsman Halberd").
- `Artifact_VampiricDaisho.cs` — **Artifact_VampiricDaisho : GiftDaisho** — Artifact item ("Vampiric Daisho").

#### Scripts/Items/Magical/Gifts — Gift/reward item bases and variants.

- `GiftAttributes.cs` — **GiftAttributes** — Magical item.
- `GiftGump.cs` — **GiftGump : Gump** — Gump UI. (+1 type)
- `Gifts.cs` — **IGiftable** — Interface contract. (+1 type)

#### Scripts/Items/Magical/Gifts/Armor — Magical items: Armor. Types: BaseGiftArmor, GiftWoodenPlateLegs.

- `BaseGiftArmor.cs` — **BaseGiftArmor : BaseArmor** — Armor.
- `GiftWoodenArmor.cs` — **GiftWoodenPlateLegs : BaseGiftArmor ///////////////////////////////////////////////////////** — Magical item ("wooden leggings").

#### Scripts/Items/Magical/Gifts/Armor/Bone — Magical items: Bone. Types: GiftBoneArms, GiftBoneChest, GiftBoneGloves, GiftBoneLegs.

- `GiftBoneArms.cs` — **GiftBoneArms : BaseGiftArmor** — Magical item.
- `GiftBoneChest.cs` — **GiftBoneChest : BaseGiftArmor** — Magical item.
- `GiftBoneGloves.cs` — **GiftBoneGloves : BaseGiftArmor** — Magical item.
- `GiftBoneLegs.cs` — **GiftBoneLegs : BaseGiftArmor** — Magical item.

#### Scripts/Items/Magical/Gifts/Armor/Chain — Magical items: Chain. Types: GiftChainChest, GiftChainHatsuburi, GiftChainLegs.

- `GiftChainChest.cs` — **GiftChainChest : BaseGiftArmor** — Magical item ("chainmail tunic").
- `GiftChainHatsuburi.cs` — **GiftChainHatsuburi : BaseGiftArmor** — Magical item.
- `GiftChainLegs.cs` — **GiftChainLegs : BaseGiftArmor** — Magical item ("chainmail leggings").

#### Scripts/Items/Magical/Gifts/Armor/Dragon — Magical items: Dragon. Types: GiftDragonArms, GiftDragonChest, GiftDragonGloves, GiftDragonHelm, ....

- `GiftDragonArms.cs` — **GiftDragonArms : BaseGiftArmor** — Magical item ("dragon scale arms").
- `GiftDragonChest.cs` — **GiftDragonChest : BaseGiftArmor** — Magical item ("dragon scale tunic").
- `GiftDragonGloves.cs` — **GiftDragonGloves : BaseGiftArmor** — Magical item ("dragon scale gloves").
- `GiftDragonHelm.cs` — **GiftDragonHelm : BaseGiftArmor** — Magical item ("dragon scale helm").
- `GiftDragonLegs.cs` — **GiftDragonLegs : BaseGiftArmor** — Magical item ("dragon scale leggings").

#### Scripts/Items/Magical/Gifts/Armor/Helmets — Magical items: Helmets. Types: GiftBascinet, GiftBoneHelm, GiftChainCoif, GiftCirclet, ....

- `GiftBascinet.cs` — **GiftBascinet : BaseGiftArmor** — Magical item ("bascinet").
- `GiftBoneHelm.cs` — **GiftBoneHelm : BaseGiftArmor** — Magical item.
- `GiftChainCoif.cs` — **GiftChainCoif : BaseGiftArmor** — Magical item ("chainmail coif").
- `GiftCirclet.cs` — **GiftCirclet : BaseGiftArmor** — Magical item ("circlet").
- `GiftCloseHelm.cs` — **GiftCloseHelm : BaseGiftArmor** — Magical item.
- `GiftDreadHelm.cs` — **GiftDreadHelm : BaseGiftArmor** — Magical item ("dread helm").
- `GiftHelmet.cs` — **GiftHelmet : BaseGiftArmor** — Magical item.
- `GiftLeatherCap.cs` — **GiftLeatherCap : BaseGiftArmor** — Magical item.
- `GiftNorseHelm.cs` — **GiftNorseHelm : BaseGiftArmor** — Magical item ("norse helm").
- `GiftOniwabanHood.cs` — **GiftOniwabanHood : GiftLeatherCap** — Magical item ("oniwaban hood").
- `GiftOrcHelm.cs` — **GiftOrcHelm : BaseGiftArmor** — Magical item ("horned helm").
- `GiftPlateHelm.cs` — **GiftPlateHelm : BaseGiftArmor** — Magical item ("plate helm").
- `GiftRoyalHelm.cs` — **GiftRoyalHelm : GiftPlateHelm** — Magical item ("royal helm").

#### Scripts/Items/Magical/Gifts/Armor/Leather — Magical items: Leather. Types: GiftFemaleLeatherChest, GiftLeatherArms, GiftLeatherBustierArms, GiftLeatherChest, ....

- `GiftFemaleLeatherChest.cs` — **GiftFemaleLeatherChest : BaseGiftArmor** — Magical item.
- `GiftLeatherArms.cs` — **GiftLeatherArms : BaseGiftArmor** — Magical item.
- `GiftLeatherBustierArms.cs` — **GiftLeatherBustierArms : BaseGiftArmor** — Magical item.
- `GiftLeatherChest.cs` — **GiftLeatherChest : BaseGiftArmor** — Magical item ("leather tunic").
- `GiftLeatherCloak.cs` — **GiftLeatherCloak : BaseGiftArmor** — Magical item ("leather cloak").
- `GiftLeatherDo.cs` — **GiftLeatherDo : BaseGiftArmor** — Magical item.
- `GiftLeatherGloves.cs` — **GiftLeatherGloves : BaseGiftArmor** — Magical item.
- `GiftLeatherGorget.cs` — **GiftLeatherGorget : BaseGiftArmor** — Magical item.
- `GiftLeatherHaidate.cs` — **GiftLeatherHaidate : BaseGiftArmor** — Magical item.
- `GiftLeatherHiroSode.cs` — **GiftLeatherHiroSode : BaseGiftArmor** — Magical item.
- `GiftLeatherJingasa.cs` — **GiftLeatherJingasa : BaseGiftArmor** — Magical item.
- `GiftLeatherLegs.cs` — **GiftLeatherLegs : BaseGiftArmor** — Magical item.
- `GiftLeatherMempo.cs` — **GiftLeatherMempo : BaseGiftArmor** — Magical item.
- `GiftLeatherNinjaHood.cs` — **GiftLeatherNinjaHood : BaseGiftArmor** — Magical item.
- `GiftLeatherNinjaJacket.cs` — **GiftLeatherNinjaJacket : BaseGiftArmor** — Magical item.
- `GiftLeatherNinjaMitts.cs` — **GiftLeatherNinjaMitts : BaseGiftArmor** — Magical item.
- `GiftLeatherNinjaPants.cs` — **GiftLeatherNinjaPants : BaseGiftArmor** — Magical item.
- `GiftLeatherRobe.cs` — **GiftLeatherRobe : BaseGiftArmor** — Magical item ("leather robe").
- `GiftLeatherShorts.cs` — **GiftLeatherShorts : BaseGiftArmor** — Magical item.
- `GiftLeatherSkirt.cs` — **GiftLeatherSkirt : BaseGiftArmor** — Magical item.
- `GiftLeatherSuneate.cs` — **GiftLeatherSuneate : BaseGiftArmor** — Magical item.
- `GiftOniwabanBoots.cs` — **GiftOniwabanBoots : BaseGiftArmor** — Magical item ("oniwaban boots").
- `GiftOniwabanGloves.cs` — **GiftOniwabanGloves : GiftLeatherGloves** — Magical item ("oniwaban gloves").
- `GiftOniwabanLeggings.cs` — **GiftOniwabanLeggings : GiftLeatherLegs** — Magical item ("oniwaban leggings").
- `GiftOniwabanTunic.cs` — **GiftOniwabanTunic : GiftLeatherChest** — Magical item ("oniwaban tunic").
- `GiftShinobiGarb.cs` — **GiftShinobiRobe : GiftLeatherRobe** — Magical item ("leather shinobi robe").

#### Scripts/Items/Magical/Gifts/Armor/Plate — Magical items: Plate. Types: GiftDecorativePlateKabuto, GiftFemalePlateChest, GiftHeavyPlateJingasa, GiftLightPlateJingasa, ....

- `GiftDecorativePlateKabuto.cs` — **GiftDecorativePlateKabuto : BaseGiftArmor** — Magical item.
- `GiftFemalePlateChest.cs` — **GiftFemalePlateChest : BaseGiftArmor** — Magical item.
- `GiftHeavyPlateJingasa.cs` — **GiftHeavyPlateJingasa : BaseGiftArmor** — Magical item.
- `GiftLightPlateJingasa.cs` — **GiftLightPlateJingasa : BaseGiftArmor** — Magical item.
- `GiftPlateArms.cs` — **GiftPlateArms : BaseGiftArmor** — Magical item ("platemail arms").
- `GiftPlateBattleKabuto.cs` — **GiftPlateBattleKabuto : BaseGiftArmor** — Magical item.
- `GiftPlateChest.cs` — **GiftPlateChest : BaseGiftArmor** — Magical item ("platemail").
- `GiftPlateDo.cs` — **GiftPlateDo : BaseGiftArmor** — Magical item.
- `GiftPlateGloves.cs` — **GiftPlateGloves : BaseGiftArmor** — Magical item.
- `GiftPlateGorget.cs` — **GiftPlateGorget : BaseGiftArmor** — Magical item.
- `GiftPlateHaidate.cs` — **GiftPlateHaidate : BaseGiftArmor** — Magical item.
- `GiftPlateHatsuburi.cs` — **GiftPlateHatsuburi : BaseGiftArmor** — Magical item.
- `GiftPlateHiroSode.cs` — **GiftPlateHiroSode : BaseGiftArmor** — Magical item.
- `GiftPlateLegs.cs` — **GiftPlateLegs : BaseGiftArmor** — Magical item ("platemail legs").
- `GiftPlateMempo.cs` — **GiftPlateMempo : BaseGiftArmor** — Magical item.
- `GiftPlateSuneate.cs` — **GiftPlateSuneate : BaseGiftArmor** — Magical item.
- `GiftSmallPlateJingasa.cs` — **GiftSmallPlateJingasa : BaseGiftArmor** — Magical item.
- `GiftStandardPlateKabuto.cs` — **GiftStandardPlateKabuto : BaseGiftArmor** — Magical item.

#### Scripts/Items/Magical/Gifts/Armor/Ring — Magical items: Ring. Types: GiftRingmailArms, GiftRingmailChest, GiftRingmailGloves, GiftRingmailLegs.

- `GiftRingmailArms.cs` — **GiftRingmailArms : BaseGiftArmor** — Magical item.
- `GiftRingmailChest.cs` — **GiftRingmailChest : BaseGiftArmor** — Magical item.
- `GiftRingmailGloves.cs` — **GiftRingmailGloves : BaseGiftArmor** — Magical item.
- `GiftRingmailLegs.cs` — **GiftRingmailLegs : BaseGiftArmor** — Magical item.

#### Scripts/Items/Magical/Gifts/Armor/Royal — Magical items: Royal. Types: GiftRoyalArms, GiftRoyalBoots, GiftRoyalChest, GiftRoyalGloves, ....

- `GiftRoyalArms.cs` — **GiftRoyalArms : GiftPlateArms** — Magical item ("royal mantle").
- `GiftRoyalBoots.cs` — **GiftRoyalBoots : GiftPlateGorget** — Magical item ("royal boots").
- `GiftRoyalChest.cs` — **GiftRoyalChest : GiftPlateChest** — Magical item ("royal tunic").
- `GiftRoyalGloves.cs` — **GiftRoyalGloves : GiftPlateGloves** — Magical item ("royal bracers").
- `GiftRoyalGorget.cs` — **GiftRoyalGorget : GiftPlateGorget** — Magical item ("royal gorget").
- `GiftRoyalLegs.cs` — **GiftRoyalsLegs : GiftPlateLegs** — Magical item ("royal leggings").

#### Scripts/Items/Magical/Gifts/Armor/Studded — Magical items: Studded. Types: GiftFemaleStuddedChest, GiftStuddedArms, GiftStuddedBustierArms, GiftStuddedChest, ....

- `GiftFemaleStuddedChest.cs` — **GiftFemaleStuddedChest : BaseGiftArmor** — Magical item.
- `GiftStuddedArms.cs` — **GiftStuddedArms : BaseGiftArmor** — Magical item.
- `GiftStuddedBustierArms.cs` — **GiftStuddedBustierArms : BaseGiftArmor** — Magical item.
- `GiftStuddedChest.cs` — **GiftStuddedChest : BaseGiftArmor** — Magical item.
- `GiftStuddedDo.cs` — **GiftStuddedDo : BaseGiftArmor** — Magical item.
- `GiftStuddedGloves.cs` — **GiftStuddedGloves : BaseGiftArmor** — Magical item.
- `GiftStuddedGorget.cs` — **GiftStuddedGorget : BaseGiftArmor** — Magical item.
- `GiftStuddedHaidate.cs` — **GiftStuddedHaidate : BaseGiftArmor** — Magical item.
- `GiftStuddedHiroSode.cs` — **GiftStuddedHiroSode : BaseGiftArmor** — Magical item.
- `GiftStuddedLegs.cs` — **GiftStuddedLegs : BaseGiftArmor** — Magical item.
- `GiftStuddedMempo.cs` — **GiftStuddedMempo : BaseGiftArmor** — Magical item.
- `GiftStuddedSuneate.cs` — **GiftStuddedSuneate : BaseGiftArmor** — Magical item.

#### Scripts/Items/Magical/Gifts/Clothing — Magical items: Clothing. Types: BaseGiftClothing, BaseGiftCloak, BaseGiftHat, BaseGiftMiddleTorso, ....

- `BaseGiftClothing.cs` — **BaseGiftClothing : BaseClothing** — Clothing.
- `GiftCloaks.cs` — **BaseGiftCloak : BaseGiftClothing** — Magical item.
- `GiftHats.cs` — **BaseGiftHat : BaseGiftClothing** — Magical item ("bear mask").
- `GiftMiddleTorso.cs` — **BaseGiftMiddleTorso : BaseGiftClothing** — Magical item.
- `GiftOuterLegs.cs` — **BaseGiftOuterLegs : BaseGiftClothing** — Magical item.
- `GiftOuterTorso.cs` — **BaseGiftOuterTorso : BaseGiftClothing** — Magical item ("gilded dress").
- `GiftPants.cs` — **BaseGiftPants : BaseGiftClothing** — Magical item ("short pants").
- `GiftRoyalCloak.cs` — **GiftRoyalCape : BaseGiftCloak** — Magical item ("royal cloak").
- `GiftShirts.cs` — **BaseGiftShirt : BaseGiftClothing** — Magical item ("shirt").
- `GiftShoes.cs` — **GiftShoes : BaseGiftShoes** — Magical item ("fancy boots").
- `GiftWaist.cs` — **BaseGiftWaist : BaseGiftClothing** — Magical item ("loin cloth").

#### Scripts/Items/Magical/Gifts/Jewels — Magical items: Jewels. Types: BaseGiftJewel, BaseGiftBracelet, BaseGiftEarrings, GiftNecklace, ....

- `BaseGiftJewel.cs` — **BaseGiftJewel : BaseTrinket** — Trinket/jewelry.
- `GiftBracelet.cs` — **BaseGiftBracelet : BaseGiftJewel** — Magical item ("bracelet").
- `GiftEarrings.cs` — **BaseGiftEarrings : BaseGiftJewel** — Magical item ("earrings").
- `GiftNecklace.cs` — **GiftNecklace : BaseGiftNecklace** — Magical item ("beads").
- `GiftRing.cs` — **BaseGiftRing : BaseGiftJewel** — Magical item ("ring").
- `MagicBelt.cs` — **GiftBelt : GiftGoldRing** — ///////////////////////////////////////////////////////////////////////////////////// [Constructable] (+1 type)
- `MagicCandle.cs` — **GiftCandle : GiftGoldRing** — Magical item ("candle").
- `MagicLantern.cs` — **GiftLantern : GiftGoldRing** — Magical item ("lantern").
- `MagicSash.cs` — **GiftSash : GiftGoldRing** — Magical item ("sash").
- `MagicTalisman.cs` — **GiftTalismanLeather : GiftGoldRing** — /////////////////////////////////////////////////////////////////// [Constructable] (+3 types)
- `MagicTorch.cs` — **GiftTorch : GiftGoldRing** — Magical item ("torch").

#### Scripts/Items/Magical/Gifts/Shields — Magical items: Shields. Types: BaseGiftShield, GiftBronzeShield, GiftBuckler, GiftChampionShield, ....

- `BaseGiftShield.cs` — **BaseGiftShield : BaseShield** — Shield.
- `GiftBronzeShield.cs` — **GiftBronzeShield : BaseGiftShield** — Magical item ("large shield").
- `GiftBuckler.cs` — **GiftBuckler : BaseGiftShield** — Magical item.
- `GiftChampionShield.cs` — **GiftChampionShield : BaseGiftShield** — Magical item ("champion shield").
- `GiftChaosShield.cs` — **GiftChaosShield : BaseGiftShield** — Magical item.
- `GiftCrestedShield.cs` — **GiftCrestedShield : BaseGiftShield** — Magical item ("crested shield").
- `GiftDarkShield.cs` — **GiftDarkShield : BaseGiftShield** — Magical item ("dark shield").
- `GiftElvenShield.cs` — **GiftElvenShield : BaseGiftShield** — Magical item ("elven shield").
- `GiftGuardsmanShield.cs` — **GiftGuardsmanShield : BaseGiftShield** — Magical item ("guardsman shield").
- `GiftHeaterShield.cs` — **GiftHeaterShield : BaseGiftShield** — Magical item ("heater shield").
- `GiftJeweledShield.cs` — **GiftJeweledShield : BaseGiftShield** — Magical item ("jeweled shield").
- `GiftMetalKiteShield.cs` — **GiftMetalKiteShield : BaseGiftShield** — Magical item ("metal kite shield").
- `GiftMetalShield.cs` — **GiftMetalShield : BaseGiftShield** — Magical item ("metal shield").
- `GiftOrderShield.cs` — **GiftOrderShield : BaseGiftShield** — Magical item.
- `GiftRoyalShield.cs` — **GiftRoyalShield : GiftHeaterShield** — Magical item ("royal shield").
- `GiftScalemailShield.cs` — **GiftScalemailShield : BaseGiftShield** — Magical item ("scalemail shield").
- `GiftSunShield.cs` — **GiftSunShield : BaseGiftShield** — Magical item ("sun shield").
- `GiftVirtueShield.cs` — **GiftVirtueShield : BaseGiftShield** — Magical item ("virtue shield").
- `GiftWoodenKiteShield.cs` — **GiftWoodenKiteShield : BaseGiftShield** — Magical item ("wooden kite shield").
- `GiftWoodenShield.cs` — **GiftWoodenShield : BaseGiftShield** — Magical item ("wooden shield").

#### Scripts/Items/Magical/Gifts/Weapons — Magical items: Weapons. Types: GiftHarpoon, GiftPugilistGloves, GiftStave, GiftThrowingGloves.

- `GiftHarpoon.cs` — **GiftHarpoon : BaseGiftRanged** — Magical item ("harpoon").
- `GiftPugilistMits.cs` — **GiftPugilistGloves : BaseGiftBashing** — Magical item ("pugilist gloves").
- `GiftStave.cs` — **GiftStave : BaseGiftStave** — Magical item ("stave").
- `GiftThrowingGloves.cs` — **GiftThrowingGloves : BaseGiftRanged** — Magical item ("throwing gloves").

#### Scripts/Items/Magical/Gifts/Weapons/Axes — Magical items: Axes. Types: BaseGiftAxe, GiftAxe, GiftBattleAxe, GiftDoubleAxe, ....

- `BaseGiftAxe.cs` — **BaseGiftAxe : BaseAxe** — Axe weapon.
- `GiftAxe.cs` — **GiftAxe : BaseGiftAxe** — Magical item ("axe").
- `GiftBattleAxe.cs` — **GiftBattleAxe : BaseGiftAxe** — Magical item ("battle axe").
- `GiftDoubleAxe.cs` — **GiftDoubleAxe : BaseGiftAxe** — Magical item.
- `GiftExecutionersAxe.cs` — **GiftExecutionersAxe : BaseGiftAxe** — Magical item ("great axe").
- `GiftHatchet.cs` — **GiftHatchet : BaseGiftAxe** — Magical item.
- `GiftLargeBattleAxe.cs` — **GiftLargeBattleAxe : BaseGiftAxe** — Magical item ("large battle axe").
- `GiftPickaxe.cs` — **GiftPickaxe : BaseGiftAxe** — Magical item.
- `GiftTwoHandedAxe.cs` — **GiftTwoHandedAxe : BaseGiftAxe** — Magical item ("two handed axe").
- `GiftWarAxe.cs` — **GiftWarAxe : BaseGiftAxe** — Magical item ("war axe").

#### Scripts/Items/Magical/Gifts/Weapons/Knives — Magical items: Knives. Types: BaseGiftKnife, GiftButcherKnife, GiftCleaver, GiftDagger, ....

- `BaseGiftKnife.cs` — **BaseGiftKnife : BaseKnife** — Knife weapon.
- `GiftButcherKnife.cs` — **GiftButcherKnife : BaseGiftKnife** — Magical item.
- `GiftCleaver.cs` — **GiftCleaver : BaseGiftKnife** — Magical item ("cleaver").
- `GiftDagger.cs` — **GiftDagger : BaseGiftKnife** — Magical item ("dagger").
- `GiftLargeKnife.cs` — **GiftLargeKnife : BaseGiftKnife** — Magical item ("large knife").

#### Scripts/Items/Magical/Gifts/Weapons/ML Weapons — Magical items: Ml weapons. Types: GiftAssassinSpike, GiftDiamondMace, GiftElvenCompositeLongbow, GiftElvenMachete, ....

- `GiftAssassinSpike.cs` — **GiftAssassinSpike : BaseGiftKnife** — Magical item ("assassin dagger").
- `GiftDiamondMace.cs` — **GiftDiamondMace : BaseGiftBashing** — Magical item ("battle mace").
- `GiftElvenCompositeLongbow.cs` — **GiftElvenCompositeLongbow : BaseGiftRanged** — Magical item ("woodland longbow").
- `GiftElvenMachete.cs` — **GiftElvenMachete : BaseGiftSword** — Magical item ("machete").
- `GiftElvenSpellblade.cs` — **GiftElvenSpellblade : BaseGiftKnife** — Magical item ("assassin sword").
- `GiftLeafblade.cs` — **GiftLeafblade : BaseGiftKnife** — Magical item ("war dagger").
- `GiftMagicalShortbow.cs` — **GiftMagicalShortbow : BaseGiftRanged** — Magical item ("woodland shortbow").
- `GiftOrnateAxe.cs` — **GiftOrnateAxe : BaseGiftAxe** — Magical item ("barbarian axe").
- `GiftRadiantScimitar.cs` — **GiftRadiantScimitar : BaseGiftSword** — Magical item ("falchion").
- `GiftRuneBlade.cs` — **GiftRuneBlade : BaseGiftSword** — Magical item ("war blades").
- `GiftWarCleaver.cs` — **GiftWarCleaver : BaseGiftKnife** — Magical item.
- `GiftWildStaff.cs` — **GiftWildStaff : BaseGiftStaff** — Magical item ("druid staff").

#### Scripts/Items/Magical/Gifts/Weapons/Maces — Magical items: Maces. Types: BaseGiftBashing, BaseGiftWhip, GiftClub, GiftHammerPick, ....

- `BaseGiftBashing.cs` — **BaseGiftBashing : BaseBashing** — Mace/hammer weapon.
- `BaseGiftWhip.cs` — **BaseGiftWhip : BaseWhip** — Magical item ("whip").
- `GiftClub.cs` — **GiftClub : BaseGiftBashing** — Magical item ("club").
- `GiftHammerPick.cs` — **GiftHammerPick : BaseGiftBashing** — Magical item ("hammer pick").
- `GiftHammers.cs` — **GiftHammers : BaseGiftBashing** — Magical item ("hammer").
- `GiftMace.cs` — **GiftMace : BaseGiftBashing** — Magical item ("mace").
- `GiftMaul.cs` — **GiftMaul : BaseGiftBashing** — Magical item.
- `GiftScepter.cs` — **GiftScepter : BaseGiftBashing** — Magical item.
- `GiftSpikedClub.cs` — **GiftSpikedClub : BaseGiftBashing** — Magical item ("spiked club").
- `GiftWarHammer.cs` — **GiftWarHammer : BaseGiftBashing** — Magical item ("war hammer").
- `GiftWarMace.cs` — **GiftWarMace : BaseGiftBashing** — Magical item ("war mace").
- `GiftWhip.cs` — **GiftWhips : BaseGiftWhip** — Magical item.

#### Scripts/Items/Magical/Gifts/Weapons/PoleArms — Magical items: Pole arms. Types: BaseGiftPoleArm, GiftBardiche, GiftHalberd, GiftScythe.

- `BaseGiftPoleArm.cs` — **BaseGiftPoleArm : BasePoleArm** — Polearm weapon.
- `GiftBardiche.cs` — **GiftBardiche : BaseGiftPoleArm** — Magical item.
- `GiftHalberd.cs` — **GiftHalberd : BaseGiftPoleArm** — Magical item ("halberd").
- `GiftScythe.cs` — **GiftScythe : BaseGiftPoleArm** — Magical item ("scythe").

#### Scripts/Items/Magical/Gifts/Weapons/Ranged — Magical items: Ranged. Types: BaseGiftRanged, GiftBow, GiftCompositeBow, GiftCrossbow, ....

- `BaseGiftRanged.cs` — **BaseGiftRanged : BaseRanged** — Ranged weapon.
- `GiftBow.cs` — **GiftBow : BaseGiftRanged** — Magical item ("bow").
- `GiftCompositeBow.cs` — **GiftCompositeBow : BaseGiftRanged** — Magical item ("composite bow").
- `GiftCrossbow.cs` — **GiftCrossbow : BaseGiftRanged** — Magical item ("crossbow").
- `GiftHeavyCrossbow.cs` — **GiftHeavyCrossbow : BaseGiftRanged** — Magical item.
- `GiftRepeatingCrossbow.cs` — **GiftRepeatingCrossbow : BaseGiftRanged** — Magical item.

#### Scripts/Items/Magical/Gifts/Weapons/SE Weapons — Magical items: Se weapons. Types: GiftBokuto, GiftDaisho, GiftKama, GiftLajatang, ....

- `GiftBokuto.cs` — **GiftBokuto : BaseGiftSword** — Magical item.
- `GiftDaisho.cs` — **GiftDaisho : BaseGiftSword** — Magical item.
- `GiftKama.cs` — **GiftKama : BaseGiftKnife** — Magical item.
- `GiftLajatang.cs` — **GiftLajatang : BaseGiftKnife** — Magical item.
- `GiftNoDachi.cs` — **GiftNoDachi : BaseGiftSword** — Magical item.
- `GiftNunchaku.cs` — **GiftNunchaku : BaseGiftBashing** — Magical item.
- `GiftSai.cs` — **GiftSai : BaseGiftKnife** — Magical item.
- `GiftTekagi.cs` — **GiftTekagi : BaseGiftKnife** — Magical item.
- `GiftTessen.cs` — **GiftTessen : BaseGiftBashing** — Magical item.
- `GiftTetsubo.cs` — **GiftTetsubo : BaseGiftBashing** — Magical item.
- `GiftWakizashi.cs` — **GiftWakizashi : BaseGiftSword** — Magical item.
- `GiftYumi.cs` — **GiftYumi : BaseGiftRanged** — Magical item ("yumi").

#### Scripts/Items/Magical/Gifts/Weapons/SpearsAndForks — Magical items: Spears and forks. Types: BaseGiftSpear, GiftBladedStaff, GiftDoubleBladedStaff, GiftPike, ....

- `BaseGiftSpear.cs` — **BaseGiftSpear : BaseSpear** — Spear weapon.
- `GiftBladedStaff.cs` — **GiftBladedStaff : BaseGiftSpear** — Magical item.
- `GiftDoubleBladedStaff.cs` — **GiftDoubleBladedStaff : BaseGiftSpear** — Magical item ("double bladed staff").
- `GiftPike.cs` — **GiftPike : BaseGiftSpear** — Magical item.
- `GiftPitchfork.cs` — **GiftPitchfork : BaseGiftSpear** — Magical item ("trident").
- `GiftShortSpear.cs` — **GiftShortSpear : BaseGiftSpear** — Magical item ("rapier").
- `GiftSpear.cs` — **GiftSpear : BaseGiftSpear** — Magical item ("spear").
- `GiftTribalSpear.cs` — **GiftTribalSpear : BaseGiftSpear** — Magical item.
- `GiftWarFork.cs` — **GiftWarFork : BaseGiftSpear** — Magical item ("war fork").

#### Scripts/Items/Magical/Gifts/Weapons/Staves — Magical items: Staves. Types: BaseGiftStaff, GiftBlackStaff, GiftGnarledStaff, GiftQuarterStaff, ....

- `BaseGiftStaff.cs` — **BaseGiftStaff : BaseStaff** — Staff weapon.
- `GiftBlackStaff.cs` — **GiftBlackStaff : BaseGiftStaff** — Magical item ("wizard staff").
- `GiftGnarledStaff.cs` — **GiftGnarledStaff : BaseGiftStaff** — Magical item.
- `GiftQuarterStaff.cs` — **GiftQuarterStaff : BaseGiftStaff** — Magical item.
- `GiftShepherdsCrook.cs` — **GiftShepherdsCrook : BaseGiftStaff** — Magical item.

#### Scripts/Items/Magical/Gifts/Weapons/Swords — Magical items: Swords. Types: BaseGiftSword, GiftBoneHarvester, GiftBroadsword, GiftClaymore, ....

- `BaseGiftSword.cs` — **BaseGiftSword : BaseSword** — Sword weapon.
- `GiftBoneHarvester.cs` — **GiftBoneHarvester : BaseGiftSword** — Magical item ("sickle").
- `GiftBroadsword.cs` — **GiftBroadsword : BaseGiftSword** — Magical item ("broadsword").
- `GiftClaymore.cs` — **GiftClaymore : BaseGiftSword** — Magical item ("claymore").
- `GiftCrescentBlade.cs` — **GiftCrescentBlade : BaseGiftSword** — Magical item.
- `GiftCutlass.cs` — **GiftCutlass : BaseGiftSword** — Magical item ("cutlass").
- `GiftKatana.cs` — **GiftKatana : BaseGiftSword** — Magical item ("katana").
- `GiftKryss.cs` — **GiftKryss : BaseGiftSword** — Magical item.
- `GiftLance.cs` — **GiftLance : BaseGiftSword** — Magical item.
- `GiftLongsword.cs` — **GiftLongsword : BaseGiftSword** — Magical item ("longsword").
- `GiftRoyalSword.cs` — **GiftRoyalSword : GiftVikingSword** — Magical item ("royal sword").
- `GiftScimitar.cs` — **GiftScimitar : BaseGiftSword** — Magical item ("scimitar").
- `GiftShortSword.cs` — **GiftShortSword : BaseGiftSword** — Magical item ("short sword").
- `GiftThinLongsword.cs` — **GiftThinLongsword : BaseGiftSword** — Magical item ("sword").
- `GiftVikingSword.cs` — **GiftVikingSword : BaseGiftSword** — Magical item ("barbarian sword").

#### Scripts/Items/Magical/God — Magical items: God. Types: ILevelable, ItemExperienceToken, LegendaryArtifactRename, LegendsBook, ....

- `ILevelable.cs` — **ILevelable** — Summary description for ILevelable.
- `ItemExperienceToken.cs` — **ItemExperienceToken : Item** — Item ("Experience token"). [Constructable] (+1 type)
- `LegendaryArtifactRename.cs` — **LegendaryArtifactRename : Item** — Item ("Legendary Branding Iron"). [Constructable] (+2 types)
- `LegendsBook.cs` — **LegendsBook : Item** — Item ("Legendary Artefacts"). [Constructable] (+1 type)
- `LevelAttributes.cs` — **LevelAttributes** — Magical item.
- `LevelInfoEntry.cs` — **LevelInfoEntry : ContextMenuEntry** — Context-menu entry.
- `LevelItemManager.cs` — **LevelItemManager** — Manager/service.
- `LevelUpScroll.cs` — **LevelUpScroll : Item** — Item ("Enhancement Rune"). [Constructable] (+2 types)
- `LevelableItemSettings.cs` — **LevelItems** — Magical item.

#### Scripts/Items/Magical/God/Armor — Magical items: Armor. Types: BaseLevelArmor, LevelWoodenPlateLegs.

- `BaseLevelArmor.cs` — **BaseLevelArmor : BaseArmor** — Armor.
- `LevelWoodenArmor.cs` — **LevelWoodenPlateLegs : BaseLevelArmor ///////////////////////////////////////////////////////** — Magical item ("wooden leggings").

#### Scripts/Items/Magical/God/Armor/Bone — Magical items: Bone. Types: LevelBoneArms, LevelBoneChest, LevelBoneGloves, LevelBoneLegs.

- `LevelBoneArms.cs` — **LevelBoneArms : BaseLevelArmor** — Magical item.
- `LevelBoneChest.cs` — **LevelBoneChest : BaseLevelArmor** — Magical item.
- `LevelBoneGloves.cs` — **LevelBoneGloves : BaseLevelArmor** — Magical item.
- `LevelBoneLegs.cs` — **LevelBoneLegs : BaseLevelArmor** — Magical item.

#### Scripts/Items/Magical/God/Armor/Chain — Magical items: Chain. Types: LevelChainChest, LevelChainHatsuburi, LevelChainLegs.

- `LevelChainChest.cs` — **LevelChainChest : BaseLevelArmor** — Magical item ("chainmail tunic").
- `LevelChainHatsuburi.cs` — **LevelChainHatsuburi : BaseLevelArmor** — Magical item.
- `LevelChainLegs.cs` — **LevelChainLegs : BaseLevelArmor** — Magical item ("chainmail leggings").

#### Scripts/Items/Magical/God/Armor/Dragon — Magical items: Dragon. Types: LevelDragonArms, LevelDragonChest, LevelDragonGloves, LevelDragonHelm, ....

- `LevelDragonArms.cs` — **LevelDragonArms : BaseLevelArmor** — Magical item ("dragon scale arms").
- `LevelDragonChest.cs` — **LevelDragonChest : BaseLevelArmor** — Magical item ("dragon scale tunic").
- `LevelDragonGloves.cs` — **LevelDragonGloves : BaseLevelArmor** — Magical item ("dragon scale gloves").
- `LevelDragonHelm.cs` — **LevelDragonHelm : BaseLevelArmor** — Magical item ("dragon scale helm").
- `LevelDragonLegs.cs` — **LevelDragonLegs : BaseLevelArmor** — Magical item ("dragon scale leggings").

#### Scripts/Items/Magical/God/Armor/Helmets — Magical items: Helmets. Types: LevelBascinet, LevelBoneHelm, LevelChainCoif, LevelCirclet, ....

- `LevelBascinet.cs` — **LevelBascinet : BaseLevelArmor** — Magical item ("bascinet").
- `LevelBoneHelm.cs` — **LevelBoneHelm : BaseLevelArmor** — Magical item.
- `LevelChainCoif.cs` — **LevelChainCoif : BaseLevelArmor** — Magical item ("chainmail coif").
- `LevelCirclet.cs` — **LevelCirclet : BaseLevelArmor** — Magical item ("circlet").
- `LevelCloseHelm.cs` — **LevelCloseHelm : BaseLevelArmor** — Magical item.
- `LevelDreadHelm.cs` — **LevelDreadHelm : BaseLevelArmor** — Magical item ("dread helm").
- `LevelHelmet.cs` — **LevelHelmet : BaseLevelArmor** — Magical item.
- `LevelLeatherCap.cs` — **LevelLeatherCap : BaseLevelArmor** — Magical item.
- `LevelNorseHelm.cs` — **LevelNorseHelm : BaseLevelArmor** — Magical item ("norse helm").
- `LevelOniwabanHood.cs` — **LevelOniwabanHood : LevelLeatherCap** — Magical item ("oniwaban hood").
- `LevelOrcHelm.cs` — **LevelOrcHelm : BaseLevelArmor** — Magical item ("horned helm").
- `LevelPlateHelm.cs` — **LevelPlateHelm : BaseLevelArmor** — Magical item ("plate helm").
- `LevelRoyalHelm.cs` — **LevelRoyalHelm : LevelPlateHelm** — Magical item ("royal helm").

#### Scripts/Items/Magical/God/Armor/Leather — Magical items: Leather. Types: LevelFemaleLeatherChest, LevelLeatherArms, LevelLeatherBustierArms, LevelLeatherChest, ....

- `LevelFemaleLeatherChest.cs` — **LevelFemaleLeatherChest : BaseLevelArmor** — Magical item.
- `LevelLeatherArms.cs` — **LevelLeatherArms : BaseLevelArmor** — Magical item.
- `LevelLeatherBustierArms.cs` — **LevelLeatherBustierArms : BaseLevelArmor** — Magical item.
- `LevelLeatherChest.cs` — **LevelLeatherChest : BaseLevelArmor** — Magical item ("leather tunic").
- `LevelLeatherCloak.cs` — **LevelLeatherCloak : BaseLevelArmor** — Magical item ("leather cloak").
- `LevelLeatherDo.cs` — **LevelLeatherDo : BaseLevelArmor** — Magical item.
- `LevelLeatherGloves.cs` — **LevelLeatherGloves : BaseLevelArmor** — Magical item.
- `LevelLeatherGorget.cs` — **LevelLeatherGorget : BaseLevelArmor** — Magical item.
- `LevelLeatherHaidate.cs` — **LevelLeatherHaidate : BaseLevelArmor** — Magical item.
- `LevelLeatherHiroSode.cs` — **LevelLeatherHiroSode : BaseLevelArmor** — Magical item.
- `LevelLeatherJingasa.cs` — **LevelLeatherJingasa : BaseLevelArmor** — Magical item.
- `LevelLeatherLegs.cs` — **LevelLeatherLegs : BaseLevelArmor** — Magical item.
- `LevelLeatherMempo.cs` — **LevelLeatherMempo : BaseLevelArmor** — Magical item.
- `LevelLeatherNinjaHood.cs` — **LevelLeatherNinjaHood : BaseLevelArmor** — Magical item.
- `LevelLeatherNinjaJacket.cs` — **LevelLeatherNinjaJacket : BaseLevelArmor** — Magical item.
- `LevelLeatherNinjaMitts.cs` — **LevelLeatherNinjaMitts : BaseLevelArmor** — Magical item.
- `LevelLeatherNinjaPants.cs` — **LevelLeatherNinjaPants : BaseLevelArmor** — Magical item.
- `LevelLeatherRobe.cs` — **LevelLeatherRobe : BaseLevelArmor** — Magical item ("leather robe").
- `LevelLeatherShorts.cs` — **LevelLeatherShorts : BaseLevelArmor** — Magical item.
- `LevelLeatherSkirt.cs` — **LevelLeatherSkirt : BaseLevelArmor** — Magical item.
- `LevelLeatherSuneate.cs` — **LevelLeatherSuneate : BaseLevelArmor** — Magical item.
- `LevelOniwabanBoots.cs` — **LevelOniwabanBoots : BaseLevelArmor** — Magical item ("oniwaban boots").
- `LevelOniwabanGloves.cs` — **LevelOniwabanGloves : LevelLeatherGloves** — Magical item ("oniwaban gloves").
- `LevelOniwabanLeggings.cs` — **LevelOniwabanLeggings : LevelLeatherLegs** — Magical item ("oniwaban leggings").
- `LevelOniwabanTunic.cs` — **LevelOniwabanTunic : LevelLeatherChest** — Magical item ("oniwaban tunic").
- `LevelShinobiGarb.cs` — **LevelShinobiRobe : LevelLeatherRobe** — Magical item ("leather shinobi robe").

#### Scripts/Items/Magical/God/Armor/Plate — Magical items: Plate. Types: LevelDecorativePlateKabuto, LevelFemalePlateChest, LevelHeavyPlateJingasa, LevelLightPlateJingasa, ....

- `LevelDecorativePlateKabuto.cs` — **LevelDecorativePlateKabuto : BaseLevelArmor** — Magical item.
- `LevelFemalePlateChest.cs` — **LevelFemalePlateChest : BaseLevelArmor** — Magical item.
- `LevelHeavyPlateJingasa.cs` — **LevelHeavyPlateJingasa : BaseLevelArmor** — Magical item.
- `LevelLightPlateJingasa.cs` — **LevelLightPlateJingasa : BaseLevelArmor** — Magical item.
- `LevelPlateArms.cs` — **LevelPlateArms : BaseLevelArmor** — Magical item ("platemail arms").
- `LevelPlateBattleKabuto.cs` — **LevelPlateBattleKabuto : BaseLevelArmor** — Magical item.
- `LevelPlateChest.cs` — **LevelPlateChest : BaseLevelArmor** — Magical item ("platemail").
- `LevelPlateDo.cs` — **LevelPlateDo : BaseLevelArmor** — Magical item.
- `LevelPlateGloves.cs` — **LevelPlateGloves : BaseLevelArmor** — Magical item.
- `LevelPlateGorget.cs` — **LevelPlateGorget : BaseLevelArmor** — Magical item.
- `LevelPlateHaidate.cs` — **LevelPlateHaidate : BaseLevelArmor** — Magical item.
- `LevelPlateHatsuburi.cs` — **LevelPlateHatsuburi : BaseLevelArmor** — Magical item.
- `LevelPlateHiroSode.cs` — **LevelPlateHiroSode : BaseLevelArmor** — Magical item.
- `LevelPlateLegs.cs` — **LevelPlateLegs : BaseLevelArmor** — Magical item ("platemail legs").
- `LevelPlateMempo.cs` — **LevelPlateMempo : BaseLevelArmor** — Magical item.
- `LevelPlateSuneate.cs` — **LevelPlateSuneate : BaseLevelArmor** — Magical item.
- `LevelSmallPlateJingasa.cs` — **LevelSmallPlateJingasa : BaseLevelArmor** — Magical item.
- `LevelStandardPlateKabuto.cs` — **LevelStandardPlateKabuto : BaseLevelArmor** — Magical item.

#### Scripts/Items/Magical/God/Armor/Ring — Magical items: Ring. Types: LevelRingmailArms, LevelRingmailChest, LevelRingmailGloves, LevelRingmailLegs.

- `LevelRingmailArms.cs` — **LevelRingmailArms : BaseLevelArmor** — Magical item.
- `LevelRingmailChest.cs` — **LevelRingmailChest : BaseLevelArmor** — Magical item.
- `LevelRingmailGloves.cs` — **LevelRingmailGloves : BaseLevelArmor** — Magical item.
- `LevelRingmailLegs.cs` — **LevelRingmailLegs : BaseLevelArmor** — Magical item.

#### Scripts/Items/Magical/God/Armor/Royal — Magical items: Royal. Types: LevelRoyalArms, LevelRoyalBoots, LevelRoyalChest, LevelRoyalGloves, ....

- `LevelRoyalArms.cs` — **LevelRoyalArms : LevelPlateArms** — Magical item ("royal mantle").
- `LevelRoyalBoots.cs` — **LevelRoyalBoots : LevelPlateGorget** — Magical item ("royal boots").
- `LevelRoyalChest.cs` — **LevelRoyalChest : LevelPlateChest** — Magical item ("royal tunic").
- `LevelRoyalGloves.cs` — **LevelRoyalGloves : LevelPlateGloves** — Magical item ("royal bracers").
- `LevelRoyalGorget.cs` — **LevelRoyalGorget : LevelPlateGorget** — Magical item ("royal gorget").
- `LevelRoyalLegs.cs` — **LevelRoyalsLegs : LevelPlateLegs** — Magical item ("royal leggings").

#### Scripts/Items/Magical/God/Armor/Studded — Magical items: Studded. Types: LevelFemaleStuddedChest, LevelStuddedArms, LevelStuddedBustierArms, LevelStuddedChest, ....

- `LevelFemaleStuddedChest.cs` — **LevelFemaleStuddedChest : BaseLevelArmor** — Magical item.
- `LevelStuddedArms.cs` — **LevelStuddedArms : BaseLevelArmor** — Magical item.
- `LevelStuddedBustierArms.cs` — **LevelStuddedBustierArms : BaseLevelArmor** — Magical item.
- `LevelStuddedChest.cs` — **LevelStuddedChest : BaseLevelArmor** — Magical item.
- `LevelStuddedDo.cs` — **LevelStuddedDo : BaseLevelArmor** — Magical item.
- `LevelStuddedGloves.cs` — **LevelStuddedGloves : BaseLevelArmor** — Magical item.
- `LevelStuddedGorget.cs` — **LevelStuddedGorget : BaseLevelArmor** — Magical item.
- `LevelStuddedHaidate.cs` — **LevelStuddedHaidate : BaseLevelArmor** — Magical item.
- `LevelStuddedHiroSode.cs` — **LevelStuddedHiroSode : BaseLevelArmor** — Magical item.
- `LevelStuddedLegs.cs` — **LevelStuddedLegs : BaseLevelArmor** — Magical item.
- `LevelStuddedMempo.cs` — **LevelStuddedMempo : BaseLevelArmor** — Magical item.
- `LevelStuddedSuneate.cs` — **LevelStuddedSuneate : BaseLevelArmor** — Magical item.

#### Scripts/Items/Magical/God/Clothing — Magical items: Clothing. Types: BaseLevelClothing, BaseLevelCloak, BaseLevelHat, BaseLevelMiddleTorso, ....

- `BaseLevelClothing.cs` — **BaseLevelClothing : BaseClothing** — Clothing.
- `LevelCloaks.cs` — **BaseLevelCloak : BaseLevelClothing** — Magical item.
- `LevelHats.cs` — **BaseLevelHat : BaseLevelClothing** — Magical item ("bear mask").
- `LevelMiddleTorso.cs` — **BaseLevelMiddleTorso : BaseLevelClothing** — Magical item.
- `LevelOuterLegs.cs` — **BaseLevelOuterLegs : BaseLevelClothing** — Magical item.
- `LevelOuterTorso.cs` — **BaseLevelOuterTorso : BaseLevelClothing** — Magical item ("gilded dress").
- `LevelPants.cs` — **BaseLevelPants : BaseLevelClothing** — Magical item ("short pants").
- `LevelRoyalCloak.cs` — **LevelRoyalCape : BaseLevelCloak** — Magical item ("royal cloak").
- `LevelShirts.cs` — **BaseLevelShirt : BaseLevelClothing** — Magical item ("shirt").
- `LevelShoes.cs` — **LevelShoes : BaseLevelShoes** — Magical item ("fancy boots").
- `LevelWaist.cs` — **BaseLevelWaist : BaseLevelClothing** — Magical item ("loin cloth").

#### Scripts/Items/Magical/God/Commands — Magical items: Commands. Types: ixpCommand.

- `ItemExpCommand.cs` — **ixpCommand** — Commands: [itemexp, [ixp (+1 type)

#### Scripts/Items/Magical/God/Gumps — Magical items: Gumps. Types: AwaitingSmithApprovalGump, ItemExperienceGump, LevelUpAcceptGump.

- `AwaitingSmithApprovalGump.cs` — **AwaitingSmithApprovalGump : Gump** — Gump UI.
- `ItemExperienceGump.cs` — **ItemExperienceGump : Gump** — Gump UI. (+1 type)
- `LevelUpAcceptGump.cs` — **LevelUpAcceptGump : Gump** — Gump UI.

#### Scripts/Items/Magical/God/Jewels — Magical items: Jewels. Types: BaseLevelJewel, BaseLevelBracelet, BaseLevelEarrings, LevelNecklace, ....

- `BaseLevelJewel.cs` — **BaseLevelJewel : BaseTrinket** — Trinket/jewelry.
- `LevelBracelet.cs` — **BaseLevelBracelet : BaseLevelJewel** — Magical item ("bracelet").
- `LevelEarrings.cs` — **BaseLevelEarrings : BaseLevelJewel** — Magical item ("earrings").
- `LevelNecklace.cs` — **LevelNecklace : BaseLevelNecklace** — Magical item ("beads").
- `LevelRing.cs` — **BaseLevelRing : BaseLevelJewel** — Magical item ("ring").
- `MagicBelt.cs` — **LevelBelt : LevelGoldRing** — ///////////////////////////////////////////////////////////////////////////////////// [Constructable] (+1 type)
- `MagicCandle.cs` — **LevelCandle : LevelGoldRing** — Magical item ("candle").
- `MagicLantern.cs` — **LevelLantern : LevelGoldRing** — Magical item ("lantern").
- `MagicSash.cs` — **LevelSash : LevelGoldRing** — Magical item ("sash").
- `MagicTalisman.cs` — **LevelTalismanLeather : LevelGoldRing** — /////////////////////////////////////////////////////////////////// [Constructable] (+3 types)
- `MagicTorch.cs` — **LevelTorch : LevelGoldRing** — Magical item ("torch").

#### Scripts/Items/Magical/God/Shields — Magical items: Shields. Types: BaseLevelShield, LevelBronzeShield, LevelBuckler, LevelChampionShield, ....

- `BaseLevelShield.cs` — **BaseLevelShield : BaseShield** — Shield.
- `LevelBronzeShield.cs` — **LevelBronzeShield : BaseLevelShield** — Magical item ("large shield").
- `LevelBuckler.cs` — **LevelBuckler : BaseLevelShield** — Magical item.
- `LevelChampionShield.cs` — **LevelChampionShield : BaseLevelShield** — Magical item ("champion shield").
- `LevelChaosShield.cs` — **LevelChaosShield : BaseLevelShield** — Magical item.
- `LevelCrestedShield.cs` — **LevelCrestedShield : BaseLevelShield** — Magical item ("crested shield").
- `LevelDarkShield.cs` — **LevelDarkShield : BaseLevelShield** — Magical item ("dark shield").
- `LevelElvenShield.cs` — **LevelElvenShield : BaseLevelShield** — Magical item ("elven shield").
- `LevelGuardsmanShield.cs` — **LevelGuardsmanShield : BaseLevelShield** — Magical item ("guardsman shield").
- `LevelHeaterShield.cs` — **LevelHeaterShield : BaseLevelShield** — Magical item ("heater shield").
- `LevelJeweledShield.cs` — **LevelJeweledShield : BaseLevelShield** — Magical item ("jeweled shield").
- `LevelMetalKiteShield.cs` — **LevelMetalKiteShield : BaseLevelShield** — Magical item ("metal kite shield").
- `LevelMetalShield.cs` — **LevelMetalShield : BaseLevelShield** — Magical item ("metal shield").
- `LevelOrderShield.cs` — **LevelOrderShield : BaseLevelShield** — Magical item.
- `LevelRoyalShield.cs` — **LevelRoyalShield : LevelHeaterShield** — Magical item ("royal shield").
- `LevelScalemailShield.cs` — **LevelScalemailShield : BaseLevelShield** — Magical item ("scalemail shield").
- `LevelSunShield.cs` — **LevelSunShield : BaseLevelShield** — Magical item ("sun shield").
- `LevelVirtueShield.cs` — **LevelVirtueShield : BaseLevelShield** — Magical item ("virtue shield").
- `LevelWoodenKiteShield.cs` — **LevelWoodenKiteShield : BaseLevelShield** — Magical item ("wooden kite shield").
- `LevelWoodenShield.cs` — **LevelWoodenShield : BaseLevelShield** — Magical item ("wooden shield").

#### Scripts/Items/Magical/God/Weapons — Magical items: Weapons. Types: LevelHarpoon, LevelPugilistGloves, LevelStave, LevelThrowingGloves.

- `LevelHarpoon.cs` — **LevelHarpoon : BaseLevelRanged** — Magical item ("harpoon").
- `LevelPugilistMits.cs` — **LevelPugilistGloves : BaseLevelBashing** — Magical item ("pugilist gloves").
- `LevelStave.cs` — **LevelStave : BaseLevelStave** — Magical item ("stave").
- `LevelThrowingGloves.cs` — **LevelThrowingGloves : BaseLevelRanged** — Magical item ("throwing gloves").

#### Scripts/Items/Magical/God/Weapons/Axes — Magical items: Axes. Types: BaseLevelAxe, LevelAxe, LevelBattleAxe, LevelDoubleAxe, ....

- `BaseLevelAxe.cs` — **BaseLevelAxe : BaseAxe** — Axe weapon.
- `LevelAxe.cs` — **LevelAxe : BaseLevelAxe** — Magical item ("axe").
- `LevelBattleAxe.cs` — **LevelBattleAxe : BaseLevelAxe** — Magical item ("battle axe").
- `LevelDoubleAxe.cs` — **LevelDoubleAxe : BaseLevelAxe** — Magical item.
- `LevelExecutionersAxe.cs` — **LevelExecutionersAxe : BaseLevelAxe** — Magical item ("great axe").
- `LevelHatchet.cs` — **LevelHatchet : BaseLevelAxe** — Magical item.
- `LevelLargeBattleAxe.cs` — **LevelLargeBattleAxe : BaseLevelAxe** — Magical item ("large battle axe").
- `LevelPickaxe.cs` — **LevelPickaxe : BaseLevelAxe** — Magical item.
- `LevelTwoHandedAxe.cs` — **LevelTwoHandedAxe : BaseLevelAxe** — Magical item ("two handed axe").
- `LevelWarAxe.cs` — **LevelWarAxe : BaseLevelAxe** — Magical item ("war axe").

#### Scripts/Items/Magical/God/Weapons/Knives — Magical items: Knives. Types: BaseLevelKnife, LevelButcherKnife, LevelCleaver, LevelDagger, ....

- `BaseLevelKnife.cs` — **BaseLevelKnife : BaseKnife** — Knife weapon.
- `LevelButcherKnife.cs` — **LevelButcherKnife : BaseLevelKnife** — Magical item.
- `LevelCleaver.cs` — **LevelCleaver : BaseLevelKnife** — Magical item ("cleaver").
- `LevelDagger.cs` — **LevelDagger : BaseLevelKnife** — Magical item ("dagger").
- `LevelLargeKnife.cs` — **LevelLargeKnife : BaseLevelKnife** — Magical item ("large knife").

#### Scripts/Items/Magical/God/Weapons/ML Weapons — Magical items: Ml weapons. Types: LevelAssassinSpike, LevelDiamondMace, LevelElvenCompositeLongbow, LevelElvenMachete, ....

- `LevelAssassinSpike.cs` — **LevelAssassinSpike : BaseLevelKnife** — Magical item ("assassin dagger").
- `LevelDiamondMace.cs` — **LevelDiamondMace : BaseLevelBashing** — Magical item ("battle mace").
- `LevelElvenCompositeLongbow.cs` — **LevelElvenCompositeLongbow : BaseLevelRanged** — Magical item ("woodland longbow").
- `LevelElvenMachete.cs` — **LevelElvenMachete : BaseLevelSword** — Magical item ("machete").
- `LevelElvenSpellblade.cs` — **LevelElvenSpellblade : BaseLevelKnife** — Magical item ("assassin sword").
- `LevelLeafblade.cs` — **LevelLeafblade : BaseLevelKnife** — Magical item ("war dagger").
- `LevelMagicalShortbow.cs` — **LevelMagicalShortbow : BaseLevelRanged** — Magical item ("woodland shortbow").
- `LevelOrnateAxe.cs` — **LevelOrnateAxe : BaseLevelAxe** — Magical item ("barbarian axe").
- `LevelRadiantScimitar.cs` — **LevelRadiantScimitar : BaseLevelSword** — Magical item ("falchion").
- `LevelRuneBlade.cs` — **LevelRuneBlade : BaseLevelSword** — Magical item ("war blades").
- `LevelWarCleaver.cs` — **LevelWarCleaver : BaseLevelKnife** — Magical item.
- `LevelWildStaff.cs` — **LevelWildStaff : BaseLevelStaff** — Magical item ("druid staff").

#### Scripts/Items/Magical/God/Weapons/Maces — Magical items: Maces. Types: BaseLevelBashing, BaseLevelWhip, LevelClub, LevelHammerPick, ....

- `BaseLevelBashing.cs` — **BaseLevelBashing : BaseBashing** — Mace/hammer weapon.
- `BaseLevelWhip.cs` — **BaseLevelWhip : BaseWhip** — Magical item ("whip").
- `LevelClub.cs` — **LevelClub : BaseLevelBashing** — Magical item ("club").
- `LevelHammerPick.cs` — **LevelHammerPick : BaseLevelBashing** — Magical item ("hammer pick").
- `LevelHammers.cs` — **LevelHammers : BaseLevelBashing** — Magical item ("hammer").
- `LevelMace.cs` — **LevelMace : BaseLevelBashing** — Magical item ("mace").
- `LevelMaul.cs` — **LevelMaul : BaseLevelBashing** — Magical item.
- `LevelScepter.cs` — **LevelScepter : BaseLevelBashing** — Magical item.
- `LevelSpikedClub.cs` — **LevelSpikedClub : BaseLevelBashing** — Magical item ("spiked club").
- `LevelWarHammer.cs` — **LevelWarHammer : BaseLevelBashing** — Magical item ("war hammer").
- `LevelWarMace.cs` — **LevelWarMace : BaseLevelBashing** — Magical item ("war mace").
- `LevelWhip.cs` — **LevelWhips : BaseLevelWhip** — Magical item.

#### Scripts/Items/Magical/God/Weapons/PoleArms — Magical items: Pole arms. Types: BaseLevelPoleArm, LevelBardiche, LevelHalberd, LevelScythe.

- `BaseLevelPoleArm.cs` — **BaseLevelPoleArm : BasePoleArm** — Polearm weapon.
- `LevelBardiche.cs` — **LevelBardiche : BaseLevelPoleArm** — Magical item.
- `LevelHalberd.cs` — **LevelHalberd : BaseLevelPoleArm** — Magical item ("halberd").
- `LevelScythe.cs` — **LevelScythe : BaseLevelPoleArm** — Magical item ("scythe").

#### Scripts/Items/Magical/God/Weapons/Ranged — Magical items: Ranged. Types: BaseLevelRanged, LevelBow, LevelCompositeBow, LevelCrossbow, ....

- `BaseLevelRanged.cs` — **BaseLevelRanged : BaseRanged** — Ranged weapon.
- `LevelBow.cs` — **LevelBow : BaseLevelRanged** — Magical item ("bow").
- `LevelCompositeBow.cs` — **LevelCompositeBow : BaseLevelRanged** — Magical item ("composite bow").
- `LevelCrossbow.cs` — **LevelCrossbow : BaseLevelRanged** — Magical item ("crossbow").
- `LevelHeavyCrossbow.cs` — **LevelHeavyCrossbow : BaseLevelRanged** — Magical item.
- `LevelRepeatingCrossbow.cs` — **LevelRepeatingCrossbow : BaseLevelRanged** — Magical item.

#### Scripts/Items/Magical/God/Weapons/SE Weapons — Magical items: Se weapons. Types: LevelBokuto, LevelDaisho, LevelKama, LevelLajatang, ....

- `LevelBokuto.cs` — **LevelBokuto : BaseLevelSword** — Magical item.
- `LevelDaisho.cs` — **LevelDaisho : BaseLevelSword** — Magical item.
- `LevelKama.cs` — **LevelKama : BaseLevelKnife** — Magical item.
- `LevelLajatang.cs` — **LevelLajatang : BaseLevelKnife** — Magical item.
- `LevelNoDachi.cs` — **LevelNoDachi : BaseLevelSword** — Magical item.
- `LevelNunchaku.cs` — **LevelNunchaku : BaseLevelBashing** — Magical item.
- `LevelSai.cs` — **LevelSai : BaseLevelKnife** — Magical item.
- `LevelTekagi.cs` — **LevelTekagi : BaseLevelKnife** — Magical item.
- `LevelTessen.cs` — **LevelTessen : BaseLevelBashing** — Magical item.
- `LevelTetsubo.cs` — **LevelTetsubo : BaseLevelBashing** — Magical item.
- `LevelWakizashi.cs` — **LevelWakizashi : BaseLevelSword** — Magical item.
- `LevelYumi.cs` — **LevelYumi : BaseLevelRanged** — Magical item ("yumi").

#### Scripts/Items/Magical/God/Weapons/SpearsAndForks — Magical items: Spears and forks. Types: BaseLevelSpear, LevelBladedStaff, LevelDoubleBladedStaff, LevelPike, ....

- `BaseLevelSpear.cs` — **BaseLevelSpear : BaseSpear** — Spear weapon.
- `LevelBladedStaff.cs` — **LevelBladedStaff : BaseLevelSpear** — Magical item.
- `LevelDoubleBladedStaff.cs` — **LevelDoubleBladedStaff : BaseLevelSpear** — Magical item ("double bladed staff").
- `LevelPike.cs` — **LevelPike : BaseLevelSpear** — Magical item.
- `LevelPitchfork.cs` — **LevelPitchfork : BaseLevelSpear** — Magical item ("trident").
- `LevelShortSpear.cs` — **LevelShortSpear : BaseLevelSpear** — Magical item ("rapier").
- `LevelSpear.cs` — **LevelSpear : BaseLevelSpear** — Magical item ("spear").
- `LevelTribalSpear.cs` — **LevelTribalSpear : BaseLevelSpear** — Magical item.
- `LevelWarFork.cs` — **LevelWarFork : BaseLevelSpear** — Magical item ("war fork").

#### Scripts/Items/Magical/God/Weapons/Staves — Magical items: Staves. Types: BaseLevelStaff, LevelBlackStaff, LevelGnarledStaff, LevelQuarterStaff, ....

- `BaseLevelStaff.cs` — **BaseLevelStaff : BaseStaff** — Staff weapon.
- `LevelBlackStaff.cs` — **LevelBlackStaff : BaseLevelStaff** — Magical item ("wizard staff").
- `LevelGnarledStaff.cs` — **LevelGnarledStaff : BaseLevelStaff** — Magical item.
- `LevelQuarterStaff.cs` — **LevelQuarterStaff : BaseLevelStaff** — Magical item.
- `LevelShepherdsCrook.cs` — **LevelShepherdsCrook : BaseLevelStaff** — Magical item.

#### Scripts/Items/Magical/God/Weapons/Swords — Magical items: Swords. Types: BaseLevelSword, LevelBoneHarvester, LevelBroadsword, LevelClaymore, ....

- `BaseLevelSword.cs` — **BaseLevelSword : BaseSword** — Sword weapon.
- `LevelBoneHarvester.cs` — **LevelBoneHarvester : BaseLevelSword** — Magical item ("sickle").
- `LevelBroadsword.cs` — **LevelBroadsword : BaseLevelSword** — Magical item ("broadsword").
- `LevelClaymore.cs` — **LevelClaymore : BaseLevelSword** — Magical item ("claymore").
- `LevelCrescentBlade.cs` — **LevelCrescentBlade : BaseLevelSword** — Magical item.
- `LevelCutlass.cs` — **LevelCutlass : BaseLevelSword** — Magical item ("cutlass").
- `LevelDoubleLaserSword.cs` — **LevelDoubleLaserSword : BaseLevelSword** — Magical item ("Double Laser Sword").
- `LevelKatana.cs` — **LevelKatana : BaseLevelSword** — Magical item ("katana").
- `LevelKryss.cs` — **LevelKryss : BaseLevelSword** — Magical item.
- `LevelLance.cs` — **LevelLance : BaseLevelSword** — Magical item.
- `LevelLaserSword.cs` — **LevelLaserSword : BaseLevelSword** — Magical item ("Laser Sword").
- `LevelLongsword.cs` — **LevelLongsword : BaseLevelSword** — Magical item ("longsword").
- `LevelRoyalSword.cs` — **LevelRoyalSword : LevelVikingSword** — Magical item ("royal sword").
- `LevelScimitar.cs` — **LevelScimitar : BaseLevelSword** — Magical item ("scimitar").
- `LevelShortSword.cs` — **LevelShortSword : BaseLevelSword** — Magical item ("short sword").
- `LevelThinLongsword.cs` — **LevelThinLongsword : BaseLevelSword** — Magical item ("sword").
- `LevelVikingSword.cs` — **LevelVikingSword : BaseLevelSword** — Magical item ("barbarian sword").

#### Scripts/Items/Misc — Miscellaneous items (dyes, tools, bodies, fragments, currency).

- `AcidSlime.cs` — **AcidSlime : Item** — Item. [Constructable]
- `AltarGods.cs` — **AltarGodsEast : Item** — ///////////////////////////////////////////////////////////////////////////////////////////////////////////... [Constructable] (+11 types)
- `ArcaneGem.cs` — **ArcaneGem : Item** — Item. [Constructable]
- `BankCheck.cs` — **BankCheck : Item** — Item. [Constructable]
- `Blocker.cs` — **Blocker : Item** — Item. [Constructable] (+1 type)
- `Blood.cs` — **Blood : Item** — Item. [Constructable] (+1 type)
- `Bola.cs` — **Bola : Item** — Item. [Constructable] (+1 type)
- `BolaBall.cs` — **BolaBall : Item** — Item. [Constructable]
- `BulletinBoards.cs` — **BulletinBoard : BaseBulletinBoard** — Item.
- `ClockworkAssembly.cs` — **ClockworkAssembly : Item** — Item. [Constructable]
- `CommunicationCrystals.cs` — **CrystalRechargeInfo** — Info/data container. [Constructable] (+4 types)
- `DruidMirror.cs` — **DruidMirror : Item** — Item ("druid mirror"). [Constructable, Flipable]
- `EffectController.cs` — **EffectController : Item** — Item. (+2 types)
- `EffectItem.cs` — **EffectItem : Item** — Item. (+1 type)
- `Firebomb.cs` — **Firebomb : Item** — Item ("a firebomb"). [Constructable] (+1 type)
- `FlipableAddonAttribute.cs` — **FlipableAddonAttribute : Attribute** — Item.
- `FlipableAttribute.cs` — **FlipableAttribute : Attribute** — Commands: [Flip (+3 types)
- `FragmentOfInfinity.cs` — **FragmentOfInfinity : Item** — Item ("Fragment of Infinity"). [Constructable] (+2 types)
- `GlassItems.cs` — **SmallFlask : Item** — Item. [Constructable, Flipable] (+42 types)
- `Gold.cs` — **Gold : Item** — Item ("gold coins"). [Constructable]
- `Guillotine.cs` — **Guillotine : Item** — Item. [Constructable]
- `HairDye.cs` — **HairDye : Item** — Item. [Constructable] (+2 types)
- `HomePlants.cs` — **HomePlants_Flower : Item** — ///////////////////////////////////////////////////////////////////////////////////////////////////////////... [Constructable] (+8 types)
- `IDurability.cs` — **IDurability** — Interface contract. (+1 type)
- `IvoryTusk.cs` — **IvoryTusk : Item** — Item ("ivory tusk"). [Constructable]
- `Key.cs` — **Key : Item** — Item. [Constructable] (+5 types)
- `KeyRing.cs` — **KeyRing : Item** — Item. [Constructable] (+1 type)
- `LOSBlocker.cs` — **LOSBlocker : Item** — Item. [Constructable] (+1 type)
- `LifeFountain.cs` — **LifeFountain : Item** — Item ("fountain of life"). [Constructable]
- `MagicForges.cs` — **MagicForges : Item** — Item ("Chest of Magical Serpent Relics").
- `MannequinDeed.cs` — **MannequinDeed : Item** — Item ("a mannequin deed"). [Constructable] (+1 type)
- `MegalodonTooth.cs` — **MegalodonTooth : Item** — Item ("megalodon tooth"). [Constructable]
- `MementoCoin.cs` — **MementoCoin : Item** — Item ("Memento Coins"). [Constructable]
- `MusicBox.cs` — **MusicBox : Item** — Item ("Lute of Many Songs"). [Constructable]
- `OilCloth.cs` — **OilCloth : Item** — Item ("oil cloth"). [Constructable]
- `Origami.cs` — **OrigamiPaper : Item** — Item. [Constructable] (+6 types)
- `PlayerBulletinBoards.cs` — **PlayerBBSouth : BasePlayerBB** — Item.
- `PlayerVendorDeed.cs` — **ContractOfEmployment : Item** — Item. [Constructable]
- `PoolOfAcid.cs` — **PoolOfAcid : Item** — Item. [Constructable]
- `PowerCrystal.cs` — **PowerCrystal : Item** — Item. [Constructable] (+1 type)
- `PowerGenerator.cs` — **PowerGenerator : BaseAddon** — House addon ("a power generator"). [Constructable] (+6 types)
- `PromotionalToken.cs` — **PromotionalToken : Item** — Item. [Constructable] (+2 types)
- `Puke.cs` — **Puke : Item** — Item ("A Pile of Puke"). [Constructable] (+1 type)
- `QuestSouvenir.cs` — **QuestSouvenir : Item** — Item ("souvenir"). [Constructable]
- `Rares.cs` — **Rope : Item** — Item ("decorative whip"). [Constructable] (+22 types)
- `ResurrectTile.cs` — **ResurrectTile : Item** — Item ("resurrection tile"). [Constructable]
- `Scales.cs` — **Scales : Item** — Item. [Constructable] (+1 type)
- `SerpentPillar.cs` — **SerpentPillar : Item** — Item. [Constructable]
- `SpecialBeardDye.cs` — **SpecialBeardDye : Item** — Item. [Constructable] (+2 types)
- `SpecialHairDye.cs` — **SpecialHairDye : Item** — Item. [Constructable] (+2 types)
- `SpecialJars.cs` — **SpecialJars : Item** — Item ("candle in a bottle"). [Constructable]
- `Static.cs` — **Static : Item** — Item. [Constructable] (+1 type)
- `SwarmOfFlies.cs` — **SwarmOfFlies : Item** — Item. [Constructable]
- `Telescope.cs` — **TelescopeAddon : BaseAddon** — House addon ("telescope"). [Constructable] (+1 type)
- `TrashBarrel.cs` — **TrashBarrel : Container** — Container. [Constructable] (+1 type)
- `TrashChest.cs` — **TrashChest : Container** — Container ("donation box"). [Constructable, Flipable] (+1 type)
- `WarningItem.cs` — **WarningItem : Item** — Item. [Constructable] (+1 type)
- `Waypoint.cs` — **WayPoint : Item** — Commands: [WayPointSeq [Constructable, Flipable] (+2 types)
- `WindChimes.cs` — **WindChimes : BaseWindChimes** — Item.
- `WornHumanDeco.cs` — **WornHumanDeco : JewelryRing** — Item ("demon wings").

#### Scripts/Items/Misc/Bodies — Items: Bodies. Types: BodyPart, BonePile, Head, LeftArm, ....

- `BodyParts.cs` — **BodyPart : Item** — Item. [Constructable]
- `BonePile.cs` — **BonePile : Item** — Item. [Constructable, Flipable]
- `Head.cs` — **Head : Item** — Item ("head"). [Constructable] (+1 type)
- `LeftArm.cs` — **LeftArm : Item** — Item ("arm"). [Constructable]
- `LeftLeg.cs` — **LeftLeg : Item** — Item ("leg"). [Constructable]
- `RibCage.cs` — **RibCage : Item** — Item. [Constructable, Flipable]
- `RightArm.cs` — **RightArm : Item** — Item ("arm"). [Constructable]
- `RightLeg.cs` — **RightLeg : Item** — Item ("leg"). [Constructable]
- `Torso.cs` — **Torso : Item** — Item ("torso"). [Constructable]

#### Scripts/Items/Misc/Bodies/Corpses — Items: Corpses. Types: Corpse, CorpseNameAttribute, DecayedCorpse, CorpseEquip.

- `Corpse.cs` — **Corpse : Container** — Container. (+5 types)
- `CorpseNameAttribute.cs` — **CorpseNameAttribute : Attribute** — Item.
- `DecayedCorpse.cs` — **DecayedCorpse : Container** — Container. (+1 type)
- `Packets.cs` — **CorpseEquip : Packet** — Network packet. (+1 type)

#### Scripts/Items/Misc/Bodies/LivingDead — Items: Living dead. Types: BookofDead, DarkHeart, SummonCorpse.

- `BookofDead.cs` — **BookofDead : Item** — Item. [Constructable]
- `DarkHeart.cs` — **DarkHeart : Item** — Item ("dark heart"). [Constructable]
- `SummonCorpse.cs` — **SummonCorpse : BaseCreature** — Creature ("a summoned corpse", AI_Melee, Closest). [Constructable, CorpseName]

#### Scripts/Items/Misc/Christmas — Items: Christmas. Types: ChristmasRobe, GiftBox, GingerBreadHouseDeed, HolidayBell, ....

- `Christmas Robe.cs` — **ChristmasRobe : BaseOuterTorso** — Item ("Christmas Robe").
- `GiftBox.cs` — **GiftBox : BaseContainer** — Container. [Constructable, Flipable]
- `GingerBreadHouseDeed.cs` — **GingerBreadHouseDeed : BaseAddonDeed** — Addon deed. [Constructable] (+1 type)
- `HolidayBell.cs` — **HolidayBell : Item** — Item. [Constructable]
- `HolidayBells.cs` — **HolidayBells : BaseHolidayBells** — Item ("holiday bells").
- `HolidayFoods.cs` — **CandyCane : Food** — Item.
- `HolidayGiftBoxes.cs` — **GiftBoxHues** — Item.
- `HolidayTree.cs` — **HolidayTreeAddon : BaseAddon** — House addon ("holiday tree"). [Constructable] (+1 type)
- `HolidayTreeFlat.cs` — **HolidayTreeFlat : BaseAddon** — House addon ("holiday tree"). [Constructable] (+1 type)
- `HolidayTreesNew.cs` — **HolidayTreeNew1Addon : BaseAddon** — House addon ("grand holiday tree"). (+13 types)
- `Icecicles.cs` — **IcicleLargeSouth : Item** — Item. [Constructable] (+5 types)
- `IcyPatch.cs` — **IcyPatch : Item** — Item. [Constructable]
- `PKHolidayStuff.cs` — **Coal : Item** — Item. [Constructable] (+2 types)
- `Poinsettias.cs` — **RedPoinsettia : Item** — Item. [Constructable] (+1 type)
- `RibbonTrees.cs` — **RibbonTree : Item** — Item ("ribbon tree"). [Constructable] (+1 type)
- `SnowPiles.cs` — **SnowPileDeco : Item** — Item. [Constructable]
- `Snowflakes.cs` — **BlueSnowflake : Item** — Item. [Constructable] (+1 type)
- `Snowman.cs` — **Snowman : Item** — Item. [Constructable, Flipable]
- `Stockings.cs` — **GreenStocking : BaseContainer** — Container. [Constructable, Flipable] (+1 type)
- `WinterGiftPackage2003.cs` — **WinterGiftPackage2003 : GiftBox** — Item.
- `Wreath.cs` — **WreathAddon : Item** — Item. [Constructable, Flipable] (+3 types)

#### Scripts/Items/Misc/Crystal — Items: Crystal. Types: ECrystalAltarComponent, ECrystalBeggarStatueComponent, ECrystalBrazierComponent, ECrystalBullComponent, ....

- `ECrystalAltar.cs` — **ECrystalAltarComponent : AddonComponent** — Item.
- `ECrystalBeggarStatue.cs` — **ECrystalBeggarStatueComponent : AddonComponent** — Item.
- `ECrystalBrazier.cs` — **ECrystalBrazierComponent : AddonComponent** — Item.
- `ECrystalBull.cs` — **ECrystalBullComponent : AddonComponent** — Item.
- `ECrystalRunnerStatue.cs` — **ECrystalRunnerStatueComponent : AddonComponent** — Item.
- `ECrystalSupplicantStatue.cs` — **ECrystalSupplicantStatueComponent : AddonComponent** — Item.
- `ECrystalTable.cs` — **ECrystalTableComponent : AddonComponent** — Item.
- `ECrystalThrone.cs` — **ECrystalThroneComponent : AddonComponent** — Item.

#### Scripts/Items/Misc/Dyes — Items: Dyes. Types: AllDyeTubsArmor, AllDyeTubsBookRune, AllDyeTubsBookSpell, AllDyeTubsFurniture, ....

- `AllDyeTubsArmor.cs` — **AllDyeTubsArmor : DyeTub** — Item ("Armor and Shield Dye Tub").
- `AllDyeTubsBookRune.cs` — **AllDyeTubsBookRune : DyeTub** — Item ("Rune Book Dye Tub").
- `AllDyeTubsBookSpell.cs` — **AllDyeTubsBookSpell : DyeTub** — Item ("Spell Book Dye Tub").
- `AllDyeTubsFurniture.cs` — **AllDyeTubsFurniture : DyeTub** — Item ("Furniture Dye Tub").
- `AllDyeTubsMountEthereal.cs` — **AllDyeTubsMountEthereal : DyeTub** — Item ("Ethereal Mount Dye Tub").
- `AllDyeTubsWeapon.cs` — **AllDyeTubsWeapon : DyeTub** — Item ("Weapon Dye Tub").
- `AllHues.cs` — **AllHuesInfo** — Info/data container.
- `AssassinDyeTub.cs` — **AssassinDyeTub : DyeTub** — Item.
- `BlackDyeTub.cs` — **BlackDyeTub : DyeTub** — Item.
- `BlazeDyeTub.cs` — **BlazeDyeTub : DyeTub** — Item.
- `CustomHuePicker.cs` — **CustomHuePicker** — Item.
- `DyeTub.cs` — **DyeTub : Item** — Item. [Constructable] (+2 types)
- `FurnitureDyeTub.cs` — **FurnitureDyeTub : DyeTub** — Item.
- `HueStone.cs` — **HueStone : Item** — Item ("Illusionist Stone"). [Constructable] (+1 type)
- `LeatherDyeTub.cs` — **LeatherDyeTub : DyeTub** — Item.
- `MagicPigment.cs` — **MagicPigment : Item** — Item ("mystical paints"). [Constructable] (+1 type)
- `MagicalDyes.cs` — **MagicalDyes : Item** — Item ("magical dye"). [Constructable] (+1 type)
- `RewardBlackDyeTub.cs` — **RewardBlackDyeTub : DyeTub** — Item.
- `RunebookDyeTub.cs` — **RunebookDyeTub : DyeTub** — Item.
- `SpecialDyeTub.cs` — **SpecialDyeTub : DyeTub** — Item.
- `StatuetteDyeTub.cs` — **StatuetteDyeTub : DyeTub** — Item.
- `UnusualDyes.cs` — **UnusualDyes : Item** — Item ("an odd jar of dye"). [Constructable] (+1 type)
- `WhiteDyeTub.cs` — **WhiteDyeTub : DyeTub** — Item ("white dye tub").

#### Scripts/Items/Misc/Dyes/Essence — Items: Essence. Types: EssenceBase, EssenceBaseEmpty, EssenceOrb.

- `EssenceBase.cs` — **EssenceBase : BaseAddon** — House addon.
- `EssenceBaseEmpty.cs` — **EssenceBaseEmpty : Item** — Item ("Pedestal"). [Constructable]
- `EssenceOrb.cs` — **EssenceOrb : Item** — Item ("Essence"). [Constructable]

#### Scripts/Items/Misc/Evil — Items: Evil. Types: EGlobeOfSosariaComponent, EObsidianPillarComponent, EObsidianRockComponent, EShadowAltarComponent, ....

- `EGlobeOfSosaria.cs` — **EGlobeOfSosariaComponent : AddonComponent** — Item.
- `EObsidianPillar.cs` — **EObsidianPillarComponent : AddonComponent** — Item ("box containing an obsidian pillar").
- `EObsidianRock.cs` — **EObsidianRockComponent : AddonComponent** — Item ("box containing an obsidian rock").
- `EShadowAltar.cs` — **EShadowAltarComponent : AddonComponent** — Item ("box containing a shadow altar").
- `EShadowBanner.cs` — **EShadowBannerComponent : AddonComponent** — Item ("box containing a shadow banner").
- `EShadowFirePit.cs` — **EShadowFirePitComponent : AddonComponent** — Item ("box containing a shadow fire pit").
- `EShadowFirePitCross.cs` — **EShadowFirePitCrossComponent : AddonComponent** — Item ("box containing a crossed shadow fire pit").
- `EShadowPillar.cs` — **EShadowPillarComponent : AddonComponent** — Item ("box containing a shadow pillar").
- `ESpikeColumn.cs` — **ESpikeColumnComponent : AddonComponent** — Item ("box containing a spike column").
- `ESpikePostEast.cs` — **ESpikePostEastComponent : AddonComponent** — Item ("box containing a spike post facing east").
- `ESpikePostSouth.cs` — **ESpikePostSouthComponent : AddonComponent** — Item ("box containing a spike post facing south").

#### Scripts/Items/Misc/Facial — Items: Facial. Types: Beard, Hair.

- `Beard.cs` — **Beard : Item** — Item. (+8 types)
- `Hair.cs` — **Hair : Item** — Item. (+11 types)

#### Scripts/Items/Misc/Halloween — Items: Halloween. Types: AppleBobbingBarrel, BloodPentagramDeed, BloodyTableAddon, CarvedPumpkin, ....

- `AppleBobbingBarrel.cs` — **AppleBobbingBarrel : Item** — Item ("apple bobbing barrel"). [Constructable] (+1 type)
- `BloodPentagramDeed.cs` — **BloodPentagramDeed : BaseAddonDeed** — Addon deed ("Blood Pentagram Deed"). [Constructable] (+1 type)
- `BloodyTableAddon.cs` — **BloodyTableAddon : BaseAddon** — House addon ("BloodyTable"). (+1 type)
- `CarvedPumpkins.cs` — **CarvedPumpkin : BaseLight** — Light source ("Jack-O-Lantern"). [Constructable] (+19 types)
- `DaemonDartBoard.cs` — **DaemonDartBoard : AddonComponent** — Item ("daemon dart board").
- `DeadBodyDeedEW.cs` — **DeadBodyEWAddon : BaseAddon** — House addon ("Dead Body"). [Constructable] (+1 type)
- `DeadBodyDeedNS.cs` — **DeadBodyNSAddon : BaseAddon** — House addon ("Dead Body"). [Constructable] (+1 type)
- `EerieGhost.cs` — **EerieGhost : BaseLight** — Light source ("eerie ghost"). [Constructable]
- `EvilFireplaceFace.cs` — **EvilFireplaceSouthFaceAddon : BaseAddon** — House addon ("Evil Fireplace South Deed"). [Constructable] (+3 types)
- `HalloweenAddons.cs` — **halloween_coffin_eastAddon : BaseAddon** — /////////////////////////////////////////////////////////////////////////////////////////////////////////// [Constructable] (+7 types)
- `HalloweenBag.cs` — **HalloweenBag : BoneContainer** — Item ("Dragon Costume").
- `HalloweenBlood.cs` — **HalloweenBlood : Item** — Item ("Blood"). [Constructable, Flipable]
- `HalloweenBonePile.cs` — **HalloweenBonePileAddon : BaseAddon** — House addon ("Bone Pile"). [Constructable] (+1 type)
- `HalloweenChopper.cs` — **HalloweenChopper : Item** — Item ("Guillotine"). [Constructable, Flipable]
- `HalloweenColumn.cs` — **HalloweenColumn : Item** — Item ("Column"). [Constructable]
- `HalloweenGift.cs` — **HalloweenGift : Item** — Item ("Stuffed Nightmare"). [Constructable]
- `HalloweenGrave1.cs` — **HalloweenGrave1 : Item** — Item ("Grave Stone"). [Constructable, Flipable] (+1 type)
- `HalloweenGrave2.cs` — **HalloweenGrave2 : Item** — Item ("Grave Stone"). [Constructable, Flipable] (+1 type)
- `HalloweenGrave3.cs` — **HalloweenGrave3 : Item** — Item ("Grave Stone"). [Constructable, Flipable] (+1 type)
- `HalloweenItems.cs` — **LargeDyingPlant : Item** — /////////////////////////////////////////////////////////////////////////// [Constructable, Flipable] (+13 types)
- `HalloweenMaiden.cs` — **HalloweenMaiden : Item** — Item ("Iron Maiden"). [Constructable]
- `HalloweenPack.cs` — **HalloweenPack** — Source file (no top-level type declaration detected).
- `HalloweenPylon.cs` — **HalloweenPylon : Item** — Item ("Pylon"). [Constructable]
- `HalloweenPylonFire.cs` — **HalloweenPylonFire : Item** — Item ("Fire"). [Constructable]
- `HalloweenShrineChaos.cs` — **HalloweenShrineChaosAddon : BaseAddon** — House addon ("Chaos Shrine"). [Constructable] (+1 type)
- `HalloweenSkullPole.cs` — **HalloweenSkullPole : Item** — Item ("Skull Pole"). [Constructable]
- `HalloweenStoneColumn.cs` — **HalloweenStoneColumn : Item** — Item ("Stone Column"). [Constructable]
- `HalloweenStoneSpike.cs` — **HalloweenStoneSpike : Item** — Item ("Stone Spike"). [Constructable]
- `HalloweenStoneSpike2.cs` — **HalloweenStoneSpike2 : Item** — Item ("Stone Spike"). [Constructable]
- `HalloweenTortSkel.cs` — **HalloweenTortSkel : Item** — Item ("Skeleton"). [Constructable, Flipable]
- `HalloweenTree1.cs` — **HalloweenTree1 : Item** — Item ("Tree"). [Constructable]
- `HalloweenTree2.cs` — **HalloweenTree2 : Item** — Item ("Tree"). [Constructable]
- `HalloweenTree3.cs` — **HalloweenTree3 : Item** — Item ("Tree"). [Constructable]
- `HalloweenTree4.cs` — **HalloweenTree4 : Item** — Item ("Tree"). [Constructable]
- `HalloweenTree5.cs` — **HalloweenTree5 : Item** — Item ("Tree"). [Constructable]
- `HalloweenTree6.cs` — **HalloweenTree6 : Item** — Item ("Tree"). [Constructable]
- `MongbatDartBoard.cs` — **MongbatDartBoard : AddonComponent** — Item ("Mongbat Dark Board (east)").
- `NecromancerBanner.cs` — **NecromancerBanner : Item** — Item ("deathly banner"). [Constructable, Flipable]
- `NecromancerTable.cs` — **NecromancerTable : Item** — Item ("necromancer table"). [Constructable, Flipable]
- `PackedCostume.cs` — **PackedCostume** — Source file (no top-level type declaration detected).
- `TrickOrTreat.cs` — **TrickOrTreat** — Item.
- `TrickOrTreatBag.cs` — **TrickOrTreatBag : BaseContainer** — Container ("trick or treat bag"). [Constructable]
- `WrappedCandy.cs` — **WrappedCandy** — Source file (no top-level type declaration detected).

#### Scripts/Items/Misc/Market — Items: Market. Types: AppleTrunkAddon, BlueDecorativeRugAddon, BlueFancyRugAddon, BluePlainRugAddon, ....

- `AppleTrunk.cs` — **AppleTrunkAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `BlueDecorativeRug.cs` — **BlueDecorativeRugAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `BlueFancyRug.cs` — **BlueFancyRugAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `BluePlainRug.cs` — **BluePlainRugAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `BoilingCauldron.cs` — **BoilingCauldronAddon : BaseAddonContainer** — Item.
- `CherryBlossomTree.cs` — **CherryBlossomTreeAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `CherryBlossomTrunk.cs` — **CherryBlossomTrunkAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `CinnamonFancyRug.cs` — **CinnamonFancyRugAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `Curtains.cs` — **CurtainsComponent : AddonComponent** — Item.
- `Fountain.cs` — **FountainAddon : StoneFountainAddon** — Item.
- `FruitTrees.cs` — **BaseFruitTreeAddon : BaseAddon** — House addon. [Constructable] (+4 types)
- `GoldenDecorativeRug.cs` — **GoldenDecorativeRugAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `Guillotine.cs` — **GuillotineComponent : AddonComponent** — Item.
- `HangingAxes.cs` — **HangingAxesAddon : BaseAddon** — House addon. [Constructable] (+2 types)
- `HangingSwords.cs` — **HangingSwordsAddon : BaseAddon** — House addon. [Constructable] (+2 types)
- `HouseLadder.cs` — **HouseLadderAddon : BaseAddon** — House addon. [Constructable] (+2 types)
- `IronMaiden.cs` — **IronMaidenAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `LargeFishingNet.cs` — **LargeFishingNetComponent : AddonComponent** — Item.
- `PeachTrunk.cs` — **PeachTrunkAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `PinkFancyRug.cs` — **PinkFancyRugAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `RedPlainRug.cs` — **RedPlainRugAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `Scarecrow.cs` — **ScarecrowComponent : AddonComponent** — Item.
- `SmallFishingNet.cs` — **SmallFishingNetComponent : AddonComponent** — Item.
- `Statue.cs` — **StoneStatueAddon : BaseAddon** — House addon ("tall statue of a warrior"). [Constructable] (+2 types)
- `SuitOfGoldArmor.cs` — **SuitOfGoldArmorComponent : AddonComponent** — Item.
- `SuitOfSilverArmor.cs` — **SuitOfSilverArmorComponent : AddonComponent** — Item.
- `TableWithBlueCloth.cs` — **TableWithBlueClothAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `TableWithOrangeCloth.cs` — **TableWithOrangeClothAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `TableWithPurpleCloth.cs` — **TableWithPurpleClothAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `TableWithRedCloth.cs` — **TableWithRedClothAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `UnmadeBed.cs` — **UnmadeBedAddon : BaseAddon** — House addon. [Constructable] (+2 types)
- `Vanity.cs` — **VanityAddon : BaseAddonContainer** — Item.
- `WallTorch.cs` — **WallTorchComponent : AddonComponent** — Item.
- `WoodenCoffin.cs` — **WoodenCoffinComponent : AddonComponent** — Item.

#### Scripts/Items/Misc/Mounts — Items: Mounts. Types: DaemonMount, DeathKnightWarhorse, NecroHorse, PaladinWarhorse, ....

- `DaemonMount.cs` — **DaemonMount : EtherealMount** — Item ("Daemon Servant").
- `DeathKnightWarhorse.cs` — **DeathKnightWarhorse : EtherealMount** — Item ("Dread Horse").
- `NecroHorse.cs` — **NecroHorse : EtherealMount** — Item ("Undead Horse").
- `PaladinWarhorse.cs` — **PaladinWarhorse : EtherealMount** — Item ("Silver Griffon").
- `Warhorse.cs` — **Warhorse : EtherealMount** — Item ("warhorse").

#### Scripts/Items/Misc/Rugs — Items: Rugs. Types: RoseEastLargeAddon, RoseEastSmallAddon, RoseSouthLargeAddon, RoseSouthSmallAddon.

- `RoseEastLargeAddon.cs` — **RoseEastLargeAddon : BaseAddon** — House addon ("large rose rug (east)"). [Constructable] (+1 type)
- `RoseEastSmallAddon.cs` — **RoseEastSmallAddon : BaseAddon** — House addon ("small rose rug (east)"). [Constructable] (+1 type)
- `RoseSouthLargeAddon.cs` — **RoseSouthLargeAddon : BaseAddon** — House addon ("large rose rug (south)"). [Constructable] (+1 type)
- `RoseSouthSmallAddon.cs` — **RoseSouthSmallAddon : BaseAddon** — House addon ("small rose rug (south)"). [Constructable] (+1 type)

#### Scripts/Items/Misc/Scrolls — Items: Scrolls. Types: ScrollofAlacrity, ScrollofTranscendence, SpecialScroll.

- `ScrollofAlacrity.cs` — **ScrollofAlacrity : SpecialScroll** — Item ("Scroll of Alacrity").
- `ScrollofTranscendence.cs` — **ScrollofTranscendence : SpecialScroll** — Item ("Scroll of Transcendence").
- `SpecialScroll.cs` — **SpecialScroll : Item** — Item. (+1 type)

#### Scripts/Items/Misc/Translocation — Items: Translocation. Types: BagOfSending, BallOfSummoning, BraceletOfBinding, MessageHelper, ....

- `BagOfSending.cs` — **BagOfSending : Item** — Item. [Constructable] (+3 types)
- `BallOfSummoning.cs` — **BallOfSummoning : Item** — Item ("Orb of Pet Summoning"). [Constructable] (+3 types)
- `BraceletOfBinding.cs` — **BraceletOfBinding : JewelryBracelet** — Item ("Bracelet of Protection").
- `MessageHelper.cs` — **MessageHelper** — Helper utilities.
- `PowderOfTranslocation.cs` — **PowderOfTranslocation : Item** — Item. [Constructable] (+2 types)

#### Scripts/Items/Potions — Items: Potions. Types: BasePotion.

- `BasePotion.cs` — **BasePotion : Item** — Item. (+1 type)

#### Scripts/Items/Potions/Elixirs — Items: Elixirs. Types: BaseElixir, ElixirAlchemy.

- `BaseElixir.cs` — **BaseElixir : BasePotion** — Potion. [Constructable]
- `Elixirs.cs` — **ElixirAlchemy : BaseElixir** — Potion ("elixir of alchemy").

#### Scripts/Items/Potions/Mixtures — Items: Mixtures. Types: AlchemicSlime, BaseLiquid, BaseMixture, LiquidFire, ....

- `AlchemicSlime.cs` — **AlchemicSlime : BaseCreature** — Creature ("a slime", AI_Melee, Closest). [Constructable, CorpseName]
- `BaseLiquid.cs` — **BaseLiquid : BasePotion** — Potion. (+1 type)
- `BaseMixture.cs` — **BaseMixture : BasePotion** — Potion. [Constructable] (+1 type)
- `LiquidFire.cs` — **LiquidFire : BaseLiquid** — Potion ("liquid fire").
- `LiquidGoo.cs` — **LiquidGoo : BaseLiquid** — Potion ("liquid goo").
- `LiquidIce.cs` — **LiquidIce : BaseLiquid** — Potion ("liquid ice").
- `LiquidPain.cs` — **LiquidPain : BaseLiquid** — Potion ("liquid pain").
- `LiquidRot.cs` — **LiquidRot : BaseLiquid** — Potion ("liquid rot").
- `MixtureDiseasedSlime.cs` — **MixtureDiseasedSlime : BaseMixture** — Potion ("slimy diseased mixture").
- `MixtureFireSlime.cs` — **MixtureFireSlime : BaseMixture** — Potion ("slimy fire mixture").
- `MixtureIceSlime.cs` — **MixtureIceSlime : BaseMixture** — Potion ("slimy ice mixture").
- `MixtureRadiatedSlime.cs` — **MixtureRadiatedSlime : BaseMixture** — Potion ("slimy irradiated mixture").
- `MixtureSlime.cs` — **MixtureSlime : BaseMixture** — Potion ("slimy mixture").

#### Scripts/Items/Potions/Special — Items: Special. Types: AutoResPotion, BottleOfAcid, BrewCauldron, CanopicJar, ....

- `AutoResPotion.cs` — **AutoResPotion : Item** — Item ("Potion Of Rebirth"). [Constructable]
- `BottleOfAcid.cs` — **BottleOfAcid : Item** — Item ("bottle of acid"). [Constructable]
- `BrewCauldron.cs` — **BrewCauldron : Item** — Item ("Cauldron"). [Constructable]
- `CanopicJar.cs` — **CanopicJar : Item** — Item ("canopic jar"). [Constructable]
- `DurabilityPotion.cs` — **DurabilityPotion : BasePotion** — Potion ("durability potion"). [Constructable] (+1 type)
- `EmptyCanopicJar.cs` — **EmptyCanopicJar : Item** — Item ("canopic jar"). [Constructable]
- `EvilSkull.cs` — **EvilSkull : Item** — Item ("evil skull"). [Constructable]
- `GenderPotion.cs` — **GenderPotion : Item** — Item ("potion of gender change"). [Constructable]
- `HairDyeBottle.cs` — **HairDyeBottle : Item** — Item ("hair dye mixture"). [Constructable]
- `HairDyePotion.cs` — **HairDyePotion : BasePotion** — Potion ("hair dye potion"). [Constructable]
- `HairOilPotion.cs` — **HairOilPotion : BasePotion** — Potion ("hair styling potion"). [Constructable] (+1 type)
- `HolyWater.cs` — **HolyWater : Item** — Item ("holy water"). [Constructable]
- `InvulnerabilityPotion.cs` — **InvulnerabilityPotion : BasePotion** — Potion ("invulnerability potion"). [Constructable] (+1 type)
- `MonsterSplatter.cs` — **MonsterSplatter : Item** — Item ("splatter"). [Constructable] (+1 type)
- `NecroSkinPotion.cs` — **NecroSkinPotion : Item** — Item ("jar of skull dust"). [Constructable]
- `PotionCauldron.cs` — **PotionCauldron : BaseAddon** — House addon. (+1 type)
- `PotionOfDexterity.cs` — **PotionOfDexterity : Item** — Item ("potion of dexterity"). [Constructable]
- `PotionOfMight.cs` — **PotionOfMight : Item** — Item ("potion of might"). [Constructable]
- `PotionOfWisdom.cs` — **PotionOfWisdom : Item** — Item ("potion of wisdom"). [Constructable]
- `RepairPotion.cs` — **RepairPotion : BasePotion** — Potion ("repairing potion"). [Constructable] (+1 type)
- `ResurrectPotion.cs` — **ResurrectPotion : BasePotion** — Potion ("resurrection potion"). [Constructable] (+1 type)
- `SuperPotion.cs` — **SuperPotion : BasePotion** — Potion ("superior potion"). [Constructable]
- `TransmutationPotion.cs` — **TransmutationPotion : BasePotion** — Potion ("transmutation potion"). [Constructable] (+1 type)

#### Scripts/Items/Potions/Standard — Items: Standard. Types: NightSightPotion.

- `NightSight.cs` — **NightSightPotion : BasePotion** — Potion ("nightsight potion"). [Constructable]

#### Scripts/Items/Potions/Standard/Agility Potions — Items: Agility potions. Types: AgilityPotion, BaseAgilityPotion, GreaterAgilityPotion.

- `AgilityPotion.cs` — **AgilityPotion : BaseAgilityPotion** — Potion ("agility potion").
- `BaseAgilityPotion.cs` — **BaseAgilityPotion : BasePotion** — Potion.
- `GreaterAgilityPotion.cs` — **GreaterAgilityPotion : BaseAgilityPotion** — Potion ("greater agility potion").

#### Scripts/Items/Potions/Standard/Conflagration Potions — Items: Conflagration potions. Types: BaseConflagrationPotion, ConflagrationPotion, GreaterConflagrationPotion.

- `BaseConflagrationPotion.cs` — **BaseConflagrationPotion : BasePotion** — Potion. (+3 types)
- `ConflagrationPotion.cs` — **ConflagrationPotion : BaseConflagrationPotion** — Potion ("conflagration potion").
- `GreaterConflagrationPotion.cs` — **GreaterConflagrationPotion : BaseConflagrationPotion** — Potion ("greater conflagration potion").

#### Scripts/Items/Potions/Standard/Confusion Blast Potions — Items: Confusion blast potions. Types: BaseConfusionBlastPotion, ConfusionBlastPotion, GreaterConfusionBlastPotion.

- `BaseConfusionBlastPotion.cs` — **BaseConfusionBlastPotion : BasePotion** — Potion. (+1 type)
- `ConfusionBlastPotion.cs` — **ConfusionBlastPotion : BaseConfusionBlastPotion** — Potion ("confusion blast potion").
- `GreaterConfusionBlastPotion.cs` — **GreaterConfusionBlastPotion : BaseConfusionBlastPotion** — Potion ("greater confusion blast potion").

#### Scripts/Items/Potions/Standard/Cure Potions — Items: Cure potions. Types: BaseCurePotion, CurePotion, GreaterCurePotion, LesserCurePotion.

- `BaseCurePotion.cs` — **BaseCurePotion : BasePotion** — These potions can perhaps cure up to (+1 type)
- `CurePotion.cs` — **CurePotion : BaseCurePotion** — Potion ("cure potion").
- `GreaterCurePotion.cs` — **GreaterCurePotion : BaseCurePotion** — Potion ("greater cure potion").
- `LesserCurePotion.cs` — **LesserCurePotion : BaseCurePotion** — Potion ("lesser cure potion").

#### Scripts/Items/Potions/Standard/Explosion Potions — Items: Explosion potions. Types: BaseExplosionPotion, ExplosionPotion, GreaterExplosionPotion, LesserExplosionPotion.

- `BaseExplosionPotion.cs` — **BaseExplosionPotion : BasePotion** — Potion. (+1 type)
- `ExplosionPotion.cs` — **ExplosionPotion : BaseExplosionPotion** — Potion ("explosion potion").
- `GreaterExplosionPotion.cs` — **GreaterExplosionPotion : BaseExplosionPotion** — Potion ("greater explosion potion").
- `LesserExplosionPotion.cs` — **LesserExplosionPotion : BaseExplosionPotion** — Potion ("lesser explosion potion").

#### Scripts/Items/Potions/Standard/Frostbite Potions — Items: Frostbite potions. Types: BaseFrostbitePotion, FrostbitePotion, GreaterFrostbitePotion.

- `BaseFrostbitePotion.cs` — **BaseFrostbitePotion : BasePotion** — Potion. (+3 types)
- `FrostbitePotion.cs` — **FrostbitePotion : BaseFrostbitePotion** — Potion ("frostbite potion").
- `GreaterFrostbitePotion.cs` — **GreaterFrostbitePotion : BaseFrostbitePotion** — Potion ("greater frostbite potion").

#### Scripts/Items/Potions/Standard/Heal Potions — Items: Heal potions. Types: BaseHealPotion, GreaterHealPotion, HealPotion, LesserHealPotion.

- `BaseHealPotion.cs` — **BaseHealPotion : BasePotion** — Potion.
- `GreaterHealPotion.cs` — **GreaterHealPotion : BaseHealPotion** — Potion ("greater heal potion").
- `HealPotion.cs` — **HealPotion : BaseHealPotion** — Potion ("heal potion").
- `LesserHealPotion.cs` — **LesserHealPotion : BaseHealPotion** — Potion ("lesser heal potion").

#### Scripts/Items/Potions/Standard/Poison Potions — Items: Poison potions. Types: BasePoisonPotion, DeadlyPoisonPotion, GreaterPoisonPotion, LesserPoisonPotion, ....

- `BasePoisonPotion.cs` — **BasePoisonPotion : BasePotion** — Potion. (+1 type)
- `DeadlyPoisonPotion.cs` — **DeadlyPoisonPotion : BasePoisonPotion** — Potion ("deadly poison potion").
- `GreaterPoisonPotion.cs` — **GreaterPoisonPotion : BasePoisonPotion** — Potion ("greater poison potion").
- `LesserPoisonPotion.cs` — **LesserPoisonPotion : BasePoisonPotion** — Potion ("lesser poison potion").
- `LethalPoisonPotion.cs` — **LethalPoisonPotion : BasePoisonPotion** — Potion ("lethal poison potion").
- `PoisonPotion.cs` — **PoisonPotion : BasePoisonPotion** — Potion ("poison potion").
- `VenomSack.cs` — **VenomSack** — Source file (no top-level type declaration detected).

#### Scripts/Items/Potions/Standard/Refresh Potions — Items: Refresh potions. Types: BaseRefreshPotion, RefreshPotion, TotalRefreshPotion.

- `BaseRefreshPotion.cs` — **BaseRefreshPotion : BasePotion** — Potion.
- `RefreshPotion.cs` — **RefreshPotion : BaseRefreshPotion** — Potion ("refresh potion").
- `TotalRefreshPotion.cs` — **TotalRefreshPotion : BaseRefreshPotion** — Potion ("total refresh potion").

#### Scripts/Items/Potions/Standard/Strength Potions — Items: Strength potions. Types: BaseStrengthPotion, GreaterStrengthPotion, StrengthPotion.

- `BaseStrengthPotion.cs` — **BaseStrengthPotion : BasePotion** — Potion.
- `GreaterStrengthPotion.cs` — **GreaterStrengthPotion : BaseStrengthPotion** — Potion ("greater strength potion").
- `StrengthPotion.cs` — **StrengthPotion : BaseStrengthPotion** — Potion ("strength potion").

#### Scripts/Items/Potions/Unique — Items: Unique. Types: BaseManaRefreshPotion, BaseRejuvenatePotion, GreaterInvisibilityPotion, GreaterManaPotion, ....

- `BaseManaRefreshPotion.cs` — **BaseManaRefreshPotion : BasePotion** — Potion.
- `BaseRejuvenatePotion.cs` — **BaseRejuvenatePotion : BasePotion** — Potion.
- `GreaterInvisibilityPotion.cs` — **GreaterInvisibilityPotion : BasePotion** — Potion ("greater invisibility potion"). [Constructable] (+1 type)
- `GreaterManaPotion.cs` — **GreaterManaPotion : BaseManaRefreshPotion** — Potion ("greater mana potion").
- `GreaterRejuvenatePotion.cs` — **GreaterRejuvenatePotion : BaseRejuvenatePotion** — Potion ("greater rejuvenate potion").
- `InvisibilityPotion.cs` — **InvisibilityPotion : BasePotion** — Potion ("invisibility potion"). [Constructable] (+1 type)
- `LesserInvisibilityPotion.cs` — **LesserInvisibilityPotion : BasePotion** — Potion ("lesser invisibility potion"). [Constructable] (+1 type)
- `LesserManaPotion.cs` — **LesserManaPotion : BaseManaRefreshPotion** — Potion ("lesser mana potion").
- `LesserRejuvenatePotion.cs` — **LesserRejuvenatePotion : BaseRejuvenatePotion** — Potion ("lesser rejuvenate potion").
- `ManaPotion.cs` — **ManaPotion : BaseManaRefreshPotion** — Potion ("mana potion").
- `RejuvenatePotion.cs` — **RejuvenatePotion : BaseRejuvenatePotion** — Potion ("rejuvenate potion").

#### Scripts/Items/Quivers — Items: Quivers. Types: ArcherQuiver, BaseQuiver, ElvenQuiver.

- `ArcherQuiver.cs` — **ArcherQuiver : BaseQuiver** — Archer quiver loot item with ColorText1/2 headers.
- `BaseQuiver.cs` — **BaseQuiver : Container** — Base quiver: weight reduction, damage bonus, ColorText1/2 header.
- `ElvenQuiver.cs` — **ElvenQuiver : BaseQuiver** — Elven quiver loot item with ColorText1/2 headers.

#### Scripts/Items/Relics — Relic loot family, identification flow and legacy worth text.

- `DDRelicAlchemy.cs` — **DDRelicAlchemy : Item** — Item ("alchemy flask"). [Constructable]
- `DDRelicArmor.cs` — **DDRelicArmor : Item** — Item. [Constructable]
- `DDRelicArts.cs` — **DDRelicArts : Item** — Item. [Constructable]
- `DDRelicBanner.cs` — **DDRelicBanner : Item** — Item ("a painting of "). [Constructable]
- `DDRelicBearRugs.cs` — **DDRelicBearRugsAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `DDRelicBook.cs` — **DDRelicBook : Item** — Item. [Constructable]
- `DDRelicCloth.cs` — **DDRelicCloth : Item** — Item. [Constructable]
- `DDRelicCoins.cs` — **DDRelicCoins : Item** — Item ("coins "). [Constructable]
- `DDRelicDrink.cs` — **DDRelicDrink : Item** — Item. [Constructable]
- `DDRelicFur.cs` — **DDRelicFur : Item** — Item. [Constructable]
- `DDRelicGem.cs` — **DDRelicGem : Item** — Item. [Constructable]
- `DDRelicGrave.cs` — **DDRelicGrave : Item** — Item ("gravestone"). [Constructable]
- `DDRelicInstrument.cs` — **DDRelicInstrument : Item** — Item. [Constructable]
- `DDRelicJewels.cs` — **DDRelicJewels : Item** — Item. [Constructable]
- `DDRelicLeather.cs` — **DDRelicLeather : Item** — Item. [Constructable]
- `DDRelicLight.cs` — **DDRelicLight1 : BaseLight** — ////////////////////////////////////////////////////////////////////////////////////////////////////// [Constructable] (+2 types)
- `DDRelicMoney.cs` — **DDCopper : Item** — Item ("copper coins"). [Constructable] (+5 types)
- `DDRelicOrbs.cs` — **DDRelicOrbs : Item** — Item. [Constructable]
- `DDRelicPainting.cs` — **DDRelicPainting : Item** — Item. [Constructable]
- `DDRelicReagent.cs` — **DDRelicReagent : Item** — Item ("bottle of "). [Constructable]
- `DDRelicRug.cs` — **DDRelicRugAddon : BaseAddon** — House addon. [Constructable] (+1 type)
- `DDRelicScrolls.cs` — **DDRelicScrolls : Item** — Item. [Constructable]
- `DDRelicStatue.cs` — **DDRelicStatue : Item** — Item. [Constructable]
- `DDRelicTablet.cs` — **DDRelicTablet : Item** — Item. [Constructable] (+1 type)
- `DDRelicVase.cs` — **DDRelicVase : Item** — Item. [Constructable]
- `DDRelicWeapon.cs` — **DDRelicWeapon : Item** — Item. [Constructable]
- `NotIdentified.cs` — **NotIdentified : LockableContainer** — Unidentified loot wrapper: packaging, ID flow, base-worth display support.
- `RelicFunctions.cs` — **RelicFunctions** — Relic identification mechanics and vendor/player ID rules.

#### Scripts/Items/Sharpening — Items: Sharpening. Types: AddDamageItemBase, ConsecrateItemBase, SplitElementalItemBase.

- `AddDamageItemBase.cs` — **AddDamageItemBase : Item** — Item. (+1 type)
- `ConsecrateItemBase.cs` — **ConsecrateItemBase : Item** — Item. (+1 type)
- `SplitElementalItemBase.cs` — **SplitElementalItemBase : Item** — Item. (+1 type)

#### Scripts/Items/Sharpening/BowStrings — Items: Bow strings. Types: ConsecratedBowString, DamageIncreaseBowStringBase, DenseBowString, ElementalBowString, ....

- `ConsecratedBowString.cs` — **ConsecratedBowString : ConsecrateItemBase** — Item ("Consecrated Bow String").
- `DamageIncreaseBowStringBase.cs` — **DamageIncreaseBowStringBase : AddDamageItemBase** — Item.
- `DenseBowString.cs` — **DenseBowString : DamageIncreaseBowStringBase** — Item ("Mighty Bow String").
- `ElementalBowString.cs` — **ElementalBowString : SplitElementalItemBase** — Item ("Elemental Bow String").
- `HeavyBowString.cs` — **HeavyBowString : DamageIncreaseBowStringBase** — Item ("Sturdy Bow String").
- `RoughBowString.cs` — **RoughBowString : DamageIncreaseBowStringBase** — Item ("Weak Bow String").

#### Scripts/Items/Sharpening/SharpeningStones — Items: Sharpening stones. Types: ConsecratedSharpeningStone, DamageIncreaseSharpeningStoneBase, DenseSharpeningStone, ElementalSharpeningStone, ....

- `ConsecratedSharpeningStone.cs` — **ConsecratedSharpeningStone : ConsecrateItemBase** — Item ("Consecrated Sharpening Stone").
- `DamageIncreaseSharpeningStoneBase.cs` — **DamageIncreaseSharpeningStoneBase : AddDamageItemBase** — Item.
- `DenseSharpeningStone.cs` — **DenseSharpeningStone : DamageIncreaseSharpeningStoneBase** — Item ("Dense Sharpening Stone").
- `ElementalSharpeningStone.cs` — **ElementalSharpeningStone : SplitElementalItemBase** — Item ("Elemental Sharpening Stone").
- `HeavySharpeningStone.cs` — **HeavySharpeningStone : DamageIncreaseSharpeningStoneBase** — Item ("Heavy Sharpening Stone").
- `RoughSharpeningStone.cs` — **RoughSharpeningStone : DamageIncreaseSharpeningStoneBase** — Item ("Rough Sharpening Stone").
- `SharpeningStoneBase.cs` — **SharpeningStoneBase : Item** — Item. (+1 type)

#### Scripts/Items/Sharpening/WeightingStones — Items: Weighting stones. Types: ConsecratedWeightingStone, DamageIncreaseWeightingStoneBase, DenseWeightingStone, ElementalWeightingStone, ....

- `ConsecratedWeightingStone.cs` — **ConsecratedWeightingStone : ConsecrateItemBase** — Item ("Consecrated Weighting Stone").
- `DamageIncreaseWeightingStoneBase.cs` — **DamageIncreaseWeightingStoneBase : AddDamageItemBase** — Item.
- `DenseWeightingStone.cs` — **DenseWeightingStone : DamageIncreaseWeightingStoneBase** — Item ("Dense Weighting Stone").
- `ElementalWeightingStone.cs` — **ElementalWeightingStone : SplitElementalItemBase** — Item ("Elemental Weighting Stone").
- `HeavyWeightingStone.cs` — **HeavyWeightingStone : DamageIncreaseWeightingStoneBase** — Item ("Heavy Weighting Stone").
- `RoughWeightingStone.cs` — **RoughWeightingStone : DamageIncreaseWeightingStoneBase** — Item ("Rough Weighting Stone").
- `WeightingStoneBase.cs` — **WeightingStoneBase : Item** — Item. (+1 type)

#### Scripts/Items/Special — Special/quest/event items and one-off systems.

- `AlternateRealityMap.cs` — **AlternateRealityMap : Item** — Item ("Map of an Alternate Reality"). [Constructable, Flipable] (+1 type)
- `BarbaricSatchel.cs` — **BarbaricSatchel : Item** — Item ("barbaric satchel"). [Constructable] (+2 types)
- `BaseImprisonedMobile.cs` — **BaseImprisonedMobile : Item** — Item. [Constructable]
- `CharacterStatue.cs` — **CharacterStatue : Mobile** — Mobile. (+15 types)
- `DemonPrison.cs` — **DemonPrison : Item** — Item ("Daemon Shard"). [Constructable] (+3 types)
- `DracolichSkull.cs` — **DracolichSkull : Item** — Item ("Mysterious Dragon Skull"). [Constructable] (+1 type)
- `DragonEgg.cs` — **DragonEgg : Item** — Item ("Dragon Egg"). [Constructable] (+1 type)
- `DragonOrbStatue.cs` — **DragonOrbStatue : Item** — Item ("Statue of "). [Constructable, Flipable]
- `DragonPedStatue.cs` — **DragonPedStatue : Item** — Item ("dragon statue"). [Constructable, Flipable]
- `DrakkhenEgg.cs` — **DrakkhenEggRed : Item** — Item ("Drakkhen Crystal"). [Constructable] (+3 types)
- `FountainOfLife.cs` — **FountainOfLife : BaseAddonContainer** — Item.
- `GrizzledMareStatuette.cs` — **GrizzledMareStatuette : BaseImprisonedMobile** — Item.
- `HearthOfHomeFire.cs` — **HearthOfHomeFire : BaseAddon** — House addon. [Constructable] (+2 types)
- `MiniHouses.cs` — **MiniHouseAddon : BaseAddon** — House addon. [Constructable] (+3 types)
- `MinotaurHedge.cs` — **MinotaurHedge : Item** — Item ("minotaur hedge"). [Constructable]
- `MonsterStatuette.cs` — **MonsterStatuette : Item** — Item. (+3 types)
- `OrbOfTheAbyss.cs` — **OrbOfTheAbyss : TrinketTalisman** — Item ("orb of the abyss").
- `RedVelvetGiftBox.cs` — **RedVelvetGiftBox : BaseContainer** — Container. [Constructable]
- `RewardCake.cs` — **RewardCake : Item** — Item. [Constructable]
- `RoseInAVase.cs` — **RoseInAVase : Item /* TODO: when dye tub changes are implemented** — Item.
- `RoseOfTrinsic.cs` — **RoseOfMoon : Item** — Item. [Constructable, Flipable] (+2 types)
- `SlaversNet.cs` — **SlaversNet : Item** — Item ("throwing net"). [Constructable] (+1 type)
- `SoulStone.cs` — **SoulStone : Item** — Item. [Constructable] (+8 types)
- `Talismans.cs` — **TalismanForm** — Enum with about 4 members. [Constructable] (+5 types)
- `TapestryOfSosaria.cs` — **TapestryOfSosaria : Item** — Item. [Constructable, Flipable] (+1 type)
- `TormentedChains.cs` — **TormentedChains : Item** — Item ("chains of the tormented"). [Constructable]
- `ValentinesCard.cs` — **ValentinesCard : Item** — Item. [Constructable] (+2 types)
- `WindSpirit.cs` — **WindSpirit : Item** — Item ("wind spirit"). [Constructable]
- `WizardsStatue.cs` — **WizardsStatue : Item** — Item ("Statue of "). [Constructable, Flipable]

#### Scripts/Items/Special/Broken Furniture Collection — Items: Broken furniture collection. Types: BrokenArmoireComponent, BrokenBedAddon, BrokenBookcaseComponent, BrokenChestOfDrawersComponent, ....

- `BrokenArmoire.cs` — **BrokenArmoireComponent : AddonComponent** — Item.
- `BrokenBed.cs` — **BrokenBedAddon : BaseAddon** — House addon. [Constructable] (+2 types)
- `BrokenBookcase.cs` — **BrokenBookcaseComponent : AddonComponent** — Item.
- `BrokenChestOfDrawers.cs` — **BrokenChestOfDrawersComponent : AddonComponent** — Item.
- `BrokenCoveredChair.cs` — **BrokenCoveredChairComponent : AddonComponent** — Item.
- `BrokenFallenChair.cs` — **BrokenFallenChairComponent : AddonComponent** — Item.
- `BrokenVanity.cs` — **BrokenVanityAddon : BaseAddon** — House addon. [Constructable] (+2 types)
- `StandingBrokenChair.cs` — **StandingBrokenChairComponent : AddonComponent** — Item.

#### Scripts/Items/Special/Evil Home Decor Collection — Items: Evil home decor collection. Types: AwesomeDisturbingPortraitComponent, BedOfNailsComponent, BoneCouchComponent, BoneTableAddon, ....

- `AwesomeDisturbingPortrait.cs` — **AwesomeDisturbingPortraitComponent : AddonComponent** — Item ("box containing a disturbing portrait").
- `BedOfNails.cs` — **BedOfNailsComponent : AddonComponent** — Item ("box containing a bed of nails").
- `BoneCouch.cs` — **BoneCouchComponent : AddonComponent** — Item ("box containing a couch of bones").
- `BoneTable.cs` — **BoneTableAddon : BaseAddon** — House addon ("box containing a table of bones"). [Constructable] (+1 type)
- `BoneThrone.cs` — **BoneThroneComponent : AddonComponent** — Item ("box containing a throne of bones").
- `CreepyPortrait.cs` — **CreepyPortraitComponent : AddonComponent** — Item ("box containing a creepy portrait").
- `DisturbingPortrait.cs` — **DisturbingPortraitComponent : AddonComponent** — Item ("box containing a disturbing portrait").
- `HauntedMirror.cs` — **HaunterMirrorComponent : AddonComponent** — Item ("box containing a haunted mirror").
- `MountedPixieBlue.cs` — **MountedPixieBlueComponent : AddonComponent** — Item.
- `MountedPixieGreen.cs` — **MountedPixieGreenComponent : AddonComponent** — Item.
- `MountedPixieLime.cs` — **MountedPixieLimeComponent : AddonComponent** — Item.
- `MountedPixieOrange.cs` — **MountedPixieOrangeComponent : AddonComponent** — Item.
- `MountedPixieWhite.cs` — **MountedPixieWhiteComponent : AddonComponent** — Item.
- `SacrificialAltar.cs` — **SacrificialAltarAddon : BaseAddonContainer** — Item ("box containing a sacrificial altar").
- `UnsettlingPortrait.cs` — **UnsettlingPortraitComponent : AddonComponent** — Item ("box containing an unsettling portrait").

#### Scripts/Items/Special/Items — Items: Items. Types: Banner, BloodyPentagramComponent, RewardBrazier, DecorativeShield, ....

- `Banner.cs` — **Banner : Item** — Item. [Constructable] (+5 types)
- `BloodyPentagram.cs` — **BloodyPentagramComponent : AddonComponent** — Item ("bowl of blood for a pentagram").
- `Brazier.cs` — **RewardBrazier : Item** — Item. [Constructable] (+2 types)
- `DecorativeShield.cs` — **DecorativeShield : Item** — Item. [Constructable] (+5 types)
- `FlamingHead.cs` — **FlamingHead : StoneFaceTrapNoDamage** — Item.
- `HangingSkeleton.cs` — **HangingSkeleton : Item** — Item. [Constructable] (+5 types)
- `PottedCactus.cs` — **RewardPottedCactus : Item** — Item. [Constructable] (+2 types)
- `StoneAnkh.cs` — **StoneAnkh : BaseAddon** — House addon. [Constructable] (+4 types)
- `WallBanner.cs` — **WallBanner : BaseAddon** — House addon. [Constructable] (+3 types)
- `WeaponEngravingTool.cs` — **WeaponEngravingTool : Item** — Item. [Constructable] (+5 types)

#### Scripts/Items/Special/Rares/Containers — Items: Containers. Types: BaseWaterContainer, Bucket, ClosedBarrel, HugeWaterTub, ....

- `BaseWaterContainer.cs` — **BaseWaterContainer : Container** — Container.
- `Bucket.cs` — **Bucket : BaseWaterContainer** — Item.
- `ClosedBarrel.cs` — **ClosedBarrel : TrapableContainer** — Item.
- `HugeWaterTub.cs` — **HugeWaterTub : Item** — Item ("huge tub of water"). [Constructable, Flipable]
- `UnfinishedBarrel.cs` — **UnfinishedBarrel : Item** — Item. [Constructable]
- `WaterBarrel.cs` — **WaterBarrel : BaseWaterContainer** — Item.
- `WaterTub.cs` — **Tub : BaseWaterContainer** — Item.

#### Scripts/Items/Special/Rares/Daily — Items: Daily. Types: DecoRock, DecoRock2, DecoRocks, DecoRocks2.

- `DecoRock.cs` — **DecoRock : Item** — Item. [Constructable]
- `DecoRock2.cs` — **DecoRock2 : Item** — Item. [Constructable]
- `DecoRocks.cs` — **DecoRocks : Item** — Item. [Constructable]
- `DecoRocks2.cs` — **DecoRocks2 : Item** — Item. [Constructable]

#### Scripts/Items/Special/Rares/Flowers — Items: Flowers. Types: DecoFlower, DecoFlower2, DecoRoseOfTrinsic, DecoRoseOfTrinsic2, ....

- `DecoFlower.cs` — **DecoFlower : Item** — Item ("white roses"). [Constructable]
- `DecoFlower2.cs` — **DecoFlower2 : Item** — Item ("white roses"). [Constructable]
- `DecoRoseOfTrinsic.cs` — **DecoRoseOfTrinsic : Item** — Item ("velvet rose"). [Constructable]
- `DecoRoseOfTrinsic2.cs` — **DecoRoseOfTrinsic2 : Item** — Item ("velvet rose"). [Constructable]
- `DecoRoseOfTrinsic3.cs` — **DecoRoseOfTrinsic3 : Item** — Item ("velvet rose"). [Constructable]

#### Scripts/Items/Special/Rares/Food — Items: Food. Types: DecoBottlesOfLiquor, DecoTray2, DecoTray.

- `BottlesOfLiquor.cs` — **DecoBottlesOfLiquor : Item** — Item. [Constructable]
- `Tray2.cs` — **DecoTray2 : Item** — Item. [Constructable]
- `Trays.cs` — **DecoTray : Item** — Item. [Constructable]

#### Scripts/Items/Special/Rares/Furniture — Items: Furniture. Types: BrokenChair.

- `BrokenChair.cs` — **BrokenChair : Item** — Item. [Constructable]

#### Scripts/Items/Special/Rares/GamePieces — Items: Game pieces. Types: Checkers, Checkers2, Chessmen, Chessmen2, ....

- `Checkers.cs` — **Checkers : Item** — Item. [Constructable]
- `Checkers2.cs` — **Checkers2 : Item** — Item. [Constructable]
- `Chessmen.cs` — **Chessmen : Item** — Item. [Constructable]
- `Chessmen2.cs` — **Chessmen2 : Item** — Item. [Constructable]
- `Chessmen3.cs` — **Chessmen3 : Item** — Item. [Constructable]

#### Scripts/Items/Special/Rares/Ingots — Items: Ingots. Types: DecoGoldIngot, DecoGoldIngot2, DecoGoldIngots, DecoGoldIngots2, ....

- `DecoGoldIngot.cs` — **DecoGoldIngot : Item** — Item. [Constructable]
- `DecoGoldIngot2.cs` — **DecoGoldIngot2 : Item** — Item. [Constructable]
- `DecoGoldIngots.cs` — **DecoGoldIngots : Item** — Item. [Constructable]
- `DecoGoldIngots2.cs` — **DecoGoldIngots2 : Item** — Item. [Constructable]
- `DecoGoldIngots3.cs` — **DecoGoldIngots3 : Item** — Item. [Constructable]
- `DecoGoldIngots4.cs` — **DecoGoldIngots4 : Item** — Item. [Constructable]
- `DecoIronIngot.cs` — **DecoIronIngot : Item** — Item. [Constructable]
- `DecoIronIngot2.cs` — **DecoIronIngot2 : Item** — Item. [Constructable]
- `DecoIronIngots.cs` — **DecoIronIngots : Item** — Item. [Constructable]
- `DecoIronIngots2.cs` — **DecoIronIngots2 : Item** — Item. [Constructable]
- `DecoIronIngots3.cs` — **DecoIronIngots3 : Item** — Item. [Constructable]
- `DecoIronIngots4.cs` — **DecoIronIngots4 : Item** — Item. [Constructable]
- `DecoIronIngots5.cs` — **DecoIronIngots5 : Item** — Item. [Constructable]
- `DecoIronIngots6.cs` — **DecoIronIngots6 : Item** — Item. [Constructable]
- `DecoSilverIngot.cs` — **DecoSilverIngot : Item** — Item. [Constructable]
- `DecoSilverIngot2.cs` — **DecoSilverIngot2 : Item** — Item. [Constructable]
- `DecoSilverIngots.cs` — **DecoSilverIngots : Item** — Item. [Constructable]
- `DecoSilverIngots2.cs` — **DecoSilverIngots2 : Item** — Item. [Constructable]
- `DecoSilverIngots3.cs` — **DecoSilverIngots3 : Item** — Item. [Constructable]
- `DecoSilverIngots4.cs` — **DecoSilverIngots4 : Item** — Item. [Constructable]
- `DecoSilverIngots5.cs` — **DecoSilverIngots5 : Item** — Item. [Constructable]

#### Scripts/Items/Special/Rares/Jars — Items: Jars. Types: EmptyJars, DecoFullJar, HalfEmptyJars.

- `EmptyJars.cs` — **EmptyJars : Item** — Item. [Constructable] (+4 types)
- `FullJars.cs` — **DecoFullJar : Item** — Item. [Constructable] (+2 types)
- `HalfEmptyJars.cs` — **HalfEmptyJars : Item** — Item. [Constructable] (+4 types)

#### Scripts/Items/Special/Rares/Magic — Items: Magic. Types: DecoCrystalBall, DecoMagicalCrystal.

- `DecoCrystalBall.cs` — **DecoCrystalBall : Item** — Item. [Constructable]
- `DecoMagicalCrystal.cs` — **DecoMagicalCrystal : Item** — Item. [Constructable]

#### Scripts/Items/Special/Rares/Misc — Items: Misc. Types: DecoSpittoon.

- `DecoSpittoon.cs` — **DecoSpittoon : Item** — Item. [Constructable]

#### Scripts/Items/Special/Rares/PaganReagents — Items: Pagan reagents. Types: DecoBlackmoor, DecoBloodspawn, DecoBrimstone, DecoDragonsBlood, ....

- `DecoBlackmoor.cs` — **DecoBlackmoor : Item** — Item. [Constructable]
- `DecoBloodspawn.cs` — **DecoBloodspawn : Item** — Item. [Constructable]
- `DecoBrimstone.cs` — **DecoBrimstone : Item** — Item. [Constructable]
- `DecoDragonsBlood.cs` — **DecoDragonsBlood : Item** — Item. [Constructable]
- `DecoDragonsBlood2.cs` — **DecoDragonsBlood2 : Item** — Item. [Constructable]
- `DecoEyeOfNewt.cs` — **DecoEyeOfNewt : Item** — Item. [Constructable]
- `DecoGarlic.cs` — **DecoGarlic : Item** — Item. [Constructable]
- `DecoGarlic2.cs` — **DecoGarlic2 : Item** — Item. [Constructable]
- `DecoGarlicBulb.cs` — **DecoGarlicBulb : Item** — Item. [Constructable]
- `DecoGarlicBulb2.cs` — **DecoGarlicBulb2 : Item** — Item. [Constructable]
- `DecoGinseng.cs` — **DecoGinseng : Item** — Item. [Constructable]
- `DecoGinseng2.cs` — **DecoGinseng2 : Item** — Item. [Constructable]
- `DecoGinsengRoot.cs` — **DecoGinsengRoot : Item** — Item. [Constructable]
- `DecoGinsengRoot2.cs` — **DecoGinsengRoot2 : Item** — Item. [Constructable]
- `DecoMandrake.cs` — **DecoMandrake : Item** — Item. [Constructable]
- `DecoMandrake2.cs` — **DecoMandrake2 : Item** — Item. [Constructable]
- `DecoMandrake3.cs` — **DecoMandrake3 : Item** — Item. [Constructable]
- `DecoMandrakeRoot.cs` — **DecoMandrakeRoot : Item** — Item. [Constructable]
- `DecoMandrakeRoot2.cs` — **DecoMandrakeRoot2 : Item** — Item. [Constructable]
- `DecoNightshade.cs` — **DecoNightshade : Item** — Item. [Constructable]
- `DecoNightshade2.cs` — **DecoNightshade2 : Item** — Item. [Constructable]
- `DecoNightshade3.cs` — **DecoNightshade3 : Item** — Item. [Constructable]
- `DecoObsidian.cs` — **DecoObsidian : Item** — Item. [Constructable]
- `DecoPumice.cs` — **DecoPumice : Item** — Item. [Constructable]
- `DecoWyrmsHeart.cs` — **DecoWyrmsHeart : Item** — Item. [Constructable]

#### Scripts/Items/Special/Rares/PlayingCards — Items: Playing cards. Types: Cards, Cards2, Cards3, Cards4, ....

- `Cards.cs` — **Cards : Item** — Item. [Constructable]
- `Cards2.cs` — **Cards2 : Item** — Item. [Constructable]
- `Cards3.cs` — **Cards3 : Item** — Item. [Constructable]
- `Cards4.cs` — **Cards4 : Item** — Item. [Constructable]
- `Cards5.cs` — **DecoCards5 : Item** — Item. [Constructable]
- `PlayingCards.cs` — **PlayingCards : Item** — Item. [Constructable]
- `PlayingCards2.cs` — **PlayingCards2 : Item** — Item. [Constructable]

#### Scripts/Items/Special/Rares/Stables — Items: Stables. Types: DecoBridle, DecoBridle2, DecoHay, DecoHay2, ....

- `Bridle.cs` — **DecoBridle : Item** — Item. [Constructable]
- `Bridle2.cs` — **DecoBridle2 : Item** — Item. [Constructable]
- `DecoHay.cs` — **DecoHay : Item** — Item. [Constructable]
- `DecoHay2.cs` — **DecoHay2 : Item** — Item. [Constructable]
- `DecoHorseDung.cs` — **DecoHorseDung : Item** — Item. [Constructable]

#### Scripts/Items/Special/Rares/TarotCards — Items: Tarot cards. Types: DecoDeckOfTarot, DecoDeckOfTarot2, DecoTarot, DecoTarot2, ....

- `DecoDeckOfTarot.cs` — **DecoDeckOfTarot : Item** — Item ("tarot cards"). [Constructable] (+1 type)
- `DecoDeckOfTarot2.cs` — **DecoDeckOfTarot2 : Item** — Item ("tarot cards"). [Constructable]
- `DecoTarot.cs` — **DecoTarot : Item** — Item ("tarot cards"). [Constructable]
- `DecoTarot2.cs` — **DecoTarot2 : Item** — Item ("tarot cards"). [Constructable]
- `DecoTarot3.cs` — **DecoTarot3 : Item** — Item ("tarot cards"). [Constructable]
- `DecoTarot4.cs` — **DecoTarot4 : Item** — Item ("tarot cards"). [Constructable]
- `DecoTarot5.cs` — **DecoTarot5 : Item** — Item ("tarot cards"). [Constructable]
- `DecoTarot6.cs` — **DecoTarot6 : Item** — Item ("tarot cards"). [Constructable]
- `DecoTarot7.cs` — **DecoTarot7 : Item** — Item ("tarot cards"). [Constructable]

#### Scripts/Items/Special/Rares/Tinker — Items: Tinker. Types: DecoArrowShafts, CrossbowBolts, EmptyToolKit, EmptyToolKit2, ....

- `ArrowShafts.cs` — **DecoArrowShafts : Item** — Item. [Constructable]
- `CrossbowBolts.cs` — **CrossbowBolts : Item** — Item. [Constructable]
- `EmptyToolKit.cs` — **EmptyToolKit : Item** — Item. [Constructable]
- `EmptyToolKit2.cs` — **EmptyToolKit2 : Item** — Item. [Constructable]
- `Lockpicks.cs` — **Lockpicks : Item** — Item. [Constructable]
- `ToolKit.cs` — **ToolKit : Item** — Item. [Constructable]

#### Scripts/Items/Technology — Sci-fi/technology items (ZorkPad, DataPads, devices).

- `AlienEgg.cs` — **AlienEgg : Item** — Item ("Alien Egg"). [Constructable] (+1 type)
- `BaseKilrathi.cs` — **BaseKilrathi : BaseMeleeWeapon** — Melee weapon.
- `Canteen.cs` — **Canteen : Item** — Item ("canteen"). [Constructable]
- `Chainsaw.cs` — **Chainsaw** — Source file (no top-level type declaration detected).
- `ComputerDatabase.cs` — **ComputerDatabase : Item** — Item ("computer terminal"). [Constructable] (+1 type)
- `DataPad.cs` — **DataPad : Item** — Item. [Constructable] (+1 type)
- `DoubleLaserSword.cs` — **DoubleLaserSword : BaseSword** — Sword weapon ("Double Laser Sword"). [Constructable]
- `DuctTape.cs` — **DuctTape : Item** — Item ("duct tape"). [Constructable] (+1 type)
- `FirstAidKit.cs` — **FirstAidKit** — Source file (no top-level type declaration detected).
- `KilrathiGun.cs` — **KilrathiGun : BaseKilrathi** — Item ("Kilrathi bowcaster").
- `KilrathiHeavyGun.cs` — **KilrathiHeavyGun : BaseKilrathi** — Item ("Kilrathi heavy bowcaster").
- `Krystal.cs` — **Krystal : Item** — Item ("krystal"). [Constructable]
- `Landmine.cs` — **Landmine : Item** — Item ("landmine"). [Constructable] (+1 type)
- `LightSword.cs` — **LightSword : BaseSword** — Sword weapon ("Laser Sword"). [Constructable]
- `MaterialLiquifier.cs` — **MaterialLiquifier : Item** — Item ("material liquifier"). [Constructable] (+1 type)
- `MedicalRecord.cs` — **MedicalRecord : Item** — Item ("Medical Record"). [Constructable] (+1 type)
- `PlasmaGrenade.cs` — **PlasmaGrenade : Item** — Item ("plasma grenade"). [Constructable] (+1 type)
- `PlasmaTorch.cs` — **PlasmaTorch : Item** — Item ("plasma torch"). [Constructable]
- `PortableSmelter.cs` — **PortableSmelter : Item** — Item ("portable smelter"). [Constructable] (+1 type)
- `ReagentJar.cs` — **ReagentJar : LockableContainer** — Item ("jar").
- `RomulanAle.cs` — **RomulanAle : Item** — Item ("romulan ale"). [Constructable]
- `SciFiJunk.cs` — **SciFiJunk : Item** — Item ("broken plastic bottle"). [Constructable]
- `ThermalDetonator.cs` — **ThermalDetonator : Item** — Item ("thermal detonator"). [Constructable] (+1 type)
- `ZorkGame.cs` — **ZorkGame** — Item.
- `ZorkPad.cs` — **ZorkPad : Item** — Item ("Zork!"). [Constructable] (+1 type)
- `ZorkWorld.cs` — **ZorkGame** — Item.

#### Scripts/Items/Trades — Crafting tools, resources and trade skill items.

- `BarkFragment.cs` — **BarkFragment : Item** — Item. [Constructable]
- `BaseHarvestTool.cs` — **BaseHarvestTool : Item** — Item. (+2 types)
- `BaseTool.cs` — **BaseTool : Item** — Item. (+1 type)
- `ProspectorsTool.cs` — **ProspectorsTool : BaseBashing** — Mace/hammer weapon. [Constructable] (+1 type)
- `SkinningKnifeTool.cs` — **SkinningKnifeTool : BaseHarvestTool** — Harvest tool. [Constructable]
- `Special.cs` — **BaseSpecial : Item** — Item. [Constructable] (+15 types)

#### Scripts/Items/Trades/Alchemy — Items: Alchemy. Types: AlchemyTub, GodBrewing, Bottle, CrystallineJar, ....

- `AlchemyTub.cs` — **AlchemyTub : Item** — Item ("alchemy tub"). [Constructable]
- `ApothecaryVials.cs` — **GodBrewing : BaseTool** — Tool ("apothecary set"). [Constructable]
- `Bottle.cs` — **Bottle : Item** — Item. [Constructable]
- `CrystallineJar.cs` — **CrystallineJar : Item** — Item ("crystalline flask"). [Constructable] (+2 types)
- `Jar.cs` — **Jar : Item** — Item ("jar"). [Constructable]
- `MortarPestle.cs` — **MortarPestle : BaseTool** — Tool ("alchemy set"). [Constructable]
- `PotionKeg.cs` — **PotionKeg : Item** — Item ("empty potion keg"). [Constructable, Flipable]

#### Scripts/Items/Trades/Blacksmithing — Items: Blacksmithing. Types: Anvil, BaseBlocks, DwarvenForge, FireGiantForge, ....

- `AnvilForge.cs` — **Anvil : Item** — Item. [Constructable, Flipable] (+1 type)
- `Blocks.cs` — **BaseBlocks : Item** — Item. [Constructable] (+15 types)
- `DwarvenForge.cs` — **DwarvenForge : Item** — Item ("dwarven forge"). [Constructable]
- `FireGiantForge.cs` — **FireGiantForge : Item** — Item ("smoldering cauldron"). [Constructable] (+1 type)
- `Ingots.cs` — **BaseIngot : Item** — Item. [Constructable] (+32 types)
- `LapidaryTools.cs` — **GodSmithing : BaseTool** — Tool ("lapidary hammer"). [Constructable]
- `LargeForge.cs` — **LargeForgeWest : Item** — Item. [Constructable] (+5 types)
- `Ore.cs` — **BaseOre : Item** — Item. (+32 types)
- `Pickaxe.cs` — **Pickaxe : BaseAxe** — Axe weapon ("gargoyle pickaxe"). [Constructable, Flipable]
- `RepairDeed.cs` — **RepairDeed : Item** — Item. [Constructable] (+2 types)
- `RubyPickaxe.cs` — **RubyPickaxe : BaseSword** — Sword weapon ("adamantium pickaxe"). [Constructable, Flipable] (+1 type)
- `Scales.cs` — **BaseScales : Item** — Item. [Constructable] (+17 types)
- `ScalingTools.cs` — **ScalingTools : BaseTool** — Tool ("scaling tools"). [Constructable]
- `SmithHammer.cs` — **SmithHammer : BaseTool** — Tool ("smith hammer"). [Constructable, Flipable]
- `SmithHammerRunic.cs` — **SmithHammerRunicI : BaseTool** — This tool can be used to create slightly magical items. [Constructable, Flipable] (+2 types)
- `Spade.cs` — **Spade : BaseAxe** — Axe weapon ("shovel"). [Constructable]

#### Scripts/Items/Trades/Bowcraft — Items: Bowcraft. Types: Arrow, ArrowsAndBolts, Bolt, Feather, ....

- `Arrow.cs` — **Arrow : Item** — Item ("arrow"). [Constructable]
- `ArrowsAndBolts.cs` — **ArrowsAndBolts** — Source file (no top-level type declaration detected).
- `Bolt.cs` — **Bolt : Item** — Item ("bolt"). [Constructable]
- `Feather.cs` — **Feather : Item** — Item ("feather"). [Constructable]
- `FletcherTools.cs` — **FletcherTools : BaseTool** — Tool ("bowcrafting tools"). [Constructable]
- `FletcherToolsRunic.cs` — **FletcherToolsRunicI : BaseTool** — This tool can be used to create slightly magical items. [Constructable] (+2 types)
- `Shaft.cs` — **Shaft : Item** — Item ("shaft"). [Constructable]

#### Scripts/Items/Trades/Carpentry — Items: Carpentry. Types: Board, CarpenterTools, CarpenterToolsRunicI, TaxidermyKit, ....

- `Board.cs` — **Board : BaseWoodBoard** — Trade item/tool.
- `CarpenterTools.cs` — **CarpenterTools : BaseTool** — Tool ("carpenter tools"). [Constructable]
- `CarpenterToolsRunic.cs` — **CarpenterToolsRunicI : BaseTool** — This tool can be used to create slightly magical items. [Constructable] (+2 types)
- `TaxidermyKit.cs` — **TaxidermyKit : Item** — Item. [Constructable, Flipable] (+4 types)
- `WoodworkingTools.cs` — **WoodworkingTools : BaseTool** — Tool ("woodworking tools"). [Constructable]

#### Scripts/Items/Trades/Cartography — Items: Cartography. Types: MapmakersPen.

- `MapmakersPen.cs` — **MapmakersPen : BaseTool** — Tool ("cartography quill"). [Constructable]

#### Scripts/Items/Trades/Cartography/Maps — Items: Maps. Types: BlankMap, CityMap, IndecipherableMap, LocalMap, ....

- `BlankMap.cs` — **BlankMap : MapItem** — Trade item/tool.
- `CityMap.cs` — **CityMap : MapItem** — Trade item/tool.
- `IndecipherableMap.cs` — **IndecipherableMap : MapItem** — Trade item/tool.
- `LocalMap.cs` — **LocalMap : MapItem** — Trade item/tool.
- `MapItem.cs` — **MapItem : Item** — Item. [Constructable, Flipable] (+6 types)
- `MapRanger.cs` — **MapRanger : Item** — //////////////////////////////////////////////////////////////////////////////////////////// [Constructable] (+4 types)
- `MapWorld.cs` — **MapWorld : Item** — Item ("world map"). [Constructable]
- `PlaceMap.cs` — **PlaceMap : Item** — Item ("map"). [Constructable]
- `PresetMap.cs` — **PresetMap : MapItem** — Trade item/tool.
- `SeaChart.cs` — **SeaChart : MapItem** — Trade item/tool.
- `TreasureMap.cs` — **TreasureMap : MapItem** — Commands: [MapTreasure [Constructable] (+8 types)
- `WorldMap.cs` — **WorldMap : MapItem** — Trade item/tool.

#### Scripts/Items/Trades/Cooking — Items: Cooking. Types: CulinarySet.

- `CulinarySet.cs` — **CulinarySet : BaseTool** — Tool ("culinary set"). [Constructable]

#### Scripts/Items/Trades/Druid — Items: Druid. Types: DruidCauldron.

- `DruidCauldron.cs` — **DruidCauldron : BaseTool** — Tool ("druid's cauldron"). [Constructable]

#### Scripts/Items/Trades/Fishing — Items: Fishing. Types: BigFish, FabledFishingNet, Fish, FishBarrel, ....

- `BigFish.cs` — **BigFish : Item** — Item. [Constructable]
- `FabledFishingNet.cs` — **FabledFishingNet : Item** — Item ("Ancient Fishing Net"). [Constructable]
- `Fish.cs` — **Fish : Item** — Item. [Constructable]
- `FishBarrel.cs` — **FishBarrel : Item** — Item ("Exotic Fish Tub"). [Constructable]
- `FishingNet.cs` — **FishingNet : Item** — Item ("fishing net"). [Constructable]
- `FishingPole.cs` — **FishingPole : BaseHarvestTool** — Harvest tool ("fishing pole"). [Constructable]
- `HighSeasRelic.cs` — **HighSeasRelic : Item** — Item ("anchor"). [Constructable]
- `MagicFish.cs` — **BaseMagicFish : Item** — Item. [Constructable] (+4 types)
- `NeptunesFishingNet.cs` — **NeptunesFishingNet : Item** — Item ("Neptune's Fishing Net"). [Constructable]
- `NewFish.cs` — **NewFish : Item** — Item ("red "). [Constructable]
- `PearlSkull.cs` — **PearlSkull** — Source file (no top-level type declaration detected).
- `RustyJunk.cs` — **RustyJunk : Item** — Item ("rusty shield"). [Constructable] (+1 type)
- `ScrapIronBarrel.cs` — **ScrapIronBarrel : Item** — Item ("Scrap Iron Barrel"). [Constructable]
- `SpecialFishingNet.cs` — **SpecialFishingNet : Item** — Item ("strong fishing net"). [Constructable]
- `SpecialSeaweed.cs` — **SpecialSeaweed : Item** — Item ("Seaweed of Nightsight"). [Constructable]
- `WetClothes.cs` — **WetClothes : Item** — Item ("clothes"). [Constructable]

#### Scripts/Items/Trades/Fishing/Misc — Items: Misc. Types: MagicSextant, MessageInABottle, SOS, Sextant, ....

- `MagicSextant.cs` — **MagicSextant : Sextant** — Trade item/tool ("magic sextant").
- `MessageInABottle.cs` — **MessageInABottle : Item** — Item. [Constructable]
- `SOS.cs` — **SOS : Item** — Item. [Constructable] (+1 type)
- `Sextant.cs` — **Sextant : Item** — Item ("sextant"). (+2 types)
- `ShipwreckedItem.cs` — **ShipwreckedItem : Item** — Item. [Constructable] (+1 type)

#### Scripts/Items/Trades/Fishing/Sea Artifacts — Items: Sea artifacts. Types: AquariumEastAddon, AquariumSouthAddon, DolphinEastLargeAddon, DolphinEastSmallAddon, ....

- `AquariumEastAddon.cs` — **AquariumEastAddon : BaseAddon** — House addon ("aquarium (east)"). [Constructable] (+1 type)
- `AquariumSouthAddon.cs` — **AquariumSouthAddon : BaseAddon** — House addon ("aquarium (south)"). [Constructable] (+1 type)
- `DolphinEastLargeAddon.cs` — **DolphinEastLargeAddon : BaseAddon** — House addon ("large dolphin rug (east)"). [Constructable] (+1 type)
- `DolphinEastSmallAddon.cs` — **DolphinEastSmallAddon : BaseAddon** — House addon ("small dolphin run (east)"). [Constructable] (+1 type)
- `DolphinSouthLargeAddon.cs` — **DolphinSouthLargeAddon : BaseAddon** — House addon ("large dolphin rug (south)"). [Constructable] (+1 type)
- `DolphinSouthSmallAddon.cs` — **DolphinSouthSmallAddon : BaseAddon** — House addon ("small dolphin rug (south)"). [Constructable] (+1 type)
- `LightHouse.cs` — **LightHouseAddon : BaseAddon** — House addon ("lighthouse"). [Constructable] (+1 type)
- `MarlinEastAddon.cs` — **MarlinEastAddon : BaseAddon** — House addon ("Marlin Trophy (east)"). [Constructable] (+1 type)
- `MarlinSouthAddon.cs` — **MarlinSouthAddon : BaseAddon** — House addon ("Marlin Trophy (south)"). [Constructable] (+1 type)
- `SeaShells.cs` — **SeaShell : Item** — Item ("sea shell"). [Constructable]
- `SkullEastLargeAddon.cs` — **SkullEastLargeAddon : BaseAddon** — House addon ("large skull rug (east)"). [Constructable] (+1 type)
- `SkullEastSmallAddon.cs` — **SkullEastSmallAddon : BaseAddon** — House addon ("small skull rug (east)"). [Constructable] (+1 type)
- `SkullSouthLargeAddon.cs` — **SkullSouthLargeAddon : BaseAddon** — House addon ("large skull rug (south)"). [Constructable] (+1 type)
- `SkullSouthSmallAddon.cs` — **SkullSouthSmallAddon : BaseAddon** — House addon ("small skull rug (south)"). [Constructable] (+1 type)

#### Scripts/Items/Trades/Forensics — Items: Forensics. Types: Bones, GraveSpade, PolishBoneBrush, BaseSkeletal, ....

- `Bones.cs` — **Bones : Item** — Item ("bones"). [Constructable]
- `GraveSpade.cs` — **GraveSpade : BaseAxe** — Axe weapon ("grave shovel"). [Constructable]
- `PolishBoneBrush.cs` — **PolishBoneBrush : Item** — Item ("bone polishing brush"). [Constructable] (+1 type)
- `Skeletal.cs` — **BaseSkeletal : Item** — Item. [Constructable] (+26 types)
- `UndertakerKit.cs` — **UndertakerKit : BaseTool** — Tool ("undertaker kit"). [Constructable]
- `UndertakerKitRunic.cs` — **UndertakerKitRunicI : BaseTool** — This tool can be used to create slightly magical items. [Constructable] (+2 types)

#### Scripts/Items/Trades/Glass Stone — Items: Glass stone. Types: Blowpipe, GlassblowingBook, Granite, MalletAndChisel, ....

- `Blowpipe.cs` — **Blowpipe : BaseTool** — Tool ("blowpipe"). [Constructable, Flipable]
- `GlassblowingBook.cs` — **GlassblowingBook : Item** — Item. [Constructable]
- `Granite.cs` — **Granite : BaseGranite** — Trade item/tool.
- `MalletAndChisel.cs` — **MalletAndChisel : BaseTool** — Tool ("mallet and chisel"). [Constructable]
- `MasonryBook.cs` — **MasonryBook : Item** — Item. [Constructable]
- `Sand.cs` — **Sand : Item** — Item. [Constructable]
- `SandMiningBook.cs` — **SandMiningBook : Item** — Item. [Constructable]
- `StoneMiningBook.cs` — **StoneMiningBook : Item** — Item. [Constructable]

#### Scripts/Items/Trades/Inscription — Items: Inscription. Types: Monocle, ScribesPen, ScribesPenRunicI, TomeOfWands.

- `Monocle.cs` — **Monocle : BaseHarvestTool** — Harvest tool ("librarian set"). [Constructable]
- `ScribesPen.cs` — **ScribesPen : BaseTool** — Tool ("scribe quill"). [Constructable]
- `ScribesPenRunic.cs` — **ScribesPenRunicI : BaseTool** — This tool can be used to create slightly magical items. [Constructable] (+2 types)
- `TomeOfWands.cs` — **TomeOfWands : BaseTool** — Tool ("tome of wands"). [Constructable, Flipable]

#### Scripts/Items/Trades/Lumberjack — Items: Lumberjack. Types: Log, SawMillEastAddon, SawMillSouthAddon.

- `Log.cs` — **Log : BaseLog** — Trade item/tool.
- `SawMillEastAddon.cs` — **SawMillEastAddon : BaseAddon** — House addon ("saw mill deed (east)"). [Constructable] (+1 type)
- `SawMillSouthAddon.cs` — **SawMillSouthAddon : BaseAddon** — House addon ("saw mill deed (south)"). [Constructable] (+1 type)

#### Scripts/Items/Trades/Magical — Items: Magical. Types: ArcaneScroll, BlankScroll, Moongate, RecallRune.

- `ArcaneScroll.cs` — **ArcaneScroll : Item** — Item ("Arcane Scroll"). [Constructable]
- `BlankScroll.cs` — **BlankScroll : Item** — Item. [Constructable]
- `Moongate.cs` — **Moongate : Item** — Item. [Constructable] (+3 types)
- `RecallRune.cs` — **RecallRune : Item** — Item. [Flipable] (+1 type)

#### Scripts/Items/Trades/Magical/Tools — Items: Tools. Types: BaseRunicTool, RunicFletching, RunicHammer, RunicLeatherKit, ....

- `BaseRunicTool.cs` — **BaseRunicTool : BaseTool** — Tool. (+2 types)
- `RunicFletching.cs` — **RunicFletching : BaseRunicTool** — Trade item/tool ("bowyer tools").
- `RunicHammer.cs` — **RunicHammer : BaseRunicTool** — Trade item/tool ("smith hammer").
- `RunicLeatherKit.cs` — **RunicLeatherKit : BaseRunicTool** — Trade item/tool ("tanning kit").
- `RunicSaw.cs` — **RunicSaw : BaseRunicTool** — Trade item/tool ("woodworking tools").
- `RunicScales.cs` — **RunicScales : BaseRunicTool** — Trade item/tool ("scaling tools").
- `RunicSewingKit.cs` — **RunicSewingKit : BaseRunicTool** — Trade item/tool ("sewing kit").
- `RunicTinker.cs` — **RunicTinker : BaseRunicTool** — Trade item/tool ("tinker tools").
- `RunicUndertaker.cs` — **RunicUndertaker : BaseRunicTool** — Trade item/tool ("undertaker tools").

#### Scripts/Items/Trades/Misc — Items: Misc. Types: Bandage, FireHorn.

- `Bandage.cs` — **Bandage : Item** — Item ("bandage"). [Constructable] (+4 types)
- `FireHorn.cs` — **FireHorn : Item** — Item. [Constructable] (+1 type)

#### Scripts/Items/Trades/Ninjitsu — Items: Ninjitsu. Types: EggBomb, Fukiya, FukiyaDarts, LeatherNinjaBelt, ....

- `EggBomb.cs` — **EggBomb : Item** — Item. [Constructable]
- `Fukiya.cs` — **Fukiya : Item** — Item. [Constructable, Flipable]
- `FukiyaDarts.cs` — **FukiyaDarts : Item** — Item. [Constructable]
- `LeatherNinjaBelt.cs` — **LeatherNinjaBelt : BaseWaist** — Trade item/tool ("ninja belt").
- `NinjaWeapons.cs` — **INinjaAmmo : IUsesRemaining** — Interface contract. Base: IUsesRemaining. (+4 types)
- `Shuriken.cs` — **Shuriken : Item** — Item. [Constructable, Flipable]
- `SmokeBomb.cs` — **SmokeBomb : Item** — Item. [Constructable]

#### Scripts/Items/Trades/Plants/Potted — Items: Potted. Types: SmallEmptyPot, PottedCactus, PottedPlant, PottedTree.

- `EmptyPots.cs` — **SmallEmptyPot : Item** — Item. [Constructable] (+1 type)
- `PottedCactus.cs` — **PottedCactus : Item** — Item. [Constructable] (+5 types)
- `PottedPlants.cs` — **PottedPlant : Item** — Item. [Constructable] (+2 types)
- `PottedTrees.cs` — **PottedTree : Item** — Item. [Constructable] (+1 type)

#### Scripts/Items/Trades/Reagents — Items: Reagents. Types: BagOfAlchemicReagents, BagOfAllReagents, BagOfNecroReagents, BagOfReagents, ....

- `BagOfAlchemicReagents.cs` — **BagOfAlchemicReagents : Bag** — Trade item/tool.
- `BagOfAllReagents.cs` — **BagOfAllReagents : Bag** — Trade item/tool.
- `BagOfNecroReagents.cs` — **BagOfNecroReagents : Bag** — Trade item/tool.
- `BagOfReagents.cs` — **BagOfReagents : Bag** — Trade item/tool.
- `BaseReagent.cs` — **BaseReagent : Item** — Item.
- `Reagents.cs` — **Reagents** — Source file (no top-level type declaration detected).

#### Scripts/Items/Trades/Reagents/Alchemy — Items: Alchemy. Types: BeetleShell, Brimstone, ButterflyWings, EyeOfToad, ....

- `BeetleShell.cs` — **BeetleShell : BaseReagent** — Trade item/tool ("beetle shell").
- `Brimstone.cs` — **Brimstone : BaseReagent** — Trade item/tool ("brimstone").
- `ButterflyWings.cs` — **ButterflyWings : BaseReagent** — Trade item/tool ("butterfly wings").
- `EyeOfToad.cs` — **EyeOfToad : BaseReagent** — Trade item/tool ("eye of toad").
- `FairyEgg.cs` — **FairyEgg : BaseReagent** — Trade item/tool ("fairy egg").
- `GargoyleEar.cs` — **GargoyleEar : BaseReagent** — Trade item/tool ("gargoyle ear").
- `MoonCrystal.cs` — **MoonCrystal : BaseReagent** — Trade item/tool ("moon crystal").
- `PixieSkull.cs` — **PixieSkull : BaseReagent** — Trade item/tool ("pixie skull").
- `RedLotus.cs` — **RedLotus : BaseReagent** — Trade item/tool ("red lotus").
- `SeaSalt.cs` — **SeaSalt : BaseReagent** — Trade item/tool ("sea salt").
- `SilverWidow.cs` — **SilverWidow : BaseReagent** — Trade item/tool ("silver widow").
- `SwampBerries.cs` — **SwampBerries : BaseReagent** — Trade item/tool ("swamp berries").

#### Scripts/Items/Trades/Reagents/Common — Items: Common. Types: BlackPearl, Bloodmoss, Garlic, Ginseng, ....

- `BlackPearl.cs` — **BlackPearl : BaseReagent** — Trade item/tool ("black pearl").
- `Bloodmoss.cs` — **Bloodmoss : BaseReagent** — Trade item/tool.
- `Garlic.cs` — **Garlic : BaseReagent** — Trade item/tool.
- `Ginseng.cs` — **Ginseng : BaseReagent** — Trade item/tool.
- `MandrakeRoot.cs` — **MandrakeRoot : BaseReagent** — Trade item/tool.
- `Nightshade.cs` — **Nightshade : BaseReagent** — Trade item/tool.
- `SpidersSilk.cs` — **SpidersSilk : BaseReagent** — Trade item/tool.
- `SulfurousAsh.cs` — **SulfurousAsh : BaseReagent** — Trade item/tool.

#### Scripts/Items/Trades/Reagents/Farm — Items: Farm. Types: FarmableGarlic, FarmableGinseng, FarmableMandrakeRoot, FarmableNightshade.

- `FarmableGarlic.cs` — **FarmableGarlic : FarmableCrop** — Trade item/tool.
- `FarmableGinseng.cs` — **FarmableGinseng : FarmableCrop** — Trade item/tool.
- `FarmableMandrakeRoot.cs` — **FarmableMandrakeRoot : FarmableCrop** — Trade item/tool.
- `FarmableNightshade.cs` — **FarmableNightshade : FarmableCrop** — Trade item/tool.

#### Scripts/Items/Trades/Reagents/Necromancy — Items: Necromancy. Types: BatWing, DaemonBlood, GraveDust, NoxCrystal, ....

- `BatWing.cs` — **BatWing : BaseReagent** — Trade item/tool.
- `DaemonBlood.cs` — **DaemonBlood : BaseReagent** — Trade item/tool.
- `GraveDust.cs` — **GraveDust : BaseReagent** — Trade item/tool.
- `NoxCrystal.cs` — **NoxCrystal : BaseReagent** — Trade item/tool.
- `PigIron.cs` — **PigIron : BaseReagent** — Trade item/tool.

#### Scripts/Items/Trades/Reagents/Unique — Items: Unique. Types: DemigodBlood, DemonClaw, DragonBlood, DragonTooth, ....

- `DemigodBlood.cs` — **DemigodBlood : Item** — Item ("demigod blood"). [Constructable]
- `DemonClaw.cs` — **DemonClaw : Item** — Item ("demon claw"). [Constructable]
- `DragonBlood.cs` — **DragonBlood : Item** — Item ("dragon blood"). [Constructable]
- `DragonTooth.cs` — **DragonTooth : Item** — Item ("dragon tooth"). [Constructable]
- `EnchantedSeaweed.cs` — **EnchantedSeaweed : Item** — Item ("enchanted seaweed"). [Constructable]
- `GhostlyDust.cs` — **GhostlyDust : Item** — Item ("ghostly dust"). [Constructable]
- `GoldenFeathers.cs` — **GoldenFeathers : Item** — Item ("golden feathers"). [Constructable]
- `GoldenSerpentVenom.cs` — **GoldenSerpentVenom : BasePoisonPotion** — Trade item/tool ("golden serpent venom").
- `LichDust.cs` — **LichDust : Item** — Item ("lich dust"). [Constructable]
- `PegasusFeather.cs` — **PegasusFeather : Item** — Item ("pegasus feather"). [Constructable]
- `PhoenixFeather.cs` — **PhoenixFeather : Item** — Item ("phoenix feather"). [Constructable]
- `SilverSerpentVenom.cs` — **SilverSerpentVenom : BasePoisonPotion** — Trade item/tool ("silver serpent venom").
- `UnicornHorn.cs` — **UnicornHorn : Item** — Item ("unicorn horn"). [Constructable]

#### Scripts/Items/Trades/Reagents/Witch — Items: Witch. Types: BitterRoot, BlackSand, BloodRose, DriedToad, ....

- `BitterRoot.cs` — **BitterRoot : BaseReagent** — Trade item/tool ("bitter root").
- `BlackSand.cs` — **BlackSand : BaseReagent** — Trade item/tool ("black sand").
- `BloodRose.cs` — **BloodRose : BaseReagent** — Trade item/tool ("blood rose").
- `DriedToad.cs` — **DriedToad : BaseReagent** — Trade item/tool ("dried toad").
- `Maggot.cs` — **Maggot : BaseReagent** — Trade item/tool ("maggot").
- `MummyWrap.cs` — **MummyWrap : BaseReagent** — Trade item/tool ("mummy wrap").
- `VioletFungus.cs` — **VioletFungus : BaseReagent** — Trade item/tool ("violet fungus").
- `WerewolfClaw.cs` — **WerewolfClaw : BaseReagent** — Trade item/tool ("werewolf claw").
- `Wolfsbane.cs` — **Wolfsbane : BaseReagent** — Trade item/tool ("wolfsbane").

#### Scripts/Items/Trades/Tailoring — Items: Tailoring. Types: Bone, Cotton, Dressform, Dyes, ....

- `Bone.cs` — **Bone : Item** — Item. [Constructable]
- `Cotton.cs` — **Cotton : Item** — Item. [Constructable] (+1 type)
- `Dressform.cs` — **Dressform : Item** — Item. [Constructable, Flipable]
- `Dyes.cs` — **Dyes : Item** — Item. [Constructable] (+2 types)
- `Fabric.cs` — **Fabric : BaseFabric** — Trade item/tool.
- `Flax.cs` — **Flax : Item** — Item. [Constructable] (+1 type)
- `Hides.cs` — **Hides : BaseHides** — Trade item/tool.
- `Leathers.cs` — **BaseLeather : Item** — Item. [Constructable] (+23 types)
- `LeatherworkingTools.cs` — **LeatherworkingTools : BaseTool** — Tool ("tanning tools"). [Constructable]
- `LeatherworkingToolsRunic.cs` — **LeatherworkingToolsRunicI : BaseTool** — This tool can be used to create slightly magical items. [Constructable] (+2 types)
- `Scissors.cs` — **Scissors : Item** — Item. [Constructable, Flipable] (+2 types)
- `SewingKit.cs` — **SewingKit : BaseTool** — Tool ("sewing kit"). [Constructable]
- `SewingKitRunic.cs` — **SewingKitRunicI : BaseTool** — This tool can be used to create slightly magical items. [Constructable] (+2 types)
- `Skins.cs` — **BaseSkins : Item** — Item. [Constructable] (+10 types)
- `SpoolOfThread.cs` — **SpoolOfThread : BaseClothMaterial** — Trade item/tool ("spool of string").
- `StitchingTools.cs` — **GodSewing : BaseTool** — Tool ("stitching tools"). [Constructable]
- `Wool.cs` — **Wool : Item** — Item. [Constructable] (+2 types)

#### Scripts/Items/Trades/Thieving — Items: Thieving. Types: DisguiseKit, DisguisePersistance, ILockpickable, MagicSkeltonsKey, ....

- `DisguiseKit.cs` — **DisguiseKit : Item** — Item ("disguise kit"). [Constructable] (+2 types)
- `DisguisePersistance.cs` — **DisguisePersistance : Item** — Item.
- `LockPick.cs` — **ILockpickable : IPoint2D** — Those skilled in lockpicking, can use these to open locked items. Use the lockpick and select the locked it... [Constructable, Flipable] (+4 types)
- `MagicSkeltonsKey.cs` — **MagicSkeltonsKey : Item** — Item ("magic skeleton key"). [Constructable]
- `MasterSkeltonsKey.cs` — **MasterSkeletonsKey : Item** — These access cards can open almost any technological door or container. Use the access card and select lock... [Constructable]
- `PickBox.cs` — **IAutoLockingContainer** — ////////////////////////////////////////////////////////////////////////////////////// [Constructable, Flipable] (+8 types)
- `SkeltonsKey.cs` — **SkeletonsKey : Item** — These access cards can open many technological doors or containers. Use the access card and select locked i... [Constructable]
- `StolenChest.cs` — **StolenChest : Bag** — Trade item/tool.

#### Scripts/Items/Trades/Tinkering — Items: Tinkering. Types: Axle, AxleGears, ClockFrame, ClockParts, ....

- `Axle.cs` — **Axle : Item** — Item. [Constructable, Flipable]
- `AxleGears.cs` — **AxleGears : Item** — Item. [Constructable, Flipable]
- `ClockFrame.cs` — **ClockFrame : Item** — Item. [Constructable, Flipable]
- `ClockParts.cs` — **ClockParts : Item** — Item. [Constructable, Flipable]
- `Clocks.cs` — **MoonPhase** — Enum with about 8 members. [Constructable, Flipable] (+6 types)
- `Gears.cs` — **Gears : Item** — Item. [Constructable, Flipable]
- `Globe.cs` — **Globe : Item** — Item. [Constructable]
- `Hinge.cs` — **Hinge : Item** — Item. [Constructable, Flipable]
- `SextantParts.cs` — **SextantParts : Item** — Item. [Constructable, Flipable]
- `Springs.cs` — **Springs : Item** — Item. [Constructable, Flipable]
- `TinkerTools.cs` — **TinkerTools : BaseTool** — Tool ("tinker tools"). [Constructable]
- `TinkerToolsRunic.cs` — **TinkerToolsRunicI : BaseTool** — This tool can be used to create slightly magical items. [Constructable] (+2 types)
- `Utensils.cs` — **Fork : Item** — Item. [Constructable, Flipable] (+9 types)

#### Scripts/Items/Trades/Witch — Items: Witch. Types: WitchCauldron.

- `WitchCauldron.cs` — **WitchCauldron : BaseTool** — Tool ("witch's cauldron"). [Constructable]

#### Scripts/Items/Traps — Items: Traps. Types: BaseTrap, StopBlock, BookBox, BrokenGear, ....

- `BaseTrap.cs` — **BaseTrap : Item** — Item.
- `Blocker.cs` — **StopBlock : Item** — Item ("blocker"). [Constructable]
- `BookBox.cs` — **BookBox : LockableContainer** — Item ("books").
- `BrokenGear.cs` — **BrokenGear** — Source file (no top-level type declaration detected).
- `CurseItem.cs` — **CurseItem : LockableContainer** — Item ("cursed item").
- `DroppedPack.cs` — **DroppedContainer : Container** — Container ("dropped backpack"). [Constructable] (+1 type)
- `FireColumnTrap.cs` — **FireColumnTrap : BaseTrap** — Trap. [Constructable]
- `FlameSpurtTrap.cs` — **FlameSpurtTrap : BaseTrap** — Trap. [Constructable]
- `GasTrap.cs` — **GasTrap : BaseTrap** — Trap. [Constructable] (+1 type)
- `GiantSpikeTrap.cs` — **GiantSpikeTrap : BaseTrap** — Trap. [Constructable]
- `HiddenTrap.cs` — **HiddenTrap : Item** — Item ("a hidden trap"). [Constructable] (+2 types)
- `JewelryBox.cs` — **JewelryBox : LockableContainer** — Item ("jewelry box").
- `KillerTile.cs` — **KillerTile : Item** — Item ("killer"). [Constructable]
- `MushroomTrap.cs` — **MushroomTrap : BaseTrap** — Trap ("mushroom"). [Constructable]
- `RottedReagents.cs` — **RottedReagents** — Source file (no top-level type declaration detected).
- `RuinedGems.cs` — **RuinedGems** — Source file (no top-level type declaration detected).
- `SawTrap.cs` — **SawTrap : BaseTrap** — Trap. [Constructable] (+1 type)
- `SetTrap.cs` — **SetTrap : Item** — Item ("a trap"). [Constructable]
- `SewageItem.cs` — **SewageItem : LockableContainer** — Item ("disgusting item").
- `SlimeItem.cs` — **SlimeItem : LockableContainer** — Item ("slime covered item").
- `SpellTrap.cs` — **SpellTrap : Item** — Item ("a magical trap"). [Constructable]
- `SpikeTrap.cs` — **SpikeTrap : BaseTrap** — Trap. [Constructable] (+1 type)
- `StoneFaceTrap.cs` — **StoneFaceTrap : BaseTrap** — Trap. [Constructable] (+2 types)
- `TaintedBandage.cs` — **TaintedBandage** — Source file (no top-level type declaration detected).
- `TenFootPole.cs` — **TenFootPole : Item** — Item ("ten foot pole"). [Constructable]
- `TrapKit.cs` — **TrapKit : Item** — Item ("trapping tools"). [Constructable] (+4 types)
- `TrapPotionEffects.cs` — **TrapPotionEffects** — Item.
- `TrapWand.cs` — **TrapWand : Item** — Item ("orb of trap finding for "). [Constructable] (+1 type)
- `WeedItem.cs` — **WeededItem : LockableContainer** — Item ("weed covered item").

#### Scripts/Items/Trinkets — Items: Trinkets. Types: BaseTrinket, GuildRing, JewelryBracelet, JewelryCirclet, ....

- `BaseTrinket.cs` — **BaseTrinket : Item** — Item. (+2 types)
- `GuildRing.cs` — **GuildRing : JewelryRing** — Item ("guild ring").
- `JewelryBracelet.cs` — **JewelryBracelet : BaseTrinket** — Trinket/jewelry ("bracelet"). [Constructable]
- `JewelryCirclet.cs` — **JewelryCirclet : BaseTrinket** — Trinket/jewelry ("circlet"). [Constructable]
- `JewelryEarrings.cs` — **JewelryEarrings : BaseTrinket** — Trinket/jewelry ("earrings"). [Constructable]
- `JewelryNecklace.cs` — **JewelryNecklace : BaseTrinket** — Trinket/jewelry ("necklace"). [Constructable]
- `JewelryRing.cs` — **JewelryRing : BaseTrinket** — Trinket/jewelry ("ring"). [Constructable]
- `MagicalWand.cs` — **MagicalWand : BaseTrinket** — Trinket/jewelry ("Magic Wand of "). [Constructable]
- `OldSwordTalisman.cs` — **OldSwordTalisman : BaseTrinket** — Trinket/jewelry ("An old sword"). [Constructable]
- `SavageTalisman.cs` — **SavageTalisman : TrinketTalisman** — Item ("barbaric talisman").
- `TrinketCandle.cs` — **TrinketCandle : BaseTrinket** — Trinket/jewelry ("candle"). [Constructable]
- `TrinketLantern.cs` — **TrinketLantern : BaseTrinket** — Trinket/jewelry ("lantern"). [Constructable]
- `TrinketTalisman.cs` — **TrinketTalisman : BaseTrinket** — Trinket/jewelry ("talisman"). [Constructable]
- `TrinketTorch.cs` — **TrinketTorch : BaseTrinket** — Trinket/jewelry ("torch"). [Constructable]

#### Scripts/Items/Weapons — Weapon item classes grouped by family.

- `BaseMeleeWeapon.cs` — **BaseMeleeWeapon : BaseWeapon** — Weapon.
- `BaseWeapon.cs` — **BaseWeapon : Item** — Base weapon: identity, name/property building, quality, durability, slayers.
- `HitLower.cs` — **HitLower** — Weapon item.
- `SlayerEntry.cs` — **SlayerEntry** — Data entry/record.
- `SlayerGroup.cs` — **SlayerGroup** — Weapon item.
- `SlayerName.cs` — **SlayerName** — Enum with about 36 members.
- `WeaponEnums.cs` — **WeaponQuality** — Enum with about 3 members. (+5 types)

#### Scripts/Items/Weapons/Axes — Weapon items: Axes. Types: Axe, BaseAxe, BattleAxe, DoubleAxe, ....

- `Axe.cs` — **Axe : BaseAxe** — Axe weapon ("axe"). [Constructable, Flipable]
- `BaseAxe.cs` — **BaseAxe : BaseMeleeWeapon** — When holding this in your hand, you can use it to select a tree you wish to chop. Doing this may help you g... (+1 type)
- `BattleAxe.cs` — **BattleAxe : BaseAxe** — Axe weapon ("battle axe"). [Constructable, Flipable]
- `DoubleAxe.cs` — **DoubleAxe : BaseAxe** — Axe weapon. [Constructable, Flipable]
- `ExecutionersAxe.cs` — **ExecutionersAxe : BaseAxe** — Axe weapon ("great axe"). [Constructable]
- `Hatchet.cs` — **Hatchet : BaseAxe** — Axe weapon. [Constructable, Flipable]
- `LargeBattleAxe.cs` — **LargeBattleAxe : BaseAxe** — Axe weapon ("large battle axe"). [Constructable]
- `OrnateAxe.cs` — **OrnateAxe : BaseAxe** — Axe weapon ("barbarian axe"). [Constructable]
- `TwoHandedAxe.cs` — **TwoHandedAxe : BaseAxe** — Axe weapon ("two handed axe"). [Constructable]
- `WarAxe.cs` — **WarAxe : BaseAxe** — Axe weapon ("war axe"). [Constructable]

#### Scripts/Items/Weapons/Bows — Weapon items: Bows. Types: BaseRanged, Bow, CompositeBow, Crossbow, ....

- `BaseRanged.cs` — **BaseRanged : BaseMeleeWeapon** — Melee weapon.
- `Bow.cs` — **Bow : BaseRanged** — Ranged weapon ("bow"). [Constructable]
- `CompositeBow.cs` — **CompositeBow : BaseRanged** — Ranged weapon ("composite bow"). [Constructable]
- `Crossbow.cs` — **Crossbow : BaseRanged** — Ranged weapon ("crossbow"). [Constructable]
- `ElvenCompositeLongbow.cs` — **ElvenCompositeLongbow : BaseRanged** — Ranged weapon ("woodland longbow"). [Constructable]
- `HeavyCrossbow.cs` — **HeavyCrossbow : BaseRanged** — Ranged weapon. [Constructable, Flipable]
- `JukaBow.cs` — **JukaBow : Bow** — Weapon item.
- `MagicalShortbow.cs` — **MagicalShortbow : BaseRanged** — Ranged weapon ("woodland shortbow"). [Constructable]
- `RepeatingCrossbow.cs` — **RepeatingCrossbow : BaseRanged** — Ranged weapon. [Constructable, Flipable]

#### Scripts/Items/Weapons/Hands — Weapon items: Hands. Types: Fists, PugilistGlove, PugilistMits.

- `Fists.cs` — **Fists : BaseMeleeWeapon** — Melee weapon. (+1 type)
- `PugilistGlove.cs` — **PugilistGlove : PugilistGloveWeapon** — Weapon item ("pugilist gloves").
- `PugilistMits.cs` — **PugilistMits : PugilistGloves** — Weapon item.

#### Scripts/Items/Weapons/Knives — Weapon items: Knives. Types: AssassinSpike, BaseKnife, ButcherKnife, Cleaver, ....

- `AssassinSpike.cs` — **AssassinSpike : BaseKnife** — Knife weapon ("assassin dagger"). [Constructable]
- `BaseKnife.cs` — **BaseKnife : BaseMeleeWeapon** — Melee weapon.
- `ButcherKnife.cs` — **ButcherKnife : BaseKnife** — Knife weapon. [Constructable, Flipable]
- `Cleaver.cs` — **Cleaver : BaseKnife** — Knife weapon ("cleaver"). [Constructable]
- `Dagger.cs` — **Dagger : BaseKnife** — Knife weapon ("dagger"). [Constructable]
- `LargeKnife.cs` — **LargeKnife : BaseKnife** — Knife weapon ("large knife"). [Constructable]
- `Leafblade.cs` — **Leafblade : BaseKnife** — Knife weapon ("war dagger"). [Constructable, Flipable]
- `ThrowingDagger.cs` — **ThrowingDagger : Item** — Item. [Constructable, Flipable] (+1 type)
- `WarCleaver.cs` — **WarCleaver : BaseKnife** — Knife weapon. [Constructable, Flipable]

#### Scripts/Items/Weapons/Maces — Weapon items: Maces. Types: BaseBashing, BaseWhip, Club, DiamondMace, ....

- `BaseBashing.cs` — **BaseBashing : BaseMeleeWeapon** — Melee weapon.
- `BaseWhip.cs` — **BaseWhip : BaseBashing** — Mace/hammer weapon ("whip").
- `Club.cs` — **Club : BaseBashing** — Mace/hammer weapon ("club"). [Constructable]
- `DiamondMace.cs` — **DiamondMace : BaseBashing** — Mace/hammer weapon ("battle mace"). [Constructable]
- `HammerPick.cs` — **HammerPick : BaseBashing** — Mace/hammer weapon ("hammer pick"). [Constructable]
- `Hammers.cs` — **Hammers : BaseBashing** — Mace/hammer weapon ("hammer"). [Constructable]
- `Mace.cs` — **Mace : BaseBashing** — Mace/hammer weapon ("mace"). [Constructable]
- `Maul.cs` — **Maul : BaseBashing** — Mace/hammer weapon. [Constructable, Flipable]
- `Scepter.cs` — **Scepter : BaseBashing** — Mace/hammer weapon. [Constructable, Flipable]
- `SpikedClub.cs` — **SpikedClub : BaseBashing** — Mace/hammer weapon ("spiked club"). [Constructable]
- `WarHammer.cs` — **WarHammer : BaseBashing** — Mace/hammer weapon ("war hammer"). [Constructable]
- `WarMace.cs` — **WarMace : BaseBashing** — Mace/hammer weapon ("war mace"). [Constructable]
- `Whips.cs` — **Whips : BaseWhip** — Weapon item.
- `WizardWand.cs` — **WizardWand : BaseBashing** — Mace/hammer weapon. [Constructable]

#### Scripts/Items/Weapons/Marksman — Weapon items: Marksman. Types: Harpoon, MonsterGloves, ThrowingGloves, ThrowingWeapon, ....

- `Harpoon.cs` — **Harpoon : BaseRanged** — Ranged weapon ("harpoon"). [Constructable] (+2 types)
- `MonsterGloves.cs` — **MonsterGloves : BaseRanged** — Ranged weapon ("monster gloves"). [Constructable, Flipable]
- `ThrowingGloves.cs` — **ThrowingGloves : BaseRanged** — Ranged weapon ("throwing gloves"). [Constructable, Flipable] (+2 types)
- `ThrowingWeapon.cs` — **ThrowingWeapon : Item** — Item ("throwing stone"). [Constructable] (+1 type)
- `WizardStaff.cs` — **WizardStaff : BaseWizardStaff** — Weapon item ("stave").

#### Scripts/Items/Weapons/Oriental — Weapon items: Oriental. Types: Bokuto, Daisho, Kama, Lajatang, ....

- `Bokuto.cs` — **Bokuto : BaseSword** — Sword weapon. [Constructable, Flipable]
- `Daisho.cs` — **Daisho : BaseSword** — Sword weapon. [Constructable, Flipable]
- `Kama.cs` — **Kama : BaseKnife** — Knife weapon. [Constructable, Flipable]
- `Lajatang.cs` — **Lajatang : BaseKnife** — Knife weapon. [Constructable, Flipable]
- `NoDachi.cs` — **NoDachi : BaseSword** — Sword weapon. [Constructable, Flipable]
- `Nunchaku.cs` — **Nunchaku : BaseBashing** — Mace/hammer weapon. [Constructable, Flipable]
- `Sai.cs` — **Sai : BaseKnife** — Knife weapon. [Constructable, Flipable]
- `Tekagi.cs` — **Tekagi : BaseKnife** — Knife weapon. [Constructable, Flipable]
- `Tessen.cs` — **Tessen : BaseBashing** — Mace/hammer weapon. [Constructable, Flipable]
- `Tetsubo.cs` — **Tetsubo : BaseBashing** — Mace/hammer weapon. [Constructable, Flipable]
- `Wakizashi.cs` — **Wakizashi : BaseSword** — Sword weapon. [Constructable, Flipable]
- `Yumi.cs` — **Yumi : BaseRanged** — Ranged weapon ("yumi"). [Constructable]

#### Scripts/Items/Weapons/PoleArms — Weapon items: Pole arms. Types: Bardiche, BasePoleArm, Halberd, Scythe.

- `Bardiche.cs` — **Bardiche : BasePoleArm** — Polearm weapon. [Constructable, Flipable]
- `BasePoleArm.cs` — **BasePoleArm : BaseMeleeWeapon** — When holding this in your hand, you can use it to select a tree you wish to chop. Doing this may help you g...
- `Halberd.cs` — **Halberd : BasePoleArm** — Polearm weapon ("halberd"). [Constructable]
- `Scythe.cs` — **Scythe : BasePoleArm** — Polearm weapon ("scythe"). [Constructable]

#### Scripts/Items/Weapons/SpearsAndForks — Weapon items: Spears and forks. Types: BaseSpear, BladedStaff, DoubleBladedStaff, Pike, ....

- `BaseSpear.cs` — **BaseSpear : BaseMeleeWeapon** — Melee weapon.
- `BladedStaff.cs` — **BladedStaff : BaseSpear** — Spear weapon. [Constructable, Flipable]
- `DoubleBladedStaff.cs` — **DoubleBladedStaff : BaseSpear** — Spear weapon ("double bladed staff"). [Constructable]
- `Pike.cs` — **Pike : BaseSpear** — Spear weapon. [Constructable, Flipable]
- `Pitchfork.cs` — **Pitchfork : BaseSpear** — Spear weapon ("trident"). [Constructable]
- `Pitchforks.cs` — **Pitchforks : BaseSpear** — Spear weapon ("pitchfork"). [Constructable]
- `ShortSpear.cs` — **ShortSpear : BaseSpear** — Spear weapon ("rapier"). [Constructable, Flipable]
- `Spear.cs` — **Spear : BaseSpear** — Spear weapon ("spear"). [Constructable]
- `TribalSpear.cs` — **TribalSpear : BaseSpear** — Spear weapon. [Constructable, Flipable]
- `WarFork.cs` — **WarFork : BaseSpear** — Spear weapon ("war fork"). [Constructable, Flipable]

#### Scripts/Items/Weapons/Staves — Weapon items: Staves. Types: BaseStaff, BlackStaff, GlacialStaff, GnarledStaff, ....

- `BaseStaff.cs` — **BaseStaff : BaseMeleeWeapon** — Melee weapon.
- `BlackStaff.cs` — **BlackStaff : BaseStaff** — Staff weapon ("wizard staff"). [Constructable]
- `GlacialStaff.cs` — **GlacialStaff : BlackStaff** — Weapon item ("glacial staff").
- `GnarledStaff.cs` — **GnarledStaff : BaseStaff** — Staff weapon. [Constructable, Flipable]
- `QuarterStaff.cs` — **QuarterStaff : BaseStaff** — Staff weapon. [Constructable, Flipable]
- `ShepherdsCrook.cs` — **ShepherdsCrook : BaseStaff** — Staff weapon. [Constructable, Flipable] (+2 types)
- `WildStaff.cs` — **WildStaff : BaseStaff** — Staff weapon ("druid staff"). [Constructable, Flipable]

#### Scripts/Items/Weapons/Swords — Weapon items: Swords. Types: BaseSword, BoneHarvester, Broadsword, Claymore, ....

- `BaseSword.cs` — **BaseSword : BaseMeleeWeapon** — Melee weapon.
- `BoneHarvester.cs` — **BoneHarvester : BaseSword** — Sword weapon ("sickle"). [Constructable]
- `Broadsword.cs` — **Broadsword : BaseSword** — Sword weapon ("broadsword"). [Constructable]
- `Claymore.cs` — **Claymore : BaseSword** — Sword weapon ("claymore"). [Constructable]
- `CrescentBlade.cs` — **CrescentBlade : BaseSword** — Sword weapon. [Constructable, Flipable]
- `Cutlass.cs` — **Cutlass : BaseSword** — Sword weapon ("cutlass"). [Constructable]
- `ElvenMachete.cs` — **ElvenMachete : BaseSword** — Sword weapon ("machete"). [Constructable, Flipable]
- `ElvenSpellblade.cs` — **ElvenSpellblade : BaseSword** — Sword weapon ("assassin sword"). [Constructable]
- `Katana.cs` — **Katana : BaseSword** — Sword weapon ("katana"). [Constructable]
- `Kryss.cs` — **Kryss : BaseSword** — Sword weapon. [Constructable, Flipable]
- `Lance.cs` — **Lance : BaseSword** — Sword weapon. [Constructable, Flipable]
- `Longsword.cs` — **Longsword : BaseSword** — Sword weapon ("longsword"). [Constructable]
- `RadiantScimitar.cs` — **RadiantScimitar : BaseSword** — Sword weapon ("falchion"). [Constructable, Flipable]
- `RoyalSword.cs` — **RoyalSword : VikingSword** — Weapon item ("royal sword").
- `RuneBlade.cs` — **RuneBlade : BaseSword** — Sword weapon ("war blades"). [Constructable, Flipable]
- `Scimitar.cs` — **Scimitar : BaseSword** — Sword weapon ("scimitar"). [Constructable]
- `ShortSword.cs` — **ShortSword : BaseSword** — Sword weapon ("short sword"). [Constructable]
- `ThinLongsword.cs` — **ThinLongsword : BaseSword** — Sword weapon ("sword"). [Constructable]
- `VikingSword.cs` — **VikingSword : BaseSword** — Sword weapon ("barbarian sword"). [Constructable]

#### Scripts/Mobiles/Animals/Bears — Creatures/NPCs: Bears. Types: BlackBear, BrownBear, CaveBearRiding, DireBear, ....

- `BlackBear.cs` — **BlackBear : BaseCreature** — Creature ("a black bear", AI_Animal, Aggressor). [Constructable, CorpseName, TypeAlias]
- `BrownBear.cs` — **BrownBear : BaseCreature** — Creature ("a brown bear", AI_Animal, Aggressor). [Constructable, CorpseName]
- `CaveBearRiding.cs` — **CaveBearRiding : BaseMount** — Mount. [Constructable, CorpseName]
- `DireBear.cs` — **DireBear : BaseMount** — Mount. [Constructable, CorpseName]
- `ElderBlackBearRiding.cs` — **ElderBlackBearRiding : BaseMount** — Mount. [Constructable, CorpseName]
- `ElderBrownBearRiding.cs` — **ElderBrownBearRiding : BaseMount** — Mount. [Constructable, CorpseName]
- `ElderPolarBearRiding.cs` — **ElderPolarBearRiding : BaseMount** — Mount. [Constructable, CorpseName]
- `GreatBear.cs` — **GreatBear : BaseMount** — Mount. [Constructable, CorpseName]
- `GrizzlyBearRiding.cs` — **GrizzlyBearRiding : BaseMount** — Mount. [Constructable, CorpseName]
- `KodiakBear.cs` — **KodiakBear : BaseMount** — Mount. [Constructable, CorpseName]
- `Owlbear.cs` — **Owlbear : BaseCreature** — Creature ("an owlbear", AI_Melee, Closest). [Constructable, CorpseName]
- `PackBear.cs` — **PackBear : BaseCreature** — Creature ("a pack bear", AI_Animal, Aggressor). [Constructable, CorpseName]
- `PandaRiding.cs` — **PandaRiding : BaseMount** — Mount. [Constructable, CorpseName]
- `PolarBear.cs` — **PolarBear : BaseMount** — Mount. [Constructable, CorpseName]
- `SabreclawCub.cs` — **SabreclawCub : BaseCreature** — Creature ("a sabreclaw cub", AI_Animal, Aggressor). [Constructable, CorpseName]
- `SabretoothBearRiding.cs` — **SabretoothBearRiding : BaseMount** — Mount. [Constructable, CorpseName]
- `Trollbear.cs` — **Trollbear : BaseCreature** — Creature ("a trollbear", AI_Melee, Closest). [Constructable, CorpseName]

#### Scripts/Mobiles/Animals/Birds — Creatures/NPCs: Birds. Types: AxeBeakRiding, Bird, Chicken, Crane, ....

- `AxeBeakRiding.cs` — **AxeBeakRiding : BaseMount** — Mount ("raw axe beak leg"). [Constructable, CorpseName]
- `Bird.cs` — **Bird : BaseCreature** — Creature ("a seagull", AI_Animal, Aggressor). [Constructable, CorpseName]
- `Chicken.cs` — **Chicken : BaseCreature** — Creature ("a chicken", AI_Animal, Aggressor). [Constructable, CorpseName]
- `Crane.cs` — **Crane : BaseCreature** — Creature ("a crane", AI_Animal, Aggressor). [Constructable, CorpseName]
- `DesertBird.cs` — **DesertBird : BaseCreature** — Creature ("a roadrunner", AI_Animal, Aggressor). [Constructable, CorpseName]
- `Eagle.cs` — **Eagle : BaseCreature** — Creature ("an eagle", AI_Animal, Aggressor). [Constructable, CorpseName]
- `Hawk.cs` — **Hawk : BaseCreature** — Creature ("a hawk", AI_Animal, Aggressor). [Constructable, CorpseName]
- `Ostrich.cs` — **Ostrich : BaseCreature** — Creature ("an ostrich", AI_Animal, Aggressor). [Constructable, CorpseName]
- `Penquin.cs` — **Penguin : BaseCreature** — Creature ("a penguin", AI_Animal, Aggressor). [Constructable, CorpseName]
- `SwampBird.cs` — **SwampBird : BaseCreature** — Creature ("a heron", AI_Animal, Aggressor). [Constructable, CorpseName]
- `TropicalBird.cs` — **TropicalBird : BaseCreature** — Creature ("a lorikeet", AI_Animal, Aggressor). [Constructable, CorpseName]
- `Turkey.cs` — **Turkey : BaseCreature** — Creature ("a turkey", AI_Animal, Aggressor). [Constructable, CorpseName]

#### Scripts/Mobiles/Animals/Canines — Creatures/NPCs: Canines. Types: BlackWolf, Dog, Fox, GreyWolf, ....

- `BlackWolf.cs` — **BlackWolf : BaseMount** — Mount ("a black wolf"). [Constructable, CorpseName, TypeAlias]
- `Dog.cs` — **Dog : BaseCreature** — Creature ("a dog", AI_Animal, Aggressor). [Constructable, CorpseName]
- `Fox.cs` — **Fox : BaseCreature** — Creature ("a fox", AI_Animal, Aggressor). [Constructable, CorpseName]
- `GreyWolf.cs` — **GreyWolf : BaseCreature** — Creature ("a grey wolf", AI_Animal, Aggressor). [Constructable, CorpseName, TypeAlias]
- `Hyena.cs` — **Hyena : BaseCreature** — Creature ("a hyena", AI_Animal, Aggressor). [Constructable, CorpseName, TypeAlias]
- `Jackal.cs` — **Jackal : BaseCreature** — Creature ("a jackal", AI_Animal, Aggressor). [Constructable, CorpseName, TypeAlias]
- `MadDog.cs` — **MadDog : BaseCreature** — Creature ("a mad dog", AI_Melee, Closest). [Constructable, CorpseName]
- `TimberWolf.cs` — **TimberWolf : BaseCreature** — Creature ("a timber wolf", AI_Animal, Aggressor). [Constructable, CorpseName, TypeAlias]
- `WereWolf.cs` — **WereWolf : BaseCreature** — Creature ("a werewolf", AI_Melee, Closest). [Constructable, CorpseName]
- `WhiteWolf.cs` — **WhiteWolf : BaseMount** — Mount ("a white wolf"). [Constructable, CorpseName, TypeAlias]
- `WinterWolf.cs` — **WinterWolf : BaseMount** — Mount ("a winter wolf"). [Constructable, CorpseName]
- `WolfDire.cs` — **WolfDire : BaseMount** — Mount. [Constructable, CorpseName, TypeAlias]
- `WolfMan.cs` — **WolfMan : BaseCreature** — Creature ("a wolf man", AI_Melee, Closest). [Constructable, CorpseName]
- `Worg.cs` — **Worg : BaseCreature** — Creature ("a worg", AI_Animal, Closest). [Constructable, CorpseName]

#### Scripts/Mobiles/Animals/Cows — Creatures/NPCs: Cows. Types: Bull, BullradonRiding, Cow.

- `Bull.cs` — **Bull : BaseCreature** — Creature ("a bull", AI_Animal, Aggressor). [Constructable, CorpseName]
- `BullradonRiding.cs` — **BullradonRiding : BaseMount** — Mount. [Constructable, CorpseName]
- `Cow.cs` — **Cow : BaseCreature** — Creature ("a cow", AI_Animal, Aggressor). [Constructable, CorpseName]

#### Scripts/Mobiles/Animals/Felines — Creatures/NPCs: Felines. Types: BlackCat, Bobcat, Cat, Cheetah, ....

- `BlackCat.cs` — **BlackCat : BaseCreature** — Creature ("a cat", AI_Mage, Aggressor). [Constructable, CorpseName]
- `Bobcat.cs` — **Bobcat : BaseCreature** — Creature ("a bobcat", AI_Animal, Aggressor). [Constructable, CorpseName]
- `Cat.cs` — **Cat : BaseCreature** — Creature ("a cat", AI_Animal, Aggressor). [Constructable, CorpseName, TypeAlias]
- `Cheetah.cs` — **Cheetah : BaseCreature** — Creature ("a cheetah", AI_Animal, Aggressor). [Constructable, CorpseName]
- `Cougar.cs` — **Cougar : BaseCreature** — Creature ("a cougar", AI_Animal, Aggressor). [Constructable, CorpseName]
- `CragCat.cs` — **CragCat : BaseCreature** — Creature ("a crag cat", AI_Melee, Closest). [Constructable, CorpseName]
- `HellCat.cs` — **HellCat : BaseCreature** — Creature ("a hell cat", AI_Melee, Closest). [Constructable, CorpseName, TypeAlias]
- `Jaguar.cs` — **Jaguar : BaseCreature** — Creature ("a jaguar", AI_Animal, Aggressor). [Constructable, CorpseName]
- `LionRiding.cs` — **LionRiding : BaseMount** — Mount. [Constructable, CorpseName]
- `Panther.cs` — **Panther : BaseCreature** — Creature ("a panther", AI_Animal, Aggressor). [Constructable, CorpseName]
- `PredatorHellCatRiding.cs` — **PredatorHellCatRiding : BaseMount** — Mount. [Constructable, CorpseName]
- `SabretoothCub.cs` — **SabretoothCub : BaseCreature** — Creature ("a sabretooth cub", AI_Animal, Aggressor). [Constructable, CorpseName]
- `SabretoothTigerRiding.cs` — **SabretoothTigerRiding : BaseMount** — Mount. [Constructable, CorpseName]
- `SnowLeopard.cs` — **SnowLeopard : BaseCreature** — Creature ("a snow leopard", AI_Animal, Aggressor). [Constructable, CorpseName, TypeAlias]
- `SnowLion.cs` — **SnowLion : BaseMount** — Mount. [Constructable, CorpseName]
- `TigerRiding.cs` — **TigerRiding : BaseMount** — Mount. [Constructable, CorpseName]
- `WhiteCat.cs` — **WhiteCat : BaseCreature** — Creature ("a cat", AI_Mage, Aggressor). [Constructable, CorpseName]
- `WhiteTigerRiding.cs` — **WhiteTigerRiding : BaseMount** — Mount. [Constructable, CorpseName]

#### Scripts/Mobiles/Animals/Misc — Creatures/NPCs: Misc. Types: Antelope, Ape, Boar, DireBoar, ....

- `Antelope.cs` — **Antelope : BaseCreature** — Creature ("an antelope", AI_Animal, Aggressor). [Constructable, CorpseName]
- `Ape.cs` — **Ape : BaseCreature** — Creature ("an ape", AI_Melee, Closest). [Constructable, CorpseName]
- `Boar.cs` — **Boar : BaseCreature** — Creature ("a boar", AI_Animal, Aggressor). [Constructable, CorpseName]
- `DireBoar.cs` — **DireBoar : BaseCreature** — Creature ("a dire boar", AI_Melee, Closest). [Constructable, CorpseName]
- `DireGoat.cs` — **DireGoat : BaseCreature** — Creature ("a dire goat", AI_Animal, Aggressor). [Constructable, CorpseName]
- `Dolphin.cs` — **Dolphin : BaseCreature** — Creature ("a dolphin", AI_Animal, Aggressor). [Constructable, CorpseName]
- `Elephant.cs` — **Elephant : BaseCreature** — Creature ("an elephant", AI_Animal, Aggressor). [Constructable, CorpseName]
- `Gazelle.cs` — **Gazelle : BaseCreature** — Creature ("a gazelle", AI_Animal, Aggressor). [Constructable, CorpseName]
- `Giraffe.cs` — **Giraffe : BaseCreature** — Creature ("a giraffe", AI_Animal, Aggressor). [Constructable, CorpseName]
- `Goat.cs` — **Goat : BaseCreature** — Creature ("a goat", AI_Animal, Aggressor). [Constructable, CorpseName]
- `Gorakong.cs` — **Gorakong : BaseCreature** — Creature ("a gorakong", AI_Melee, Closest). [Constructable, CorpseName]
- `Gorilla.cs` — **Gorilla : BaseCreature** — Creature ("a gorilla", AI_Animal, Aggressor). [Constructable, CorpseName]
- `GreatHart.cs` — **GreatHart : BaseCreature** — Creature ("a stag", AI_Animal, Aggressor). [Constructable, CorpseName, TypeAlias]
- `Grum.cs` — **Grum : BaseCreature** — Creature ("a grum", AI_Animal, Aggressor). [Constructable, CorpseName]
- `Hind.cs` — **Hind : BaseCreature** — Creature ("a deer", AI_Animal, Aggressor). [Constructable, CorpseName]
- `Infected.cs` — **Infected : BaseCreature** — Creature ("an infected", AI_Melee, Closest). [Constructable, CorpseName]
- `Llama.cs` — **Llama : BaseCreature** — Creature ("a llama", AI_Animal, Aggressor). [Constructable, CorpseName]
- `Mammoth.cs` — **Mammoth : Elephant** — Creature/NPC ("a mammoth").
- `Mastadon.cs` — **Mastadon : Elephant** — Creature/NPC ("a mastadon").
- `Monkey.cs` — **Monkey : BaseCreature** — Creature ("a monkey", AI_Animal, Aggressor). [Constructable, CorpseName]
- `Moose.cs` — **Moose : BaseCreature** — Creature ("a moose", AI_Animal, Aggressor). [Constructable, CorpseName]
- `MountainGoat.cs` — **MountainGoat : BaseCreature** — Creature ("a mountain goat", AI_Animal, Aggressor). [Constructable, CorpseName]
- `PackHorse.cs` — **PackHorse : BaseCreature** — Creature ("a pack horse", AI_Animal, Aggressor). [Constructable, CorpseName] (+2 types)
- `PackLlama.cs` — **PackLlama : BaseCreature** — Creature ("a pack llama", AI_Animal, Aggressor). [Constructable, CorpseName]
- `PackMule.cs` — **PackMule : BaseCreature** — Creature ("a pack mule", AI_Animal, Aggressor). [Constructable, CorpseName]
- `PackNecroHound.cs` — **PackNecroHound : BaseCreature** — Creature ("a pack hound", AI_Animal, Aggressor). [Constructable, CorpseName]
- `PackNecroSpider.cs` — **PackNecroSpider : BaseCreature** — Creature ("a pack spider", AI_Animal, Aggressor). [Constructable, CorpseName]
- `Pig.cs` — **Pig : BaseCreature** — Creature ("a pig", AI_Animal, Aggressor). [Constructable, CorpseName]
- `Ramadon.cs` — **Ramadon : BaseMount** — Mount. [Constructable, CorpseName]
- `Sheep.cs` — **Sheep : BaseCreature** — Creature ("a sheep", AI_Animal, Aggressor). [Constructable, CorpseName]
- `Tuskadon.cs` — **Tuskadon : BaseCreature** — Creature ("a tuskadon", AI_Animal, Aggressor). [Constructable, CorpseName]
- `Walrus.cs` — **Walrus : BaseCreature** — Creature ("a seal", AI_Animal, Aggressor). [Constructable, CorpseName]

#### Scripts/Mobiles/Animals/Mounts — Creatures/NPCs: Mounts. Types: DesertOstard, EtherealMount, EvilMount, ForestOstard, ....

- `DesertOstard.cs` — **DesertOstard : BaseMount** — Mount. [Constructable, CorpseName]
- `Ethereals.cs` — **EtherealMount : Item** — Item ("ethereal horse"). [Constructable] (+14 types)
- `EvilMounts.cs` — **EvilMount : BaseMount** — Mount. [Constructable, CorpseName]
- `ForestOstard.cs` — **ForestOstard : BaseMount** — Mount. [Constructable, CorpseName]
- `FrenziedOstard.cs` — **FrenziedOstard : BaseMount** — Mount. [Constructable, CorpseName]
- `GiantHawk.cs` — **GiantHawk : BaseMount** — Mount. [Constructable, CorpseName]
- `GiantRaven.cs` — **GiantRaven : BaseMount** — Mount ("a giant vulture"). [Constructable, CorpseName]
- `Horse.cs` — **Horse : BaseMount** — Mount. [Constructable, CorpseName, TypeAlias]
- `RidableLlama.cs` — **RidableLlama : BaseMount** — Mount. [Constructable, CorpseName]
- `Roc.cs` — **Roc : BaseMount** — Mount. [Constructable, CorpseName] (+1 type)
- `SeaHorse.cs` — **SeaHorse : BaseMount** — Mount. [Constructable, CorpseName]
- `SilverSteed.cs` — **SilverSteed : BaseMount** — Mount. [Constructable, CorpseName]
- `SkeletalMount.cs` — **SkeletalMount : BaseMount** — Mount ("a skeletal steed"). [Constructable, CorpseName]
- `SnowOstard.cs` — **SnowOstard : BaseMount** — Mount. [Constructable, CorpseName]
- `YoungRoc.cs` — **YoungRoc : BaseMount** — Mount. [Constructable, CorpseName] (+1 type)
- `ZebraRiding.cs` — **ZebraRiding : BaseMount** — Mount. [Constructable, CorpseName]

#### Scripts/Mobiles/Animals/Rodents — Creatures/NPCs: Rodents. Types: AlbinoBat, Bat, Critter, DiseasedRat, ....

- `AlbinoBat.cs` — **AlbinoBat : BaseCreature** — Creature ("an albino bat", AI_Animal, Aggressor). [Constructable, CorpseName]
- `Bat.cs` — **Bat : BaseCreature** — Creature ("a bat", AI_Melee, Closest). [Constructable, CorpseName]
- `Critter.cs` — **Critter : BaseCreature** — Creature ("a rat", AI_Animal, Aggressor). [Constructable, CorpseName]
- `DiseasedRat.cs` — **DiseasedRat : BaseCreature** — Creature ("a diseased rat", AI_Melee, Closest). [Constructable, CorpseName]
- `Ferret.cs` — **Ferret : BaseCreature** — Creature ("a ferret", AI_Animal, Aggressor). [Constructable, CorpseName]
- `FireBat.cs` — **FireBat : BaseCreature** — Creature ("a fire bat", AI_Mage, Closest). [Constructable, CorpseName]
- `GiantBat.cs` — **GiantBat : BaseCreature** — Creature ("a giant bat", AI_Melee, Closest). [Constructable, CorpseName]
- `GiantRat.cs` — **GiantRat : BaseCreature** — Creature ("a giant rat", AI_Melee, Closest). [Constructable, CorpseName, TypeAlias]
- `JackRabbit.cs` — **JackRabbit : BaseCreature** — Creature ("a jack rabbit", AI_Animal, Aggressor). [Constructable, CorpseName, TypeAlias]
- `Mouse.cs` — **Mouse : BaseCreature** — Creature ("a mouse", AI_Animal, Aggressor). [Constructable, CorpseName]
- `Rabbit.cs` — **Rabbit : BaseCreature** — Creature ("a rabbit", AI_Animal, Aggressor). [Constructable, CorpseName]
- `Rat.cs` — **Rat : BaseCreature** — Creature ("a rat", AI_Animal, Aggressor). [Constructable, CorpseName]
- `SewerRat.cs` — **Sewerrat : BaseCreature** — Creature ("a sewer rat", AI_Melee, Closest). [Constructable, CorpseName]
- `SicklyRat.cs` — **SicklyRat : BaseCreature** — Creature ("a sewer rat", AI_Melee, Closest). [Constructable, CorpseName]
- `Squirrel.cs` — **Squirrel : BaseCreature** — Creature ("a squirrel", AI_Animal, Aggressor). [Constructable, CorpseName]
- `Stirge.cs` — **Stirge : BaseCreature** — Creature ("a stirge", AI_Melee, Closest). [Constructable, CorpseName]
- `VampireBat.cs` — **VampireBat : BaseCreature** — Creature ("a vampire bat", AI_Melee, Closest). [Constructable, CorpseName]
- `Weasel.cs` — **Weasel : BaseCreature** — Creature ("a weasel", AI_Animal, Aggressor). [Constructable, CorpseName]
- `WhiteRabbit.cs` — **WhiteRabbit : BaseCreature** — Creature ("a rabbit", AI_Animal, Aggressor). [Constructable, CorpseName]

#### Scripts/Mobiles/Base — Base mobile classes, AI/behavior helpers, vendors, player preferences.

- `AnimalBuy.cs` — **AnimalBuyInfo : GenericBuyInfo** — Info/data container.
- `BaseCreature.cs` — **BaseCreature : Mobile** — Base creature: AI, combat, skills, loot drops and corpse handling.
- `BaseFamiliar.cs` — **BaseFamiliar : BaseCreature** — Creature (AI_Melee, Closest). (+1 type)
- `BaseGuildmaster.cs` — **BaseGuildmaster : BaseVendor** — Vendor NPC. (+2 types)
- `BaseHealer.cs` — **BaseHealer : BaseVendor** — Vendor NPC ("fighter henchman"). (+2 types)
- `BaseMount.cs` — **BaseMount : BaseCreature** — Creature. (+3 types)
- `BaseNPC.cs` — **BaseNPC : BaseCreature** — Creature (AI_Animal, Aggressor). [Constructable]
- `BasePerson.cs` — **BasePerson : BaseCreature** — Creature ("person", AI_Melee, Closest). [Constructable]
- `BaseVendor.cs` — **BaseVendor : BaseCreature** — Vendor AI: buy/sell flow, ID service, pricing and stock.
- `Behavior.cs` — **SpeedInfo** — Info/data container. (+20 types)
- `BeverageBuy.cs` — **BeverageBuyInfo : GenericBuyInfo** — Info/data container.
- `BlackMarket.cs` — **BlackMarketGump : Gump** — Gump UI.
- `DropRelic.cs` — **DropRelic** — Creature/NPC ("mask of ").
- `GenericBuy.cs` — **GenericBuyInfo : IBuyItemInfo** — Info/data container. (+1 type)
- `GenericSell.cs` — **GenericSellInfo : IShopSellInfo** — Vendor sell table: GetSellPriceFor / GetBasePriceFor and sellability.
- `Mannequin.cs` — **Mannequin : Mobile** — Mobile ("a mannequin"). (+3 types)
- `Paragon.cs` — **Paragon** — Creature/NPC.
- `PlayerBarkeeper.cs` — **PlayerBarkeeper : BaseVendor** — Vendor NPC. (+11 types)
- `PlayerMobile.cs` — **PlayerMobile : Mobile** — Player mobile: skills, movement, preferences, tooltip/quest integration.
- `PlayerPreferenceContext.cs` — **PlayerPreferenceContext** — Player preferences incl. ShowLootTiers/ShowVendorValues; serialization v12.
- `PlayerQuestContext.cs` — **PlayerQuestContext** — Context/state holder.
- `PlayerSettings.cs` — **PlayerSettings** — Settings holder.
- `PlayerVendor.cs` — **PlayerVendor : Mobile** — Mobile. (+13 types)
- `PresetMapBuy.cs` — **PresetMapBuyInfo : GenericBuyInfo** — Info/data container.
- `RentedVendor.cs` — **RentedVendor : PlayerVendor** — Creature/NPC.
- `VendorInventory.cs` — **VendorInventory** — Creature/NPC.

#### Scripts/Mobiles/Civilized — Town NPCs, vendors, guards, citizens and services.

- `Actions.cs` — **Actions : Item** — Item ("forge"). (+2 types)
- `Actor.cs` — **Actor : BaseCreature** — Creature (AI_Animal, None). [Constructable]
- `Chuckles.cs` — **ChucklesJester : BasePerson** — Creature/NPC ("Chuckles").
- `DeathKnightDemon.cs` — **DeathKnightDemon : BasePerson** — Creature/NPC ("demon wings").
- `DraculaBride.cs` — **DraculaBride : BaseCreature** — Creature ("Dolengen", AI_Animal, Aggressor). [Constructable, CorpseName]
- `Genie.cs` — **Genie : BasePerson** — Creature/NPC ("a genie").
- `GypsyLady.cs` — **GypsyLady : BasePerson** — Creature/NPC.
- `NecroGreeter.cs` — **NecroGreeter : BaseNPC** — Creature/NPC ("tattered robe").
- `ShardGreeter.cs` — **ShardGreeter : BasePerson** — Creature/NPC.
- `Sherry.cs` — **SherryTheMouse : BasePerson** — Creature/NPC ("Sherry").
- `TimeLord.cs` — **TimeLord : BasePerson** — Creature/NPC ("the Time Lord").
- `TownGuards.cs` — **TownGuards : BasePerson** — Creature/NPC ("tunic").
- `TownHerald.cs` — **TownHerald : BasePerson** — Creature/NPC.

#### Scripts/Mobiles/Civilized/Citizens — Creatures/NPCs: Citizens. Types: AdventurerEast, Citizens, DragonRider, HouseVisitor, ....

- `Adventurers.cs` — **AdventurerEast : Citizens** — Creature/NPC.
- `Citizens.cs` — **Citizens : BaseCreature** — Creature ("crate of ", AI_Citizen, None). [Constructable] (+2 types)
- `DragonRider.cs` — **DragonRider : Citizens** — Creature/NPC.
- `HouseVisitor.cs` — **HouseVisitor : Citizens** — Creature/NPC.
- `Humanoid.cs` — **Humanoid : Citizens** — Creature/NPC ("a shadow demon").
- `MeetingPets.cs` — **MeetingPets : Item** — Item ("meeting pets"). [Constructable]
- `MeetingSpots.cs` — **MeetingSpots : Item** — Item ("meeting spot"). [Constructable]
- `TavernPatron.cs` — **TavernPatronEast : Citizens** — Creature/NPC.
- `TavernPatrons.cs` — **TavernPatrons** — Creature/NPC.
- `TradesmanAlchemist.cs` — **TradesmanAlchemist : Citizens** — Creature/NPC ("cauldron").
- `TradesmanBard.cs` — **TradesmanBard : Citizens** — Creature/NPC ("instrument").
- `TradesmanButcher.cs` — **TradesmanButcher : Citizens** — Creature/NPC ("cleaver").
- `TradesmanCook.cs` — **TradesmanCook : Citizens** — Creature/NPC ("skillet").
- `TradesmanLeather.cs` — **TradesmanLeather : Citizens** — Creature/NPC ("stretched hide").
- `TradesmanLogger.cs` — **TradesmanLogger : Citizens** — Creature/NPC ("axe").
- `TradesmanLumber.cs` — **TradesmanLumber : Citizens** — Creature/NPC ("hammer").
- `TradesmanMiner.cs` — **TradesmanMiner : Citizens** — Creature/NPC ("rock").
- `TradesmanSmelter.cs` — **TradesmanSmelter : Citizens** — Creature/NPC ("forge").
- `TradesmanSmith.cs` — **TradesmanSmith : Citizens** — Creature/NPC ("hammer").
- `TrainingBow.cs` — **TrainingBow : Citizens** — Creature/NPC.
- `TrainingFishing.cs` — **TrainingFishing : Citizens** — Creature/NPC ("water hit").
- `TrainingMagery.cs` — **TrainingMagery : Citizens** — Creature/NPC ("magic wand").
- `TrainingSingle.cs` — **TrainingSingle : Citizens** — Creature/NPC.
- `TrainingSpirits.cs` — **TrainingSpirits : Citizens** — Creature/NPC.
- `Warriors.cs` — **Warriors : Citizens** — Creature/NPC.
- `WorkingSpots.cs` — **WorkingSpots : Item** — Item ("working spot"). [Constructable]

#### Scripts/Mobiles/Civilized/Comrades — Creatures/NPCs: Comrades. Types: HenchHorse, HenchmanArcher, HenchmanArcherItem, HenchmanFighter, ....

- `HenchHorse.cs` — **HenchHorse : BaseMount** — Mount. [Constructable, CorpseName]
- `HenchmanArcher.cs` — **HenchmanArcher : BaseCreature** — Creature ("henchman", AI_Archer, Closest). [Constructable, CorpseName]
- `HenchmanArcherItem.cs` — **HenchmanArcherItem** — Source file (no top-level type declaration detected).
- `HenchmanFighter.cs` — **HenchmanFighter : BaseCreature** — Creature ("henchman", AI_Melee, Closest). [Constructable, CorpseName]
- `HenchmanFighterItem.cs` — **HenchmanFighterItem** — Source file (no top-level type declaration detected).
- `HenchmanFunctions.cs` — **HenchmanFunctions** — Creature/NPC ("archer henchman").
- `HenchmanItem.cs` — **HenchmanItem** — Source file (no top-level type declaration detected).
- `HenchmanMonster.cs` — **HenchmanMonster : BaseCreature** — Creature ("henchman", AI_Melee, Closest). [Constructable, CorpseName]
- `HenchmanMonsterItem.cs` — **HenchmanMonsterItem** — Source file (no top-level type declaration detected).
- `HenchmanWizard.cs` — **HenchmanWizard : BaseCreature** — Creature ("henchman", AI_Mage, Closest). [Constructable, CorpseName]
- `HenchmanWizardItem.cs` — **HenchmanWizardItem** — Source file (no top-level type declaration detected).

#### Scripts/Mobiles/Civilized/Familiars — Creatures/NPCs: Familiars. Types: DarkWolfFamiliar, DeathAdder, HenchmanFamiliar, HenchmanFamiliarItem, ....

- `DarkWolf.cs` — **DarkWolfFamiliar : BaseFamiliar** — Creature/NPC ("a dark wolf").
- `DeathAdder.cs` — **DeathAdder : BaseFamiliar** — Creature/NPC ("a death adder").
- `Familiar.cs` — **HenchmanFamiliar : BaseCreature** — Creature ("a familiar", AI_Animal, Aggressor). [Constructable, CorpseName] (+1 type)
- `FamiliarItem.cs` — **HenchmanFamiliarItem : Item** — Item ("crystal ball of summoning"). [Constructable]
- `HoardMinionFamiliar.cs` — **HoardMinionFamiliar : HordeMinionFamiliar** — Creature/NPC ("a hoard minion").
- `HoardMinionFamiliarItem.cs` — **HoardMinionFamiliarItem : Item** — Item ("A hoard minion"). [Constructable]
- `HordeMinion.cs` — **HordeMinionFamiliar : BaseFamiliar** — Creature/NPC ("a horde minion").
- `ShadowWisp.cs` — **ShadowWispFamiliar : BaseFamiliar** — Creature/NPC ("a shadow wisp").
- `VampireBat.cs` — **VampireBatFamiliar : BaseFamiliar** — Creature/NPC ("a vampire bat").

#### Scripts/Mobiles/Civilized/Guilds — Creatures/NPCs: Guilds. Types: AlchemistGuildmaster, ArcherGuildmaster, AssassinGuildmaster, BardGuildmaster, ....

- `AlchemistGuildmaster.cs` — **AlchemistGuildmaster : BaseGuildmaster** — Creature/NPC.
- `ArcherGuildmaster.cs` — **ArcherGuildmaster : BaseGuildmaster** — Creature/NPC.
- `AssassinGuildmaster.cs` — **AssassinGuildmaster : BaseGuildmaster** — Creature/NPC.
- `BardGuildmaster.cs` — **BardGuildmaster : BaseGuildmaster** — Creature/NPC.
- `BlacksmithGuildmaster.cs` — **BlacksmithGuildmaster : BaseGuildmaster** — Creature/NPC.
- `CarpenterGuildmaster.cs` — **CarpenterGuildmaster : BaseGuildmaster** — Creature/NPC.
- `CartographersGuildmaster.cs` — **CartographersGuildmaster : BaseGuildmaster** — Creature/NPC.
- `CulinaryGuildmaster.cs` — **CulinaryGuildmaster : BaseGuildmaster** — Creature/NPC.
- `DruidGuildmaster.cs` — **DruidGuildmaster : BaseGuildmaster** — Creature/NPC.
- `ElementalGuildmaster.cs` — **ElementalGuildmaster : BaseGuildmaster** — Creature/NPC.
- `FisherGuildmaster.cs` — **FisherGuildmaster : BaseGuildmaster** — Creature/NPC.
- `HealerGuildmaster.cs` — **HealerGuildmaster : BaseGuildmaster** — Creature/NPC.
- `LibrarianGuildmaster.cs` — **LibrarianGuildmaster : BaseGuildmaster** — Creature/NPC ("diabolically coded parchment").
- `MageGuildmaster.cs` — **MageGuildmaster : BaseGuildmaster** — Creature/NPC.
- `MerchantGuildmaster.cs` — **MerchantGuildmaster : BaseGuildmaster** — Creature/NPC.
- `MinerGuildmaster.cs` — **MinerGuildmaster : BaseGuildmaster** — Creature/NPC.
- `NecromancerGuildmaster.cs` — **NecromancerGuildmaster : BaseGuildmaster** — Creature/NPC.
- `RangerGuildmaster.cs` — **RangerGuildmaster : BaseGuildmaster** — Creature/NPC.
- `TailorGuildmaster.cs` — **TailorGuildmaster : BaseGuildmaster** — Creature/NPC.
- `ThiefGuildmaster.cs` — **ThiefGuildmaster : BaseGuildmaster** — Creature/NPC.
- `TinkerGuildmaster.cs` — **TinkerGuildmaster : BaseGuildmaster** — Creature/NPC.
- `WarriorGuildmaster.cs` — **WarriorGuildmaster : BaseGuildmaster** — Creature/NPC.

#### Scripts/Mobiles/Civilized/Healers — Creatures/NPCs: Healers. Types: EvilHealer, Healer, WanderingHealer.

- `EvilHealer.cs` — **EvilHealer : BaseHealer** — Creature/NPC.
- `Healer.cs` — **Healer : BaseHealer** — Creature/NPC.
- `WanderingHealer.cs` — **WanderingHealer : BaseHealer** — Creature/NPC.

#### Scripts/Mobiles/Civilized/Merchants — Creatures/NPCs: Merchants. Types: Alchemist, AnimalTrainer, Architect, Armorer, ....

- `Alchemist.cs` — **Alchemist : BaseVendor** — Vendor NPC. [Constructable] (+3 types)
- `AnimalTrainer.cs` — **AnimalTrainer : BaseVendor** — Vendor NPC. [Constructable] (+10 types)
- `Architect.cs` — **Architect : BaseVendor** — Vendor NPC. [Constructable] (+3 types)
- `Armorer.cs` — **Armorer : BaseVendor** — Vendor NPC. [Constructable] (+3 types)
- `Baker.cs` — **Baker : BaseVendor** — Vendor NPC. [Constructable] (+3 types)
- `Banker.cs` — **Banker : BaseVendor** — Vendor NPC. [Constructable] (+3 types)
- `Bard.cs` — **Bard : BaseVendor** — Vendor NPC. [Constructable] (+3 types)
- `Barkeeper.cs` — **Barkeeper : BaseVendor** — Vendor NPC. [Constructable] (+3 types)
- `Beekeeper.cs` — **Beekeeper : BaseVendor** — Vendor NPC. [Constructable] (+3 types)
- `Blacksmith.cs` — **Blacksmith : BaseVendor** — Vendor NPC. [Constructable] (+3 types)
- `Bowyer.cs` — **Bowyer : BaseVendor** — Vendor NPC. [Constructable, TypeAlias] (+3 types)
- `Butcher.cs` — **Butcher : BaseVendor** — Vendor NPC. [Constructable] (+3 types)
- `Carpenter.cs` — **Carpenter : BaseVendor** — Vendor NPC. [Constructable] (+3 types)
- `Cobbler.cs` — **Cobbler : BaseVendor** — Vendor NPC. [Constructable] (+3 types)
- `Cook.cs` — **Cook : BaseVendor** — Vendor NPC. [Constructable] (+3 types)
- `CustomHairstylist.cs` — **CustomHairstylist : BaseVendor** — Vendor NPC. [Constructable] (+10 types)
- `Druid.cs` — **Druid : BaseVendor** — Vendor NPC. [Constructable] (+3 types)
- `DruidTree.cs` — **DruidTree : BaseVendor** — Vendor NPC. [Constructable] (+3 types)
- `DrunkenPirate.cs` — **DrunkenPirate : BaseVendor** — Vendor NPC. [Constructable] (+3 types)
- `Elementalist.cs` — **Elementalist : BaseVendor** — Vendor NPC. [Constructable] (+3 types)
- `Enchanter.cs` — **Enchanter : BaseVendor** — Vendor NPC. [Constructable] (+3 types)
- `EtherealDealer.cs` — **EtherealDealer : BaseVendor** — Vendor NPC ("Ethereal Guide"). [Constructable] (+3 types)
- `Farmer.cs` — **Farmer : BaseVendor** — Vendor NPC. [Constructable] (+3 types)
- `Fighter.cs` — **Fighter : BaseVendor** — Vendor NPC. [Constructable] (+3 types)
- `Fisherman.cs` — **Fisherman : BaseVendor** — Vendor NPC. [Constructable] (+3 types)
- `Furtrader.cs` — **Furtrader : BaseVendor** — Vendor NPC. [Constructable] (+3 types)
- `Glassblower.cs` — **Glassblower : BaseVendor** — Vendor NPC. [Constructable, TypeAlias] (+3 types)
- `HairStylist.cs` — **HairStylist : BaseVendor** — Vendor NPC. [Constructable] (+3 types)
- `Herbalist.cs` — **Herbalist : BaseVendor** — Vendor NPC. [Constructable] (+3 types)
- `InnKeeper.cs` — **InnKeeper : BaseVendor** — Vendor NPC ("candle"). [Constructable] (+4 types)
- `IronWorker.cs` — **IronWorker : BaseVendor** — Vendor NPC. [Constructable] (+3 types)
- `Jester.cs` — **Jester : BaseVendor** — Vendor NPC. [Constructable] (+5 types)
- `Jeweler.cs` — **Jeweler : BaseVendor** — Vendor NPC. [Constructable] (+3 types)
- `KeeperOfChivalry.cs` — **KeeperOfChivalry : BaseVendor** — Vendor NPC. [Constructable] (+3 types)
- `KungFu.cs` — **KungFu : BaseVendor** — Vendor NPC. [Constructable] (+3 types)
- `LeatherWorker.cs` — **LeatherWorker : BaseVendor** — Vendor NPC. [Constructable] (+3 types)
- `Lumberjack.cs` — **Lumberjack : BaseVendor** — Vendor NPC. [Constructable] (+3 types)
- `Mage.cs` — **Mage : BaseVendor** — Vendor NPC. [Constructable] (+3 types)
- `Mapmaker.cs` — **Mapmaker : BaseVendor** — Vendor NPC. [Constructable] (+5 types)
- `MementoCollector.cs` — **MementoCollector : BaseNPC** — Creature/NPC.
- `Merchant.cs` — **Merchant : BaseVendor** — Vendor NPC. [Constructable]
- `Miller.cs` — **Miller : BaseVendor** — Vendor NPC. [Constructable] (+3 types)
- `Miner.cs` — **Miner : BaseVendor** — Vendor NPC. [Constructable] (+3 types)
- `Minter.cs` — **Minter : Banker** — Creature/NPC.
- `Necromancer.cs` — **Necromancer : BaseVendor** — Vendor NPC. [Constructable] (+3 types)
- `Painter.cs` — **Painter : BaseVendor** — Vendor NPC ("painting of "). [Constructable] (+3 types)
- `Priest.cs` — **Priest : BaseHealer** — Creature/NPC.
- `Provisioner.cs` — **Provisioner : BaseVendor** — Vendor NPC. [Constructable] (+3 types)
- `Rancher.cs` — **Rancher : BaseVendor** — Vendor NPC. [Constructable] (+3 types)
- `Ranger.cs` — **Ranger : BaseVendor** — Vendor NPC. [Constructable] (+3 types)
- `RealEstateBroker.cs` — **RealEstateBroker : BaseVendor** — Vendor NPC. [Constructable] (+3 types)
- `Sage.cs` — **Sage : BaseVendor** — Vendor NPC. [Constructable] (+3 types)
- `Scribe.cs` — **Scribe : BaseVendor** — Vendor NPC. [Constructable] (+3 types)
- `Shepherd.cs` — **Shepherd : BaseVendor** — Vendor NPC. [Constructable] (+9 types)
- `Shipwright.cs` — **Shipwright : BaseVendor** — Vendor NPC. [Constructable] (+3 types)
- `StoneCrafter.cs` — **StoneCrafter : BaseVendor** — Vendor NPC. [Constructable, TypeAlias] (+3 types)
- `Tailor.cs` — **Tailor : BaseVendor** — Vendor NPC ("robe"). [Constructable] (+5 types)
- `Tanner.cs` — **Tanner : BaseVendor** — Vendor NPC. [Constructable] (+3 types)
- `TavernKeeper.cs` — **TavernKeeper : BaseVendor** — Vendor NPC. [Constructable] (+3 types)
- `Thief.cs` — **Thief : BaseVendor** — Vendor NPC. [Constructable] (+5 types)
- `Tinker.cs` — **Tinker : BaseVendor** — Vendor NPC. [Constructable] (+3 types)
- `Undertaker.cs` — **Undertaker : BaseVendor** — Vendor NPC. [Constructable] (+3 types)
- `VarietyDealer.cs` — **VarietyDealer : BaseVendor** — Vendor NPC. [Constructable] (+3 types)
- `Veterinarian.cs` — **Veterinarian : BaseVendor** — Vendor NPC. [Constructable] (+5 types)
- `Waiter.cs` — **Waiter : BaseVendor** — Vendor NPC. [Constructable] (+3 types)
- `Weaponsmith.cs` — **Weaponsmith : BaseVendor** — Vendor NPC. [Constructable] (+3 types)
- `Weaver.cs` — **Weaver : BaseVendor** — Vendor NPC. [Constructable] (+5 types)
- `Witches.cs` — **Witches : BaseVendor** — Vendor NPC. [Constructable] (+3 types)

#### Scripts/Mobiles/Civilized/Porters — Creatures/NPCs: Porters. Types: PackBeast, PackBeastItem.

- `Porter.cs` — **PackBeast : BaseCreature** — Creature ("a pack animal", AI_Animal, Aggressor). [Constructable, CorpseName]
- `PorterItem.cs` — **PackBeastItem : Item** — Item ("pack animal"). [Constructable]

#### Scripts/Mobiles/Civilized/Special — Creatures/NPCs: Special. Types: Courier, Devon, EpicCharacter, EpicPet, ....

- `Courier.cs` — **Courier : BasePerson** — Creature/NPC.
- `Devon.cs` — **Devon : BaseVendor** — Vendor NPC ("Devon"). [Constructable] (+3 types)
- `EpicCharacter.cs` — **EpicCharacter : BasePerson** — Creature/NPC ("a stranger").
- `EpicPet.cs` — **EpicPet : BasePerson** — Creature/NPC ("an enslaved demon").
- `Garth.cs` — **Garth : BaseVendor** — Vendor NPC ("Garth"). [Constructable] (+3 types)
- `GodOfCourage.cs` — **GodOfLegends : BasePerson** — Creature/NPC ("Arez").
- `Kylearan.cs` — **Kylearan : BaseNPC** — Creature/NPC ("Kylearan").
- `MadGodPriest.cs` — **MadGodPriest : BaseNPC** — Creature/NPC ("priest robe").
- `Roscoe.cs` — **Roscoe : BaseVendor** — Vendor NPC ("Roscoe"). [Constructable] (+3 types)
- `Xardok.cs` — **Xardok : BasePerson** — Creature/NPC ("Xardok").

#### Scripts/Mobiles/Civilized/Teachers — Creatures/NPCs: Teachers. Types: ExaltedDealer, LegendaryDealer, MythicalDealer, PowerDealer, ....

- `Teacher_Exalted.cs` — **ExaltedDealer : BaseVendor** — Vendor NPC. [Constructable] (+1 type)
- `Teacher_Legendary.cs` — **LegendaryDealer : BaseVendor** — Vendor NPC. [Constructable] (+1 type)
- `Teacher_Mythical.cs` — **MythicalDealer : BaseVendor** — Vendor NPC. [Constructable] (+1 type)
- `Teacher_Power.cs` — **PowerDealer : BaseVendor** — Vendor NPC. [Constructable] (+1 type)
- `Teacher_Wonderous.cs` — **WonderousDealer : BaseVendor** — Vendor NPC. [Constructable] (+1 type)

#### Scripts/Mobiles/Constructs — Creatures/NPCs: Constructs. Types: WaxSculpture.

- `WaxSculpture.cs` — **WaxSculpture : BaseCreature** — Creature ("a wax golem", AI_Melee, Closest). [Constructable, CorpseName]

#### Scripts/Mobiles/Constructs/Alien — Creatures/NPCs: Alien. Types: BattleDroid, CombatDroid, ExcavationDroid, MaintenanceDroid, ....

- `BattleDroid.cs` — **BattleDroid : BaseCreature** — Creature (AI_Melee, Closest). [Constructable, CorpseName]
- `CombatDroid.cs` — **CombatDroid : BaseCreature** — Creature (AI_Melee, Closest). [Constructable, CorpseName]
- `ExcavationDroid.cs` — **ExcavationDroid : BaseCreature** — Creature (AI_Melee, Closest). [Constructable, CorpseName]
- `MaintenanceDroid.cs` — **MaintenanceDroid : BaseCreature** — Creature (AI_Melee, Closest). [Constructable, CorpseName]
- `Mutant.cs` — **Mutant : BaseCreature** — Creature ("a mutant", AI_Melee, Closest). [Constructable, CorpseName]
- `SecurityDroid.cs` — **SecurityDroid : BaseCreature** — Creature (AI_Melee, Closest). [Constructable, CorpseName]
- `ServiceDroid.cs` — **ServiceDroid : BaseCreature** — Creature (AI_Melee, Closest). [Constructable, CorpseName]

#### Scripts/Mobiles/Constructs/Golems — Creatures/NPCs: Golems. Types: AncientFleshGolem, BoneGolem, CaddelliteGolem, FleshGolem, ....

- `AncientFleshGolem.cs` — **AncientFleshGolem : BaseCreature** — Creature ("an ancient flesh golem", AI_Melee, Closest). [Constructable, CorpseName]
- `BoneGolem.cs` — **BoneGolem : BaseCreature** — Creature ("a bone golem", AI_Melee, Closest). [Constructable, CorpseName]
- `CaddelliteGolem.cs` — **CaddelliteGolem : BaseCreature** — Creature ("a caddellite golem", AI_Melee, Closest). [Constructable, CorpseName]
- `FleshGolem.cs` — **FleshGolem : BaseCreature** — Creature ("a flesh golem", AI_Melee, Closest). [Constructable, CorpseName]
- `Golem.cs` — **Golem : BaseCreature** — Creature ("an iron golem", AI_Melee, Closest). [Constructable, CorpseName]
- `IceGolem.cs` — **IceGolem : BaseCreature** — Creature ("an ice golem", AI_Melee, Closest). [Constructable, CorpseName]
- `IronBeetle.cs` — **IronBeetle : BaseCreature** — Creature ("an iron beetle", AI_Melee, Closest). [Constructable, CorpseName]
- `IronCobra.cs` — **IronCobra : BaseCreature** — Creature ("an iron cobra", AI_Melee, Closest). [Constructable, CorpseName]
- `MechanicalScorpion.cs` — **MechanicalScorpion : BaseCreature** — Creature ("mechanical scorpion", AI_Melee, Closest). [Constructable, CorpseName]
- `MetalGolem.cs` — **MetalGolem : BaseCreature** — Creature ("a construct", AI_Melee, Closest). [Constructable, CorpseName]
- `RunicGolem.cs` — **RunicGolem : BaseCreature** — Creature ("a runic golem", AI_Melee, Closest). [Constructable, CorpseName]
- `RustGolem.cs` — **RustGolem : BaseCreature** — Creature ("a rust golem", AI_Melee, Closest). [Constructable, CorpseName]
- `SkinGolem.cs` — **SkinGolem : BaseCreature** — Creature ("a flesh golem", AI_Melee, Closest). [Constructable, CorpseName]
- `WoodenGolem.cs` — **WoodenGolem : BaseCreature** — Creature ("a wooden golem", AI_Melee, Closest). [Constructable, CorpseName]

#### Scripts/Mobiles/Constructs/Statues — Creatures/NPCs: Statues. Types: AnyStatue, LivingBronzeStatue, LivingGoldStatue, LivingIronStatue, ....

- `AnyStatue.cs` — **AnyStatue : BaseCreature** — Creature ("an animated statue", AI_Melee, Closest). [Constructable, CorpseName]
- `LivingBronzeStatue.cs` — **LivingBronzeStatue : BaseCreature** — Creature ("a bronze statue", AI_Melee, Closest). [Constructable, CorpseName]
- `LivingGoldStatue.cs` — **LivingGoldStatue : BaseCreature** — Creature ("a golden statue", AI_Melee, Closest). [Constructable, CorpseName]
- `LivingIronStatue.cs` — **LivingIronStatue : BaseCreature** — Creature ("an iron statue", AI_Melee, Closest). [Constructable, CorpseName]
- `LivingJadeStatue.cs` — **LivingJadeStatue : BaseCreature** — Creature ("a jade statue", AI_Melee, Closest). [Constructable, CorpseName]
- `LivingMarbleStatue.cs` — **LivingMarbleStatue : BaseCreature** — Creature ("a marble statue", AI_Melee, Closest). [Constructable, CorpseName]
- `LivingShadowIronStatue.cs` — **LivingShadowIronStatue : BaseCreature** — Creature ("a dark iron statue", AI_Melee, Closest). [Constructable, CorpseName]
- `LivingSilverStatue.cs` — **LivingSilverStatue : BaseCreature** — Creature ("a silver statue", AI_Melee, Closest). [Constructable, CorpseName]
- `LivingStoneStatue.cs` — **LivingStoneStatue : BaseCreature** — Creature ("a stone statue", AI_Melee, Closest). [Constructable, CorpseName]

#### Scripts/Mobiles/Demons — Creatures/NPCs: Demons. Types: AbysmalDaemon, Afreet, Archfiend, Balron, ....

- `AbysmalDaemon.cs` — **AbysmalDaemon : BaseCreature** — Creature ("staff of the abyss", AI_Mage, Closest). [Constructable, CorpseName]
- `Afreet.cs` — **Afreet : BaseCreature** — Creature ("an afreet", AI_Melee, Closest). [Constructable, CorpseName]
- `Archfiend.cs` — **Archfiend : BaseCreature** — Creature ("sword of ", AI_Mage, Closest). [Constructable, CorpseName]
- `Balron.cs` — **Balron : BaseCreature** — Creature ("sword of ", AI_Mage, Closest). [Constructable, CorpseName] (+2 types)
- `BlackGateDemon.cs` — **BlackGateDemon : BaseCreature** — Creature ("demonic chest", AI_Mage, Closest). [Constructable, CorpseName] (+1 type)
- `BloodDemigod.cs` — **BloodDemigod : BaseCreature** — Creature ("Chest of Bloody Relics", AI_Mage, Closest). [Constructable, CorpseName]
- `BloodDemon.cs` — **BloodDemon : BaseCreature** — Creature ("sword ", AI_Mage, Closest). [Constructable, CorpseName]
- `BoneDemon.cs` — **BoneDemon : BaseCreature** — Creature ("a bone demon", AI_Mage, Closest). [Constructable, CorpseName]
- `ClassicBalron.cs` — **ClassicBalron : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName]
- `ClassicDaemon.cs` — **ClassicDaemon : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName]
- `Daemon.cs` — **Daemon : BaseCreature** — Creature ("sword of ", AI_Mage, Closest). [Constructable, CorpseName]
- `DaemonTemplate.cs` — **DaemonTemplate : BaseCreature** — Creature ("a demon", AI_Mage, Closest). [Constructable, CorpseName]
- `DeepSeaDevil.cs` — **DeepSeaDevil : BaseCreature** — Creature ("manacle of the sea devil", AI_Mage, Closest). [Constructable, CorpseName]
- `Demon.cs` — **Demon : BaseCreature** — Creature ("a demon", AI_Mage, Closest). [Constructable, CorpseName]
- `DemonOfTheSea.cs` — **DemonOfTheSea : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName]
- `DemonicGhost.cs` — **DemonicGhost : BaseCreature** — Creature ("a demonic ghost", AI_Mage, Closest). [Constructable, CorpseName]
- `Devil.cs` — **Devil : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName]
- `Fiend.cs` — **Fiend : BaseCreature** — Creature ("sword of ", AI_Mage, Closest). [Constructable, CorpseName]
- `FireDemon.cs` — **FireDemon : BaseCreature** — Creature ("a fire demon", AI_Mage, Closest). [Constructable, CorpseName]
- `FireMephit.cs` — **FireMephit : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName]
- `IceDevil.cs` — **IceDevil : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName]
- `Ifreet.cs` — **Ifreet : BaseCreature** — Creature ("an ifreet", AI_Melee, Closest). [Constructable, CorpseName]
- `Imp.cs` — **Imp : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName]
- `LesserDemon.cs` — **LesserDemon : BaseCreature** — Creature ("a lesser demon", AI_Mage, Closest). [Constructable, CorpseName]
- `LowerDemon.cs` — **LowerDemon : BaseCreature** — Creature ("a lesser devil", AI_Mage, Closest). [Constructable, CorpseName]
- `Marilith.cs` — **Marilith : BaseCreature** — Creature ("scimitar of ", AI_Melee, Closest). [Constructable, CorpseName]
- `MutantDaemon.cs` — **MutantDaemon : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName]
- `Satan.cs` — **Satan : BaseCreature** — Creature ("Satan", AI_Mage, Closest). [Constructable, CorpseName]
- `ShadowDemon.cs` — **ShadowDemon : BaseCreature** — Creature ("a shadow demon", AI_Mage, Closest). [Constructable, CorpseName]
- `ShadowHound.cs` — **ShadowHound : BaseCreature** — Creature ("a shadow hound", AI_Melee, Closest). [Constructable, CorpseName]
- `Succubus.cs` — **Succubus : BaseCreature** — Creature ("a succubus", AI_Mage, Closest). [Constructable, CorpseName]
- `Xurtzar.cs` — **Xurtzar : BaseCreature** — Creature ("Xurtzar", AI_Mage, Closest). [Constructable, CorpseName] (+2 types)

#### Scripts/Mobiles/Dragons — Creatures/NPCs: Dragons. Types: DragonTurtle, Dragonogre, DrakkhenRed.

- `DragonTurtle.cs` — **DragonTurtle : BaseCreature** — Creature (AI_Melee, Closest). [Constructable, CorpseName]
- `Dragonogre.cs` — **Dragonogre : BaseCreature** — Creature ("a dragonogre", AI_Melee, Closest). [Constructable, CorpseName]
- `Drakkhen.cs` — **DrakkhenRed : BaseMount** — Mount ("a drakkhen"). [Constructable, CorpseName] (+1 type)

#### Scripts/Mobiles/Dragons/Dragons — Creatures/NPCs: Dragons. Types: AsianDragon, DragonGolem, Dragons, Dragoon, ....

- `AsianDragon.cs` — **AsianDragon : BaseCreature** — Creature (AI_Mage, Evil). [Constructable, CorpseName]
- `DragonGolem.cs` — **DragonGolem : BaseCreature** — Creature ("a dragon golem", AI_Melee, Closest). [Constructable, CorpseName]
- `Dragons.cs` — **Dragons : BaseCreature** — Creature ("rusted item", AI_Mage, Closest). [Constructable, CorpseName]
- `Dragoon.cs` — **Dragoon : BaseMount** — Mount ("a dragoon"). [Constructable, CorpseName]
- `GemDragons.cs` — **GemDragon : BaseMount** — Mount ("an elder dragyn"). [Constructable, CorpseName, TypeAlias]
- `RidingDragon.cs` — **RidingDragon : BaseMount** — Mount ("rusted item"). [Constructable, CorpseName]
- `SeaDragon.cs` — **SeaDragon : BaseCreature** — Creature ("a sea dragon", AI_Mage, Closest). [Constructable, CorpseName]

#### Scripts/Mobiles/Dragons/Drakes — Creatures/NPCs: Drakes. Types: AbysmalDrake, AncientDrake, Drake, SeaDrake, ....

- `AbysmalDrake.cs` — **AbysmalDrake : BaseCreature** — Creature ("a drake", AI_Melee, Closest). [Constructable, CorpseName]
- `AncientDrake.cs` — **AncientDrake : BaseCreature** — Creature ("an ancient drake", AI_Melee, Closest). [Constructable, CorpseName]
- `Drake.cs` — **Drake : BaseCreature** — Creature ("a drake", AI_Melee, Closest). [Constructable, CorpseName]
- `SeaDrake.cs` — **SeaDrake : BaseCreature** — Creature ("a sea drake", AI_Melee, Closest). [Constructable, CorpseName]
- `SwampDrakeRiding.cs` — **SwampDrakeRiding : BaseMount** — Mount. [Constructable, CorpseName]

#### Scripts/Mobiles/Dragons/Great Dragons — Creatures/NPCs: Great dragons. Types: AshDragon, BottleDragon, CaddelliteDragon, CrystalDragon, ....

- `AshDragon.cs` — **AshDragon : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName]
- `BottleDragon.cs` — **BottleDragon : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName]
- `CaddelliteDragon.cs` — **CaddelliteDragon : BaseCreature** — Creature ("Vordinax", AI_Mage, Closest). [Constructable, CorpseName] (+2 types)
- `CrystalDragon.cs` — **CrystalDragon : BaseCreature** — Creature ("crystal chest", AI_Mage, Closest). [Constructable, CorpseName]
- `DragonKing.cs` — **DragonKing : BaseCreature** — Creature ("Chest of Dragon King Relics", AI_Mage, Closest). [Constructable, CorpseName]
- `ElderDragon.cs` — **ElderDragon : BaseCreature** — Creature ("dragon's chest", AI_Mage, Closest). [Constructable, CorpseName]
- `RadiationDragon.cs` — **RadiationDragon : BaseCreature** — Creature ("irradiated chest", AI_Mage, Closest). [Constructable, CorpseName]
- `VoidDragon.cs` — **VoidDragon : BaseCreature** — Creature ("chest from the void", AI_Mage, Closest). [Constructable, CorpseName]

#### Scripts/Mobiles/Dragons/Hydras — Creatures/NPCs: Hydras. Types: EnergyHydra, Hydra, Wyvra.

- `EnergyHydra.cs` — **EnergyHydra : BaseCreature** — Creature ("Hydra", AI_Melee, Closest). [Constructable, CorpseName]
- `Hydra.cs` — **Hydra : BaseCreature** — Creature ("Hydra", AI_Mage, Closest). [Constructable, CorpseName]
- `Wyvra.cs` — **Wyvra : BaseCreature** — Creature ("a wyvra", AI_Mage, Closest). [Constructable, CorpseName]

#### Scripts/Mobiles/Dragons/Primeval — Creatures/NPCs: Primeval. Types: PrimevalAbysmalDragon, PrimevalAmberDragon, PrimevalBlackDragon, PrimevalDragon, ....

- `PrimevalAbysmalDragon.cs` — **PrimevalAbysmalDragon : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName]
- `PrimevalAmberDragon.cs` — **PrimevalAmberDragon : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName]
- `PrimevalBlackDragon.cs` — **PrimevalBlackDragon : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName]
- `PrimevalDragon.cs` — **PrimevalDragon : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName]
- `PrimevalFireDragon.cs` — **PrimevalFireDragon : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName]
- `PrimevalGreenDragon.cs` — **PrimevalGreenDragon : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName]
- `PrimevalNightDragon.cs` — **PrimevalNightDragon : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName]
- `PrimevalRedDragon.cs` — **PrimevalRedDragon : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName]
- `PrimevalRoyalDragon.cs` — **PrimevalRoyalDragon : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName]
- `PrimevalRunicDragon.cs` — **PrimevalRunicDragon : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName]
- `PrimevalSeaDragon.cs` — **PrimevalSeaDragon : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName]
- `PrimevalSilverDragon.cs` — **PrimevalSilverDragon : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName]
- `PrimevalStygianDragon.cs` — **PrimevalStygianDragon : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName]
- `PrimevalVolcanicDragon.cs` — **PrimevalVolcanicDragon : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName]
- `ReanimatedDragon.cs` — **ReanimatedDragon : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName]
- `VampiricDragon.cs` — **VampiricDragon : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName]

#### Scripts/Mobiles/Dragons/Wyrms — Creatures/NPCs: Wyrms. Types: AncientWyrm, ShadowWyrm, VolcanicDragon, Wyrm, ....

- `AncientWyrm.cs` — **AncientWyrm : BaseCreature** — Creature ("venom sack", AI_Mage, Closest). [Constructable, CorpseName]
- `ShadowWyrm.cs` — **ShadowWyrm : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName]
- `VolcanicDragon.cs` — **VolcanicDragon : BaseCreature** — Creature ("obsidian chest", AI_Mage, Closest). [Constructable, CorpseName]
- `Wyrm.cs` — **Wyrm : BaseCreature** — Creature ("a draguul", AI_Mage, Closest). [Constructable, CorpseName]
- `Wyrms.cs` — **Wyrms : BaseCreature** — Creature ("rusted item", AI_Mage, Closest). [Constructable, CorpseName]

#### Scripts/Mobiles/Dragons/Wyverns — Creatures/NPCs: Wyverns. Types: AncientWyvern, Wyverns.

- `AncientWyvern.cs` — **AncientWyvern : BaseCreature** — Creature (AI_Melee, Closest). [Constructable, CorpseName]
- `Wyverns.cs` — **Wyverns : BaseMount** — Mount ("deadly venom sack"). [Constructable, CorpseName]

#### Scripts/Mobiles/Dragons/Young — Creatures/NPCs: Young. Types: BabyDragon, FireWyrmling, YoungDragon.

- `BabyDragon.cs` — **BabyDragon : BaseCreature** — Creature ("a baby dragon", AI_Melee, Closest). [Constructable, CorpseName]
- `FireWyrmling.cs` — **FireWyrmling : BaseCreature** — Creature ("a fire wyrmling", AI_Melee, Closest). [Constructable, CorpseName]
- `YoungDragon.cs` — **YoungDragon : BaseCreature** — Creature ("rusted item", AI_Melee, Closest). [Constructable, CorpseName]

#### Scripts/Mobiles/Elementals — Creatures/NPCs: Elementals. Types: CrystalGoliath, Efreet, IceColossus, MudMan, ....

- `CrystalGoliath.cs` — **CrystalGoliath : BaseCreature** — Creature ("a goliath", AI_Melee, Closest). [Constructable, CorpseName]
- `Efreet.cs` — **Efreet : BaseCreature** — Creature ("an efreet", AI_Mage, Closest). [Constructable, CorpseName]
- `IceColossus.cs` — **IceColossus : BaseCreature** — Creature ("an ice colossus", AI_Melee, Closest). [Constructable, CorpseName]
- `MudMan.cs` — **MudMan : BaseCreature** — Creature ("a mud man", AI_Melee, Closest). [Constructable, CorpseName]
- `Necromental.cs` — **Necromental : BaseCreature** — Creature ("a necromental", AI_Melee, Closest). [Constructable, CorpseName] (+2 types)
- `PoisonCloud.cs` — **PoisonCloud : BaseCreature** — Creature ("a poison cloud", AI_Mage, Closest). [Constructable, CorpseName]
- `SandVortex.cs` — **SandVortex : BaseCreature** — Creature ("a sand vortex", AI_Melee, Closest). [Constructable, CorpseName] (+1 type)
- `StormCloud.cs` — **StormCloud : BaseCreature** — Creature ("a storm cloud", AI_Melee, Closest). [Constructable, CorpseName]
- `Typhoon.cs` — **Typhoon : BaseCreature** — Creature ("a typhoon", AI_Mage, Closest). [Constructable, CorpseName]
- `WaterWeird.cs` — **WaterWeird : BaseCreature** — Creature ("a water weird", AI_Mage, Closest). [Constructable, CorpseName]

#### Scripts/Mobiles/Elementals/Elementals — Creatures/NPCs: Elementals. Types: AirElemental, AnyElemental, AnyGemElemental, BloodElemental, ....

- `AirElemental.cs` — **AirElemental : BaseCreature** — Creature ("an air elemental", AI_Mage, Closest). [Constructable, CorpseName]
- `AnyElemental.cs` — **AnyElemental : BaseCreature** — Creature ("a grue", AI_Melee, Closest). [Constructable, CorpseName]
- `AnyGemElemental.cs` — **AnyGemElemental : BaseCreature** — Creature ("a grue", AI_Melee, Closest). [Constructable, CorpseName]
- `BloodElemental.cs` — **BloodElemental : BaseCreature** — Creature ("a blood elemental", AI_Mage, Closest). [Constructable, CorpseName]
- `CinderElemental.cs` — **CinderElemental : BaseCreature** — Creature ("a cinder elemental", AI_Mage, Closest). [Constructable, CorpseName]
- `DeepWaterElemental.cs` — **DeepWaterElemental : BaseCreature** — Creature ("a deep sea elemental", AI_Mage, Closest). [Constructable, CorpseName]
- `DriftwoodElemental.cs` — **DriftwoodElemental : BaseCreature** — Creature ("a driftwood elemental", AI_Melee, Closest). [Constructable, CorpseName]
- `DustElemental.cs` — **DustElemental : BaseCreature** — Creature ("a dust elemental", AI_Mage, Closest). [Constructable, CorpseName]
- `ElectricalElemental.cs` — **ElectricalElemental : BaseCreature** — Creature ("an electrical elemental", AI_Mage, Closest). [Constructable, CorpseName]
- `FireElemental.cs` — **FireElemental : BaseCreature** — Creature ("a fire elemental", AI_Mage, Closest). [Constructable, CorpseName]
- `ForestElemental.cs` — **ForestElemental : BaseCreature** — Creature ("a forest elemental", AI_Mage, Closest). [Constructable, CorpseName] (+1 type)
- `GraveDustElemental.cs` — **GraveDustElemental : BaseCreature** — Creature ("a grave dust elemental", AI_Mage, Closest). [Constructable, CorpseName]
- `IceElemental.cs` — **IceElemental : BaseCreature** — Creature ("an ice elemental", AI_Mage, Closest). [Constructable, CorpseName]
- `IcebergElemental.cs` — **IcebergElemental : BaseCreature** — Creature ("a glacial elemental", AI_Melee, Closest). [Constructable, CorpseName]
- `KelpElemental.cs` — **KelpElemental : BaseCreature** — Creature ("a kelp elemental", AI_Mage, Closest). [Constructable, CorpseName]
- `LavaElemental.cs` — **LavaElemental : BaseCreature** — Creature ("a lava elemental", AI_Mage, Closest). [Constructable, CorpseName]
- `LightningElemental.cs` — **LightningElemental : BaseCreature** — Creature ("a lightning elemental", AI_Melee, Closest). [Constructable, CorpseName] (+1 type)
- `MagmaElemental.cs` — **MagmaElemental : BaseCreature** — Creature ("a magma elemental", AI_Mage, Closest). [Constructable, CorpseName]
- `MeteorElemental.cs` — **MeteorElemental : BaseCreature** — Creature ("a meteor elemental", AI_Mage, Closest). [Constructable, CorpseName]
- `MudElemental.cs` — **MudElemental : BaseCreature** — Creature ("a mud elemental", AI_Melee, Closest). [Constructable, CorpseName]
- `PoisonElemental.cs` — **PoisonElemental : BaseCreature** — Creature ("a poison elemental", AI_Mage, Closest). [Constructable, CorpseName]
- `SeaweedElemental.cs` — **SeaweedElemental : BaseCreature** — Creature ("a seaweed elemental", AI_Melee, Closest). [Constructable, CorpseName]
- `SewageElemental.cs` — **SewageElemental : BaseCreature** — Creature ("a sewage elemental", AI_Melee, Closest). [Constructable, CorpseName]
- `SnowElemental.cs` — **SnowElemental : BaseCreature** — Creature ("a snow elemental", AI_Melee, Closest). [Constructable, CorpseName]
- `SteamElemental.cs` — **SteamElemental : BaseCreature** — Creature ("a steam elemental", AI_Melee, Closest). [Constructable, CorpseName]
- `ToxicElemental.cs` — **ToxicElemental : BaseCreature** — Creature ("an acid elemental", AI_Mage, Closest). [Constructable, CorpseName]
- `Vulcrum.cs` — **Vulcrum : BaseCreature** — Creature ("Vulcrum", AI_Mage, Closest). [Constructable, CorpseName] (+2 types)
- `WaterElemental.cs` — **WaterElemental : BaseCreature** — Creature ("a water elemental", AI_Mage, Closest). [Constructable, CorpseName]
- `WeedElemental.cs` — **WeedElemental : BaseCreature** — Creature ("a weed elemental", AI_Melee, Closest). [Constructable, CorpseName]

#### Scripts/Mobiles/Elementals/Gemmed — Creatures/NPCs: Gemmed. Types: CaddelliteElemental, CrystalElemental, DilithiumElemental, GarnetElemental, ....

- `CaddelliteElemental.cs` — **CaddelliteElemental : BaseCreature** — Creature ("a caddellite elemental", AI_Mage, Closest). [Constructable, CorpseName]
- `CrystalElemental.cs` — **CrystalElemental : BaseCreature** — Creature ("a crystal elemental", AI_Mage, Closest). [Constructable, CorpseName]
- `DilithiumElemental.cs` — **DilithiumElemental : BaseCreature** — Creature ("a dilithium elemental", AI_Mage, Closest). [Constructable, CorpseName]
- `GarnetElemental.cs` — **GarnetElemental : BaseCreature** — Creature ("a garnet elemental", AI_Mage, Closest). [Constructable, CorpseName]
- `GemElemental.cs` — **GemElemental : BaseCreature** — Creature ("an elemental mineral", AI_Mage, Closest). [Constructable, CorpseName]
- `ObsidianElemental.cs` — **ObsidianElemental : BaseCreature** — Creature ("an obsidian elemental", AI_Melee, Closest). [Constructable, CorpseName] (+1 type)
- `QuartzElemental.cs` — **QuartzElemental : BaseCreature** — Creature ("a quartz elemental", AI_Mage, Closest). [Constructable, CorpseName]
- `SapphireElemental.cs` — **SapphireElemental : BaseCreature** — Creature ("a sapphire elemental", AI_Mage, Closest). [Constructable, CorpseName]
- `SilverElemental.cs` — **SilverElemental : BaseCreature** — Creature ("a silver elemental", AI_Mage, Closest). [Constructable, CorpseName]
- `SpinelElemental.cs` — **SpinelElemental : BaseCreature** — Creature ("a spinel elemental", AI_Mage, Closest). [Constructable, CorpseName]
- `StarRubyElemental.cs` — **StarRubyElemental : BaseCreature** — Creature ("a star ruby elemental", AI_Mage, Closest). [Constructable, CorpseName]
- `TopazElemental.cs` — **TopazElemental : BaseCreature** — Creature ("a topaz elemental", AI_Mage, Closest). [Constructable, CorpseName]
- `TrilithiumElemental.cs` — **TrilithiumElemental : BaseCreature** — Creature ("a trilithium elemental", AI_Mage, Closest). [Constructable, CorpseName]
- `XormiteElemental.cs` — **XormiteElemental : BaseCreature** — Creature ("a xormite elemental", AI_Mage, Closest). [Constructable, CorpseName]

#### Scripts/Mobiles/Elementals/Lesser — Creatures/NPCs: Lesser. Types: BloodSpawn, WaterSpawn.

- `BloodSpawn.cs` — **BloodSpawn : BaseCreature** — Creature ("a blood spawn", AI_Mage, Closest). [Constructable, CorpseName]
- `WaterSpawn.cs` — **WaterSpawn : BaseCreature** — Creature ("a water spawn", AI_Mage, Closest). [Constructable, CorpseName]

#### Scripts/Mobiles/Elementals/Ore Elementals — Creatures/NPCs: Ore elementals. Types: AgapiteElemental, AnimatedRocks, BronzeElemental, CopperElemental, ....

- `AgapiteElemental.cs` — **AgapiteElemental : BaseCreature** — Creature ("an agapite elemental", AI_Melee, Closest). [Constructable, CorpseName]
- `AnimatedRocks.cs` — **AnimatedRocks : BaseCreature** — Creature ("animated rocks", AI_Melee, Closest). [Constructable, CorpseName]
- `BronzeElemental.cs` — **BronzeElemental : BaseCreature** — Creature ("a bronze elemental", AI_Melee, Closest). [Constructable, CorpseName]
- `CopperElemental.cs` — **CopperElemental : BaseCreature** — Creature ("a copper elemental", AI_Melee, Closest). [Constructable, CorpseName]
- `DullCopperElemental.cs` — **DullCopperElemental : BaseCreature** — Creature ("a dull copper elemental", AI_Melee, Closest). [Constructable, CorpseName]
- `EarthElemental.cs` — **EarthElemental : BaseCreature** — Creature ("an earth elemental", AI_Melee, Closest). [Constructable, CorpseName]
- `GoldenElemental.cs` — **GoldenElemental : BaseCreature** — Creature ("a golden elemental", AI_Melee, Closest). [Constructable, CorpseName]
- `ShadowIronElemental.cs` — **ShadowIronElemental : BaseCreature** — Creature ("a shadow iron elemental", AI_Melee, Closest). [Constructable, CorpseName]
- `StoneElemental.cs` — **StoneElemental : BaseCreature** — Creature ("a stone elemental", AI_Melee, Closest). [Constructable, CorpseName]
- `ValoriteElemental.cs` — **ValoriteElemental : BaseCreature** — Creature ("a valorite elemental", AI_Melee, Closest). [Constructable, CorpseName]
- `VeriteElemental.cs` — **VeriteElemental : BaseCreature** — Creature ("a verite elemental", AI_Melee, Closest). [Constructable, CorpseName]

#### Scripts/Mobiles/Gargoyles — Creatures/NPCs: Gargoyles. Types: AncientGargoyle, CosmicGargoyle, FireGargoyle, Gargoyle, ....

- `AncientGargoyle.cs` — **AncientGargoyle : BaseCreature** — Creature ("an ancient gargoyle", AI_Mage, Closest). [Constructable, CorpseName]
- `CosmicGargoyle.cs` — **CosmicGargoyle : BaseCreature** — Creature (AI_Melee, Closest). [Constructable, CorpseName]
- `FireGargoyle.cs` — **FireGargoyle : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName]
- `Gargoyle.cs` — **Gargoyle : BaseCreature** — Creature ("a gargoyle", AI_Mage, Closest). [Constructable, CorpseName]
- `GargoyleAmethyst.cs` — **GargoyleAmethyst : BaseCreature** — Creature ("an amethyst gargoyle", AI_Mage, Closest). [Constructable, CorpseName]
- `GargoyleEmerald.cs` — **GargoyleEmerald : BaseCreature** — Creature ("an emerald gargoyle", AI_Mage, Closest). [Constructable, CorpseName]
- `GargoyleMarble.cs` — **GargoyleMarble : BaseCreature** — Creature ("a marble gargoyle", AI_Mage, Closest). [Constructable, CorpseName]
- `GargoyleOnyx.cs` — **GargoyleOnyx : BaseCreature** — Creature ("an onyx gargoyle", AI_Mage, Closest). [Constructable, CorpseName]
- `GargoyleRuby.cs` — **GargoyleRuby : BaseCreature** — Creature ("a ruby gargoyle", AI_Mage, Closest). [Constructable, CorpseName]
- `GargoyleSapphire.cs` — **GargoyleSapphire : BaseCreature** — Creature ("a sapphire gargoyle", AI_Mage, Closest). [Constructable, CorpseName]
- `GargoyleWarrior.cs` — **GargoyleWarrior : BaseCreature** — Creature ("a gargoyle guard", AI_Mage, Closest). [Constructable, CorpseName]
- `MutantGargoyle.cs` — **MutantGargoyle : BaseCreature** — Creature ("a mutant gargoyle", AI_Melee, Closest). [Constructable, CorpseName]
- `StoneGargoyle.cs` — **StoneGargoyle : BaseCreature** — Creature ("a stone gargoyle", AI_Melee, Closest). [Constructable, CorpseName]
- `StygianGargoyle.cs` — **StygianGargoyle : BaseCreature** — Creature ("a gargoyle", AI_Mage, Closest). [Constructable, CorpseName]
- `StygianGargoyleLord.cs` — **StygianGargoyleLord : BaseCreature** — Creature ("a gargoyle lord", AI_Mage, Closest). [Constructable, CorpseName]

#### Scripts/Mobiles/Goliaths — Creatures/NPCs: Goliaths. Types: OrkDemigod, ZornTheBlacksmith.

- `OrkDemigod.cs` — **OrkDemigod : BaseCreature** — Creature ("orkish war chest", AI_Mage, Closest). [Constructable, CorpseName]
- `ZornTheBlacksmith.cs` — **ZornTheBlacksmith : BaseCreature** — Creature ("Zorn", AI_Mage, Closest). [Constructable, CorpseName]

#### Scripts/Mobiles/Goliaths/Cyclops — Creatures/NPCs: Cyclops. Types: AncientCyclops, Cyclops, ShamanicCyclops.

- `AncientCyclops.cs` — **AncientCyclops : BaseCreature** — Creature ("cyclopean chest", AI_Melee, Closest). [Constructable, CorpseName]
- `Cyclops.cs` — **Cyclops : BaseCreature** — Creature ("cyclopean hammer", AI_Melee, Closest). [Constructable, CorpseName]
- `ShamanicCyclops.cs` — **ShamanicCyclops : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName]

#### Scripts/Mobiles/Goliaths/Ettins — Creatures/NPCs: Ettins. Types: AncientEttin, ArcticEttin, Ettin, EttinShaman.

- `AncientEttin.cs` — **AncientEttin : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName]
- `ArcticEttin.cs` — **ArcticEttin : BaseCreature** — Creature ("an arctic ettin", AI_Melee, Closest). [Constructable, CorpseName]
- `Ettin.cs` — **Ettin : BaseCreature** — Creature ("an ettin", AI_Melee, Closest). [Constructable, CorpseName]
- `EttinShaman.cs` — **EttinShaman : BaseCreature** — Creature ("an ettin shaman", AI_Mage, Closest). [Constructable, CorpseName]

#### Scripts/Mobiles/Goliaths/Giants — Creatures/NPCs: Giants. Types: AbyssGiant, CloudGiant, DeepSeaGiant, FireGiant, ....

- `AbyssGiant.cs` — **AbyssGiant : BaseCreature** — Creature ("abysmal chest", AI_Mage, Closest). [Constructable, CorpseName]
- `CloudGiant.cs` — **CloudGiant : BaseCreature** — Creature ("cloud giant chest", AI_Mage, Closest). [Constructable, CorpseName]
- `DeepSeaGiant.cs` — **DeepSeaGiant : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName]
- `FireGiant.cs` — **FireGiant : BaseCreature** — Creature ("fire giant sack", AI_Melee, Closest). [Constructable, CorpseName]
- `ForestGiant.cs` — **ForestGiant : BaseCreature** — Creature ("forest giant sack", AI_Mage, Closest). [Constructable, CorpseName]
- `FrostGiant.cs` — **FrostGiant : BaseCreature** — Creature ("frost giant sack", AI_Melee, Closest). [Constructable, CorpseName]
- `Giant.cs` — **Giant : BaseCreature** — Creature ("giant sack", AI_Melee, Closest). [Constructable, CorpseName]
- `HillGiant.cs` — **HillGiant : BaseCreature** — Creature ("hill giant sack", AI_Melee, Closest). [Constructable, CorpseName]
- `HillGiantShaman.cs` — **HillGiantShaman : BaseCreature** — Creature ("hill giant sack", AI_Mage, Closest). [Constructable, CorpseName]
- `IceGiant.cs` — **IceGiant : BaseCreature** — Creature ("ice chest", AI_Mage, Closest). [Constructable, CorpseName]
- `JungleGiant.cs` — **JungleGiant : BaseCreature** — Creature ("jade chest", AI_Mage, Closest). [Constructable, CorpseName]
- `LavaGiant.cs` — **LavaGiant : BaseCreature** — Creature ("molten chest", AI_Mage, Closest). [Constructable, CorpseName]
- `MountainGiant.cs` — **MountainGiant : BaseCreature** — Creature ("mountain giant sack", AI_Melee, Closest). [Constructable, CorpseName]
- `SandGiant.cs` — **SandGiant : BaseCreature** — Creature ("sapphire chest", AI_Mage, Closest). [Constructable, CorpseName]
- `SeaGiant.cs` — **SeaGiant : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName]
- `StarGiant.cs` — **StarGiant : BaseCreature** — Creature ("star giant chest", AI_Mage, Closest). [Constructable, CorpseName]
- `StoneGiant.cs` — **StoneGiant : BaseCreature** — Creature ("stone giant sack", AI_Melee, Closest). [Constructable, CorpseName]
- `StormGiant.cs` — **StormGiant : BaseCreature** — Creature ("onyx chest", AI_Mage, Closest). [Constructable, CorpseName]

#### Scripts/Mobiles/Goliaths/Ogres — Creatures/NPCs: Ogres. Types: AbysmalOgre, ArcticOgreLord, Ogre, OgreLord, ....

- `AbysmalOgre.cs` — **AbysmalOgre : BaseCreature** — Creature ("an abysmal ogre", AI_Melee, Closest). [Constructable, CorpseName]
- `ArcticOgreLord.cs` — **ArcticOgreLord : BaseCreature** — Creature ("an arctic ogre lord", AI_Melee, Closest). [Constructable, CorpseName, TypeAlias]
- `Ogre.cs` — **Ogre : BaseCreature** — Creature ("an ogre", AI_Melee, Closest). [Constructable, CorpseName]
- `OgreLord.cs` — **OgreLord : BaseCreature** — Creature ("an ogre lord", AI_Melee, Closest). [Constructable, CorpseName]
- `OgreMagi.cs` — **OgreMagi : BaseCreature** — Creature ("an ogre magi", AI_Mage, Closest). [Constructable, CorpseName]
- `TundraOgre.cs` — **TundraOgre : BaseCreature** — Creature ("an arctic ogre", AI_Melee, Closest). [Constructable, CorpseName]

#### Scripts/Mobiles/Goliaths/Titans — Creatures/NPCs: Titans. Types: ElderTitan, Titan.

- `ElderTitan.cs` — **ElderTitan : BaseCreature** — Creature ("ancient titan chest", AI_Mage, Closest). [Constructable, CorpseName]
- `Titan.cs` — **Titan : BaseCreature** — Creature ("titan chest", AI_Mage, Closest). [Constructable, CorpseName]

#### Scripts/Mobiles/Goliaths/Trolls — Creatures/NPCs: Trolls. Types: FrostTroll, FrostTrollShaman, SeaTroll, SwampTroll, ....

- `FrostTroll.cs` — **FrostTroll : BaseCreature** — Creature ("a frost troll", AI_Melee, Closest). [Constructable, CorpseName]
- `FrostTrollShaman.cs` — **FrostTrollShaman : BaseCreature** — Creature ("a frost troll shaman", AI_Mage, Closest). [Constructable, CorpseName]
- `SeaTroll.cs` — **SeaTroll : BaseCreature** — Creature ("a sea troll", AI_Melee, Closest). [Constructable, CorpseName]
- `SwampTroll.cs` — **SwampTroll : BaseCreature** — Creature ("a swamp troll", AI_Melee, Closest). [Constructable, CorpseName]
- `Troll.cs` — **Troll : BaseCreature** — Creature ("a troll", AI_Melee, Closest). [Constructable, CorpseName]
- `TrollWitchDoctor.cs` — **TrollWitchDoctor : BaseCreature** — Creature ("a troll witch doctor", AI_Mage, Closest). [Constructable, CorpseName]

#### Scripts/Mobiles/Hellish — Creatures/NPCs: Hellish. Types: AncientNightmareRiding, Cerberus, Chimera, DarkHound, ....

- `AncientNightmareRiding.cs` — **AncientNightmareRiding : BaseMount** — Mount. [Constructable, CorpseName]
- `Cerberus.cs` — **Cerberus : BaseCreature** — Creature ("a cerberus", AI_Melee, Closest). [Constructable, CorpseName]
- `Chimera.cs` — **Chimera : BaseCreature** — Creature ("a chimera", AI_Melee, Closest). [Constructable, CorpseName]
- `DarkHound.cs` — **DarkHound : BaseCreature** — Creature ("a dark hound", AI_Melee, Closest). [Constructable, CorpseName]
- `DemonDog.cs` — **DemonDog : BaseMount** — Mount. [Constructable, CorpseName]
- `FireSteed.cs` — **FireSteed : BaseMount** — Mount. [Constructable, CorpseName]
- `HellBeast.cs` — **HellBeast : BaseCreature** — Creature ("a hell beast", AI_Melee, Closest). [Constructable, CorpseName]
- `HellHound.cs` — **HellHound : BaseCreature** — Creature ("a hell hound", AI_Melee, Closest). [Constructable, CorpseName]
- `HordeMinion.cs` — **HordeMinion : BaseCreature** — Creature ("a horde minion", AI_Melee, Closest). [Constructable, CorpseName]
- `ManticoreRiding.cs` — **ManticoreRiding : BaseMount** — Mount. [Constructable, CorpseName]
- `Nightmare.cs` — **Nightmare : BaseMount** — Mount. [Constructable, CorpseName]
- `ShadowFiend.cs` — **ShadowFiend : BaseCreature** — Creature ("a shadow fiend", AI_Melee, Closest). [Constructable] (+1 type)
- `Vrock.cs` — **Vrock : BaseCreature** — Creature ("a vrock", AI_Melee, Closest). [Constructable, CorpseName]

#### Scripts/Mobiles/Humanoids — Creatures/NPCs: Humanoids. Types: Bugbear, Drider, DriderWizard, Durgar, ....

- `Bugbear.cs` — **Bugbear : BaseCreature** — Creature ("a bugbear", AI_Melee, Closest). [Constructable, CorpseName]
- `Drider.cs` — **Drider : BaseCreature** — Creature ("a drider", AI_Melee, Closest). [Constructable, CorpseName]
- `DriderWizard.cs` — **DriderWizard : BaseCreature** — Creature ("a drider wizard", AI_Mage, Closest). [Constructable, CorpseName]
- `Durgar.cs` — **Durgar : BaseCreature** — Creature ("a durgar", AI_Melee, Closest). [Constructable, CorpseName]
- `Gnoll.cs` — **Gnoll : BaseCreature** — Creature ("a gnoll", AI_Melee, Closest). [Constructable, CorpseName]
- `Goblin.cs` — **Goblin : BaseCreature** — Creature ("a goblin", AI_Melee, Closest). [Constructable, CorpseName]
- `GoblinArcher.cs` — **GoblinArcher : BaseCreature** — Creature ("a goblin archer", AI_Archer, Closest). [Constructable, CorpseName]
- `Hobgoblin.cs` — **Hobgoblin : BaseCreature** — Creature ("a hobgoblin", AI_Melee, Closest). [Constructable, CorpseName]
- `Jackalwitch.cs` — **Jackalwitch : BaseCreature** — Creature ("a jackalwitch", AI_Mage, Closest). [Constructable, CorpseName]
- `Medusa.cs` — **Medusa : BaseCreature** — Creature ("a medusa", AI_Melee, Closest). [Constructable, CorpseName]
- `MindFlayer.cs` — **MindFlayer : BaseCreature** — Creature ("a mind flayer", AI_Mage, Closest). [Constructable, CorpseName]
- `Mongbat.cs` — **Mongbat : BaseCreature** — Creature ("a mongbat", AI_Melee, Closest). [Constructable, CorpseName]
- `Morlock.cs` — **Morlock : BaseCreature** — Creature ("a morlock", AI_Melee, Closest). [Constructable, CorpseName]
- `Neanderthal.cs` — **Neanderthal : BaseCreature** — Creature ("a neanderthal", AI_Melee, Closest). [Constructable, CorpseName]
- `Yeti.cs` — **Yeti : BaseCreature** — Creature ("a yeti", AI_Melee, Closest). [Constructable, CorpseName]

#### Scripts/Mobiles/Humanoids/Aliens — Creatures/NPCs: Aliens. Types: BombWorshipper, Jedi, Psionicist, SavageAlien, ....

- `BombWorshipper.cs` — **BombWorshipper : BaseCreature** — Creature ("hull plate", AI_Melee, Closest). [Constructable]
- `Jedi.cs` — **Jedi : BaseCreature** — Creature ("Jedi robe", AI_Mage, Evil). [Constructable]
- `Psionicist.cs` — **Psionicist : BaseCreature** — Creature (AI_Mage, Closest). [Constructable]
- `SavageAlien.cs` — **SavageAlien : BaseCreature** — Creature ("an alien savage", AI_Melee, Closest). [Constructable, CorpseName]
- `Syth.cs` — **Syth : BaseCreature** — Creature ("Syth robe", AI_Mage, Closest). [Constructable]

#### Scripts/Mobiles/Humanoids/Ambroz — Creatures/NPCs: Ambroz. Types: AbrozChieftain, AbrozShaman, AbrozWarrior.

- `AbrozChieftain.cs` — **AbrozChieftain : BaseCreature** — Creature ("an ambroz chieftain", AI_Melee, Closest). [Constructable, CorpseName]
- `AbrozShaman.cs` — **AbrozShaman : BaseCreature** — Creature ("an ambroz shaman", AI_Mage, Closest). [Constructable, CorpseName]
- `AbrozWarrior.cs` — **AbrozWarrior : BaseCreature** — Creature ("an ambroz warrior", AI_Melee, Closest). [Constructable, CorpseName]

#### Scripts/Mobiles/Humanoids/Drakkul — Creatures/NPCs: Drakkul. Types: Drakkul, DrakkulChief, DrakkulMage.

- `Drakkul.cs` — **Drakkul : BaseCreature** — Creature (AI_Melee, Closest). [Constructable, CorpseName]
- `DrakkulChief.cs` — **DrakkulChief : BaseCreature** — Creature (AI_Melee, Closest). [Constructable, CorpseName]
- `DrakkulMage.cs` — **DrakkulMage : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName]

#### Scripts/Mobiles/Humanoids/Elves — Creatures/NPCs: Elves. Types: ElfBerserker, ElfMage, ElfMinstrel, ElfMonks, ....

- `ElfBerserker.cs` — **ElfBerserker : BaseCreature** — Creature (AI_Melee, Closest). [Constructable, CorpseName]
- `ElfMage.cs` — **ElfMage : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName]
- `ElfMinstrel.cs` — **ElfMinstrel : BaseCreature** — Creature (AI_Archer, Closest). [Constructable]
- `ElfMonks.cs` — **ElfMonks : BaseCreature** — Creature (AI_Melee, Closest). [Constructable]
- `ElfRogue.cs` — **ElfRogue : BaseCreature** — Creature (AI_Melee, Closest). [Constructable, CorpseName]

#### Scripts/Mobiles/Humanoids/Gnome — Creatures/NPCs: Gnome. Types: Gnome, GnomeMage, GnomeWarrior.

- `Gnome.cs` — **Gnome : BaseCreature** — Creature ("a gnome", AI_Melee, Closest). [Constructable, CorpseName]
- `GnomeMage.cs` — **GnomeMage : BaseCreature** — Creature ("a gnome magician", AI_Mage, Closest). [Constructable, CorpseName]
- `GnomeWarrior.cs` — **GnomeWarrior : BaseCreature** — Creature ("a gnome warrior", AI_Melee, Closest). [Constructable, CorpseName]

#### Scripts/Mobiles/Humanoids/Harpies — Creatures/NPCs: Harpies. Types: Harpy, HarpyElder, HarpyHen, SnowHarpy, ....

- `Harpy.cs` — **Harpy : BaseCreature** — Creature ("a harpy", AI_Melee, Closest). [Constructable, CorpseName]
- `HarpyElder.cs` — **HarpyElder : BaseCreature** — Creature ("an elder harpy", AI_Mage, Closest). [Constructable, CorpseName]
- `HarpyHen.cs` — **HarpyHen : BaseCreature** — Creature ("a harpy hen", AI_Mage, Closest). [Constructable, CorpseName]
- `SnowHarpy.cs` — **SnowHarpy : BaseCreature** — Creature ("a snow harpy", AI_Melee, Closest). [Constructable, CorpseName]
- `StoneHarpy.cs` — **StoneHarpy : BaseCreature** — Creature ("a stone harpy", AI_Melee, Closest). [Constructable, CorpseName]

#### Scripts/Mobiles/Humanoids/Humans — Creatures/NPCs: Humans. Types: Adventurers, Archmage, Bandit, Berserker, ....

- `Adventurers.cs` — **Adventurers : BaseCreature** — Creature (AI_Melee, Evil). [Constructable, CorpseName]
- `Archmage.cs` — **Archmage : BaseCreature** — Creature ("staff", AI_Mage, Closest). [Constructable, CorpseName]
- `Bandit.cs` — **Bandit : BaseCreature** — Creature (AI_Melee, Closest). [Constructable]
- `Berserker.cs` — **Berserker : BaseCreature** — Creature (AI_Melee, Closest). [Constructable]
- `BlackKnight.cs` — **BlackKnight : BaseCreature** — Creature ("the Black Knights's box", AI_Melee, Closest). [Constructable] (+2 types)
- `BloodAssassin.cs` — **BloodAssassin : BaseCreature** — Creature ("a disciple of blood", AI_Melee, Closest). [Constructable, CorpseName]
- `Brigand.cs` — **Brigand : BaseCreature** — Creature (AI_Melee, Closest). [Constructable]
- `EvilMage.cs` — **EvilMage : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName]
- `EvilMageLord.cs` — **EvilMageLord : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName]
- `Executioner.cs` — **Executioner : BaseCreature** — Creature ("executioner's hood", AI_Melee, Closest). [Constructable]
- `GolemController.cs` — **GolemController : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName]
- `Minstrel.cs` — **Minstrel : BaseCreature** — Creature (AI_Archer, Closest). [Constructable]
- `Monks.cs` — **Monks : BaseCreature** — Creature (AI_Melee, Closest). [Constructable]
- `Rogue.cs` — **Rogue : BaseCreature** — Creature (AI_Melee, Closest). [Constructable]

#### Scripts/Mobiles/Humanoids/Kilrathi — Creatures/NPCs: Kilrathi. Types: Kilrathi, KilrathiGunner.

- `Kilrathi.cs` — **Kilrathi : BaseCreature** — Creature ("a kilrathi", AI_Melee, Closest). [Constructable, CorpseName]
- `KilrathiGunner.cs` — **KilrathiGunner : BaseCreature** — Creature ("a kilrathi", AI_Archer, Closest). [Constructable, CorpseName]

#### Scripts/Mobiles/Humanoids/Lizardmen — Creatures/NPCs: Lizardmen. Types: Grathek, Kobold, KoboldMage, Lizardman, ....

- `Grathek.cs` — **Grathek : BaseCreature** — Creature ("grathek battle axe", AI_Melee, Closest). [Constructable, CorpseName]
- `Kobold.cs` — **Kobold : BaseCreature** — Creature ("a kobold", AI_Mage, Closest). [Constructable, CorpseName]
- `KoboldMage.cs` — **KoboldMage : BaseCreature** — Creature ("a kobold shaman", AI_Mage, Closest). [Constructable, CorpseName]
- `Lizardman.cs` — **Lizardman : BaseCreature** — Creature (AI_Melee, Closest). [Constructable, CorpseName]
- `LizardmanArcher.cs` — **LizardmanArcher : BaseCreature** — Creature (AI_Archer, Closest). [Constructable, CorpseName]
- `MutantLizardman.cs` — **MutantLizardman : BaseCreature** — Creature (AI_Melee, Closest). [Constructable, CorpseName]
- `Reptalar.cs` — **Reptalar : BaseCreature** — Creature (AI_Melee, Closest). [Constructable, CorpseName]
- `ReptalarChieftain.cs` — **ReptalarChieftain : BaseCreature** — Creature (AI_Melee, Closest). [Constructable, CorpseName]
- `ReptalarShaman.cs` — **ReptalarShaman : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName]
- `Reptaur.cs` — **Reptaur : BaseCreature** — Creature (AI_Melee, Aggressor). [Constructable, CorpseName]
- `Sakleth.cs` — **Sakleth : BaseCreature** — Creature ("sakkhra spear", AI_Melee, Closest). [Constructable, CorpseName]
- `SaklethArcher.cs` — **SaklethArcher : BaseCreature** — Creature (AI_Archer, Closest). [Constructable, CorpseName]
- `SaklethMage.cs` — **SaklethMage : BaseCreature** — Creature ("sakkhra staff", AI_Mage, Closest). [Constructable, CorpseName]
- `Sleestax.cs` — **Sleestax : BaseCreature** — Creature ("sleestax trident", AI_Melee, Closest). [Constructable, CorpseName]

#### Scripts/Mobiles/Humanoids/Minotaurs — Creatures/NPCs: Minotaurs. Types: Minotaur, MinotaurCaptain, MinotaurScout, MinotaurSmall, ....

- `Minotaur.cs` — **Minotaur : BaseCreature** — Creature ("a minotaur", AI_Melee, Closest). [Constructable, CorpseName]
- `MinotaurCaptain.cs` — **MinotaurCaptain : BaseCreature** — Creature ("a minotaur lord", AI_Melee, Closest). [Constructable, CorpseName]
- `MinotaurScout.cs` — **MinotaurScout : BaseCreature** — Creature ("a minotaur warrior", AI_Melee, Closest). [Constructable, CorpseName]
- `MinotaurSmall.cs` — **MinotaurSmall : BaseCreature** — Creature ("a minotaur", AI_Melee, Closest). [Constructable, CorpseName]
- `MutantMinotaur.cs` — **MutantMinotaur : BaseCreature** — Creature ("a mutant minotaur", AI_Melee, Closest). [Constructable, CorpseName]

#### Scripts/Mobiles/Humanoids/Orcs — Creatures/NPCs: Orcs. Types: Orc, OrcBomber, OrcCaptain, OrcishLord, ....

- `Orc.cs` — **Orc : BaseCreature** — Creature (AI_Melee, Closest). [Constructable, CorpseName]
- `OrcBomber.cs` — **OrcBomber : BaseCreature** — Creature (AI_Melee, Closest). [Constructable, CorpseName] (+1 type)
- `OrcCaptain.cs` — **OrcCaptain : BaseCreature** — Creature (AI_Melee, Closest). [Constructable, CorpseName]
- `OrcishLord.cs` — **OrcishLord : BaseCreature** — Creature (AI_Melee, Closest). [Constructable, CorpseName]
- `OrcishMage.cs` — **OrcishMage : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName]
- `OrkMage.cs` — **OrkMage : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName]
- `OrkMonks.cs` — **OrkMonks : BaseCreature** — Creature (AI_Melee, Closest). [Constructable, CorpseName]
- `OrkRogue.cs` — **OrkRogue : BaseCreature** — Creature (AI_Melee, Closest). [Constructable, CorpseName]
- `OrkWarrior.cs` — **OrkWarrior : BaseCreature** — Creature (AI_Melee, Closest). [Constructable, CorpseName]
- `Orx.cs` — **Orx : BaseCreature** — Creature (AI_Melee, Closest). [Constructable, CorpseName]
- `OrxWarrior.cs` — **OrxWarrior : BaseCreature** — Creature (AI_Melee, Closest). [Constructable, CorpseName]
- `Urc.cs` — **Urc : BaseCreature** — Creature (AI_Melee, Closest). [Constructable, CorpseName]
- `UrcBowman.cs` — **UrcBowman : BaseCreature** — Creature (AI_Archer, Closest). [Constructable, CorpseName]
- `UrcShaman.cs` — **UrcShaman : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName]
- `Urk.cs` — **Urk : BaseCreature** — Creature ("urkish face", AI_Melee, Closest). [Constructable, CorpseName]
- `UrkShaman.cs` — **UrkShaman : BaseCreature** — Creature ("urkish face", AI_Mage, Closest). [Constructable, CorpseName]

#### Scripts/Mobiles/Humanoids/Ratmen — Creatures/NPCs: Ratmen. Types: Ratman, RatmanArcher, RatmanMage.

- `Ratman.cs` — **Ratman : BaseCreature** — Creature (AI_Melee, Closest). [Constructable, CorpseName]
- `RatmanArcher.cs` — **RatmanArcher : BaseCreature** — Creature (AI_Archer, Closest). [Constructable, CorpseName]
- `RatmanMage.cs` — **RatmanMage : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName]

#### Scripts/Mobiles/Humanoids/Sailors — Creatures/NPCs: Sailors. Types: BaseSailor, BoatPirateArcher, BoatSailorArcher.

- `BaseSailor.cs` — **BaseSailor : BaseCreature** — Creature (AI_Melee, Closest). [Constructable]
- `BoatPirates.cs` — **BoatPirateArcher : BaseSailor** — Creature/NPC.
- `BoatSailors.cs` — **BoatSailorArcher : BaseSailor** — Creature/NPC.

#### Scripts/Mobiles/Humanoids/Sailors/Galleons — Creatures/NPCs: Galleons. Types: BasePirate, PirateCult, PirateCyclops, PirateDaemon, ....

- `BasePirate.cs` — **BasePirate : BaseCreature** — Creature ("Native Tunic", AI_Melee, Closest). [Constructable]
- `PirateCult.cs` — **PirateCult : BasePirate** — Creature/NPC.
- `PirateCyclops.cs` — **PirateCyclops : BasePirate** — Creature/NPC.
- `PirateDaemon.cs` — **PirateDaemon : BasePirate** — Creature/NPC.
- `PirateDarkLord.cs` — **PirateDarkLord : BasePirate** — Creature/NPC.
- `PirateDemon.cs` — **PirateDemon : BasePirate** — Creature/NPC.
- `PirateDemoness.cs` — **PirateDemoness : BasePirate** — Creature/NPC.
- `PirateDevil.cs` — **PirateDevil : BasePirate** — Creature/NPC.
- `PirateDragonogre.cs` — **PirateDragonogre : BasePirate** — Creature/NPC.
- `PirateDrow.cs` — **PirateDrow : BasePirate** — Creature/NPC.
- `PirateEttinMage.cs` — **PirateEttinMage : BasePirate** — Creature/NPC.
- `PirateGargoyle.cs` — **PirateGargoyle : BasePirate** — Creature/NPC.
- `PirateGhost.cs` — **PirateGhost : BasePirate** — Creature/NPC.
- `PirateGrathek.cs` — **PirateGrathek : BasePirate** — Creature/NPC.
- `PirateLich.cs` — **PirateLich : BasePirate** — Creature/NPC.
- `PirateLichLord.cs` — **PirateLichLord : BasePirate** — Creature/NPC.
- `PirateMen.cs` — **PirateMen : BasePirate** — Creature/NPC.
- `PirateMinotaur.cs` — **PirateMinotaur : BasePirate** — Creature/NPC.
- `PirateNatives.cs` — **PirateNatives : BasePirate** — Creature/NPC ("Native Tunic").
- `PirateOgreLord.cs` — **PirateOgreLord : BasePirate** — Creature/NPC.
- `PirateOphidian.cs` — **PirateOphidian : BasePirate** — Creature/NPC.
- `PirateSakleth.cs` — **PirateSakleth : BasePirate** — Creature/NPC.
- `PirateSkeleton.cs` — **PirateSkeleton : BasePirate** — Creature/NPC.
- `PirateSnakeMan.cs` — **PirateSnakeMan : BasePirate** — Creature/NPC.
- `PirateSuccubus.cs` — **PirateSuccubus : BasePirate** — Creature/NPC.
- `PirateTitan.cs` — **PirateTitan : BasePirate** — Creature/NPC.
- `PirateTroll.cs` — **PirateTroll : BasePirate** — Creature/NPC.
- `PirateUndead.cs` — **PirateUndead : BasePirate** — Creature/NPC.
- `SailorAngel.cs` — **SailorAngel : BasePirate** — Creature/NPC.
- `SailorAngelLord.cs` — **SailorAngelLord : BasePirate** — Creature/NPC.
- `SailorElf.cs` — **SailorElf : BasePirate** — Creature/NPC.
- `SailorElfGuards.cs` — **SailorElfGuards : BasePirate** — Creature/NPC.
- `SailorGuards.cs` — **SailorGuards : BasePirate** — Creature/NPC.
- `SailorMerchant.cs` — **SailorMerchant : BasePirate** — Creature/NPC.
- `SailorOrkGuards.cs` — **SailorOrkGuards : BasePirate** — Creature/NPC.

#### Scripts/Mobiles/Humanoids/Sailors/Pirates — Creatures/NPCs: Pirates. Types: ElfPirateCaptain, ElfPirateCrew, ElfPirateCrewBow, ElfPirateCrewMage, ....

- `ElfPirateCaptain.cs` — **ElfPirateCaptain : BaseCreature** — Creature (AI_Melee, Closest). [Constructable, CorpseName]
- `ElfPirateCrew.cs` — **ElfPirateCrew : BaseCreature** — Creature (AI_Melee, Closest). [Constructable, CorpseName]
- `ElfPirateCrewBow.cs` — **ElfPirateCrewBow : BaseCreature** — Creature (AI_Melee, Closest). [Constructable, CorpseName]
- `ElfPirateCrewMage.cs` — **ElfPirateCrewMage : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName]
- `GalleonAddon.cs` — **GalleonAddon : BaseAddon** — House addon ("Necromancer Ship"). (+1 type)
- `PirateCaptain.cs` — **PirateCaptain : BaseCreature** — Creature (AI_Melee, Closest). [Constructable, CorpseName]
- `PirateCrew.cs` — **PirateCrew : BaseCreature** — Creature (AI_Melee, Closest). [Constructable, CorpseName]
- `PirateCrewBow.cs` — **PirateCrewBow : BaseCreature** — Creature (AI_Melee, Closest). [Constructable, CorpseName]
- `PirateCrewMage.cs` — **PirateCrewMage : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName]
- `PirateLand.cs` — **PirateLand : BaseCreature** — Creature (AI_Melee, Closest). [Constructable]
- `PirateShip.cs` — **PirateShipAddon : BaseAddon** — House addon ("Pirate Ship"). (+1 type)

#### Scripts/Mobiles/Humanoids/Savages — Creatures/NPCs: Savages. Types: Native, NativeArcher, NativeWitchDoctor, Savage, ....

- `Native.cs` — **Native : BaseCreature** — Creature ("a tribesman", AI_Melee, Closest). [Constructable, CorpseName]
- `NativeArcher.cs` — **NativeArcher : BaseCreature** — Creature ("a tribesman", AI_Archer, Closest). [Constructable, CorpseName]
- `NativeWitchDoctor.cs` — **NativeWitchDoctor : BaseCreature** — Creature ("a tribesman witch doctor", AI_Mage, Closest). [Constructable, CorpseName]
- `Savage.cs` — **Savage : BaseCreature** — Creature ("dracosaur tunic", AI_Melee, Closest). [Constructable, CorpseName]
- `SavageLord.cs` — **SavageLord : BaseCreature** — Creature ("chieftain pouch", AI_Mage, Closest). [Constructable, CorpseName]
- `SavageRider.cs` — **SavageRider : BaseCreature** — Creature ("dracosaur tunic", AI_Melee, Closest). [Constructable, CorpseName]
- `SavageShaman.cs` — **SavageShaman : BaseCreature** — Creature ("dracosaur tunic", AI_Mage, Closest). [Constructable, CorpseName]
- `ZuluuArcher.cs` — **ZuluuArcher : BaseCreature** — Creature ("a zuluu", AI_Archer, Closest). [Constructable, CorpseName]
- `ZuluuNative.cs` — **ZuluuNative : BaseCreature** — Creature ("a zuluu", AI_Melee, Closest). [Constructable, CorpseName]
- `ZuluuWitchDoctor.cs` — **ZuluuWitchDoctor : BaseCreature** — Creature ("a zuluu witch doctor", AI_Mage, Closest). [Constructable, CorpseName]

#### Scripts/Mobiles/Humanoids/Sea — Creatures/NPCs: Sea. Types: Dagon, Krakoa, Lobstran, Locathah, ....

- `Dagon.cs` — **Dagon : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName]
- `Krakoa.cs` — **Krakoa : BaseCreature** — Creature ("a krakoa", AI_Mage, Closest). [Constructable, CorpseName]
- `Lobstran.cs` — **Lobstran : BaseCreature** — Creature ("a lobstran", AI_Melee, Closest). [Constructable, CorpseName]
- `Locathah.cs` — **Locathah : BaseCreature** — Creature ("a locathah", AI_Melee, Closest). [Constructable, CorpseName]
- `Neptar.cs` — **Neptar : BaseCreature** — Creature ("a neptar", AI_Melee, Closest). [Constructable, CorpseName]
- `NeptarWizard.cs` — **NeptarWizard : BaseCreature** — Creature ("a neptar wizard", AI_Mage, Closest). [Constructable, CorpseName]
- `SeaHag.cs` — **SeaHag : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName]
- `SeaHagGreater.cs` — **SeaHagGreater : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName]
- `Tritun.cs` — **Tritun : BaseCreature** — Creature ("a tritun", AI_Melee, Closest). [Constructable, CorpseName]
- `TritunMage.cs` — **TritunMage : BaseCreature** — Creature ("a tritun wizard", AI_Mage, Closest). [Constructable, CorpseName]

#### Scripts/Mobiles/Humanoids/Serpents — Creatures/NPCs: Serpents. Types: FireNaga, FireSalamander, Naga, OphidianArchmage, ....

- `FireNaga.cs` — **FireNaga : BaseCreature** — Creature ("a fire naga", AI_Mage, Closest). [Constructable, CorpseName]
- `FireSlamander.cs` — **FireSalamander : BaseCreature** — Creature ("a fire slamander", AI_Mage, Closest). [Constructable, CorpseName]
- `Naga.cs` — **Naga : BaseCreature** — Creature ("a naga", AI_Mage, Closest). [Constructable, CorpseName]
- `OphidianArchmage.cs` — **OphidianArchmage : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName, TypeAlias]
- `OphidianKnight.cs` — **OphidianKnight : BaseCreature** — Creature ("lethal venom sack", AI_Melee, Closest). [Constructable, CorpseName, TypeAlias]
- `OphidianMage.cs` — **OphidianMage : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName, TypeAlias]
- `OphidianMatriarch.cs` — **OphidianMatriarch : BaseCreature** — Creature ("an ophidian matriarch", AI_Mage, Closest). [Constructable, CorpseName]
- `OphidianWarrior.cs` — **OphidianWarrior : BaseCreature** — Creature (AI_Melee, Closest). [Constructable, CorpseName]
- `SandSerpyn.cs` — **SandSerpyn : BaseCreature** — Creature ("a serpyn", AI_Melee, Closest). [Constructable, CorpseName]
- `Serpentar.cs` — **Serpentar : BaseCreature** — Creature ("a serpentar", AI_Melee, Closest). [Constructable, CorpseName]
- `SerpentarWizard.cs` — **SerpentarWizard : BaseCreature** — Creature ("a serpentar wizard", AI_Mage, Closest). [Constructable, CorpseName]
- `Serpyn.cs` — **Serpyn : BaseCreature** — Creature ("a serpyn", AI_Melee, Closest). [Constructable, CorpseName]
- `SerpynChampion.cs` — **SerpynChampion : BaseCreature** — Creature ("a serpyn champion", AI_Melee, Closest). [Constructable, CorpseName]
- `SerpynSorceress.cs` — **SerpynSorceress : BaseCreature** — Creature ("a serpyn sorceress", AI_Mage, Closest). [Constructable, CorpseName]
- `WaterNaga.cs` — **WaterNaga : BaseCreature** — Creature ("a water naga", AI_Mage, Closest). [Constructable, CorpseName]

#### Scripts/Mobiles/Humanoids/Terathan — Creatures/NPCs: Terathan. Types: TerathanAvenger, TerathanDrone, TerathanMatriarch, TerathanWarrior.

- `TerathanAvenger.cs` — **TerathanAvenger : BaseCreature** — Creature ("a terathan avenger", AI_Mage, Closest). [Constructable, CorpseName]
- `TerathanDrone.cs` — **TerathanDrone : BaseCreature** — Creature ("a terathan drone", AI_Melee, Closest). [Constructable, CorpseName]
- `TerathanMatriarch.cs` — **TerathanMatriarch : BaseCreature** — Creature ("a terathan matriarch", AI_Mage, Closest). [Constructable, CorpseName]
- `TerathanWarrior.cs` — **TerathanWarrior : BaseCreature** — Creature ("a terathan warrior", AI_Melee, Closest). [Constructable, CorpseName]

#### Scripts/Mobiles/Insects — Creatures/NPCs: Insects. Types: Anhkheg, DeadlyScorpion, Lavapede, Mantis, ....

- `Anhkheg.cs` — **Anhkheg : BaseCreature** — Creature ("an anhkheg", AI_Melee, Closest). [Constructable, CorpseName]
- `DeadlyScorpion.cs` — **DeadlyScorpion : BaseCreature** — Creature ("a deadly scorpion", AI_Melee, Closest). [Constructable, CorpseName]
- `Lavapede.cs` — **Lavapede : BaseCreature** — Creature ("a lavapede", AI_Melee, Closest). [Constructable, CorpseName]
- `Mantis.cs` — **Mantis : BaseCreature** — Creature ("a mantis", AI_Melee, Closest). [Constructable, CorpseName, TypeAlias]
- `Scorpion.cs` — **Scorpion : BaseCreature** — Creature ("a scorpion", AI_Melee, Closest). [Constructable, CorpseName]

#### Scripts/Mobiles/Insects/Aliens — Creatures/NPCs: Aliens. Types: Alien, AlienSmall, AlienSpider, Shaclaw.

- `Alien.cs` — **Alien : BaseCreature** — Creature ("an alien", AI_Melee, Closest). [Constructable, CorpseName]
- `AlienSmall.cs` — **AlienSmall : BaseCreature** — Creature ("an alien hatchling", AI_Melee, Closest). [Constructable, CorpseName]
- `AlienSpider.cs` — **AlienSpider : BaseCreature** — Creature ("an alien spider", AI_Melee, Closest). [Constructable, CorpseName]
- `Shaclaw.cs` — **Shaclaw : BaseCreature** — Creature ("a shaclaw", AI_Melee, Closest). [Constructable, CorpseName]

#### Scripts/Mobiles/Insects/Ants — Creatures/NPCs: Ants. Types: AntLion, AntaurKing, AntaurProgenitor, AntaurSoldier, ....

- `AntLion.cs` — **AntLion : BaseCreature** — Creature ("an ant lion", AI_Melee, Closest). [Constructable, CorpseName]
- `AntaurKing.cs` — **AntaurKing : BaseCreature** — Creature ("Xthizx", AI_Mage, Closest). [Constructable, CorpseName]
- `AntaurProgenitor.cs` — **AntaurProgenitor : BaseCreature** — Creature ("an antaur lord", AI_Mage, Closest). [Constructable, CorpseName]
- `AntaurSoldier.cs` — **AntaurSoldier : BaseCreature** — Creature ("an antaur soldier", AI_Melee, Closest). [Constructable, CorpseName]
- `AntaurWorker.cs` — **AntaurWorker : BaseCreature** — Creature ("an antaur worker", AI_Melee, Closest). [Constructable, CorpseName]

#### Scripts/Mobiles/Insects/Beetles — Creatures/NPCs: Beetles. Types: Beetle, DeathwatchBeetle, DeathwatchBeetleHatchling, DuneBeetle, ....

- `Beetle.cs` — **Beetle : BaseMount** — Mount. [Constructable, CorpseName]
- `DeathWatchBeetle.cs` — **DeathwatchBeetle : BaseCreature** — Creature ("a skitter", AI_Melee, Closest). [Constructable, CorpseName, TypeAlias]
- `DeathWatchBeetleHatchling.cs` — **DeathwatchBeetleHatchling : BaseCreature** — Creature ("a skitterling", AI_Melee, Closest). [Constructable, CorpseName, TypeAlias]
- `DuneBeetle.cs` — **DuneBeetle : BaseCreature** — Creature ("a dune beetle", AI_Melee, Closest). [Constructable, CorpseName]
- `FireBeetle.cs` — **FireBeetle : BaseMount** — Mount. [Constructable, CorpseName]
- `GlowBeetleRiding.cs` — **GlowBeetleRiding : BaseMount** — Mount. [Constructable, CorpseName]
- `MetalBeetle.cs` — **MetalBeetle : BaseCreature** — Creature ("a metallic beetle", AI_Melee, Closest). [Constructable, CorpseName]
- `PoisonBeetleRiding.cs` — **PoisonBeetleRiding : BaseMount** — Mount ("lethal venom sack"). [Constructable, CorpseName]
- `RuneBeetle.cs` — **RuneBeetle : BaseCreature** — Creature ("a rune beetle", AI_Mage, Closest). [Constructable, CorpseName] (+1 type)
- `Skellot.cs` — **Skellot : BaseCreature** — Creature ("a skellot", AI_Animal, Aggressor). [Constructable, CorpseName]
- `TigerBeetleRiding.cs` — **TigerBeetleRiding : BaseMount** — Mount. [Constructable, CorpseName]
- `WaterBeetleRiding.cs` — **WaterBeetleRiding : BaseMount** — Mount. [Constructable, CorpseName]

#### Scripts/Mobiles/Insects/Spiders — Creatures/NPCs: Spiders. Types: AbyssCrawler, Arachnar, CaveFisher, DreadSpider, ....

- `AbyssCrawler.cs` — **AbyssCrawler : BaseCreature** — Creature ("a kith", AI_Melee, Closest). [Constructable, CorpseName]
- `Arachnar.cs` — **Arachnar : BaseCreature** — Creature ("Arachnar", AI_Melee, Closest). [Constructable, CorpseName] (+2 types)
- `CaveFisher.cs` — **CaveFisher : BaseCreature** — Creature ("a cave fisher", AI_Melee, Closest). [Constructable, CorpseName]
- `DreadSpider.cs` — **DreadSpider : BaseCreature** — Creature ("a dread spider", AI_Mage, Closest). [Constructable, CorpseName]
- `FrostSpider.cs` — **FrostSpider : BaseCreature** — Creature ("a frost spider", AI_Melee, Closest). [Constructable, CorpseName]
- `GiantBlackWidow.cs` — **GiantBlackWidow : BaseCreature** — Creature ("a giant black widow", AI_Melee, Closest). [Constructable, CorpseName]
- `GiantSpider.cs` — **GiantSpider : BaseCreature** — Creature ("a giant spider", AI_Melee, Closest). [Constructable, CorpseName]
- `LargeSpider.cs` — **LargeSpider : BaseCreature** — Creature ("a huge spider", AI_Melee, Closest). [Constructable, CorpseName]
- `MonstrousSpider.cs` — **MonstrousSpider : BaseCreature** — Creature ("a monstrous spider", AI_Melee, Closest). [Constructable, CorpseName]
- `PhaseSpider.cs` — **PhaseSpider : BaseCreature** — Creature ("a phase spider", AI_Melee, Closest). [Constructable, CorpseName]
- `SandSpider.cs` — **SandSpider : BaseCreature** — Creature ("a sand spider", AI_Melee, Closest). [Constructable, CorpseName]
- `ShadowRecluse.cs` — **ShadowRecluse : BaseCreature** — Creature ("a shadow recluse", AI_Mage, Closest). [Constructable, CorpseName]
- `Tarantula.cs` — **Tarantula : BaseCreature** — Creature ("a tarantula", AI_Melee, Closest). [Constructable, CorpseName]
- `WaterStrider.cs` — **WaterStrider : BaseCreature** — Creature ("a water strider", AI_Melee, Closest). [Constructable, CorpseName]
- `ZombieSpider.cs` — **ZombieSpider : BaseCreature** — Creature ("a decaying spider", AI_Melee, Closest). [Constructable, CorpseName]

#### Scripts/Mobiles/Mystical — Creatures/NPCs: Mystical. Types: AncientSphinx, Angel, Archangel, Centaur, ....

- `AncientSphinx.cs` — **AncientSphinx : BaseCreature** — Creature ("an ancient sphinx", AI_Mage, Closest). [Constructable, CorpseName]
- `Angel.cs` — **Angel : BaseCreature** — Creature ("an angel", AI_Mage, Evil). [Constructable, CorpseName]
- `Archangel.cs` — **Archangel : BaseCreature** — Creature ("an archangel", AI_Mage, Evil). [Constructable, CorpseName]
- `Centaur.cs` — **Centaur : BaseCreature** — Creature (AI_Melee, Evil). [Constructable, CorpseName]
- `CorruptCentaur.cs` — **CorruptCentaur : BaseCreature** — Creature ("a sentaur", AI_Melee, Closest). [Constructable, CorpseName]
- `CuSidhe.cs` — **CuSidhe : BaseMount** — Mount. [Constructable, CorpseName]
- `DarkUnicornRiding.cs` — **DarkUnicornRiding : BaseMount** — Mount. [Constructable, CorpseName]
- `DarkWisp.cs` — **DarkWisp : BaseCreature** — Creature ("a wisp", AI_Mage, Closest). [Constructable, CorpseName]
- `Dreadhorn.cs` — **Dreadhorn : BaseMount** — Mount. [Constructable, CorpseName]
- `EtherealWarrior.cs` — **EtherealWarrior : BaseCreature** — Creature (AI_Mage, Evil). [Constructable, CorpseName]
- `Faerie.cs` — **Faerie : BaseCreature** — Creature (AI_Mage, Evil). [Constructable, CorpseName]
- `GriffonRiding.cs` — **GriffonRiding : BaseMount** — Mount. [Constructable, CorpseName]
- `GuardianWolf.cs` — **GuardianWolf : BaseCreature** — Creature ("a guardian wolf", AI_Melee, Closest). [Constructable, CorpseName] (+1 type)
- `HippogriffRiding.cs` — **HippogriffRiding : BaseMount** — Mount. [Constructable, CorpseName]
- `IceSteed.cs` — **IceSteed : BaseMount** — Mount. [Constructable, CorpseName]
- `Kirin.cs` — **Kirin : BaseMount** — Mount. [Constructable, CorpseName]
- `MLDryad.cs` — **MLDryad : BaseCreature** — Creature ("a dryad", AI_Mage, Evil). [Constructable, CorpseName]
- `MysticalFox.cs` — **MysticalFox : BaseCreature** — Creature ("a mystical fox", AI_Mage, Closest). [Constructable, CorpseName] (+1 type)
- `Pegasus.cs` — **Pegasus : BaseCreature** — Creature ("a pegasus", AI_Mage, Evil). [Constructable, CorpseName]
- `Phoenix.cs` — **Phoenix : BaseMount** — Mount. [Constructable, CorpseName]
- `Pixie.cs` — **Pixie : BaseCreature** — Creature (AI_Mage, Evil). [Constructable, CorpseName]
- `Placeron.cs` — **Placeron : BaseCreature** — Creature ("a placeron", AI_Mage, Closest). [Constructable, CorpseName]
- `Reptalon.cs` — **Reptalon : BaseMount** — Mount. [Constructable, CorpseName]
- `RoyalSphinx.cs` — **RoyalSphinx : BaseCreature** — Creature ("a royal sphinx", AI_Mage, Closest). [Constructable, CorpseName]
- `Satyr.cs` — **Satyr : BaseCreature** — Creature ("a satyr", AI_Animal, Evil). [Constructable, CorpseName]
- `ShadowWisp.cs` — **ShadowWisp : BaseCreature** — Creature ("a shadow wisp", AI_Mage, Closest). [Constructable, CorpseName]
- `SphinxRiding.cs` — **SphinxRiding : BaseMount** — Mount. [Constructable, CorpseName]
- `Sprite.cs` — **Sprite : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName]
- `Sunlyte.cs` — **Sunlyte : BaseCreature** — Creature ("a sunlyte", AI_Mage, Closest). [Constructable, CorpseName]
- `Unicorn.cs` — **Unicorn : BaseMount** — Mount. [Constructable, CorpseName]
- `Wisp.cs` — **Wisp : BaseCreature** — Creature ("a wisp", AI_Mage, Aggressor). [Constructable, CorpseName]
- `Xatyr.cs` — **Xatyr : BaseCreature** — Creature ("a xatyr", AI_Melee, Closest). [Constructable, CorpseName]
- `xDryad.cs` — **xDryad : BaseCreature** — Creature ("a dryad", AI_Mage, Evil). [Constructable, CorpseName]

#### Scripts/Mobiles/Omni AI — OmniAI behaviour framework used by custom creatures.

- `AITester.cs` — **AITester : BaseCreature** — Creature ("Dark Knight", AI_Melee, Closest). [Constructable, CorpseName]
- `OmniAI Bard.cs` — **OmniAI : BaseAI** — Creature/NPC.
- `OmniAI Bushido.cs` — **OmniAI : BaseAI** — Creature/NPC.
- `OmniAI Chivalry.cs` — **OmniAI : BaseAI** — Creature/NPC.
- `OmniAI Core.cs` — **OmniAI : BaseAI** — Creature/NPC.
- `OmniAI Magery.cs` — **OmniAI : BaseAI** — Creature/NPC.
- `OmniAI Necromancy.cs` — **OmniAI : BaseAI** — Creature/NPC.
- `OmniAI Ninjitsu.cs` — **OmniAI : BaseAI** — Creature/NPC.
- `OmniAI Shared.cs` — **OmniAI : BaseAI** — Creature/NPC.

#### Scripts/Mobiles/Plants — Creatures/NPCs: Plants. Types: AncientEnt, AncientReaper, BloodLotus, BogThing, ....

- `AncientEnt.cs` — **AncientEnt : BaseCreature** — Creature (AI_Mage, Evil). [Constructable, CorpseName]
- `AncientReaper.cs` — **AncientReaper : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName]
- `BloodLotus.cs` — **BloodLotus : BaseCreature** — Creature ("a blood lotus", AI_Melee, Closest). [Constructable, CorpseName]
- `BogThing.cs` — **BogThing : BaseCreature** — Creature ("a shambling mound", AI_Melee, Closest). [Constructable, CorpseName]
- `Bogling.cs` — **Bogling : BaseCreature** — Creature ("a shambler", AI_Melee, Closest). [Constructable, CorpseName]
- `Corpser.cs` — **Corpser : BaseCreature** — Creature ("a corpser", AI_Melee, Closest). [Constructable, CorpseName]
- `CreepingFungus.cs` — **CreepingFungus : BaseCreature** — Creature ("a creeping fungus", AI_Melee, Closest). [Constructable, CorpseName]
- `DeadReaper.cs` — **DeadReaper : BaseCreature** — Creature ("a dead wood tree", AI_Mage, Closest). [Constructable, CorpseName]
- `Ent.cs` — **Ent : BaseCreature** — Creature ("a treant", AI_Mage, Evil). [Constructable, CorpseName]
- `EvilEnt.cs` — **EvilEnt : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName]
- `Fungal.cs` — **Fungal : BaseCreature** — Creature ("a fungal", AI_Melee, Closest). [Constructable, CorpseName]
- `FungalMage.cs` — **FungalMage : BaseCreature** — Creature ("a fungal mage", AI_Mage, Closest). [Constructable, CorpseName]
- `Quagmire.cs` — **Quagmire : BaseCreature** — Creature ("a quagmire", AI_Melee, Closest). [Constructable, CorpseName]
- `Reaper.cs` — **Reaper : BaseCreature** — Creature ("a reaper", AI_Mage, Closest). [Constructable, CorpseName]
- `SeaWeeder.cs` — **SeaWeeder : BaseCreature** — Creature ("a seaweeder", AI_Melee, Closest). [Constructable, CorpseName]
- `SwampTentacle.cs` — **SwampTentacle : BaseCreature** — Creature ("a swamp tentacle", AI_Melee, Closest). [Constructable, CorpseName]
- `SwampThing.cs` — **SwampThing : BaseCreature** — Creature ("a swamp thing", AI_Melee, Closest). [Constructable, CorpseName]
- `TheAncientTree.cs` — **TheAncientTree : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName]
- `WalkingReaper.cs` — **WalkingReaper : BaseCreature** — Creature ("a reaper", AI_Mage, Closest). [Constructable, CorpseName]
- `WhippingVine.cs` — **WhippingVine : BaseCreature** — Creature ("a whipping vine", AI_Melee, Closest). [Constructable, CorpseName]
- `WoodlandDevil.cs` — **WoodlandDevil : BaseCreature** — Creature ("a woodland devil", AI_Mage, Closest). [Constructable, CorpseName]

#### Scripts/Mobiles/Races — Creatures/NPCs: Races. Types: BaseRace, NPCRace, RacePotions.

- `BaseRace.cs` — **BaseRace : Item** — Item. [Constructable]
- `NPCRace.cs` — **NPCRace : Item** — Item. [Constructable]
- `RacePotions.cs` — **RacePotions : Item** — Item ("gypsy potion shelf"). [Constructable] (+1 type)

#### Scripts/Mobiles/Reptilian — Creatures/NPCs: Reptilian. Types: Alligator, BasiliskRiding, CaveLizard, Crocodile, ....

- `Alligator.cs` — **Alligator : BaseCreature** — Creature ("an alligator", AI_Melee, Closest). [Constructable, CorpseName]
- `BasiliskRiding.cs` — **BasiliskRiding : BaseMount** — Mount ("useless stone"). [Constructable, CorpseName]
- `CaveLizard.cs` — **CaveLizard : BaseCreature** — Creature ("a cave lizard", AI_Melee, Closest). [Constructable, CorpseName]
- `Crocodile.cs` — **Crocodile : BaseCreature** — Creature ("a crocodile", AI_Melee, Closest). [Constructable, CorpseName]
- `GiantLizard.cs` — **GiantLizard : BaseCreature** — Creature ("a giant lizard", AI_Melee, Closest). [Constructable, CorpseName]
- `HugeLizard.cs` — **HugeLizard : BaseCreature** — Creature ("a huge lizard", AI_Melee, Closest). [Constructable, CorpseName, TypeAlias]
- `Iguana.cs` — **Iguana : BaseCreature** — Creature ("an iguana", AI_Melee, Closest). [Constructable, CorpseName]
- `LavaLizard.cs` — **LavaLizard : BaseCreature** — Creature ("a lava lizard", AI_Melee, Closest). [Constructable, CorpseName, TypeAlias]
- `PackTurtle.cs` — **PackTurtle : BaseCreature** — Creature ("a pack turtle", AI_Animal, Aggressor). [Constructable, CorpseName]
- `Ridgeback.cs` — **Ridgeback : BaseMount** — Mount. [Constructable, CorpseName]
- `SandSquid.cs` — **SandSquid : BaseCreature** — Creature ("a sand squid", AI_Melee, Closest). [Constructable, CorpseName]
- `SavageRidgeback.cs` — **SavageRidgeback : BaseMount** — Mount. [Constructable, CorpseName]
- `Serpentaur.cs` — **Serpentaur : BaseCreature** — Creature ("a serpentaur", AI_Melee, Closest). [Constructable, CorpseName]
- `Snapper.cs` — **Snapper : BaseCreature** — Creature ("a snapper", AI_Melee, Closest). [Constructable, CorpseName]
- `Stalker.cs` — **Stalker : BaseCreature** — Creature ("a stalker", AI_Melee, Closest). [Constructable, CorpseName]
- `Styguana.cs` — **Styguana : BaseCreature** — Creature ("a styguana", AI_Animal, Aggressor). [Constructable, CorpseName]
- `SwampDragon.cs` — **SwampDragon : BaseMount** — Mount. [Constructable, CorpseName]
- `SwampGator.cs` — **SwampGator : BaseCreature** — Creature ("a swamp gator", AI_Melee, Aggressor). [Constructable, CorpseName]
- `Toraxen.cs` — **Toraxen : BaseCreature** — Creature ("a toraxen", AI_Animal, Aggressor). [Constructable, CorpseName]
- `Tortuga.cs` — **Tortuga : BaseCreature** — Creature ("a tortuga", AI_Melee, Closest). [Constructable, CorpseName] (+1 type)
- `Turtle.cs` — **Turtle : BaseCreature** — Creature ("a turtle", AI_Animal, Aggressor). [Constructable, CorpseName]

#### Scripts/Mobiles/Reptilian/Dinosaurs — Creatures/NPCs: Dinosaurs. Types: Basilosaurus, Brontosaur, GorceratopsRiding, Iguanodon, ....

- `Basilosaurus.cs` — **Basilosaurus : BaseCreature** — Creature ("a basilosaurus", AI_Melee, Closest). [Constructable, CorpseName]
- `Brontosaur.cs` — **Brontosaur : BaseCreature** — Creature ("a brontosaur", AI_Animal, Aggressor). [Constructable, CorpseName]
- `GorceratopsRiding.cs` — **GorceratopsRiding : BaseMount** — Mount. [Constructable, CorpseName]
- `Iguanodon.cs` — **Iguanodon : BaseMount** — Mount. [Constructable, CorpseName]
- `Meglasaur.cs` — **Meglasaur : BaseCreature** — Creature ("a meglasaur", AI_Melee, Closest). [Constructable, CorpseName]
- `PackStegosaurus.cs` — **PackStegosaurus : BaseCreature** — Creature ("a pack stegosaurus", AI_Animal, Aggressor). [Constructable, CorpseName]
- `RaptorRiding.cs` — **RaptorRiding : BaseMount** — Mount ("a darkrazor"). [Constructable, CorpseName]
- `RavenousRiding.cs` — **RavenousRiding : BaseMount** — Mount. [Constructable, CorpseName]
- `Stegosaurus.cs` — **Stegosaurus : BaseCreature** — Creature ("a stegosaurus", AI_Animal, Aggressor). [Constructable, CorpseName]
- `Teradactyl.cs` — **Teradactyl : BaseMount** — Mount. [Constructable, CorpseName]
- `Titanoboa.cs` — **Titanoboa : BaseCreature** — Creature ("a titanoboa", AI_Melee, Closest). [Constructable, CorpseName]
- `Tyranasaur.cs` — **Tyranasaur : BaseCreature** — Creature ("a tyranasaur", AI_Melee, Closest). [Constructable, CorpseName]

#### Scripts/Mobiles/Reptilian/Fish — Creatures/NPCs: Fish. Types: GreatWhite, Lochasaur, Megalodon, SeaHorses, ....

- `GreatWhite.cs` — **GreatWhite : BaseCreature** — Creature ("a great white shark", AI_Melee, Closest). [Constructable, CorpseName]
- `Lochasaur.cs` — **Lochasaur : BaseCreature** — Creature ("a lochasaur", AI_Melee, Closest). [Constructable, CorpseName]
- `Megalodon.cs` — **Megalodon : BaseCreature** — Creature ("a megalodon", AI_Melee, Closest). [Constructable, CorpseName]
- `SeaHorses.cs` — **SeaHorses : BaseCreature** — Creature ("a sea horse", AI_Mage, Aggressor). [Constructable, CorpseName]
- `Shark.cs` — **Shark : BaseCreature** — Creature ("a shark", AI_Melee, Closest). [Constructable, CorpseName]

#### Scripts/Mobiles/Reptilian/Frogs — Creatures/NPCs: Frogs. Types: BullFrog, FireToad, Frog, GiantToad, ....

- `BullFrog.cs` — **BullFrog : BaseCreature** — Creature ("a bull frog", AI_Melee, Closest). [Constructable, CorpseName, TypeAlias]
- `FireToad.cs` — **FireToad : BaseCreature** — Creature ("a fire toad", AI_Mage, Closest). [Constructable, CorpseName]
- `Frog.cs` — **Frog : BaseCreature** — Creature ("a frog", AI_Animal, Aggressor). [Constructable, CorpseName]
- `GiantToad.cs` — **GiantToad : BaseCreature** — Creature ("a giant toad", AI_Melee, Closest). [Constructable, CorpseName, TypeAlias] (+1 type)
- `IceToad.cs` — **IceToad : BaseCreature** — Creature ("an ice toad", AI_Melee, Closest). [Constructable, CorpseName, TypeAlias]
- `PoisonFrog.cs` — **PoisonFrog : BaseCreature** — Creature ("a marsh frog", AI_Melee, Closest). [Constructable, CorpseName]
- `Toad.cs` — **Toad : BaseCreature** — Creature ("a toad", AI_Animal, Aggressor). [Constructable, CorpseName]

#### Scripts/Mobiles/Reptilian/Sea — Creatures/NPCs: Sea. Types: Calamari, GiantCrab, GiantEel, GiantLamprey, ....

- `Calamari.cs` — **Calamari : BaseCreature** — Creature ("a calamari", AI_Mage, Closest). [Constructable, CorpseName]
- `GiantCrab.cs` — **GiantCrab : BaseCreature** — Creature ("a giant crab", AI_Melee, Closest). [Constructable, CorpseName]
- `GiantEel.cs` — **GiantEel : BaseCreature** — Creature ("a giant eel", AI_Melee, Closest). [Constructable, CorpseName]
- `GiantLamprey.cs` — **GiantLamprey : BaseCreature** — Creature ("a sleech", AI_Melee, Closest). [Constructable, CorpseName]
- `GiantSquid.cs` — **GiantSquid : BaseCreature** — Creature ("a giant squid", AI_Melee, Closest). [Constructable, CorpseName]
- `Kraken.cs` — **Kraken : BaseCreature** — Creature ("a kraken", AI_Melee, Closest). [Constructable, CorpseName]
- `Leviathan.cs` — **Leviathan : BaseCreature** — Creature ("a leviathan", AI_Mage, Closest). [Constructable, CorpseName]
- `Lurker.cs` — **Lurker : BaseCreature** — Creature ("a lurker", AI_Melee, Closest). [Constructable, CorpseName]
- `Raptus.cs` — **Raptus : BaseCreature** — Creature ("a raptus", AI_Melee, Closest). [Constructable, CorpseName]
- `Slitheran.cs` — **Slitheran : BaseCreature** — Creature ("a slitheran", AI_Melee, Closest). [Constructable, CorpseName]

#### Scripts/Mobiles/Reptilian/Snakes — Creatures/NPCs: Snakes. Types: BloodSnake, DeepSeaSerpent, GiantAdder, GiantIceWorm, ....

- `BloodSnake.cs` — **BloodSnake : BaseCreature** — Creature ("a blood snake", AI_Melee, Closest). [Constructable, CorpseName]
- `DeepSeaSerpent.cs` — **DeepSeaSerpent : BaseCreature** — Creature ("a deep sea serpent", AI_Mage, Closest). [Constructable, CorpseName]
- `GiantAdder.cs` — **GiantAdder : BaseCreature** — Creature ("a giant adder", AI_Melee, Closest). [Constructable, CorpseName, TypeAlias]
- `GiantIceWorm.cs` — **GiantIceWorm : BaseCreature** — Creature ("an arcticonda", AI_Melee, Closest). [Constructable, CorpseName]
- `GiantSerpent.cs` — **GiantSerpent : BaseCreature** — Creature ("a giant serpent", AI_Melee, Closest). [Constructable, CorpseName, TypeAlias]
- `GiantSnake.cs` — **GiantSnake : BaseCreature** — Creature ("a giant snake", AI_Melee, Aggressor). [Constructable, CorpseName]
- `GoldenSerpent.cs` — **GoldenSerpent : BaseCreature** — Creature ("a golden serpent", AI_Melee, Closest). [Constructable, CorpseName, TypeAlias]
- `IceSerpent.cs` — **IceSerpent : BaseCreature** — Creature ("an ice serpent", AI_Mage, Closest). [Constructable, CorpseName, TypeAlias]
- `IceSnake.cs` — **IceSnake : BaseCreature** — Creature ("an ice snake", AI_Melee, Closest). [Constructable, CorpseName, TypeAlias]
- `JadeSerpent.cs` — **JadeSerpent : BaseCreature** — Creature ("a jade serpent", AI_Mage, Aggressor). [Constructable, CorpseName]
- `Jormungand.cs` — **Jormungandr : BaseCreature** — Creature ("Jormungandr", AI_Mage, Closest). [Constructable, CorpseName] (+1 type)
- `JungleViper.cs` — **JungleViper : BaseCreature** — Creature ("a jungle viper", AI_Melee, Closest). [Constructable, CorpseName]
- `LargeSnake.cs` — **LargeSnake : BaseCreature** — Creature ("a large snake", AI_Melee, Closest). [Constructable, CorpseName]
- `LavaSerpent.cs` — **LavaSerpent : BaseCreature** — Creature ("a lava serpent", AI_Melee, Closest). [Constructable, CorpseName, TypeAlias]
- `LavaSnake.cs` — **LavaSnake : BaseCreature** — Creature ("a magma snake", AI_Melee, Closest). [Constructable, CorpseName, TypeAlias]
- `LesserSeaSnake.cs` — **LesserSeaSnake : BaseCreature** — Creature ("a deep sea snake", AI_Mage, Closest). [Constructable, CorpseName]
- `RandomSerpent.cs` — **RandomSerpent : BaseCreature** — Creature ("an anaconda", AI_Melee, Closest). [Constructable, CorpseName]
- `SeaSerpent.cs` — **SeaSerpent : BaseCreature** — Creature ("a sea serpent", AI_Mage, Closest). [Constructable, CorpseName, TypeAlias]
- `SeaSnake.cs` — **SeaSnake : BaseCreature** — Creature ("a sea snake", AI_Melee, Closest). [Constructable, CorpseName]
- `SilverSerpent.cs` — **SilverSerpent : BaseCreature** — Creature ("a silver serpent", AI_Melee, Closest). [Constructable, CorpseName, TypeAlias]
- `Snake.cs` — **Snake : BaseCreature** — Creature ("a snake", AI_Animal, Aggressor). [Constructable, CorpseName]

#### Scripts/Mobiles/Slimes — Creatures/NPCs: Slimes. Types: AcidPuddle, BlackPudding, BloodWorm, CarcassWorm, ....

- `AcidPuddle.cs` — **AcidPuddle : BaseCreature** — Creature ("an acid puddle", AI_Melee, Closest). [Constructable, CorpseName]
- `BlackPudding.cs` — **BlackPudding : BaseCreature** — Creature ("a black pudding", AI_Melee, Closest). [Constructable, CorpseName]
- `BloodWorm.cs` — **BloodWorm : BaseCreature** — Creature ("a blood worm", AI_Melee, Closest). [Constructable, CorpseName]
- `CarcassWorm.cs` — **CarcassWorm : BaseCreature** — Creature ("a carcass worm", AI_Melee, Closest). [Constructable, CorpseName]
- `FrostOoze.cs` — **FrostOoze : BaseCreature** — Creature ("a frost ooze", AI_Melee, Closest). [Constructable, CorpseName]
- `GiantLeech.cs` — **GiantLeech : BaseCreature** — Creature ("a giant leech", AI_Melee, Closest). [Constructable, CorpseName]
- `GreenSlime.cs` — **GreenSlime : BaseCreature** — Creature ("a green slime", AI_Melee, Closest). [Constructable, CorpseName]
- `Jellyfish.cs` — **Jellyfish : BaseCreature** — Creature ("a jellyfish", AI_Melee, Closest). [Constructable, CorpseName]
- `LavaPuddle.cs` — **LavaPuddle : BaseCreature** — Creature ("a lava puddle", AI_Melee, Closest). [Constructable, CorpseName]
- `MarshWurm.cs` — **MarshWurm : BaseCreature** — Creature ("a marsh wurm", AI_Melee, Closest). [Constructable, CorpseName]
- `OilSlick.cs` — **OilSlick : BaseCreature** — Creature ("an oil slick", AI_Melee, Closest). [Constructable, CorpseName]
- `Slime.cs` — **Slime : BaseCreature** — Creature ("a slime", AI_Melee, Closest). [Constructable, CorpseName]
- `SlimeDevil.cs` — **SlimeDevil : BaseCreature** — Creature ("a slime devil", AI_Mage, Closest). [Constructable, CorpseName]
- `StrangleVine.cs` — **StrangleVine : BaseCreature** — Creature ("a strangle vine", AI_Melee, Closest). [Constructable, CorpseName]
- `Viscera.cs` — **Viscera : BaseCreature** — Creature ("a pile of viscera", AI_Melee, Closest). [Constructable, CorpseName]

#### Scripts/Mobiles/Summoned — Creatures/NPCs: Summoned. Types: BladeSpirits, DeathVortex, EnergyVortex, EvilBladeSpirits, ....

- `BladeSpirits.cs` — **BladeSpirits : BaseCreature** — Creature ("a blade spirit", AI_Melee, Closest). [Constructable, CorpseName]
- `DeathVortex.cs` — **DeathVortex : BaseCreature** — Creature ("a death vortex", AI_Melee, Closest). [Constructable, CorpseName]
- `EnergyVortex.cs` — **EnergyVortex : BaseCreature** — Creature ("an energy vortex", AI_Melee, Closest). [Constructable, CorpseName]
- `EvilBladeSpirits.cs` — **EvilBladeSpirits : BaseCreature** — Creature ("a blade spirit", AI_Melee, Closest). [Constructable, CorpseName]
- `EvilEnergyVortex.cs` — **EvilEnergyVortex : BaseCreature** — Creature ("an energy vortex", AI_Melee, Closest). [Constructable, CorpseName]
- `EvilIcyVortex.cs` — **EvilIcyVortex : BaseCreature** — Creature ("an icy vortex", AI_Melee, Closest). [Constructable, CorpseName]
- `EvilPlagueVortex.cs` — **EvilPlagueVortex : BaseCreature** — Creature ("a plague vortex", AI_Melee, Closest). [Constructable, CorpseName]
- `EvilScorchingVortex.cs` — **EvilScorchingVortex : BaseCreature** — Creature ("a scorching vortex", AI_Melee, Closest). [Constructable, CorpseName]
- `Fairy.cs` — **Fairy : BaseCreature** — Creature ("a fairy", AI_Mage, Closest). [Constructable, CorpseName]
- `GasCloud.cs` — **GasCloud : BaseCreature** — Creature ("a poison cloud", AI_Melee, Closest). [Constructable, CorpseName]
- `IceBladeSpirits.cs` — **IceBladeSpirits : BaseCreature** — Creature ("an icy blade spirit", AI_Melee, Closest). [Constructable, CorpseName]
- `ManureGolem.cs` — **ManureGolem : BaseCreature** — Creature ("a manure golem", AI_Melee, Closest). [Constructable, CorpseName]
- `SummonedAirElemental.cs` — **SummonedAirElemental : BaseCreature** — Creature ("an air elemental", AI_Mage, Closest). [Constructable, CorpseName]
- `SummonedAirElementalGreater.cs` — **SummonedAirElementalGreater : BaseCreature** — Creature ("an air elemental", AI_Mage, Closest). [Constructable, CorpseName]
- `SummonedCorpse.cs` — **SummonedCorpse : BaseCreature** — Creature ("an undead creature", AI_Melee, Closest). [Constructable, CorpseName]
- `SummonedDaemon.cs` — **SummonedDaemon : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName]
- `SummonedDaemonGreater.cs` — **SummonedDaemonGreater : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName]
- `SummonedEarthElemental.cs` — **SummonedEarthElemental : BaseCreature** — Creature ("an earth elemental", AI_Melee, Closest). [Constructable, CorpseName]
- `SummonedEarthElementalGreater.cs` — **SummonedEarthElementalGreater : BaseCreature** — Creature ("an earth elemental", AI_Melee, Closest). [Constructable, CorpseName]
- `SummonedFireElemental.cs` — **SummonedFireElemental : BaseCreature** — Creature ("a fire elemental", AI_Mage, Closest). [Constructable, CorpseName]
- `SummonedFireElementalGreater.cs` — **SummonedFireElementalGreater : BaseCreature** — Creature ("a fire elemental", AI_Mage, Closest). [Constructable, CorpseName]
- `SummonedTiger.cs` — **SummonedTiger : BaseCreature** — Creature ("a mystical tiger", AI_Melee, Closest). [Constructable, CorpseName]
- `SummonedWaterElemental.cs` — **SummonedWaterElemental : BaseCreature** — Creature ("a water elemental", AI_Mage, Closest). [Constructable, CorpseName]
- `SummonedWaterElementalGreater.cs` — **SummonedWaterElementalGreater : BaseCreature** — Creature ("a water elemental", AI_Mage, Closest). [Constructable, CorpseName]
- `Swarm.cs` — **Swarm : BaseCreature** — Creature ("a swarm of insects", AI_Melee, Closest). [Constructable, CorpseName]
- `WineElemental.cs` — **WineElemental : BaseCreature** — Creature ("a wine elemental", AI_Mage, Closest). [Constructable, CorpseName]

#### Scripts/Mobiles/Undead — Creatures/NPCs: Undead. Types: AncientLich, AquaticGhoul, Bodak, BoneKnight, ....

- `AncientLich.cs` — **AncientLich : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName]
- `AquaticGhoul.cs` — **AquaticGhoul : BaseCreature** — Creature ("an aquatic ghoul", AI_Melee, Closest). [Constructable, CorpseName]
- `Bodak.cs` — **Bodak : BaseCreature** — Creature ("a bodak", AI_Mage, Closest). [Constructable, CorpseName]
- `BoneKnight.cs` — **BoneKnight : BaseCreature** — Creature ("a bone knight", AI_Melee, Closest). [Constructable, CorpseName]
- `BoneMagi.cs` — **BoneMagi : BaseCreature** — Creature ("a bone magi", AI_Mage, Closest). [Constructable, CorpseName]
- `BoneSailor.cs` — **BoneSailor : BaseCreature** — Creature ("an undead sailor", AI_Melee, Closest). [Constructable, CorpseName]
- `BoneSlasher.cs` — **BoneSlasher : BaseCreature** — Creature ("a bone slasher", AI_Melee, Closest). [Constructable, CorpseName]
- `DarkReaper.cs` — **DarkReaper : BaseCreature** — Creature ("a dark reaper", AI_Mage, Closest). [Constructable, CorpseName]
- `DeadKnight.cs` — **DeadKnight : BaseCreature** — Creature ("skull", AI_Melee, Closest). [Constructable, CorpseName]
- `DeadWizard.cs` — **DeadWizard : BaseCreature** — Creature ("staff", AI_Mage, Closest). [Constructable, CorpseName]
- `DeathBear.cs` — **DeathBear : BaseCreature** — Creature ("an undead bear", AI_Melee, Closest). [Constructable, CorpseName]
- `DeathWolf.cs` — **DeathWolf : BaseCreature** — Creature ("an undead wolf", AI_Melee, Closest). [Constructable, CorpseName]
- `DemiLich.cs` — **DemiLich : BaseCreature** — Creature ("skull", AI_Mage, Closest). [Constructable, CorpseName]
- `DiseasedMummy.cs` — **DiseasedMummy : BaseCreature** — Creature ("a diseased mummy", AI_Melee, Closest). [Constructable, CorpseName]
- `Dracolich.cs` — **Dracolich : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName]
- `Dracula.cs` — **Dracula : BaseCreature** — Creature ("Dracula", AI_Mage, Closest). [Constructable, CorpseName]
- `DragonGhost.cs` — **DragonGhost : BaseCreature** — Creature ("a ghostly dragon", AI_Mage, Closest). [Constructable, CorpseName]
- `FrailSkeleton.cs` — **FrailSkeleton : BaseCreature** — Creature ("a frail skeleton", AI_Melee, Closest). [Constructable, CorpseName]
- `FrozenCorpse.cs` — **FrozenCorpse : BaseCreature** — Creature ("a frozen corpse", AI_Melee, Closest). [Constructable, CorpseName]
- `GargoyleBones.cs` — **GargoyleBones : BaseCreature** — Creature ("gargoyle bones", AI_Melee, Closest). [Constructable, CorpseName]
- `GhostDragyn.cs` — **GhostDragyn : BaseMount** — Mount ("a ghost dragyn"). [Constructable, CorpseName, TypeAlias]
- `GhostGargoyle.cs` — **GhostGargoyle : BaseCreature** — Creature ("a ghostly gargoyle", AI_Mage, Closest). [Constructable, CorpseName]
- `GhostPirate.cs` — **GhostPirate : BaseCreature** — Creature ("a ghost", AI_Melee, Closest). [Constructable, CorpseName]
- `GhostWarrior.cs` — **GhostWarrior : BaseCreature** — Creature ("a ghost", AI_Melee, Closest). [Constructable, CorpseName]
- `GhostWizard.cs` — **GhostWizard : BaseCreature** — Creature ("a ghost", AI_Mage, Closest). [Constructable, CorpseName]
- `Ghostly.cs` — **Ghostly : BaseCreature** — Creature ("shroud of ", AI_Mage, Closest). [Constructable, CorpseName]
- `Ghoul.cs` — **Ghoul : BaseCreature** — Creature ("a ghoul", AI_Melee, Closest). [Constructable, CorpseName]
- `GiantSkeleton.cs` — **GiantSkeleton : BaseCreature** — Creature ("bone carved chest", AI_Melee, Closest). [Constructable, CorpseName]
- `GraveSeeker.cs` — **GraveSeeker : BaseCreature** — Creature ("a grave seeker", AI_Melee, Closest). [Constructable, CorpseName]
- `GrundulVarg.cs` — **GrundulVarg : BaseCreature** — Creature ("Grundul Varg", AI_Melee, Closest). [Constructable, CorpseName]
- `HellSteed.cs` — **HellSteed : BaseMount** — Mount. [Constructable, CorpseName]
- `IceGhoul.cs` — **IceGhoul : BaseCreature** — Creature ("an icy ghoul", AI_Melee, Closest). [Constructable, CorpseName]
- `Kull.cs` — **Kull : BaseCreature** — Creature ("Spirit of Kull", AI_Melee, Closest). [Constructable, CorpseName]
- `Lich.cs` — **Lich : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName]
- `LichKing.cs` — **LichKing : BaseCreature** — Creature ("Chest of Lich King Relics", AI_Mage, Closest). [Constructable, CorpseName]
- `LichLord.cs` — **LichLord : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName]
- `LostKnight.cs` — **LostKnight : BaseCreature** — Creature (AI_Melee, Closest). [Constructable, CorpseName]
- `Mummy.cs` — **Mummy : BaseCreature** — Creature ("a mummy", AI_Melee, Closest). [Constructable, CorpseName]
- `MummyGiant.cs` — **MummyGiant : BaseCreature** — Creature ("pharaoh chest", AI_Mage, Closest). [Constructable, CorpseName]
- `MummyLord.cs` — **MummyLord : BaseCreature** — Creature (AI_Melee, Closest). [Constructable, CorpseName]
- `Murk.cs` — **Murk : BaseCreature** — Creature ("Murk", AI_Melee, Closest). [Constructable, CorpseName]
- `Nazghoul.cs` — **Nazghoul : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName]
- `NecroticHound.cs` — **NecroticHound : BaseCreature** — Creature ("a necrotic hound", AI_Melee, Closest). [Constructable, CorpseName]
- `Phantom.cs` — **Phantom : BaseCreature** — Creature ("a phantom", AI_Mage, Closest). [Constructable, CorpseName]
- `RestlessSoul.cs` — **RestlessSoul : BaseCreature** — Creature ("restless soul", AI_Melee, Closest). [Constructable, CorpseName]
- `Revenant.cs` — **Revenant : BaseCreature** — Creature ("a revenant", AI_Melee, Closest).
- `RevenantLion.cs` — **RevenantLion : BaseCreature** — Creature ("an undead lion", AI_Mage, Closest). [Constructable, CorpseName]
- `RottingCorpse.cs` — **RottingCorpse : BaseCreature** — Creature (AI_Melee, Closest). [Constructable, CorpseName]
- `RottingMinotaur.cs` — **RottingMinotaur : BaseCreature** — Creature ("a rotting minotaur", AI_Melee, Closest). [Constructable, CorpseName]
- `RottingSquid.cs` — **RottingSquid : BaseCreature** — Creature ("a rotting squid", AI_Melee, Closest). [Constructable, CorpseName]
- `SeaGhost.cs` — **SeaGhost : BaseCreature** — Creature ("a specter", AI_Mage, Closest). [Constructable, CorpseName]
- `SeaZombie.cs` — **SeaZombie : BaseCreature** — Creature ("a zombie", AI_Melee, Closest). [Constructable, CorpseName]
- `Shade.cs` — **Shade : BaseCreature** — Creature ("a shade", AI_Mage, Closest). [Constructable, CorpseName]
- `Shroud.cs` — **Shroud : BaseCreature** — Creature ("evil robe of ", AI_Mage, Closest). [Constructable, CorpseName]
- `SkeletalDragon.cs` — **SkeletalDragon : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName]
- `SkeletalGargoyle.cs` — **SkeletalGargoyle : BaseCreature** — Creature ("a skeletal gargoyle", AI_Mage, Closest). [Constructable, CorpseName]
- `SkeletalKnight.cs` — **SkeletalKnight : BaseCreature** — Creature ("a skeletal knight", AI_Melee, Closest). [Constructable, CorpseName]
- `SkeletalMage.cs` — **SkeletalMage : BaseCreature** — Creature ("a skeletal mage", AI_Mage, Closest). [Constructable, CorpseName]
- `SkeletalPirate.cs` — **SkeletalPirate : BaseCreature** — Creature ("a skeletal pirate", AI_Melee, Closest). [Constructable, CorpseName]
- `SkeletalSamurai.cs` — **SkeletalSamurai : BaseCreature** — Creature ("a skeletal samurai", AI_Melee, Closest). [Constructable, CorpseName]
- `SkeletalWarrior.cs` — **SkeletalWarrior : BaseCreature** — Creature ("a skeletal warrior", AI_Melee, Closest). [Constructable, CorpseName]
- `SkeletalWizard.cs` — **SkeletalWizard : BaseCreature** — Creature ("a skeletal wizard", AI_Mage, Closest). [Constructable, CorpseName]
- `Skeleton.cs` — **Skeleton : BaseCreature** — Creature ("a skeleton", AI_Melee, Closest). [Constructable, CorpseName]
- `SkeletonArcher.cs` — **SkeletonArcher : BaseCreature** — Creature ("a skeletal archer", AI_Archer, Closest). [Constructable, CorpseName]
- `SkeletonDragon.cs` — **SkeletonDragon : BaseCreature** — Creature ("a skeletal dragon", AI_Mage, Closest). [Constructable, CorpseName]
- `SkeletonHorse.cs` — **SkeletonHorse : BaseMount** — Mount. [Constructable, CorpseName]
- `SoulReaper.cs` — **SoulReaper : BaseCreature** — Creature ("a soul reaper", AI_Melee, Closest). [Constructable, CorpseName]
- `SoulWorm.cs` — **SoulWorm : BaseCreature** — Creature ("a grave wurm", AI_Melee, Closest). [Constructable, CorpseName]
- `SpectralGargoyle.cs` — **SpectralGargoyle : BaseCreature** — Creature ("a spectral gargoyle", AI_Mage, Closest). [Constructable, CorpseName]
- `Spectre.cs` — **Spectre : BaseCreature** — Creature ("a spectre", AI_Mage, Closest). [Constructable, CorpseName]
- `Spirit.cs` — **Spirit : BaseCreature** — Creature ("a spirit", AI_Mage, Closest). [Constructable, CorpseName]
- `Surtaz.cs` — **Surtaz : BaseCreature** — Creature ("Surtaz", AI_Mage, Closest). [Constructable, CorpseName] (+2 types)
- `TitanLich.cs` — **TitanLich : BaseCreature** — Creature ("titan lich chest", AI_Mage, Closest). [Constructable, CorpseName]
- `Undead.cs` — **Undead : BaseCreature** — Creature ("a frail skeleton", AI_Melee, Closest). [Constructable, CorpseName]
- `UndeadDruid.cs` — **UndeadDruid : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName]
- `UndeadGiant.cs` — **UndeadGiant : BaseCreature** — Creature ("an undead giant", AI_Melee, Closest). [Constructable, CorpseName]
- `Vampire.cs` — **Vampire : BaseCreature** — Creature ("a young vampire", AI_Mage, Closest). [Constructable, CorpseName]
- `VampireLord.cs` — **VampireLord : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName]
- `VampirePrince.cs` — **VampirePrince : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName]
- `VampireWoods.cs` — **VampireWoods : BaseCreature** — Creature ("a vampire", AI_Mage, Closest). [Constructable, CorpseName]
- `Vordo.cs` — **Vordo : BaseCreature** — Creature ("Vordo", AI_Mage, Closest). [Constructable, CorpseName] (+1 type)
- `WailingBanshee.cs` — **WailingBanshee : BaseCreature** — Creature ("a wailing banshee", AI_Melee, Closest). [Constructable, CorpseName]
- `WalkingCorpse.cs` — **WalkingCorpse : BaseCreature** — Creature ("a walking corpse", AI_Melee, Closest). [Constructable, CorpseName]
- `Wight.cs` — **Wight : BaseCreature** — Creature ("a wight", AI_Melee, Closest). [Constructable, CorpseName]
- `Wraith.cs` — **Wraith : BaseCreature** — Creature ("a wraith", AI_Mage, Closest). [Constructable, CorpseName]
- `Zombie.cs` — **Zombie : BaseCreature** — Creature ("a zombie", AI_Melee, Closest). [Constructable, CorpseName]
- `ZombieDragon.cs` — **ZombieDragon : BaseCreature** — Creature ("a zombie dragon", AI_Mage, Closest). [Constructable, CorpseName]
- `ZombieGargoyle.cs` — **ZombieGargoyle : BaseCreature** — Creature ("a zombie gargoyle", AI_Melee, Closest). [Constructable, CorpseName]
- `ZombieGiant.cs` — **ZombieGiant : BaseCreature** — Creature ("dead giant's hand scythe", AI_Melee, Closest). [Constructable, CorpseName]
- `ZombieMage.cs` — **ZombieMage : BaseCreature** — Creature ("a zombie mage", AI_Mage, Closest). [Constructable, CorpseName]

#### Scripts/Mobiles/Unique — Creatures/NPCs: Unique. Types: BaneOfAnarchy, BaneOfInsanity, BaneOfWantoness, BaronAlmric, ....

- `BaneOfAnarchy.cs` — **BaneOfAnarchy : BaseCreature** — Creature ("plate tunic of anarchy", AI_Melee, Closest). [Constructable]
- `BaneOfInsanity.cs` — **BaneOfInsanity : BaseCreature** — Creature ("plate tunic of insanity", AI_Melee, Closest). [Constructable]
- `BaneOfWantoness.cs` — **BaneOfWantoness : BaseCreature** — Creature ("plate tunic of wantoness", AI_Melee, Closest). [Constructable]
- `BaronAlmric.cs` — **BaronAlmric : BaseCreature** — Creature ("Almric", AI_Melee, Closest). [Constructable, CorpseName]
- `CodexGargoyles.cs` — **CodexGargoyleA : BaseCreature** — Creature ("Naxatilor", AI_Mage, Closest). [Constructable] (+3 types)
- `Exodus.cs` — **Exodus : BaseCreature** — Creature ("Exodus", AI_Mage, Closest). [Constructable, CorpseName] (+4 types)
- `GrayDragon.cs` — **GrayDragon : BaseCreature** — Creature ("a gray dragon", AI_Melee, Closest). [Constructable, CorpseName] (+1 type)
- `KhumashGor.cs` — **KhumashGor : BaseCreature** — Creature ("Khumash-Gor", AI_Mage, Closest). [Constructable, CorpseName]
- `Mangar.cs` — **Mangar : BaseCreature** — Creature ("Mangar", AI_Mage, Closest). [Constructable, CorpseName] (+3 types)
- `RuneGuardian.cs` — **RuneGuardian : BaseCreature** — Creature ("a sentinel", AI_Mage, Closest). [Constructable] (+2 types)
- `SerpentOfChaos.cs` — **SerpentOfChaos : BaseCreature** — Creature ("Serpent of Chaos", AI_Melee, Closest). [Constructable, CorpseName]
- `SerpentOfOrder.cs` — **SerpentOfOrder : BaseCreature** — Creature ("Serpent of Order", AI_Melee, Closest). [Constructable, CorpseName]
- `Shadowlord.cs` — **Shadowlord : BaseCreature** — Creature ("shadowlord robe", AI_Mage, Closest). [Constructable, CorpseName]
- `SlasherOfVoid.cs` — **SlasherOfVoid : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName]
- `Spectres.cs` — **Spectres : BaseCreature** — Creature ("a spectre", AI_Mage, Closest). [Constructable, CorpseName] (+1 type)
- `Tarjan.cs` — **Tarjan : BaseCreature** — Creature ("Tarjan", AI_Mage, Closest). [Constructable, CorpseName] (+3 types)
- `TitanHydros.cs` — **TitanHydros : BaseCreature** — Creature ("Hydros", AI_Mage, Closest). [Constructable, CorpseName]
- `TitanLithos.cs` — **TitanLithos : BaseCreature** — Creature ("Lithos", AI_Melee, Closest). [Constructable, CorpseName]
- `TitanPyros.cs` — **TitanPyros : BaseCreature** — Creature ("Pyros", AI_Mage, Closest). [Constructable, CorpseName]
- `TitanStratos.cs` — **TitanStratos : BaseCreature** — Creature ("Stratos", AI_Mage, Closest). [Constructable, CorpseName]

#### Scripts/Mobiles/Unusual — Creatures/NPCs: Unusual. Types: Beholder, BloodGodTentacles, Cronosaurus, ElderGazer, ....

- `Beholder.cs` — **Beholder : BaseCreature** — Creature (AI_Mage, Closest). [Constructable, CorpseName]
- `BloodGodTentacles.cs` — **BloodGodTentacles : BaseCreature** — Creature ("tentacles of the blood god", AI_Melee, Closest). [Constructable, CorpseName] (+1 type)
- `Cronosaurus.cs` — **Cronosaurus : BaseCreature** — Creature ("a cronosaurus", AI_Melee, Closest). [Constructable, CorpseName]
- `ElderGazer.cs` — **ElderGazer : BaseCreature** — Creature ("an elder gazer", AI_Mage, Closest). [Constructable, CorpseName]
- `EyeOfTheDeep.cs` — **EyeOfTheDeep : BaseCreature** — Creature ("an eye of the deep", AI_Mage, Closest). [Constructable, CorpseName]
- `FloatingEye.cs` — **FloatingEye : BaseCreature** — Creature ("a floating eye", AI_Mage, Closest). [Constructable, CorpseName]
- `Gazer.cs` — **Gazer : BaseCreature** — Creature ("a gazer", AI_Mage, Closest). [Constructable, CorpseName]
- `GorgonRiding.cs` — **GorgonRiding : BaseMount** — Mount ("useless stone"). [Constructable, CorpseName]
- `HeadlessOne.cs` — **HeadlessOne : BaseCreature** — Creature ("a headless one", AI_Melee, Closest). [Constructable, CorpseName]
- `HookHorror.cs` — **HookHorror : BaseCreature** — Creature ("a hook horror", AI_Melee, Closest). [Constructable, CorpseName]
- `Kuthulu.cs` — **Kuthulu : BaseCreature** — Creature ("a kuthulu", AI_Mage, Closest). [Constructable, CorpseName]
- `Roper.cs` — **Roper : BaseCreature** — Creature ("a roper", AI_Melee, Closest). [Constructable, CorpseName]
- `Seeker.cs` — **Seeker : BaseCreature** — Creature ("a seeker", AI_Mage, Closest). [Constructable, CorpseName]
- `SoulSucker.cs` — **SoulSucker : BaseCreature** — Creature ("a soul sucker", AI_Mage, Closest). [Constructable, CorpseName]
- `StoneRoper.cs` — **StoneRoper : BaseCreature** — Creature ("a stone roper", AI_Melee, Closest). [Constructable, CorpseName]
- `UmberHulk.cs` — **UmberHulk : BaseCreature** — Creature ("an umber hulk", AI_Melee, Closest). [Constructable, CorpseName]
- `VorpalBunny.cs` — **VorpalBunny : BaseCreature** — Creature ("a vorpal bunny", AI_Melee, Closest). [Constructable, CorpseName] (+1 type)
- `Watcher.cs` — **Watcher : BaseCreature** — Creature ("a watcher", AI_Melee, Closest). [Constructable, CorpseName]
- `Xenomorph.cs` — **Xenomorph : BaseCreature** — Creature ("a xenomorph", AI_Melee, Closest). [Constructable, CorpseName]
- `Xenomutant.cs` — **Xenomutant : BaseCreature** — Creature ("a xenomutant", AI_Mage, Closest). [Constructable, CorpseName]
- `Xorn.cs` — **Xorn : BaseCreature** — Creature ("a xorn", AI_Melee, Closest). [Constructable, CorpseName]

#### Scripts/System/Cache — Support system: Cache. Types: ItemSnapshot, ItemSnapshotCache.

- `ItemSnapshot.cs` — **ItemSnapshot** — class (~6 public members).
- `ItemSnapshotCache.cs` — **ItemSnapshotCache** — class (~3 public members).

#### Scripts/System/Chat — Support system: Chat. Types: ChatJail, RUOVersion.

- `Jail.cs` — **ChatJail** — class (~2 public members).
- `RUOVersion.cs` — **RUOVersion** — class (~5 public members).

#### Scripts/System/Chat/Channels — Support system: Channels. Types: Alliance, Channel, Faction, Guild, ....

- `Alliance.cs` — **Alliance : Channel** — class (~7 public members).
- `Channel.cs` — **Channel** — class (~45 public members). (+3 types)
- `Faction.cs` — **Faction : Channel** — class (~4 public members).
- `Guild.cs` — **Guild : Channel** — class (~8 public members).
- `Irc.cs` — **Irc : Channel** — class (~6 public members).
- `Multi.cs` — **Multi : Channel** — class (~6 public members).
- `Public.cs` — **Public : Channel** — class (~2 public members).
- `Staff.cs` — **Staff : Channel** — class (~4 public members).

#### Scripts/System/Chat/Commands — Support system: Commands. Types: All, Friends, HelpContents, Mail, ....

- `All.cs` — **All** — class (~2 public members).
- `Friends.cs` — **Friends** — class (~2 public members).
- `HelpContents.cs` — **HelpContents** — class (~2 public members).
- `Mail.cs` — **Mail** — class (~2 public members).
- `Pm.cs` — **Pm** — class (~3 public members). (+1 type)
- `ViewAll.cs` — **ViewAll** — class (~2 public members).

#### Scripts/System/Chat/General — Support system: General. Types: Avatar, Chat3Guild, Chat3Party, CommandInfo, ....

- `Avatar.cs` — **Avatar** — class (~9 public members).
- `Chat3Guild.cs` — **Chat3Guild** — class (~5 public members).
- `Chat3Party.cs` — **Chat3Party : PartyCommands** — class (~9 public members).
- `CommandInfo.cs` — **CommandInfo** — Info/data container.
- `Data.cs` — **Data** — Data holder. (+2 types)
- `DefaultLocal.cs` — **DefaultLocal** — class (~2 public members).
- `Events.cs` — **Events** — class (~28 public members). (+4 types)
- `Filter.cs` — **Filter** — class (~4 public members). (+1 type)
- `General.cs` — **General** — class (~20 public members).
- `IrcConnection.cs` — **IrcConnection** — class (~15 public members). (+1 type)
- `Logging.cs` — **Logging** — class (~3 public members).
- `Message.cs` — **Message** — class (~14 public members). (+1 type)
- `MultiConnection.cs` — **MultiConnection** — class (~19 public members). (+1 type)
- `Notification.cs` — **Notification** — class (~12 public members). (+3 types)
- `TrackSpam.cs` — **TrackSpam** — class (~3 public members).

#### Scripts/System/Chat/Gumps/1.0 Skin — Support system: 1.0 skin. Types: ListGump10, PmNotifyGump10.

- `ListGump10.cs` — **ListGump10 : GumpPlus** — class (~14 public members). (+8 types)
- `PmNotifyGump10.cs` — **PmNotifyGump10 : GumpPlus** — class (~2 public members).

#### Scripts/System/Chat/Gumps/2.0 Skin — Support system: 2.0 skin. Types: ListGump20, PmNotifyGump20.

- `ListGump20.cs` — **ListGump20 : GumpPlus** — class (~15 public members). (+9 types)
- `PmNotifyGump20.cs` — **PmNotifyGump20 : GumpPlus** — class (~2 public members).

#### Scripts/System/Chat/Gumps/3.0 Skin — Support system: 3.0 skin. Types: ChannelGump, ColorsGump, EditNotGump, FilterGump, ....

- `ChannelGump.cs` — **ChannelGump : GumpPlus** — class (~4 public members). (+1 type)
- `ColorsGump.cs` — **ColorsGump : GumpPlus** — class (~7 public members). (+1 type)
- `EditNotGump.cs` — **EditNotGump : GumpPlus** — class (~3 public members). (+1 type)
- `FilterGump.cs` — **FilterGump : GumpPlus** — class (~3 public members). (+1 type)
- `GeneralGump.cs` — **GeneralGump : GumpPlus** — class (~2 public members).
- `GlobalGump.cs` — **GlobalGump : GumpPlus** — class (~4 public members).
- `HelpContentsGump.cs` — **HelpContentsGump : GumpPlus** — class (~4 public members). (+1 type)
- `HistoryGump.cs` — **HistoryGump : GumpPlus** — class (~2 public members).
- `IrcGump.cs` — **IrcGump : GumpPlus** — class (~3 public members). (+1 type)
- `ListGump.cs` — **ListGump : GumpPlus** — class (~11 public members). (+5 types)
- `LoggingGump.cs` — **LoggingGump : GumpPlus** — class (~2 public members).
- `MailGump.cs` — **MailGump : GumpPlus** — class (~4 public members).
- `MessageGump.cs` — **MessageGump : GumpPlus** — class (~2 public members).
- `MultiGump.cs` — **MultiGump : GumpPlus** — class (~2 public members).
- `PmNotifyGump.cs` — **PmNotifyGump : GumpPlus** — class (~2 public members).
- `ProfileGump.cs` — **ProfileGump : GumpPlus** — class (~3 public members). (+1 type)
- `SendMessageGump.cs` — **SendMessageGump : GumpPlus** — class (~5 public members). (+1 type)
- `SpamGump.cs` — **SpamGump : GumpPlus** — class (~2 public members).

#### Scripts/System/Chat/Gumps/Error Reporting — Support system: Error reporting. Types: Errors, ErrorsGump, ErrorsNotifyGump.

- `Errors.cs` — **Errors** — class (~7 public members).
- `ErrorsGump.cs` — **ErrorsGump : GumpPlus** — class (~2 public members).
- `ErrorsNotifyGump.cs` — **ErrorsNotifyGump : GumpPlus** — class (~2 public members).

#### Scripts/System/Chat/Gumps/Gumps Plus — Support system: Gumps plus. Types: BackgroundPlus, ButtonPlus, GumpInfo, GumpPlus, ....

- `BackgroundPlus.cs` — **BackgroundPlus : GumpBackground** — class (~4 public members).
- `ButtonPlus.cs` — **ButtonPlus : GumpButton** — class (~5 public members).
- `GumpInfo.cs` — **GumpInfo** — Info/data container.
- `GumpPlus.cs` — **GumpPlus : Gump** — Gump UI.
- `HTML.cs` — **HTML** — class (~12 public members).
- `HtmlPlus.cs` — **HtmlPlus : GumpHtml** — class (~4 public members).
- `InfoGump.cs` — **InfoGump : GumpPlus** — class (~2 public members).

#### Scripts/System/Commands — GM/player commands and command frameworks.

- `Add.cs` — **Add** — Commands: [Tile, [TileRXYZ, [TileXYZ, [TileZ (+2 types)
- `AddToBank.cs` — **AddToBank** — Commands: [AddToBank (+4 types)
- `AddTrap.cs` — **AddTrapGump : Gump** — Commands: [AddTrap
- `Adddoorgump.cs` — **AddDoorGump : Gump** — Commands: [AddDoor (+1 type)
- `AddonGenerator.cs` — **AddonGenerator** — Commands: [AddonGen (+3 types)
- `AdminGump.cs` — **AdminGump : Gump** — Commands: [Admin (+7 types)
- `AreaLog.cs` — **AreaLog** — Commands: [AreaLog
- `Attributes.cs` — **UsageAttribute : Attribute** — Command helper.
- `Batch.cs` — **Batch : BaseCommand** — Commands: [Batch (+3 types)
- `BodyValues.cs` — **BodyValues** — Commands: [BodyValues (+1 type)
- `BoundingBoxPicker.cs` — **BoundingBoxPicker** — Command helper.
- `Builders.cs` — **Builder** — Commands: [Builders (+1 type)
- `CacheClear.cs` — **CacheClear** — Commands: [CacheClear, [CacheClear-ChangeUpdateRange, [CacheClear-GlobalLightLevel, [CacheClear-MovementAck, [CacheClear-PingACk, [CacheClear-SeasonChange
- `CleanUp.cs` — **CleanUpCommand** — Commands: [Internal-CountItems, [Internal-DeleteOrphans
- `ConvertPlayers.cs` — **ConvertPlayers** — Commands: [ConvertPlayers
- `Docs.cs` — **Docs** — Commands: [DocGen (+10 types)
- `Dupe.cs` — **Dupe** — Commands: [Dupe, [DupeInBag (+1 type)
- `ExportWSC.cs` — **ExportCommand** — Commands: [ExportWSC
- `FaceLog.cs` — **FaceLog** — Commands: [FaceLog
- `GenCategorization.cs` — **Categorization** — Commands: [RebuildCategorization (+4 types)
- `Handlers.cs` — **CommandHandlers** — Enables a speed boost for the invoker.  Disable with paramaters. (+8 types)
- `HelpAdmin.cs` — **HelpAdmin** — Commands: [helpadmin
- `HelpInfo.cs` — **HelpInfo** — Commands: [HelpInfo (+2 types)
- `ItemPrices.cs` — **ItemPrices** — Commands: [ItemPrices-ExportCraft, [ItemPrices-RecreateCraft
- `LogTreasureChests.cs` — **LogTreasureChests** — Commands: [logchests
- `Logging.cs` — **CommandLogging** — Command helper.
- `LootTierCommands.cs` — **LootTierCommands** — Dev commands: `[TierCommon`…`[TierPrimal`, `[TierAll`, `[TierItem`, `[LootTiers`.
- `MultiMaker.cs` — **MultiGenerator** — Commands: [MultiGen (+3 types)
- `PointLog.cs` — **PointLog** — Commands: [PointLog (+1 type)
- `Profiling.cs` — **Profiling** — Commands: [CountObjects, [DumpTimers, [ProfileWorld, [SetProfiles, [TraceExpanded, [TraceInternal (+1) (+1 type)
- `Properties.cs` — **Properties** — Commands: [Props (+15 types)
- `RecordItems.cs` — **RecordItems** — Commands: [RecordItems
- `ScanGump.cs` — **ScanGump : Gump** — Commands: [Scan (+2 types)
- `SignParser.cs` — **SignParser** — Commands: [SignGen (+1 type)
- `Skills.cs` — **SkillsCommand** — Commands: [GetSkill, [GetSkillCap, [SetAllSkills, [SetSkill, [SetSkillCap, [UseSkill (+3 types)
- `SkillsMenu.cs` — **Skills** — Commands: [Skills (+1 type)
- `SpawnerCatalog.cs` — **SpawnerCatalog** — Commands: [SpawnerCatalog
- `SpecialExport.cs` — **SpecialExporters** — Commands: [SpcEx, [SpecialExport
- `StaticExport.cs` — **Exporters** — Commands: [StaEx, [StaticExport
- `Statics.cs` — **Statics** — Commands: [Freeze, [Freeze-SetId, [FreezeMap, [FreezeWorld, [Unfreeze, [UnfreezeMap (+1) (+2 types)
- `TargetLog.cs` — **TargetLog** — Commands: [TargetLog (+1 type)
- `VisibilityList.cs` — **VisibilityList** — Commands: [Vis, [VisClear, [VisList (+1 type)
- `WandOfColors.cs` — **WandOfColors : Item** — Item ("wand of colors"). [Constructable, Flipable] (+1 type)
- `Wipe.cs` — **Wipe** — Commands: [Wipe, [WipeItems, [WipeMultis, [WipeNPCs (+1 type)

#### Scripts/System/Commands/Commands — Command modules: Commands. Types: BaseCommand, TargetCommands, InterfaceCommand.

- `BaseCommand.cs` — **BaseCommand** — Command implementation. (+2 types)
- `Commands.cs` — **TargetCommands** — Command helper.
- `Interface.cs` — **InterfaceCommand : BaseCommand** — Command implementation. (+3 types)

#### Scripts/System/Commands/Extensions — Command modules: Extensions. Types: BaseExtension, DistinctExtension, LimitExtension, SortExtension, ....

- `BaseExtension.cs` — **BaseExtension** — Command helper.
- `DistinctExtension.cs` — **DistinctExtension : BaseExtension** — Command helper.
- `LimitExtension.cs` — **LimitExtension : BaseExtension** — Command helper.
- `SortExtension.cs` — **SortExtension : BaseExtension** — Command helper.
- `WhereExtension.cs` — **WhereExtension : BaseExtension** — Command helper.

#### Scripts/System/Commands/Extensions/Compilers — Command modules: Compilers. Types: ConditionalCompiler, DistinctCompiler, SortCompiler.

- `ConditionalCompiler.cs` — **ConditionalCompiler** — Command helper.
- `DistinctCompiler.cs` — **DistinctCompiler** — Command helper.
- `SortCompiler.cs` — **SortCompiler** — Command helper.

#### Scripts/System/Commands/Implementors — Command modules: Implementors. Types: AreaCommandImplementor, BaseCommandImplementor, ContainedCommandImplementor, GlobalCommandImplementor, ....

- `AreaCommandImplementor.cs` — **AreaCommandImplementor : BaseCommandImplementor** — Command helper.
- `BaseCommandImplementor.cs` — **BaseCommandImplementor** — Command helper.
- `ContainedCommandImplementor.cs` — **ContainedCommandImplementor : BaseCommandImplementor** — Command helper.
- `GlobalCommandImplementor.cs` — **GlobalCommandImplementor : BaseCommandImplementor** — Command helper.
- `MultiCommandImplementor.cs` — **MultiCommandImplementor : BaseCommandImplementor** — Command helper.
- `ObjectConditional.cs` — **ObjectConditional** — Command helper.
- `OnlineCommandImplementor.cs` — **OnlineCommandImplementor : BaseCommandImplementor** — Command helper.
- `RegionCommandImplementor.cs` — **RegionCommandImplementor : BaseCommandImplementor** — Command helper.
- `SelfCommandImplementor.cs` — **SelfCommandImplementor : BaseCommandImplementor** — Command helper.
- `SerialCommandImplementor.cs` — **SerialCommandImplementor : BaseCommandImplementor** — Command helper.
- `SingleCommandImplementor.cs` — **SingleCommandImplementor : BaseCommandImplementor** — Command helper.

#### Scripts/System/Commands/Player — Command modules: Player. Types: AFK, AutoAttack, AutoSheatheWeapon, BandSelf, ....

- `Afk.cs` — **AFK : Timer** — Summary description for AFK.
- `AutoAttack.cs` — **AutoAttack** — Commands: [autoattack
- `AutoSheatheWeapon.cs` — **AutoSheatheWeapon** — Commands: [sheathe (+1 type)
- `BandSelf.cs` — **BandSelf** — Commands: [bandother, [bandself (+1 type)
- `BandagePacket.cs` — **BandagePacket** — Command helper.
- `Basics.cs` — **BasicsGump : Gump** — Gump UI.
- `ClassicPoisoning.cs` — **ClassicPoison** — Commands: [poisons
- `CombatBar.cs` — **CombatBar** — Commands: [combatbar (+1 type)
- `CorpseSearch.cs` — **CorpseSearch** — Commands: [corpse, [corpseclear (+4 types)
- `CreatureHelp.cs` — **CreatureHelpGump : Gump** — Gump UI.
- `Emote.cs` — **Emote** — Commands: [e, [emote (+7 types)
- `FameKarma.cs` — **FameKarma : Gump** — Gump UI.
- `ItemProps.cs` — **ItemPropsGump : Gump** — Gump UI.
- `Loot.cs` — **Loot** — Commands: [loot (+3 types)
- `MobileUO.cs` — **MobileUO** — Commands: [SuppressTooltips (+1 type)
- `MoonSearch.cs` — **MoonSearch** — Commands: [magicgate (+3 types)
- `MusicPlayer.cs` — **Musical** — Commands: [musical
- `MusicPlaylist.cs` — **MusicPlaylist : Gump** — Commands: [music (+1 type)
- `MyChat.cs` — **MyChat : Gump** — Gump UI. (+1 type)
- `MyLibrary.cs` — **MyLibrary : Gump** — Gump UI. (+1 type)
- `Organize.cs` — **OrganizeCommand : BaseCommand** — Commands: [OrganizeMe (+2 types)
- `PartyGump.cs` — **PartyGump : Gump** — Gump UI.
- `PlayBarbaric.cs` — **PlayBarbaric** — Commands: [barbaric
- `PlayEvil.cs` — **PlayEvil** — Commands: [evil
- `PlayOriental.cs` — **PlayOriental** — Commands: [oriental
- `PrivateTime.cs` — **PrivateTime** — Commands: [private
- `Quests.cs` — **QuestsGump : Gump** — Gump UI.
- `QuickBar.cs` — **QuickBar : Gump** — Commands: [quickbar (+1 type)
- `RegBar.cs` — **RegBar : Gump** — Commands: [regbar, [regclose (+1 type)
- `ReleaseSummons.cs` — **ReleaseSummons** — Commands: [ReleaseSummons
- `Rename.cs` — **RenameCommand : BaseCommand** — Command implementation.
- `SkillListing.cs` — **SkillListingGump : Gump** — Commands: [skilllist
- `SkillName.cs` — **SkillTitle** — Commands: [SkillName
- `Skills.cs` — **NewSkillsGump : Gump** — Commands: [skill
- `SkillsGump.cs` — **SkillTitleGump : Gump** — Gump UI.
- `SpellHue.cs` — **SpellHue** — Commands: [spellhue
- `VendorGold.cs` — **VendorGold** — Commands: [VendorGold
- `Wanted.cs` — **Wanted : Gump** — Gump UI.
- `WealthBar.cs` — **WealthBar : Gump** — Commands: [wealth

#### Scripts/System/Gumps — Server-side gump UIs.

- `AddGump.cs` — **AddGump : Gump** — Commands: [AddMenu (+2 types)
- `BanDurationGump.cs` — **BanDurationGump : Gump** — Gump UI.
- `BaseConfirmGump.cs` — **BaseConfirmGump : Gump** — Gump UI. (+1 type)
- `BaseGridGump.cs` — **BaseGridGump : Gump** — Gump UI.
- `BaseImageTileButtonsGump.cs` — **BaseImageTileButtonsGump : Gump** — Gump UI. (+1 type)
- `CategorizedAddGump.cs` — **CategorizedAddGump : Gump** — Gump UI. (+3 types)
- `ClientGump.cs` — **ClientGump : Gump** — Gump UI.
- `ClueGump.cs` — **ClueGump : Gump** — Gump UI.
- `ConfirmBreakCrystalGump.cs` — **ConfirmBreakCrystalGump : BaseConfirmGump** — Gump component.
- `ConfirmHouseResize.cs` — **ConfirmHouseResize : Gump** — Gump UI.
- `ConfirmReleaseGump.cs` — **ConfirmReleaseGump : Gump** — Gump UI.
- `ConfirmationGump.cs` — **ConfirmationGump : Gump** — Gump UI.
- `GumpColors.cs` — **HtmlColors** — Gump component.
- `HouseDemolishGump.cs` — **HouseDemolishGump : Gump** — Gump UI.
- `HouseGump.cs` — **HouseGump : Gump** — Gump UI. (+3 types)
- `HouseGumpAOS.cs` — **HouseGumpAOS : Gump** — Gump UI. (+1 type)
- `HouseTransferGump.cs` — **HouseTransferGump : Gump** — Gump UI.
- `MannequinGumps.cs` — **MannequinOwnerGump : Gump** — Gump UI. (+1 type)
- `NoticeGump.cs` — **NoticeGump : Gump** — Gump UI.
- `PetResurrectGump.cs` — **PetResurrectGump : Gump** — Gump UI.
- `PlayerVendorGumps.cs` — **PlayerVendorBuyGump : Gump** — Gump UI. (+10 types)
- `PolymorphGump.cs` — **PolymorphGump : Gump** — Gump UI. (+3 types)
- `PrisonGump.cs` — **PrisonGump : Gump** — Gump UI.
- `ReclaimVendorGump.cs` — **ReclaimVendorGump : Gump** — Gump UI.
- `ReportMurderer.cs` — **ReportMurdererGump : Gump** — Gump UI. (+1 type)
- `ResurrectGump.cs` — **ResurrectGump : Gump** — Gump UI. (+1 type)
- `RunebookGump.cs` — **RunebookGump : Gump** — Gump UI. (+5 types)
- `SetSecureLevelGump.cs` — **SetSecureLevelGump : Gump** — Gump UI. (+1 type)
- `SkillsGump.cs` — **SkillsGump : Gump** — Gump UI. (+3 types)
- `TithingGump.cs` — **TithingGump : Gump** — Gump UI.
- `VendorContainerSellConfigGump.cs` — **VendorContainerSellConfigGump : Gump** — Gump UI. (+1 type)
- `VendorContainerSellGump.cs` — **VendorContainerSellGump : Gump** — Gump UI.
- `VendorContainerSellItemsPerPagePrompt.cs` — **VendorContainerSellItemsPerPagePrompt : Prompt** — Gump component.
- `VendorInventoryGump.cs` — **VendorInventoryGump : Gump** — Gump UI.
- `VendorRentalGumps.cs` — **BaseVendorRentalGump : Gump** — Gump UI. (+9 types)
- `ViewHousesGump.cs` — **ViewHousesGump : Gump** — Commands: [ViewHouses (+1 type)
- `WarningGump.cs` — **WarningGump : Gump** — Gump UI.
- `WhoGump.cs` — **WhoGump : Gump** — Commands: [Who, [WhoList (+1 type)
- `YoungGumps.cs` — **YoungDungeonWarning : Gump** — Gump UI. (+2 types)

#### Scripts/System/Gumps/Go — Gump UI: Go. Types: ChildNode, GoGump, LocationTree, ParentNode.

- `ChildNode.cs` — **ChildNode** — Gump component.
- `GoGump.cs` — **GoGump : Gump** — Gump UI.
- `LocationTree.cs` — **LocationTree** — Gump component.
- `ParentNode.cs` — **ParentNode** — Gump component.

#### Scripts/System/Gumps/Guilds — Gump UI: Guilds. Types: DeclareFealtyGump, GrantGuildTitleGump, GuildAbbrvPrompt, GuildAcceptWarGump, ....

- `DeclareFealtyGump.cs` — **DeclareFealtyGump : GuildMobileListGump** — Gump component.
- `GrantGuildTitleGump.cs` — **GrantGuildTitleGump : GuildMobileListGump** — Gump component.
- `GuildAbbrvPrompt.cs` — **GuildAbbrvPrompt : Prompt** — Gump component.
- `GuildAcceptWarGump.cs` — **GuildAcceptWarGump : GuildListGump** — Gump component.
- `GuildAdminCandidatesGump.cs` — **GuildAdminCandidatesGump : GuildMobileListGump** — Gump component.
- `GuildCandidatesGump.cs` — **GuildCandidatesGump : GuildMobileListGump** — Gump component.
- `GuildChangeTypeGump.cs` — **GuildChangeTypeGump : Gump** — Gump UI.
- `GuildCharterGump.cs` — **GuildCharterGump : Gump** — Gump UI.
- `GuildCharterPrompt.cs` — **GuildCharterPrompt : Prompt** — Gump component.
- `GuildDeclarePeaceGump.cs` — **GuildDeclarePeaceGump : GuildListGump** — Gump component.
- `GuildDeclareWarGump.cs` — **GuildDeclareWarGump : GuildListGump** — Gump component.
- `GuildDeclareWarPrompt.cs` — **GuildDeclareWarPrompt : Prompt** — Gump component.
- `GuildDismissGump.cs` — **GuildDismissGump : GuildMobileListGump** — Gump component.
- `GuildGump.cs` — **GuildGump : Gump** — Gump UI.
- `GuildListGump.cs` — **GuildListGump : Gump** — Gump UI.
- `GuildMobileListGump.cs` — **GuildMobileListGump : Gump** — Gump UI.
- `GuildNamePrompt.cs` — **GuildNamePrompt : Prompt** — Gump component.
- `GuildRejectWarGump.cs` — **GuildRejectWarGump : GuildListGump** — Gump component.
- `GuildRescindDeclarationGump.cs` — **GuildRescindDeclarationGump : GuildListGump** — Gump component.
- `GuildRosterGump.cs` — **GuildRosterGump : GuildMobileListGump** — Gump component.
- `GuildTitlePrompt.cs` — **GuildTitlePrompt : Prompt** — Gump component.
- `GuildWarAdminGump.cs` — **GuildWarAdminGump : Gump** — Gump UI.
- `GuildWarGump.cs` — **GuildWarGump : Gump** — Gump UI.
- `GuildWebsitePrompt.cs` — **GuildWebsitePrompt : Prompt** — Gump component.
- `GuildmasterGump.cs` — **GuildmasterGump : Gump** — Gump UI.
- `RecruitTarget.cs` — **GuildRecruitTarget : Target** — Targeting prompt.

#### Scripts/System/Gumps/Guilds/New Guild System — Gump UI: New guild system. Types: GuildAdvancedSearchGump, BaseGuildGump, BaseGuildListGump, CreateGuildGump, ....

- `AdvancedSearch.cs` — **GuildAdvancedSearchGump : BaseGuildGump** — Gump component.
- `BaseGuildGump.cs` — **BaseGuildGump : Gump** — Gump UI.
- `BaseGuildListGump.cs` — **BaseGuildListGump** — Gump component.
- `Create Guild Gump.cs` — **CreateGuildGump : Gump** — Gump UI.
- `DiplomacyGump.cs` — **GuildDisplayType** — Enum with about 3 members. (+5 types)
- `GuildInfoGump.cs` — **GuildInfoGump : BaseGuildGump** — Gump component.
- `GuildInvitationRequest.cs` — **GuildInvitationRequest : BaseGuildGump** — Gump component.
- `GuildMemberInfoGump.cs` — **GuildMemberInfoGump : BaseGuildGump** — Gump component.
- `GuildRosterGump.cs` — **GuildRosterGump : BaseGuildListGump** — Gump component.
- `OtherGuildInfo.cs` — **OtherGuildInfo : BaseGuildGump** — Info/data container.
- `War Declaration gump.cs` — **WarDeclarationGump : BaseGuildGump** — Gump component.

#### Scripts/System/Gumps/Properties — Gump UI: Properties. Types: PropsConfig, PropertiesGump, SetBodyGump, SetCustomEnumGump, ....

- `PropsConfig.cs` — **PropsConfig** — Gump component.
- `PropsGump.cs` — **PropertiesGump : Gump** — Gump UI. (+3 types)
- `SetBodyGump.cs` — **SetBodyGump : Gump** — Gump UI. (+1 type)
- `SetCustomEnumGump.cs` — **SetCustomEnumGump : SetListOptionGump** — Gump component.
- `SetGump.cs` — **SetGump : Gump** — Gump UI. (+1 type)
- `SetListOptionGump.cs` — **SetListOptionGump : Gump** — Gump UI.
- `SetObjectGump.cs` — **SetObjectGump : Gump** — Gump UI. (+1 type)
- `SetObjectTarget.cs` — **SetObjectTarget : Target** — Targeting prompt.
- `SetPoint2DGump.cs` — **SetPoint2DGump : Gump** — Gump UI. (+1 type)
- `SetPoint3DGump.cs` — **SetPoint3DGump : Gump** — Gump UI. (+1 type)
- `SetTimeSpanGump.cs` — **SetTimeSpanGump : Gump** — Gump UI.

#### Scripts/System/Help — Help gump, settings UI and player preference pages.

- `HelpGump.cs` — **HelpGump : Gump** — Help gump: main pages, settings pages, toggles and info panels.
- `HelpGump_Preferences.cs` — **HelpGump** — class (~1 public members).
- `HelpGump_Settings.cs` — **HelpGump** — MobileUO settings layout (loot tiers, worth, preferences).
- `InfoHelpGump.cs` — **InfoHelpGump : Gump** — Gump UI.
- `PagePrompt.cs` — **PagePrompt : Prompt** — class (~4 public members).
- `PagePromptGump.cs` — **PagePromptGump : Gump** — Gump UI.
- `PageQueue.cs` — **PageQueue** — Commands: [Pages (+3 types)
- `PageQueueGump.cs` — **PageQueueGump : Gump** — Gump UI. (+4 types)
- `PageResponseGump.cs` — **PageResponseGump : Gump** — Gump UI.
- `SpeechLog.cs` — **SpeechLog : IEnumerable** — Commands: [SpeechLog (+2 types)
- `SpeechLogGump.cs` — **SpeechLogGump : Gump** — Gump UI.
- `StuckMenu.cs` — **StuckMenu : Gump** — Gump UI. (+3 types)

#### Scripts/System/Misc — Cross-cutting systems (tooltips/worth/tiers, item sales, settings, random things).

- `AOS.cs` — **AOS** — AOS-era helpers and property plumbing.
- `Accounts.cs` — **Accounts** — Commands: [Password (+16 types)
- `Animations.cs` — **Animations** — class (~2 public members).
- `AttackMessage.cs` — **AttackMessage** — class (~4 public members).
- `AutoRestart.cs` — **AutoRestart : Timer** — Commands: [Restart
- `AutoSave.cs` — **AutoSave : Timer** — Commands: [SetSaves
- `Broadcast.cs` — **Announce** — class (~3 public members).
- `Broadcasts.cs` — **Broadcasts** — class (~4 public members).
- `BuffIcons.cs` — **BuffInfo** — Info/data container. (+3 types)
- `Build.cs` — **BuildQuestItems** — Commands: [BuildWorld, [Decorate (+8 types)
- `Captcha.cs` — **CaptchaGump : Gump** — Gump UI.
- `ChangeLog.cs` — **ChangeLog** — class (~3 public members).
- `CharacterCreation.cs` — **CharacterCreation** — class (~10 public members). (+1 type)
- `Cleanup.cs` — **Cleanup** — class (~9 public members).
- `ClientVerification.cs` — **ClientVerification** — class (~7 public members). (+1 type)
- `Console.cs` — **ServerConsole** — class (~7 public members).
- `ContextMenu.cs` — **AddToSpellbookEntry : ContextMenuEntry** — Context-menu entry. (+10 types)
- `CrashGuard.cs` — **CrashGuard** — class (~3 public members).
- `CurrentExpansion.cs` — **CurrentExpansion** — class (~2 public members).
- `CustomEventSink.cs` — **CustomEventSink** — class (~66 public members). (+9 types)
- `DataPath.cs` — **DataPath** — class (~2 public members).
- `Death.cs` — **Death** — class (~24 public members). (+7 types)
- `DispellableAttribute.cs` — **DispellableAttribute : Attribute** — class (~1 public members).
- `DispellableFieldAttribute.cs` — **DispellableFieldAttribute : Attribute** — class (~1 public members).
- `DoorGenerator.cs` — **DoorGenerator** — Commands: [DoorGen
- `Email.cs` — **Email** — class (~11 public members).
- `Emitter.cs` — **AssemblyEmitter** — class (~54 public members). (+2 types)
- `Environment.cs` — **ComputerBeeps : Item** — Item ("lightning crackle"). [Constructable] (+9 types)
- `Fastwalk.cs` — **Fastwalk** — class (~3 public members).
- `FoodDecay.cs` — **FoodDecayTimer : Timer** — Timer.
- `Geometry.cs` — **Geometry** — Static class (~16 public members). (+1 type)
- `Guilds.cs` — **RankFlags** — Commands: [GuildProps (+13 types)
- `HardwareInfo.cs` — **HardwareInfo** — Commands: [HWInfo
- `InhumanSpeech.cs` — **InhumanSpeech** — class (~23 public members). (+1 type)
- `ItemProperties.cs` — **ItemProps** — class (~20 public members).
- `ItemRank.cs` — **ItemRank** — Loot tiers (Common→Artefact), tier name/text colours, quiver header hue, artefact ground label hue, wrappers.
- `ItemSales.cs` — **ItemInformation** — ItemInformation: sell/buy tables, AddUpBenefits property pricing, buy lists, vendor deal rules.
- `ItemSkills.cs` — **ItemSkills** — class (~8 public members).
- `Keywords.cs` — **Keywords** — class (~3 public members).
- `LanguageStatistics.cs` — **LanguageStatistics** — Commands: [LanguageStatistics (+3 types)
- `LightCycle.cs` — **LightCycle** — Commands: [GlobalLight (+2 types)
- `LoginStats.cs` — **LoginStats** — class (~2 public members).
- `Logs.cs` — **LogEventType** — Enum with about 7 members. (+3 types)
- `MOTD.cs` — **MOTD_Gump : Gump** — Commands: [MOTD (+4 types)
- `MapDefinitions.cs` — **MapDefinitions** — class (~3 public members).
- `MorphingTime.cs` — **MorphingTime** — Commands: [HueGear (+1 type)
- `Naming.cs` — **NameList** — Commands: [ValidateName (+7 types)
- `Notoriety.cs` — **NotorietyHandlers** — class (~12 public members). (+1 type)
- `Paperdoll.cs` — **Paperdoll** — class (~3 public members).
- `Party.cs` — **Party : IParty** — Commands: [ListenToParty (+11 types)
- `Paths.cs` — **PathNode** — Commands: [Path (+8 types)
- `PlayMusicOnLogin.cs` — **PlayMusicOnLogin** — Static system module (Initialize). (+1 type)
- `Players.cs` — **GetPlayerInfo** — Commands: [status (+3 types)
- `Poison.cs` — **PoisonImpl : Poison** — class (~10 public members). (+1 type)
- `ProfanityProtection.cs` — **ProfanityProtection** — class (~6 public members). (+1 type)
- `Profile.cs` — **Profile** — class (~5 public members).
- `ProtocolExtensions.cs` — **ProtocolExtensions** — Abstract class (~10 public members). (+2 types)
- `RaceDefinitions.cs` — **RaceDefinitions** — class (~21 public members). (+2 types)
- `RandomThings.cs` — **RandomThings** — class (~53 public members).
- `RegenRates.cs` — **RegenRates** — class (~3 public members).
- `Remote.cs` — **AdminNetwork** — class (~38 public members). (+11 types)
- `RenameRequests.cs` — **RenameRequests** — class (~2 public members).
- `Reporting.cs` — **Reports** — Strongly typed collection of Server.Engines.Reports.PersistableObject. (+52 types)
- `ResourceInfo.cs` — **CraftResourceType** — Enum with about 10 members. (+3 types)
- `ResourceMods.cs` — **ResourceMods** — class (~100 public members). (+2 types)
- `ServerList.cs` — **ServerList** — class (~11 public members).
- `Settings.cs` — **MyServerSettings** — Settings holder.
- `ShardPoller.cs` — **ShardPoller : Item** — Item. [Constructable] (+3 types)
- `ShrinkTable.cs` — **ShrinkTable** — class (~6 public members).
- `SocketOptions.cs` — **SocketOptions** — class (~2 public members).
- `Statistics.cs` — **Statistics** — Commands: [Statistics, [UpdateStatistics (+2 types)
- `Talk.cs` — **SpeechGump : Gump** — Gump UI. (+2 types)
- `Targeting.cs` — **BladedItemTarget : Target** — Targeting prompt. (+2 types)
- `TextDefinition.cs` — **TextDefinition** — class (~23 public members).
- `Titles.cs` — **Titles** — class (~17 public members). (+2 types)
- `UnhandledPackets.cs` — **UnhandledPackets** — class (~2 public members).
- `ValidationQueue.cs` — **ValidationQueue** — Static system module (Initialize). (+2 types)
- `VendorValue.cs` — **VendorValue** — Per-player **Worth** line: relic/unidentified pricing, Show Worth toggle, tooltip refresh, 0xD6/0xBF handlers, legacy relic worth suppression.
- `Weather.cs` — **Weather** — Commands: [weather (+1 type)
- `WeightOverloading.cs` — **WeightOverloading** — class (~10 public members). (+1 type)
- `WelcomeTimer.cs` — **WelcomeTimer : Timer** — Timer.

#### Scripts/System/Network — Packets, packet handlers, networking extensions.

- `DynamicMapDefinitions.cs` — **DynamicMapDefinitions** — class (~5 public members). (+1 type)

#### Scripts/System/Regions — Custom regions and region logic.

- `BardDungeonRegion.cs` — **BardDungeonRegion : BaseRegion** — class (~6 public members).
- `BardTownRegion.cs` — **BardTownRegion : BaseRegion** — class (~5 public members).
- `BargeDeadRegion.cs` — **BargeDeadRegion : BaseRegion** — class (~6 public members).
- `CaveRegion.cs` — **CaveRegion : BaseRegion** — class (~6 public members).
- `CrashRegion.cs` — **CrashRegion : BaseRegion** — class (~7 public members).
- `DawnRegion.cs` — **DawnRegion : BaseRegion** — class (~5 public members).
- `DeadRegion.cs` — **DeadRegion : BaseRegion** — class (~6 public members).
- `DungeonHomeRegion.cs` — **DungeonHomeRegion : BaseRegion** — class (~6 public members).
- `DungeonRegion.cs` — **DungeonRegion : BaseRegion** — class (~9 public members).
- `GargoyleRegion.cs` — **GargoyleRegion : BaseRegion** — class (~6 public members).
- `GuardedRegion.cs` — **GuardedRegion : BaseRegion** — class (~2 public members).
- `LunaRegion.cs` — **LunaRegion : BaseRegion** — class (~5 public members).
- `MazeRegion.cs` — **MazeRegion : BaseRegion** — class (~5 public members).
- `MoonCore.cs` — **MoonCore : BaseRegion** — class (~7 public members).
- `NecromancerRegion.cs` — **NecromancerRegion : BaseRegion** — class (~7 public members).
- `NoHousingRegion.cs` — **NoHousingRegion : BaseRegion** — class (~4 public members).
- `OutDoorBadRegion.cs` — **OutDoorBadRegion : BaseRegion** — class (~6 public members).
- `OutDoorRegion.cs` — **OutDoorRegion : BaseRegion** — class (~5 public members).
- `PirateRegion.cs` — **PirateRegion : BaseRegion** — class (~4 public members).
- `PrisonArea.cs` — **PrisonArea : BaseRegion** — class (~8 public members).
- `ProtectedRegion.cs` — **ProtectedRegion : BaseRegion** — class (~8 public members).
- `PublicRegion.cs` — **PublicRegion : BaseRegion** — class (~9 public members).
- `SafeRegion.cs` — **SafeRegion : BaseRegion** — class (~8 public members).
- `SavageRegion.cs` — **SavageRegion : BaseRegion** — class (~8 public members).
- `SeaSpawnRegion.cs` — **SeaSpawnRegion : BaseRegion** — class (~2 public members).
- `SkyHomeRegion.cs` — **SkyHomeDwelling : BaseRegion** — class (~5 public members).
- `StartRegion.cs` — **StartRegion : BaseRegion** — class (~9 public members).
- `TownRegion.cs` — **TownRegion : GuardedRegion** — class (~3 public members).
- `UmbraRegion.cs` — **UmbraRegion : BaseRegion** — class (~6 public members).
- `UnderHouseRegion.cs` — **UnderHouseRegion : BaseRegion** — class (~7 public members).
- `VillageRegion.cs` — **VillageRegion : BaseRegion** — class (~6 public members).
- `WantedRegion.cs` — **WantedRegion : BaseRegion** — class (~8 public members).

#### Scripts/System/Regions/Core — Support system: Core. Types: BaseRegion, HouseRegion, Jail, RegionMusic, ....

- `BaseRegion.cs` — **BaseRegion : Region** — class (~25 public members). (+1 type)
- `HouseRegion.cs` — **HouseRegion : BaseRegion** — class (~23 public members).
- `Jail.cs` — **Jail : BaseRegion** — class (~2 public members).
- `RegionMusic.cs` — **RegionMusic** — class (~16 public members).
- `World.cs` — **Worlds** — Commands: [world (+1 type)

#### Scripts/System/Regions/Spawning — Support system: Spawning. Types: SpawnDefinition, SpawnEntry, SpawnPersistence.

- `SpawnDefinition.cs` — **SpawnDefinition** — Abstract class (~37 public members). (+6 types)
- `SpawnEntry.cs` — **SpawnEntry : ISpawner** — Commands: [DelAllRegionSpawns, [DelRegionSpawns, [RespawnAllRegions, [RespawnRegion, [StartAllRegionSpawns, [StartRegionSpawns (+2)
- `SpawnPersistence.cs` — **SpawnPersistence : Item** — Item.

#### Scripts/System/Skills — Skill systems and skill-specific mechanics.

- `Anatomy.cs` — **Anatomy** — class (~4 public members). (+1 type)
- `ArmsLore.cs` — **ArmsLore** — class (~5 public members). (+1 type)
- `Begging.cs` — **Begging** — class (~11 public members). (+2 types)
- `Discordance.cs` — **Discordance** — class (~19 public members). (+3 types)
- `Druidism.cs` — **Druidism** — class (~7 public members). (+3 types)
- `Forensics.cs` — **Forensics** — class (~5 public members). (+1 type)
- `Healing.cs` — **Healing** — class (~3 public members).
- `Hiding.cs` — **Hiding** — class (~4 public members).
- `Inscribe.cs` — **Inscribe** — class (~8 public members). (+2 types)
- `Meditation.cs` — **Meditation** — class (~3 public members).
- `Mercantile.cs` — **ItemIdentification** — class (~4 public members). (+1 type)
- `Parrying.cs` — **Parrying** — class (~4 public members).
- `Peacemaking.cs` — **Peacemaking** — class (~5 public members). (+1 type)
- `Poisoning.cs` — **Poisoning** — class (~6 public members). (+3 types)
- `Provocation.cs` — **Provocation** — class (~6 public members). (+2 types)
- `Psychology.cs` — **Psychology** — class (~4 public members). (+1 type)
- `RemoveTrap.cs` — **RemoveTrap** — class (~7 public members). (+2 types)
- `Searching.cs` — **Searching** — class (~6 public members). (+1 type)
- `SkillCheck.cs` — **SkillCheck** — class (~17 public members). (+1 type)
- `Snooping.cs` — **Snooping** — class (~4 public members).
- `Spiritualism.cs` — **Spiritualism** — class (~17 public members). (+2 types)
- `Stealing.cs` — **Stealing** — class (~22 public members). (+2 types)
- `Stealth.cs` — **Stealth** — class (~4 public members).
- `Taming.cs` — **Taming** — Static system module (Initialize). (+4 types)
- `Tasting.cs` — **Tasting** — class (~4 public members). (+1 type)
- `Tracking.cs` — **Tracking** — class (~28 public members). (+6 types)

#### Scripts/System/Skills/Weapon Abilities — Skill system: Weapon abilities. Types: WeaponAbilityBook, ArmorIgnore, ArmorPierce, Bladeweave, ....

- `AbilityBook.cs` — **WeaponAbilityBook : Item** — Item ("Weapon Abilities"). [Constructable] (+1 type)
- `ArmorIgnore.cs` — **ArmorIgnore : WeaponAbility** — This special move allows the skilled warrior to bypass his target's physical resistance, for one shot only....
- `ArmorPierce.cs` — **ArmorPierce : WeaponAbility** — Strike your opponent with great force, partially bypassing their armor and inflicting greater damage.
- `Bladeweave.cs` — **Bladeweave : WeaponAbility** — class (~11 public members). (+1 type)
- `BleedAttack.cs` — **BleedAttack : WeaponAbility** — Make your opponent bleed profusely with this wicked use of your weapon. When successful, the target will bl... (+1 type)
- `Block.cs` — **Block : WeaponAbility** — Raises your defenses for a short time. (+2 types)
- `ConcussionBlow.cs` — **ConcussionBlow : WeaponAbility** — This devastating strike is most effective against those who are in good health and whose reserves of mana a...
- `CrushingBlow.cs` — **CrushingBlow : WeaponAbility** — Also known as the Haymaker, this attack dramatically increases the damage done by a weapon reaching its mark.
- `CustomWeaponAbilities.cs` — **CustomWeaponAbilities** — Static system module (Initialize).
- `DefenseMastery.cs` — **DefenseMastery : WeaponAbility** — Raises your physical resistance for a short time while lowering your ability to inflict damage. (+1 type)
- `Disarm.cs` — **Disarm : WeaponAbility** — This attack allows you to disarm your foe. Now in Age of Shadows, a successful Disarm leaves the victim una...
- `Dismount.cs` — **Dismount : WeaponAbility** — Perfect for the foot-soldier, the Dismount special attack can unseat a mounted opponent. The fighter using...
- `Disrobe.cs` — **Disrobe : WeaponAbility** — This attack allows you to disrobe your foe.
- `DoubleShot.cs` — **DoubleShot : WeaponAbility** — class (~7 public members).
- `DoubleStrike.cs` — **DoubleStrike : WeaponAbility** — The highly skilled warrior can use this special attack to make two quick swings in succession. Landing both...
- `DualWield.cs` — **DualWield : WeaponAbility** — Attack faster as you swing with both weapons. (+1 type)
- `Feint.cs` — **Feint : WeaponAbility** — Gain a defensive advantage over your primary opponent for a short time. (+1 type)
- `ForceArrow.cs` — **ForceArrow : WeaponAbility** — class (~5 public members).
- `ForceofNature.cs` — **ForceOfNature : WeaponAbility** — class (~6 public members). (+1 type)
- `FrenziedWhirlwind.cs` — **FrenziedWhirlwind : WeaponAbility** — A quick attack to all enemies in range of your weapon that causes damage over time. (+1 type)
- `InfectiousStrike.cs` — **InfectiousStrike : WeaponAbility** — This special move represents a significant change to the use of poisons in Age of Shadows. Now, only certai...
- `LightningArrow.cs` — **LightningArrow : WeaponAbility** — class (~4 public members).
- `MortalStrike.cs` — **MortalStrike : WeaponAbility** — The assassin's friend. A successful Mortal Strike will render its victim unable to heal any damage for seve... (+1 type)
- `MovingShot.cs` — **MovingShot : WeaponAbility** — Available on some crossbows, this special move allows archers to fire while on the move. This shot is somew...
- `NerveStrike.cs` — **NerveStrike : WeaponAbility** — Does damage and paralyses your opponent for a short time.
- `ParalyzingBlow.cs` — **ParalyzingBlow : WeaponAbility** — A successful Paralyzing Blow will leave the target stunned, unable to move, attack, or cast spells, for a f... (+1 type)
- `PsychicAttack.cs` — **PsychicAttack : WeaponAbility** — class (~4 public members).
- `RidingSwipe.cs` — **RidingSwipe : WeaponAbility** — If you are on foot, dismounts your opponent and damage the ethereal's rider or the living mount(which must...
- `SerpentArrow.cs` — **SerpentArrow : WeaponAbility** — class (~4 public members).
- `ShadowStrike.cs` — **ShadowStrike : WeaponAbility** — This powerful ability requires secondary skills to activate. Successful use of Shadowstrike deals extra dam...
- `SpecialAttackCommands.cs` — **SpecialAttackCommands** — Commands: [Set1, [Set2, [Set3, [Set4, [Set5, [SetFifthAbility (+4)
- `SpecialAttackGump.cs` — **SpecialAttackGump : Gump** — Commands: [abilitynames (+1 type)
- `SpecialAttacksDisplay.cs` — **SpecialAttacksDisplayCommand** — Commands: [SAD, [SpecialAttacksDisplay
- `TalonStrike.cs` — **TalonStrike : WeaponAbility** — Attack with increased damage with additional damage over time. (+1 type)
- `WeaponAbility.cs` — **WeaponAbility** — Abstract class (~84 public members). (+2 types)
- `WeaponArmorCalls.cs` — **GetTrueBestSkill** — class (~21 public members). (+5 types)
- `WhirlwindAttack.cs` — **WhirlwindAttack : WeaponAbility** — A godsend to a warrior surrounded, the Whirlwind Attack allows the fighter to strike at all nearby targets...

#### Scripts/System/Skills/Weapon Abilities/Extra — Skill system: Extra. Types: AchillesStrike, ConsecratedStrike, DeathBlow, DevastatingBlow, ....

- `AchiliesStrike.cs` — **AchillesStrike : WeaponAbility** — class (~4 public members).
- `ConsecratedStrike.cs` — **ConsecratedStrike : WeaponAbility** — class (~5 public members).
- `DeathBlow.cs` — **DeathBlow : WeaponAbility** — class (~5 public members).
- `DevestatingBlow.cs` — **DevastatingBlow : WeaponAbility** — class (~6 public members).
- `DoubleWhirlwindAttack.cs` — **DoubleWhirlwindAttack : WeaponAbility** — class (~4 public members).
- `EarthStrike.cs` — **EarthStrike : WeaponAbility** — class (~4 public members).
- `ElementalStrike.cs` — **ElementalStrike : WeaponAbility** — class (~4 public members).
- `FireStrike.cs` — **FireStrike : WeaponAbility** — class (~4 public members).
- `FistsOfFury.cs` — **FistsOfFury : WeaponAbility** — class (~6 public members).
- `FreezeStrike.cs` — **FreezeStrike : WeaponAbility** — class (~4 public members).
- `LightningStrike.cs` — **LightningStriker : WeaponAbility** — class (~4 public members).
- `MagicProtection.cs` — **MagicProtection : WeaponAbility** — class (~5 public members).
- `MagicProtection2.cs` — **MagicProtection2 : WeaponAbility** — class (~5 public members).
- `MeleeProtection.cs` — **MeleeProtection : WeaponAbility** — class (~5 public members).
- `MeleeProtection2.cs` — **MeleeProtection2 : WeaponAbility** — class (~5 public members).
- `RidingAttack.cs` — **RidingAttack : WeaponAbility** — class (~5 public members).
- `ShadowInfectiousStrike.cs` — **ShadowInfectiousStrike : WeaponAbility** — class (~4 public members).
- `SpinAttack.cs` — **SpinAttack : WeaponAbility** — class (~6 public members).
- `StunningStrike.cs` — **StunningStrike : WeaponAbility** — class (~4 public members).
- `ToxicStrike.cs` — **ToxicStrike : WeaponAbility** — class (~4 public members).
- `ZapDexStrike.cs` — **ZapDexStrike : WeaponAbility** — class (~4 public members).
- `ZapIntStrike.cs` — **ZapIntStrike : WeaponAbility** — class (~4 public members).
- `ZapManaStrike.cs` — **ZapManaStrike : WeaponAbility** — class (~4 public members).
- `ZapStamStrike.cs` — **ZapStamStrike : WeaponAbility** — class (~4 public members).
- `ZapStrStrike.cs` — **ZapStrStrike : WeaponAbility** — class (~4 public members).

#### Scripts/System/Spawns — Spawners and spawn management.

- `Commands.cs` — **OptionsGumps** — Commands: [GumpRemoveCoordinate, [GumpRemoveID, [GumpRemoveRegion, [GumpSaveCoordinate, [GumpSaveRegion, [RSE (+1) (+7 types)
- `Gumps.cs` — **PremiumSpawnerGump : Gump** — Commands: [Editor, [SpawnEditor (+2 types)
- `ReadMe.cs` — **ReadMe** — Readme for the Premium Spawner derived from Ultimate Spawner; explains spawn properties/engine.
- `SpawnGen.cs` — **SpawnGenerator** — Commands: [SpawnGen
- `SpawnLists.cs` — **SpawnList** — class (~6 public members).
- `SpawnMan.cs` — **SpawnMan : BaseCreature** — Creature (AI_Melee, Closest). [Constructable] (+2 types)
- `SpawnRef.cs` — **BaseSpawner : Item** — Item ("spawn ref"). [Constructable] (+8 types)
- `Spawner.cs` — **PremiumSpawner : Item** — Item ("PremiumSpawner"). (+3 types)
- `SpawnerClassic.cs` — **Spawner : Item** — Item. [Constructable] (+4 types)

#### Scripts/System/Tasks — Support system: Tasks. Types: TaskManager150Min, TaskManager200Min, TaskManager250Min, TaskManager60Min, ....

- `TaskManager150Min.cs` — **TaskManager150Min : Item** — Item ("Task Manager 150 Min"). [Constructable] (+2 types)
- `TaskManager200Min.cs` — **TaskManager200Min : Item** — Item ("Task Manager 200 Minutes"). [Constructable] (+2 types)
- `TaskManager250Min.cs` — **TaskManager250Min : Item** — Item ("Task Manager 250 Minutes"). [Constructable] (+2 types)
- `TaskManager60Min.cs` — **TaskManager60Min : Item** — Item ("Task Manager 1 Hour"). [Constructable] (+3 types)
- `TaskManagerDaily.cs` — **TaskManagerDaily : Item** — Item ("Task Manager Daily"). [Constructable] (+2 types)

#### Scripts/Systems/Transmogrify — Transmogrify. Types: TransmogrifyCatalog, TransmogrifyFees, TransmogrifyGem, TransmogrifyGump, ....

- `TransmogrifyCatalog.cs` — **TransmogrifyCatalog** — Static class (~14 public members). [Constructable] (+1 type)
- `TransmogrifyFees.cs` — **TransmogrifyFees** — Static class (~6 public members).
- `TransmogrifyGem.cs` — **TransmogrifyGem : TransmogrifyTool** — Static system module (Initialize). [Constructable] (+1 type)
- `TransmogrifyGump.cs` — **TransmogrifyGump : Gump** — Gump UI.
- `TransmogrifyPreview.cs` — **TransmogrifyPreview : Mobile** — Mobile. (+1 type)
- `TransmogrifyRegistry.cs` — **TransmogrifyRegistry** — Static system module (Initialize). (+1 type)
- `TransmogrifyTestItem.cs` — **TransmogrifyTestItem : TransmogrifyTool** — class (~5 public members). [Constructable]
- `TransmogrifyTool.cs` — **TransmogrifyTool : Item** — Item. (+1 type)

#### Scripts/Timers — Timers. Types: GoodiesTimer, RepeatableAction.

- `GoodiesTimer.cs` — **GoodiesTimer : Timer** — Timer.
- `RepeatableActions.cs` — **RepeatableAction** — class (~10 public members). (+2 types)

#### Scripts/Utilities — Shared helpers, constants and UTC utilities.

- `ArtifactQuestList.cs` — **ArtifactQuestList** — Static class (~3 public members).
- `GumpUtilities.cs` — **GumpUtilities** — Static class (~3 public members).
- `HueUtilities.cs` — **HueUtilities** — Static class (~3 public members).
- `ItemUtilities.cs` — **ItemUtilities** — Static class (~8 public members).
- `MobileUtilities.cs` — **MobileUtilities** — class (~5 public members).
- `SkillUtilities.cs` — **SkillUtilities** — Static class (~4 public members).
- `TypeUtilities.cs` — **TypeUtilities** — Static class (~6 public members).
- `UnlockUtilities.cs` — **UnlockUtilities** — Static class (~68 public members). (+7 types)
- `WorldUtilities.cs` — **WorldUtilities** — Static class (~23 public members).

#### Scripts/Utilities/Constants — Constants. Types: EmoteHues.

- `EmoteHues.cs` — **EmoteHues** — Static class (~20 public members).

### Core

#### Core — RunUO/Memento core compiled into WorldLinux.exe.

- `AggressorInfo.cs` — **AggressorInfo** — Info/data container.
- `AssemblyInfo.cs` — **AssemblyInfo** — Assembly metadata (name/version).
- `Attributes.cs` — **HueAttribute : Attribute** — class (~33 public members). (+11 types)
- `BaseVendor.cs` — **BuyItemStateComparer : IComparer** — class (~24 public members). (+4 types)
- `Body.cs` — **Body** — class (~25 public members). (+1 type)
- `CliLoc.cs` — **CliLocTable** — class (~2 public members).
- `ClientVersion.cs` — **ClientVersion : IComparable** — class (~24 public members). (+1 type)
- `Commands.cs` — **CommandEventArgs : EventArgs** — Static class (~27 public members). (+2 types)
- `Effects.cs` — **Effects** — Static class (~31 public members). (+2 types)
- `EventLog.cs` — **EventLog** — Static class (~7 public members).
- `EventSink.cs` — **EventSink** — Static class (~357 public members). (+45 types)
- `ExpansionInfo.cs` — **ExpansionInfo** — Info/data container. (+4 types)
- `Geometry.cs` — **Point2D : IPoint2D** — class (~72 public members). (+3 types)
- `Guild.cs` — **GuildType** — Enum with about 3 members. (+1 type)
- `HuePicker.cs` — **HuePicker** — class (~6 public members).
- `IAccount.cs` — **IAccount** — Interface contract.
- `IEntity.cs` — **IEntity : IPoint3D** — Interface contract. Base: IPoint3D. (+1 type)
- `Insensitive.cs` — **Insensitive** — Static class (~7 public members).
- `Interfaces.cs` — **IMount** — Interface contract. (+11 types)
- `Item.cs` — **Item : IEntity** — Core item: serialization, OPL/name hooks, loot/ID flags, container/movement base.
- `ItemBounds.cs` — **ItemBounds** — Static class (~2 public members).
- `KeywordList.cs` — **KeywordList** — class (~6 public members).
- `Main.cs` — **Core** — Static system module (Initialize). (+3 types)
- `Map.cs` — **Map : IComparable** — Core map/sector storage, item/mobile lookup and update ranges.
- `Mobile.cs` — **Mobile : IEntity** — Core mobile: movement, skills, visibility, labels, combat base.
- `Movement.cs` — **Movement** — Static class (~6 public members). (+1 type)
- `MultiData.cs` — **MultiData** — Data holder. (+2 types)
- `NativeReader.cs` — **NativeReader** — Static class (~9 public members). (+3 types)
- `Notoriety.cs` — **Notoriety** — Static class (~13 public members).
- `ObjectPropertyList.cs` — **ObjectPropertyList : Packet** — Network packet. (+1 type)
- `Party.cs` — **PartyCommands** — Abstract class (~9 public members).
- `Point3DList.cs` — **Point3DList** — class (~9 public members).
- `Poison.cs` — **Poison** — Abstract class (~17 public members).
- `Prompt.cs` — **Prompt** — Abstract class (~4 public members).
- `QuestArrow.cs` — **QuestArrow** — class (~10 public members).
- `Race.cs` — **Race** — Abstract class (~36 public members).
- `Region.cs` — **Region : IComparable** — class (~106 public members). (+1 type)
- `ScriptCompiler.cs` — **ScriptCompiler** — Compiles/loads the script assembly and resolves script types for core hooks.
- `Sector.cs` — **Sector** — class (~28 public members). (+1 type)
- `SecureTrade.cs` — **SecureTrade** — class (~14 public members). (+1 type)
- `Serial.cs` — **Serial : IComparable** — class (~25 public members).
- `Serialization.cs` — **GenericReader** — Abstract class (~255 public members). (+6 types)
- `Skills.cs` — **Skills** — class (~117 public members). (+4 types)
- `TileData.cs` — **TileData** — Data holder. (+3 types)
- `TileList.cs` — **TileList** — class (~6 public members).
- `TileMatrix.cs` — **TileMatrix** — class (~41 public members). (+2 types)
- `TileMatrixPatch.cs` — **TileMatrixPatch** — class (~5 public members).
- `Timer.cs` — **Timer** — class (~58 public members). (+6 types)
- `Utility.cs` — **Utility** — Static class (~107 public members).
- `VirtueInfo.cs` — **VirtueInfo** — Info/data container.
- `World.cs` — **World** — Static class (~52 public members). (+5 types)

#### Core/ContextMenus — Context menu primitives.

- `ContextMenu.cs` — **ContextMenu** — Represents the state of an active context menu. This includes who opened the menu, the menu's focus object,...
- `ContextMenuEntry.cs` — **ContextMenuEntry** — Represents a single entry of a <see cref="ContextMenu">context menu</see>.
- `OpenBackpackEntry.cs` — **OpenBackpackEntry : ContextMenuEntry** — Context-menu entry.
- `PaperdollEntry.cs` — **PaperdollEntry : ContextMenuEntry** — Context-menu entry.

#### Core/Diagnostics — Diagnostics and logging.

- `BaseProfile.cs` — **BaseProfile** — Abstract class (~10 public members).
- `GumpProfile.cs` — **GumpProfile : BaseProfile** — class (~4 public members).
- `PacketProfile.cs` — **BasePacketProfile : BaseProfile** — Abstract class (~15 public members). (+2 types)
- `TargetProfile.cs` — **TargetProfile : BaseProfile** — class (~4 public members).
- `TimerProfile.cs` — **TimerProfile : BaseProfile** — class (~8 public members).

#### Core/Gumps — Core gump primitives.

- `Gump.cs` — **Gump** — class (~46 public members).
- `GumpAlphaRegion.cs` — **GumpAlphaRegion : GumpEntry** — class (~8 public members).
- `GumpBackground.cs` — **GumpBackground : GumpEntry** — class (~9 public members).
- `GumpButton.cs` — **GumpButton : GumpEntry** — class (~12 public members). (+1 type)
- `GumpCheck.cs` — **GumpCheck : GumpEntry** — class (~10 public members).
- `GumpEntry.cs` — **GumpEntry** — Data entry/record.
- `GumpGroup.cs` — **GumpGroup : GumpEntry** — class (~5 public members).
- `GumpHtml.cs` — **GumpHtml : GumpEntry** — class (~11 public members).
- `GumpHtmlLocalized.cs` — **GumpHtmlLocalized : GumpEntry** — class (~17 public members). (+1 type)
- `GumpImage.cs` — **GumpImage : GumpEntry** — class (~9 public members).
- `GumpImageTileButton.cs` — **GumpImageTileButton : GumpEntry** — class (~17 public members).
- `GumpImageTiled.cs` — **GumpImageTiled : GumpEntry** — class (~9 public members).
- `GumpItem.cs` — **GumpItem : GumpEntry** — class (~9 public members).
- `GumpItemProperty.cs` — **GumpItemProperty : GumpEntry** — class (~5 public members).
- `GumpLabel.cs` — **GumpLabel : GumpEntry** — class (~8 public members).
- `GumpLabelCropped.cs` — **GumpLabelCropped : GumpEntry** — class (~10 public members).
- `GumpPage.cs` — **GumpPage : GumpEntry** — class (~5 public members).
- `GumpRadio.cs` — **GumpRadio : GumpEntry** — class (~10 public members).
- `GumpSpriteImage.cs` — **GumpSpriteImage : GumpEntry** — class (~11 public members).
- `GumpTextEntry.cs` — **GumpTextEntry : GumpEntry** — Data entry/record.
- `GumpTextEntryLimited.cs` — **GumpTextEntryLimited : GumpEntry** — class (~12 public members).
- `GumpTooltip.cs` — **GumpTooltip : GumpEntry** — Adds that hover over tooltip to the previous Gump object, it can be text or an image, or button, item etc Y...
- `RelayInfo.cs` — **RelayInfo** — Info/data container. (+1 type)

#### Core/Items — Core item/container primitives.

- `BaseMulti.cs` — **BaseMulti : Item** — Item. [Constructable]
- `Container.cs` — **Container : Item** — Item. (+3 types)
- `Containers.cs` — **BankBox : Container** — Container. (+1 type)
- `SecureTradeContainer.cs` — **SecureTradeContainer : Container** — Container.
- `VirtualHair.cs` — **BaseHairInfo** — Info/data container. (+6 types)

#### Core/Menus — Menu primitives.

- `IMenu.cs` — **IMenu** — Interface contract.
- `ItemListMenu.cs` — **ItemListMenu : IMenu** — class (~13 public members). (+1 type)
- `QuestionMenu.cs` — **QuestionMenu : IMenu** — class (~7 public members).

#### Core/Network — Core packet definitions, handlers, NetState and encryption.

- `BufferPool.cs` — **BufferPool** — class (~7 public members).
- `ByteQueue.cs` — **ByteQueue** — class (~8 public members).
- `Compression.cs` — **Compression** — Handles outgoing packet compression for the network. (+7 types)
- `EncodedPacketHandler.cs` — **EncodedPacketHandler** — Handler.
- `EncodedReader.cs` — **EncodedReader** — class (~8 public members).
- `Listener.cs` — **Listener : IDisposable** — class (~5 public members).
- `MessagePump.cs` — **MessagePump** — class (~7 public members).
- `NetState.cs` — **NetState** — Per-connection state, send queue and encryption.
- `PacketHandler.cs` — **PacketHandler** — Handler.
- `PacketHandlers.cs` — **PacketHandlers** — Core packet handler tables and registration (0xD6, 0xBF, etc.).
- `PacketReader.cs` — **PacketReader** — class (~28 public members).
- `PacketWriter.cs` — **PacketWriter** — Provides functionality for writing primitive binary data.
- `Packets.cs` — **PMMessage : byte** — Causes the client to walk in a given direction. It does not send a movement request. (+161 types)
- `SendQueue.cs` — **SendQueue** — class (~22 public members). (+2 types)

#### Core/Persistence — World/account/timer persistence.

- `BinaryMemoryWriter.cs` — **BinaryMemoryWriter : BinaryFileWriter** — class (~3 public members).
- `DualSaveStrategy.cs` — **DualSaveStrategy : StandardSaveStrategy** — class (~4 public members).
- `DynamicSaveStrategy.cs` — **DynamicSaveStrategy : SaveStrategy** — class (~5 public members).
- `FileOperations.cs` — **FileOperations** — Static class (~12 public members). (+1 type)
- `FileQueue.cs` — **FileQueue : IDisposable** — class (~15 public members). (+2 types)
- `ParallelSaveStrategy.cs` — **ParallelSaveStrategy : SaveStrategy** — class (~13 public members). (+3 types)
- `Persistence.cs` — **Persistence** — Static class (~7 public members).
- `QueuedMemoryWriter.cs` — **QueuedMemoryWriter : BinaryFileWriter** — class (~7 public members). (+1 type)
- `SaveMetrics.cs` — **SaveMetrics : IDisposable** — class (~7 public members).
- `SaveStrategy.cs` — **SaveStrategy** — Abstract class (~5 public members).
- `SequentialFileWriter.cs` — **SequentialFileWriter : Stream** — class (~12 public members).
- `StandardSaveStrategy.cs` — **StandardSaveStrategy : SaveStrategy** — class (~5 public members).

#### Core/Targeting — Targeting primitives.

- `LandTarget.cs` — **LandTarget : IPoint3D** — class (~9 public members).
- `MultiTarget.cs` — **MultiTarget : Target** — Targeting prompt.
- `StaticTarget.cs` — **StaticTarget : IPoint3D** — class (~9 public members).
- `Target.cs` — **Target** — Abstract class (~17 public members). (+1 type)
- `TargetCancelType.cs` — **TargetCancelType** — Enum with about 4 members.
- `TargetFlags.cs` — **TargetFlags : byte** — Enum with about 3 members.

### Info scripts

#### Info scripts — Startup settings projects compiled separately from shard scripts.

- `Merchant.cs` — **StoreMerchant : SBInfo** — Vendor SBInfo definitions (Merchant buy/sell tables).
- `Settings.cs` — **MySettings** — Global shard settings (`MySettings`) toggles for systems, loot, skills, vendors.
- `Settings.override.cs` — **SettingOverrides** — Explicit `MySettings` overrides; never overwritten by Memento updates.
