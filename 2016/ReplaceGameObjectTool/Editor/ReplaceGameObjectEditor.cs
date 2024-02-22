using UnityEngine;
using UnityEditor;

namespace ReplaceGameObjectTool
{
    [CustomEditor(typeof(ReplaceGameObject))]
    public class ReplaceGOEditor : Editor
    {
        static Color _redColor = new Color(1f, 0.25f, 0.25f);
        static Color _greenColor = new Color(0.25f, 1f, 0.25f);

        ReplaceGameObject t;
        SerializedObject GetTarget;
        SerializedProperty ThisList;
        int ListSize;

        static GUIStyle boxScopeStyle;
        public static GUIStyle BoxScopeStyle
        {
            get
            {
                if (boxScopeStyle == null)
                {
                    boxScopeStyle = new GUIStyle(EditorStyles.helpBox);
                    var p = boxScopeStyle.padding;
                    p.right += 6;
                    p.top += 1;
                    p.left += 3;
                }

                return boxScopeStyle;
            }
        }

        static GUIStyle verticalCenterStyle;
        public static GUIStyle VerticalCenterStyle
        {
            get
            {
                if (verticalCenterStyle == null)
                {
                    verticalCenterStyle = new GUIStyle();
                    verticalCenterStyle.alignment = TextAnchor.MiddleCenter;
                }

                return verticalCenterStyle;
            }
        }

        void OnEnable()
        {
            t = (ReplaceGameObject)target;
            GetTarget = new SerializedObject(t);
            ThisList = GetTarget.FindProperty("MatchedPrefabsList"); // Find the MatchedPrefabsList in our script and create a refrence of it
        }

        public override void OnInspectorGUI()
        {
            GetTarget.Update();

            for (int i = 0; i < ThisList.arraySize; i++)
            {
                SerializedProperty MyListRef = ThisList.GetArrayElementAtIndex(i);
                SerializedProperty _newPrefab = MyListRef.FindPropertyRelative("newPrefab");
                SerializedProperty _oldPrefab = MyListRef.FindPropertyRelative("oldPrefab");

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField((i + 1).ToString(), VerticalCenterStyle, GUILayout.Width(20f), GUILayout.Height(60));

                EditorGUILayout.BeginVertical(BoxScopeStyle);
                EditorGUILayout.LabelField(_oldPrefab.objectReferenceValue == null ? "None" : _oldPrefab.objectReferenceValue.name, EditorStyles.boldLabel/*, GUILayout.Width(_labelFieldWidth)*/);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("NEW", GUILayout.Width(30f));
                _newPrefab.objectReferenceValue = EditorGUILayout.ObjectField(_newPrefab.objectReferenceValue, typeof(GameObject), true);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("old", GUILayout.Width(30f));
                _oldPrefab.objectReferenceValue = EditorGUILayout.ObjectField(_oldPrefab.objectReferenceValue, typeof(GameObject), true);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();

                Color _GUIColor1 = GUI.color;
                GUI.color = _redColor;
                if (GUILayout.Button("-", EditorStyles.miniButton, GUILayout.Width(25)))
                {
                    ThisList.DeleteArrayElementAtIndex(i);
                }
                GUI.color = _GUIColor1;

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();
            }

            EditorGUILayout.BeginHorizontal();
            Color _GUIColor = GUI.color;

            GUI.color = _greenColor;
            if (GUILayout.Button("Add New"))
            {
                t.MatchedPrefabsList.Add(new MatchedPrefabs());
            }
            GUI.color = _GUIColor;


            _GUIColor = GUI.color;
            GUI.color = Color.cyan;

            if (GUILayout.Button("Replace gameobjects"))
            {
                t.ReplaceObjects();
            }
            EditorGUILayout.EndHorizontal();
            GUI.color = _GUIColor;

            EditorGUILayout.HelpBox("Match the two prefabs of gameobject to replace with the NEW one's instance.", MessageType.Info);
            EditorGUILayout.HelpBox("WARNING! Before the replacement make sure you made a backup of the main blocks prefabs just in case.", MessageType.Warning);

            GetTarget.ApplyModifiedProperties();
        }
    }
}