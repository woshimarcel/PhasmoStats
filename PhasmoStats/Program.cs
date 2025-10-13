namespace PhasmoStats;

using Logic;
using Serilog;
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
		SetupLogger();
		CheckData();
		string path = Directory.GetCurrentDirectory();
		Console.Title = "PhasmoStats";
		InterfacePrinter.PrintInputPrompt();
		Task.Run(CheckFileUpdates);
		Task.Run(ReadInput);

		while (true) { }
	}

	private static void SetupLogger()
	{
		Log.Logger = new LoggerConfiguration()
			.MinimumLevel.Debug()
			.WriteTo.File("log.txt", rollingInterval: RollingInterval.Day)
			.CreateLogger();

		LogDivider();
		Log.Information("Application starting...");

		AppDomain.CurrentDomain.ProcessExit += (s, e) =>
		{
			Log.Information("Exiting Application...");
			LogDivider();
			Log.CloseAndFlush();
		};
	}

	private static void LogDivider()
	{
		Log.Information("");
		Log.Information("=================================");
		Log.Information("");
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
				Log.Debug("Data updated. Last Change: {lastChange}", lastChange);
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
					Log.Debug($"Input - {input}");
					_input = string.Empty;

					if (input == "refresh")
					{
						Log.Debug("Refreshing manually.");
						FileDeserializer.UpdateData();
					}
					else if (input.StartsWith("sort "))
					{
						Sorting = GetSorting(input, Sorting);
						Log.Debug("Sorting changed to {sorting}", Sorting);
					}
					else
					{
						Categories category = Category;
						TryGetCategory(input, ref category);
						Category = category;
						Log.Debug("Category changed to {category}", Category);
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
			Log.Error("Data empty. File not found.");
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("Data empty. File not found.");
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

		if (CompareCategory(category, Categories.Case))
			DataPrinter.PrintCaseData(data);
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

	private static bool CompareCategory(Categories selected, Categories category)
	{
		return selected == Categories.All || selected == category;
	}
}
