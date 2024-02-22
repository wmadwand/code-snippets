using Sirenix.OdinInspector;
using UnityEngine;
using Cysharp.Threading.Tasks;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

#if UNITY_EDITOR
public class UnitSpawnAreaDrawer : Sirenix.OdinInspector.Editor.OdinEditorWindow
{
    [MenuItem("Window/Unit Spawn Area Drawer")]
    private static void OpenWindow()
    {
        GetWindow<UnitSpawnAreaDrawer>().Show();
    }

    #region Fields
    [InfoBox("$InfoBoxMessage", InfoMessageType.Error, "CheckDefaultSpawnArea")]

    [Sirenix.OdinInspector.FilePath(Extensions = "asset"), LabelText("Default shapes list")]
    [OnValueChanged("OnDefaultShapesPathChanged"), Required(ErrorMessage = "Select DefaultShapesList"), PropertyOrder(20)]
    [SerializeField] private string DefaultShapesPath;

    [AssetsOnly, HideInInspector, InlineEditor(Expanded = false, ObjectFieldMode = InlineEditorObjectFieldModes.CompletelyHidden), PropertyOrder(20)]
    [SerializeField] private UnitSpawnAreaDefaultList DefaultShapes;

    [Sirenix.OdinInspector.FilePath(Extensions = "prefab"), LabelText("Spawn point prefab")]
    [OnValueChanged("OnSpawnPointPrefabPathChanged"), Required(ErrorMessage = "Select UnitSpawnAreaPoint"), PropertyOrder(20)]
    [SerializeField] private string SpawnPointPrefabPath;

    [BoxGroup("Spawn Point Settings")]
    [OnValueChanged("OnSpawnAreaSettingsChanged", true)]
    [/*AssetsOnly,*/ InlineEditor(Expanded = true, ObjectFieldMode = InlineEditorObjectFieldModes.CompletelyHidden), PropertyOrder(20)]
    [SerializeField] private SpawnPoint SpawnPointPrefab;

    [Space]
    [OnValueChanged("OnSpawnAreaSettingsChanged", true)]
    [SceneObjectsOnly, PropertyOrder(25), HorizontalGroup("Area")]
    [SerializeField, InlineEditor(Expanded = true)] IUnitSpawnArea UnitSpawnArea;
    #endregion

    #region Buttons    
    [ButtonGroup("Shapes"), Button("Line", ButtonSizes.Large)]
    private void CreateLine() { CreateSpawnArea<UnitSpawnAreaLine>(); }

    [ButtonGroup("Shapes")]
    [Button("Ring", ButtonSizes.Large)]
    private void CreateRing() => CreateSpawnArea<UnitSpawnAreaRing>();

    [ButtonGroup("Shapes")]
    [Button("Circle", ButtonSizes.Large)]
    private void SomeButton3() => CreateSpawnArea<UnitSpawnAreaCircle>();

    [ButtonGroup("Shapes")]
    [Button("Rectangle", ButtonSizes.Large)]
    private void SomeButton4() => CreateSpawnArea<UnitSpawnAreaRectangle>();

    [ButtonGroup("Shapes")]
    [Button("Triangle", ButtonSizes.Large)]
    private void SomeButton5() => CreateSpawnArea<UnitSpawnAreaTriangle>();

    [ButtonGroup("Shapes")]
    [Button("Wedge", ButtonSizes.Large)]
    private void SomeButton6() => CreateSpawnArea<UnitSpawnAreaWedge>();

    [PropertyOrder(100)]
    [ButtonGroup("AreaButtons")]
    [Button(ButtonSizes.Large), GUIColor(1, 0.5f, 0)]
    [DisableIf("@this.UnitSpawnArea == null")]
    private void Remove()
    {
        //if (UnitSpawnArea != null && EditorUtility.DisplayDialog("Remove Area?", "Are you sure you wanna remove " + UnitSpawnArea + "?", "Yep", "Nope"))
        //{
        //    //var dd = Selection.SetActiveObjectWithContext()

        //    //Undo.RecordObjects(Selection.transforms, "Move Platform");

        //    UnitSpawnArea?.Destroy();
        //    UnitSpawnArea = null;
        //}

        UnitSpawnArea?.Destroy();
        UnitSpawnArea = null;
    }
    #endregion

    private bool CheckDefaultSpawnArea() => HasUnitDefaultSpawnAreaNotFound;
    private bool HasUnitDefaultSpawnAreaNotFound = false;
    private string InfoBoxMessage = "InfoBoxMessage";

    private UnitSpawnAreaOrderFactory SpawnOrderFactory;
    private const string UnitSpawnAreaDrawerSaveName = "UnitSpawnAreaDrawerSave";
    private SpawnPoint SpawnPointPrefabOrigin;

    //------------------------------------------------------------

    protected override void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;

        var data = EditorPrefs.GetString(UnitSpawnAreaDrawerSaveName, JsonUtility.ToJson(this, false));
        JsonUtility.FromJsonOverwrite(data, this);

        if (SpawnPointPrefabPath == "")
        {
            var assets = AssetsUtils.GetAssetsOfType<SpawnPoint>();
            SpawnPointPrefab = assets.Find(item => item.GetComponent<SpawnPointIcon>());
            SpawnPointPrefabPath = AssetDatabase.GetAssetPath(SpawnPointPrefab);
        }
        else
        {
            SpawnPointPrefab = GetAsset<SpawnPoint>(SpawnPointPrefabPath);
        }

