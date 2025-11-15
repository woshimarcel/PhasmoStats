using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Windows;
using File = System.IO.File;

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
		await DownloadAndInstall();
	}

	private async Task DownloadAndInstall()
	{
		try
		{
			StatusText.Text = "Downloading new Version...";
			Progress.Value = 5;
			using var client = new HttpClient();
			client.DefaultRequestHeaders.UserAgent.ParseAdd("PhasmoStatsUpdater");

			var url = $"https://api.github.com/repos/{REPO_OWNER}/{REPO_NAME}/releases/latest";
			var release = await client.GetFromJsonAsync<GitHubRelease>(url);
			if (release == null)
				return;

			Progress.Value = 25;
			string downloadUrl = release.Assets
				.FirstOrDefault(a => a.Name.EndsWith(".zip"))?
				.BrowserDownloadUrl ?? "";

			if (string.IsNullOrWhiteSpace(downloadUrl))
				throw new Exception("No downloadable asset found.");

			Progress.Value = 44;
			var data = await client.GetByteArrayAsync(downloadUrl);
			await File.WriteAllBytesAsync(zipPath, data);

			if (Directory.Exists(installDir))
			{
				StatusText.Text = "Removing old Version...";
				RemoveOldFiles();
			}
			Progress.Value = 58;

			StatusText.Text = "Extracting new Version...";
			Directory.CreateDirectory(installDir);
			string extractDir = Path.Combine(Path.GetTempPath(), "PhasmoStatsExtract");
			ZipFile.ExtractToDirectory(zipPath, extractDir, overwriteFiles: true);
			string newExe = Directory.GetFiles(extractDir, "*.exe", SearchOption.AllDirectories).FirstOrDefault();
			string currentExe = Environment.ProcessPath!;
			Progress.Value = 80;

			StatusText.Text = "Starting installer...";
			Process.Start(new ProcessStartInfo
			{
				FileName = "powershell",
				Arguments =
					$"Start-Process '{newExe}'\"",
				CreateNoWindow = true,
				UseShellExecute = false,
				WindowStyle = ProcessWindowStyle.Hidden
			});

			StatusText.Text = "Installation complete.";
			Progress.Value = 100;
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
			File.Delete(file);
		}
	}
}