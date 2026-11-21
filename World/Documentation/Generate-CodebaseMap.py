#!/usr/bin/env python3
import os, re, datetime

ROOT = os.path.abspath(os.path.join(os.path.dirname(__file__), "..", ".."))
SRC = os.path.join(ROOT, "World/Source")
OUT = os.path.join(ROOT, "Memento_Codebase_Map.md")
SCAN = [("Scripts", os.path.join(SRC, "Scripts")),
        ("Core", os.path.join(SRC, "System")),
        ("Info scripts", os.path.join(ROOT, "World/Info/Scripts"))]

TYPE_RE = re.compile(r"(?m)^[\t ]*(?:(?:public|internal|protected|private)\s+)?(?:(?:static|abstract|sealed|partial|new)\s+)*(class|interface|enum|struct)\s+([A-Za-z_][A-Za-z0-9_]*)\s*(?::\s*([^\{\r\n]+))?")
CMD_RE = re.compile(r'CommandSystem\.Register\s*\(\s*"([^"]+)"')
NAME_RE = re.compile(r'\bName\s*=\s*"([^"]{1,60})"')
AI_RE = re.compile(r'AIType\.(AI_[A-Za-z0-9_]+)')
FIGHT_RE = re.compile(r'FightMode\.([A-Za-z0-9_]+)')
SUMMARY_RE = re.compile(r"((?:[\t ]*///[^\n]*\n)+)[\t ]*(?:\[[^\n]*\]\s*\n[\t ]*)*(?:public|internal|protected|private)[^\n]*(?:class|interface|enum|struct)\s+", re.M)
DD_RE = re.compile(r'DefaultDescription[^\n]*\n(?:[^\n]*\n){0,14}?[^\n]*return\s+"([^"]{8,140})"')
DESC_ATTR_RE = re.compile(r'\[Description\s*\(\s*"([^"]{3,120})"')

BASE_TEXT = {
    "BaseCreature": "creature", "BaseVendor": "vendor NPC", "BaseMount": "mount",
    "BaseWeapon": "weapon", "BaseMeleeWeapon": "melee weapon", "BaseRanged": "ranged weapon",
    "BaseSword": "sword weapon", "BaseAxe": "axe weapon", "BasePoleArm": "polearm weapon",
    "BaseBashing": "mace/hammer weapon", "BaseSpear": "spear weapon", "BaseStaff": "staff weapon",
    "BaseKnife": "knife weapon", "BaseThrown": "thrown weapon", "SBInfo": "vendor buy/sell profile",
    "ContextMenuEntry": "context-menu entry", "Target": "targeting prompt", "BaseGump": "gump UI",
    "BaseSellInfo": "vendor sell profile", "BasePurchaseInfo": "vendor purchase profile",
    "BaseArmor": "armor", "BaseShield": "shield", "BaseClothing": "clothing", "BaseHat": "hat",
    "BaseTrinket": "trinket/jewelry", "BaseQuiver": "quiver", "BaseInstrument": "instrument",
    "BaseContainer": "container", "BaseBag": "bag", "BaseAddon": "house addon",
    "BaseAddonDeed": "addon deed", "BaseDoor": "door", "BasePotion": "potion",
    "BaseBeverage": "drink", "BaseFood": "food", "BaseBook": "book", "BaseTool": "tool",
    "BaseHarvestTool": "harvest tool", "BaseTrap": "trap", "BaseGift": "gift item",
    "BaseDecorationArtifact": "decorative artefact", "BaseMulti": "multi", "Gump": "gump UI",
    "Timer": "timer", "Spell": "spell", "Packet": "network packet", "Item": "item",
    "Mobile": "mobile", "Container": "container", "BaseLight": "light source",
    "BaseBoat": "boat", "BaseHouse": "house", "BaseAggregate": "aggregate",
}

def humanize(name):
    s = name.replace("_", " ").replace("-", " ")
    s = re.sub(r"(?<=[a-z])(?=[A-Z])", " ", s)
    s = re.sub(r"(?<=[A-Z])(?=[A-Z][a-z])", " ", s)
    return re.sub(r"\s+", " ", s).strip().lower()

def read(path):
    try:
        with open(path, "rb") as f:
            data = f.read()
        if data.startswith(b"\xff\xfe") or data.startswith(b"\xfe\xff"):
            return data.decode("utf-16", errors="replace")
        return data.decode("utf-8-sig", errors="replace")
    except Exception:
        return ""