        SpawnPointPrefabOrigin = SpawnPointPrefab;

        if (DefaultShapesPath == "")
        {
            var assets = AssetsUtils.GetAssetsOfType<UnitSpawnAreaDefaultList>();
            DefaultShapes = assets.GetSafe(0);
            DefaultShapesPath = AssetDatabase.GetAssetPath(DefaultShapes);
        }
        else
        {
            DefaultShapes = GetAsset<UnitSpawnAreaDefaultList>(DefaultShapesPath);
        }

        SpawnOrderFactory = new UnitSpawnAreaOrderFactory();
    }

    private void OnDisable()
    {
        UnitSpawnArea = null;

        var data = JsonUtility.ToJson(this, false);
        EditorPrefs.SetString(UnitSpawnAreaDrawerSaveName, data);
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        EventType eventType = Event.current.type;

        using (var check = new EditorGUI.ChangeCheckScope())
        {
            if (eventType != EventType.Repaint && eventType != EventType.Layout)
            {
                var e = Event.current;
                if (e.type == EventType.MouseDown && e.button == 0 && e.control)
                {
                    UnitSpawnArea?.InputHandler();
                }

                if (e.type == EventType.MouseDrag /*&& e.shift*/)
                {
                    UnitSpawnArea?.DragHandler();
                }
            }
        }
    }

    private void OnSelectionChange()
    {
        if (Selection.activeGameObject?.GetComponent<IUnitSpawnArea>() == null
            || !Selection.activeGameObject.activeInHierarchy
            || PrefabUtility.IsPartOfAnyPrefab(Selection.activeGameObject))
        {
            return;
        }

        UnitSpawnArea = Selection.activeGameObject.GetComponent<IUnitSpawnArea>();
        var area = (UnitSpawnArea)UnitSpawnArea;
        SpawnPointPrefab = area.SpawnPointPrototype;
    }

    private void CreateSpawnArea<T>() where T : IUnitSpawnArea
    {
        UnitSpawnArea = Event.current.control ? CreateCustomSpawnArea<T>() : CreateDefaultSpawnArea<T>();
    }

    private async void CheckDefaultSpawnAreaa()
    {
        if (HasUnitDefaultSpawnAreaNotFound)
        {
            return;
        }

        HasUnitDefaultSpawnAreaNotFound = true;
        await UniTask.Delay(System.TimeSpan.FromSeconds(3));
        HasUnitDefaultSpawnAreaNotFound = false;

        Repaint();
    }

    private IUnitSpawnArea CreateCustomSpawnArea<T>() where T : IUnitSpawnArea
    {
        var go = new GameObject(typeof(T).Name, typeof(T));

        UnitSpawnArea = go.GetComponent<T>();
        var SpawnPointPrototype = Instantiate(SpawnPointPrefab, Vector3.zero, Quaternion.identity, go.transform);
        SpawnPointPrototype.gameObject.SetActive(false);
        SpawnPointPrototype.name = "SpawnPointPrototype";
        UnitSpawnArea.Init(SpawnPointPrototype, SpawnOrderFactory);

        StageUtility.PlaceGameObjectInCurrentStage(go);
        Selection.SetActiveObjectWithContext(go.gameObject, null);

        return UnitSpawnArea;
    }

    private IUnitSpawnArea CreateDefaultSpawnArea<T>() where T : IUnitSpawnArea
    {
        if (HasUnitDefaultSpawnAreaNotFound)
        {
            return null;
        }

        var prefab = DefaultShapes.GetByName(typeof(T).Name);
        if (!prefab)
        {
            InfoBoxMessage = $"Default shape for {typeof(T).Name} is not found. Check your DefaultShapesList.";
            CheckDefaultSpawnAreaa();
            Debug.LogError(InfoBoxMessage);

            return null;
        }

        var spawnArea = Instantiate(prefab);
        spawnArea.name = typeof(T).Name;

        //TODO: implement more lightweight method
        spawnArea.Refresh(SpawnOrderFactory);

        StageUtility.PlaceGameObjectInCurrentStage(spawnArea.gameObject);
        Selection.SetActiveObjectWithContext(spawnArea.gameObject, null);

        return spawnArea;
    }

    private void OnSpawnAreaSettingsChanged()
    {
        SpawnPointPrefab = SpawnPointPrefab ?? SpawnPointPrefabOrigin;

        //TODO: Rebuild all the points when UnitSpawnArea changed;
        //Just update points settings when SpawnPointPrefab values changed
        UnitSpawnArea?.Refresh(SpawnOrderFactory);
    }

    private void OnSpawnPointPrefabPathChanged()
    {
        SpawnPointPrefab = GetAsset<SpawnPoint>(SpawnPointPrefabPath);
    }

    private void OnDefaultShapesPathChanged()
    {
        DefaultShapes = GetAsset<UnitSpawnAreaDefaultList>(DefaultShapesPath);
    }

    private T GetAsset<T>(string path) where T : Object
    {
        return (T)AssetDatabase.LoadAssetAtPath(path, typeof(T));
    }
}
#endif