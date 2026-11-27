#!/usr/bin/env python3
"""Generate World/Data/System/CFG/GypsyLore.txt from the shard's books and NPC talk.

Sources:
  * Items/Books/LoreBook.cs      - the 47 lore books written by GetText()
  * Items/Books/DynamicBook.cs   - journals and clue books written by SetStaticText()
  * Items/Books/BardsTaleNote.cs - the scattered bard's-tale clues
  * Items/Books/DoomFlayerNote.cs- the Doom Flayer note
  * System/Misc/Talk.cs          - every NPC conversation in SpeechFunctions
  * Quests/Summon/SummonPrison.cs - the tribute relic table (dungeon, guardian, item)
  * Quests/NoticeClue.cs, BulletinBoards/WantedMangar.cs and SomeRandomNote.cs
                                - fixed notice clues, the wanted poster, and the truthful scattered notes

Run from anywhere:
    python3 World/Documentation/Build-GypsyLore.py

The script overwrites GypsyLore.txt. Add new lore to this script (or edit the
generated file after the last run) and restart/reload the shard.
"""
import html
import re
from pathlib import Path

WORLD = Path(__file__).resolve().parents[1]
OUT = WORLD / "Data/System/CFG/GypsyLore.txt"

# Talk.cs entries that are object descriptions or client/meta instructions, not
# conversations with an NPC. They add noise to the gypsy's reading.
SKIP_TALK = {
    "ScrapMetal", "Aquarium", "Patch", "Trophy", "Stuff", "Stuffing", "Stake",
}

# Some conversation keys are roles rather than a name; make the reconstructed
# sentence read naturally ("My name is the Elementalist", not "the Elementalism").
SPEAKERS = {
    "Elementalism": "the Elementalist",
    "Variety": "the merchant",
    "Tavern": "the barkeep",
    "Pets": "the animal trainer",
    "Powerscroll": "the elven teacher",
    "XardokFail": "Xardok",
    "Frankenstein": "Frankenstein",
    "NecroGreeter": "the greeter",
}


def read(path):
    return path.read_text(encoding="utf-8", errors="replace")


def parse_string(src, i):
    """Parse a C# string literal at src[i] (or @"...") -> (text, end_index)."""
    verbatim = False
    if src[i] == "@":
        verbatim = True
        i += 1
    if i >= len(src) or src[i] != '"':
        raise ValueError("not a string at %d" % i)
    i += 1
    out = []
    escapes = {"n": "\n", "r": "\r", "t": "\t", '"': '"', "\\": "\\", "'": "'", "0": "\0"}
    while i < len(src):
        c = src[i]
        if c == '"':
            if verbatim and i + 1 < len(src) and src[i + 1] == '"':
                out.append('"')
                i += 2
                continue
            return "".join(out), i + 1
        if not verbatim and c == "\\":
            i += 1
            if i >= len(src):
                break
            e = src[i]
            i += 1
            if e in escapes:
                out.append(escapes[e])
            elif e == "u" and i + 4 <= len(src):
                try:
                    out.append(chr(int(src[i:i + 4], 16)))
                except ValueError:
                    pass
                i += 4
            else:
                out.append(e)
            continue
        out.append(c)
        i += 1
    return "".join(out), i


def expr_text(expr, speaker=None, locals_map=None):
    """Literal text of a C# expression, or None when it uses unknown variables/calls.

    A few simple talk-template variables are replaced so the NPC conversations
    survive extraction instead of being dropped with the greetings.
    """
    known = {
        "sYourName": "traveler",
        "sMyName": speaker or "the speaker",
    }
    if locals_map:
        for key, value in locals_map.items():
            known.setdefault(key, value)
    parts = []
    i = 0
    while i < len(expr):
        c = expr[i]
        if c == "@" and i + 1 < len(expr) and expr[i + 1] == '"':
            text, i = parse_string(expr, i)
            parts.append(text)
            continue
        if c == '"':
            text, i = parse_string(expr, i)
            parts.append(text)
            continue
        if c.isalpha() or c == "_":
            j = i
            while j < len(expr) and (expr[j].isalnum() or expr[j] == "_"):
                j += 1
            name = expr[i:j]
            if name not in known:
                return None
            parts.append(known[name])
            i = j
            continue
        if c in "+ \t\r\n()":
            i += 1
            continue
        return None
    return "".join(parts)


