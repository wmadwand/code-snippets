using System.Collections.Generic;
using System.Linq;
using MiniGames.Games.SpaceDefence.Core;
using UnityEngine;

namespace MiniGames.Games.SpaceDefence.SpawnSystem
{
    public class EnemySpotCollection : MonoBehaviour
    {
        public EnemySpot[] Spots => _spots;
        [SerializeField] private EnemySpot[] _spots;

        public void SetActive(bool value)
        {
            gameObject.SetActive(value);
        }

        public IEnumerable<EnemySpot> GetFreeSpots()
        {
            return _spots.Where(item => item.IsFree);
        }

        public EnemySpot GetRandomFreeSpot()
        {
            return GetFreeSpots().Count() > 0 ? GetFreeSpots().GetRandomItem() : null;
        }
    }
}