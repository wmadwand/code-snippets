using DG.Tweening;
using Coconut.Core;
using Coconut.Core.Asyncs;
using UnityEngine;

public class Background : MonoBehaviour
{
    [SerializeField] private Transform _hexagon;
    [SerializeField] private Transform _geometry;

    [SerializeField] private RangeFloat _speed = new RangeFloat(8, 10);
    [SerializeField] private RangeFloat _scale = new RangeFloat(1.2f, 1.5f);

    //---------------------------------------------------

    public void StartAnimation()
    {
        _hexagon.gameObject.Promise(ZoomLoop, _hexagon);
        _geometry.gameObject.Promise(ZoomLoop, _geometry);
    }

    public void StopAnimation()
    {
        _hexagon.gameObject.CancelPromise<Transform>(ZoomLoop);
        _geometry.gameObject.CancelPromise<Transform>(ZoomLoop);
    }

    //---------------------------------------------------

    private void Start()
    {
        StartAnimation();
    }

    private async Promise ZoomLoop(Transform transform)
    {
        while (true)
        {
            await Zoom(transform);
        }
    }

    private Tween Zoom(Transform transform)
    {
        var speed = _speed.Random();
        var scale = _scale.Random();

        return DOTween.Sequence()
            .Append(transform.DOScale(scale, speed))
            .Append(transform.DOScale(1, speed))
            ;
    }
}