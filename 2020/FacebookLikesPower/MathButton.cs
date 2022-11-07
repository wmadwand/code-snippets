using DG.Tweening;
using Coconut.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class MathButton : MonoBehaviour, IPointerDownHandler
{
    public string Symbol => _symbol;
    public Sprite SymbolSprite => _symbolRenderer.sprite;

    [SerializeField] private string _symbol;
    [SerializeField] private SpriteRenderer _symbolRenderer;

    [Inject] private LikesMessageBus _messageBus;
    private bool _isLock;

    //---------------------------------------------------

    public void SetLock(bool value)
    {
        _isLock = value;
    }

    public Tween Pulse()
    {
        return transform
            .DOPunchScale(transform.localScale / 8f, .3f, 1, 1)
            .SetUpdate(UpdateType.Normal, true)
            ;
    }

    public void Reset()
    {
        transform.DOKill();
        transform.localScale = Vector3.one;
    }

    //---------------------------------------------------

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        if (_isLock)
        {
            return;
        }

        SetLock(true);

        _messageBus.mathButtonClick.Send(this);

        Reset();
        Push();
    }

    private Tween Push()
    {
        return transform.DOPunchScale(transform.localScale / -3f, .3f, 8, 1)
            .OnComplete(() => Reset())
            .SetUpdate(UpdateType.Normal, true)
            ;
    }
}