using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Localization.Settings;
using sVersion = System.Version;

namespace Project.Applikation.AndroidBuilder
{
#if UNITY_EDITOR
    [Serializable]
    public class SceneMarkPair
    {
        public bool use = true;
        public SceneAsset scene;
    }
#endif

    [ExecuteInEditMode] // TODO: remove
    //TODO: deal with #if UNITY_EDITOR
    [CreateAssetMenu(fileName = "AndroidBuilderData", menuName = "Project/AndroidBuilderData")]
    public class AndroidBuilderData : ScriptableObject
    {
        public string Version => _version;
        public int Counter
        {
            get
            {
                if (_day != DateTime.Now.Day) { _counter = 1; }

                return _counter;
            }
        }        

        [ReadOnly] [SerializeField] private string _version;
        [HideInInspector] [SerializeField] private int _day = 0;
        [HideInInspector] [SerializeField] private int _counter = 1;
        public string BuildDirection = "Builds/Android/";
        public string KeystorePass;
        public string KeyaliasPass;
        public AndroidBuilderGameConfig preset;

#if UNITY_EDITOR
       public List<SceneMarkPair> scenes;
#endif

        //--------------------------------------------------------------

        public void Upgrade()
        {
            _version = GetBuildVersion();
            _day = DateTime.Now.Day;
            _counter++;

#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
#endif
        }

        public string GetBuildVersion()
        {
#if UNITY_EDITOR
            var date = System.DateTime.Now.ToString("MMdd");
            var counter = string.Format("{0:00}", Counter);

            return $"{PlayerSettings.productName}_{date}_{counter}";
#else
            return "";
#endif
        }

        public string CreateBundleVersion(string currentVersion)
        {
            sVersion version;
            if (sVersion.TryParse(currentVersion, out sVersion result))
            {
                version = new sVersion(result.Major, result.Minor, result.Build + 1);
            }
            else
            {
                version = new sVersion(0, 1, 1);
            }

            return version.ToString();
        }

        public string[] GetScenes()
        {
#if UNITY_EDITOR
            return scenes.Where(s => s.scene != null && s.use).Select(s => AssetDatabase.GetAssetPath(s.scene)).ToArray();
#else
            return new string[0];
#endif
        }

        public void UsePassword()
        {
#if UNITY_EDITOR
            PlayerSettings.Android.keystorePass = KeystorePass;
            //PlayerSettings.Android.keyaliasName = "ALIAS_NAME";
            PlayerSettings.Android.keyaliasPass = KeyaliasPass;
#endif
        }

        public AndroidBuilderBuildConfig GetBuildConfig()
        {
            return preset.buildConfig;
        }

        public void ApplyPreset()
        {
            Game.Config.App.Map.showFog = preset.showFog;
            Game.Config.App.Locale = preset.locale;

            var rewardsGS = Game.Config.App.GoogleSheets.Sheets.Find(s => s.type == Data.Google.GoogleSheet.Rewards);
            if (rewardsGS != null)
            {
                rewardsGS.name = preset.rewardsGoogleSheetName;
            }

            var settingsGS = Game.Config.App.GoogleSheets.Sheets.Find(s => s.type == Data.Google.GoogleSheet.Settings);
            if (settingsGS != null)
            {
                settingsGS.name = preset.settingsGoogleSheetName;
            }
        }
    }
}