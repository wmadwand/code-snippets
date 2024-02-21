using Cysharp.Threading.Tasks;
using Project.Analytics;
using Project.Data;
using Project.Data.Google;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class Game
{
    private static readonly CacheData Cache = new CacheData();

    public class PlayerProfile
    {
        //TODO: UserCredentialsLocalStorage, TreasureLocalStorage, AvatarDefaultSettings
    }

    private class CacheData
    {
        public PlayerProfile PlayerProfile;
        public GameConfig GameConfig;
        public bool IsInited = false;
    }

    public static bool IsInited => Cache.IsInited;

    public static GameConfig Config
    {
        get
        {
            if (!Cache.GameConfig)
            {
                Cache.GameConfig = Resources.Load<GameConfig>("GameConfig");
            }
            return Cache.GameConfig;
        }
    }

    public static async UniTask Init()
    {
        if (Cache.IsInited)
        {
            Debug.LogError("Cache is already inited");
            return;
        }

        if (await LoadGoogleSheets() == false) { return; }

        var settings = Config.App.GoogleSheets.SheetsData[GoogleSheet.Settings];
        Config.Meta.Init(settings);
        Vibration.Init();

        PlayerPrefs.SetString("selected-locale", Config.App.Locale.Identifier.Code);
        await UniTask.NextFrame();
        await LocalizationSettings.InitializationOperation.ToUniTask();
        await UniTask.WaitUntil(() => LocalizationSettings.InitializationOperation.IsDone);
        await LocalizationSettings.StringDatabase.GetTableAsync("StringTableCollection01");
        

        Cache.IsInited = true;

        if (!Cache.IsInited)
        {
            Debug.LogError("Failed to load save. New save will be created");

            // reset game state and player profile and restart game
        }
    }

    private static async UniTask<bool> LoadGoogleSheets()
    {
        var errorStr = await Config.App.GoogleSheets.Load();

        if (!string.IsNullOrEmpty(errorStr))
        {
            Debug.LogError($"GoogleSheets Error! {errorStr}");
            return false;
        }

        return true;
    }
}