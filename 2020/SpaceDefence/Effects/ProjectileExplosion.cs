using UnityEngine;

namespace MiniGames.Games.SpaceDefence.Effects
{
    public class ProjectileExplosion : MonoBehaviour
    {
        public float destroyAfterTime = 3;

        private void Start()
        {
            Destroy(gameObject, destroyAfterTime);
        }
    }
}