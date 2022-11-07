using DG.Tweening;
using Coconut.Core;
using Coconut.Core.Asyncs;
using TMPro;
using UnityEngine;
using Zenject;

public class TargetView : MonoBehaviour
{
    [SerializeField] private TextMeshPro _text;
    [SerializeField] private string _incorretcTrigger = "Incorrect";
    [SerializeField] private string _idleState = "Idle";
    [SerializeField] private Transform _shadow;

    [Inject] SceneHelper _sceneHelper;
    private Animator _animator;

    //---------------------------------------------------

    public void ResetView()
    {
        transform.DOKill();
        transform.localScale = Vector3.one;
        _text.transform.localScale = Vector3.zero;

        _text.enabled = false;
    }

    public void SetNumber(string number)
    {
        _text.text = number;
    }

    public Tween Pulse()
    {
        return transform
            .DOPunchScale(transform.localScale / 8f, .3f, 1, 1)
            .SetUpdate(UpdateType.Normal, true)
            ;
    }

    public async Promise Incorrect()
    {
        _animator.SetTrigger(_incorretcTrigger);
        await _animator.WaitStateAsync(_idleState);
    }

    public Tween ShowNumber02()
    {
        _text.enabled = true;
        return transform.DOScale(1, .5f);
    }

    //---------------------------------------------------

    public Tween ShowNumber()
    {
        _text.enabled = true;

        return DOTween.Sequence()
            .AppendInterval(new RangeFloat(0.2f, .5f).Random())
            .Append(_text.transform.DOScale(1, .4f))
            //.AppendCallback(() => { IsReady = true; })
            .Append(_text.transform.DOPunchScale(Vector3.one / 2/* + new Vector3(.02f, .02f, .02f)*/, .5f, 2))
            .Append(_text.transform.DOScale(1, 0))
            .SetUpdate(UpdateType.Normal, true)
            ;
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        var offset = new RangeFloat(-.1f, .1f).Random();
        _shadow.transform.SetY(_sceneHelper.GroundPoint.position.y + offset);
    }

    private Tween Push()
    {
        return transform.DOPunchScale(transform.localScale / -3f, .3f, 8, 1)
            .OnComplete(() => ResetView())
            .SetUpdate(UpdateType.Normal, true)
            ;
    }
}