def summary_for(text):
    m = SUMMARY_RE.search(text)
    if not m:
        return None
    bits = []
    for ln in m.group(1).splitlines():
        ln = re.sub(r"^\s*///\s?", "", ln).strip()
        if ln and not ln.startswith("<"):
            bits.append(ln)
    if not bits:
        return None
    s = re.sub(r"\s+", " ", " ".join(bits)).strip()
    return (s[:107].rstrip() + "...") if len(s) > 110 else s
def parse(path):
    text = read(path)
    stem = os.path.splitext(os.path.basename(path))[0]
    types = []
    for m in TYPE_RE.finditer(text):
        kind, name, bases = m.group(1), m.group(2), (m.group(3) or "").strip()
        base = ""
        if bases:
            base = bases.split(",")[0].strip().split(".")[-1]
            base = re.sub(r"<.*", "", base).strip()
        types.append((kind, name, base))
    if not types:
        return None
    primary = next((t for t in types if t[1] == stem), types[0])
    kind, name, base = primary
    cmds = sorted(set(CMD_RE.findall(text)))
    namelit = next((nm for nm in NAME_RE.findall(text) if nm and len(nm) < 45), None)
    ai, fight = AI_RE.search(text), FIGHT_RE.search(text)
    attrs = [a for a in ("Constructable", "CorpseName", "Flipable", "TypeAlias", "Serializable")
             if "[" + a in text[:4000]]
    abstract = bool(re.search(r"abstract\s+" + kind, text))
    static = bool(re.search(r"static\s+" + kind, text))
    summary = summary_for(text)
    dd = DD_RE.search(text)
    desc_attr = DESC_ATTR_RE.search(text)
    if summary:
        desc = summary
    elif cmds:
        desc = "Commands: " + ", ".join("[" + c for c in cmds[:6])
        if len(cmds) > 6:
            desc += " (+%d)" % (len(cmds) - 6)
    elif dd:
        desc = re.sub(r"\s+", " ", dd.group(1)).strip()
        if len(desc) > 110:
            desc = desc[:107].rstrip() + "..."
    elif desc_attr:
        desc = desc_attr.group(1)
    elif kind == "enum":
        body = text.split("{", 1)[-1].split("}", 1)[0]
        desc = "Enum with about %d members." % len([x for x in body.split(",") if x.strip()])
    elif kind == "interface":
        desc = "Interface contract." + (" Base: %s." % base if base else "")
    elif base == "BaseCreature":
        bits = []
        if namelit: bits.append('"%s"' % namelit)
        if ai: bits.append(ai.group(1))
        if fight: bits.append(fight.group(1))
        desc = "Creature" + (" (" + ", ".join(bits) + ")" if bits else "") + "."
    elif base in BASE_TEXT:
        desc = BASE_TEXT[base][0].upper() + BASE_TEXT[base][1:]
        if namelit: desc += ' ("%s")' % namelit
        desc += "."
    elif static and "Initialize(" in text:
        desc = "Static system module (Initialize)."
    elif kind in ("class", "struct"):
        suf = name.lower()
        pairs = (("manager", "Manager/service."), ("provider", "Provider/service."),
                 ("factory", "Factory/service."), ("handler", "Handler."),
                 ("helper", "Helper utilities."), ("util", "Helper utilities."),
                 ("entry", "Data entry/record."), ("info", "Info/data container."),
                 ("context", "Context/state holder."), ("state", "State holder."),
                 ("data", "Data holder."), ("settings", "Settings holder."),
                 ("command", "Command implementation."))
        desc = next((v for k, v in pairs if suf.endswith(k)), None)
        if not desc:
            pub = len(re.findall(r"\bpublic\s+", text))
            desc = ("Abstract " if abstract else "") + ("Static " if static else "") + "class (~%d public members)." % pub
    else:
        desc = kind.capitalize() + "."
    if attrs: desc += " [" + ", ".join(attrs) + "]"
    more = len(types) - 1
    if more > 0: desc += " (+%d type%s)" % (more, "" if more == 1 else "s")
    return {"file": os.path.basename(path), "kind": kind, "name": name,
            "base": base, "desc": desc, "namelit": namelit}

