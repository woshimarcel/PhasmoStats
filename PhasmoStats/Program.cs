namespace PhasmoStats;

using Logic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Program
{
	private static void Main(string[] args)
	{
		Dictionary<string, object> data;

		try
		{
			data = FileDeserializer.LoadAndDecryptFile();
		}
		catch (FileNotFoundException ex)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("Could not find save file. Please make sure the following file exists:");
			Console.WriteLine(ex.FileName);
			return;
		}

		PrintDivider();
		PrintGhosts(data);
		PrintDivider();
		PrintMaps(data);
		PrintDivider();
		PrintCursed(data);
		PrintDivider();
		PrintTarots(data);
		PrintDivider();
		PrintBones(data);

		Console.CursorVisible = false;
		Console.ReadKey(intercept: true);
	}

	private static void PrintDivider()
	{
		const string DIVIDER = "   ---=====--- ---=====---";
		Console.WriteLine();
		Console.WriteLine();
		Console.WriteLine();
		Console.WriteLine(DIVIDER);
		Console.WriteLine();
	}

	private static void PrintGhosts(Dictionary<string, object> data)
	{
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

		Console.WriteLine("Ghosts (deadliest first):\n");
		var sortedStats = stats.OrderByDescending(s => s.Value.ratio).ThenBy(s => s.Key);
		foreach (var pair in sortedStats)
		{
			string ratio = pair.Value.ratio.ToString("P2");
			Console.WriteLine($"  {pair.Key + ":",-18} {pair.Value.seen} sightings   {pair.Value.died} deaths ({ratio})");
		}
		Console.WriteLine($"  Total: {"",-11} {totalSeen} sightings, {totalDeaths} deaths ({(double)totalDeaths / totalSeen:P2})");
	}

	private static void PrintMaps(Dictionary<string, object> data)
	{
		Dictionary<string, string> mapNames = Dictionaries.GetMapNames();
		Dictionary<string, int> maps = DataGetter.GetData(data, SaveKeys.PLAYED_MAPS);
		int total = maps.Values.Sum();

		Console.WriteLine("Maps played:\n");
		foreach (var kv in maps.OrderByDescending(m => m.Value).ThenBy(m => mapNames.ContainsKey(m.Key) ? mapNames[m.Key] : m.Key))
		{
			string name = mapNames.TryGetValue(kv.Key, out string? value) ? value : $"Unknown map #{kv.Key}";
			Console.WriteLine($"  {name + ":",-30}{kv.Value} ({(double)kv.Value / total:P2})");
		}
		Console.WriteLine($"  Total: {"",-22} {total}");
	}

	private static void PrintCursed(Dictionary<string, object> data)
	{
		Dictionary<string, string> cursedNames = Dictionaries.GetCursedObjectNames();
		Dictionary<string, int> cursed = new();

		foreach (var kv in cursedNames)
			cursed[kv.Value] = DataGetter.GetInt(data, kv.Key);

		int total = cursed.Values.Sum();

		Console.WriteLine("Cursed Possessions used (excluding Tarot Decks):\n");
		foreach (var kv in cursed.OrderByDescending(c => c.Value).ThenBy(c => c.Key))
			Console.WriteLine($"  {kv.Key + ":",-23} {kv.Value} ({(double)kv.Value / total:P2})");

		int mapsPlayed = DataGetter.GetData(data, "playedMaps").Values.Sum();
		Console.WriteLine($"  Total: {"",-16} {total} ({(double)total / mapsPlayed:P2} of maps played)");
	}

	private static void PrintTarots(Dictionary<string, object> data)
	{
		Dictionary<string, string> cardNames = Dictionaries.GetTarotCardNames();
		Dictionary<string, int> tarots = new();

		foreach (var kv in cardNames)
			tarots[kv.Value] = DataGetter.GetInt(data, "Tarot" + kv.Key);

		int total = tarots.Values.Sum();

		Console.WriteLine("Tarot Cards pulled:\n");
		foreach (var kv in tarots.OrderByDescending(t => t.Value).ThenBy(t => t.Key))
			Console.WriteLine($"  {kv.Key + ":",-23} {kv.Value} ({(double)kv.Value / total:P2})");

		Console.WriteLine($"  Total: {"",-16} {total}");
	}

	private static void PrintBones(Dictionary<string, object> data)
	{
		Dictionary<string, string> boneNames = Dictionaries.GetBoneNames();
		Dictionary<string, int> bones = new();

		foreach (var kv in boneNames)
			bones[kv.Value] = DataGetter.GetInt(data, "Bone" + kv.Key);

		int total = bones.Values.Sum();

		Console.WriteLine("Bones found:\n");
		foreach (var kv in bones.OrderByDescending(b => b.Value))
			Console.WriteLine($"  {kv.Key + ":",-14} {kv.Value} ({(double)kv.Value / total:P2})");

		int mapsPlayed = DataGetter.GetData(data, "playedMaps").Values.Sum();
		Console.WriteLine($"  Total: {"",-7} {total} bones found ({(double)total / mapsPlayed:P2} of maps played)");
	}
}
