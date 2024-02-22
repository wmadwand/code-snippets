using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggableCardCatapultSelectorTutorialTarget : MonoBehaviour
{
    [SerializeField] GameObject NotMatched;
    [SerializeField] GameObject Matched;

    public void Refresh(bool match)
    {
        NotMatched.SetActive(!match);
        Matched.SetActive(match);
    }

}
