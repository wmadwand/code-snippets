using System;
using UnityEngine;

public class UnitSpawnAreaTriangle : UnitSpawnArea
{
    public float Angle = 0;

    [HideInInspector]
    [SerializeField] SpawnPoint BasePoint01;

    [HideInInspector]
    [SerializeField] SpawnPoint BasePoint02;

    [HideInInspector]
    [SerializeField] SpawnPoint BasePoint03;

    [Range(.3f, 10)]
    public float step;

    private Vector3 Magnitude => BasePoint02.transform.position - BasePoint01.transform.position;

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
        }

        if (BasePoint01 && BasePoint02 && BasePoint03)
        {
            Create();
            ApplyChanges();
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
        var points = new[] { new Vector2(BasePoint01.transform.position.x, BasePoint01.transform.position.z),
                             new Vector2(BasePoint02.transform.position.x, BasePoint02.transform.position.z),
                             new Vector2(BasePoint03.transform.position.x, BasePoint03.transform.position.z) };

        var boundsCenter = DescribedCircleCenter(points[0], points[1], points[2]);
        var boundRadius = DescribedCircleRadius(boundsCenter, points[0]);

        var top = boundsCenter.y - boundRadius;
        var bottom = boundsCenter.y + boundRadius;
        var left = boundsCenter.x - boundRadius;
        var right = boundsCenter.x + boundRadius;

        for (float y = top; y <= bottom;)
        {
            for (float x = left; x <= right;)
            {
                if (IsPointInTriangle(new Vector2(x, y), points[0], points[1], points[2]))
                {
                    var newPos = new Vector3(x, 0, y);
                    var obj = CreateSpawnPoint(newPos, SpawnPointsParent.transform, 1);
                    SpawnPoints.Add(obj.GetComponent<SpawnPoint>());
                }

                x += step;
            }

            y += step;
        }
    }

    private Vector2 GetTriangleCenter(Vector2 p0, Vector2 p1, Vector2 p2)
    {
        return new Vector2(p0.x + p1.x + p2.x / 3, p0.y + p1.y + p2.y / 3);
    }

    private bool IsPointInTriangle(Vector3 p, Vector3 a, Vector3 b, Vector3 c)
    {
        var v0 = c - a;
        var v1 = b - a;
        var v2 = p - a;

        var dot00 = Vector3.Dot(v0, v0);
        var dot01 = Vector3.Dot(v0, v1);
        var dot02 = Vector3.Dot(v0, v2);
        var dot11 = Vector3.Dot(v1, v1);
        var dot12 = Vector3.Dot(v1, v2);

        var invDenominator = 1f / (dot00 * dot11 - dot01 * dot01);
        var u = (dot11 * dot02 - dot01 * dot12) * invDenominator;
        var v = (dot00 * dot12 - dot01 * dot02) * invDenominator;

        return u >= 0f && v >= 0f && u + v < 1f;
    }

    private float DescribedCircleRadius(Vector2 center, Vector2 vertex)
    {
        return (float)Math.Sqrt(Math.Pow(center.x - vertex.x, 2) + Math.Pow(center.y - vertex.y, 2));
    }

    private Vector2 DescribedCircleCenter(Vector2 pt1, Vector2 pt2, Vector2 pt3)
    {
        Vector2 result = new Vector2();
        float ka, kb;

        var pp0 = (float)(pt2.x - pt1.x);
        var pp1 = CheckForNaN(pp0);/* Mathf.Approximately(pp0, 0) ? 0.0001f : pp0;*/

        var pp00 = (float)(pt3.x - pt2.x);
        var pp2 = CheckForNaN(pp00); /*Mathf.Approximately(pp00, 0) ? 0.0001f : pp00;*/

        ka = (float)(pt2.y - pt1.y) / pp1;
        kb = (float)(pt3.y - pt2.y) / pp2;
        result.x = (ka * kb * (pt1.y - pt3.y) + kb * (pt1.x + pt2.x) - ka * (pt2.x + pt3.x)) / (2 * (kb - ka));

        var yy = -(result.x - (pt1.x + pt2.x) / 2) / ka + (pt1.y + pt2.y) / 2;

        //if (float.IsNaN(yy) || float.IsInfinity(yy) || Mathf.Approximately(yy, 0))
        //{
        //    yy = 0.0001f;
        //}

        result.y = CheckForNaN(yy);
        return result;
    }

    float CheckForNaN(float yy)
    {
        if (float.IsNaN(yy) || float.IsInfinity(yy) || Mathf.Approximately(yy, 0))
        {
            yy = 0.0001f;
        }

        return yy;
    }

    protected override void RefreshBasePoints()
    {
        RefreshSpawnPoint(ref BasePoint01);
        RefreshSpawnPoint(ref BasePoint02);
        RefreshSpawnPoint(ref BasePoint03);

        //SpawnPoints.Insert(0, BasePoint01);
        //SpawnPoints[PointCount - 1] = BasePoint02;
    }
}