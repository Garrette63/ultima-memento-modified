using System;
using System.Collections.Generic;

namespace Server.Items
{
	public static partial class ZorkGame
	{
		// ---------------------------------------------------------------------
		// Room graph, object table and exit gates ported from the Zork I ZIL source
		// (github.com/historicalsource/zork1). Room/object text and structure are
		// reproduced under its MIT licence, (c) Copyright 2025 Microsoft.
		// ~110 rooms, every exit, every object placement and every exit gate.
		// ---------------------------------------------------------------------

		private static Dictionary<string, ZRoom> BuildRooms()
		{
			Dictionary<string, ZRoom> d = new Dictionary<string, ZRoom>();

			d["westofhouse"] = new ZRoom( "West of House", "You are standing in an open field west of a white house, with a boarded front door and a small mailbox.", false, 0 )
				.Exit( "north", "northofhouse" )
				.Exit( "south", "southofhouse" )
				.Exit( "ne", "northofhouse" )
				.Exit( "se", "southofhouse" )
				.Exit( "west", "forest1" )
				.Wall( "east", "The door is boarded and you can't remove the boards." )
				.Exit( "sw", "stonebarrow" )
				.Exit( "in", "stonebarrow" );

			d["stonebarrow"] = new ZRoom( "Stone Barrow", "You are standing in front of a massive barrow of stone. In the east face is a huge stone door which is open. You cannot see into the dark of the tomb.", false, 0 )
				.Exit( "ne", "westofhouse" );

			d["northofhouse"] = new ZRoom( "North of House", "You are facing the north side of a white house. There is no door here, and all the windows are boarded up. To the north a narrow path winds through the trees.", false, 0 )
				.Exit( "sw", "westofhouse" )
				.Exit( "se", "eastofhouse" )
				.Exit( "west", "westofhouse" )
				.Exit( "east", "eastofhouse" )
				.Exit( "north", "path" )
				.Wall( "south", "The windows are all boarded." );

			d["southofhouse"] = new ZRoom( "South of House", "You are facing the south side of a white house. There is no door here, and all the windows are boarded.", false, 0 )
				.Exit( "west", "westofhouse" )
				.Exit( "east", "eastofhouse" )
				.Exit( "ne", "eastofhouse" )
				.Exit( "nw", "westofhouse" )
				.Exit( "south", "forest3" )
				.Exit( "se", "forest3" )   // mirrored 14/06/2026: forest3's NW comes here
				.Wall( "north", "The windows are all boarded." );

			d["eastofhouse"] = new ZRoom( "Behind House", "You are behind the white house. A small window is slightly ajar, and a path leads east into the trees.", false, 0 )
				.Exit( "north", "northofhouse" )
				.Exit( "south", "southofhouse" )
				.Exit( "sw", "southofhouse" )
				.Exit( "nw", "northofhouse" )
				.Exit( "east", "clearing" )
				.Exit( "west", "kitchen" )
				.Exit( "in", "kitchen" );

			d["forest1"] = new ZRoom( "Forest", "This is a forest, with trees in all directions. To the east, there appears to be sunlight.", false, 0 )
				.Wall( "up", "There is no tree here suitable for climbing." )
				.Exit( "north", "gratingclearing" )
				.Exit( "east", "path" )
				.Exit( "south", "forest3" )
				.Wall( "west", "You would need a machete to go further west." );

			d["forest2"] = new ZRoom( "Forest", "This is a dimly lit forest, with large trees all around.", false, 0 )
				.Wall( "up", "There is no tree here suitable for climbing." )
				.Wall( "north", "The forest becomes impenetrable to the north." )
				.Exit( "east", "mountains" )
				.Exit( "south", "clearing" )
				.Exit( "west", "path" );

			d["mountains"] = new ZRoom( "Forest", "The forest thins out, revealing impassable mountains.", false, 0 )
				.Wall( "up", "The mountains are impassable." )
				.Exit( "north", "forest2" )
				.Wall( "east", "The mountains are impassable." )
				.Exit( "south", "forest2" )
				.Exit( "west", "forest2" );

			d["forest3"] = new ZRoom( "Forest", "This is a dimly lit forest, with large trees all around.", false, 0 )
				.Wall( "up", "There is no tree here suitable for climbing." )
				.Exit( "north", "clearing" )
				.Wall( "east", "The rank undergrowth prevents eastward movement." )
				.Wall( "south", "Storm-tossed trees block your way." )
				.Exit( "west", "forest1" )
				.Exit( "nw", "southofhouse" );

			d["path"] = new ZRoom( "Forest Path", "This is a path winding through a dimly lit forest. The path heads north-south here. One particularly large tree with some low branches stands at the edge of the path.", false, 0 )
				.Exit( "up", "upatree" )
				.Exit( "north", "gratingclearing" )
				.Exit( "east", "forest2" )
				.Exit( "south", "northofhouse" )
				.Exit( "west", "forest1" );

			d["upatree"] = new ZRoom( "Up a Tree", "You are about 10 feet above the ground nestled among some large branches. A birds nest is here.", false, 0 )
				.Exit( "down", "path" )
				.Wall( "up", "You cannot climb any higher." );

			d["gratingclearing"] = new ZRoom( "Clearing", "You are in a clearing with a pile of leaves and a grating set into the ground.", false, 0 )
				.Wall( "north", "The forest becomes impenetrable to the north." )
				.Exit( "east", "forest2" )
				.Exit( "west", "forest1" )
				.Exit( "south", "path" )
				.Exit( "down", "gratingroom" );

			d["clearing"] = new ZRoom( "Clearing", "You are in a small clearing in a well marked forest path that extends to the east and west.", false, 0 )
				.Wall( "up", "There is no tree here suitable for climbing." )
				.Exit( "se", "canyonview" )   // mirrored: canyon view's NW comes here
				.Exit( "east", "canyonview" )
				.Exit( "north", "forest2" )
				.Exit( "south", "forest3" )
				.Exit( "west", "eastofhouse" );

			d["kitchen"] = new ZRoom( "Kitchen", "You are in the kitchen of the white house. A table seems to have been used recently for the preparation of food. A passage leads to the west and a dark staircase can be seen leading upward. A dark chimney leads down and to the east is a small window which is slightly ajar.", false, 10 )
				.Exit( "east", "eastofhouse" )
				.Exit( "west", "livingroom" )
				.Exit( "out", "eastofhouse" )
				.Exit( "up", "attic" )
				.Exit( "down", "studio" );

			d["attic"] = new ZRoom( "Attic", "This is the attic. The only exit is a stairway leading down.", true, 0 )
				.Exit( "down", "kitchen" );

			d["livingroom"] = new ZRoom( "Living Room", "You are in the living room. There is a doorway to the east, a wooden door with strange gothic lettering to the west which appears to be nailed shut, a trophy case, and a large oriental rug in the center of the room.", false, 0 )
				.Exit( "east", "kitchen" )
				.Exit( "west", "strangepassage" )
				.Exit( "down", "cellar" );

			d["cellar"] = new ZRoom( "Cellar", "You are in a dark and damp cellar with a narrow passageway leading north, and a crawlway to the south. On the west is the bottom of a steep metal ramp which is unclimbable.", true, 25 )
				.Exit( "north", "trollroom" )
				.Exit( "south", "eastofchasm" )
				.Exit( "up", "livingroom" )
				.Wall( "west", "You try to ascend the ramp, but it is impossible, and you slide back down." );

			d["trollroom"] = new ZRoom( "The Troll Room", "This is a small room with passages to the east and south and a forbidding hole leading west. Bloodstains and deep scratches (perhaps made by an axe) mar the walls.", true, 0 )
				.Exit( "south", "cellar" )
				.Exit( "east", "ewpassage" )
				.Exit( "west", "maze1" );

			d["eastofchasm"] = new ZRoom( "East of Chasm", "You are on the east edge of a chasm, the bottom of which cannot be seen. A narrow passage goes north, and the path you are on continues to the east.", true, 0 )
				.Exit( "north", "cellar" )
				.Exit( "east", "gallery" )
				.Wall( "down", "The chasm probably leads straight to the infernal regions." );

			d["gallery"] = new ZRoom( "Gallery", "This is an art gallery. Most of the paintings have been stolen by vandals with exceptional taste. The vandals left through either the north or west exits.", false, 0 )
				.Exit( "west", "eastofchasm" )
				.Exit( "north", "studio" );

			d["studio"] = new ZRoom( "Studio", "This appears to have been an artist's studio. The walls and floors are splattered with paints of 69 different colors. Strangely enough, nothing of value is hanging here. At the south end of the room is an open door (also covered with paint). A dark and narrow chimney leads up from a fireplace; although you might be able to get up it, it seems unlikely you could get back down.", true, 0 )
				.Exit( "south", "gallery" )
				.Exit( "up", "kitchen" );

			d["maze1"] = new ZRoom( "Maze", "This is part of a maze of twisty little passages, all alike.", true, 0 )
				.Exit( "east", "trollroom" )
				.Exit( "north", "maze1" )
				.Exit( "south", "maze2" )
				.Exit( "west", "maze4" );

			d["maze2"] = new ZRoom( "Maze", "This is part of a maze of twisty little passages, all alike.", true, 0 )
				.Exit( "south", "maze1" )
				.Exit( "down", "maze9" )
				.Exit( "east", "maze3" );

			d["maze3"] = new ZRoom( "Maze", "This is part of a maze of twisty little passages, all alike.", true, 0 )
				.Exit( "west", "maze2" )
				.Exit( "north", "maze4" )
				.Exit( "up", "maze5" );

			d["maze4"] = new ZRoom( "Maze", "This is part of a maze of twisty little passages, all alike.", true, 0 )
				.Exit( "west", "maze3" )
				.Exit( "north", "maze1" )
				.Exit( "east", "deadend1" );

			d["deadend1"] = new ZRoom( "Dead End", "You have come to a dead end in the maze.", true, 0 )
				.Exit( "south", "maze4" );

			d["maze5"] = new ZRoom( "Maze", "This is part of a maze of twisty little passages, all alike. A skeleton, probably the remains of a luckless adventurer, lies here.", true, 0 )
				.Exit( "east", "deadend2" )
				.Exit( "north", "maze3" )
				.Exit( "sw", "maze6" );

			d["deadend2"] = new ZRoom( "Dead End", "You have come to a dead end in the maze.", true, 0 )
				.Exit( "west", "maze5" );

			d["maze6"] = new ZRoom( "Maze", "This is part of a maze of twisty little passages, all alike.", true, 0 )
				.Exit( "down", "maze5" )
				.Exit( "east", "maze7" )
				.Exit( "west", "maze6" )
				.Exit( "up", "maze9" );

			d["maze7"] = new ZRoom( "Maze", "This is part of a maze of twisty little passages, all alike.", true, 0 )
				.Exit( "up", "maze14" )
				.Exit( "west", "maze6" )
				.Exit( "down", "deadend1" )
				.Exit( "east", "maze8" )
				.Exit( "south", "maze15" );

			d["maze8"] = new ZRoom( "Maze", "This is part of a maze of twisty little passages, all alike.", true, 0 )
				.Exit( "ne", "maze7" )
				.Exit( "west", "maze8" )
				.Exit( "se", "deadend3" );

			d["deadend3"] = new ZRoom( "Dead End", "You have come to a dead end in the maze.", true, 0 )
				.Exit( "north", "maze8" );

			d["maze9"] = new ZRoom( "Maze", "This is part of a maze of twisty little passages, all alike.", true, 0 )
				.Exit( "north", "maze6" )
				.Exit( "down", "maze11" )
				.Exit( "east", "maze10" )
				.Exit( "south", "maze13" )
				.Exit( "west", "maze12" )
				.Exit( "nw", "maze9" );

			d["maze10"] = new ZRoom( "Maze", "This is part of a maze of twisty little passages, all alike.", true, 0 )
				.Exit( "east", "maze9" )
				.Exit( "west", "maze13" )
				.Exit( "up", "maze11" );

			d["maze11"] = new ZRoom( "Maze", "This is part of a maze of twisty little passages, all alike.", true, 0 )
				.Exit( "ne", "gratingroom" )
				.Exit( "down", "maze10" )
				.Exit( "nw", "maze13" )
				.Exit( "sw", "maze12" );

			d["gratingroom"] = new ZRoom( "Grating Room", "You are in a small room near the surface, with a grating overhead.", true, 0 )
				.Exit( "sw", "maze11" )
				.Exit( "up", "gratingclearing" );

			d["maze12"] = new ZRoom( "Maze", "This is part of a maze of twisty little passages, all alike.", true, 0 )
				.Exit( "down", "maze5" )
				.Exit( "sw", "maze11" )
				.Exit( "east", "maze13" )
				.Exit( "up", "maze9" )
				.Exit( "north", "deadend4" );

			d["deadend4"] = new ZRoom( "Dead End", "You have come to a dead end in the maze.", true, 0 )
				.Exit( "south", "maze12" );

			d["maze13"] = new ZRoom( "Maze", "This is part of a maze of twisty little passages, all alike.", true, 0 )
				.Exit( "east", "maze9" )
				.Exit( "down", "maze12" )
				.Exit( "south", "maze10" )
				.Exit( "west", "maze11" );

			d["maze14"] = new ZRoom( "Maze", "This is part of a maze of twisty little passages, all alike.", true, 0 )
				.Exit( "west", "maze15" )
				.Exit( "nw", "maze14" )
				.Exit( "ne", "maze7" )
				.Exit( "south", "maze7" );

			d["maze15"] = new ZRoom( "Maze", "This is part of a maze of twisty little passages, all alike.", true, 0 )
				.Exit( "west", "maze14" )
				.Exit( "south", "maze7" )
				.Exit( "se", "cyclopsroom" );

			d["cyclopsroom"] = new ZRoom( "Cyclops Room", "This room has an exit on the west side, and a narrow staircase leading up. A hungry-looking cyclops stands here.", true, 0 )
				.Exit( "nw", "maze15" )
				.Exit( "east", "strangepassage" )
				.Exit( "up", "treasureroom" );

			d["strangepassage"] = new ZRoom( "Strange Passage", "This is a long passage. To the west is one entrance. On the east there is an old wooden door, with a large opening in it (about cyclops sized).", true, 0 )
				.Exit( "west", "cyclopsroom" )
				.Exit( "in", "cyclopsroom" )
				.Exit( "east", "livingroom" );

			d["treasureroom"] = new ZRoom( "Treasure Room", "This is a large room, whose east wall is solid granite. A number of discarded bags, which crumble at your touch, are scattered about on the floor. There is an exit down a staircase.", true, 25 )
				.Exit( "down", "cyclopsroom" );

			d["reservoirsouth"] = new ZRoom( "Reservoir South", "You are on what used to be the south shore of a large reservoir. The water is now low.", true, 0 )
				.Exit( "se", "deepcanyon" )
				.Exit( "sw", "chasmroom" )
				.Exit( "east", "damroom" )
				.Exit( "west", "streamview" )
				.Exit( "north", "reservoir" );

			d["reservoir"] = new ZRoom( "Reservoir", "You are in a large reservoir. The water is deep and cold.", true, 0 )
				.Exit( "north", "reservoirnorth" )
				.Exit( "south", "reservoirsouth" )
				.Exit( "up", "instream" )
				.Exit( "west", "instream" )
				.Wall( "down", "The dam blocks your way." );

			d["reservoirnorth"] = new ZRoom( "Reservoir North", "You are on the north shore of the reservoir. A hand pump lies here.", true, 0 )
				.Exit( "north", "atlantisroom" )
				.Exit( "south", "reservoir" );

			d["streamview"] = new ZRoom( "Stream View", "You are standing on a path beside a gently flowing stream. The path follows the stream, which flows from west to east.", true, 0 )
				.Exit( "east", "reservoirsouth" )
				.Wall( "west", "The stream emerges from a spot too small for you to enter." );

			d["instream"] = new ZRoom( "Stream", "You are on the gently flowing stream. The upstream route is too narrow to navigate, and the downstream route is invisible due to twisting walls. There is a narrow beach to land on.", true, 0 )
				.Wall( "up", "The channel is too narrow." )
				.Wall( "west", "The channel is too narrow." )
				.Exit( "land", "streamview" )
				.Exit( "down", "reservoir" )
				.Exit( "east", "reservoir" );

			d["mirrorroom1"] = new ZRoom( "Mirror Room", "You are in a large square room with a polished surface on the north wall, and passages to the south and east.", true, 0 )
				.Exit( "north", "coldpassage" )
				.Exit( "west", "twistingpassage" )
				.Exit( "east", "smallcave" );

			d["mirrorroom2"] = new ZRoom( "Mirror Room", "You are in a room with a large mirror on the south wall. Passages lead north, south and west.", false, 0 )
				.Exit( "west", "windingpassage" )
				.Exit( "north", "narrowpassage" )
				.Exit( "east", "tinycave" );

			d["smallcave"] = new ZRoom( "Cave", "This is a tiny cave with entrances west and north, and a staircase leading down.", true, 0 )
				.Exit( "north", "mirrorroom1" )
				.Exit( "down", "atlantisroom" )
				.Exit( "south", "atlantisroom" )
				.Exit( "west", "twistingpassage" );

			d["tinycave"] = new ZRoom( "Cave", "This is a tiny cave with entrances west and north, and a dark, forbidding staircase leading down.", true, 0 )
				.Exit( "north", "mirrorroom2" )
				.Exit( "west", "windingpassage" )
				.Exit( "down", "entrancetohades" );

			d["coldpassage"] = new ZRoom( "Cold Passage", "This is a cold and damp corridor where a long east-west passageway turns into a southward path.", true, 0 )
				.Exit( "south", "mirrorroom1" )
				.Exit( "west", "slideroom" );

			d["narrowpassage"] = new ZRoom( "Narrow Passage", "This is a long and narrow corridor where a long north-south passageway briefly narrows even further.", true, 0 )
				.Exit( "north", "roundroom" )
				.Exit( "south", "mirrorroom2" );

			d["windingpassage"] = new ZRoom( "Winding Passage", "This is a winding passage. It seems that there are only exits on the east and north.", true, 0 )
				.Exit( "north", "mirrorroom2" )
				.Exit( "east", "tinycave" );

			d["twistingpassage"] = new ZRoom( "Twisting Passage", "This is a winding passage. It seems that there are only exits on the east and north.", true, 0 )
				.Exit( "north", "mirrorroom1" )
				.Exit( "east", "smallcave" );

			d["atlantisroom"] = new ZRoom( "Atlantis Room", "This is an ancient room, long under water. There is an exit to the south and a staircase leading up.", true, 0 )
				.Exit( "up", "smallcave" )
				.Exit( "south", "reservoirnorth" );

			d["ewpassage"] = new ZRoom( "East-West Passage", "This is a narrow east-west passageway. There is a narrow stairway leading down at the north end of the room.", true, 5 )
				.Exit( "east", "roundroom" )
				.Exit( "west", "trollroom" )
				.Exit( "down", "chasmroom" )
				.Exit( "ne", "chasmroom" )   // mirrored: the chasm's SW comes here
				.Exit( "north", "chasmroom" );

			d["roundroom"] = new ZRoom( "Round Room", "This is a circular stone room with passages in all directions. Several of them have unfortunately been blocked by cave-ins.", true, 0 )
				.Exit( "east", "loudroom" )
				.Exit( "west", "ewpassage" )
				.Exit( "north", "nspassage" )
				.Exit( "south", "narrowpassage" )
				.Exit( "se", "engravingscave" );

			d["deepcanyon"] = new ZRoom( "Deep Canyon", "You are on the rim of a deep canyon; the path follows the rim southwest to northeast.", true, 0 )
				.Exit( "nw", "reservoirsouth" )
				.Exit( "east", "damroom" )
				.Exit( "sw", "nspassage" )
				.Exit( "down", "loudroom" );

			d["dampcave"] = new ZRoom( "Damp Cave", "This cave has exits to the west and east, and narrows to a crack toward the south. The earth is particularly damp here.", true, 0 )
				.Exit( "west", "loudroom" )
				.Exit( "east", "whitecliffsnorth" )
				.Wall( "south", "It is too narrow for most insects." );

			d["loudroom"] = new ZRoom( "Loud Room", "This is a large room with a ceiling which cannot be detected from the ground. There is a narrow passage from east to west and a stone stairway leading upward.", true, 0 )
				.Exit( "east", "dampcave" )
				.Exit( "west", "roundroom" )
				.Exit( "up", "deepcanyon" );

			d["nspassage"] = new ZRoom( "North-South Passage", "This is a high north-south passage, which forks to the northeast.", true, 0 )
				.Exit( "north", "chasmroom" )
				.Exit( "ne", "deepcanyon" )
				.Exit( "south", "roundroom" );

			d["chasmroom"] = new ZRoom( "Chasm", "A chasm runs southwest to northeast and the path follows it. You are on the south side of the chasm, where a crack opens into a passage.", true, 0 )
				.Exit( "ne", "reservoirsouth" )
				.Exit( "sw", "ewpassage" )
				.Exit( "up", "ewpassage" )
				.Exit( "south", "nspassage" )
				.Wall( "down", "Are you out of your mind?" );

			d["entrancetohades"] = new ZRoom( "Entrance to Hades", "You are at the entrance to Hades. A grey mist swirls before you, and a great gate stands to the south.", false, 0 )
				.Exit( "up", "tinycave" )
				.Exit( "in", "landoflivingdead" )
				.Exit( "south", "landoflivingdead" );

			d["landoflivingdead"] = new ZRoom( "Land of the Dead", "You have entered the Land of the Living Dead. Thousands of lost souls can be heard weeping and moaning. In the corner are stacked the remains of dozens of previous adventurers less fortunate than yourself. A passage exits to the north.", false, 0 )
				.Exit( "out", "entrancetohades" )
				.Exit( "north", "entrancetohades" );

			d["engravingscave"] = new ZRoom( "Engravings Cave", "You have entered a low cave with passages leading northwest and east.", true, 0 )
				.Exit( "nw", "roundroom" )
				.Exit( "east", "domeroom" );

			d["egyptroom"] = new ZRoom( "Egyptian Room", "This is a room which looks like an Egyptian tomb. There is an ascending staircase to the west.", true, 0 )
				.Exit( "west", "northtemple" )
				.Exit( "up", "northtemple" );

			d["domeroom"] = new ZRoom( "Dome Room", "You are at the periphery of a large dome, which forms the ceiling of another room below. Protecting you from a precipitous drop is a wooden railing which circles the dome.", true, 0 )
				.Exit( "west", "engravingscave" )
				.Exit( "down", "torchroom" );

			d["torchroom"] = new ZRoom( "Torch Room", "This is a large room with a prominent doorway leading to a down staircase. Above you is a large dome. Up around the edge of the dome (20 feet up) is a wooden railing. In the center of the room sits a white marble pedestal.", true, 0 )
				.Wall( "up", "You cannot reach the rope." )
				.Exit( "south", "northtemple" )
				.Exit( "down", "northtemple" );

			d["northtemple"] = new ZRoom( "Temple", "This is the north end of a large temple. On the east wall is an ancient inscription, probably a prayer in a long-forgotten language. Below the prayer is a staircase leading down. The west wall is solid granite. The exit to the north end of the room is through huge marble pillars.", false, 0 )
				.Exit( "down", "egyptroom" )
				.Exit( "east", "egyptroom" )
				.Exit( "north", "torchroom" )
				.Exit( "out", "torchroom" )
				.Exit( "up", "torchroom" )
				.Exit( "south", "southtemple" );

			d["southtemple"] = new ZRoom( "Altar", "This is the south end of a large temple. In front of you is what appears to be an altar. In one corner is a small hole in the floor which leads into darkness. You probably could not get back up it.", false, 0 )
				.Exit( "north", "northtemple" )
				.Exit( "down", "tinycave" );

			d["damroom"] = new ZRoom( "Dam", "You are standing on the top of the Flood Control Dam #3, which was quite a tourist attraction in times far distant. There are paths to the north, south, and west, and a scramble down.", false, 0 )
				.Exit( "south", "deepcanyon" )
				.Exit( "down", "dambase" )
				.Exit( "east", "dambase" )
				.Exit( "north", "damlobby" )
				.Exit( "west", "reservoirsouth" );

			d["damlobby"] = new ZRoom( "Dam Lobby", "This room appears to have been the waiting room for groups touring the dam. There are open doorways here to the north and east marked \"Private\", and there is a path leading south over the top of the dam.", false, 0 )
				.Exit( "south", "damroom" )
				.Exit( "north", "maintenanceroom" )
				.Exit( "east", "maintenanceroom" );

			d["maintenanceroom"] = new ZRoom( "Maintenance Room", "This is what appears to have been the maintenance room for Flood Control Dam #3. Apparently, this room has been ransacked recently, for most of the valuable equipment is gone. On the wall in front of you is a group of buttons colored blue, yellow, brown, and red. There are doorways to the west and south.", true, 0 )
				.Exit( "south", "damlobby" )
				.Exit( "west", "damlobby" );

			d["dambase"] = new ZRoom( "Dam Base", "You are at the base of Flood Control Dam #3, which looms above you and to the north. The river Frigid is flowing by here. Along the river are the White Cliffs which seem to form giant walls stretching from north to south along the shores of the river as it winds its way downstream.", false, 0 )
				.Exit( "north", "damroom" )
				.Exit( "up", "damroom" );

			d["river1"] = new ZRoom( "Frigid River", "You are on the Frigid River in the vicinity of the Dam. The river flows quietly here. There is a landing on the west shore.", false, 0 )
				.Wall( "up", "You cannot go upstream due to strong currents." )
				.Exit( "west", "dambase" )
				.Exit( "land", "dambase" )
				.Exit( "down", "river2" )
				.Wall( "east", "The White Cliffs prevent your landing here." );

			d["river2"] = new ZRoom( "Frigid River", "The river turns a corner here making it impossible to see the Dam. The White Cliffs loom on the east bank and large rocks prevent landing on the west.", true, 0 )
				.Wall( "up", "You cannot go upstream due to strong currents." )
				.Exit( "down", "river3" )
				.Wall( "land", "There is no safe landing spot here." )
				.Wall( "east", "The White Cliffs prevent your landing here." )
				.Wall( "west", "Just in time you steer away from the rocks." );

			d["river3"] = new ZRoom( "Frigid River", "The river descends here into a valley. There is a narrow beach on the west shore below the cliffs. In the distance a faint rumbling can be heard.", true, 0 )
				.Wall( "up", "You cannot go upstream due to strong currents." )
				.Exit( "down", "river4" )
				.Exit( "land", "whitecliffsnorth" )
				.Exit( "west", "whitecliffsnorth" );

			d["whitecliffsnorth"] = new ZRoom( "White Cliffs Beach", "You are on a narrow strip of beach which runs along the base of the White Cliffs. There is a narrow path heading south along the Cliffs and a tight passage leading west into the cliffs themselves.", true, 0 )
				.Exit( "south", "whitecliffssouth" )
				.Exit( "west", "dampcave" );

			d["whitecliffssouth"] = new ZRoom( "White Cliffs Beach", "You are on a rocky, narrow strip of beach beside the Cliffs. A narrow path leads north along the shore.", true, 0 )
				.Exit( "north", "whitecliffsnorth" );

			d["river4"] = new ZRoom( "Frigid River", "The river is running faster here and the sound ahead appears to be that of rushing water. On the east shore is a sandy beach. A small area of beach can also be seen below the cliffs on the west shore.", true, 0 )
				.Wall( "up", "You cannot go upstream due to strong currents." )
				.Exit( "down", "river5" )
				.Wall( "land", "You can land either to the east or the west." )
				.Exit( "west", "whitecliffssouth" )
				.Exit( "east", "sandybeach" );

			d["river5"] = new ZRoom( "Frigid River", "The sound of rushing water is nearly unbearable here. On the east shore is a large landing area.", false, 0 )
				.Wall( "up", "You cannot go upstream due to strong currents." )
				.Exit( "east", "shore" )
				.Exit( "land", "shore" );

			d["shore"] = new ZRoom( "Shore", "You are on the east shore of the river. The water here seems somewhat treacherous. A path travels from north to south here, the south end quickly turning around a sharp corner.", false, 0 )
				.Exit( "north", "sandybeach" )
				.Exit( "south", "aragainfalls" );

			d["sandybeach"] = new ZRoom( "Sandy Beach", "You are on a large sandy beach on the east shore of the river, which is flowing quickly by. A path runs beside the river to the south here, and a passage is partially buried in sand to the northeast.", true, 0 )
				.Exit( "ne", "sandycave" )
				.Exit( "south", "shore" );

			d["sandycave"] = new ZRoom( "Sandy Cave", "This is a sand-filled cave whose exit is to the southwest.", true, 0 )
				.Exit( "sw", "sandybeach" );

			d["aragainfalls"] = new ZRoom( "Aragain Falls", "You are at the top of Aragain Falls. A rainbow spans the gorge.", false, 0 )
				.Exit( "west", "onrainbow" )
				.Wall( "down", "It's a long way..." )
				.Exit( "north", "shore" )
				.Exit( "up", "onrainbow" );

			d["onrainbow"] = new ZRoom( "On the Rainbow", "You are on top of a rainbow (I bet you never thought you would walk on a rainbow), with a magnificent view of the Falls. The rainbow travels east-west here.", false, 0 )
				.Exit( "sw", "endofrainbow" )   // mirrored: the far end's NE comes here
				.Exit( "west", "endofrainbow" )
				.Exit( "east", "aragainfalls" );

			d["endofrainbow"] = new ZRoom( "End of Rainbow", "You are on a small, rocky beach on the continuation of the Frigid River past the Falls. The beach is narrow due to the presence of the White Cliffs. The river canyon opens here and sunlight shines in from above. A rainbow crosses over the falls to the east and a narrow path continues to the southwest.", false, 0 )
				.Exit( "up", "onrainbow" )
				.Exit( "ne", "onrainbow" )
				.Exit( "east", "onrainbow" )
				.Exit( "sw", "canyonbottom" );

			d["canyonbottom"] = new ZRoom( "Canyon Bottom", "You are beneath the walls of the river canyon which may be climbable here. The lesser part of the runoff of Aragain Falls flows by below. To the north is a narrow path.", false, 0 )
				.Exit( "up", "cliffmiddle" )
				.Exit( "north", "endofrainbow" );

			d["cliffmiddle"] = new ZRoom( "Rocky Ledge", "You are on a ledge about halfway up the wall of the river canyon. You can see from here that the main flow from Aragain Falls twists along a passage which it is impossible for you to enter. Below you is the canyon bottom. Above you is more cliff, which appears climbable.", false, 0 )
				.Exit( "up", "canyonview" )
				.Exit( "down", "canyonbottom" );

			d["canyonview"] = new ZRoom( "Canyon View", "You are at the top of the Great Canyon on its west wall. From here there is a marvelous view of the canyon and parts of the Frigid River upstream. Across the canyon, the walls of the White Cliffs join the mighty ramparts of the Flathead Mountains to the east. Following the Canyon upstream to the north, Aragain Falls may be seen, complete with rainbow. The mighty Frigid River flows out from a great dark cavern. To the west and south can be seen an immense forest, stretching for miles around. A path leads northwest. It is possible to climb down into the canyon from here.", false, 0 )
				.Exit( "east", "cliffmiddle" )
				.Exit( "down", "cliffmiddle" )
				.Exit( "nw", "clearing" )
				.Exit( "west", "forest3" )
				.Wall( "south", "Storm-tossed trees block your way." );

			d["mineentrance"] = new ZRoom( "Mine Entrance", "You are standing at the entrance of what might have been a coal mine. The shaft enters the west wall, and there is another exit on the south end of the room.", true, 0 )
				.Exit( "south", "slideroom" )
				.Exit( "in", "squeekyroom" )
				.Exit( "west", "squeekyroom" );

			d["squeekyroom"] = new ZRoom( "Squeaky Room", "You are in a small room. Strange squeaky sounds may be heard coming from the passage at the north end. You may also escape to the east.", true, 0 )
				.Exit( "north", "batroom" )
				.Exit( "east", "mineentrance" );

			d["batroom"] = new ZRoom( "Bat Room", "You are in a small room with a bat hanging from the ceiling.", true, 0 )
				.Exit( "south", "squeekyroom" )
				.Exit( "east", "shaftroom" );

			d["shaftroom"] = new ZRoom( "Shaft Room", "This is a large room, in the middle of which is a small shaft descending through the floor into darkness below. To the west and the north are exits from this room. Constructed over the top of the shaft is a metal framework to which a heavy iron chain is attached.", true, 0 )
				.Wall( "down", "You wouldn't fit and would die if you could." )
				.Exit( "west", "batroom" )
				.Exit( "north", "smellyroom" );

			d["smellyroom"] = new ZRoom( "Smelly Room", "This is a small nondescript room. However, from the direction of a small descending staircase a foul odor can be detected. To the south is a narrow tunnel.", true, 0 )
				.Exit( "down", "gasroom" )
				.Exit( "south", "shaftroom" );

			d["gasroom"] = new ZRoom( "Gas Room", "This is a small room which smells strongly of coal gas. There is a short climb up some stairs and a narrow tunnel leading east.", true, 0 )
				.Exit( "up", "smellyroom" )
				.Exit( "east", "mine1" );

			d["laddertop"] = new ZRoom( "Ladder Top", "This is a very small room. In the corner is a rickety wooden ladder, leading downward. It might be safe to descend. There is also a staircase leading upward.", true, 0 )
				.Exit( "down", "ladderbottom" )
				.Exit( "up", "mine4" );

			d["ladderbottom"] = new ZRoom( "Ladder Bottom", "This is a rather wide room. On one side is the bottom of a narrow wooden ladder. To the west and the south are passages leaving the room.", true, 0 )
				.Exit( "south", "deadend5" )
				.Exit( "west", "timberroom" )
				.Exit( "up", "laddertop" );

			d["deadend5"] = new ZRoom( "Dead End", "You have come to a dead end in the mine.", true, 0 )
				.Exit( "north", "ladderbottom" );

			d["timberroom"] = new ZRoom( "Timber Room", "This is a long and narrow passage, which is cluttered with broken timbers. A wide passage comes from the east and turns at the west end of the room into a very narrow passageway. From the west comes a strong draft.", true, 0 )
				.Exit( "east", "ladderbottom" )
				.Exit( "west", "lowershaft" );

			d["lowershaft"] = new ZRoom( "Drafty Room", "This is a small drafty room in which is the bottom of a long shaft. To the south is a passageway and to the east a very narrow passage. In the shaft can be seen a heavy iron chain.", true, 0 )
				.Exit( "south", "machineroom" )
				.Exit( "out", "timberroom" )
				.Exit( "east", "timberroom" );

			d["machineroom"] = new ZRoom( "Machine Room", "You are in a machine room, full of strange machinery and a switch.", true, 0 )
				.Exit( "north", "lowershaft" );

			d["mine1"] = new ZRoom( "Coal Mine", "This is a nondescript part of a coal mine.", true, 0 )
				.Exit( "north", "gasroom" )
				.Exit( "east", "mine1" )
				.Exit( "ne", "mine2" );

			d["mine2"] = new ZRoom( "Coal Mine", "This is a nondescript part of a coal mine.", true, 0 )
				.Exit( "north", "mine2" )
				.Exit( "south", "mine1" )
				.Exit( "se", "mine3" );

			d["mine3"] = new ZRoom( "Coal Mine", "This is a nondescript part of a coal mine.", true, 0 )
				.Exit( "south", "mine3" )
				.Exit( "sw", "mine4" )
				.Exit( "east", "mine2" );

			d["mine4"] = new ZRoom( "Coal Mine", "This is a nondescript part of a coal mine.", true, 0 )
				.Exit( "north", "mine3" )
				.Exit( "west", "mine4" )
				.Exit( "down", "laddertop" );

			d["slideroom"] = new ZRoom( "Slide Room", "This is a small chamber, which appears to have been part of a coal mine. On the south wall of the chamber the letters \"Granite Wall\" are etched in the rock. To the east is a long passage, and there is a steep metal slide twisting downward. To the north is a small opening.", true, 0 )
				.Exit( "east", "coldpassage" )
				.Exit( "north", "mineentrance" )
				.Exit( "down", "cellar" );

			return d;
		}

