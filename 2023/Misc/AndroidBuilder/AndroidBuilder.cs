using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Project.Applikation.AndroidBuilder
{
    public enum AndroidBuildExtension
    {
        APK = 0,
        AAB = 1
    }

    public class AndroidBuilder : Editor, IPreprocessBuildWithReport
    {
        int IOrderedCallback.callbackOrder => 1;

        //--------------------------------------------------------------

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            BuildPlayerWindow.RegisterBuildPlayerHandler(BuildPlayer);
        }

        public static void Build(AndroidBuildExtension extension)
        {
            BuildPlayerOptions settings = default;
            switch (extension)
            {
                case AndroidBuildExtension.APK: settings = GetSettings(AndroidBuildExtension.APK); break;
                case AndroidBuildExtension.AAB: settings = GetSettings(AndroidBuildExtension.AAB); break;
                default: { Debug.LogError($"AndroidBuildExtension ({extension}) is not found"); } break;
            }

            BuildPlayer(settings);
        }

        public static void TestApplySettings()
        {
            var data = Resources.Load<AndroidBuilderData>("AndroidBuilderData");
            data.ApplyPreset();
        }

        //--------------------------------------------------------------

        void IPreprocessBuildWithReport.OnPreprocessBuild(BuildReport report)
        {
            PlayerSettings.Android.bundleVersionCode = System.DateTime.Now.ToUnixTimestamp();
        }

        private static void BuildPlayer(BuildPlayerOptions settings)
        {
            // Localization
            // https://gist.github.com/favoyang/cd2cf2ed9df7e2538f3630610c604c51
            AddressableAssetSettings.CleanPlayerContent();
            AddressableAssetSettings.BuildPlayerContent();

            var data = Resources.Load<AndroidBuilderData>("AndroidBuilderData");
            data.Upgrade();

            var report = BuildPipeline.BuildPlayer(settings);
            var summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
            }
            else if (summary.result == BuildResult.Failed)
            {
                Debug.LogError($"Build failed");
            }
        }

        private static BuildPlayerOptions GetSettings(AndroidBuildExtension extension)
        {
            var data = Resources.Load<AndroidBuilderData>("AndroidBuilderData");
            var version = data.GetBuildVersion();
            PlayerSettings.bundleVersion = data.CreateBundleVersion(PlayerSettings.bundleVersion);
            data.UsePassword();

            data.ApplyPreset();
            var buildConfig = data.GetBuildConfig();

            BuildPlayerOptions options = default;
            if (extension == AndroidBuildExtension.APK)
            {
                PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, buildConfig.scriptingBackend);
                PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7;
                EditorUserBuildSettings.buildAppBundle = false;

                options = new BuildPlayerOptions
                {
                    scenes = data.GetScenes(),
                    locationPathName = $"{data.BuildDirection}{version}.apk",
                    target = BuildTarget.Android,
                    options = buildConfig.options
                    //,options = BuildOptions.Development | BuildOptions.AllowDebugging /*| BuildOptions.ConnectWithProfiler | BuildOptions.EnableDeepProfilingSupport*/
                };
            }
            else if (extension == AndroidBuildExtension.AAB)
            {
                PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
                PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7 | AndroidArchitecture.ARM64;
                EditorUserBuildSettings.buildAppBundle = true;

                options = new BuildPlayerOptions
                {
                    scenes = data.GetScenes(),
                    locationPathName = $"{data.BuildDirection}{version}.aab",
                    target = BuildTarget.Android,
                    options = buildConfig.options
                };
            }

            return options;
        }
    }
}