CATEGORIES = (
    ("Items/Magical/Artifacts", "Artifact item"),
    ("Items/Magical", "Magical item"),
    ("Items/Armor", "Armor item"),
    ("Items/Weapons", "Weapon item"),
    ("Items/Clothing", "Clothing item"),
    ("Items/Containers", "Container item"),
    ("Items/Food", "Food/drink item"),
    ("Items/Potions", "Potion"),
    ("Items/Trades", "Trade item/tool"),
    ("Items/Relics", "Relic item"),
    ("Items/Quivers", "Quiver"),
    ("Items", "Item"),
    ("Mobiles", "Creature/NPC"),
    ("Engines and Systems/Quests", "Quest object"),
    ("System/Commands", "Command helper"),
    ("System/Gumps", "Gump component"),
)

def collect(base_dir, label):
    files = {}
    for dirpath, dirnames, filenames in os.walk(base_dir):
        dirnames.sort()
        rel = os.path.relpath(dirpath, base_dir).replace(os.sep, "/")
        if rel == ".": rel = ""
        entry = []
        for fn in sorted(filenames):
            if not fn.endswith(".cs"):
                continue
            p = os.path.join(dirpath, fn)
            info = parse(p)
            if not info:
                stem = os.path.splitext(fn)[0]
                info = {"file": fn, "kind": "file", "name": stem, "base": "",
                        "desc": "Source file (no top-level type declaration detected)."}
            key = (label + "/" + rel).strip("/") + "/" + fn
            info["desc"] = FILE_OVERRIDES.get(key, info["desc"])
            if "public members" in info["desc"] or info["desc"] in ("Class.", "Static class.", "Abstract class."):
                cat = next((v for k, v in CATEGORIES if rel == k or rel.startswith(k + "/")), None)
                if cat:
                    info["desc"] = cat + ((' ("%s")' % info["namelit"]) if info.get("namelit") else "") + "."
            entry.append(info)
        files[rel] = entry
    return files
def folder_desc(label, rel, entries):
    full = (label + "/" + rel).strip("/")
    if full in FOLDER_OVERRIDES:
        return FOLDER_OVERRIDES[full]
    if rel in FOLDER_OVERRIDES:
        return FOLDER_OVERRIDES[rel]
    name = humanize(os.path.basename(rel) or label)
    if not entries:
        return "No direct scripts; groups subfolders."
    types = ", ".join(e["name"] for e in entries[:4])
    if len(entries) > 4:
        types += ", ..."
    prefixes = (
        ("Items/Magical/Artifacts", "Artifact items"),
        ("Items/Magical", "Magical items"),
        ("Items/Armor", "Armor/shield items"),
        ("Items/Weapons", "Weapon items"),
        ("Items/Clothing", "Clothing items"),
        ("Items", "Items"),
        ("Mobiles", "Creatures/NPCs"),
        ("Engines and Systems/Magic", "Magic/spell system"),
        ("Engines and Systems/Quests", "Quest content"),
        ("Engines and Systems/Trades", "Trade/craft content"),
        ("System/Commands", "Command modules"),
        ("System/Gumps", "Gump UI"),
        ("System/Skills", "Skill system"),
        ("System", "Support system"),
    )
    pre = next((p for k, p in prefixes if rel == k or rel.startswith(k + "/")), None)
    lead = (pre + ": ") if pre else ""
    leaf = humanize(os.path.basename(rel))
    leaf = leaf[:1].upper() + leaf[1:]
    return "%s%s. Types: %s." % (lead, leaf, types)

