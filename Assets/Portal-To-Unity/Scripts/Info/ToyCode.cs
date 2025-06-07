// a.k.a Character ID/kTfbSpyroTag_ToyType

namespace PortalToUnity
{
#if UNITY_EDITOR
    [MarkAsToyCode]
#endif
    public enum ToyCode
    {
        // TFB figures (debugging characters all end with "99")

        // SSA Characters (0-99)
        Character_Whirlwind = 0,
        Character_SonicBoom = 1,
        Character_Warnado = 2,
        Character_LightningRod = 3,
        Character_Bash = 4,
        Character_Terrafin = 5,
        Character_DinoRang = 6,
        Character_PrismBreak = 7,
        Character_Sunburn = 8,
        Character_Eruptor = 9,
        Character_Ignitor = 10,
        Character_Flameslinger = 11,
        Character_Zap = 12,
        Character_WhamShell = 13,
        Character_GillGrunt = 14,
        Character_SlamBam = 15,
        Character_Spyro = 16,
        Character_Voodood = 17,
        Character_DoubleTrouble = 18,
        Character_TriggerHappy = 19,
        Character_Drobot = 20,
        Character_DrillSergeant = 21,
        Character_Boomer = 22,
        Character_WreckingBall = 23,
        Character_Camo = 24,
        Character_Zook = 25,
        Character_StealthElf = 26,
        Character_StumpSmash = 27,
        Character_DarkSpyro = 28,
        Character_Hex = 29,
        Character_ChopChop = 30,
        Character_GhostRoaster = 31,
        Character_Cynder = 32,

        // Giants characters (100-199)
        Character_JetVac = 100,
        Character_Swarm = 101,
        Character_Crusher = 102,
        Character_Flashwing = 103,
        Character_HotHead = 104,
        Character_HotDog = 105,
        Character_Chill = 106,
        Character_Thumpback = 107,
        Character_PopFizz = 108,
        Character_Ninjini = 109,
        Character_Bouncer = 110,
        Character_Sprocket = 111,
        Character_TreeRex = 112,
        Character_Shroomboom = 113,
        Character_EyeBrawl = 114,
        Character_FrightRider = 115,

        // TFB Items/Traps (200-299)
        Item_AnvilRain = 200,
        Item_HiddenTreasure = 201,
        Item_HealingElixir = 202,
        Item_GhostPirateSwords = 203,
        Item_TimeTwisterHourglass = 204,
        Item_SkyIronShield = 205,
        Item_WingedBoots = 206,
        Item_SparxDragonfly = 207,
        BattlePiece_Cannon = 208,
        BattlePiece_Catapult = 209,
        Trap_Magic = 210,
        Trap_Water = 211,
        Trap_Air = 212,
        Trap_Undead = 213,
        Trap_Tech = 214,
        Trap_Fire = 215,
        Trap_Earth = 216,
        Trap_Life = 217,
        Trap_Dark = 218,
        Trap_Light = 219,
        Trap_Kaos = 220,
        Item_HandOfFate = 230,
        Item_PiggyBank = 231,
        Item_RocketRam = 232,
        Item_TikiSpeaky = 233,
        Item_MysteryChest = 235,

        // TFB Expansions (300-399)
        Expansion_DragonsPeak = 300,
        Expansion_EmpireOfIce = 301,
        Expansion_PirateSeas = 302,
        Expansion_DarklightCrypt = 303,
        Expansion_VolcanicVault = 304,
        Expansion_MirrorOfMystery = 305,
        Expansion_NightmareExpress = 306,
        Expansion_SunscraperSpire = 307,
        Expansion_MidnightMuseum = 308,
        Expansion_GryphonParkObservatory = 310,
        Expansion_EnchantedElvenForest = 311,

        // SSA Legendaries (400-449)
        Character_LegendaryBash = 404,
        Character_LegendarySpyro = 416,
        Character_LegendaryTriggerHappy = 419,
        Character_LegendaryChopChop = 430,

