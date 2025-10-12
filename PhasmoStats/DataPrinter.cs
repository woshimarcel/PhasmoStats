using Logic;
using Serilog;
using static PhasmoStats.Program;

namespace PhasmoStats;

internal static class DataPrinter
{
	private const ConsoleColor FIRST_COLOR = ConsoleColor.White;
	private const ConsoleColor SECOND_COLOR = ConsoleColor.DarkGray;

	internal static void PrintGhosts(Dictionary<string, object> data, Sortings sorting)
	{
		Log.Debug("Printing Ghosts. Sorting: {sorting}", sorting);
		InterfacePrinter.PrintDivider();

		string[] ghostTypes = Dictionaries.GetGhostTypes();
		Dictionary<string, int> ghostsSeen = DataGetter.GetData(data, SaveKeys.MOST_COMMON_GHOSTS);
		Dictionary<string, int> ghostDeaths = DataGetter.GetData(data, SaveKeys.GHOST_KILLS);

		int totalSeen = ghostsSeen.Values.Sum();
		int totalDeaths = ghostDeaths.Values.Sum();
		var stats = new Dictionary<string, (int seen, int died, double ratio)>();

		foreach (string ghostType in ghostTypes)
		{
			int seen = ghostsSeen.ContainsKey(ghostType) ? ghostsSeen[ghostType] : 0;
			int died = ghostDeaths.ContainsKey(ghostType) ? ghostDeaths[ghostType] : 0;
			stats[ghostType] = (seen, died, seen > 0 ? (double)died / seen : 0);
		}

		stats = sorting switch
		{
			Sortings.Alphabetically => stats.OrderBy(x => x.Key).ToDictionary(),
			Sortings.Sightings => stats.OrderByDescending(x => x.Value.seen).ToDictionary(),
			Sortings.Deaths => stats.OrderByDescending(x => x.Value.died).ToDictionary(),
			Sortings.Percentage => stats.OrderByDescending(x => x.Value.ratio).ToDictionary(),
			_ => stats
		};

		Console.WriteLine("Ghosts:\n");
		bool color = false;
		foreach (var pair in stats)
		{
			Console.ForegroundColor = color ? SECOND_COLOR : FIRST_COLOR;
			string ratio = pair.Value.ratio.ToString("P2");
			Console.WriteLine($"  {pair.Key + ":",-18} {pair.Value.seen} sightings      {pair.Value.died} deaths ({ratio})");
			color = !color;
		}

		Console.ForegroundColor = ConsoleColor.White;
		Console.WriteLine($"  Total: {"",-11} {totalSeen} sightings     {totalDeaths} deaths ({(double)totalDeaths / totalSeen:P2})");
	}

	internal static void PrintMaps(Dictionary<string, object> data, Sortings sorting)
	{
		Log.Debug("Printing Maps. Sorting: {sorting}", sorting);
		InterfacePrinter.PrintDivider();

		Dictionary<string, string> mapNames = Dictionaries.GetMapNames();
		Dictionary<string, int> tempMaps = DataGetter.GetData(data, SaveKeys.PLAYED_MAPS);
		Dictionary<string, int> maps = new();
		foreach (var kv in tempMaps)
		{
			if (!mapNames.TryGetValue(kv.Key, out string name))
				name = kv.Key;

			maps.Add(name, kv.Value);
		}

		int total = maps.Values.Sum();
		maps = sorting switch
		{
			Sortings.Alphabetically => maps.OrderBy(x => x.Key).ToDictionary(),
			Sortings.Sightings => maps.OrderByDescending(x => x.Value).ToDictionary(),
			_ => maps.OrderByDescending(x => (double)x.Value / total).ToDictionary(),
		};

		Console.WriteLine("Maps played:\n");
		bool color = false;
		foreach (var kv in maps)
		{
			Console.ForegroundColor = color ? SECOND_COLOR : FIRST_COLOR;
			Console.WriteLine($"  {kv.Key + ":",-30}{kv.Value} ({(double)kv.Value / total:P2})");
			color = !color;
		}

		Console.ForegroundColor = ConsoleColor.White;
		Console.WriteLine($"  Total: {"",-22} {total}");
	}

