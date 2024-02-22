using UnityEngine;

public class UnitSpawnAreaLine : UnitSpawnArea
{
    [HideInInspector]
    [SerializeField] SpawnPoint StartPoint;

    [HideInInspector]
    [SerializeField] SpawnPoint EndPoint;

    //private Vector3 Magnitude => EndPoint.transform.position - StartPoint.transform.position;

    private Vector3 Magnitude()
    {
        Vector3 result = Vector3.zero;

        if (EndPoint && StartPoint)
        {
            result = EndPoint.transform.position - StartPoint.transform.position;
        }

        return result;
    }

    //------------------------------------------------------------

    public override void InputHandler()
    {
        if (StartPoint && EndPoint)
        {
            return;
        }

        var e = Event.current;

        if (StartPoint == null)
        {
            StartPoint = CreateSpawnPoint(GetMouseClickPoint(), transform, 0);
            SpawnPoints.Add(StartPoint);
        }
        else if (EndPoint == null)
        {
            EndPoint = CreateSpawnPoint(GetMouseClickPoint(), transform, 0);

            if (e.control && e.shift)
            {
                EndPoint.transform.SetZ(StartPoint.transform.position.z);
            }
        }

        if (StartPoint && EndPoint)
        {
            Create();
            SpawnPoints.Add(EndPoint);
            SetSpawnOrder();
            ApplyChanges();
        }
    }

    public override void DragHandler()
    {
        if (!StartPoint || !EndPoint) { return; }

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
        var step = Magnitude() / (PointCount - 1);
        var pos = StartPoint.transform.position;

        for (int i = 0; i < PointCount - 2; i++)
        {
            pos += step;
            var obj = CreateSpawnPoint(pos, SpawnPointsParent.transform, 1);

            SpawnPoints.Add(obj.GetComponent<SpawnPoint>());
        }
    }

    protected override void RefreshBasePoints()
    {
        RefreshSpawnPoint(ref StartPoint);
        RefreshSpawnPoint(ref EndPoint);

        SpawnPoints.Insert(0, StartPoint);
        SpawnPoints.Insert(SpawnPoints.Count - 1, EndPoint);
    }
}