        // Trap Team characters (450-499)
        Character_Gusto = 450,
        Character_Thunderbolt = 451,
        Character_FlingKong = 452,
        Character_Blades = 453,
        Character_Wallop = 454,
        Character_HeadRush = 455,
        Character_FistBump = 456,
        Character_RockyRoll = 457,
        Character_Wildfire = 458,
        Character_KaBoom = 459,
        Character_TrailBlazer = 460, 
        Character_Torch = 461,
        Character_SnapShot = 462,
        Character_LobStar = 463,
        Character_FlipWreck = 464,
        Character_Echo = 465,
        Character_Blastermind = 466,
        Character_Enigma = 467,
        Character_DejaVu = 468,
        Character_CobraCadabra = 469,
        Character_Jawbreaker = 470,
        Character_Gearshift = 471,
        Character_Chopper = 472,
        Character_TreadHead = 473,
        Character_Bushwhack = 474,
        Character_TuffLuck = 475,
        Character_FoodFight = 476,
        Character_HighFive = 477,
        Character_KryptKing = 478,
        Character_ShortCut = 479,
        Character_BatSpin = 480,
        Character_FunnyBone = 481,
        Character_KnightLight = 482,
        Character_Spotlight = 483,
        Character_KnightMare = 484,
        Character_Blackout = 485,

        // Minis/Sidekicks (500-599)
        Mini_Bop = 502,
        Mini_Spry = 503,
        Mini_Hijinx = 504,
        Mini_Terrabite = 505,
        Mini_Breeze = 506,
        Mini_Weeruptor = 507,
        Mini_PetVac = 508,
        Mini_SmallFry = 509,
        Mini_Drobit = 510,
        Mini_GillRunt = 511,
        Mini_TriggerSnappy = 519,
        Mini_WhisperElf = 526,
        Mini_Barkley = 540,
        Mini_Thumpling = 541,
        Mini_MiniJini = 542,
        Mini_EyeSmall = 543,

        // Imaginators characters (600-699)
        Character_KingPen = 601,
        Character_TriTip = 602,
        Character_Chopscotch = 603,
        Character_BoomBloom = 604,
        Character_PitBoss = 605,
        Character_Barbella = 606,
        Character_AirStrike = 607,
        Character_Ember = 608,
        Character_Ambush = 609,
        Character_DrKrankcase = 610,
        Character_HoodSickle = 611,
        Character_TaeKwonCrow = 612,
        Character_GoldenQueen = 613,
        Character_Wolfgang = 614,
        Character_PainYatta = 615,
        Character_Mysticat = 616,
        Character_Starcast = 617,
        Character_Buckshot = 618,
        Character_Aurora = 619,
        Character_FlareWolf = 620,
        Character_ChompyMage = 621,
        Character_BadJuju = 622,
        Character_GraveClobber = 623,
        Character_BlasterTron = 624,
        Character_RoBow = 625,
        Character_ChainReaction = 626,
        Character_Kaos = 627,
        Character_WildStorm = 628,
        Character_Tidepool = 629,
        Character_CrashBandicoot = 630,
        Character_DrNeoCortex = 631,

        // CYOS figures has ID 661 between here. Thanks Fershock
        Character_CYOS = 661,

        // Imaginators Creation Crystals (680-689)
        Crystal_Magic = 680,
        Crystal_Water = 681,
        Crystal_Air = 682,
        Crystal_Undead = 683,
        Crystal_Tech = 684,
        Crystal_Fire = 685,
        Crystal_Earth = 686,
        Crystal_Life = 687,
        Crystal_Dark = 688,
        Crystal_Light = 689,

        Character_Goldie = 699,

        // Debug Minion
        Character_DEBUG = 999,

        // VV used drastically different toy codes, perhaps to prevent interference with TFB IDs

