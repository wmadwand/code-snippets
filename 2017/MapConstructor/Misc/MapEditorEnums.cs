namespace MapConstructor
{
    public enum MapLevelType
    {
        Circle = 10,
        Arrow = 20,
        Palm = 30,
        Bonus = 40
    }

    public enum MapLevelState
    {
        Blocked = 10,
        Available = 20,
        Passed = 30,
        Hidden = 40
    }

    public enum MapBonusLevelType { Basic, Chest }

    public enum BirdType
    {
        Seagull = 10,
        Swallow = 20,
        Parrot = 30
    }

    public enum SpecialType { Basic, MapAnimXFrames }
}