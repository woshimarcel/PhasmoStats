using Logic;
using Serilog;

namespace PhasmoStats;

internal class CategoryChanger
{
	internal static Categories Category { get; set; } = Categories.None;
	private static readonly string[] _writingOptions =
	[
		"category ",
		"cat ",
		"c ",
	];

	internal static string[] GetWritingOptions()
	{
		return _writingOptions;
	}

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

	internal static void UpdateCategory(string input)
	{
		Categories category = Category;
		TryGetCategory(input, ref category);
		Category = category;
		Log.Debug("Category changed to {category}", Category);
	}

	private static bool TryGetCategory(string input, ref Categories category)
	{
		input = input.ToLower();
		Categories temp = category;
		category = input switch
		{
			"all" or "a" => Categories.All,
			"ghosts" or "g" => Categories.Ghosts,
			"maps" or "m" => Categories.Maps,
			"cursed objects" or "co" => Categories.CursedObjects,
			"bones" or "b" => Categories.Bones,
			"none" or "n" => Categories.None,
			"case" or "c" => Categories.Case,
			_ => Categories.NOT_FOUND,
		};

		if (category == Categories.NOT_FOUND)
		{
			if (temp == Categories.NOT_FOUND)
				category = Categories.None;
			else
				category = temp;

			return false;
		}

		return true;
	}
}
