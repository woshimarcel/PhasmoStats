namespace PhasmoStatsBlazor;

internal class DataRefresher
{
	public static event Action? OnDataUpdated;
	public static void NotifyDataUpdated() => OnDataUpdated?.Invoke();
}
