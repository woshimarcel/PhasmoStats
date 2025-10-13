namespace PhasmoStats;

using Logic;
using System;

public class Program
{
	private static void Main(string[] args)
	{
		Logger.SetupLogger();
		FileUpdater.CheckData();
		Console.Title = "PhasmoStats";
		InterfacePrinter.PrintInputPrompt();
		Task.Run(FileUpdater.UpdateFile);
		Task.Run(InputReader.ReadInput);

		while (true) { }
	}
}
