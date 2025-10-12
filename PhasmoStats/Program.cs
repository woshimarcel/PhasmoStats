namespace PhasmoStats;

using Logic;
using System;
using System.Collections.Generic;

public class Program
{
	public static Categories Category { get; private set; } = Categories.None;
	public static Sortings Sorting { get; private set; } = Sortings.Percentage;
	private static string _input = string.Empty;

	public enum Sortings
	{
		Alphabetically,
		Sightings,
		Deaths,
		Percentage
	}

	private static void Main(string[] args)
	{
		CheckData();
		string path = Directory.GetCurrentDirectory();
		File.AppendAllText(path + "/Log.txt", $"[{DateTime.Now}] Starte Applikation\n");
		Console.Title = "PhasmoStats";
		InterfacePrinter.PrintInputPrompt();
		Task.Run(CheckFileUpdates);
		Task.Run(ReadInput);

		while (true) { }
	}

	private static async Task CheckFileUpdates()
	{
		DateTime lastChange = File.GetLastWriteTime(FileDeserializer.GetSaveFilePath());
		while (true)
		{
			if (lastChange < File.GetLastWriteTime(FileDeserializer.GetSaveFilePath()))
			{
				lastChange = File.GetLastWriteTime(FileDeserializer.GetSaveFilePath());
				File.AppendAllText(Directory.GetCurrentDirectory() + "/Log.txt", $"[{DateTime.Now}] Erfolgreich geupdatet\n");
				FileDeserializer.UpdateData();
				CheckData();

				Console.Clear();
				Console.SetCursorPosition(0, 0);
				PrintData(FileDeserializer.Data, Category, Sorting);
				InterfacePrinter.PrintInputPrompt();
				Console.Write(_input);
			}

			await Task.Delay(millisecondsDelay: 2000);
		}
	}

	private static async Task ReadInput()
	{
		while (true)
		{
			if (Console.KeyAvailable)
			{
				ConsoleKeyInfo keyInfo = Console.ReadKey(intercept: true);

				if (keyInfo.Key == ConsoleKey.Backspace && _input.Length > 0)
				{
					Console.Write("\b \b");
					_input = _input[..^1];
				}
				else if (keyInfo.Key == ConsoleKey.Enter)
				{
					string input = _input;
					_input = string.Empty;

					if (input == "refresh")
					{
						FileDeserializer.UpdateData();
					}
					else if (input.StartsWith("sort "))
					{
						Sorting = GetSorting(input, Sorting);
					}
					else
					{
						Categories category = Category;
						TryGetCategory(input, ref category);
						Category = category;
					}

					Console.Clear();
					if (Category == Categories.None)
					{
						InterfacePrinter.PrintInputPrompt();
						continue;
					}

					PrintData(FileDeserializer.Data, Category, Sorting);
					InterfacePrinter.PrintInputPrompt();
				}
				else if (char.IsLetter(keyInfo.KeyChar) || keyInfo.KeyChar == ' ')
				{
					Console.Write(keyInfo.KeyChar);
					_input += keyInfo.KeyChar;
				}
			}

			await Task.Delay(millisecondsDelay: 10);
		}
	}

	private static void CheckData()
	{
		if (FileDeserializer.Data == new Dictionary<string, object>())
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("File not found.");
			Console.WriteLine("Path -> " + FileDeserializer.GetSaveFilePath());
			Console.WriteLine("Only Windows is supported currently.");
			Environment.Exit(exitCode: 0);
		}
	}

	private static void PrintData(Dictionary<string, object> data, Categories category, Sortings sorting)
	{
		if (CompareCategory(category, Categories.Ghosts))
			DataPrinter.PrintGhosts(data, sorting);

		if (CompareCategory(category, Categories.Maps))
			DataPrinter.PrintMaps(data, sorting);

		if (CompareCategory(category, Categories.Bones))
			DataPrinter.PrintBones(data, sorting);

		if (CompareCategory(category, Categories.CursedObjects))
		{
			DataPrinter.PrintCursedObjects(data, sorting);
			DataPrinter.PrintTarots(data, sorting);
		}
	}

	private static Sortings GetSorting(string input, Sortings currentSorting)
	{
		int sortLength = "sort ".Length;
		if (input.Length < sortLength)
			return currentSorting;

		input = input[sortLength..].ToLower();
		return input switch
		{
			"alphabetically" or "alph" or "a" => Sortings.Alphabetically,
			"deaths" or "death" or "d" => Sortings.Deaths,
			"sightings" or "sights" or "s" or "used" or "u" => Sortings.Sightings,
			"percentage" or "p" => Sortings.Percentage,
			_ => currentSorting,
		};
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

	private static bool CompareCategory(Categories selected, Categories category)
	{
		return selected == Categories.All || selected == category;
	}
}
