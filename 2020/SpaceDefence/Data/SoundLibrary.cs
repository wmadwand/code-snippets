using Coconut.SoundSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MiniGames.Games.SpaceDefence.Game
{
    public enum AudioName
    {
        MusicTheme = 10,
        PopUpAppear = 20,
        Progress = 30,
        PopUpHide = 40,
        EnemyAppear = 50,
        EnemyShowLaser = 60,
        EnemyDestroyed = 70,
        EnemyAttack = 80,
        CometAppear = 90,
        PlayerDamaged = 100,
        ShieldReflectProjectile = 110,
        ShieldReflectComet = 120,
        ShieldReflectRemedy = 130,
        BrokenHeart = 140,
        RecoverHeart = 150,
        ShieldMovement = 160,
        WinGame = 200
    }

    //-------------------------------------------------

    [Serializable]
    public class SoundLibrary
    {
        [SerializeField] private List<Audio> _data;

        public AudioModel GetSound(AudioName name)
        {
            return _data.FirstOrDefault(i => i.name == name).model;
        }
    }

    //-------------------------------------------------    

    [Serializable]
    public struct Audio
    {
        public AudioName name;
        public AudioModel model;
    }
}