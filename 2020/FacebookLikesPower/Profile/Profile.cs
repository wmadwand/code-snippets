using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Profile", menuName = "MiniGames/Likes/Profile")]
public class Profile : ScriptableObject
{
    public int id;
    public new string name;
    public Sprite avatar;
    public Sprite[] photos;
}