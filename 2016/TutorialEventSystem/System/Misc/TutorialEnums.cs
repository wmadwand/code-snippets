namespace TutorialEventSystem
{
    public enum SystemMessage
    {
        None = 100,
        RoundFinish = 200,
        RevealMap = 300,
        NotEnoughPoints = 400,
        GotEnoughPoints = 500,

        ImproveAirSpirit = 600,
        OpenSpellChest = 800,
        AddEarthSpirit = 900,

        AnomalySpellCollect = 1000,
        AnomalyClose = 1100,
        AnomalyRecover = 1200,
        HasNotUsedSpirit = 1300,
        ZeroDamageWeapon = 1400,

        RevealMapCell0102 = 1500,
        RevealMapCell0103 = 1600,
        RevealMapCell0104 = 1700,
        RevealMapCell0105 = 1800,
        RevealMapCell0115 = 1900,

        Greetings = 2000,
        NowJump = 2050,
        FirstStep = 2100,
        NowGetWeapon = 2200,
        NowUseWeapon = 2300,
        NowBecomeSpirit = 2400,
        NowUseAbility = 2500,
        YouAreDone = 2600,
        NowAutoAttack = 2700
    }

    public enum Chapter
    {
        None = 100,
        SpiritWorld = 200,
        FirstRun = 300,
        RevealMap = 400,
        ImproveAirSpirit = 500,
        SpellChest = 600,
        CrystalEther = 700,
        Anomaly = 800
    }

    public enum PageType { Executable, Slave }

    public enum PlaceToShow { Map, Level }

    public enum InfoType { Text, Image }

    public enum ButtonType { Finger, Cover }

    public enum TooltipPosition { LeftBottom, CenterBottom, RightBottom }

    public enum GameButton
    {
        None = 100,
        TwoFingersTap = 150,

        Bonus = 200,
        PlayGame = 300,
        SpiritMenu = 400,

        TweakAirSpirit = 500,
        ChooseAirSpirit = 505,

        TweakEarthSpirit = 510,
        ChooseEarthSpirit = 515,

        SpiritPower = 520,
        SpiritWill = 530,
        SpellSection = 540,

        UseWeapon = 600,
        UseAbility = 700,
        Jump = 750,

        MapCell0102 = 800,
        MapCell0103 = 900,
        MapCell0104 = 1000,
        OpenSpellChest = 1100,
        CollectEther = 1200,
        CollectSpell01 = 1300,

        Test1 = 10000,
        Test2 = 11000
    }
}