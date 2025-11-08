using Serilog;
using System.Text;
using System.Text.Json;

namespace Logic;

public static class DataGetter
{
	private const string NUMBER_FORMAT = "#,#0.##";

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

			string name = ghostType switch
			{
				"Mimic" => "The Mimic",
				"TheTwins" => "The Twins",
				_ => ghostType,
			};

			stats[name] = (seen, died, ratio, percentage);
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
		Log.Debug("Getting Bones");
		Dictionary<string, string> boneNames = Dictionaries.GetBoneNames();
		Dictionary<string, int> bonesAmount = [];
		int bonesCollected = GetInt(data, SaveKeys.BONES_COLLECTED);

		foreach (var kv in boneNames)
			bonesAmount[kv.Value] = GetInt(data, SaveKeys.BONE + kv.Key);
		int oldBonesCollected = bonesCollected - bonesAmount.Values.Sum();
		bonesAmount["Old Bone"] = oldBonesCollected;

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

	public static Dictionary<string, (string, string)> GetCasePlayerData(Dictionary<string, object> data)
	{
		Log.Debug("Getting Case: Player");

		int identified = GetInt(data, SaveKeys.IDENTIFIED);
		int misidentified = GetInt(data, SaveKeys.MISIDENTIFIED);
		int totalCases = identified + misidentified;
		int photosTaken = GetInt(data, SaveKeys.PHOTOS_TAKEN);
		int videosTaken = GetInt(data, SaveKeys.VIDEOS_TAKEN);
		int soundsTaken = GetInt(data, SaveKeys.SOUNDS_TAKEN);
		int deaths = GetInt(data, SaveKeys.DEATHS);
		int revives = GetInt(data, SaveKeys.REVIVES);
		int objectives = GetInt(data, SaveKeys.OBJECTIVES_COMPLETED);
		int cursedHunts = GetInt(data, SaveKeys.CURSED_HUNTS_TRIGGERED);
		double timeInvestigated = GetDouble(data, SaveKeys.TIME_INVESTIGATED);
		double timeTruck = GetDouble(data, SaveKeys.TIME_IN_TRUCK);
		double timeChased = GetDouble(data, SaveKeys.TIME_CHASED);
		double timeGhostRoom = GetDouble(data, SaveKeys.TIME_IN_GHOST_ROOM);
		double timeDark = GetDouble(data, SaveKeys.TIME_IN_DARK);
		double timeLight = GetDouble(data, SaveKeys.TIME_IN_LIGHT);
		int hunts = GetInt(data, SaveKeys.HUNTS);
		double sanityLost = GetDouble(data, SaveKeys.SANITY_LOST);
		double sanityGained = GetDouble(data, SaveKeys.SANITY_GAINED);
		double distancedTravelled = GetDouble(data, SaveKeys.PLAYER_DISTANCE_TRAVELLED);

		return new Dictionary<string, (string, string)>
		{
			{ "Total Cases", (totalCases.ToString(NUMBER_FORMAT), "") },
			{ "Ghosts Identified", (identified.ToString(NUMBER_FORMAT), $"{GetRatio(totalCases, identified):P2} of time") },
			{ "Ghosts Misidentified", (misidentified.ToString(NUMBER_FORMAT), $"{GetRatio(totalCases, misidentified):P2} of time") },
			{ "Photos taken", (photosTaken.ToString(NUMBER_FORMAT), $"{GetRatio(totalCases, photosTaken):F2} per case") },
			{ "Videos taken", (videosTaken.ToString(NUMBER_FORMAT), $"{GetRatio(totalCases, videosTaken):F2} per case") },
			{ "Sounds taken", (soundsTaken.ToString(NUMBER_FORMAT), $"{GetRatio(totalCases, soundsTaken):F2} per case") },
			{ "Deaths", (deaths.ToString(NUMBER_FORMAT), $"{GetRatio(totalCases, deaths):P2} of cases played") },
			{ "Revives", (revives.ToString(NUMBER_FORMAT), $"{GetRatio(deaths, revives):P2} of deaths") },
			{ "Sanity lost", ($"{sanityLost.ToString(NUMBER_FORMAT)} %", $"{GetRatio(totalCases, sanityLost):F2} per case") },
			{ "Sanity gained", ($"{sanityGained.ToString(NUMBER_FORMAT)} %", $"{GetRatio(totalCases, sanityGained):F2} % per case") },
			{ "Objectives completed", ($"{objectives.ToString(NUMBER_FORMAT)}", $"{GetRatio(totalCases, objectives):F2} % per case") },
			{ "Cursed Hunts triggered", ($"{cursedHunts.ToString(NUMBER_FORMAT)}", $"1 every {GetRatio(cursedHunts, totalCases):F2} cases") },
			{ "Distance travelled", ($"{distancedTravelled.ToString(NUMBER_FORMAT)} meters", $"{GetRatio(totalCases, distancedTravelled):F2} meters per case") },
			{ "Time Investigated", (GetReadableTime(timeInvestigated), GetTimePer(timeInvestigated, totalCases) + " average duration") },
			{ "Truck Princess", (GetReadableTime(timeTruck), GetTimePer(timeTruck, totalCases) + " per case") },
			{ "Time in Ghost Room", (GetReadableTime(timeGhostRoom), GetTimePer(timeGhostRoom, totalCases) + " per case") },
			{ "Time in Dark", (GetReadableTime(timeDark), GetTimePer(timeDark, totalCases) + " per case") },
			{ "Time in Light", (GetReadableTime(timeLight), GetTimePer(timeLight, totalCases) + " per case") },
			{ "Time Chased", (GetReadableTime(timeChased), GetTimePer(timeChased, hunts) + " per hunt") },
		};
	}