	internal static void PrintCursedObjects(Dictionary<string, object> data, Sortings sorting)
	{
		Log.Debug("Printing Cursed Objects. Sorting: {sorting}", sorting);
		InterfacePrinter.PrintDivider();

		Dictionary<string, string> cursedNames = Dictionaries.GetCursedObjectNames();
		Dictionary<string, int> cursed = new();

		foreach (var kv in cursedNames)
			cursed[kv.Value] = DataGetter.GetInt(data, kv.Key);

		int total = cursed.Values.Sum();
		cursed = sorting switch
		{
			Sortings.Alphabetically => cursed.OrderBy(x => x.Key).ToDictionary(),
			Sortings.Sightings => cursed.OrderByDescending(x => x.Value).ToDictionary(),
			_ => cursed.OrderByDescending(x => (double)x.Value / total).ToDictionary(),
		};

		Console.WriteLine("Cursed Possessions used (excluding Tarot Decks):\n");
		bool color = false;
		foreach (var kv in cursed)
		{
			Console.ForegroundColor = color ? SECOND_COLOR : FIRST_COLOR;
			Console.WriteLine($"  {kv.Key + ":",-23} {kv.Value} ({(double)kv.Value / total:P2})");
			color = !color;
		}

		Console.ForegroundColor = ConsoleColor.White;
		int mapsPlayed = DataGetter.GetData(data, "playedMaps").Values.Sum();
		Console.WriteLine($"  Total: {"",-16} {total} ({(double)total / mapsPlayed:P2} of maps played)");
	}

	internal static void PrintTarots(Dictionary<string, object> data, Sortings sorting)
	{
		Log.Debug("Printing Tarots. Sorting: {sorting}", sorting);
		InterfacePrinter.PrintDivider();

		Dictionary<string, string> cardNames = Dictionaries.GetTarotCardNames();
		Dictionary<string, int> tarots = new();

		foreach (var kv in cardNames)
			tarots[kv.Value] = DataGetter.GetInt(data, "Tarot" + kv.Key);

		int total = tarots.Values.Sum();

		tarots = sorting switch
		{
			Sortings.Alphabetically => tarots.OrderBy(x => x.Key).ToDictionary(),
			Sortings.Sightings => tarots.OrderByDescending(x => x.Value).ToDictionary(),
			_ => tarots.OrderByDescending(x => (double)x.Value / total).ToDictionary(),
		};

		Console.WriteLine("Tarot Cards pulled:\n");
		bool color = false;
		foreach (var kv in tarots)
		{
			Console.ForegroundColor = color ? SECOND_COLOR : FIRST_COLOR;
			Console.WriteLine($"  {kv.Key + ":",-23} {kv.Value} ({(double)kv.Value / total:P2})");
			color = !color;
		}

		Console.ForegroundColor = ConsoleColor.White;
		Console.WriteLine($"  Total: {"",-16} {total}");
	}

	internal static void PrintBones(Dictionary<string, object> data, Sortings sorting)
	{
		Log.Debug("Printing Bones. Sorting: {sorting}", sorting);
		InterfacePrinter.PrintDivider();

		Dictionary<string, string> boneNames = Dictionaries.GetBoneNames();
		Dictionary<string, int> bones = new();

		foreach (var kv in boneNames)
			bones[kv.Value] = DataGetter.GetInt(data, "Bone" + kv.Key);

		int total = bones.Values.Sum();
		bones = sorting switch
		{
			Sortings.Alphabetically => bones.OrderBy(x => x.Key).ToDictionary(),
			Sortings.Sightings => bones.OrderByDescending(x => x.Value).ToDictionary(),
			_ => bones.OrderByDescending(x => (double)x.Value / total).ToDictionary(),
		};

		Console.WriteLine("Bones found:\n");
		bool color = false;
		foreach (var kv in bones)
		{
			Console.ForegroundColor = color ? SECOND_COLOR : FIRST_COLOR;
			Console.WriteLine($"  {kv.Key + ":",-14} {kv.Value} ({(double)kv.Value / total:P2})");
			color = !color;
		}

		Console.ForegroundColor = ConsoleColor.White;
		int mapsPlayed = DataGetter.GetData(data, "playedMaps").Values.Sum();
		Console.WriteLine($"  Total: {"",-7} {total} bones found ({(double)total / mapsPlayed:P2} of maps played)");
	}
}
