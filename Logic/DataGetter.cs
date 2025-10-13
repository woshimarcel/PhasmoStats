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
}