def main():
    catalogs = [(label, collect(base, label)) for label, base in SCAN]
    total_files = sum(len(v) for _, fs in catalogs for v in fs.values())
    total_folders = sum(len(fs) for _, fs in catalogs)
    out = []
    a = out.append
    a("# Memento Codebase Map")
    a("")
    a("Generated %s from the `World/` tree. Covers **%d** `.cs` files in **%d** source folders, plus every non-source folder." % (datetime.date.today().isoformat(), total_files, total_folders))
    a("")
    a("## How to use this map")
    a("")
    a("- Find a file: `grep -n \"VendorValue.cs\" Memento_Codebase_Map.md` or `grep -n \"Items/Weapons/Axes\" Memento_Codebase_Map.md`.")
    a("- Find a class: `grep -n \"class BrownBear\" ...` (entries show `Type : Base`).")
    a("- Browse by area: the folder index lists every folder with its direct file count and contained types.")
    a("- Entry format: `File.cs` — **PrimaryType : Base** — purpose (`(+N types)` = extra types in that file; brackets = attributes such as `Constructable`/`CorpseName`).")
    a("- Descriptions are extracted from XML summaries, `DefaultDescription`, command registrations, AI/name literals and class/base heuristics; generic classes report their public-member count.")
    a("")
    a("## Repository at a glance")
    a("")
    a("| Path | What it is |")
    a("|---|---|")
    a("| `World/Source/System/` | RunUO/Memento core: Item, Mobile, Map, network, persistence. Built into `WorldLinux.exe`. |")
    a("| `World/Source/Scripts/` | Shard script assembly: items, mobiles, engines, commands, skills (~5,172 files). Recompiled on startup. |")
    a("| `World/Source/Tools/` | `compile-world-linux.sh` (mcs); output `World/WorldLinux.exe`. |")
    a("| `World/Info/Scripts/` | Startup settings projects: `Settings.cs`, `Settings.override.cs`, `Merchant.cs`. |")
    a("| `World/Info/` | Spawn maps, articles, decoration data. |")
    a("| `World/Data/` | Runtime data: `Files/` (mul), `System/` (config/cliloc), `Spawns/`, `Decoration/`, `Logs/`, `RichVeins/`. |")
    a("| `World/Saves/` | Serialized accounts/items/mobiles/guilds/player data. Do not hand-edit. |")
    a("| `World/Documentation/` | Design/showcase docs; not loaded by the server. |")
    a("| `World/Backups/` | Timestamped `.bak` copies of edited files (git-ignored). |")
    a("")
    a("## Cross-cutting systems")
    a("")
    a("| Topic | Start here |")
    a("|---|---|")
    a("| Tooltip worth/tiers | `World/Source/Scripts/System/Misc/VendorValue.cs`, `World/Source/Scripts/System/Misc/ItemRank.cs` |")
    a("| Core->script hooks | `World/Source/System/Item.cs` (`InvokeItemTier`, `InvokeColoredName`, `InvokeNameHue`, `InvokeSuppressColorText3`, `InvokeLabelHue`) |")
    a("| Vendor pricing | `World/Source/Scripts/System/Misc/ItemSales.cs`, `World/Source/Scripts/Mobiles/Base/GenericSell.cs`, `World/Source/Scripts/Mobiles/Base/BaseVendor.cs` |")
    a("| Preferences/settings | `World/Source/Scripts/Mobiles/Base/PlayerPreferenceContext.cs`, `World/Source/Scripts/System/Help/HelpGump*.cs` |")
    a("| Relic loot/ID | `World/Source/Scripts/Items/Relics/*` (`DDRelic*`, `HighSeasRelic`, `NotIdentified`, `RelicFunctions`) |")
    a("| Loot generation | `World/Source/Scripts/Items/Containers/LootPack.cs`, `World/Source/Scripts/Mobiles/Base/BaseCreature.cs` |")
    a("| Commands | `World/Source/Scripts/System/Commands/`, `World/Source/Scripts/System/Commands/LootTierCommands.cs` |")
    a("| Dungeon challenge | `World/Source/Scripts/Items/Misc/DungeonChallengeOrb.cs`, `World/Source/System/Map.cs` (`Difficult.InvokeDifficultyBoost`) |")
    a("| Random equipment names | `World/Source/Scripts/System/Misc/RandomThings.cs` (`MagicItemName`, `SpecialName`, `SkipMagicName`) |")
    a("| Spawns/regions | `World/Source/Scripts/System/Spawns/`, `World/Source/Scripts/System/Regions/`, `World/Info/Spawns/` |")
    a("| Persistence | `World/Source/System/Persistence/`, `World/Saves/` |")
    a("")
    a("### Common tasks")
    a("")
    a("| Want to... | Look at |")
    a("|---|---|")
    a("| Add/modify a weapon or armor | `Scripts/Items/Weapons/<family>`, `Scripts/Items/Armor/<family>`, base class in same folder |")
    a("| Add a creature | `Scripts/Mobiles/<type>/<family>`, `Mobiles/Base/BaseCreature.cs` |")
    a("| Add a command | `Scripts/System/Commands/`, follow `Initialize()` + `CommandSystem.Register` |")
    a("| Add a spell | `Scripts/Engines and Systems/Magic/<school>/Spells`, base `Spell` |")
    a("| Add quest content | `Scripts/Engines and Systems/Quests/<quest>` |")
    a("| Tune crafting/trades | `Scripts/Engines and Systems/Trades/`, `Scripts/Items/Trades/` |")
    a("| Tune loot/relics | `Scripts/Items/Containers/LootPack.cs`, `Scripts/Items/Relics/` |")
    a("| Tune prices/vendors | `Scripts/System/Misc/ItemSales.cs`, `Scripts/Mobiles/Base/{BaseVendor,GenericSell}.cs` |")
    a("| Change tooltips/prefs | `Scripts/System/Misc/{VendorValue,ItemRank}.cs`, `Scripts/System/Help/HelpGump*.cs`, `Mobiles/Base/PlayerPreferenceContext.cs` |")
    a("| Add a gump | `Scripts/System/Gumps/`, derive from `Gump` |")
    a("| Respawn/boost a dungeon | `Scripts/Items/Misc/DungeonChallengeOrb.cs` (use item), `Source/System/Map.cs` (heat hook) |")
    a("")
    a("### Conventions")
    a("")
    a("- File name = main class name. One feature per folder; base classes sit beside their concrete items.")
    a("- Items: `[Constructable]` ctor + `(Serial)` ctor + `Serialize`/`Deserialize`; mobiles: `[CorpseName]`, stats via `SetStr`/`SetSkill`.")
    a("- Commands: static `Initialize()` with `CommandSystem.Register`; gumps: `Gump` subclasses.")
    a("- Core never references script types directly: `World/Source/System/Item.cs` resolves `Server.ItemRank` / `Server.VendorValue` by reflection (`Invoke*` hooks). Keep hook signatures stable.")
    a("- Scripts compile at startup; core changes require rebuilding `World/WorldLinux.exe`.")
    a("- Record source changes in `Memento_System_Changes.md`; backups go to `World/Backups/<File>.<timestamp>.bak`.")
    a("")
    a("### Maintenance")
    a("")
    a("- This map is generated from source. Regenerate with `python3 World/Documentation/Generate-CodebaseMap.py`; hand-edits are overwritten.")
    a("- Keep the counts in the header and the folder index in sync when files move; `Memento_System_Changes.md` remains the authoritative change log.")
    a("")
    a("")
    a("## Folder index")
    a("")
    a("One line per source folder (direct `.cs` counts). Curated purposes for major folders, type-derived otherwise.")
    a("")
    for label, files in catalogs:
        for rel in sorted(files):
            entries = files[rel]
            full = (label + "/" + rel).strip("/")
            a("- **%s** — %d file%s — %s" % (full, len(entries), "" if len(entries) == 1 else "s", folder_desc(label, rel, entries)))
    a("")
    a("## Non-source folders")
    a("")
    a("| Path | Purpose |")
    a("|---|---|")
    for p, d in nonsource_dirs():
        a("| `%s` | %s |" % (p, d))
    a("")
    a("Timestamped snapshots inside `World/Backups/Automatic` are copies and are not listed individually.")
    a("")
    a("## Script catalogs")
    a("")
    a("Format: `File.cs` — **PrimaryType : Base** — purpose. `(+N types)` = extra types in the file. Item attributes appear in brackets.")
    a("")
    for label, files in catalogs:
        a("### %s" % label)
        a("")
        for rel in sorted(files):
            entries = files[rel]
            if not entries:
                continue
            full = (label + "/" + rel).strip("/")
            a("#### %s — %s" % (full, folder_desc(label, rel, entries)))
            a("")
            for e in entries:
                decl = e["name"] + (" : " + e["base"] if e["base"] else "")
                a("- `%s` — **%s** — %s" % (e["file"], decl, e["desc"]))
            a("")
    with open(OUT, "w", encoding="utf-8") as f:
        f.write("\n".join(out))
    print("wrote", OUT, len(out), "lines")
    print("files", sum(len(v) for _, fs in catalogs for v in fs.values()))
    print("folders", sum(len(fs) for _, fs in catalogs))
