using UnityEngine;

public class UnitSpawnAreaRectangle : UnitSpawnArea
{
    public int RowCount;

    [HideInInspector]
    [SerializeField] SpawnPoint StartPoint;

    [HideInInspector]
    [SerializeField] SpawnPoint EndPoint;

    private Vector3 Magnitude => EndPoint.transform.position - StartPoint.transform.position;

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
            StartPoint.gameObject.name = "StartPoint";
            SpawnPoints.Add(StartPoint);
        }
        else if (EndPoint == null)
        {
            EndPoint = CreateSpawnPoint(GetMouseClickPoint(), transform, 0);
            EndPoint.gameObject.name = "EndPoint";

            if (e.control && e.shift)
            {
                //EndPoint.transform.SetZ(StartPoint.transform.position.z);
            }
        }

        if (StartPoint && EndPoint)
        {
            Create();
            SpawnPoints.Add(EndPoint);
            ApplyChanges();
            SetSpawnOrder();
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
        var heightStep = Magnitude.z / (RowCount - 1);
        var widthStep = Magnitude.x / (PointCount - 1);
        var startPos = StartPoint.transform.position;

        for (int i = 0; i < RowCount; i++)
        {
            for (int j = 0; j < PointCount; j++)
            {
                if ((i == 0 && j == 0) || (i == RowCount - 1 && j == PointCount - 1))
                {
                    startPos.x += widthStep;
                    continue;
                }

                var obj = CreateSpawnPoint(startPos, SpawnPointsParent.transform, 1);
                SpawnPoints.Add(obj.GetComponent<SpawnPoint>());
                startPos.x += widthStep;
            }

            startPos.z += heightStep;
            startPos.x = StartPoint.transform.position.x;
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