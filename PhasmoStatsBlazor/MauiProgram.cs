namespace PhasmoStatsBlazor
{
	public static class MauiProgram
	{
		public static MauiApp CreateMauiApp()
		{
			var builder = MauiApp.CreateBuilder();
			builder
				.UseMauiApp<App>()
				.ConfigureFonts(fonts =>
				{
					fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
					fonts.AddFont("October Crow.ttf", "October Crow");
					fonts.AddFont("Codystar-Regular.ttf", "Codystar");
					fonts.AddFont("RockSalt.ttf", "Rock Salt");
				});

			builder.Services.AddMauiBlazorWebView();

#if DEBUG
			builder.Services.AddBlazorWebViewDeveloperTools();
			builder.Logging.AddDebug();
#endif

			Task.Run(FileUpdater.UpdateFile);
			return builder.Build();
		}
	}
}
