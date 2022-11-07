using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "ProfileCollection", menuName = "MiniGames/Likes/ProfileCollection")]
public class ProfileCollection : ScriptableObject
{
    [SerializeField] private List<Profile> _collection;

    public Profile GetProfile(int id)
    {
        return _collection.FirstOrDefault(p => p.id == id);
    }
}