		private static Dictionary<string, ZGate> BuildGates()
		{
			Dictionary<string, ZGate> g = new Dictionary<string, ZGate>();

			g["gratingclearing|down"] = new ZGate( "GRATING-EXIT", "The grating is closed." );
			g["livingroom|down"] = new ZGate( "TRAP-DOOR-EXIT", "The trap door is closed." );
			g["studio|up"] = new ZGate( "UP-CHIMNEY-FUNCTION", "You can't go that way." );
			g["maze2|down"] = new ZGate( "MAZE-DIODES", "You can't go that way." );
			g["maze7|down"] = new ZGate( "MAZE-DIODES", "You can't go that way." );
			g["maze9|down"] = new ZGate( "MAZE-DIODES", "You can't go that way." );
			g["maze12|down"] = new ZGate( "MAZE-DIODES", "You can't go that way." );
			g["westofhouse|sw"] = new ZGate( "WON-FLAG", "You can't go that way." );
			g["westofhouse|in"] = new ZGate( "WON-FLAG", "You can't go that way." );
			g["eastofhouse|west"] = new ZGate( "KITCHEN-WINDOW-OPEN", "The window is closed." );
			g["eastofhouse|in"] = new ZGate( "KITCHEN-WINDOW-OPEN", "The window is closed." );
			g["kitchen|east"] = new ZGate( "KITCHEN-WINDOW-OPEN", "The window is closed." );
			g["kitchen|out"] = new ZGate( "KITCHEN-WINDOW-OPEN", "The window is closed." );
			g["kitchen|down"] = new ZGate( "FALSE-FLAG", "Only Santa Claus climbs down chimneys." );
			g["livingroom|west"] = new ZGate( "MAGIC-FLAG", "The door is nailed shut." );
			g["cellar|up"] = new ZGate( "TRAP-DOOR-OPEN", "The trap door is closed." );
			g["trollroom|east"] = new ZGate( "TROLL-FLAG", "The troll fends you off with a menacing gesture." );
			g["trollroom|west"] = new ZGate( "TROLL-FLAG", "The troll fends you off with a menacing gesture." );
			g["gratingroom|up"] = new ZGate( "GRATE-OPEN", "The grating is closed." );
			g["cyclopsroom|east"] = new ZGate( "MAGIC-FLAG", "The east wall is solid rock." );
			g["cyclopsroom|up"] = new ZGate( "CYCLOPS-FLAG", "The cyclops doesn't look like he'll let you past." );
			g["reservoirsouth|north"] = new ZGate( "LOW-TIDE", "The water is too deep." );
			g["reservoirnorth|south"] = new ZGate( "LOW-TIDE", "The water is too deep." );
			g["entrancetohades|in"] = new ZGate( "LLD-FLAG", "Some invisible force prevents you from passing through the gate." );
			g["entrancetohades|south"] = new ZGate( "LLD-FLAG", "Some invisible force prevents you from passing through the gate." );
			g["domeroom|down"] = new ZGate( "DOME-FLAG", "You cannot go down without fracturing many bones." );
			g["southtemple|down"] = new ZGate( "COFFIN-CURE", "You haven't a prayer of getting the coffin down there." );
			g["whitecliffsnorth|south"] = new ZGate( "DEFLATE", "The path is too narrow." );
			g["whitecliffsnorth|west"] = new ZGate( "DEFLATE", "The path is too narrow." );
			g["whitecliffssouth|north"] = new ZGate( "DEFLATE", "The path is too narrow." );
			g["aragainfalls|west"] = new ZGate( "RAINBOW-FLAG", "The rainbow is too insubstantial to walk on." );
			g["aragainfalls|up"] = new ZGate( "RAINBOW-FLAG", "The rainbow is too insubstantial to walk on." );
			g["endofrainbow|up"] = new ZGate( "RAINBOW-FLAG", "The rainbow is too insubstantial to walk on." );
			g["endofrainbow|ne"] = new ZGate( "RAINBOW-FLAG", "The rainbow is too insubstantial to walk on." );
			g["endofrainbow|east"] = new ZGate( "RAINBOW-FLAG", "The rainbow is too insubstantial to walk on." );
			g["timberroom|west"] = new ZGate( "EMPTY-HANDED", "You are carrying too much to fit through." );
			g["lowershaft|out"] = new ZGate( "EMPTY-HANDED", "You are carrying too much to fit through." );
			g["lowershaft|east"] = new ZGate( "EMPTY-HANDED", "You are carrying too much to fit through." );

			return g;
		}

