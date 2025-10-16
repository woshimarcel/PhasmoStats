using Logic;
using Serilog;

namespace PhasmoStatsBlazor;

internal class FileUpdater
{
	private static bool _autoRefresh = true;

	internal static async Task UpdateFile()
	{
		DateTime lastChange = File.GetLastWriteTime(FileDeserializer.GetSaveFilePath());
		while (true)
		{
			if (_autoRefresh && lastChange < File.GetLastWriteTime(FileDeserializer.GetSaveFilePath()))
			{
				lastChange = File.GetLastWriteTime(FileDeserializer.GetSaveFilePath());
				FileDeserializer.UpdateData();
				if (CheckData())
					DataRefresher.NotifyDataUpdated();
			}

			await Task.Delay(millisecondsDelay: 2000);
		}
	}

	internal static bool CheckData()
	{
		if (FileDeserializer.Data == new Dictionary<string, object>())
		{
			Log.Error("Data empty. File not found.");
			Application.Current.Windows[0].Page.DisplayAlert("Error", "There was an error while getting data out off your save file.", "OK");
			return false;
		}

		return true;
	}
}
