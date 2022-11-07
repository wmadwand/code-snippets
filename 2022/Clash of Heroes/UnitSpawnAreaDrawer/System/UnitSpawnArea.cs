using PathCreationEditor;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class UnitSpawnArea : MonoBehaviour, IUnitSpawnArea
{
    public SpawnPoint SpawnPointPrototype => SpawnPointPrefab;

    [SerializeField] protected int PointCount;
    [SerializeField] protected float DelayStep;
    [SerializeField] protected UnitSpawnAreaOrder SpawnOrder = UnitSpawnAreaOrder.AtOnce;

    [HideInInspector]
    [SerializeField] protected SpawnPoint SpawnPointPrefab;

    [HideInInspector]
    [SerializeField] protected Transform SpawnPointsParent;

    [HideInInspector]
    [SerializeField] protected List<SpawnPoint> SpawnPoints = new List<SpawnPoint>();

    protected UnitSpawnAreaOrderFactory SpawnOrderFactory;
    protected bool IsValid => SpawnPoints.Count > 0;

    //------------------------------------------------------------

    public abstract void DragHandler();

    public abstract void InputHandler();

    protected abstract void Create();

    protected abstract void RefreshBasePoints();

    //------------------------------------------------------------

    public void Init(SpawnPoint spawnPointPrefab, UnitSpawnAreaOrderFactory spawnOrderFactory)
    {
        SpawnPointPrefab = spawnPointPrefab;
        SpawnOrderFactory = spawnOrderFactory;

        var spawnPointsParent = new GameObject("SpawnPoints");
        spawnPointsParent.transform.SetParent(transform);
        SpawnPointsParent = spawnPointsParent.transform;
    }

    public void Refresh(UnitSpawnAreaOrderFactory spawnOrderFactory, bool withBasePoints = true)
    {
        try
        {
            if (!IsValid || !SpawnPointsParent)
            {
                return;
            }

            if (SpawnOrderFactory == null)
            {
                SpawnOrderFactory = spawnOrderFactory;
            }

            while (SpawnPointsParent.transform.childCount > 0)
            {
                DestroyImmediate(SpawnPointsParent.transform.GetChild(0).gameObject);
            }

            SpawnPoints.Clear();
            Create();

            if (withBasePoints)
            {
                RefreshBasePoints();
            }

            SetSpawnOrder();
        }
        catch { }
    }

    public void Destroy()
    {
        SpawnPoints.Clear();
        DestroyImmediate(gameObject);
    }

    //------------------------------------------------------------

    protected void ApplyChanges()
    {
#if UNITY_EDITOR
        Selection.SetActiveObjectWithContext(gameObject, null);
        EditorUtility.SetDirty(transform.parent.gameObject);
#endif
    }

    protected Vector3 GetMouseClickPoint()
    {
        float dstCamToEndpoint = (Camera.current.transform.position - transform.position).magnitude;
        return MouseUtility.GetMouseWorldPosition(PathSpace.XZ, dstCamToEndpoint);
    }

    protected void SetSpawnOrder()
    {
        var SpawnOrderHandler = SpawnOrderFactory.Get(SpawnOrder);
        SpawnOrderHandler.Apply(SpawnPoints, DelayStep, PointCount, this.GetType());
    }

    protected SpawnPoint CreateSpawnPoint(Vector3 position, Transform parent, int spriteIndex)
    {
        var spawnPoint = Instantiate(SpawnPointPrefab, position, Quaternion.Euler(90, 0, 0), parent);
        spawnPoint.GetComponent<SpawnPointIcon>().SetSprite(spriteIndex);
        spawnPoint.gameObject.SetActive(true);

        return spawnPoint;
    }

    protected void RefreshSpawnPoint(ref SpawnPoint spawnPoint)
    {
        var newStartPoint = CreateSpawnPoint(spawnPoint.transform.position, transform, 0);
        DestroyImmediate(spawnPoint.gameObject);
        spawnPoint = newStartPoint;

        //return newStartPoint;
        //if (SpawnPointPrototype == null)
        //{
        //    SpawnPointPrototype = newStartPoint;

        //}
    }
}