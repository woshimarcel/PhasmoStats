namespace Logic;

public class SaveKeys
{
	public const string MOST_COMMON_GHOSTS = "mostCommonGhosts";
	public const string PLAYED_MAPS = "playedMaps";
	public const string GHOST_KILLS = "ghostKills";
	/// <summary>
	/// Needs an index number at the end, starting at 0
	/// </summary>
	public const string BONE = "Bone";
	public const string MISIDENTIFIED = "ghostsMisidentifiedAmount";
	public const string IDENTIFIED = "ghostsIdentifiedAmount";
	public const string TIME_INVESTIGATED = "timeSpentInvestigating";
	public const string VIDEOS_TAKEN = "videosTaken";
	public const string SOUNDS_TAKEN = "soundsTaken";
	public const string PHOTOS_TAKEN = "photosTaken";
	public const string REVIVES = "revivedAmount";
	/// <summary>
	/// Includes the old bone
	/// </summary>
	public const string BONES_COLLECTED = "amountOfBonesCollected";
	public const string DEATHS = "diedAmount";
	public const string OBJECTIVES_COMPLETED = "objectivesCompleted";
	public const string CURSED_HUNTS_TRIGGERED = "amountOfCursedHuntsTriggered";
	public const string TIME_IN_TRUCK = "timeSpentInTruck";
	public const string TIME_CHASED = "timeSpentBeingChased";
	public const string TIME_IN_GHOST_ROOM = "timeSpentInGhostsRoom";
	public const string TIME_IN_DARK = "timeSpentInDark";
	public const string TIME_IN_LIGHT = "timeSpentInLight";
	public const string SANITY_GAINED = "sanityGained";
	public const string SANITY_LOST = "sanityLost";
	public const string PLAYER_DISTANCE_TRAVELLED = "distanceTravelled";
	public const string TIME_HUNTED = "totalHuntTime";
	public const string HUNTS = "amountOfGhostHunts";
	public const string TIME_FAVORITE_ROOM = "timeInFavouriteRoom";
	public const string ABILITIES_USED = "abilitiesUsed";
	public const string GHOST_EVENTS = "amountOfGhostEvents";
	public const string FUSE_BOX_TOGGLES = "fuseboxToggles";
	public const string LIGHT_SWITCHES = "lightsSwitched";
	public const string DOORS_MOVED = "doorsMoved";
	public const string OBJECTS_USED = "objectsUsed";
	public const string INTERACTIONS = "amountOfGhostInteractions";
	public const string GHOST_DISTANCE_TRAVELLED = "ghostDistanceTravelled";
	public const string ROOM_CHANGED = "roomChanged";
	public const string PHRASES_RECOGNIZED = "phrasesRecognized";
	public const string EXPERIENCE = "Experience";
	public const string LEVEL = "NewLevel";
	public const string MONEY = "PlayersMoney";
	/// <summary>
	/// Might be an incorrect number. Further testing required. <br />
	/// At level 94, you should have a minimum of 254,375 total XP, i had 171,923. <br />
	/// It cannot be my XP pre-progression because I was level 3663; way above 171,923 total XP
	/// </summary>
	public const string TOTAL_EXPERIENCE = "myTotalExp";
	public const string PRESTIGE = "Prestige";
}
