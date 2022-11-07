using UnityEngine;

public class BGScroller : MonoBehaviour
{
    public float scrollSpeed;
    public float tileSizeX = 38.35814f;
    public SpriteRenderer[] renderers;
    public bool IsStoppped { get; private set; }

    //---------------------------------------------------

    public void Stop(bool val)
    {
        IsStoppped = val;
    }

    //---------------------------------------------------

    private void Start()
    {
        var width = GetComponentInChildren<SpriteRenderer>().bounds.size.x;
        Debug.Log(width);
    }

    private void Update()
    {
        if (IsStoppped)
        {
            return;
        }

        transform.position += Vector3.left * Time.deltaTime * scrollSpeed;
    }
}