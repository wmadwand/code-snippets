using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableCardCatapultSelectorTutorial : DraggableCardCatapultSelector
{
    [SerializeField] DraggableCardCatapultSelectorTutorialTarget TargetPrefab;
    [SerializeField] float DistanceToTarget;
    [SerializeField] float DestroyDelayAfterApply;

    static TutorialCardUsageUI TutorialCardUsageUI;

    private DraggableCardCatapultSelectorTutorialTarget Target;

    protected override void Awake()
    {
        base.Awake();
        if (TargetPrefab)
        {
            Target = Instantiate(TargetPrefab);
        }

        if (!TutorialCardUsageUI)
        {
            TutorialCardUsageUI = transform.root.gameObject.GetComponentInChildren<TutorialCardUsageUI>(includeInactive: true);
        }
        TutorialCardUsageUI?.gameObject.SetActive(true);

    }

    private void OnDisable()
    {
        if (Target) { Target.gameObject.SetActive(false); }
    }

    private void OnEnable()
    {
        if (Target) { Target.gameObject.SetActive(true); }
    }

    public override void Invalidate()
    {
        if (Target)
        {
            Destroy(Target.gameObject);
        }
        TutorialCardUsageUI?.gameObject.SetActive(true);
        base.Invalidate();
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        LineRenderer.material.SetFloat(ShaderConstants.ClipMask, 1);
        TutorialCardUsageUI?.gameObject.SetActive(false);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        TutorialCardUsageUI?.gameObject.SetActive(true);
        base.OnEndDrag(eventData);
    }

    protected override void ApplyDeploy()
    {
        if (!Target)
        {
            base.ApplyDeploy();
            return;
        }
        if ((ViewOverField.transform.position - Target.transform.position).sqrMagnitude < DistanceToTarget)
        {
            base.ApplyDeploy();
            Time.timeScale = 1;
            Destroy(Target.gameObject, DestroyDelayAfterApply);
            TutorialCardUsageUI?.gameObject.SetActive(false);
        }
        else
        {
            Select(false);
        }
    }



    protected override void Update()
    {
        base.Update();
        if (Target)
        {
            var matched = (ViewOverField.transform.position - Target.transform.position).sqrMagnitude < DistanceToTarget;
            Target.Refresh(matched);
        }

    }
}
