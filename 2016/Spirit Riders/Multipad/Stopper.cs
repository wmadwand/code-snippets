using UnityEngine;
using System.Collections.Generic;
using System;

public class Stopper : Multipad
{
    [SerializeField]
    bool resumeWithJump;

    public override void OnTriggerExit2D(Collider2D other)
    {
		if (other.CompareTag("Player") && other is CapsuleCollider2D)
        {
            /*GameController.Singleton.GetClientController(other.name).*/ _unitControllerV2.SetIsIntoMultiPadCol(_auto: false);
        }
    }

	public override void ProcessCollide(Collider2D other)
	{
		bool _isPlayer = other.CompareTag("Player");

		if (_isPlayer)
		{
			_unitControllerV2 = GameController.Singleton.GetClientController(other.name)._unitControllerV2;

//			if (!_unitControllerV2.IsJumpClickedNow())
//			{
//				return;
//			}

			_rigidbody2D = other.GetComponent<Rigidbody2D>();
			_unitSettingsV2 = GameController.Singleton.GetClientSettings(other.name);

			if (_rigidbody2D != null && _unitControllerV2 != null)
			{
				RunCurrentAction();
			}
		}
	}

    public override void RunCurrentAction()
    {
//        if (_unitControllerV2.IsJumpClickedNow())
//        {
		if (!_unitControllerV2.stopper && _unitControllerV2.GetIsIntoMultiPadCol() && !_unitControllerV2.IsJumpClickedNow())
            {
                StopMovement();
            }

		if (_unitControllerV2.IsJumpClickedNow())
            {
                ResumeMovement();
            }
//        }
    }

    private void StopMovement()
    {
        _unitControllerV2.DisableMovement("Stopper");
    }

    private void ResumeMovement()
    {
        if (resumeWithJump)
        {
            _unitControllerV2.SetIsIntoMultiPadCol();
            _unitControllerV2.EnableMovement();
            _unitControllerV2.JumpButtonDown();
            _unitControllerV2.ResetJumpClickTime();
        }
        else
        {
			_unitControllerV2.SetIsIntoMultiPadCol();
			_unitControllerV2.EnableMovement();
        }
    }
}