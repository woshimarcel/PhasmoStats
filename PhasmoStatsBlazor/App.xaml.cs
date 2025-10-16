using Logic;

namespace PhasmoStatsBlazor
{
	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();
		}

		protected override Window CreateWindow(IActivationState? activationState)
		{
			Logger.SetupLogger();
			Task.Run(async () => await GitHubUpdater.CheckForUpdateAsync());
			Window window = new Window(new MainPage()) { Title = "PhasmoStats" };
			return window;
		}
	}
}
