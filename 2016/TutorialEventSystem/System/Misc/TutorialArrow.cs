using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TutorialEventSystem;

public class TutorialArrow : MonoBehaviour
{
    public GameObject arrowView;

    private void Awake()
    {
        arrowView.SetActive(false);
    }

    private void Start()
    {
        TutorialEventCollection.SystemMessage += ControlArrow;
    }


    private void OnDestroy()
    {
        TutorialEventCollection.SystemMessage -= ControlArrow;
    }

    private void ControlArrow(SystemMessage msg)
    {

        if (msg == SystemMessage.NowGetWeapon)
        {
            arrowView.SetActive(!arrowView.activeInHierarchy);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            arrowView.SetActive(false);
        }
    }

}