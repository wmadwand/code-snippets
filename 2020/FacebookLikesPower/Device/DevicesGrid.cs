using Coconut.Core.Collections;
using System;
using UnityEngine;
using DG.Tweening;
using Zenject;
using MiniGames.Games.FBLikesPower;

public class DevicesGrid : MonoBehaviour
{
    public SpriteRenderer[] renderers;
    [SerializeField] private float _fadeRate = .7f;
    [SerializeField] private Transform _originParent;

    private DeviceNode[] _devices;
    [Inject] private FBLikesPowerController _gameController;


    //---------------------------------------------------

    public DeviceNode GetRandomDevice()
    {
        return Array.FindAll(_devices, d => d.IsVisible
        && d.IsSelectable
        && d.IsReady
        && !d.IsChosen
        && d.transform.position.x > _gameController.centerPoint.position.x)
            .GetRandom();
    }

    public Tween FadeIn()
    {
        var seq = DOTween.Sequence();

        foreach (var item in renderers)
        {
            seq.Join(item.DOFade(1f, 1));
        }

        foreach (var item1 in _devices)
        {
            seq.Join(item1.FadeIn());
        }

        return seq;
    }

    public Tween FadeOut(float fadeRate, float speed, DeviceNode excludeNode = null)
    {
        var seq = DOTween.Sequence();

        foreach (var item in renderers)
        {
            seq.Join(item.DOFade(fadeRate, speed));
        }

        foreach (var item1 in _devices)
        {
            if (item1 == excludeNode)
            {
                continue;
            }

            seq.Join(item1.FadeOut());
        }

        return seq;
    }

    public void RestoreParent()
    {
        transform.parent = _originParent;
    }

    //---------------------------------------------------

    private void Awake()
    {
        _devices = GetComponentsInChildren<DeviceNode>(false);
    }

    private void Start()
    {
        foreach (var item in renderers)
        {
            item.DOFade(.5f, 0);
        }

        FadeIn();
    }
}