		private static ZThing T( string name, string aliases, string desc )
		{
			ZThing t = new ZThing();
			t.Name = name;
			t.Aliases = aliases;
			t.Desc = desc;
			return t;
		}

		private static Dictionary<string, ZThing> BuildThings()
		{
			Dictionary<string, ZThing> d = new Dictionary<string, ZThing>();

			d["board"] = T( "board", "board boards", "board" );
			d["board"].Global = true;

			d["teeth"] = T( "set of teeth", "overboard teeth", "set of teeth" );
			d["teeth"].Global = true;

			d["wall"] = T( "surrounding wall", "surrounding wall walls", "surrounding wall" );
			d["wall"].Global = true;

			d["granitewall"] = T( "granite wall", "granite wall", "granite wall" );
			d["granitewall"].Global = true;

			d["songbird"] = T( "songbird", "bird song songbird", "songbird" );
			d["songbird"].Global = true;

			d["whitehouse"] = T( "white house", "beauti coloni house white", "white house" );
			d["whitehouse"].Global = true;

			d["forest"] = T( "forest", "forest hemlocks pines trees", "forest" );
			d["forest"].Global = true;

			d["tree"] = T( "tree", "branch large storm tree", "tree" );
			d["tree"].Global = true;

			d["mountainrange"] = T( "mountain range", "flathead impassable mountain range", "mountain range" );

			d["globalwater"] = T( "water", "quantity water", "water" );
			d["globalwater"].Global = true;

			d["water"] = T( "quantity of water", "h2o liquid quantity water", "quantity of water" );
			d["water"].Portable = true;
			d["water"].Size = 4;
			d["water"].Listed = true;

			d["kitchenwindow"] = T( "kitchen window", "kitchen small window", "kitchen window" );
			d["kitchenwindow"].Global = true;

			d["chimney"] = T( "chimney", "chimney dark narrow", "chimney" );
			d["chimney"].Global = true;

			d["ghosts"] = T( "number of ghosts", "evil fiends force ghosts invisible spirits", "number of ghosts" );
			d["ghosts"].Listed = true;

			d["skull"] = T( "crystal skull", "crystal head skull treasure", "crystal skull" );
			d["skull"].Portable = true;
			d["skull"].Value = 10;
			d["skull"].TValue = 10;
			d["skull"].Listed = true;

			d["loweredbasket"] = T( "basket", "basket cage dumbwaiter lowered", "From the chain is suspended a basket." );

			d["raisedbasket"] = T( "basket", "basket cage dumbwaiter", "At the end of the chain is a basket." );
			d["raisedbasket"].Capacity = 50;
			d["raisedbasket"].Container = true;
			d["raisedbasket"].Listed = true;

			d["lunch"] = T( "lunch", "dinner food hot lunch pepper sandwich", "A hot pepper sandwich is here." );
			d["lunch"].Portable = true;
			d["lunch"].Food = true;
			d["lunch"].Listed = true;

			d["bat"] = T( "bat", "bat deranged vampire", "bat" );
			d["bat"].Listed = true;

			d["bell"] = T( "brass bell", "bell brass small", "brass bell" );
			d["bell"].Portable = true;
			d["bell"].Listed = true;

			d["hotbell"] = T( "red hot brass bell", "bell brass hot red small", "On the ground is a red hot bell." );

			d["axe"] = T( "bloody axe", "ax axe bloody", "bloody axe" );
			d["axe"].Portable = true;
			d["axe"].Size = 25;
			d["axe"].Listed = true;

			d["bolt"] = T( "bolt", "bolt large metal nut", "bolt" );

			d["bubble"] = T( "green bubble", "bubble green plastic small", "green bubble" );

			d["altar"] = T( "altar", "altar", "altar" );
			d["altar"].Capacity = 50;
			d["altar"].Container = true;
			d["altar"].Surface = true;
			d["altar"].Listed = true;

			d["book"] = T( "black book", "black book books large page prayer", "black book" );
			d["book"].Portable = true;
			d["book"].Size = 10;
			d["book"].Container = true;
			d["book"].Listed = true;
			d["book"].ReadText = "Commandment #12592| | Oh ye who go about saying unto each: \"Hello sailor\":| Dost thou know the magnitude of thy sin before the gods?| Yea, verily, thou shalt be ground between two stones.| Shall the angry gods cast thy body into the whirlpool?| Surely, thy eye shall be put out with a sharp stick!| Even unto the ends of the earth shalt thou wander and| Unto the land of the dead shalt thou be sent at last.| Surely thou shalt repent of thy cunning.";

			d["brokenlamp"] = T( "broken lantern", "broken lamp lantern", "broken lantern" );
			d["brokenlamp"].Portable = true;
			d["brokenlamp"].Listed = true;

			d["sceptre"] = T( "sceptre", "ancient egyptian enameled scepter sceptre sharp treasure", "An ornamented sceptre, tapering to a sharp point, is here." );
			d["sceptre"].Portable = true;
			d["sceptre"].Value = 4;
			d["sceptre"].TValue = 6;
			d["sceptre"].Size = 3;
			d["sceptre"].Listed = true;

			d["timbers"] = T( "broken timber", "broken pile timbers wooden", "broken timber" );
			d["timbers"].Portable = true;
			d["timbers"].Size = 50;
			d["timbers"].Listed = true;

			d["slide"] = T( "chute", "chute metal ramp slide steep twisting", "chute" );
			d["slide"].Global = true;

			d["kitchentable"] = T( "kitchen table", "kitchen table", "kitchen table" );
			d["kitchentable"].Capacity = 50;
			d["kitchentable"].Container = true;
			d["kitchentable"].Surface = true;
			d["kitchentable"].Listed = true;

			d["attictable"] = T( "table", "table", "table" );
			d["attictable"].Capacity = 40;
			d["attictable"].Container = true;
			d["attictable"].Surface = true;
			d["attictable"].Listed = true;

			d["sandwichbag"] = T( "brown sack", "bag brown elongated sack smelly", "brown sack" );
			d["sandwichbag"].Portable = true;
			d["sandwichbag"].Size = 9;
			d["sandwichbag"].Capacity = 9;
			d["sandwichbag"].Container = true;
			d["sandwichbag"].Listed = true;

			d["toolchest"] = T( "group of tool chests", "chest chests group tool toolchests", "group of tool chests" );
			d["toolchest"].Container = true;
			d["toolchest"].Listed = true;

			d["yellowbutton"] = T( "yellow button", "button switch yellow", "yellow button" );

			d["brownbutton"] = T( "brown button", "brown button switch", "brown button" );

			d["redbutton"] = T( "red button", "button red switch", "red button" );

			d["bluebutton"] = T( "blue button", "blue button switch", "blue button" );

			d["trophycase"] = T( "trophy case", "case trophy", "trophy case" );
			d["trophycase"].Capacity = 10000;
			d["trophycase"].Container = true;
			d["trophycase"].Listed = true;

			d["rug"] = T( "carpet", "carpet large oriental rug", "carpet" );

			d["chalice"] = T( "chalice", "chalice cup engravings silver treasure", "There is a silver chalice, intricately engraved, here." );
			d["chalice"].Portable = true;
			d["chalice"].Value = 10;
			d["chalice"].TValue = 5;
			d["chalice"].Size = 10;
			d["chalice"].Capacity = 5;
			d["chalice"].Container = true;
			d["chalice"].Listed = true;

			d["garlic"] = T( "clove of garlic", "clove garlic", "clove of garlic" );
			d["garlic"].Portable = true;
			d["garlic"].Size = 4;
			d["garlic"].Food = true;
			d["garlic"].Listed = true;

			d["trident"] = T( "crystal trident", "crystal fork own poseidon treasure trident", "crystal trident" );
			d["trident"].Portable = true;
			d["trident"].Value = 4;
			d["trident"].TValue = 11;
			d["trident"].Size = 20;
			d["trident"].Listed = true;

			d["cyclops"] = T( "cyclops", "cyclops eye giant hungry monster", "cyclops" );
			d["cyclops"].Listed = true;

			d["dam"] = T( "dam", "dam fcd\\#3 gate gates", "dam" );

			d["trapdoor"] = T( "trap door", "cover door dusty trap trap-door trapdoor", "trap door" );

			d["boardedwindow"] = T( "boarded window", "boarded window", "boarded window" );
			d["boardedwindow"].Global = true;

			d["frontdoor"] = T( "door", "boarded door front", "door" );

			d["barrowdoor"] = T( "stone door", "door huge stone", "stone door" );

			d["barrow"] = T( "stone barrow", "barrow massive stone tomb", "stone barrow" );

			d["bottle"] = T( "glass bottle", "bottle clear container glass", "glass bottle" );
			d["bottle"].Portable = true;
			d["bottle"].Capacity = 4;
			d["bottle"].Container = true;
			d["bottle"].Listed = true;

			d["crack"] = T( "crack", "crack narrow", "crack" );
			d["crack"].Global = true;

			d["coffin"] = T( "gold coffin", "casket coffin gold solid treasure", "The solid-gold coffin used for the burial of Ramses II is here." );
			d["coffin"].Portable = true;
			d["coffin"].Value = 10;
			d["coffin"].TValue = 15;
			d["coffin"].Size = 55;
			d["coffin"].Capacity = 35;
			d["coffin"].Container = true;
			d["coffin"].Listed = true;

			d["grate"] = T( "grating", "grate grating", "grating" );
			d["grate"].Global = true;

			d["pump"] = T( "hand-held air pump", "air-pump hand-held pump small tool tools", "hand-held air pump" );
			d["pump"].Portable = true;
			d["pump"].Listed = true;

			d["diamond"] = T( "huge diamond", "diamond enormous huge treasure", "There is an enormous diamond (perfectly cut) here." );
			d["diamond"].Portable = true;
			d["diamond"].Value = 10;
			d["diamond"].TValue = 10;
			d["diamond"].Listed = true;

			d["jade"] = T( "jade figurine", "exquisite figurine jade treasure", "There is an exquisite jade figurine here." );
			d["jade"].Portable = true;
			d["jade"].Value = 5;
			d["jade"].TValue = 5;
			d["jade"].Size = 10;
			d["jade"].Listed = true;

			d["knife"] = T( "nasty knife", "blade knife knives nasty unrusty", "nasty knife" );
			d["knife"].Portable = true;
			d["knife"].Listed = true;

			d["bones"] = T( "skeleton", "body bones skeleton", "skeleton" );

			d["burnedoutlantern"] = T( "burned-out lantern", "burned dead lamp lantern rusty useless", "burned-out lantern" );
			d["burnedoutlantern"].Portable = true;
			d["burnedoutlantern"].Size = 20;
			d["burnedoutlantern"].Listed = true;

			d["bagofcoins"] = T( "leather bag of coins", "bag coins leather old treasure", "An old leather bag, bulging with coins, is here." );
			d["bagofcoins"].Portable = true;
			d["bagofcoins"].Value = 10;
			d["bagofcoins"].TValue = 5;
			d["bagofcoins"].Size = 15;
			d["bagofcoins"].Listed = true;

			d["lamp"] = T( "brass lantern", "brass lamp lantern light", "There is a brass lantern (battery-powered) here." );
			d["lamp"].Portable = true;
			d["lamp"].Size = 15;
			d["lamp"].Light = true;
			d["lamp"].Listed = true;

			d["emerald"] = T( "large emerald", "emerald large treasure", "large emerald" );
			d["emerald"].Portable = true;
			d["emerald"].Value = 5;
			d["emerald"].TValue = 10;
			d["emerald"].Listed = true;

			d["advertisement"] = T( "leaflet", "advertisement booklet leaflet mail small", "A small leaflet is on the ground." );
			d["advertisement"].Portable = true;
			d["advertisement"].Size = 2;
			d["advertisement"].Listed = true;
			d["advertisement"].ReadText = "\"WELCOME TO ZORK!| | ZORK is a game of adventure, danger, and low cunning. In it you will explore some of the most amazing territory ever seen by mortals. No computer should be without one!\"";

			d["leak"] = T( "leak", "drip leak pipe", "leak" );

			d["machine"] = T( "machine", "dryer lid machine pdp10", "machine" );
			d["machine"].Capacity = 50;
			d["machine"].Container = true;
			d["machine"].Listed = true;

			d["inflatedboat"] = T( "magic boat", "boat inflat magic plastic raft seaworthy", "magic boat" );
			d["inflatedboat"].Portable = true;
			d["inflatedboat"].Size = 20;
			d["inflatedboat"].Capacity = 100;
			d["inflatedboat"].Listed = true;

			d["mailbox"] = T( "small mailbox", "box mailbox small", "small mailbox" );
			d["mailbox"].Capacity = 10;
			d["mailbox"].Container = true;
			d["mailbox"].Listed = true;

			d["match"] = T( "matchbook", "match matchbook matches", "There is a matchbook whose cover says \"Visit Beautiful FCD#3\" here." );
			d["match"].Portable = true;
			d["match"].Size = 2;
			d["match"].Listed = true;
			d["match"].ReadText = "| (Close cover before striking)| | YOU too can make BIG MONEY in the exciting field of PAPER SHUFFLING!| | Mr. Anderson of Muddle, Mass. says: \"Before I took this course I was a lowly bit twiddler. Now with what I learned at GUE Tech I feel really important and can obfuscate and confuse with the best.\"| | Dr. Blank had this to say: \"Ten short days ago all I could look forward to was a dead-end job as a doctor. Now I have a promising future and make really big Zorkmids.\"| | GUE Tech can't promise these fantastic results to everyone. But when you earn your degree from GUE Tech, your future will be brighter.";

			d["mirror2"] = T( "mirror", "enormous mirror reflection", "mirror" );

			d["mirror1"] = T( "mirror", "enormous mirror reflection", "mirror" );

			d["painting"] = T( "painting", "art beauti canvas painting treasure", "A painting by a neglected genius is here." );
			d["painting"].Portable = true;
			d["painting"].Value = 4;
			d["painting"].TValue = 6;
			d["painting"].Size = 15;
			d["painting"].Listed = true;

			d["candles"] = T( "pair of candles", "burning candles pair", "pair of candles" );
			d["candles"].Portable = true;
			d["candles"].Size = 10;
			d["candles"].Light = true;
			d["candles"].Listed = true;

			d["gunk"] = T( "small piece of vitreous slag", "gunk piece slag small vitreous", "small piece of vitreous slag" );
			d["gunk"].Portable = true;
			d["gunk"].Size = 10;
			d["gunk"].Listed = true;

			d["bodies"] = T( "pile of bodies", "bodies body mangled pile remains", "pile of bodies" );
			d["bodies"].Global = true;

			d["leaves"] = T( "pile of leaves", "leaf leaves pile", "On the ground is a pile of leaves." );
			d["leaves"].Portable = true;
			d["leaves"].Size = 25;
			d["leaves"].Listed = true;

			d["puncturedboat"] = T( "punctured boat", "boat large pile plastic puncture", "punctured boat" );
			d["puncturedboat"].Portable = true;
			d["puncturedboat"].Size = 20;
			d["puncturedboat"].Listed = true;

			d["inflatableboat"] = T( "pile of plastic", "boat inflat pile plastic valve", "There is a folded pile of plastic here which has a small valve attached." );
			d["inflatableboat"].Portable = true;
			d["inflatableboat"].Size = 20;
			d["inflatableboat"].Listed = true;

			d["bar"] = T( "platinum bar", "bar large platinum treasure", "On the ground is a large platinum bar." );
			d["bar"].Portable = true;
			d["bar"].Value = 10;
			d["bar"].TValue = 5;
			d["bar"].Size = 20;
			d["bar"].Listed = true;

			d["potofgold"] = T( "pot of gold", "gold pot treasure", "pot of gold" );
			d["potofgold"].Portable = true;
			d["potofgold"].Value = 10;
			d["potofgold"].TValue = 10;
			d["potofgold"].Size = 15;
			d["potofgold"].Listed = true;

			d["prayer"] = T( "prayer", "ancient inscription old prayer", "prayer" );
			d["prayer"].ReadText = "The prayer is inscribed in an ancient script, rarely used today. It seems to be a philippic against small insects, absent-mindedness, and the picking up and dropping of small objects. The final verse consigns trespassers to the land of the dead. All evidence indicates that the beliefs of the ancient Zorkers were obscure.";

			d["railing"] = T( "wooden railing", "rail railing wooden", "wooden railing" );

			d["rainbow"] = T( "rainbow", "rainbow", "rainbow" );
			d["rainbow"].Global = true;

			d["river"] = T( "river", "frigid river", "river" );
			d["river"].Global = true;

			d["buoy"] = T( "red buoy", "buoy red", "red buoy" );
			d["buoy"].Portable = true;
			d["buoy"].Size = 10;
			d["buoy"].Capacity = 20;
			d["buoy"].Container = true;
			d["buoy"].Listed = true;

			d["rope"] = T( "rope", "coil hemp large rope", "rope" );
			d["rope"].Portable = true;
			d["rope"].Size = 10;
			d["rope"].Listed = true;

			d["rustyknife"] = T( "rusty knife", "knife knives rusty", "rusty knife" );
			d["rustyknife"].Portable = true;
			d["rustyknife"].Size = 20;
			d["rustyknife"].Listed = true;

			d["sand"] = T( "sand", "sand", "sand" );

			d["bracelet"] = T( "sapphire-encrusted bracelet", "bracelet jewel sapphire treasure", "sapphire-encrusted bracelet" );
			d["bracelet"].Portable = true;
			d["bracelet"].Value = 5;
			d["bracelet"].TValue = 5;
			d["bracelet"].Size = 10;
			d["bracelet"].Listed = true;

			d["screwdriver"] = T( "screwdriver", "driver screw screwdriver tool tools", "screwdriver" );
			d["screwdriver"].Portable = true;
			d["screwdriver"].Listed = true;

			d["keys"] = T( "skeleton key", "key skeleton", "skeleton key" );
			d["keys"].Portable = true;
			d["keys"].Size = 10;
			d["keys"].Listed = true;

			d["shovel"] = T( "shovel", "shovel tool tools", "shovel" );
			d["shovel"].Portable = true;
			d["shovel"].Size = 15;
			d["shovel"].Listed = true;

			d["coal"] = T( "small pile of coal", "coal heap pile small", "small pile of coal" );
			d["coal"].Portable = true;
			d["coal"].Size = 20;
			d["coal"].Listed = true;

			d["ladder"] = T( "wooden ladder", "ladder narrow rickety wooden", "wooden ladder" );
			d["ladder"].Global = true;

			d["scarab"] = T( "beautiful jeweled scarab", "beauti beetle bug carved jeweled scarab treasure", "beautiful jeweled scarab" );
			d["scarab"].Portable = true;
			d["scarab"].Value = 5;
			d["scarab"].TValue = 5;
			d["scarab"].Size = 8;
			d["scarab"].Listed = true;

			d["largebag"] = T( "large bag", "bag large thiefs", "large bag" );

			d["stiletto"] = T( "stiletto", "stiletto vicious", "stiletto" );
			d["stiletto"].Portable = true;
			d["stiletto"].Size = 10;
			d["stiletto"].Listed = true;

			d["machineswitch"] = T( "switch", "switch", "switch" );

			d["woodendoor"] = T( "wooden door", "door gothic lettering strange west wooden writing", "wooden door" );
			d["woodendoor"].ReadText = "The engravings translate to \"This space intentionally left blank.\"";

			d["sword"] = T( "sword", "antique blade elvish glamdring old orcrist sword", "sword" );
			d["sword"].Portable = true;
			d["sword"].Size = 30;
			d["sword"].Listed = true;

			d["map"] = T( "ancient map", "ancient antique map old parchment", "ancient map" );
			d["map"].Portable = true;
			d["map"].Size = 2;
			d["map"].Listed = true;
			d["map"].ReadText = "The map shows a forest with three clearings. The largest clearing contains a house. Three paths leave the large clearing. One of these paths, leading southwest, is marked \"To Stone Barrow\".";

			d["boatlabel"] = T( "tan label", "fine fineprint label print tan", "tan label" );
			d["boatlabel"].Portable = true;
			d["boatlabel"].Size = 2;
			d["boatlabel"].Listed = true;
			d["boatlabel"].ReadText = "!!!!FROBOZZ MAGIC BOAT COMPANY!!!!| | Hello, Sailor!| | Instructions for use:| | To get into a body of water, say \"Launch\".| To get to shore, say \"Land\" or the direction in which you want to maneuver the boat.| | Warranty:| | This boat is guaranteed against all defects for a period of 76 milliseconds from date of purchase or until first used, whichever comes first.| | Warning:| This boat is made of thin plastic.| Good Luck!";

			d["thief"] = T( "thief", "man person robber seedy shady suspicious thief", "There is a suspicious-looking individual, holding a large bag, leaning against one wall. He is armed with a deadly stiletto." );
			d["thief"].Container = true;
			d["thief"].Listed = true;

			d["pedestal"] = T( "pedestal", "marble pedestal white", "pedestal" );
			d["pedestal"].Capacity = 30;
			d["pedestal"].Container = true;
			d["pedestal"].Surface = true;
			d["pedestal"].Listed = true;

			d["torch"] = T( "torch", "flaming ivory torch treasure", "torch" );
			d["torch"].Portable = true;
			d["torch"].Value = 14;
			d["torch"].TValue = 6;
			d["torch"].Size = 20;
			d["torch"].Light = true;
			d["torch"].Listed = true;

			d["guide"] = T( "tour guidebook", "book books guide guidebooks tour", "tour guidebook" );
			d["guide"].Portable = true;
			d["guide"].Listed = true;
			d["guide"].ReadText = "\" Flood Control Dam #3| | FCD#3 was constructed in year 783 of the Great Underground Empire to harness the mighty Frigid River. This work was supported by a grant of 37 million zorkmids from your omnipotent local tyrant Lord Dimwit Flathead the Excessive. This impressive structure is composed of 370,000 cubic feet of concrete, is 256 feet tall at the center, and 193 feet wide at the top. The lake created behind the dam has a volume of 1.7 billion cubic feet, an area of 12 million square feet, and a shore line of 36 thousand feet.| | The construction of FCD#3 took 112 days from ground breaking to the dedication. It required a work force of 384 slaves, 34 slave drivers, 12 engineers, 2 turtle doves, and a partridge in a pear tree. The work was managed by a command team composed of 2345 bureaucrats, 2347 secretaries (at least two of whom could type), 12,256 paper shufflers, 52,469 rubber stampers, 245,193 red tape processors, and nearly one million dead trees.| | We will now point out some of the more interesting features of FCD#3 as we conduct you on a guided tour of the facilities:| | 1) You start your tour here in the Dam Lobby. You will notice on your right that....";

			d["troll"] = T( "troll", "nasty troll", "A nasty-looking troll, brandishing a bloody axe, blocks all passages out of the room." );
			d["troll"].Listed = true;

			d["trunk"] = T( "trunk of jewels", "chest jewels old treasure trunk", "There is an old trunk here, bulging with assorted jewels." );
			d["trunk"].Portable = true;
			d["trunk"].Value = 15;
			d["trunk"].TValue = 5;
			d["trunk"].Size = 35;
			d["trunk"].Listed = true;

			d["tube"] = T( "tube", "paste tooth tube", "There is an object which looks like a tube of toothpaste here." );
			d["tube"].Portable = true;
			d["tube"].Size = 5;
			d["tube"].Capacity = 7;
			d["tube"].Container = true;
			d["tube"].Listed = true;
			d["tube"].ReadText = "---> Frobozz Magic Gunk Company ---| All-Purpose Gunk";

			d["putty"] = T( "viscous material", "gunk material viscous", "viscous material" );
			d["putty"].Portable = true;
			d["putty"].Size = 6;
			d["putty"].Listed = true;

			d["engravings"] = T( "wall with engravings", "ancient engravings inscription old wall", "There are old engravings on the walls here." );
			d["engravings"].ReadText = "The engravings were incised in the living rock of the cave wall by an unknown hand. They depict, in symbolic form, the beliefs of the ancient Zorkers. Skillfully interwoven with the bas reliefs are excerpts illustrating the major religious tenets of that time. Unfortunately, a later age seems to have considered them blasphemous and just as skillfully excised them.";

			d["ownersmanual"] = T( "ZORK owner's manual", "manual owners paper piece small zork", "ZORK owner's manual" );
			d["ownersmanual"].Portable = true;
			d["ownersmanual"].Listed = true;
			d["ownersmanual"].ReadText = "Congratulations!| | You are the privileged owner of ZORK I: The Great Underground Empire, a self-contained and self-maintaining universe. If used and maintained in accordance with normal operating practices for small universes, ZORK will provide many months of trouble-free operation.";

			d["climbablecliff"] = T( "cliff", "cliff ledge rocky sheer wall walls", "cliff" );
			d["climbablecliff"].Global = true;

			d["whitecliff"] = T( "white cliffs", "cliff cliffs white", "white cliffs" );
			d["whitecliff"].Global = true;

			d["wrench"] = T( "wrench", "tool tools wrench", "wrench" );
			d["wrench"].Portable = true;
			d["wrench"].Size = 10;
			d["wrench"].Listed = true;

			d["controlpanel"] = T( "control panel", "control panel", "control panel" );

			d["nest"] = T( "bird's nest", "birds nest", "bird's nest" );
			d["nest"].Portable = true;
			d["nest"].Capacity = 20;
			d["nest"].Container = true;
			d["nest"].Listed = true;

			d["egg"] = T( "jewel-encrusted egg", "birds egg encrusted jeweled treasure", "jewel-encrusted egg" );
			d["egg"].Portable = true;
			d["egg"].Value = 5;
			d["egg"].TValue = 5;
			d["egg"].Capacity = 6;
			d["egg"].Container = true;
			d["egg"].Listed = true;

			d["brokenegg"] = T( "broken jewel-encrusted egg", "birds broken egg encrusted jewel treasure", "There is a somewhat ruined egg here." );
			d["brokenegg"].Portable = true;
			d["brokenegg"].TValue = 2;
			d["brokenegg"].Capacity = 6;
			d["brokenegg"].Container = true;
			d["brokenegg"].Listed = true;

			d["bauble"] = T( "beautiful brass bauble", "bauble beauti brass treasure", "beautiful brass bauble" );
			d["bauble"].Portable = true;
			d["bauble"].Value = 1;
			d["bauble"].TValue = 1;
			d["bauble"].Listed = true;

			d["canary"] = T( "golden clockwork canary", "canary clockwork gold golden treasure", "golden clockwork canary" );
			d["canary"].Portable = true;
			d["canary"].Value = 6;
			d["canary"].TValue = 4;
			d["canary"].Listed = true;

			d["brokencanary"] = T( "broken clockwork canary", "broken canary clockwork gold golden treasure", "broken clockwork canary" );
			d["brokencanary"].Portable = true;
			d["brokencanary"].TValue = 1;
			d["brokencanary"].Listed = true;

			return d;
		}

