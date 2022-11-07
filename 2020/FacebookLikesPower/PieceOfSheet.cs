using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceOfSheet : MonoBehaviour
{
    public float tileSizeX = 38.35814f;

    private bool isDirty;
    public Renderer rivalRenderer;
    public bool IsVisible => IsOBjectInViewPort();

    private BGScroller bGScroller;

    //---------------------------------------------------

    private void Start()
    {
        bGScroller = GetComponentInParent<BGScroller>();
    }

    private void Update()
    {
        if (!IsOBjectInViewPort() && !isDirty && transform.position.x < rivalRenderer.transform.position.x && !bGScroller.IsStoppped)
        {
            transform.localPosition += new Vector3(tileSizeX * 2, 0, 0);
            isDirty = true;
        }
        else if (IsOBjectInViewPort())
        {
            isDirty = false;
        }
    }

    private bool IsOBjectInViewPort()
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        if (GeometryUtility.TestPlanesAABB(planes, GetComponent<SpriteRenderer>().bounds))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}