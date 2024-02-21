using System;
using UnityEditor;
using UnityEngine;

namespace Project.Applikation.AndroidBuilder
{
    public class AndroidBuilderWindow : EditorWindow
    {
        private int _selGridInt = 0;
        private string[] _selStrings;
        private AndroidBuilderData _data;
        private GUIStyle _webButtonStyle;
        protected readonly float ButtonHeight = 30f;
        private GUIStyle _redLabel;

        public AndroidBuilderGameConfig source;
        private static AndroidBuilderData data;
        private const string path = "Assets/_Project/Data/Settings/Resources/AndroidBuilderData.asset";

        //--------------------------------------------------------------    

        [MenuItem("Project/Android Builder")]
        public static void Init()
        {
            AndroidBuilderWindow window = (AndroidBuilderWindow)GetWindow(typeof(AndroidBuilderWindow));
            window.titleContent.text = "Android Builder";
            window.Show();
        }

        //--------------------------------------------------------------    

        [InitializeOnLoadMethod]
        private static void OnLoad()
        {
            if (!data)
            {
                data = AssetDatabase.LoadAssetAtPath<AndroidBuilderData>(path);
                if (data) return;

                data = CreateInstance<AndroidBuilderData>();
                AssetDatabase.CreateAsset(data, path);
                AssetDatabase.Refresh();
            }
        }

        private void OnEnable()
        {
            _data = Resources.Load<AndroidBuilderData>("AndroidBuilderData");
            CreateBuildTypeList();
        }

        private void OnGUI()
        {
            LoadAssets();

            var serializedObject = new SerializedObject(data);
            serializedObject.Update();

            var preset = serializedObject.FindProperty("preset");
            var buildDirection = serializedObject.FindProperty("BuildDirection");
            var keystorePass = serializedObject.FindProperty("KeystorePass");
            var keyaliasPass = serializedObject.FindProperty("KeyaliasPass");
            var scenes = serializedObject.FindProperty("scenes");


            Vertical(() =>
            {
                GUILayout.Label("Build type", EditorStyles.boldLabel);
                Horizontal(() =>
                {
                    _selGridInt = GUILayout.SelectionGrid(_selGridInt, _selStrings, 2, _webButtonStyle);
                });
            }, true);

            Vertical(() =>
            {
                GUILayout.Label("Scenes", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(scenes);

            }, true);

            Vertical(() =>
            {
                GUILayout.Label("Build Direction", EditorStyles.boldLabel);

                Vertical(() =>
                {
                    EditorGUILayout.PropertyField(buildDirection);
                });

            }, true);

            Vertical(() =>
            {
                GUILayout.Label("Passwords", EditorStyles.boldLabel);

                Vertical(() =>
                {
                    EditorGUILayout.PropertyField(keystorePass);
                    EditorGUILayout.PropertyField(keyaliasPass);
                });

            }, true);

            Vertical(() =>
            {
                GUILayout.Label("Current version", EditorStyles.boldLabel);
                DisabledGroup(() =>
                {
                    Vertical(() =>
                    {
                        EditorGUILayout.TextField("Bundle:", PlayerSettings.bundleVersion);
                        EditorGUILayout.TextField("Build:", _data.Version);
                    });
                });
            }, true);

            Vertical(() =>
            {
                GUILayout.Label("Preset", EditorStyles.boldLabel);
                Vertical(() =>
                {
                    EditorGUILayout.PropertyField(preset);
                });
            }, true);


#if (Project_DEBUG)
    EditorGUILayout.LabelField("#Project_DEBUG", _redLabel);
#endif

            if (GUILayout.Button("Build", _webButtonStyle))
            {
                var ext = (AndroidBuildExtension)_selGridInt;
                AndroidBuilder.Build(ext);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void LoadAssets()
        {
            if (_webButtonStyle == null)
            {
                _webButtonStyle = new GUIStyle(GUI.skin.button)
                {
                    fontSize = 12,                    
                    fixedHeight = ButtonHeight,
                    padding = new RectOffset(5, 5, 5, 5)
                };
            }

            if (_redLabel == null)
            {
                _redLabel = new GUIStyle(EditorStyles.label);
                _redLabel.normal.textColor = Color.red;
            }
        }

        private void CreateBuildTypeList()
        {
            var extensions = (AndroidBuildExtension[])Enum.GetValues(typeof(AndroidBuildExtension));
            _selStrings = new string[extensions.Length];
            for (int i = 0; i < extensions.Length; i++)
            {
                _selStrings[i] = extensions[i].ToString();
            }
        }

        #region Horizontal and Vertical Layouts

        protected void Vertical(Action content, bool isBox = false)
        {
            EditorGUILayout.BeginVertical(isBox ? "Box" : GUIStyle.none);
            content?.Invoke();
            EditorGUILayout.EndVertical();
        }

        protected void Vertical(Action content, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginVertical(options);
            content?.Invoke();
            EditorGUILayout.EndVertical();
        }

        protected void Horizontal(Action content, bool isBox = false)
        {
            EditorGUILayout.BeginHorizontal(isBox ? "Box" : GUIStyle.none);
            content?.Invoke();
            EditorGUILayout.EndHorizontal();
        }

        protected void Horizontal(Action content, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal(options);
            content?.Invoke();
            EditorGUILayout.EndHorizontal();
        }

        protected void DisabledGroup(Action content)
        {
            EditorGUI.BeginDisabledGroup(true);
            content?.Invoke();
            EditorGUI.EndDisabledGroup();
        }

        #endregion
    }
}