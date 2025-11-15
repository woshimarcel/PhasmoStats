using Serilog;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Logic;

public static class GitHubUpdater
{
	private const string REPO_OWNER = "woshimarcel";
	private const string REPO_NAME = "PhasmoStats";
	private static readonly HttpClient _http = new();

	private class GitHubRelease
	{
		[JsonPropertyName("tag_name")]
		public string TagName { get; set; } = "";
		[JsonPropertyName("assets")]
		public List<Asset> Assets { get; set; } = new();

		public class Asset
		{
			public string Name { get; set; } = "";
			[JsonPropertyName("browser_download_url")]
			public string BrowserDownloadUrl { get; set; } = "";
		}
	}

	public static async Task<(bool Updatable, string CurrentVersion, string LatestVersion)> UpdateAvailable()
	{
		string currentVersion = Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "0.0.0";

		try
		{
			_http.DefaultRequestHeaders.UserAgent.ParseAdd("PhasmoStatsUpdater");
			var url = $"https://api.github.com/repos/{REPO_OWNER}/{REPO_NAME}/releases/latest";
			var release = await _http.GetFromJsonAsync<GitHubRelease>(url);
			if (release == null)
				return (false, currentVersion, "");

			string latestVersion = release.TagName.TrimStart('v');

			return (IsNewerVersion(latestVersion, currentVersion), currentVersion, latestVersion);
		}
		catch (Exception ex)
		{
			Log.Error($"Update check failed: {ex.Message}");
		}

		return (false, currentVersion, "");
	}

	private static bool IsNewerVersion(string latest, string current)
	{
		if (Version.TryParse(latest, out var latestVersion) && Version.TryParse(current, out var currentVersion))
			return latestVersion > currentVersion;
		return false;
	}
}
