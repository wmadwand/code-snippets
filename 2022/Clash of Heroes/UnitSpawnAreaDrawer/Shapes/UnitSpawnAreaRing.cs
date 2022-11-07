using UnityEngine;

public class UnitSpawnAreaRing : UnitSpawnArea
{
    public float Radius = 1;

    [HideInInspector]
    [SerializeField] SpawnPoint CenterPoint;

    //------------------------------------------------------------

    public override void InputHandler()
    {
        if (CenterPoint)
        {
            return;
        }

        var e = Event.current;

        if (CenterPoint == null)
        {
            //point1 = new GameObject("Center");
            //point1.transform.position = newPathPoint;
            //point1.transform.SetParent(transform);            

            CenterPoint = CreateSpawnPoint(GetMouseClickPoint(), transform, 0);
            //CenterPoint.gameObject.SetActive(false);
        }

        if (CenterPoint)
        {
            Create();
            ApplyChanges();
            SetSpawnOrder();
        }
    }

    public override void DragHandler()
    {
        if (!CenterPoint) { return; }

#if UNITY_EDITOR
        if (UnityEditor.Selection.activeGameObject?.GetComponent<SpawnPoint>())
        {
            Refresh(SpawnOrderFactory, false);
        }
#endif
    }

    //------------------------------------------------------------

    protected override void Create()
    {
        for (int i = 0; i < PointCount; i++)
        {
            float angle = i * Mathf.PI * 2f / PointCount;
            Vector3 newPos = CenterPoint.transform.position + new Vector3(Mathf.Cos(angle) * Radius, CenterPoint.transform.position.y, Mathf.Sin(angle) * Radius);
            var obj = CreateSpawnPoint(newPos, SpawnPointsParent.transform, 1);

            SpawnPoints.Add(obj.GetComponent<SpawnPoint>());
        }
    }

    protected override void RefreshBasePoints()
    {
        RefreshSpawnPoint(ref CenterPoint);
        

        SpawnPoints.Insert(0, CenterPoint);
    }
}