		private static void InitWorld( State st )
		{
			st.Room = "westofhouse";

			st.Where["mountainrange"] = "mountains";
			st.Where["water"] = "bottle";
			st.Where["ghosts"] = "entrancetohades";
			st.Where["skull"] = "landoflivingdead";
			st.Where["loweredbasket"] = "lowershaft";
			st.Where["raisedbasket"] = "shaftroom";
			st.Set( "open:raisedbasket", true );
			st.Where["lunch"] = "sandwichbag";
			st.Where["bat"] = "batroom";
			st.Where["bell"] = "northtemple";
			st.Where["axe"] = "troll";
			st.Where["bolt"] = "damroom";
			st.Where["bubble"] = "damroom";
			st.Where["altar"] = "southtemple";
			st.Set( "open:altar", true );
			st.Where["book"] = "altar";
			st.Where["sceptre"] = "coffin";
			st.Where["timbers"] = "timberroom";
			st.Where["kitchentable"] = "kitchen";
			st.Set( "open:kitchentable", true );
			st.Where["attictable"] = "attic";
			st.Set( "open:attictable", true );
			st.Where["sandwichbag"] = "kitchentable";
			st.Where["toolchest"] = "maintenanceroom";
			st.Set( "open:toolchest", true );
			st.Where["yellowbutton"] = "maintenanceroom";
			st.Where["brownbutton"] = "maintenanceroom";
			st.Where["redbutton"] = "maintenanceroom";
			st.Where["bluebutton"] = "maintenanceroom";
			st.Where["trophycase"] = "livingroom";
			st.Where["rug"] = "livingroom";
			st.Where["chalice"] = "treasureroom";
			st.Where["garlic"] = "sandwichbag";
			st.Where["trident"] = "atlantisroom";
			st.Where["cyclops"] = "cyclopsroom";
			st.Where["dam"] = "damroom";
			st.Where["trapdoor"] = "livingroom";
			st.Where["frontdoor"] = "westofhouse";
			st.Where["barrowdoor"] = "stonebarrow";
			st.Set( "open:barrowdoor", true );
			st.Where["barrow"] = "stonebarrow";
			st.Where["bottle"] = "kitchentable";
			st.Where["coffin"] = "egyptroom";
			st.Where["pump"] = "reservoirnorth";
			st.Where["jade"] = "batroom";
			st.Where["knife"] = "attictable";
			st.Where["bones"] = "maze5";
			st.Where["burnedoutlantern"] = "maze5";
			st.Where["bagofcoins"] = "maze5";
			st.Where["lamp"] = "livingroom";
			st.Where["emerald"] = "buoy";
			st.Where["advertisement"] = "mailbox";
			st.Where["leak"] = "maintenanceroom";
			st.Where["machine"] = "machineroom";
			st.Where["mailbox"] = "westofhouse";
			st.Where["match"] = "damlobby";
			st.Where["mirror2"] = "mirrorroom2";
			st.Where["mirror1"] = "mirrorroom1";
			st.Where["painting"] = "gallery";
			st.Where["candles"] = "southtemple";
			st.Where["leaves"] = "gratingclearing";
			st.Where["inflatableboat"] = "dambase";
			st.Where["bar"] = "loudroom";
			st.Where["potofgold"] = "endofrainbow";
			st.Where["prayer"] = "northtemple";
			st.Where["railing"] = "domeroom";
			st.Where["buoy"] = "river4";
			st.Where["rope"] = "attic";
			st.Where["rustyknife"] = "maze5";
			st.Where["sand"] = "sandycave";
			st.Where["bracelet"] = "gasroom";
			st.Where["screwdriver"] = "maintenanceroom";
			st.Where["keys"] = "maze5";
			st.Where["shovel"] = "sandybeach";
			st.Where["coal"] = "deadend5";
			st.Where["scarab"] = "sandycave";
			st.Where["largebag"] = "thief";
			st.Where["stiletto"] = "thief";
			st.Where["machineswitch"] = "machineroom";
			st.Where["woodendoor"] = "livingroom";
			st.Where["sword"] = "livingroom";
			st.Where["map"] = "trophycase";
			st.Where["boatlabel"] = "inflatedboat";
			st.Where["thief"] = "roundroom";
			st.Set( "open:thief", true );
			st.Where["pedestal"] = "torchroom";
			st.Set( "open:pedestal", true );
			st.Where["torch"] = "pedestal";
			st.Where["guide"] = "damlobby";
			st.Where["troll"] = "trollroom";
			st.Set( "open:troll", true );
			st.Where["trunk"] = "reservoir";
			st.Where["tube"] = "maintenanceroom";
			st.Where["putty"] = "tube";
			st.Where["engravings"] = "engravingscave";
			st.Where["ownersmanual"] = "studio";
			st.Where["wrench"] = "maintenanceroom";
			st.Where["controlpanel"] = "damroom";
			st.Where["nest"] = "upatree";
			st.Set( "open:nest", true );
			st.Where["egg"] = "nest";
			st.Where["canary"] = "egg";
			st.Where["brokencanary"] = "brokenegg";
		}
	}
}
