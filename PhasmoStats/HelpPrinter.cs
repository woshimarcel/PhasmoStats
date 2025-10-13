using Logic;

namespace PhasmoStats;

internal class HelpPrinter
{
	private const ConsoleColor FIRST_COLOR = ConsoleColor.White;
	private const ConsoleColor SECOND_COLOR = ConsoleColor.DarkGray;
	private const int FIRST_COLUMN_LEFT = -2;
	private const int EXPLANATION_LEFT = -30;
	const string HELP_CATEGORY = "help category";
	const string HELP_REFRESH = "help refresh";
	const string HELP_SORTING = "help sorting";

	internal static void Help(string input)
	{
		CategoryChanger.Category = Categories.None;

		Console.ForegroundColor = ConsoleColor.White;
		Console.Clear();
		InterfacePrinter.PrintDivider();
		Console.WriteLine("Hello and welcome to woshi's help center.");
		Console.WriteLine("Here are some information about this program:\n");

		string[] help = GetHelp(input);
		PrintHelp(help);
		InterfacePrinter.PrintInputPrompt();
	}

	private static string[] GetHelp(string input)
	{
		return input switch
		{
			HELP_CATEGORY => GetCategoryHelp(),
			HELP_SORTING => GetSortingHelp(),
			HELP_REFRESH => GetRefreshHelp(),
			_ => GetBasicHelp(),
		};
	}

	private static string[] GetBasicHelp()
	{
		return
		[
			$"{"",FIRST_COLUMN_LEFT} {$"'help'",EXPLANATION_LEFT} This very cool window! :D",
			$"{"",FIRST_COLUMN_LEFT} {$"'{HELP_CATEGORY}'",EXPLANATION_LEFT} Detailed help about categories",
			$"{"",FIRST_COLUMN_LEFT} {$"'{HELP_SORTING}'",EXPLANATION_LEFT} Detailed help about sorting",
			$"{"",FIRST_COLUMN_LEFT} {$"'{HELP_REFRESH}'",EXPLANATION_LEFT} Detailed help about refreshing data",
		];
	}

	private static string[] GetCategoryHelp()
	{
		List<string> help =
		[
			$"{"",FIRST_COLUMN_LEFT} Categories are the main feature of this program.",
			$"{"",FIRST_COLUMN_LEFT} They display the data you want to see.\n",
			$"{"",FIRST_COLUMN_LEFT} There are multiple ways to write the category-command. \n{"",FIRST_COLUMN_LEFT} ? = category",
		];

		foreach (string option in CategoryChanger.GetWritingOptions())
			help.Add($"{"",FIRST_COLUMN_LEFT} - {option}?");

		help.Add($"\n{"",FIRST_COLUMN_LEFT} Tip: Categories can be displayed by writing just the first letter of each word! (e.g. 'g' for ghosts)");
		return [.. help];
	}

	private static string[] GetSortingHelp()
	{
		List<string> help =
		[
			$"{"",FIRST_COLUMN_LEFT} Sorting was introduced to make looking for a specific ghost easier.",
			$"{"",FIRST_COLUMN_LEFT} Now, most categories can be sorted in a specific way.\n",
			$"{"",FIRST_COLUMN_LEFT} There are multiple ways to write the sort-command. \n{"",FIRST_COLUMN_LEFT} ? = sorting",
		];

		foreach (string option in SortingChanger.GetWritingOptions())
			help.Add($"{"",FIRST_COLUMN_LEFT} - {option}?");

		help.Add($"\n{"",FIRST_COLUMN_LEFT} Tip: Sortings can be applied by writing just the first letter of each word! (e.g. 'a' for alphabetically)");
		return [.. help];
	}

	private static string[] GetRefreshHelp()
	{
		List<string> help =
		[
			$"{"",FIRST_COLUMN_LEFT} There are 2 types of refreshs: manual refresh and auto refresh.",
			$"{"",FIRST_COLUMN_LEFT} To refresh manually, your only option is to write 'refresh'.\n",
			$"{"",FIRST_COLUMN_LEFT} Auto-Refresh happens every time the Phasmophobia-Save-File is being changed.",
			$"{"",FIRST_COLUMN_LEFT} This happens when leaving a lobby or the game and starting or leaving a case.\n",
			$"{"",FIRST_COLUMN_LEFT} There are multiple ways to write the refresh-command.",
		];

		foreach (string option in FileUpdater.GetWritingOptions())
			help.Add($"{"",FIRST_COLUMN_LEFT} - {option}");
		return [.. help];
	}

	private static void PrintHelp(string[] help)
	{
		bool color = false;
		foreach (string text in help)
		{
			Console.ForegroundColor = color ? SECOND_COLOR : FIRST_COLOR;
			Console.WriteLine(text);
			color = !color;
		}
		Console.WriteLine();
	}
}
