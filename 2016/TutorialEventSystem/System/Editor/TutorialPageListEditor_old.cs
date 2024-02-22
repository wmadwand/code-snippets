//using UnityEngine;
//using System.Collections.Generic;

//#if UNITY_EDITOR
//using UnityEditor;
//using UnityEditorInternal;
//#endif

//namespace TutorialEventSystem
//{
//    [CustomEditor(typeof(TutorialPageList))]
//    public class TutorialPageListEditor : Editor
//    {
//        TutorialPageList _target;
//        //ReorderableList newPrefabsList_R;
//        List<ReorderableList> ListOfList_R = new List<ReorderableList>();

//        private void OnEnable()
//        {
//            _target = (TutorialPageList)target;

//            //_target.pageLinkGlobalList.Clear();

//            if (_target.pageLinkGlobalList.Count == 0)
//            {
//                TutorialPage tp = new TutorialPage { title = "hello" };
//                List<TutorialPage> tpList = new List<TutorialPage>();
//                tpList.Add(tp);

//                _target.pageLinkGlobalList.Add(new TutorialPageLink { chapter = Chapter.Anomaly, pageList = tpList });

//                _target.pageLinkGlobalList[0].pageList.Add(tp);
//            }

//            for (int i = 0; i < _target.pageLinkGlobalList.Count; i++)
//            {
//                ReorderableList rList = new ReorderableList(serializedObject, serializedObject.FindProperty("pageLinkGlobalList"), true, true, true, true);
//                ListOfList_R.Add(rList);

//                rList.drawHeaderCallback += DrawHeader;
//                rList.drawElementCallback += DrawElement;

//                rList.onAddCallback += AddItem;
//                rList.onRemoveCallback += RemoveItem;
//            }
//        }

//        private void OnDisable()
//        {
//            for (int i = 0; i < _target.pageLinkGlobalList.Count; i++)
//            {
//                ReorderableList rList = ListOfList_R[i];

//                rList.drawHeaderCallback -= DrawHeader;
//                rList.drawElementCallback -= DrawElement;

//                rList.onAddCallback -= AddItem;
//                rList.onRemoveCallback -= RemoveItem;
//            }
//        }

//        public override void OnInspectorGUI()
//        {
//            DrawProperties();

//            for (int i = 0; i < _target.pageLinkGlobalList.Count; i++)
//            {
//                DrawReorderableList(ListOfList_R[i]);
//                EditorGUILayout.Space();
//            }


//            if (GUI.changed)
//            {
//                EditorUtility.SetDirty(target);
//            }
//        }

//        void DrawProperties() { }

//        private void DrawReorderableList(ReorderableList _rList)
//        {
//            //EditorGUILayout.HelpBox("The order of the pages is not important.", MessageType.Info);
//            //EditorGUILayout.Space();
//            //_target.delayBetweenPages = EditorGUILayout.FloatField("Delay Between Pages", _target.delayBetweenPages);
//            //EditorGUILayout.Space();

//            serializedObject.Update();
//            _rList.DoLayoutList();
//            serializedObject.ApplyModifiedProperties();
//        }

//        private void DrawHeader(Rect rect)
//        {
//            GUI.Label(rect, "Tutorial Page List");
//        }

//        private void DrawElement(Rect _rect, int _index, bool active, bool focused)
//        {
//            //for (int i = 0; i < _target.pageLinkGlobalList.Count; i++)
//            //{
//            EditorGUI.BeginChangeCheck();

//            ListOfList_R[0].drawElementCallback = (rect, index, isActive, isFocused) =>
//            {
//                rect.y += 2;
//                var element = ListOfList_R[0].serializedProperty.GetArrayElementAtIndex(index);
//                TutorialPage el = _target.pageLinkGlobalList[0].pageList[index];



//                GUIStyle normalItemStyle = new GUIStyle(EditorStyles.label);
//                normalItemStyle.normal.textColor = Color.black;

//                GUIStyle badItemStyle = new GUIStyle(EditorStyles.label);
//                badItemStyle.normal.textColor = Color.red;

//                GUIStyle goodItemStyle = new GUIStyle(EditorStyles.label);
//                goodItemStyle.normal.textColor = Color.green;

//                GUIStyle _currItemStyle = normalItemStyle;

//                if (el != null)
//                {
//                    _currItemStyle = (el.OnEvent == SystemMessage.None) ? badItemStyle : normalItemStyle;
//                }

//                string _pageTitle = el == null ? "NONE (Page Title)" : (el.hasTitle ? el.title : "- no title -");
//                string _pageOnEvent = el == null ? "None (Page OnEvent)" : "On Event: " + el.OnEvent.ToString();

//                string _seriesOfPages = "";
//                if (el != null && el.hasSlavePages)
//                {
//                    _seriesOfPages = el.slavePageList.Count > 0 ? string.Format(" | SERIES: {0} pages", el.slavePageList.Count + 1) : "";
//                }

//                _pageOnEvent = _pageOnEvent + _seriesOfPages;

//                EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width - 15, EditorGUIUtility.singleLineHeight), new GUIContent(_pageTitle), EditorStyles.boldLabel);
//                EditorGUI.LabelField(new Rect(rect.x, rect.y + 15, rect.width - 15, EditorGUIUtility.singleLineHeight), new GUIContent(_pageOnEvent), _currItemStyle);

//                EditorGUI.PropertyField(new Rect(
//                rect.x,
//                rect.y + 34,
//                rect.width,
//                EditorGUIUtility.singleLineHeight),
//            element.FindPropertyRelative("pageList[0]"), GUIContent.none);
//            };

//            ListOfList_R[0].elementHeight = 60;

//            if (EditorGUI.EndChangeCheck())
//            {
//                EditorUtility.SetDirty(target);
//            }
//            //}
//        }

//        private void AddItem(ReorderableList list)
//        {
//            int _a = ListOfList_R.IndexOf(list);

//            _target.pageLinkGlobalList[_a].pageList.Add(new TutorialPage());

//            EditorUtility.SetDirty(target);
//        }

//        private void RemoveItem(ReorderableList list)
//        {
//            int _a = ListOfList_R.IndexOf(list);

//            _target.pageLinkGlobalList[_a].pageList.Add(new TutorialPage());

//            EditorUtility.SetDirty(target);
//        }
//    }
//}