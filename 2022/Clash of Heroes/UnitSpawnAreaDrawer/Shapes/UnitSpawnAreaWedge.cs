using UnityEngine;

public class UnitSpawnAreaWedge : UnitSpawnArea
{
    public float Width = 1;

    [HideInInspector]
    [SerializeField] SpawnPoint BasePoint01;

    [HideInInspector]
    [SerializeField] SpawnPoint BasePoint02;

    [HideInInspector]
    [SerializeField] SpawnPoint BasePoint03;

    //------------------------------------------------------------

    public override void InputHandler()
    {
        if (BasePoint01 && BasePoint02 && BasePoint03)
        {
            return;
        }

        var e = Event.current;

        if (BasePoint01 == null)
        {
            BasePoint01 = CreateSpawnPoint(GetMouseClickPoint(), transform, 0);
            BasePoint01.gameObject.name = "BasePoint01";
            SpawnPoints.Add(BasePoint01);
        }
        else if (BasePoint02 == null)
        {
            BasePoint02 = CreateSpawnPoint(GetMouseClickPoint(), transform, 0);
            BasePoint02.gameObject.name = "BasePoint02";

            if (e.control && e.shift)
            {
                BasePoint02.transform.SetZ(BasePoint01.transform.position.z);
            }
        }
        else if (BasePoint03 == null)
        {
            BasePoint03 = CreateSpawnPoint(GetMouseClickPoint(), transform, 0);
            BasePoint03.gameObject.name = "BasePoint03";
            SpawnPoints.Add(BasePoint03);

            if (e.control && e.shift)
            {
                var center = (BasePoint02.transform.position + BasePoint01.transform.position) / 2;
                BasePoint03.transform.SetX(center.x);
            }
        }

        if (BasePoint01 && BasePoint02 && BasePoint03)
        {
            Create();
            SpawnPoints.Add(BasePoint02);
            ApplyChanges();
            SetSpawnOrder();
        }
    }

    public override void DragHandler()
    {
        if (!BasePoint01 || !BasePoint02 || !BasePoint03) { return; }

#if UNITY_EDITOR

        var activeGO = UnityEditor.Selection.activeGameObject?.GetComponent<SpawnPoint>();
        if (activeGO == BasePoint01 || activeGO == BasePoint02 || activeGO == BasePoint03)
        {
            Refresh(SpawnOrderFactory, false);
        }
#endif
    }

    //------------------------------------------------------------

    protected override void Create()
    {
        var pointCount = PointCount;

        for (int c = 0; c < Width; c++)
        {
            var point1 = BasePoint01.transform.position + new Vector3(c, 0, 0);
            var point2 = BasePoint03.transform.position - new Vector3(0, 0, c);
            var magnitude = point2 - point1;
            var step1 = magnitude / (pointCount - 1);
            DoSpawnPoints(point1, step1, c, pointCount);

            point1 = BasePoint02.transform.position - new Vector3(c, 0, 0);
            magnitude = point2 - point1;
            step1 = magnitude / (pointCount - 1);
            DoSpawnPoints(point1, step1, c, pointCount);

            pointCount -= 1;
        }
    }

    private void DoSpawnPoints(Vector3 pos, Vector3 step, int cc, int pointCount)
    {
        for (int i = 0; i < pointCount; i++)
        {
            if (cc == 0)
            {
                pos += step;

                if (i == pointCount - 2)
                {
                    break;
                }
            }

            var obj = CreateSpawnPoint(pos, SpawnPointsParent.transform, 1);

            SpawnPoints.Add(obj.GetComponent<SpawnPoint>());

            if (cc != 0)
            {
                pos += step;
            }
        }
    }

    protected override void RefreshBasePoints()
    {
        RefreshSpawnPoint(ref BasePoint01);
        RefreshSpawnPoint(ref BasePoint02);
        RefreshSpawnPoint(ref BasePoint03);        

        SpawnPoints.Insert(0, BasePoint01);
        SpawnPoints.Insert(SpawnPoints.Count - 1, BasePoint02);
        SpawnPoints.Insert(SpawnPoints.Count - 2, BasePoint03);
    }
}