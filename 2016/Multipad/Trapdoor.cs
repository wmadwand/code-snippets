using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Trapdoor : Multipad
{
    [SerializeField]
    float groundOverlapRadius = 2.5f;

    [SerializeField]
    private float notRunTime = 2f;

    private BoxCollider2D[] _cols2D;
    private Animator _animator;
    private bool trapdoorActivated;

#if UNITY_EDITOR
    [Header("Debug")]
    [SerializeField]
    bool showRadius = true;
#endif

    public override void RunCurrentAction()
    {
        _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, 0f);
        _unitControllerV2.StartCoroutine(_unitControllerV2.IFallDown(transform.position, groundOverlapRadius, notRunTime));
        _animator.SetTrigger("Activate");
        SwitchColliders();
    }

    private void SwitchColliders()
    {
        trapdoorActivated = !trapdoorActivated;

        if (_cols2D.Length > 0)
        {
            foreach (BoxCollider2D col in _cols2D)
            {
                col.enabled = !col.enabled;
            }
        }
    }

    private void Start()
    {
        _cols2D = GetComponents<BoxCollider2D>();
        _animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (_animator != null && trapdoorActivated &&
            _animator.GetCurrentAnimatorStateInfo(0).IsTag("trapdoor_closing") &&
            _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f)
        {
            SwitchColliders();
        }
    }

#if UNITY_EDITOR
    const float gizmosColorAlpha = 0.75f;

    void OnDrawGizmos()
    {
        if (showRadius)
        {
            Gizmos.color = new Color(1f, 1f, 1f, gizmosColorAlpha);
            Gizmos.DrawWireSphere(transform.position, groundOverlapRadius);
        }
    }
#endif
}