using UnityEditor;
using UnityEngine;
using System.IO;

namespace MapConstructor
{
    public class SaveMapEditorData : EditorWindow
    {
        private string globalPath;
        private const string filePathLevelsMap = "json/gamedesign/levels_map.json";
        private const string filePathAnimsMap = "json/gamedesign/map_anims.json";

        [MenuItem("Vegamix/Save Map Locations")]
        public static void ShowWindow()
        {
            GetWindow(typeof(SaveMapEditorData));
        }

        private void OnGUI()
        {
            GUILayout.Label("Base Settings", EditorStyles.boldLabel);

            if (GUILayout.Button("Save Map Locations"))
            {
                globalPath = Application.dataPath + "/Resources/Server/bin/";
                SaveMapEditorItems();
            }

            //groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
            //myBool = EditorGUILayout.Toggle("Toggle", myBool);
            //myFloat = EditorGUILayout.Slider("Slider", myFloat, -3, 3);
            //EditorGUILayout.EndToggleGroup();
        }

        private void SaveMapEditorItems()
        {
            SaveMapEditorLocations.Run(Path.Combine(globalPath, filePathLevelsMap));
            SaveMapEditorAnims.Run(Path.Combine(globalPath, filePathAnimsMap));
        }


        ////IEnumerator CreatePost()
        ////{
        ////    Dictionary<string, string> headers = new Dictionary<string, string>();
        ////    headers.Add("Content-Type", "application/json");

        ////    byte[] postData = Encoding.ASCII.GetBytes(JsonUtility.ToJson(post));

        ////    WWW www = new WWW(API.baseURL + "/posts", postData, headers);

        ////    yield return www;

        ////    Debug.Log("Created a Post: " + www.text);
        ////    post = JsonUtility.FromJson<Post>(www.text);
        ////}
    }
}