	private static string GetTimePer(double time, double amount)
	{
		if (amount == 0 || time == 0)
			return "";
		return GetReadableTime(time / amount);
	}

	public static Dictionary<string, (string, string)> GetCaseGhostData(Dictionary<string, object> data)
	{
		Log.Debug("Getting Case: Ghost");
		int totalCases = GetInt(data, SaveKeys.IDENTIFIED) + GetInt(data, SaveKeys.MISIDENTIFIED);
		int roomChanged = GetInt(data, SaveKeys.ROOM_CHANGED);
		int abilitiesUsed = GetInt(data, SaveKeys.ABILITIES_USED);
		int ghostEvents = GetInt(data, SaveKeys.GHOST_EVENTS);
		int fuseBoxToggles = GetInt(data, SaveKeys.FUSE_BOX_TOGGLES);
		int lightsSwitched = GetInt(data, SaveKeys.LIGHT_SWITCHES);
		int doorsMoved = GetInt(data, SaveKeys.DOORS_MOVED);
		int objectsUsed = GetInt(data, SaveKeys.OBJECTS_USED);
		int interactions = GetInt(data, SaveKeys.INTERACTIONS);
		double distanceTravelled = GetDouble(data, SaveKeys.GHOST_DISTANCE_TRAVELLED);
		double timeInvestigated = GetDouble(data, SaveKeys.TIME_INVESTIGATED);
		double timeHunted = GetDouble(data, SaveKeys.TIME_HUNTED);
		double timeFavoriteRoom = GetDouble(data, SaveKeys.TIME_FAVORITE_ROOM);

		return new Dictionary<string, (string, string)>
		{
			{ "Total Cases", (totalCases.ToString(NUMBER_FORMAT), "") },
			{ "Interactions",  (interactions.ToString(NUMBER_FORMAT), $"{GetRatio(totalCases, interactions):F2} per case")},
			{ "Ghost Events",  (ghostEvents.ToString(NUMBER_FORMAT), $"{GetRatio(totalCases, ghostEvents):F2} per case")},
			{ "Fuse Box toggles",  (fuseBoxToggles.ToString(NUMBER_FORMAT), $"{GetRatio(totalCases, fuseBoxToggles):F2} per case")},
			{ "Lights switched",  (lightsSwitched.ToString(NUMBER_FORMAT), $"{GetRatio(totalCases, lightsSwitched):F2} per case")},
			{ "Doors moved",  (doorsMoved.ToString(NUMBER_FORMAT), $"{GetRatio(totalCases, doorsMoved):F2} per case")},
			{ "Objects used",  (objectsUsed.ToString(NUMBER_FORMAT), $"{GetRatio(totalCases, objectsUsed):F2} per case")},
			{ "Abilities used",  (abilitiesUsed.ToString(NUMBER_FORMAT), $"{GetRatio(totalCases, abilitiesUsed):F2} per case")},
			{ "Times Room changed",  (roomChanged.ToString(NUMBER_FORMAT), $"{GetRatio(totalCases, roomChanged):F2} per case")},
			{ "Distance travelled",  (distanceTravelled.ToString(NUMBER_FORMAT) + " meters", $"{GetRatio(totalCases, distanceTravelled):F2} meters per case")},
			{ "Time hunted",  (GetReadableTime(timeHunted), $"{GetTimePer(timeHunted, totalCases)} per case")},
			{ "Time inside Favorite Room",  (GetReadableTime(timeFavoriteRoom), $"{GetTimePer(timeFavoriteRoom, totalCases)} per case")},
		};
	}

	private static string GetReadableTime(double timeSeconds)
	{
		TimeSpan time = TimeSpan.FromSeconds(timeSeconds);
		StringBuilder sb = new();
		if (time.Hours > 0 || time.Days > 0)
			sb.Append($"{time.Hours + time.Days * 24}h ");
		if (time.Minutes > 0)
			sb.Append($"{time.Minutes}m ");
		if (time.Seconds > 0)
			sb.Append($"{time.Seconds}s");

		return sb.ToString();
	}

	private static double GetRatio(double total, double amount)
	{
		return total > 0 && amount > 0 ? amount / total : 0;
	}

	public static int GetTotalCases()
	{
		int identified = GetInt(FileDeserializer.Data, SaveKeys.IDENTIFIED);
		int misidentified = GetInt(FileDeserializer.Data, SaveKeys.MISIDENTIFIED);
		return identified + misidentified;
	}
}
