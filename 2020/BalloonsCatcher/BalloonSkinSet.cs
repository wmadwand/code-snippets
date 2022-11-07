using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BalloonSkinSet", menuName = "MiniGames/Balloon/BalloonSkinSet")]
public class BalloonSkinSet : ScriptableObject
{
    public Color[] colors;
    public Sprite[] sprites;
}