using Logic;
using Serilog;

namespace PhasmoStats;

internal class FileUpdater
{
	private static bool _autoRefresh = true;
	private static readonly string[] _writingOptions =
	[
		"toggle ar",
		"t ar",
		"toggle auto refresh",
		"toggle auto-refresh",
		"toggle autorefresh",
		"toggle refresh",
	];

	internal static bool IsWritingOption(string input)
	{
		foreach (string option in _writingOptions)
		{
			if (input.StartsWith(option))
				return true;
		}

		return false;
	}

	internal static async Task UpdateFile()
	{
		DateTime lastChange = File.GetLastWriteTime(FileDeserializer.GetSaveFilePath());
		while (true)
		{
			if (_autoRefresh && lastChange < File.GetLastWriteTime(FileDeserializer.GetSaveFilePath()))
			{
				lastChange = File.GetLastWriteTime(FileDeserializer.GetSaveFilePath());
				FileDeserializer.UpdateData();
				CheckData();

				Console.Clear();
				Console.SetCursorPosition(0, 0);
				DataPrinter.PrintData(FileDeserializer.Data, CategoryChanger.Category, SortingChanger.Sorting);
				InterfacePrinter.PrintInputPrompt();
				Console.Write(InputReader.GetInput());
				Log.Debug("Data updated. Last Change: {lastChange}", lastChange);
			}

			await Task.Delay(millisecondsDelay: 2000);
		}
	}

	internal static void CheckData()
	{
		if (FileDeserializer.Data == new Dictionary<string, object>())
		{
			Log.Error("Data empty. File not found.");
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("Data empty. File not found.");
			Console.WriteLine("Path -> " + FileDeserializer.GetSaveFilePath());
			Console.WriteLine("Only Windows is supported currently.");
			Environment.Exit(exitCode: 0);
		}
	}

	internal static void ToggleAutoRefresh()
	{
		_autoRefresh = !_autoRefresh;
		string state = _autoRefresh ? "enabled" : "disabled";
		Console.WriteLine($"\n   Auto-Refresh is now {state}.");
		Log.Information("Auto-Refresh is now {state}", state);
		Console.WriteLine($"   Press any key to continue.");
		Console.ReadKey(intercept: true);
	}
}
