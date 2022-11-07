using System;
using MiniGames.Games.SpaceDefence.Hazard.Enemy;
using UnityEngine;

namespace MiniGames.Games.SpaceDefence.SpawnSystem
{
    [Serializable]
    public class EnemySpot : MonoBehaviour
    {
        public Vector3 Position => transform.position;
        public bool IsFree => Enemy == null;
        public Enemy Enemy { get; private set; }
        public Vector3 EnemyStartPosition { get; private set; }

        //-------------------------------------------------

        public void SetEnemyStartPosition(Vector3 value)
        {
            EnemyStartPosition = value;
        }

        public void SetLockedWith(Enemy enemy)
        {
            Enemy = enemy;
        }

        public void SetUnlocked()
        {
            Enemy = null;
        }
    }
}