using IWshRuntimeLibrary;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Windows;

namespace Updater;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
	private const string REPO_OWNER = "woshimarcel";
	private const string REPO_NAME = "PhasmoStats";
	string installDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PhasmoStats");
	string zipPath = Path.Combine(Path.GetTempPath(), "PhasmoStats.zip");

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

	public MainWindow(string mode, string currentVersion, string targetVersion)
	{
		InitializeComponent();

		if (mode == "update")
			StatusText.Text = $"Do you want to update from {currentVersion} to {targetVersion}?";
	}

	private void CancelBtn_Click(object sender, RoutedEventArgs e) => Close();

	private async void InstallBtn_Click(object sender, RoutedEventArgs e)
	{
		InstallBtn.IsEnabled = false;
		CancelBtn.IsEnabled = false;
		StatusText.Text = "Downloading...";
		await DownloadAndInstall();
	}

	private async Task DownloadAndInstall()
	{
		try
		{
			using var client = new HttpClient();
			client.DefaultRequestHeaders.UserAgent.ParseAdd("PhasmoStatsUpdater");

			var url = $"https://api.github.com/repos/{REPO_OWNER}/{REPO_NAME}/releases/latest";
			var release = await client.GetFromJsonAsync<GitHubRelease>(url);
			if (release == null) return;

			string downloadUrl = release.Assets
				.FirstOrDefault(a => a.Name.EndsWith(".zip"))?
				.BrowserDownloadUrl ?? "";

			if (string.IsNullOrWhiteSpace(downloadUrl))
				throw new Exception("No downloadable asset found.");

			var data = await client.GetByteArrayAsync(downloadUrl);
			await System.IO.File.WriteAllBytesAsync(zipPath, data);

			if (Directory.Exists(installDir))
				RemoveOldFiles();

			Directory.CreateDirectory(installDir);
			ZipFile.ExtractToDirectory(zipPath, installDir, true);

			StatusText.Text = "Installation complete.";
			Progress.Value = 100;
			CreateShortcut();
			Close();
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message, "Error");
		}
	}

	private void RemoveOldFiles()
	{
		string[] files = Directory.GetFiles(installDir);
		string[] directories = Directory.GetDirectories(installDir);

		foreach (string directory in directories)
			Directory.Delete(directory, recursive: true);

		foreach (string file in files)
		{
			if (file.Contains("Updater"))
				continue;
			System.IO.File.Delete(file);
		}
	}

	private void CreateShortcut()
	{
		string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
		string shortcutPath = Path.Combine(desktop, "PhasmoStats.lnk");
		var shell = new WshShell();
		var link = (IWshShortcut)shell.CreateShortcut(shortcutPath);
		link.TargetPath = Path.Combine(installDir, "PhasmoStatsBlazor.exe");
		link.Save();
	}
}