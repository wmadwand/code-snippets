using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialCardUsageUI : MonoBehaviour
{
    [SerializeField] GameObject TargetAnimation;

    private void OnEnable()
    {
        if (TargetAnimation)
        {
            TargetAnimation.transform.parent = null;
            TargetAnimation.transform.position = Vector3.zero;
            TargetAnimation.transform.rotation = Quaternion.identity;
            TargetAnimation.transform.localScale = Vector3.one;
            TargetAnimation.SetActive(true);
        }
    }

    private void OnDisable()
    {
        if (TargetAnimation) { TargetAnimation.SetActive(false); }
    }
}