        // SWAP Force bottom halves (1000-1999)
        SwapBottom_Jet = 1000,
        SwapBottom_Ranger = 1001,
        SwapBottom_Rouser = 1002,
        SwapBottom_Stone = 1003,
        SwapBottom_Zone = 1004,
        SwapBottom_Kraken = 1005,
        SwapBottom_Bomb = 1006,
        SwapBottom_Drilla = 1007,
        SwapBottom_Loop = 1008,
        SwapBottom_Shadow = 1009,
        SwapBottom_Charge = 1010,
        SwapBottom_Rise = 1011,
        SwapBottom_Shift = 1012,
        SwapBottom_Shake = 1013,
        SwapBottom_Blade = 1014,
        SwapBottom_Buckler = 1015,
        SwapBottom_Template = 1999,

        // SWAP Force top halves (2000-2999)
        SwapTop_Boom = 2000,
        SwapTop_Free = 2001,
        SwapTop_Rubble = 2002,
        SwapTop_Doom = 2003,
        SwapTop_Blast = 2004,
        SwapTop_Fire = 2005,
        SwapTop_Stink = 2006,
        SwapTop_Grilla = 2007,
        SwapTop_Hoot = 2008,
        SwapTop_Trap = 2009,
        SwapTop_Magna = 2010,
        SwapTop_Spy = 2011,
        SwapTop_Night = 2012,
        SwapTop_Rattle = 2013,
        SwapTop_Freeze = 2014,
        SwapTop_Wash = 2015,
        SwapTop_Template = 2999,

        // SWAP Force cores (3000-3999)
        Character_Scratch = 3000,
        Character_PopThorn = 3001,
        Character_SlobberTooth = 3002,
        Character_Scorp = 3003,
        Character_Fryno = 3004,
        Character_Smolderdash = 3005,
        Character_BumbleBlast = 3006,
        Character_ZooLou = 3007,
        Character_DuneBug = 3008,
        Character_StarStrike = 3009,
        Character_Countdown = 3010,
        Character_WindUp = 3011,
        Character_RollerBrawl = 3012,
        Character_GrimCreeper = 3013,
        Character_RipTide = 3014,
        Character_PunkShock = 3015,
        Character_Template = 3999,

        // VV items/vehicles (3200-3299)
        Item_BattleHammer = 3200,
        Item_SkyDiamond = 3201,
        Item_PlatinumSheep = 3202,
        Item_GrooveMachine = 3203,
        Item_UFOHat = 3204,
        Vehicle_JetStream = 3220,
        Vehicle_TombBuggy = 3221,
        Vehicle_ReefRipper = 3222,
        Vehicle_BurnCycle = 3223,
        Vehicle_HotStreak = 3224,
        Vehicle_SharkTank = 3225,
        Vehicle_ThumpTruck = 3226,
        Vehicle_CryptCrusher = 3227,
        Vehicle_StealthStinger = 3228,
        Vehicle_DiveBomber = 3231,
        Vehicle_SkySlicer = 3232,
        Vehicle_ClownCruiser = 3233,
        Vehicle_GoldRusher = 3234,
        Vehicle_ShieldStriker = 3235,
        Vehicle_SunRunner = 3236,
        Vehicle_SeaShadow = 3237,
        Vehicle_SplatterSplasher = 3238,
        Vehicle_SodaSkimmer = 3239,
        Vehicle_BarrelBlaster = 3240,
        Vehicle_BuzzWing = 3241,

        // SWAP Force Expansions (VV treated Battle Pieces as Expansions it seems) (3300-3399)
        Expansion_SheepWreckIsland = 3300,
        Expansion_TowerOfTime = 3301,
        BattlePiece_FieryForge = 3302,
        BattlePiece_ArkeyanCrossbow = 3303,

