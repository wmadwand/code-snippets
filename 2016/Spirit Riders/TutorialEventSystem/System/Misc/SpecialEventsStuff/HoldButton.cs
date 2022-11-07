using UnityEngine;

public class HoldButton : MonoBehaviour
{
    [HideInInspector]
    public bool IsHold { get; private set; }

    public void OnPointerDownUp()
    {
        IsHold = !IsHold;
    }
}