FOLDER_OVERRIDES = {
    "Scripts": "Shard script assembly; gameplay/content code compiled at startup.",
    "Scripts/Engines and Systems": "Gameplay engines and content systems (magic, quests, trades, champs, UI systems).",
    "Scripts/Engines and Systems/Achievments": "Achievement definitions, tracking, gumps and rewards.",
    "Scripts/Engines and Systems/Avatar": "Avatar progression, coins, rewards, starter templates and safety-deposit items.",
    "Scripts/Engines and Systems/Champs": "Champion spawn altars, bosses, minions and rewards.",
    "Scripts/Engines and Systems/Jako Pets": "Jako pet attribute system and pet items.",
    "Scripts/Engines and Systems/Messaging": "Messaging/social system.",
    "Scripts/Engines and Systems/MobileEnhancement": "Mobile enhancement tweaks and helpers.",
    "Scripts/Engines and Systems/ModernSkill": "Modern skill UI/logic.",
    "Scripts/Engines and Systems/PuzzleChest": "Puzzle chest mini-system.",
    "Scripts/Engines and Systems/Spell Bars": "Spell-bar UI and commands.",
    "Scripts/Engines and Systems/Temptations": "Temptation/choice event system.",
    "Scripts/Engines and Systems/Magic": "Spell schools, spellbooks, reagents, summons, spell mechanics.",
    "Scripts/Engines and Systems/Quests": "Quest framework and content (standard, fishing, assassin, museum, runes).",
    "Scripts/Engines and Systems/Trades": "Crafting/trade engines: bulk orders, harvesting, shoppes, gardening, tinkering.",
    "Scripts/Items": "Item definitions: equipment, consumables, containers, relics, housing, games, tech.",
    "Scripts/Items/Armor": "Armor and shield item classes.",
    "Scripts/Items/Weapons": "Weapon item classes grouped by family.",
    "Scripts/Items/Clothing": "Clothing, hats, shoes and wearable deco.",
    "Scripts/Items/Containers": "Containers, loot packs, treasure/paragon/sunken chests, corpses.",
    "Scripts/Items/Magical": "Magical items: artifacts, quivers, wands, gift/quest rewards.",
    "Scripts/Items/Magical/Artifacts": "Artifact item definitions grouped by slot/set (armor, clothing, jewelry, weapons, books, quivers).",
    "Scripts/Items/Magical/Gifts": "Gift/reward item bases and variants.",
    "Scripts/Items/Relics": "Relic loot family, identification flow and legacy worth text.",
    "Scripts/Items/Houses": "Housing, addons, decoration, remodeling and house tools.",
    "Scripts/Items/Misc": "Miscellaneous items (dyes, tools, bodies, fragments, currency).",
    "Scripts/Items/Special": "Special/quest/event items and one-off systems.",
    "Scripts/Items/Trades": "Crafting tools, resources and trade skill items.",
    "Scripts/Items/Technology": "Sci-fi/technology items (ZorkPad, DataPads, devices).",
    "Scripts/Mobiles": "NPC and creature definitions.",
    "Scripts/Mobiles/Base": "Base mobile classes, AI/behavior helpers, vendors, player preferences.",
    "Scripts/Mobiles/Civilized": "Town NPCs, vendors, guards, citizens and services.",
    "Scripts/Mobiles/Omni AI": "OmniAI behaviour framework used by custom creatures.",
    "Scripts/System": "Support systems: commands, gumps, help, networking, skills, spawns, regions.",
    "Scripts/System/Commands": "GM/player commands and command frameworks.",
    "Scripts/System/Gumps": "Server-side gump UIs.",
    "Scripts/System/Help": "Help gump, settings UI and player preference pages.",
    "Scripts/System/Misc": "Cross-cutting systems (tooltips/worth/tiers, item sales, settings, random things).",
    "Scripts/System/Network": "Packets, packet handlers, networking extensions.",
    "Scripts/System/Regions": "Custom regions and region logic.",
    "Scripts/System/Skills": "Skill systems and skill-specific mechanics.",
    "Scripts/System/Spawns": "Spawners and spawn management.",
    "Scripts/Utilities": "Shared helpers, constants and UTC utilities.",
    "Core": "RunUO/Memento core compiled into WorldLinux.exe.",
    "Core/Network": "Core packet definitions, handlers, NetState and encryption.",
    "Core/Items": "Core item/container primitives.",
    "Core/Gumps": "Core gump primitives.",
    "Core/Persistence": "World/account/timer persistence.",
    "Core/Commands": "Core command infrastructure.",
    "Core/Targeting": "Targeting primitives.",
    "Core/ContextMenus": "Context menu primitives.",
    "Core/Menus": "Menu primitives.",
    "Core/Properties": "Property/attribute plumbing.",
    "Core/Diagnostics": "Diagnostics and logging.",
    "Info scripts": "Startup settings projects compiled separately from shard scripts.",
}

