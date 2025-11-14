using Serilog;
using System.Diagnostics;
using System.IO.Compression;
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

	public static async Task CheckForUpdateAsync()
	{
		try
		{
			_http.DefaultRequestHeaders.UserAgent.ParseAdd("PhasmoStatsUpdater");
			var url = $"https://api.github.com/repos/{REPO_OWNER}/{REPO_NAME}/releases/latest";
			var release = await _http.GetFromJsonAsync<GitHubRelease>(url);
			if (release == null) return;

			string currentVersion = Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "0.0.0";
			string latestVersion = release.TagName.TrimStart('v');

			if (!IsNewerVersion(latestVersion, currentVersion))
			{
				//RemoveOldVersions();
				return;
			}

			Process.Start("Updater.exe", $"--update --from {currentVersion} --to {latestVersion}");
			Environment.Exit(0);
			return;

			string downloadUrl = release.Assets.FirstOrDefault()?.BrowserDownloadUrl ?? "";
			if (string.IsNullOrEmpty(downloadUrl))
				return;

			string tempZip = Path.Combine(Path.GetTempPath(), "PhasmoStatsUpdate.zip");
			string extractDir = Path.Combine(Path.GetTempPath(), "PhasmoStatsExtract");

			Log.Information($"Downloading update {latestVersion}...");
			var bytes = await _http.GetByteArrayAsync(downloadUrl);
			await File.WriteAllBytesAsync(tempZip, bytes);

			if (Directory.Exists(extractDir))
				Directory.Delete(extractDir, true);
			ZipFile.ExtractToDirectory(tempZip, extractDir);

			string currentExe = Environment.ProcessPath!;
			string newExe = Directory.GetFiles(extractDir, "*.exe", SearchOption.AllDirectories).FirstOrDefault();

			if (string.IsNullOrEmpty(newExe))
			{
				Log.Error("No .exe found in update package");
				return;
			}

			Log.Information("Installing update...");
			Process.Start(new ProcessStartInfo
			{
				FileName = "powershell",
				Arguments =
					$"Copy-Item -Path '{newExe}' -Destination '{currentExe}' -Force; " +
					$"Start-Process '{currentExe}'\"",
				CreateNoWindow = true,
				UseShellExecute = false,
				WindowStyle = ProcessWindowStyle.Hidden
			});

			Environment.Exit(0);
		}
		catch (Exception ex)
		{
			Log.Error($"Update check failed: {ex.Message}");
		}
	}

	private static bool IsNewerVersion(string latest, string current)
	{
		if (Version.TryParse(latest, out var latestVersion) && Version.TryParse(current, out var currentVersion))
			return latestVersion > currentVersion;
		return false;
	}

	private static void RemoveOldVersions()
	{
		string baseDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PhasmoStats");
		if (!Directory.Exists(baseDir))
			return;

		string currentFolder = AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar);
		foreach (var dir in Directory.GetDirectories(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PhasmoStats*"))
		{
			if (dir.Equals(currentFolder, StringComparison.OrdinalIgnoreCase))
				continue;

			try
			{
				Directory.Delete(dir, true);
				Console.WriteLine($"Deleted old version: {dir}");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Failed to delete {dir}: {ex.Message}");
			}
		}
	}
}