        // SuperChargers characters (3400-3439)
        Character_Fiesta = 3400,
        Character_HighVolt = 3401,
        Character_Splat = 3402,
        Character_Stormblade = 3406,
        Character_SmashHit = 3411,
        Character_Spitfire = 3412,
        Character_DriverJetVac = 3413,
        Character_DriverTriggerHappy = 3414,
        Character_DriverStealthElf = 3415,
        Character_DriverTerrafin = 3416,
        Character_DriverRollerBrawl = 3417,
        Character_DriverPopFizz = 3420,
        Character_DriverEruptor = 3421,
        Character_DriverGillGrunt = 3422,
        Character_DonkeyKong = 3423,
        Character_Bowser = 3424,
        Character_DiveClops = 3425,
        Character_Astroblast = 3426,
        Character_Nightfall = 3427,
        Character_Thrillipede = 3428,

        // SuperChargers villain drivers (3440-3469)
        VillainDriver_ChefPepperJack = 3440,
        VillainDriver_Stratosfear = 3441,
        VillainDriver_Cluck = 3442,
        VillainDriver_Wolfgang = 3443,
        VillainDriver_Threatpack = 3444,
        VillainDriver_HoodSickle = 3445,
        VillainDriver_BadJuju = 3446,
        VillainDriver_Glumshanks = 3450,
        VillainDriver_DragonHunter = 3451,
        VillainDriver_Moneybone = 3452,
        VillainDriver_ChompyMage = 3453,
        VillainDriver_DrKrankcase = 3454,
        VillainDriver_PainYatta = 3455,
        VillainDriver_BroccoliGuy = 3456,
        VillainDriver_Mesmeralda = 3460,
        VillainDriver_CaptainFrightbeard = 3461,
        VillainDriver_GoldenQueen = 3462,
        VillainDriver_Spellslamzer = 3463,
        VillainDriver_ChillBill = 3464,
        VillainDriver_Gulper = 3465,
        VillainDriver_Noodles = 3466,
        VillainDriver_Kaos = 3467,

        // SuperChargers villain vehicles (3470-3499)
        VillainVehicle_ToasterBomber = 3470,
        VillainVehicle_StormStriker = 3471,
        VillainVehicle_SkyScrambler = 3472,
        VillainVehicle_SubWoofer = 3473,
        VillainVehicle_Threatpack = 3474,
        VillainVehicle_HoodSickle = 3475,
        VillainVehicle_BadJuju = 3476,
        VillainVehicle_SteamRoller = 3480,
        VillainVehicle_ScaleBiter = 3481,
        VillainVehicle_SpiritDragster = 3482,
        VillainVehicle_ChompyBuster = 3483,
        VillainVehicle_DrKrankcase = 3484,
        VillainVehicle_PainYatta = 3485,
        VillainVehicle_BroccoliGuy = 3486,
        VillainVehicle_WaveSinger = 3490,
        VillainVehicle_LilPhantomTide = 3491,
        VillainVehicle_GlitterGlider = 3492,
        VillainVehicle_RuneSlider = 3493,
        VillainVehicle_ChillBill = 3494,
        VillainVehicle_Gulper = 3495,
        VillainVehicle_Noodles = 3496,
        VillainVehicle_DoomJet = 3497,

        // SuperChargers trophies (3500-3999)
        RacingPack_Sky = 3500,
        RacingPack_Land = 3501,
        RacingPack_Sea = 3502,
        RacingPack_Kaos = 3503,

        // Defective figures
        Character_Nightfall_ERROR = 3527, // ID is 100 more than normal

        // Skylanders 2016 characters (4500-4599)
        Character_Fusion = 4500,
        Character_Synergy = 4501,
        Character_Unity = 4502,
        Character_BlueFalcon = 4503,

        // Template vehicles (VV template characters are in their respective places and all end with "999")
        Vehicle_Template = 4999,
        Vehicle_TemplateLand = 5999,
        Vehicle_TemplateAir = 6999,
        Vehicle_TemplateSea = 7999,

        // Trap Team DEBUG Characters (these don't normally load in any game)
        Character_DEBUG_Core = 9990,
        Character_DEBUG_Giant = 9991,
        Character_DEBUG_Ranger = 9992
    }