NS_OVERRIDES = {
    "World/Backups": "Timestamped .bak copies of edited source files; git-ignored.",
    "World/Backups/Automatic": "Automatic periodic backups.",
    "World/Backups/Crashed": "Copies captured after a crash.",
    "World/Data": "Runtime data root (client mul files, config, spawns, logs).",
    "World/Data/Decoration": "Decoration item data.",
    "World/Data/Decoration/Monopoly": "Monopoly board/deed decoration data by land.",
    "World/Data/Files": "Client mul data: maps, statics, multi, Cliloc.enu.",
    "World/Data/Logs": "Server/runtime logs.",
    "World/Data/Logs/Commands": "Command audit logs by access level.",
    "World/Data/RichVeins": "Rich vein harvesting data.",
    "World/Data/Spawns": "Spawn data files.",
    "World/Data/System": "Config and startup data.",
    "World/Data/System/CFG": "Config files including `cliloc.cfg` (cliloc text/colour markup).",
    "World/Data/System/XML": "XML settings/data used at startup.",
    "World/Data/System/Components": "Component definitions used by systems.",
    "World/Data/System/Bulk Orders": "Bulk-order reward data (blacksmith, tailoring).",
    "World/Documentation": "Prose/design documentation; not loaded by the server.",
    "World/Documentation/Development": "Developer notes/design docs.",
    "World/Documentation/Showcase": "Showcase material.",
    "World/Info": "Startup configuration and content data.",
    "World/Info/Articles": "Newspaper/article definitions.",
    "World/Info/Decorations": "Decoration definitions.",
    "World/Info/Scripts": "Standalone settings projects (cataloged under Info scripts).",
    "World/Info/Spawns": "Spawn map definitions.",
    "World/Saves": "Serialized world state.",
    "World/Saves/Accounts": "accounts.xml login/character data.",
    "World/Saves/Achievements": "Achievement save data.",
    "World/Saves/ChatBeta8": "Chat system save data.",
    "World/Saves/Craft": "Crafting save data.",
    "World/Saves/Data": "Misc persisted data.",
    "World/Saves/Guilds": "Guild save data.",
    "World/Saves/Items": "World item serialization (Items.bin/idx/tdb).",
    "World/Saves/Mobiles": "World mobile serialization (Mobiles.bin/idx/tdb).",
    "World/Saves/Player": "Per-player persisted data.",
    "World/Saves/Transmogrify": "Transmogrify system save data.",
    "World/Source/Tools": "Build helper scripts (Linux mcs build).",
}

