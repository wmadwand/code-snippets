using UnityEngine;
using System.Collections.Generic;

public abstract class Multipad : MonoBehaviour
{
    protected UnitControllerV2 _unitControllerV2;
    protected Rigidbody2D _rigidbody2D;
    protected UnitSettingsV2 _unitSettingsV2;

    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        SetPlayerIntoMultiPadCol(other);
    }

    public virtual void OnTriggerStay2D(Collider2D other)
    {
        ProcessCollide(other);
    }

    public virtual void OnTriggerExit2D(Collider2D other)
    {
        SetPlayerIntoMultiPadCol(other);
    }

    protected void SetPlayerIntoMultiPadCol(Collider2D other)
    {
        if (other.CompareTag("Player") && other is CapsuleCollider2D)
        {
            GameController.Singleton.GetClientController(other.name)._unitControllerV2.SetIsIntoMultiPadCol();
        }
    }

    public virtual void ProcessCollide(Collider2D other)
    {
        bool _isPlayer = other.CompareTag("Player");

        if (_isPlayer)
        {
            _unitControllerV2 = GameController.Singleton.GetClientController(other.name)._unitControllerV2;

            if (!_unitControllerV2.IsJumpClickedNow())
            {
                return;
            }

            _rigidbody2D = other.GetComponent<Rigidbody2D>();
            _unitSettingsV2 = GameController.Singleton.GetClientSettings(other.name);

            if (_rigidbody2D != null && _unitControllerV2 != null)
            {
                RunCurrentAction();
            }
        }
    }

    public abstract void RunCurrentAction();
}