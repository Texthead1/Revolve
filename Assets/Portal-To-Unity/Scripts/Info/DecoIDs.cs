namespace PortalToUnity
{
#if UNITY_EDITOR
    [MarkAsDecoID]
#endif
    public enum DecoID
    {
        Normal = 0,
        Repose1 = 1,
        AlternativeDeco = 2,
        Legendary = 3,
        Event = 4,
        Repose2 = 5,
        LightDirect = 6,
        LightStored = 7,
        LightEnhanced = 8,
        Repose3 = 9,
        Repose4 = 10,
        Repose5 = 11,
        Valentines = 12,
        Easter = 13,
        Winter = 14,
        Virtual = 15,
        Premium = 16,
        GlowDark = 17,
        StoneStatue = 18,
        GlitterSpectrum = 19,
        TreasureHunt2012 = 20,
        Halloween = 21,
        TreasureHunt2013 = 22,
        ColorShift = 23,
        WiiU = 24,
        TH_BestBuy = 25,
        TH_FritoLay0 = 26,
        TH_FritoLay1 = 27,
        TH_FritoLay2 = 28,
        TreasureHunt2014 = 29,
        TreasureHunt2015 = 30,
        Mobile = 31,
        Autism2016 = 32,
        TreasureHunt2016 = 33
    }

#if UNITY_EDITOR
    [MarkAsDecoID]
#endif
    public enum TrapDecoID
    {
        Tiki = 1,
        LogHolder = 2,
        Toucan = 3,
        Orb = 4,
        Torch = 5,
        Jughead = 6,
        Angel = 7,
        Skull = 8,
        Scepter = 9,
        Hammer = 10,
        Axe = 11,
        Hand = 12,
        Hourglass = 14,
        Hawk = 15,
        Snake = 16,
        Screamer = 17,
        Totem = 18,
        Spider = 20,
        Rocket = 21,
        FlyingHelmet = 22,
        CaptainHat = 23,
        Sword = 24,
        Handstand = 26,
        Yawn = 27,
        Kaos = 30,
        UltimateKaos = 31
    }

#if UNITY_EDITOR
    [MarkAsDecoID]
#endif
    public enum ChestDecoID
    {
        Bronze = 1,
        Silver = 2,
        Gold = 3,
        // MTX in this context meaning microtransactions, as these chests correspond to those you purchase externally
        MTX_Small = 4,
        MTX_Medium = 5,
        MTX_Large = 6,
        MTX_MediumSmall = 7,
        MTX_MediumMedium = 8,
        MTX_MediumLarge = 9,
        RailGrindChallenge = 10,
        EggRescue = 11,
        TrollRadio = 12,
        DoomlanderFight = 13,
        LevelUp = 14,
        LockPuzzle = 15,
        LevelStar = 16,
        // Unknown meaning
        CMR = 17,
        Emblem = 18,
        SkyStones = 19,
        SenseiShrine = 20,
        KaosDefeated = 21,
        KaosDefeatedNightmare = 22,
        CursedTikiTemple = 23,
        LostImaginiteMines = 25
    }

#if UNITY_EDITOR
    [MarkAsDecoID]
#endif
    public enum CrystalDecoID
    {
        /// <summary>
        /// UNRELEASED. An "Angel" Creation Crystal shell encasing a brilliant cut decagon
        /// </summary>
        Crystal_8j_Fire = 1,
        /// <summary>
        /// An "Angel" Creation Crystal shell encasing an anhedral corundum
        /// </summary>
        Crystal_8k_Air = 2,
        /// <summary>
        /// UNRELEASED. An "Angel" Creation Crystal shell encasing a truncated tetrahedron
        /// </summary>
        Crystal_8a_Light = 3,
        /// <summary>
        /// A "Pyramid" Creation Crystal shell encasing a faceted ovoid
        /// </summary>
        Crystal_5h_Magic = 4,
        /// <summary>
        /// UNRELEASED. A "Pyramid" Creation Crystal shell encasing a truncated tetrahedron
        /// </summary>
        Crystal_5a_Tech = 5,
        /// <summary>
        /// A "Pyramid" Creation Crystal shell encasing a hexahedron
        /// </summary>
        Crystal_5e_Dark = 6,
        /// <summary>
        /// A "Lantern" Creation Crystal shell encasing a dodecahedron
        /// </summary>
        Crystal_4b_Air = 7,
        /// <summary>
        /// A "Lantern" Creation Crystal shell encasing a hexahedron
        /// </summary>
        Crystal_4e_Magic = 8,
        /// <summary>
        /// A "Lantern" Creation Crystal shell encasing an elongated hexagonal bipyramid
        /// </summary>
        Crystal_4c_Undead = 9,
        /// <summary>
        /// A "Rune" Creation Crystal shell encasing a truncated tetrahedron
        /// </summary>
        Crystal_9a_Dark = 10,
        /// <summary>
        /// A "Rune" Creation Crystal shell encasing an elongated hexagonal bipyramid
        /// </summary>
        Crystal_9c_Light = 11,
        /// <summary>
        /// UNRELEASED. A "Rune" Creation Crystal shell encasing a dodecahedron
        /// </summary>
        Crystal_9b_Earth = 12,
        /// <summary>
        /// A "Reactor" Creation Crystal shell encasing an elongated hexagonal bipyramid
        /// </summary>
        Crystal_7c_Tech = 13,
        /// <summary>
        /// A "Reactor" Creation Crystal shell encasing a dodecahedron
        /// </summary>
        Crystal_7b_Dark = 14,
        /// <summary>
        /// A "Reactor" Creation Crystal shell encasing an icosahedron
        /// </summary>
        Crystal_7d_Fire = 15,
        /// <summary>
        /// An "Acorn" Creation Crystal shell encasing an icosahedron
        /// </summary>
        Crystal_6d_Life = 16,
        /// <summary>
        /// An "Acorn" Creation Crystal shell encasing a truncated icosahedron
        /// </summary>
        Crystal_6g_Fire = 17,
        /// <summary>
        /// UNRELEASED. An "Acorn" Creation Crystal shell encasing a hexahedron with elongated hexagonal pyramids
        /// </summary>
        Crystal_6f_Air = 18,
        /// <summary>
        /// An "Armor" Creation Crystal shell encasing an anhedral corundum
        /// </summary>
        Crystal_3k_Earth = 19,
        /// <summary>
        /// An "Armor" Creation Crystal shell encasing a hexahedron with elongated hexagonal pyramids
        /// </summary>
        Crystal_3f_Water = 20,
        /// <summary>
        /// An "Armor" Creation Crystal shell encasing a truncated icosahedron
        /// </summary>
        Crystal_3g_Tech = 21,
        /// <summary>
        /// A "Fanged" Creation Crystal shell encasing a hexahedron
        /// </summary>
        Crystal_2e_Light = 22,
        /// <summary>
        /// A "Fanged" Creation Crystal shell encasing an icosahedron
        /// </summary>
        Crystal_2d_Undead = 23,
        /// <summary>
        /// A "Fanged" Creation Crystal shell encasing a faceted ovoid
        /// </summary>
        Crystal_2h_Water = 24,
        /// <summary>
        /// A "Claw" Creation Crystal shell encasing a hexahedron with elongated hexagonal pyramids
        /// </summary>
        Crystal_lf_Undead = 25,
        /// <summary>
        /// A "Claw" Creation Crystal shell encasing a faceted ovoid
        /// </summary>
        Crystal_lh_Life = 26,
        /// <summary>
        /// A "Claw" Creation Crystal shell encaing a brilliant cut decagon
        /// </summary>
        Crystal_lj_Magic = 27,
        /// <summary>
        /// A "Rocket" Creation Crystal shell encasing a truncated icosahedron
        /// </summary>
        Crystal_0g_Water = 28,
        /// <summary>
        /// A "Rocket" Creation Crystal shell encasing a brilliant cut decagon
        /// </summary>
        Crystal_0j_Earth = 29,
        /// <summary>
        /// A "Rocket" Creation Crystal shell encasing an anhedral corundum
        /// </summary>
        Crystal_0k_Life = 30,
        /// <summary>
        /// A "Rune" Creation Crystal shell encasing a hexahedron
        /// </summary>
        Crystal_9e_Life = 31,
        Kaos = 32

        /*
        Shell names according to the Skylanders Character List naming standard
        0 - Rocket
        l (yes this is an l (L) not a 1 (one)) - Claw
        2 - Fanged
        3 - Armor
        4 - Lantern
        5 - Pyramid
        6 - Acorn
        7 - Reactor
        8 - Angel
        9 - Rune

        a - Truncated tetrahedron
        b - Dodecahedron
        c - Elongated hexagonal bipyramid
        d - Icosahedron
        e - Hexahedron
        f - Hexahedron with elongated hexagonal pyramids
        g - Truncated icosahedron
        h - Faceted ovoid
        i - UNUSED
        j - Brilliant cut decagon
        k - Anhedral corundum
        */
    }

#if UNITY_EDITOR
    [MarkAsDecoID]
#endif
    public enum CYOSDecoID
    {
        CYOS_Card_Normal = 1,
        CYOS_Card_Foil = 2,
        CYOS_Card_Special = 3,
        CYOS_3DPrint_Normal = 4,
        CYOS_3DPrint_Medium = 5,
        CYOS_3DPrint_Large = 6,
        CYOS_3DPrint_Unique = 7
    }
}