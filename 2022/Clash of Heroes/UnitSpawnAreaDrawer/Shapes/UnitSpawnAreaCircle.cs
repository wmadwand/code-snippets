using System;
using UnityEngine;

public class UnitSpawnAreaCircle : UnitSpawnArea
{
    [Range(.5f, 10f)] public float Radius = 1;

    [Range(1, 10f)] public float SliceCount = 1;

    [HideInInspector]
    [SerializeField] SpawnPoint CenterPoint;

    //------------------------------------------------------------

    public override void InputHandler()
    {
        if (CenterPoint) { return; }

        if (CenterPoint == null)
        {
            CenterPoint = CreateSpawnPoint(GetMouseClickPoint(), transform, 0);
            SpawnPoints.Add(CenterPoint);
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
        //var center = new Vector2(CenterPoint.transform.position.x, CenterPoint.transform.position.z);

        //int top = (int)Math.Floor(center.y - Radius);
        //int bottom = (int)Math.Ceiling(center.y + Radius);
        //int left = (int)Math.Floor(center.x - Radius);
        //int right = (int)Math.Ceiling(center.x + Radius);

        //for (int y = top; y <= bottom; y++)
        //{
        //    for (int x = left; x <= right; x++)
        //    {
        //        if (IsInsideCircle(center, new Vector2(x, y), Radius))
        //        {
        //            var newPos = new Vector3(x, 0, y);
        //            var obj = CreateSpawnPoint(newPos, SpawnPointsParent.transform, 1);

        //            SpawnPoints.Add(obj);
        //        }
        //    }
        //}



        //var points = SamplerUtility.PhyllotaxisSampler(PointCount, Radius, CenterPoint.transform.position, true);

        //foreach (var point in points)
        //{
        //    var obj = CreateSpawnPoint(point, SpawnPointsParent.transform, 1);
        //    SpawnPoints.Add(obj);
        //}


        //var pointCount = PointCount - 1;

        var radius = Radius;
        var pointCount = (float)Math.Ceiling(PointCount / SliceCount);

        for (int i = 0; i < SliceCount; i++)
        {
       
            //var SecondCircle = PointCount - firstCircle;

            for (int j = 0; j < pointCount; j++)
            {
                float angle = j * Mathf.PI * 2f / pointCount;
                Vector3 newPos = CenterPoint.transform.position + new Vector3(Mathf.Cos(angle) * radius, CenterPoint.transform.position.y, Mathf.Sin(angle) * radius);
                var obj = CreateSpawnPoint(newPos, SpawnPointsParent.transform, 1);

                SpawnPoints.Add(obj.GetComponent<SpawnPoint>());
            }

            radius -= Radius / SliceCount;
        }


        //for (int i = 0; i < PointCount; i++)
        //{
        //    float angle = i * Mathf.PI * 2f / PointCount;
        //    Vector3 newPos = CenterPoint.transform.position + new Vector3(Mathf.Cos(angle) * Radius, CenterPoint.transform.position.y, Mathf.Sin(angle) * Radius);
        //    var obj = CreateSpawnPoint(newPos, SpawnPointsParent.transform, 1);

        //    SpawnPoints.Add(obj.GetComponent<SpawnPoint>());
        //}
    }

    private bool IsInsideCircle(Vector2 center, Vector2 tile, float radius)
    {
        float dx = center.x - tile.x,
              dy = center.y - tile.y;
        float distance_squared = dx * dx + dy * dy;
        return distance_squared <= radius * radius;
    }

    protected override void RefreshBasePoints()
    {
        RefreshSpawnPoint(ref CenterPoint);

        SpawnPoints.Insert(0, CenterPoint);
    }
}