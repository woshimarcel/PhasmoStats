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

	public static Dictionary<string, (int seen, int died, double ratio)> GetGhosts(Dictionary<string, object> data)
	{
		string[] ghostTypes = Dictionaries.GetGhostTypes();
		Dictionary<string, int> ghostsSeen = GetData(data, SaveKeys.MOST_COMMON_GHOSTS);
		Dictionary<string, int> ghostDeaths = GetData(data, SaveKeys.GHOST_KILLS);
		var stats = new Dictionary<string, (int seen, int died, double ratio)>();

		foreach (string ghostType in ghostTypes)
		{
			int seen;
			int died;
			seen = ghostsSeen.TryGetValue(ghostType, out seen) ? seen : 0;
			died = ghostDeaths.TryGetValue(ghostType, out died) ? died : 0;
			stats[ghostType] = (seen, died, seen > 0 ? (double)died / seen : 0);
		}

		return stats;
	}
}
