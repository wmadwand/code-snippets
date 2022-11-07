
using MiniGames.Games.Bowman.Shooting;
using UnityEngine;

namespace MiniGames.Games.Bowman
{
    public class Player : MonoBehaviour
    {

        private PlayerShooting _shooting;

        private void Awake()
        {
            _shooting = GetComponent<PlayerShooting>();
        }

        public void SetShootingActive(bool value)
        {
            _shooting.SetActive(value);
        }

        public void ResetView()
        {
            _shooting.Weapon.DestroyFiredProjectiles();
        }
    }
}