    public static class ToyCodeExtensions
    {
        public const int SSA_Low = 0;
        public const int SSA_High = 99;

        public const int Giants_Low = 100;
        public const int Giants_High = 199;

        public const int TFB_Items_Low = 200;

        public const int TFB_BattlePieces_Low = 208;
        public const int TFB_BattlePieces_High = 209;
        public const int Traps_Low = 210;
        public const int Traps_High = 220;

        public const int TFB_Items_High = 299;

        public const int TFB_Expansions_Low = 300;
        public const int TFB_Expansions_High = 399;

        public const int SSA_Legendary_Low = 400;
        public const int SSA_Legendary_High = 449;

        public const int TrapTeam_Low = 450;
        public const int TrapTeam_High = 499;

        public const int Minis_Low = 500;
        public const int Minis_High = 599;

        public const int Imaginators_Low = 600;
        public const int Crystals_Low = 680;
        public const int Imaginators_High = 699;

        public const int SwapBottom_Low = 1000;
        public const int SwapBottom_High = 1999;

        public const int SwapTop_Low = 2000;
        public const int SwapTop_High = 2999;

        public const int SwapForce_Low = 3000;
        public const int SwapForce_High = 3999;

        public const int VV_Items_Low = 3200;
        public const int Vehicles_Low = 3220;
        public const int VV_Items_High = 3299;

        public const int VV_Expansions_Low = 3300;
        public const int VV_BattlePieces_Low = 3320;
        public const int VV_Expansions_High = 3399;

        public const int SuperChargers_Low = 3400;
        public const int SuperChargers_High = 3439;

        public const int VillainDrivers_Low = 3440;
        public const int VillainDrivers_High = 3469;

        public const int VillainVehicles_Low = 3470;
        public const int VillainVehicles_High = 3499;

        public const int RacingPack_Low = 3500;
        public const int RacingPack_High = 3999;

        public const int Sky_2016_Low = 4500;
        public const int Sky_2016_High = 4599;

        public const int TemplateVehicle_Low = 4999;
        public const int TemplateVehicle_High = 7999;

        public const int TrapTeam_Debug_Low = 9990;
        public const int TrapTeam_Debug_High = 9999;

        public static bool IsSSA(int toyCode) => IsSSA_Exclusive(toyCode) || toyCode == (int)ToyCode.Character_DEBUG || IsSSA_AltDeco(toyCode);
        public static bool IsSSA_Legendary(int toyCode) => toyCode.RangeInclusive(SSA_Legendary_Low, SSA_Legendary_High);
        public static bool IsSSA_AltDeco(int toyCode) => IsSSA_Legendary(toyCode) || toyCode == (int)ToyCode.Character_DarkSpyro;
        public static bool IsSSA_AltDeco_Exclusive(int toyCode) => toyCode == (int)ToyCode.Character_DarkSpyro;
        public static bool IsSSA_Exclusive(int toyCode) => toyCode.RangeInclusive(SSA_Low, SSA_High);

        public static bool IsGiants(int toyCode) => toyCode.RangeInclusive(Giants_Low, Giants_High);

        public static bool IsTFB_Item(int toyCode) => toyCode.RangeInclusive(TFB_Items_Low, TFB_Items_High);
        public static bool IsTFB_Item_Exclusive(int toyCode) => IsTFB_Item(toyCode) && !IsTFB_BattlePiece(toyCode) && !IsTrap(toyCode);
        public static bool IsTFB_BattlePiece(int toyCode) => toyCode.RangeInclusive(TFB_BattlePieces_Low, TFB_BattlePieces_High);
        public static bool IsTrap(int toyCode) => toyCode.RangeInclusive(Traps_Low, Traps_High);

        public static bool IsTFB_Expansion(int toyCode) => toyCode.RangeInclusive(TFB_Expansions_Low, TFB_Expansions_High);