FILE_OVERRIDES = {
    "Scripts/System/Misc/VendorValue.cs": "Per-player **Worth** line: relic/unidentified pricing, Show Worth toggle, tooltip refresh, 0xD6/0xBF handlers, legacy relic worth suppression.",
    "Scripts/System/Misc/ItemRank.cs": "Loot tiers (Common→Artefact), tier name/text colours, quiver header hue, artefact ground label hue, wrappers.",
    "Scripts/System/Misc/ItemSales.cs": "ItemInformation: sell/buy tables, AddUpBenefits property pricing, buy lists, vendor deal rules.",
    "Scripts/Mobiles/Base/GenericSell.cs": "Vendor sell table: GetSellPriceFor / GetBasePriceFor and sellability.",
    "Scripts/Mobiles/Base/BaseVendor.cs": "Vendor AI: buy/sell flow, ID service, pricing and stock.",
    "Scripts/Mobiles/Base/PlayerPreferenceContext.cs": "Player preferences incl. ShowLootTiers/ShowVendorValues; serialization v12.",
    "Scripts/Mobiles/Base/PlayerMobile.cs": "Player mobile: skills, movement, preferences, tooltip/quest integration.",
    "Scripts/Mobiles/Base/BaseCreature.cs": "Base creature: AI, combat, skills, loot drops and corpse handling.",
    "Scripts/Items/Containers/LootPack.cs": "Loot generation: pack tiers, loot drops, relic/artifact placement.",
    "Scripts/Items/Relics/NotIdentified.cs": "Unidentified loot wrapper: packaging, ID flow, base-worth display support.",
    "Scripts/Items/Relics/RelicFunctions.cs": "Relic identification mechanics and vendor/player ID rules.",
    "Scripts/System/Help/HelpGump.cs": "Help gump: main pages, settings pages, toggles and info panels.",
    "Scripts/System/Help/HelpGump_Settings.cs": "MobileUO settings layout (loot tiers, worth, preferences).",
    "Scripts/System/Commands/LootTierCommands.cs": "Dev commands: `[TierCommon`…`[TierPrimal`, `[TierAll`, `[TierItem`, `[LootTiers`.",
    "Scripts/Items/Weapons/BaseWeapon.cs": "Base weapon: identity, name/property building, quality, durability, slayers.",
    "Scripts/Items/Armor/BaseArmor.cs": "Base armor: identity, name/property building, resistances, durability.",
    "Scripts/Items/Clothing/BaseClothing.cs": "Base clothing: name/properties, quality and wearables.",
    "Scripts/Items/Quivers/BaseQuiver.cs": "Base quiver: weight reduction, damage bonus, ColorText1/2 header.",
    "Scripts/Items/Quivers/ArcherQuiver.cs": "Archer quiver loot item with ColorText1/2 headers.",
    "Scripts/Items/Quivers/ElvenQuiver.cs": "Elven quiver loot item with ColorText1/2 headers.",
    "Scripts/System/Misc/AOS.cs": "AOS-era helpers and property plumbing.",
    "Scripts/Items/Magical/Artifacts/Jewelry/Artifact_LuckyEarrings.cs": "Standard artifact earrings: Luck 150, regen 3, attack/defend +5, weapon speed +5, ArtifactLevel set and ArtySetup applied.",
    "Scripts/System/Spawns/ReadMe.cs": "Readme for the Premium Spawner derived from Ultimate Spawner; explains spawn properties/engine.",
    "Core/AssemblyInfo.cs": "Assembly metadata (name/version).",
    "Info scripts/Settings.cs": "Global shard settings (`MySettings`) toggles for systems, loot, skills, vendors.",
    "Info scripts/Settings.override.cs": "Explicit `MySettings` overrides; never overwritten by Memento updates.",
    "Info scripts/Merchant.cs": "Vendor SBInfo definitions (Merchant buy/sell tables).",
    "Core/Item.cs": "Core item: serialization, OPL/name hooks, loot/ID flags, container/movement base.",
    "Core/Mobile.cs": "Core mobile: movement, skills, visibility, labels, combat base.",
    "Core/Map.cs": "Core map/sector storage, land lookups, dungeon heat table (`Difficult.GetDifficulty`) and the one-tier `InvokeDifficultyBoost` script hook.",
    "Core/ScriptCompiler.cs": "Compiles/loads the script assembly and resolves script types for core hooks.",
    "Scripts/Items/Misc/DungeonChallengeOrb.cs": "Consumable orb: respawns region XML spawns plus in-region PremiumSpawner/Spawner items and applies a timed +1 heat boost (`DungeonChallengeBoost`).",
    "Scripts/System/Commands/DungeonChallengeCommands.cs": "GM `[DungeonOrb` command: places a DungeonChallengeOrb in the user's backpack.",
    "Core/Network/PacketHandlers.cs": "Core packet handler tables and registration (0xD6, 0xBF, etc.).",
    "Core/Network/NetState.cs": "Per-connection state, send queue and encryption.",
}


NS_ROOTS = ["Backups", "Data", "Documentation", "Info", "Saves", "Source/Tools"]

def nonsource_dirs():
    out = []
    world = os.path.join(ROOT, "World")
    for root in NS_ROOTS:
        base = os.path.join(world, root)
        if not os.path.isdir(base):
            continue
        for dirpath, dirnames, filenames in os.walk(base):
            dirnames.sort()
            rel = "World/" + os.path.relpath(dirpath, world).replace(os.sep, "/")
            if rel == "World/Info/Scripts":
                continue
            if rel.startswith("World/Backups/") and rel.count("/") > 2:
                continue
            desc = NS_OVERRIDES.get(rel)
            if not desc:
                leaf = humanize(rel.split("/")[-1])
                if root == "Saves":
                    desc = "Serialized %s data." % leaf
                elif root == "Data":
                    desc = "Runtime data: %s." % leaf
                elif root == "Info":
                    desc = "Startup content: %s." % leaf
                elif root == "Documentation":
                    desc = "Documentation: %s." % leaf
                else:
                    desc = "Backup storage: %s." % leaf
            out.append((rel, desc))
    return out

if __name__ == "__main__":
    main()
