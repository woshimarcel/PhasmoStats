using Logic;
using Serilog;

namespace PhasmoStats;

internal static class InputProcessor
{
	internal static void ProcessInput(string input)
	{
		Log.Debug($"Input - {input}");

		if (input == "help")
		{
			Help();
			return;
		}

		if (input == "refresh" || input == "rf")
			ManualRefresh();
		else if (FileUpdater.IsWritingOption(input))
			FileUpdater.ToggleAutoRefresh();
		else if (SortingChanger.IsWritingOption(ref input))
			SortingChanger.UpdateSorting(input);
		else if (CategoryChanger.IsWritingOption(ref input))
			CategoryChanger.UpdateCategory(input);

		Console.Clear();
		if (CategoryChanger.Category == Categories.None)
		{
			InterfacePrinter.PrintInputPrompt();
			return;
		}

		DataPrinter.PrintData(FileDeserializer.Data, CategoryChanger.Category, SortingChanger.Sorting);
		InterfacePrinter.PrintInputPrompt();
	}

	private static void ManualRefresh()
	{
		Log.Debug("Refreshing manually.");
		FileDeserializer.UpdateData();
	}

	private static void Help()
	{
		CategoryChanger.Category = Categories.None;
		InterfacePrinter.PrintHelp();
		InterfacePrinter.PrintInputPrompt();
	}
}
