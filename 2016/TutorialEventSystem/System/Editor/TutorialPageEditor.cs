using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

namespace TutorialEventSystem
{
    [CustomEditor(typeof(TutorialPage)), CanEditMultipleObjects]
    public class TutorialPageEditor : Editor
    {
        TutorialPage _target;
        ReorderableList newPrefabsList_R;

        //SerializedProperty _guid;
        SerializedProperty _type;
        SerializedProperty _OnEvent; 
        SerializedProperty _proceedButton;
        SerializedProperty _chapter;
        SerializedProperty _placeToShow;

        SerializedProperty _recoverProgressAfterCrash;

        SerializedProperty _hasTitle;
        SerializedProperty _title;
        SerializedProperty _titleFontSize;

        SerializedProperty _infoType;
        SerializedProperty _infoText;
        SerializedProperty _infoTextFontSize;
        SerializedProperty _infoImage;

        SerializedProperty _callToAction; 
        SerializedProperty _callToActionFontSize;

        SerializedProperty _spiritImage;
        SerializedProperty _tooltipPos;
        SerializedProperty _buttonType;

        SerializedProperty _hasSlavePages;
        SerializedProperty _delayBetweenPages;

        TutorialEventController tEventController;
        TutorialGameButtons tGameButtons;

        void OnEnable()
        {
            _target = (TutorialPage)target;

            //_guid = serializedObject.FindProperty("guid");

            _type = serializedObject.FindProperty("type");
            _OnEvent = serializedObject.FindProperty("OnEvent");
            _proceedButton = serializedObject.FindProperty("proceedButton");
            _chapter = serializedObject.FindProperty("chapter");
            _placeToShow = serializedObject.FindProperty("placeToShow");

            _recoverProgressAfterCrash = serializedObject.FindProperty("recoverProgressAfterCrash");

            _hasTitle = serializedObject.FindProperty("hasTitle");
            _title = serializedObject.FindProperty("title");
            _titleFontSize = serializedObject.FindProperty("titleFontSize");

            _infoType = serializedObject.FindProperty("infoType");
            _infoText = serializedObject.FindProperty("infoText");
            _infoTextFontSize = serializedObject.FindProperty("infoTextFontSize");
            _infoImage = serializedObject.FindProperty("infoImage");

            _callToAction = serializedObject.FindProperty("callToAction");
            _callToActionFontSize = serializedObject.FindProperty("callToActionFontSize");

            _spiritImage = serializedObject.FindProperty("spiritImage");
            _tooltipPos = serializedObject.FindProperty("tooltipPosition");
            _buttonType = serializedObject.FindProperty("buttonType");

            _hasSlavePages = serializedObject.FindProperty("hasSlavePages");
            _delayBetweenPages = serializedObject.FindProperty("delayBetweenPages");


            newPrefabsList_R = new ReorderableList(serializedObject, serializedObject.FindProperty("slavePageList"), true, true, true, true);

            newPrefabsList_R.drawHeaderCallback += DrawHeader;
            newPrefabsList_R.drawElementCallback += DrawElement;
            newPrefabsList_R.onAddCallback += AddItem;
            newPrefabsList_R.onRemoveCallback += RemoveItem;

            tEventController = FindObjectOfType<TutorialEventController>();
            tGameButtons = FindObjectOfType<TutorialGameButtons>();
            UpdatePagePreview(_target);

        }

        //void OnGUI()
        //{
        //    if (Event.current.type == EventType.ExecuteCommand || Event.current.type == EventType.ValidateCommand)
        //    {
        //        Event e = Event.current;

        //        if (e.commandName != "" && Event.current.commandName == "Duplicate")
        //            Debug.Log("Command duplicate recognized: " + e.commandName);

        //    }
        //}