        public static bool IsTrapTeam(int toyCode) => toyCode.RangeInclusive(TrapTeam_Low, TrapTeam_High);

        public static bool IsMini(int toyCode) => toyCode.RangeInclusive(Minis_Low, Minis_High);

        public static bool IsImaginators(int toyCode) => toyCode.RangeInclusive(Imaginators_Low, Imaginators_High);
        public static bool IsImaginators_Exclusive(int toyCode) => toyCode == Imaginators_High || toyCode.RangeInclusive(Imaginators_Low, Imaginators_High) && !IsCrystal(toyCode);
        public static bool IsCrystal(int toyCode) => toyCode.RangeInclusive(Crystals_Low, Imaginators_High);

        public static bool IsSwapBottom(int toyCode) => toyCode.RangeInclusive(SwapBottom_Low, SwapBottom_High);
        public static bool IsSwapTop(int toyCode) => toyCode.RangeInclusive(SwapTop_Low, SwapTop_High);
        public static bool IsSwapPart(int toyCode) => IsSwapBottom(toyCode) || IsSwapTop(toyCode);
        public static bool IsSwapForce(int toyCode) => toyCode.RangeInclusive(SwapBottom_Low, SwapForce_High)
                                                    && !IsVV_Item(toyCode)
                                                    && !IsVV_Expansion(toyCode)
                                                    && !IsSuperChargers(toyCode)
                                                    && !IsVillainDriver(toyCode)
                                                    && !IsVillainVehicle(toyCode)
                                                    && !IsRacingPack(toyCode)
                                                    && toyCode != (int)ToyCode.Character_Nightfall_ERROR
                                                    || toyCode == SwapForce_High;

        public static bool IsSwapForce_Exclusive(int toyCode) => toyCode.RangeInclusive(SwapForce_Low, SwapForce_High) && !IsVV_Item(toyCode) && !IsVV_Expansion(toyCode);

        public static bool IsVV_Item(int toyCode) => toyCode.RangeInclusive(VV_Items_Low, VV_Items_High);
        public static bool IsVV_Item_Exclusive(int toyCode) => IsVV_Item(toyCode) && !IsVehicle(toyCode);
        public static bool IsVehicle(int toyCode) => toyCode.RangeInclusive(Vehicles_Low, VV_Items_High);

        public static bool IsVV_Expansion(int toyCode) => toyCode.RangeInclusive(VV_Expansions_Low, VV_Expansions_High);
        public static bool IsVV_Expansion_Exclusive(int toyCode) => IsVV_Expansion(toyCode) && !IsVV_BattlePiece(toyCode);
        public static bool IsVV_BattlePiece(int toyCode) => toyCode.RangeInclusive(VV_BattlePieces_Low, VV_Expansions_High);

        public static bool IsSuperChargers(int toyCode) => toyCode.RangeInclusive(SuperChargers_Low, SuperChargers_High);

        public static bool IsVillainDriver(int toyCode) => toyCode.RangeInclusive(VillainDrivers_Low, VillainDrivers_High);
        public static bool IsVillainVehicle(int toyCode) => toyCode.RangeInclusive(VillainVehicles_Low, VillainVehicles_High);

        public static bool IsRacingPack(int toyCode) => toyCode.RangeInclusive(RacingPack_Low, RacingPack_High) && toyCode != (int)ToyCode.Character_Nightfall_ERROR;

        public static bool IsSky_2016(int toyCode) => toyCode.RangeInclusive(Sky_2016_Low, Sky_2016_High);

        public static bool IsTemplateVehicle(int toyCode) => toyCode.RangeInclusive(TemplateVehicle_Low, TemplateVehicle_High) && toyCode.ToString().Contains("999");

        public static bool IsTrapTeam_Debug(int toyCode) => toyCode.RangeInclusive(TrapTeam_Debug_Low, TrapTeam_Debug_High);
    }
}