using Coconut.Core.Collections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonSkinManager : MonoBehaviour
{
    public BalloonSkinSet skinSet;

    List<BalloonSkin> balloonSkins = new List<BalloonSkin>();

    public BalloonSkin GetRandomSkin()
    {
        return balloonSkins.GetRandom();
    }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        foreach (var color in skinSet.colors)
        {
            foreach (var sprite in skinSet.sprites)
            {
                var skin = new BalloonSkin(color, sprite);

                balloonSkins.Add(skin);
            }
        }

        balloonSkins.Shuffle();
    }

    public struct BalloonSkin
    {
        public Color color;
        public Sprite sprite;

        public BalloonSkin(Color color, Sprite sprite)
        {
            this.color = color;
            this.sprite = sprite;
        }
    }
}