using Serilog;

namespace PhasmoStats;

internal class SortingChanger
{
	internal static Sortings Sorting { get; set; } = Sortings.Percentage;
	private static readonly string[] _writingOptions =
	[
		"sort ",
		"s ",
	];

	internal static bool IsWritingOption(ref string input)
	{
		foreach (string option in _writingOptions)
		{
			if (input.StartsWith(option))
			{
				input = input[option.Length..];
				return true;
			}
		}

		return false;
	}

	internal static void UpdateSorting(string input)
	{
		input = input.ToLower();
		Sorting = input switch
		{
			"alphabetically" or "alph" or "a" => Sortings.Alphabetically,
			"deaths" or "death" or "d" => Sortings.Deaths,
			"sightings" or "sights" or "s" or "used" or "u" => Sortings.Sightings,
			"percentage" or "p" => Sortings.Percentage,
			_ => Sorting,
		};
		Log.Debug("Sorting changed to {sorting}", Sorting);
	}
}
