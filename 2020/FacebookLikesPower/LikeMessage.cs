using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LikeMessage : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _renderer;

    public Tween Show()
    {
        return DOTween.Sequence()
            .Append(_renderer.DOFade(1, .5f))
            .Join(_renderer.transform.DOLocalMoveY(0.8f, .5f))
            .AppendInterval(.5f)
            .Append(_renderer.DOFade(0, .5f))
            .Join(_renderer.transform.DOLocalMoveY(0.4f, .5f))
            ;
    }

    private void Start()
    {
        _renderer.DOFade(0, 0);
        _renderer.transform.DOLocalMoveY(1.5f, 0);
    }
}