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
		top -= 5;
		if (top < 0)
			top = Console.CursorTop;

		Console.SetCursorPosition(LEFT, top);
		Console.WriteLine("Categories:");
		top = Console.CursorTop;
		Console.SetCursorPosition(LEFT, top);
		Console.WriteLine("All | Ghosts | Maps");
		top = Console.CursorTop;
		Console.SetCursorPosition(LEFT, top);
		Console.WriteLine("Bones | Cursed Objects");
	}

	private static void PrintSortings(int top)
	{
		const int LEFT = 60;
		top -= 5;
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
		Console.WriteLine("Please select a category.\n");
		int top = Console.CursorTop;
		PrintCategories(top);
		PrintSortings(top);
		Console.SetCursorPosition(left: 0, top);
		Console.Write(" > ");
	}
}