def statement_end(src, start):
    """Index of the semicolon that ends the statement at/after start."""
    depth = 0
    i = start
    while i < len(src):
        c = src[i]
        if c == "@" and i + 1 < len(src) and src[i + 1] == '"':
            _, i = parse_string(src, i)
            continue
        if c == '"':
            _, i = parse_string(src, i)
            continue
        if c in "([{":
            depth += 1
        elif c in ")]}":
            depth = max(0, depth - 1)
        elif c == ";" and depth == 0:
            return i
        i += 1
    return len(src)


def clean(text):
    if not text:
        return ""
    text = re.sub(r"<\s*br\s*/?\s*>", "\n", text, flags=re.I)
    text = re.sub(r"<[^>]+>", "", text)
    text = html.unescape(text)
    text = text.replace("\r\n", "\n").replace("\r", "\n")
    text = re.sub(r"^[>/]+", "", text)
    text = re.sub(r"[ \t]+", " ", text)
    text = re.sub(r" *\n *", "\n", text)
    text = re.sub(r"\n{3,}", "\n\n", text)
    return text.strip()


def prettify(name):
    name = re.sub(r"(?<!^)(?=[A-Z])", " ", name)
    return re.sub(r"\s+", " ", name).strip()


def title_case(text):
    """Title-case a relic name without capitalising small words."""
    small = {"of", "the", "a", "an", "and", "in", "to", "from", "for", "on", "at", "by", "with"}
    words = text.split()
    out = []
    for i, word in enumerate(words):
        if i > 0 and word.lower() in small:
            out.append(word.lower())
        else:
            out.append(word[:1].upper() + word[1:])
    return " ".join(out)

sections = []


def add(title, text):
    text = clean(text)
    if text and len(text) > 40:
        sections.append((title, text))


# Display titles for the DynamicBook subclasses (MyLibrary.bookInfo knows them).
library = read(WORLD / "Source/Scripts/System/Commands/Player/MyLibrary.cs")
book_titles = {}
for m in re.finditer(r"item\s*=\s*typeof\((\w+)\)\.Name;\s*title\s*=\s*\"", library):
    title, _ = parse_string(library, m.end() - 1)
    book_titles[m.group(1)] = title

# --- LoreBook -----------------------------------------------------------------
lorebook = read(WORLD / "Source/Scripts/Items/Books/LoreBook.cs")
authors = {}
for m in re.finditer(r"BookTitle\s*=\s*\"", lorebook):
    title, end = parse_string(lorebook, m.end() - 1)
    m2 = re.compile(r"BookAuthor\s*=\s*\"").search(lorebook, end, end + 400)
    if m2:
        author, _ = parse_string(lorebook, m2.end() - 1)
        authors[title] = author

for m in re.finditer(r"book\.BookTitle\s*==\s*\"", lorebook):
    title, end = parse_string(lorebook, m.end() - 1)
    assign = lorebook.find("book.BookText =", end)
    if assign < 0:
        continue
    stop = statement_end(lorebook, assign)
    text = lorebook[assign + len("book.BookText ="):stop]
    author = authors.get(title, "")
    heading = "Book: %s%s" % (title, (" - by " + author) if author else "")
    add(heading, expr_text(text))

# --- DynamicBook --------------------------------------------------------------
dynamic = read(WORLD / "Source/Scripts/Items/Books/DynamicBook.cs")
for m in re.finditer(r"book is (\w+)", dynamic):
    typ = m.group(1)
    assign = dynamic.find("book.BookText =", m.end())
    if assign < 0:
        continue
    nxt = re.search(r"book is \w+", dynamic[m.end():])
    limit = m.end() + (nxt.start() if nxt else len(dynamic) - m.end())
    if assign > limit:
        continue
    stop = statement_end(dynamic, assign)
    text = dynamic[assign + len("book.BookText ="):stop]
    title = book_titles.get(typ, prettify(typ))
    add("Book: " + title, expr_text(text))

# --- Notes --------------------------------------------------------------------
notes = read(WORLD / "Source/Scripts/Items/Books/BardsTaleNote.cs")
clues = []
for m in re.finditer(r"ScrollMessage\s*=\s*\"", notes):
    text, _ = parse_string(notes, m.end() - 1)
    clues.append(text)
if clues:
    add("Book: A Bard's Tale (scattered notes)", "\n".join("- " + c for c in clues))

doom = read(WORLD / "Source/Scripts/Items/Books/DoomFlayerNote.cs")
for m in re.finditer(r"string sText\s*=\s*\"", doom):
    text, _ = parse_string(doom, m.end() - 1)
    add("Note: The Doom Flayer", text)

