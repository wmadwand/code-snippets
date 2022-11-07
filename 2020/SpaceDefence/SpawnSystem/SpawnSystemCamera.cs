using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSystemCamera : MonoBehaviour
{
    public Camera Camera { get; private set; }

    private void Awake()
    {
        Camera = GetComponent<Camera>();
        Camera.orthographicSize = 0.20f;
    }
}