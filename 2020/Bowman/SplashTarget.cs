using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SplashTarget : MonoBehaviour
{
    [SerializeField] private TextMeshPro _text;

    public void SetNumber(string number)
    {
        _text.text = number;
    }
}