        private void OnDisable()
        {
            newPrefabsList_R.drawHeaderCallback -= DrawHeader;
            newPrefabsList_R.drawElementCallback -= DrawElement;
            newPrefabsList_R.onAddCallback -= AddItem;
            newPrefabsList_R.onRemoveCallback -= RemoveItem;

            if (!Application.isPlaying && tEventController) // available only in Editor mode
            {
                tEventController.GameResume(tGameButtons.None(), _target, true);
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawProperties();
            serializedObject.ApplyModifiedProperties();

            if (SingleTargetSelected() && _target.type == PageType.Executable)
            {
                DrawReorderableList();
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(_target);
                UpdatePagePreview(_target);
            }
        }

        public void DrawProperties()
        {
            if (SingleTargetSelected())
            {
                //if (_guid.stringValue == "")
                //{
                //    _guid.SetGUID();
                //}

                string _title = _target.hasTitle ? _target.title : "- no title -";
                EditorGUILayout.LabelField(System.String.IsNullOrEmpty(_title) ? System.String.Empty : _title, EditorStyles.boldLabel);
                GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
            }

            EditorGUILayout.LabelField("System settings", EditorStyles.boldLabel);
            //EditorGUILayout.LabelField(_guid.stringValue);

            EditorGUILayout.PropertyField(_type);

            if (_target.type == PageType.Executable)
            {
                if (SingleTargetSelected())
                {
                    //_target.OnEvent = (SystemMessage)EditorGUILayout.EnumPopup("On Event", _target.OnEvent);
                    EditorGUILayout.PropertyField(_OnEvent);

                }

                EditorGUILayout.PropertyField(_placeToShow);
            }
            else
            {
                _target.OnEvent = SystemMessage.None;
                _target.placeToShow = PlaceToShow.Level;
            }

            if (SingleTargetSelected())
            {
                //_target.proceedButton = (GameButton)EditorGUILayout.EnumPopup("Proceed Button", _target.proceedButton);
                EditorGUILayout.PropertyField(_proceedButton);

            }

            EditorGUILayout.PropertyField(_chapter);

            if (SingleTargetSelected())
            {
                if (_target.type == PageType.Executable && _target.placeToShow == PlaceToShow.Map)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Crash Recovery settings", EditorStyles.boldLabel);

                    //_target.recoverProgressAfterCrash = EditorGUILayout.ToggleLeft("Recover Progress After Crash", _target.recoverProgressAfterCrash);
                    EditorGUILayout.PropertyField(_recoverProgressAfterCrash);
                }
                else
                {
                    _target.recoverProgressAfterCrash = false;
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Visual settings", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(_tooltipPos, new GUIContent("Tooltip Pos"));


            if (_target.OnEvent != SystemMessage.Greetings && _target.OnEvent != SystemMessage.NowJump)
            {
                EditorGUILayout.PropertyField(_buttonType);
            }
            else
            {
                _target.buttonType = ButtonType.Cover;
            }

            EditorGUILayout.Space();

            //_target.hasTitle = EditorGUILayout.ToggleLeft("Title", _target.hasTitle, GUILayout.Width(EditorGUIUtility.labelWidth - 5));
            EditorGUILayout.PropertyField(_hasTitle, new GUIContent("Title"));



            if (SingleTargetSelected())
            {
                if (_target.hasTitle)
                {
                    //_target.title = EditorGUILayout.TextField(_target.title);
                    EditorGUILayout.PropertyField(_title, new GUIContent(""));
                    EditorGUILayout.PropertyField(_titleFontSize, new GUIContent("Font size"));
                }
                else
                {
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.TextField("None");
                    EditorGUI.EndDisabledGroup();
                }
            }


            if (SingleTargetSelected())
            {
                EditorGUILayout.Space();

                //_target.infoType = (InfoType)EditorGUILayout.EnumPopup("Info", _target.infoType);
                EditorGUILayout.PropertyField(_infoType, new GUIContent("Info"));

                if (_target.infoType == InfoType.Text)
                {
                    //_target.infoText = EditorGUILayout.TextArea(_target.infoText, EditorStyles.textArea, GUILayout.Height(55));
                    EditorGUILayout.PropertyField(_infoText, new GUIContent(""), GUILayout.Height(55));

                    EditorGUILayout.PropertyField(_infoTextFontSize, new GUIContent("Font size"));
                }
                else if (_target.infoType == InfoType.Image)
                {
                    //_target.infoImage = (Sprite)EditorGUILayout.ObjectField("", _target.infoImage, typeof(Sprite), false);
                    EditorGUILayout.PropertyField(_infoImage, new GUIContent(""));

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("");
                    GUILayout.FlexibleSpace();

                    Texture2D infoTexture = _target.infoImage ? _target.infoImage.texture : null;
                    GUILayout.Box(infoTexture, GUILayout.Width(64), GUILayout.Height(64));
                    EditorGUILayout.EndHorizontal();

                }

                EditorGUILayout.Space();


                //EditorGUILayout.LabelField("Call-to-action");
                //_target.callToAction = EditorGUILayout.TextField(_target.callToAction);

                EditorGUILayout.LabelField("Call-to-action");
                EditorGUILayout.PropertyField(_callToAction, new GUIContent(""));

                EditorGUILayout.PropertyField(_callToActionFontSize, new GUIContent("Font size"));

                EditorGUILayout.Space();

            }

            //_target.spiritImage = (Sprite)EditorGUILayout.ObjectField("Spirit Image", _target.spiritImage, typeof(Sprite), false);
            EditorGUILayout.PropertyField(_spiritImage);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("");
            GUILayout.FlexibleSpace();

            Texture2D spiritTexture = _target.spiritImage ? _target.spiritImage.texture : null;
            GUILayout.Box(spiritTexture, GUILayout.Width(64), GUILayout.Height(64));

            EditorGUILayout.EndHorizontal();

        }

        private void DrawReorderableList()
        {
            EditorGUILayout.Space();
            //_target.hasSlavePages = EditorGUILayout.ToggleLeft("Slave Pages", _target.hasSlavePages, EditorStyles.boldLabel, GUILayout.Width(EditorGUIUtility.labelWidth - 5));
            EditorGUILayout.PropertyField(_hasSlavePages, GUILayout.Width(EditorGUIUtility.labelWidth - 5));


            if (_target.hasSlavePages)
            {
                EditorGUILayout.HelpBox("The order of the pages IS important.", MessageType.Warning);
                EditorGUILayout.Space();
                //_target.delayBetweenPages = EditorGUILayout.FloatField("Delay Between Pages", _target.delayBetweenPages);
                EditorGUILayout.PropertyField(_delayBetweenPages);
                EditorGUILayout.Space();

                serializedObject.Update();
                newPrefabsList_R.DoLayoutList();
                serializedObject.ApplyModifiedProperties();
            }

        }

        private void DrawHeader(Rect rect)
        {
            GUI.Label(rect, "Slave Page List");
        }

        private void DrawElement(Rect _rect, int _index, bool active, bool focused)
        {
            EditorGUI.BeginChangeCheck();

            newPrefabsList_R.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                rect.y += 2;
                var element = newPrefabsList_R.serializedProperty.GetArrayElementAtIndex(index);

                TutorialPage el = _target.slavePageList[index].page;

                string _pageTitle = el == null ? "NONE (Page Title)" : (el.hasTitle ? el.title : "- no title -");
                string _pageOnEvent = el == null ? "None (Page OnEvent)" : "On Event: " + el.OnEvent.ToString();

                EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width - 15, EditorGUIUtility.singleLineHeight), new GUIContent(_pageTitle), EditorStyles.boldLabel);
                EditorGUI.LabelField(new Rect(rect.x, rect.y + 15, rect.width - 15, EditorGUIUtility.singleLineHeight), new GUIContent(_pageOnEvent), EditorStyles.label);

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
            _target.slavePageList.Add(new TutorialSlavePage());

            EditorUtility.SetDirty(_target);
        }

