namespace PhasmoStatsBlazor.Services;

public class ItemTierService
{
	public enum Items
	{
		EMF,
		Video,
		Box,
		Thermometer,
		UV,
		Book,
		DOTS
	}
	public enum Tiers
	{
		Tier1,
		Tier2,
		Tier3
	}

	public static event Action? OnChange;

	public static string EmfPath { get; private set; } = "Images/Items/Tier1/emf.webp";
	public static string VideoPath { get; private set; } = "Images/Items/Tier1/video.webp";
	public static string BoxPath { get; private set; } = "Images/Items/Tier1/box.webp";
	public static string ThermoPath { get; private set; } = "Images/Items/Tier1/thermometer.webp";
	public static string UvPath { get; private set; } = "Images/Items/Tier1/uv.webp";
	public static string BookPath { get; private set; } = "Images/Items/Tier1/book.webp";
	public static string DotsPath { get; private set; } = "Images/Items/Tier1/dots.webp";

	public static void ChangeItemTier(Items item, Tiers tier)
	{
		string tierFolder = tier switch
		{
			Tiers.Tier1 => "Tier1",
			Tiers.Tier2 => "Tier2",
			Tiers.Tier3 => "Tier3",
			_ => "Tier1"
		};

		string file = item switch
		{
			Items.EMF => "emf.webp",
			Items.Video => "video.webp",
			Items.Box => "box.webp",
			Items.Thermometer => "thermometer.webp",
			Items.UV => "uv.webp",
			Items.Book => "book.webp",
			Items.DOTS => "dots.webp",
			_ => "emf.webp"
		};

		string path = $"Images/Items/{tierFolder}/{file}";

		switch (item)
		{
			case Items.EMF: EmfPath = path; break;
			case Items.Video: VideoPath = path; break;
			case Items.Box: BoxPath = path; break;
			case Items.Thermometer: ThermoPath = path; break;
			case Items.UV: UvPath = path; break;
			case Items.Book: BookPath = path; break;
			case Items.DOTS: DotsPath = path; break;
		}

		OnChange?.Invoke();
	}
}