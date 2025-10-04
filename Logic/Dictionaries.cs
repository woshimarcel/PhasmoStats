namespace Logic;
public static class Dictionaries
{
	public static string[] GetGhostTypes()
	{
		return
		[
			"Banshee",
			"Demon",
			"Deogen",
			"Goryo",
			"Hantu",
			"Jinn",
			"Mare",
			"Mimic",
			"Moroi",
			"Myling",
			"Obake",
			"Oni",
			"Onryo",
			"Phantom",
			"Poltergeist",
			"Raiju",
			"Revenant",
			"Shade",
			"Spirit",
			"Thaye",
			"TheTwins",
			"Wraith",
			"Yokai",
			"Yurei",
		];
	}

	public static Dictionary<string, string> GetTarotCardNames()
	{
		return new Dictionary<string, string>
		{
			{ "Tower", "Tower" },
			{ "Wheel","Wheel of Fortune" },
			{ "Fool", "Fool" },
			{ "Devil", "Devil" },
			{ "Death", "Death" },
			{ "Hermit", "Hermit" },
			{ "Sun", "Sun" },
			{ "Moon", "Moon" },
			{ "Priestess", "High Priestess" },
			{ "HangedMan", "Hanged Man" },
		};
	}

	public static Dictionary<string, string> GetBoneNames()
	{
		return new Dictionary<string, string>
		{
			{ "0", "Femur" },
			{ "1", "Foot" },
			{ "2", "Fibula" },
			{ "3", "Hand" },
			{ "4", "Humerus" },
			{ "5", "Jaw" },
			{ "6", "Pelvis" },
			{ "7", "Radius" },
			{ "8", "Ribcage" },
			{ "9", "Scapula" },
			{ "10", "Skull" },
			{ "11", "Spine" },
			{ "12", "Ulna" },
		};
	}

	public static Dictionary<string, string> GetCursedObjectNames()
	{
		return new Dictionary<string, string>
		{
			{ "MirrorsFound", "Mirrors" },
			{ "MonkeyPawFound", "Monkey Paws" },
			{ "MusicBoxesFound", "Music Boxes" },
			{ "OuijasFound", "Ouija Boards" },
			{ "VoodoosFound", "Voodoo Dolls" },
			{ "SummoningCirclesUsed", "Summoning Circles" },
		};
	}

	public static Dictionary<string, string> GetMapNames()
	{
		return new Dictionary<string, string>
		{
			{ "0", "Sunny Meadows Restricted" },
			{ "1", "Sunny Meadows" },
			{ "2", "Bleasdale Farmhouse" },
			{ "3", "Camp Woodwind" },
			{ "4", "Maple Lodge Campsite" },
			{ "5", "42 Edgefield Road" },
			{ "6", "Grafton Farmhouse" },
			{ "7", "Prison" },
			{ "8", "???" },
			{ "9", "10 Ridgeview court" },
			{ "10","Brownstone High School" },
			{ "11","6 Tanglewood Drive" },
			{ "12","13 Willow Street" },
			{ "13", "???" },
			{ "14" , "Point Hope" }
		};
	}
}
