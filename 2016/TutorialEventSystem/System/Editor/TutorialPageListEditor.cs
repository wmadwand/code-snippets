using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

namespace TutorialEventSystem
{
    [CustomEditor(typeof(TutorialPageList))]
    public class TutorialPageListEditor : Editor
    {
        TutorialPageList _target;
        ReorderableList newPrefabsList_R;

        private void OnEnable()
        {
            _target = (TutorialPageList)target;
            newPrefabsList_R = new ReorderableList(serializedObject, serializedObject.FindProperty("pageLinkList"), true, true, true, true);

            newPrefabsList_R.drawHeaderCallback += DrawHeader;
            newPrefabsList_R.drawElementCallback += DrawElement;

            newPrefabsList_R.onAddCallback += AddItem;
            newPrefabsList_R.onRemoveCallback += RemoveItem;
        }

        private void OnDisable()
        {
            newPrefabsList_R.drawHeaderCallback -= DrawHeader;
            newPrefabsList_R.drawElementCallback -= DrawElement;

            newPrefabsList_R.onAddCallback -= AddItem;
            newPrefabsList_R.onRemoveCallback -= RemoveItem;
        }

        public override void OnInspectorGUI()
        {
            DrawReorderableList();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(_target);
            }
        }

        private void DrawResetProgessButton()
        {
            if (GUILayout.Button("Reset the progress of all the TutorialPages"))
            {
                if (EditorUtility.DisplayDialog("Confirmation", "Are you sure in your intentions?", "Ok", "Cancel"))
                {
                    if (PlayerPrefs.HasKey(TutorialExtensions.tPageIdToRecoverAfterCrash))
                    {
                        PlayerPrefs.DeleteKey(TutorialExtensions.tPageIdToRecoverAfterCrash);
                    }

                    Debug.Log("Reset the progress of all the TutorialPages -- DONE");
                }
            }
        }

        private void DrawReorderableList()
        {
            DrawResetProgessButton();

            EditorGUILayout.HelpBox("The order of the pages is not important.", MessageType.Info);
            //EditorGUILayout.Space();
            //_target.delayBetweenPages = EditorGUILayout.FloatField("Delay Between Pages", _target.delayBetweenPages);
            //EditorGUILayout.Space();

            serializedObject.Update();
            newPrefabsList_R.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawHeader(Rect rect)
        {
            GUI.Label(rect, "Tutorial Page List");
        }

        private void DrawElement(Rect _rect, int _index, bool active, bool focused)
        {
            EditorGUI.BeginChangeCheck();

            newPrefabsList_R.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                rect.y += 2;
                var element = newPrefabsList_R.serializedProperty.GetArrayElementAtIndex(index);
                TutorialPage el = _target.pageLinkList[index].page;


                GUIStyle normalItemStyle = new GUIStyle(EditorStyles.label);
                normalItemStyle.normal.textColor = Color.black;

                GUIStyle badItemStyle = new GUIStyle(EditorStyles.label);
                badItemStyle.normal.textColor = Color.red;

                GUIStyle goodItemStyle = new GUIStyle(EditorStyles.label);
                goodItemStyle.normal.textColor = Color.green;

                GUIStyle _currItemStyle = normalItemStyle;

                if (el != null)
                {
                    _currItemStyle = (el.OnEvent == SystemMessage.None) ? badItemStyle : normalItemStyle;
                }

                string _pageTitle = el == null ? "NONE (Page Title)" : (el.hasTitle ? el.title : "- no title -");
                string _pageOnEvent = el == null ? "None (Page OnEvent)" : "On Event: " + el.OnEvent.ToString();

                string _seriesOfPages = "";
                if (el != null && el.hasSlavePages)
                {
                    _seriesOfPages = el.slavePageList.Count > 0 ? string.Format(" | SERIES: {0} pages", el.slavePageList.Count + 1) : "";
                }

                _pageOnEvent = _pageOnEvent + _seriesOfPages;

                EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width - 15, EditorGUIUtility.singleLineHeight), new GUIContent(_pageTitle), EditorStyles.boldLabel);
                EditorGUI.LabelField(new Rect(rect.x, rect.y + 15, rect.width - 15, EditorGUIUtility.singleLineHeight), new GUIContent(_pageOnEvent), _currItemStyle);

                EditorGUI.PropertyField(new Rect(
                rect.x,
                rect.y + 34,
                rect.width,
                EditorGUIUtility.singleLineHeight),
            element.FindPropertyRelative("page"), GUIContent.none);
            };

            newPrefabsList_R.elementHeight = 60;

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(_target);
            }
        }

        private void AddItem(ReorderableList list)
        {
            _target.pageLinkList.Add(new TutorialPageLink());

            EditorUtility.SetDirty(_target);
        }

        private void RemoveItem(ReorderableList list)
        {
            _target.pageLinkList.RemoveAt(list.index);

            EditorUtility.SetDirty(_target);
        }
    }
}