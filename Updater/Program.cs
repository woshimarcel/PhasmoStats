namespace Updater;

internal class Program
{
	[STAThread]
	static void Main(string[] args)
	{
		string mode = string.Empty;
		string fromVer = string.Empty;
		string toVer = string.Empty;

		for (int i = 0; i < args.Length; i++)
		{
			switch (args[i])
			{
				case "--update": mode = "update"; break;
				case "--from": fromVer = args[++i]; break;
				case "--to": toVer = args[++i]; break;
			}
		}

		var app = new App();
		app.Run(new MainWindow(mode, fromVer, toVer));
	}
}
