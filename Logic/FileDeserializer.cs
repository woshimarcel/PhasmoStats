using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Logic;

public static class FileDeserializer
{
	private static readonly string _saveFilePath = Path.Combine(
		Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
		@"..\LocalLow\Kinetic Games\Phasmophobia\SaveFile.txt");
	private const string OUTPUT_FILENAME_RAW = "raw.json";
	private const string OUTPUT_FILENAME_CLEANED = "cleaned.json";
	public static Dictionary<string, object> Data { get; private set; } = LoadAndDecryptFile();

	public static string GetSaveFilePath() => _saveFilePath;

	public static void UpdateData() => Data = LoadAndDecryptFile();

	public static Dictionary<string, object> LoadAndDecryptFile()
	{
		if (!File.Exists(_saveFilePath))
			return new();

		string rawData = LoadFileData();
		File.WriteAllText(OUTPUT_FILENAME_RAW, rawData, Encoding.UTF8);
		string cleanedData = CleanData(rawData);

		JsonSerializerOptions options = new()
		{
			PropertyNameCaseInsensitive = true
		};

		var data = JsonSerializer.Deserialize<Dictionary<string, object>>(cleanedData, options);
		data = CleanJsonData(data);

		string content = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
		File.WriteAllText(OUTPUT_FILENAME_CLEANED, content, Encoding.UTF8);

		return data;
	}

	private static string LoadFileData()
	{
		byte[] data = File.ReadAllBytes(_saveFilePath);
		return DecryptFileData(data);
	}

	private static string DecryptFileData(byte[] data)
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
}
