using Serilog;
using System.Text.Json;

namespace Logic;

public static class DataGetter
{
	public static Dictionary<string, int> GetData(Dictionary<string, object> data, string key)
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

	public static int GetInt(Dictionary<string, object> data, string key)
	{
		return (int)GetDouble(data, key);
	}

	public static double GetDouble(Dictionary<string, object> data, string key)
	{
		if (!data.TryGetValue(key, out object? value))
			return 0;

		if (value is JsonElement el && el.ValueKind == JsonValueKind.Number)
			return el.GetDouble();

		return 0;
	}

	public static Dictionary<string, (int seen, int died, double ratio, double percentage)> GetGhosts(Dictionary<string, object> data)
	{
		Log.Debug("Getting Ghosts");
		string[] ghostTypes = Dictionaries.GetGhostTypes();
		Dictionary<string, int> ghostsSeen = GetData(data, SaveKeys.MOST_COMMON_GHOSTS);
		Dictionary<string, int> ghostDeaths = GetData(data, SaveKeys.GHOST_KILLS);
		var stats = new Dictionary<string, (int, int, double, double)>();
		int total = ghostsSeen.Values.Sum();

		foreach (string ghostType in ghostTypes)
		{
			int seen;
			int died;
			seen = ghostsSeen.TryGetValue(ghostType, out seen) ? seen : 0;
			died = ghostDeaths.TryGetValue(ghostType, out died) ? died : 0;
			bool hasSeen = seen > 0;
			double ratio = hasSeen ? (double)died / seen : 0;
			double percentage = total > 0 && hasSeen ? (double)seen / total : 0;
			stats[ghostType] = (seen, died, ratio, percentage);
		}

		return stats;
	}

	public static Dictionary<string, (int visits, double percentage)> GetMaps(Dictionary<string, object> data)
	{
		Log.Debug("Getting Maps");
		Dictionary<string, string> mapNames = Dictionaries.GetMapNames();
		Dictionary<string, int> tempMaps = GetData(data, SaveKeys.PLAYED_MAPS);
		Dictionary<string, (int, double)> maps = [];
		int total = tempMaps.Values.Sum();
		bool hasPlayed = total > 0;

		foreach (var kv in tempMaps)
		{
			if (!mapNames.TryGetValue(kv.Key, out string name))
				name = kv.Key;

			int visits = kv.Value;
			bool hasVisited = kv.Value > 0;
			double percentage = hasPlayed && hasVisited ? (double)kv.Value / total : 0;
			maps.Add(name, (visits, percentage));
		}

		return maps;
	}

	public static Dictionary<string, (int uses, double percentage)> GetCursedObjects(Dictionary<string, object> data)
	{
		Log.Debug("Getting Cursed Objects");
		Dictionary<string, string> cursedNames = Dictionaries.GetCursedObjectNames();
		Dictionary<string, int> cursedAmount = [];

		foreach (var kv in cursedNames)
			cursedAmount[kv.Value] = GetInt(data, kv.Key);

		return GetAmountPercentage(cursedAmount);
	}

	public static Dictionary<string, (int draws, double percentage)> GetTarots(Dictionary<string, object> data)
	{
		Log.Debug("Getting Tarots");
		Dictionary<string, string> cardNames = Dictionaries.GetTarotCardNames();
		Dictionary<string, int> tarotsAmount = [];

		foreach (var kv in cardNames)
			tarotsAmount[kv.Value] = GetInt(data, "Tarot" + kv.Key);

		return GetAmountPercentage(tarotsAmount);
	}

	public static Dictionary<string, (int collected, double percentage)> GetBones(Dictionary<string, object> data)
	{
		Log.Debug("Printing Bones");
		Dictionary<string, string> boneNames = Dictionaries.GetBoneNames();
		Dictionary<string, int> bonesAmount = [];

		foreach (var kv in boneNames)
			bonesAmount[kv.Value] = GetInt(data, SaveKeys.BONE + kv.Key);

		return GetAmountPercentage(bonesAmount);
	}

	private static Dictionary<string, (int amount, double percentage)> GetAmountPercentage(Dictionary<string, int> keyAmount)
	{
		Dictionary<string, (int, double)> dictionary = [];
		int total = keyAmount.Values.Sum();
		bool hasGottenAny = total > 0;

		foreach (var kv in keyAmount)
		{
			bool hasGotten = kv.Value > 0;
			double percentage = hasGottenAny && hasGotten ? (double)kv.Value / total : 0;
			dictionary[kv.Key] = (kv.Value, percentage);
		}

		return dictionary;
	}

	public static Dictionary<string, string> GetCaseData(Dictionary<string, object> data)
	{
		int identified = GetInt(data, SaveKeys.IDENTIFIED);
		int misidentified = GetInt(data, SaveKeys.MISIDENTIFIED);
		int totalCases = identified + misidentified;
		double time = GetDouble(data, SaveKeys.TIME_INVESTIGATED);
		TimeSpan timeInvestigated = TimeSpan.FromSeconds(time);
		TimeSpan timePerCase = TimeSpan.FromSeconds(time / totalCases);
		int photosTaken = GetInt(data, SaveKeys.PHOTOS_TAKEN);
		int videosTaken = GetInt(data, SaveKeys.VIDEOS_TAKEN);
		int soundsTaken = GetInt(data, SaveKeys.SOUNDS_TAKEN);

		var dic = new Dictionary<string, string>
		{
			{ "Total Cases", totalCases.ToString() },
			{ "Identified", identified.ToString() },
			{ "Misidentified", misidentified.ToString() },
			{ "Time Investigated", GetTime(timeInvestigated) },
			{ "Time per Case", GetTime(timePerCase) },
			{ "Photos taken", $"{photosTaken} ({GetPerCase(totalCases, photosTaken):F2} per case)" },
			{ "Videos taken", $"{videosTaken} ({GetPerCase(totalCases, videosTaken):F2} per case)" },
			{ "Sounds taken", $"{soundsTaken} ({GetPerCase(totalCases, soundsTaken):F2} per case)" }
		};
		return dic;
	}

	private static string GetTime(TimeSpan time)
	{
		return $"{time.Hours + time.Days * 24}h {time.Minutes}m {time.Seconds}s";
	}

	private static double GetPerCase(int totalCases, int amount)
	{
		return totalCases > 0 && amount > 0 ? (double)amount / totalCases : 0;
	}

	public static int GetTotalCases()
	{
		int identified = GetInt(FileDeserializer.Data, SaveKeys.IDENTIFIED);
		int misidentified = GetInt(FileDeserializer.Data, SaveKeys.MISIDENTIFIED);
		return identified + misidentified;
	}
}
