using UnityEngine;
using System.Collections.Generic;
using System;

public class JumpPadAction : Multipad
{
    [SerializeField]
    float jumpPower = 30f;

    [SerializeField]
    Transform directionPoint;

    private Animator _animator;

    private void Start()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    public override void OnTriggerEnter2D(Collider2D other)
    {
        SetPlayerIntoMultiPadCol(other);
        _animator.SetBool("Touched", true);
    }

    public override void RunCurrentAction()
    {
        _unitControllerV2.DisableMovement("JumpPadAction");

        Vector2 Point_vec = new Vector2(directionPoint.localPosition.x, directionPoint.localPosition.y);
        Point_vec.Normalize();
        _rigidbody2D.AddForce(Point_vec * jumpPower, ForceMode2D.Impulse);

        _animator.SetBool("Activated", true);
    }
}