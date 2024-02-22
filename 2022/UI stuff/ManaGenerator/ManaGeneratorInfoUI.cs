using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ManaGeneratorInfoUI : BattleUnitInfoUI
{
    [Space(10)]
    [SerializeField] private GameObject ManaDisplay;

    private TextMeshProUGUI ValueField;
    private CanvasGroup CanvasGroup;
    private Tweener ShowTweener;
    private float ManaAmount;

    public override void Invalidate()
    {
        base.Invalidate();
        ShowTweener?.Kill();
        ShowTweener = null;
    }

    public override void Init(UnitView unitView, Vector3 offset)
    {
        base.Init(unitView, offset);
        ValueField = ManaDisplay.GetComponentInChildren<TextMeshProUGUI>();
        CanvasGroup = ManaDisplay.GetComponent<CanvasGroup>();
        Hide();
    }

    public void OnMannaIncreased(float value)
    {
        ManaAmount += value;
        if (ManaAmount >= 1)
        {
            Show(1);
            ManaAmount = 0;
        }
    }

    private async void Show(float value)
    {
        ShowTweener?.Kill();

        ValueField.text = $"+{value}";
        CanvasGroup.alpha = 0;
        ManaDisplay.SetActive(true);
        ShowTweener = CanvasGroup.DOFade(1, .2f);
        
        await UniTask.Delay(TimeSpan.FromSeconds(.9f));
        if (ManaDisplay != null)
        {
            Hide();
        }
    }
    
    private void Hide()
    {
        if (CanvasGroup.alpha > 0)
        {
            CanvasGroup.alpha = 0;
        }
        ManaDisplay.SetActive(false);
    }
}