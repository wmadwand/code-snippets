using DG.Tweening;
using Coconut.Core;
using Coconut.Core.Asyncs;
using Coconut.Core.Collections;
using Coconut.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeviceNode : MonoBehaviour
{
    public bool IsSelectable { get; private set; }
    public bool IsReady { get; private set; }
    public bool IsChosen { get; private set; }
    public bool IsVisible => IsOBjectInViewPort();

    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private Sprite[] _sprites;
    [SerializeField] private LikeMessage _likeMessage;
    //[SerializeField] private Sprite _sprites;

    private Vector3 _scaleOrigin;
    private bool _isDirty;
    private BGScroller _bgScroller;
    private Vector3 _prevScale;
    private Vector3 _prevPos;

    //---------------------------------------------------

    public void SetChosen(bool val, Transform parent = null)
    {
        if (val)
        {
            IsChosen = true;

            transform.SetParent(null);

            _prevScale = transform.localScale;
            _prevPos = transform.position;
        }
        else
        {
            transform.SetParent(parent);
        }
    }

    public Tween FadeOut()
    {
        return _renderer.DOFade(0, 1);
    }

    public Tween FadeIn()
    {
        return _renderer.DOFade(1, 1);
    }

    public Tween ShowChosen(Vector3 centerPoint)
    {
        return DOTween.Sequence()
            .Append(transform.DOScale(2, 2))
            .Join(transform.DOMove(centerPoint, 2))
            .Append(_likeMessage.Show())
            ;
    }

    public Tween RestoreOnGrid()
    {
        return DOTween.Sequence()
                   .Append(transform.DOScale(_prevScale, 1))
                   .Join(transform.DOMove(_prevPos, 1))
                   ;
    }

    public void Show()
    {
        if (_isDirty)
        {
            return;
        }

        _isDirty = true;

        _ = ShowTween();
    }

    //---------------------------------------------------

    private void Awake()
    {
        _scaleOrigin = _renderer.transform.localScale;
        _renderer.transform.localScale = Vector3.zero;

        _bgScroller = GetComponentInParent<BGScroller>();
    }

    private void Start()
    {
        ResetView();
        //Show();
    }

    private void ResetView()
    {
        IsChosen = false;
        IsReady = false;

        _renderer.transform.DOKill();
        _renderer.transform.localScale = Vector3.zero;
        _renderer.sprite = _sprites.GetRandom();

        // in case we generate cloud just for decoration
        IsSelectable = _renderer.sprite != _sprites[0];
    }

    private Tween ShowTween()
    {
        return DOTween.Sequence()
            .AppendInterval(new RangeFloat(0.5f, 1f).Random())
            .Append(_renderer.transform.DOScale(_scaleOrigin, .4f))
            .AppendCallback(() => { IsReady = true; })
            //.Append(_renderer.transform.DOPunchScale(-_scaleOrigin/2/* + new Vector3(.02f, .02f, .02f)*/, .5f, 2))
            //.Append(_renderer.transform.DOScale(_scaleOrigin, 0))
            ;
    }

    private void Update()
    {
        UpdateState();
    }

    private void UpdateState()
    {
        if (IsOBjectInViewPort() && !_isDirty)
        {
            Show();
            _isDirty = true;
        }
        else if (!IsOBjectInViewPort() && _isDirty && !_bgScroller.IsStoppped)
        {
            ResetView();
            _isDirty = false;
        }
    }

    private bool IsOBjectInViewPort()
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        if (GeometryUtility.TestPlanesAABB(planes, _renderer.bounds))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}