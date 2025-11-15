using Logic;
using System.Runtime.InteropServices;
#if WINDOWS
using WinRT.Interop;
#endif

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
			Window window = new Window(new MainPage()) { Title = "PhasmoStats" };
#if WINDOWS
			TryMaximizeWindow(window);
#endif
			return window;
		}

#if WINDOWS
		private static void TryMaximizeWindow(Window window)
		{
			window.Dispatcher.Dispatch(async () =>
			{
				await Task.Delay(20);

				var nativeWin = window?.Handler?.PlatformView as Microsoft.UI.Xaml.Window;
				if (nativeWin is null)
					return;

				IntPtr hwnd = WindowNative.GetWindowHandle(nativeWin);
				const int SW_MAXIMIZE = 3;
				ShowWindow(hwnd, SW_MAXIMIZE);
			});
		}

		[DllImport("user32.dll")]
		private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
#endif
	}
}
