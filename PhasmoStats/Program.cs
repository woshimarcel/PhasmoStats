namespace PhasmoStats;

using Logic;
using System;
using System.Collections.Generic;
using System.Linq;

public class Program
{
	private const ConsoleColor FIRST_COLOR = ConsoleColor.White;
	private const ConsoleColor SECOND_COLOR = ConsoleColor.DarkGray;
	public static Categories Category { get; private set; } = Categories.None;
	public static Sortings Sorting { get; private set; } = Sortings.Percentage;
	private static string _input = string.Empty;

	public enum Sortings
	{
		Alphabetically,
		Sightings,
		Deaths,
		Percentage
	}

	private static void Main(string[] args)
	{
		CheckData();
		string path = Directory.GetCurrentDirectory();
		File.AppendAllText(path + "/Log.txt", $"[{DateTime.Now}] Starte Applikation\n");
		Console.Title = "PhasmoStats";
		PrintInputPrompt();
		Task.Run(CheckFileUpdates);
		Task.Run(ReadInput);

		while (true) { }
	}

	private static async Task CheckFileUpdates()
	{
		DateTime lastChange = File.GetLastWriteTime(FileDeserializer.GetSaveFilePath());
		while (true)
		{
			if (lastChange < File.GetLastWriteTime(FileDeserializer.GetSaveFilePath()))
			{
				lastChange = File.GetLastWriteTime(FileDeserializer.GetSaveFilePath());
				File.AppendAllText(Directory.GetCurrentDirectory() + "/Log.txt", $"[{DateTime.Now}] Erfolgreich geupdatet\n");
				FileDeserializer.UpdateData();
				CheckData();

				Console.Clear();
				Console.SetCursorPosition(0, 0);
				PrintData(FileDeserializer.Data, Category, Sorting);
				PrintInputPrompt();
				Console.Write(_input);
			}

			await Task.Delay(millisecondsDelay: 2000);
		}
	}

	private static async Task ReadInput()
	{
		while (true)
		{
			if (Console.KeyAvailable)
			{
				ConsoleKeyInfo keyInfo = Console.ReadKey(intercept: true);

				if (keyInfo.Key == ConsoleKey.Backspace && _input.Length > 0)
				{
					Console.Write("\b \b");
					_input = _input[..^1];
				}
				else if (keyInfo.Key == ConsoleKey.Enter)
				{
					string input = _input;
					_input = string.Empty;

					if (input == "refresh")
					{
						FileDeserializer.UpdateData();
					}
					else if (input.StartsWith("sort "))
					{
						Sorting = GetSorting(input, Sorting);
					}
					else
					{
						Categories category = Category;
						TryGetCategory(input, ref category);
						Category = category;
					}

					Console.Clear();
					if (Category == Categories.None)
					{
						PrintInputPrompt();
						continue;
					}

					PrintData(FileDeserializer.Data, Category, Sorting);
					PrintInputPrompt();
				}
				else if (char.IsLetter(keyInfo.KeyChar) || keyInfo.KeyChar == ' ')
				{
					Console.Write(keyInfo.KeyChar);
					_input += keyInfo.KeyChar;
				}
			}

			await Task.Delay(millisecondsDelay: 10);
		}
	}

	private static void CheckData()
	{
		if (FileDeserializer.Data == new Dictionary<string, object>())
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("File not found.");
			Console.WriteLine("Path -> " + FileDeserializer.GetSaveFilePath());
			Console.WriteLine("Only Windows is supported currently.");
			Environment.Exit(exitCode: 0);
		}
	}

	private static void PrintInputPrompt()
	{
		Console.ForegroundColor = ConsoleColor.White;
		PrintDivider();
		Console.WriteLine("Please select a category.\n");
		int top = Console.CursorTop;
		PrintCategories(top);
		PrintSortings(top);
		Console.SetCursorPosition(left: 0, top);
		Console.Write(" > ");
	}

	private static void PrintData(Dictionary<string, object> data, Categories category, Sortings sorting)
	{
		if (CompareCategory(category, Categories.Ghosts))
			PrintGhosts(data, sorting);

		if (CompareCategory(category, Categories.Maps))
			PrintMaps(data, sorting);

		if (CompareCategory(category, Categories.Bones))
			PrintBones(data, sorting);

		if (CompareCategory(category, Categories.CursedObjects))
		{
			PrintCursedObjects(data, sorting);
			PrintTarots(data, sorting);
		}
	}

	private static Sortings GetSorting(string input, Sortings currentSorting)
	{
		int sortLength = "sort ".Length;
		if (input.Length < sortLength)
			return currentSorting;

		input = input[sortLength..].ToLower();
		return input switch
		{
			"alphabetically" or "alph" or "a" => Sortings.Alphabetically,
			"deaths" or "death" or "d" => Sortings.Deaths,
			"sightings" or "sights" or "s" or "used" or "u" => Sortings.Sightings,
			"percentage" or "p" => Sortings.Percentage,
			_ => currentSorting,
		};
	}

	private static void PrintCategories(int top)
	{
		const int LEFT = 30;
		top -= 5;
		if (top < 0)
			top = Console.CursorTop;

		Console.SetCursorPosition(LEFT, top);
		Console.WriteLine("Categories:");
		top = Console.CursorTop;
		Console.SetCursorPosition(LEFT, top);
		Console.WriteLine("All | Ghosts | Maps");
		top = Console.CursorTop;
		Console.SetCursorPosition(LEFT, top);
		Console.WriteLine("Bones | Cursed Objects");
	}

	private static void PrintSortings(int top)
	{
		const int LEFT = 60;
		top -= 5;
		if (top < 0)
			top = Console.CursorTop;

		Console.SetCursorPosition(LEFT, top);
		Console.WriteLine("Sortings:");
		top = Console.CursorTop;
		Console.SetCursorPosition(LEFT, top);
		Console.WriteLine("Alphabetically | Deaths");
		top = Console.CursorTop;
		Console.SetCursorPosition(LEFT, top);
		Console.WriteLine("Sightings/Used | Percentage");
	}

	private static bool TryGetCategory(string input, ref Categories category)
	{
		input = input.ToLower();
		Categories temp = category;
		category = input switch
		{
			"all" or "a" => Categories.All,
			"ghosts" or "g" => Categories.Ghosts,
			"maps" or "m" => Categories.Maps,
			"cursed objects" or "co" => Categories.CursedObjects,
			"bones" or "b" => Categories.Bones,
			"none" or "n" => Categories.None,
			_ => Categories.NOT_FOUND,
		};

		if (category == Categories.NOT_FOUND)
		{
			if (temp == Categories.NOT_FOUND)
				category = Categories.None;
			else
				category = temp;

			return false;
		}

		return true;
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

	private static bool CompareCategory(Categories selected, Categories category)
	{
		return selected == Categories.All || selected == category;
	}

	private static void PrintGhosts(Dictionary<string, object> data, Sortings sorting)
	{
		PrintDivider();

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

	private static void PrintMaps(Dictionary<string, object> data, Sortings sorting)
	{
		PrintDivider();

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

	private static void PrintCursedObjects(Dictionary<string, object> data, Sortings sorting)
	{
		PrintDivider();

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

	private static void PrintTarots(Dictionary<string, object> data, Sortings sorting)
	{
		PrintDivider();

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

	private static void PrintBones(Dictionary<string, object> data, Sortings sorting)
	{
		PrintDivider();

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
