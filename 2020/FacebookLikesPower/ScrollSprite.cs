using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollSprite : MonoBehaviour
{
    Renderer renderer;
    public float speed;    

    void Start()
    {
        renderer = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        Vector2 textureOffset = new Vector2(Time.time * speed, 0);
        renderer.material.mainTextureOffset = textureOffset;
    }
}
