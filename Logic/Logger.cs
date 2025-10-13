using Serilog;

namespace Logic;

public class Logger
{
	public static void SetupLogger()
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
}
