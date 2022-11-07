using SpaceDefence.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MiniGames.Games.SpaceDefence.Data
{
    [CreateAssetMenu(menuName = "MiniGames/SpaceDefence/HazardSpawnModel", fileName = "SpaceDefenceHazardSpawnModel")]
    public class HazardSpawnModel : ScriptableObject
    {
        public HazardModelBase hazardType;
        public RangeFloat timeout;
        public int count;
        public RangeFloat interval;
        public RangeFloat limit;
    }
}