using GoShared;
using Project.Applikation.AndroidBuilder;
using Project.Data.Google;
using Project.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace Project.Data
{
    [Serializable]
    public class UIData
    {
        public TokenPopup tokenPopup;
        public TokenCompletedPopup tokenCompletedPopup;
        public TreasureScreen treasureScreen;
        public ProfileScreen profileScreen;
        public TokenCollectedPopup tokenCollectedPopup;
        public GetRewardPopup getRewardPopup;
        public RewardUnlockedPopup rewardUnlockedPopup;
        public LoadingScreen loadingScreen;
        public ErrorPopup errorPopup;
    }

    [CreateAssetMenu(fileName = "GameConfig", menuName = "Project/GameConfig")]
    public class GameConfig : ScriptableObject
    {
        public MetaConfig Meta;
        public AppConfig App;

        [Serializable]
        //TODO: rework this class
        public class MetaConfig
        {
            [NonSerialized] private GOFeatureKind[] _tokenMapArea = null;
            [NonSerialized] private int _groupN = 0;
            [NonSerialized] private Dictionary<string, List<string>> _settingsDictionary = new Dictionary<string, List<string>>();

            //--------------------------------------------------------------

            public void Init(object initSettings)
            {
                _settingsDictionary = initSettings as Dictionary<string, List<string>>;
            }

            //--------------------------------------------------------------

            private string GetValue(string key, string defaultValue)
            {
                if (_settingsDictionary.ContainsKey(key))
                {
                    if (_settingsDictionary[key].Count > _groupN)
                        return _settingsDictionary[key][_groupN];
                }

                return defaultValue;
            }

            public UIData UI;
        }

        [Serializable]
        public class AppConfig
        {
            public GoogleSheets GoogleSheets;
            public AndroidBuilderData AndroidBuilder;
            public Locale Locale;
            public GameLoaderScenes Scenes;
            public MapSettings Map;
        }

        [Serializable]
        public class GameLoaderScenes
        {
            public string login = "PromptEmail";
            public string loading = "Loading";
            public string map = "Map";
            public string avatar = "Avatar";
            public string UI = "UIMain";
        }

        [Serializable]
        public class MapSettings
        {
            public int PlacePointAttemptsCount = 100;
            public List<GOFeatureKind> AvoidMapAreas;
            public bool showFog = true;
        }
    }
}