        private void RemoveItem(ReorderableList list)
        {
            _target.slavePageList.RemoveAt(list.index);

            EditorUtility.SetDirty(_target);
        }

        private bool SingleTargetSelected()
        {
            return targets.Length == 1;
        }

        private void UpdatePagePreview(TutorialPage _page)
        {
            ////AudioSource _previewer;
            ////_previewer = EditorUtility.CreateGameObjectWithHideFlags("Audio preview", HideFlags.HideAndDontSave, typeof(AudioSource)).GetComponent<AudioSource>();
            ////DestroyImmediate(_previewer.gameObject);


            ////GameObject newInstanceGO = PrefabUtility.InstantiatePrefab(newPrefab) as GameObject;

            ////Undo.RegisterCreatedObjectUndo(newInstanceGO, "Instantiate " + currInstanceGO.name);
            ////////Undo.RegisterFullObjectHierarchyUndo(newInstanceGO, newInstanceGO.name);

            ////newInstanceGO.transform.SetParent(currInstanceGO.transform.parent, true);
            ////newInstanceGO.name = currInstanceGO.name;

            ////Undo.DestroyObjectImmediate(currInstanceGO);

            if (!Application.isPlaying && tEventController) // available only in Editor mode
            {
                GameObject tEventControllerGO = tEventController.gameObject;
                tGameButtons = FindObjectOfType<TutorialGameButtons>();

                tEventController.SetupPagePanel(_target);
                tEventController.GamePause(_target, tGameButtons.None());
            }
            else
            {
                Debug.LogError("Application is playing or TutorialEventController script not found on the Scene! Check whether its GameObject is active.");
            }
        }
    }
}