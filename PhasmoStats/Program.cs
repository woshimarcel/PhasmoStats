namespace PhasmoStats;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

public class Program
{
	private readonly static string _saveFilePath = Path.Combine(
	    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
	    @"..\LocalLow\Kinetic Games\Phasmophobia\SaveFile.txt");
	private const string OUTPUT_FILENAME_RAW = "raw.json";
	private const string OUTPUT_FILENAME_CLEANED = "cleaned.json";

	private static void Main(string[] args)
	{
		if (!File.Exists(_saveFilePath))
		{
			PrintDivider();
			Console.WriteLine("File not found. Path:");
			Console.WriteLine(_saveFilePath);
			return;
		}

		var data = LoadAndDecryptFile();

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

	private static Dictionary<string, object> LoadAndDecryptFile()
	{
		string rawData = LoadFileData();
		File.WriteAllText(OUTPUT_FILENAME_RAW, rawData, Encoding.UTF8);
		string cleanedData = CleanData(rawData);

		JsonSerializerOptions options = new()
		{
			PropertyNameCaseInsensitive = true
		};

		Dictionary<string, object>? data = JsonSerializer.Deserialize<Dictionary<string, object>>(cleanedData, options);
		data = CleanJsonData(data);

		string content = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
		File.WriteAllText(OUTPUT_FILENAME_CLEANED, content, Encoding.UTF8);

		return data;
	}

	static string LoadFileData()
	{
		byte[] data = File.ReadAllBytes(_saveFilePath);
		return DecryptFileData(data);
	}

	static string DecryptFileData(byte[] data)
	{
		byte[] key = Encoding.ASCII.GetBytes("t36gref9u84y7f43g");
		byte[] salt = data.Take(16).ToArray(); // AES block size = 16
		byte[] encrypted = data.Skip(16).ToArray();

		using Rfc2898DeriveBytes derive = new(key, salt, iterations: 100, HashAlgorithmName.SHA1);
		byte[] aesKey = derive.GetBytes(16);

		using Aes aes = Aes.Create();
		aes.Mode = CipherMode.CBC;
		aes.Padding = PaddingMode.PKCS7;
		aes.Key = aesKey;
		aes.IV = salt;

		using ICryptoTransform decryptor = aes.CreateDecryptor();
		byte[] plain = decryptor.TransformFinalBlock(encrypted, 0, encrypted.Length);

		return Encoding.UTF8.GetString(plain);
	}

	private static string CleanData(string data)
	{
		return Regex.Replace(data, @"(\d+)(\:\d+)(?!.*"")", "\"$1\"$2");
	}

	private static Dictionary<string, object> CleanJsonData(Dictionary<string, object> data)
	{
		Dictionary<string, object> cleaned = new();
		foreach (var kv in data)
		{
			if (kv.Value is JsonElement element)
			{
				bool isJsonObject = element.ValueKind == JsonValueKind.Object;
				bool gotProperty = element.TryGetProperty("value", out JsonElement val);

				if (isJsonObject && gotProperty)
					cleaned[kv.Key] = val;
				else
					cleaned[kv.Key] = kv.Value;
			}
			else
			{
				cleaned[kv.Key] = kv.Value;
			}
		}
		return cleaned;
	}

	private static void PrintGhosts(Dictionary<string, object> data)
	{
		string[] ghostTypes = GetGhostTypes();
		Dictionary<string, int> ghostsSeen = GetData(data, "mostCommonGhosts");
		Dictionary<string, int> ghostDeaths = GetData(data, "ghostKills");

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
		Dictionary<string, string> mapNames = GetMapNames();
		Dictionary<string, int> maps = GetData(data, "playedMaps");
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
		Dictionary<string, string> cursedNames = GetCursedObjectNames();
		Dictionary<string, int> cursed = new();

		foreach (var kv in cursedNames)
			cursed[kv.Value] = GetInt(data, kv.Key);

		int total = cursed.Values.Sum();

		Console.WriteLine("Cursed Possessions used (excluding Tarot Decks):\n");
		foreach (var kv in cursed.OrderByDescending(c => c.Value).ThenBy(c => c.Key))
			Console.WriteLine($"  {kv.Key + ":",-23} {kv.Value} ({(double)kv.Value / total:P2})");

		int mapsPlayed = GetData(data, "playedMaps").Values.Sum();
		Console.WriteLine($"  Total: {"",-16} {total} ({(double)total / mapsPlayed:P2} of maps played)");
	}

	private static void PrintTarots(Dictionary<string, object> data)
	{
		Dictionary<string, string> cardNames = GetTarotCardNames();
		Dictionary<string, int> tarots = new();

		foreach (var kv in cardNames)
			tarots[kv.Value] = GetInt(data, "Tarot" + kv.Key);

		int total = tarots.Values.Sum();

		Console.WriteLine("Tarot Cards pulled:\n");
		foreach (var kv in tarots.OrderByDescending(t => t.Value).ThenBy(t => t.Key))
			Console.WriteLine($"  {kv.Key + ":",-23} {kv.Value} ({(double)kv.Value / total:P2})");

		Console.WriteLine($"  Total: {"",-16} {total}");
	}

	private static void PrintBones(Dictionary<string, object> data)
	{
		Dictionary<string, string> boneNames = GetBoneNames();
		Dictionary<string, int> bones = new();

		foreach (var kv in boneNames)
			bones[kv.Value] = GetInt(data, "Bone" + kv.Key);

		int total = bones.Values.Sum();

		Console.WriteLine("Bones found:\n");
		foreach (var kv in bones.OrderByDescending(b => b.Value))
			Console.WriteLine($"  {kv.Key + ":",-14} {kv.Value} ({(double)kv.Value / total:P2})");

		int mapsPlayed = GetData(data, "playedMaps").Values.Sum();
		Console.WriteLine($"  Total: {"",-7} {total} bones found ({(double)total / mapsPlayed:P2} of maps played)");
	}

	private static Dictionary<string, int> GetData(Dictionary<string, object> data, string key)
	{
		if (!data.TryGetValue(key, out object? value))
			return new Dictionary<string, int>();

		Dictionary<string, int> dict = new();
		if (value is JsonElement el && el.ValueKind == JsonValueKind.Object)
		{
			foreach (var prop in el.EnumerateObject())
				dict[prop.Name] = prop.Value.GetInt32();
		}
		return dict;
	}

	private static int GetInt(Dictionary<string, object> data, string key)
	{
		if (!data.TryGetValue(key, out object? value))
			return 0;

		if (value is JsonElement el && el.ValueKind == JsonValueKind.Number)
			return el.GetInt32();

		return 0;
	}

	private static string[] GetGhostTypes()
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

	private static Dictionary<string, string> GetTarotCardNames()
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

	private static Dictionary<string, string> GetBoneNames()
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

	private static Dictionary<string, string> GetCursedObjectNames()
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

	private static Dictionary<string, string> GetMapNames()
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
