using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EventChestKeysUI : BehaviourWithBattleSimulator
{
    public Transform TargetPanel => ShowPoint;

    [SerializeField] TextMeshProUGUI Count;
    [SerializeField] Transform Panel;
    [SerializeField] Image Icon;
    [SerializeField] float AnimationTime;
    [SerializeField] RectTransform ShowPoint;
    [SerializeField] RectTransform HidePoint;

    //------------------------------------------------------------

    [Button]
    public Tween ShowPanel(bool value, float time)
    {
        var point = value ? ShowPoint : HidePoint;
        return Panel.DOMove(point.position, time);
    }

    //------------------------------------------------------------

    private void Start()
    {
        DropView.OnCollect += OnDropViewCollect;
        Count.text = Game.PlayerProfile.Resources.EventChestKeys.ToString();
        ShowPanel(false, 0);
    }

    private void OnDestroy()
    {
        DropView.OnCollect -= OnDropViewCollect;
    }

    private void OnDropViewCollect(Drop drop, float applyAfterTime, DropView.DropType dropType)
    {
        if (dropType != DropView.DropType.ChestKey) { return; }

        StartCoroutine(OnDropViewCollectRoutine(drop, applyAfterTime));
    }

    private IEnumerator OnDropViewCollectRoutine(Drop drop, float applyAfterTime)
    {
        ShowPanel(true, AnimationTime);
        yield return new WaitForSeconds(applyAfterTime);
        Count.text = Game.PlayerProfile.Resources.EventChestKeys.ToString();

        yield return new WaitForSeconds(.2f);
        ShowPanel(false, AnimationTime);
    }
}