namespace PortalToUnity
{
#if UNITY_EDITOR
    [MarkAsHeroicChallengeID]
#endif
    public enum HeroicChallengeID
    {
        ChompyChompDown = 0,
        ThisBombsForYou = 1,
        JumpForIt = 2,
        WhereArtThouPaintings = 3,
        LairOfTheGiantSpiders = 4,
        FightTeleportFight = 5,
        TheThreeTeleporters = 6,
        StopSheepThieves = 7,
        MiningForCharms = 8,
        DungeonessCreeps = 9,
        MiningIsTheKey = 10,
        MissionAchomplished = 11,
        PodGauntlet = 12,
        TimesAWastin = 13,
        SaveThePurpleChompies = 14,
        SpawnerCave = 15,
        ArachnidAntechamber = 16,
        HobsonsChoice = 17,
        IsleOfTheAutomatons = 18,
        YouBreakItYouBuyIt = 19,
        MinefieldMishap = 20,
        LobsOFun = 21,
        SpellPunked = 22,
        CharmHunt = 23,
        FlipTheScript = 24,
        YouveStolenMyHearts = 25,
        BombsToTheWalls = 26,
        OperationSheepFreedom = 27,
        Jailbreak = 28,
        EnvironmentallyUnfriendly = 29,
        ChemicalCleanup = 30,
        BreakTheCats = 31,
        FlamePiratesOnIce = 32,
        SkylandsSalute = 33,
        SABRINA = 34,
        TheSkyIsFalling = 35,
        NortsWinterClassic = 36,
        BreakTheFakes = 37,
        BakingWithBatterson = 38,
        BlobbersFolly = 39,
        DeliveryDay = 46,
        GiveAHoot = 47,
        ZombieDanceParty = 48,
        ShepherdsPie = 49,
        WatermelonsEleven = 50,
        ARealGoatGetter = 51,
        WoolyBullies = 52,
        TheGreatPancakeSlalom = 53,
        ShootFirstShootLater = 54,
        TheKingsBreech = 55
    }

    public static class HeroicChallengeIDExtensions
    {
		public const int SSA_Low = 0;
		public const int SSA_High = 31;

		public const int Giants_Low = 32;
		public const int Giants_High = 55;

		public static bool IsSSA(int heroicID) => heroicID.RangeInclusive(SSA_Low, SSA_High);
		public static bool IsGiants(int heroicID) => heroicID.RangeInclusive(Giants_Low, Giants_High);
    }
}