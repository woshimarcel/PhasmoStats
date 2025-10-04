namespace Logic;

public class SaveFile
{
	public Wrapper<Dictionary<string, int>> mostCommonGhosts { get; set; }
	public Wrapper<Dictionary<string, int>> ghostKills { get; set; }
	public Wrapper<Dictionary<int, int>> playedMaps { get; set; }
}

public class Wrapper<T>
{
	public string __type { get; set; }
	public T Value { get; set; }
}