# --- NPC talk -----------------------------------------------------------------
talk = read(WORLD / "Source/Scripts/System/Misc/Talk.cs")
start = talk.find("public static string SpeechText")
end = talk.find("return sText;", start)
body = talk[start:end]
starts = list(re.finditer(r"sConversation\s*==\s*\"", body))
for idx, m in enumerate(starts):
    name, branch_start = parse_string(body, m.end() - 1)
    if name in SKIP_TALK:
        continue
    branch_end = starts[idx + 1].start() if idx + 1 < len(starts) else len(body)
    branch = body[branch_start:branch_end]
    speaker = SPEAKERS.get(name, "the " + prettify(name))
    locals_map = {}
    for lm in re.finditer(r"(?:string\s+)?(\w+)\s*=\s*(?!=)", branch):
        var = lm.group(1)
        if var in ("sText", "sYourName", "sMyName") or var in locals_map:
            continue
        stop = statement_end(branch, lm.end())
        value = expr_text(branch[lm.end():stop], speaker)
        if value is not None:
            locals_map[var] = value
    chunks = []
    for am in re.finditer(r"sText\s*=\s*(?!=)", branch):
        stop = statement_end(branch, am.end())
        text = expr_text(branch[am.end():stop], speaker, locals_map)
        if text:
            chunks.append(text)
    for am in re.finditer(r"sText\s*\+=\s*", branch):
        stop = statement_end(branch, am.end())
        text = expr_text(branch[am.end():stop], speaker, locals_map)
        if text:
            chunks.append(text)
    for am in re.finditer(r"builder\.Append(?:Line)?\s*\(", branch):
        stop = statement_end(branch, am.end())
        text = expr_text(branch[am.end():stop], speaker, locals_map)
        if text:
            chunks.append(text)
    seen = set()
    unique = []
    for chunk in chunks:
        if chunk not in seen:
            seen.add(chunk)
            unique.append(chunk)
    add("NPC: " + name, "\n\n".join(unique))

# --- Notes and parchments -----------------------------------------------------
# Fixed notes only. Procedural notes (ScrollClue, SearchPage, AssassinNote,
# GuardNote, ThiefNote) are generated per quest with random names/places and
# are intentionally left out; add their fixed templates here if desired.
notice = read(WORLD / "Source/Scripts/Engines and Systems/Quests/NoticeClue.cs")
clues = []
for m in re.finditer(r"new Server\.Gumps\.ClueGump\(\s*from\s*,\s*\"", notice):
    text, end = parse_string(notice, m.end() - 1)
    tm = re.match(r"\s*,\s*\"", notice[end:])
    if tm:
        title, _ = parse_string(notice, end + tm.end() - 1)
        clues.append((title, text))
if clues:
    clue_lines = ["Clues:", ""]
    for title, text in clues:
        clue_lines.append("- %s: %s" % (title, text))
    add("Notes: Notice Clues", "\n".join(clue_lines))

wanted = read(WORLD / "Source/Scripts/Items/Books/BulletinBoards/WantedMangar.cs")
m = re.search(r'">You are trapped', wanted)
if m:
    text, _ = parse_string(wanted, m.start())
    text = text.lstrip(">/ ")
    add("Notes: Wanted Poster of Mangar", "WANTED: Mangar the Dark. " + text)

scattered = read(WORLD / "Source/Scripts/Engines and Systems/Quests/SomeRandomNote.cs")
lies = scattered.find('if ( written == "lies" )')
truth = scattered.find("\n\t\t\telse\n\t\t\t{\n\t\t\t\tswitch ( amnt )", lies)
end = scattered.find("public override void OnDoubleClick", truth)
if truth >= 0 and end > truth:
    truth_source = scattered[truth:end]
    notes = []
    for m in re.finditer(r"ScrollMessage\s*=\s*", truth_source):
        stop = statement_end(truth_source, m.end())
        text = expr_text(truth_source[m.end():stop])
        if text and text not in notes:
            notes.append(text)
    if notes:
        note_lines = [
            "Fragments of old lore:",
            "",
        ]
        note_lines.extend("- " + text for text in notes)
        add("Notes: Scattered Parchments", "\n".join(note_lines))

