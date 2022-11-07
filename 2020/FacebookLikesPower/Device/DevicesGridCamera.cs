using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DevicesGridCamera : MonoBehaviour
{
    private Camera _camera;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    private void Start()
    {
        //_camera.DOOrthoSize(3, 3);
    }

    public Tween ZoomInToPosition(Vector2 value)
    {
        var pos = new Vector3(value.x, value.y, -10);

        return DOTween.Sequence()
            .Append(_camera.DOOrthoSize(3, 3))
            .Join(transform.DOMove(pos, 3))
            ;
    }

    public Tween ZoomOut()
    {
        var pos = new Vector3(0, -2.7f, -10);


        return DOTween.Sequence()
            .Append(_camera.DOOrthoSize(8.1f, 3))
            .Join(transform.DOMove(pos, 3))
            ;
    }
}