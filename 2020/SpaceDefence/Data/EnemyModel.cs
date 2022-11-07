using SpaceDefence.Core;
using UnityEngine;

namespace MiniGames.Games.SpaceDefence.Data
{
    [CreateAssetMenu(menuName = "MiniGames/SpaceDefence/EnemyModel", fileName = "SpaceDefenceEnemyModel")]
    public class EnemyModel : HazardModelBase
    {
        public RangeFloat initialTime;
        public RangeFloat prewarmTime;
        public RangeFloat afterAttackTime;
        public RangeFloat projectileSpeed;
        public int shotCount;
    }

    public class HazardModelBase : ScriptableObject { }
}