# --- Tribute relics -----------------------------------------------------------
summon = read(WORLD / "Source/Scripts/Engines and Systems/Quests/Summon/SummonPrison.cs")
tribute = []
for m in re.finditer(r"case\s+(\d+):", summon):
    num = int(m.group(1))
    if num < 1 or num > 60:
        continue
    end = summon.find("break;", m.end())
    if end < 0:
        continue
    chunk = summon[m.end():end]
    mw = re.search(r'sWhere\s*=\s*"', chunk)
    mh = re.search(r'sWho\s*=\s*"', chunk)
    mt = re.search(r'sWhat\s*=\s*"', chunk)
    if not (mw and mh and mt):
        continue
    where, _ = parse_string(chunk, mw.end() - 1)
    who, _ = parse_string(chunk, mh.end() - 1)
    what, _ = parse_string(chunk, mt.end() - 1)
    tribute.append((num, where, who, what))

tribute.sort()
if tribute:
    tribute_lines = [
        "Those who seek a gift of tribute from the great powers of the land must bring them a rare relic.",
        "Each relic is held by the guardian named below, in the dungeon or place named with it.",
        "",
    ]
    for num, where, who, what in tribute:
        if num != 60:
            tribute_lines.append("- %s - held by %s in %s." % (title_case(what), who, where))
    tribute_lines.append("")
    tribute_lines.append(
        "The Magical Prisons also use a sixtieth relic, the Scale of Scarthis, held by a silver drake "
        "in the Sanctum of Saltmarsh; it is a prison key, not a tribute."
    )
    add("Tribute Relics", "\n".join(tribute_lines))

# --- Prophecies and rare relics ----------------------------------------------
sherry = read(WORLD / "Source/Scripts/Mobiles/Civilized/Sherry.cs")
sayings = []
for m in re.finditer(r'mouse\.Say\("', sherry):
    text, _ = parse_string(sherry, m.end() - 1)
    if text and text not in sayings:
        sayings.append(text)

rewrites = {
    "Oft have I wished that stranger would return.": "The stranger is wished for, and may yet return.",
    "We must bring the shards into harmony, so that they resonate in such a manner that matches the original universe.": "The shards must be brought into harmony, so that they resonate in a manner that matches the original universe.",
    "I witnessed them all from my tiny mousehole.": "All of it was witnessed from a hidden place.",
    "But I am but a mouse, and none hear me.": "A small voice speaks, and none hear.",
}
prophecies = [rewrites.get(saying, saying) for saying in sayings]
if prophecies:
    add("Prophecies", "Fragments of prophecy and sayings:\n\n" + "\n".join("- " + prophecy for prophecy in prophecies))

tavern = read(WORLD / "Source/Scripts/Mobiles/Civilized/Citizens/TavernPatrons.cs")
rare_start = tavern.find("public static string GetRareLocation")
rare_end = tavern.find("public static string GetEvilTitle", rare_start)
rare_source = tavern[rare_start:rare_end] if rare_start >= 0 and rare_end > rare_start else ""
rare_items = []
for m in re.finditer(r'what\s*=\s*"', rare_source):
    text, _ = parse_string(rare_source, m.end() - 1)
    if text and text not in rare_items:
        rare_items.append(text)
if rare_items:
    add("Rare Relics", "Rare powers and relics of the world:\n\n" + "\n".join("- " + item for item in rare_items))

pair_lines = []
for num, where, who, what in tribute:
    if num != 60:
        pair_lines.append("- %s - %s." % (title_case(what), where))
if pair_lines:
    add("Relic Places", "Relics and the places they are found:\n\n" + "\n".join(pair_lines))

# --- Write --------------------------------------------------------------------
primer = (
    "The land is Sosaria, an old world of kingdoms, virtues, and ruins. Britain, "
    "Skara Brae, Umbra and many smaller towns and villages are home to merchants, "
    "guildmasters, healers, sages and wandering folk. Moongates bind the towns and "
    "distant places together, while dungeons, tombs and forgotten places below the "
    "earth hold monsters, traps and relics of older ages. Virtues are honoured at "
    "shrines, guilds teach their trades, and adventurers seek artefacts of power. "
    "Old wars, fallen kingdoms and pacts with demons have left their marks "
    "everywhere; the books and the talk of the townsfolk remember them, though "
    "often imperfectly."
)

lines = [
    "; Gypsy lore reference. ';' lines are comments, '# ' lines start a section.",
    "; Generated by World/Documentation/Build-GypsyLore.py from the shard's books",
    "; and NPC conversations. Re-running the script overwrites this file.",
    "",
    "# World Primer",
    primer,
    "",
]
for title, text in sections:
    lines.extend(["# " + title, text, ""])

OUT.write_text("\n".join(lines), encoding="utf-8")
print("Wrote %s: %d sections, %d bytes" % (OUT, len(sections) + 1, OUT.stat().st_size))

