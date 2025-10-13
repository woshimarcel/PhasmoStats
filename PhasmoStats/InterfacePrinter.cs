namespace PhasmoStats;

internal static class InterfacePrinter
{
	internal static void PrintDivider()
	{
		const string DIVIDER = "   ---=====--- ---=====---";
		Console.WriteLine();
		Console.WriteLine();
		Console.WriteLine();
		Console.WriteLine(DIVIDER);
		Console.WriteLine();
	}

	private static void PrintCategories(int top)
	{
		const int LEFT = 30;
		top -= 6;
		if (top < 0)
			top = Console.CursorTop;

		Console.SetCursorPosition(LEFT, top);
		Console.WriteLine("Categories:");
		top = Console.CursorTop;
		Console.SetCursorPosition(LEFT, top);
		Console.WriteLine("All | Ghosts | Maps | Bones");
		top = Console.CursorTop;
		Console.SetCursorPosition(LEFT, top);
		Console.WriteLine("Cursed Objects | Case");
	}

	private static void PrintSortings(int top)
	{
		const int LEFT = 70;
		top -= 6;
		if (top < 0)
			top = Console.CursorTop;

		Console.SetCursorPosition(LEFT, top);
		Console.WriteLine("Sortings:");
		top = Console.CursorTop;
		Console.SetCursorPosition(LEFT, top);
		Console.WriteLine("Alphabetically | Deaths");
		top = Console.CursorTop;
		Console.SetCursorPosition(LEFT, top);
		Console.WriteLine("Sightings/Used | Percentage");
	}

	internal static void PrintInputPrompt()
	{
		Console.ForegroundColor = ConsoleColor.White;
		PrintDivider();
		Console.WriteLine("Please select a category.");
		Console.WriteLine("Type 'help' if you're lost.\n");
		int top = Console.CursorTop;
		PrintCategories(top);
		PrintSortings(top);
		Console.SetCursorPosition(left: 0, top);
		Console.Write(" > ");
	}

	internal static void PrintHelp()
	{
		Console.ForegroundColor = ConsoleColor.White;
		Console.Clear();
		PrintDivider();
		Console.WriteLine("Hello and welcome to woshi's help center.");
		Console.WriteLine("Here are some information about this program:\n");
		const int FIRST_COLUMN_LEFT = -3;
		const int EXPLANATION_LEFT = -30;
		List<string> help = new()
		{
			$"{"",FIRST_COLUMN_LEFT} {"'help'",EXPLANATION_LEFT} Prints this window (cool, right?)",
			$"{"",FIRST_COLUMN_LEFT} {"'x'",EXPLANATION_LEFT} Displays a category (just first letter can be used)",
			$"{"",FIRST_COLUMN_LEFT} {"'sort x'",EXPLANATION_LEFT} Sorts most stats in a specific way (just first letter can be used)",
			$"{"",FIRST_COLUMN_LEFT} {"'refresh'",EXPLANATION_LEFT} Refreshes the data",
			$"{"",FIRST_COLUMN_LEFT} {"'toggle auto-refresh'",EXPLANATION_LEFT} Toggles auto-refresh (short: 'toggle ar')",
		};

		bool color = false;
		foreach (string text in help)
		{
			Console.ForegroundColor = color ? ConsoleColor.DarkGray : ConsoleColor.White;
			Console.WriteLine(text);
			color = !color;
		}
		Console.WriteLine();
	}
}
