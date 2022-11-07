using SpaceDefence.Core;
using UnityEngine;

namespace MiniGames.Games.SpaceDefence.Data
{
    [CreateAssetMenu(menuName = "MiniGames/SpaceDefence/RemedyModel", fileName = "SpaceDefenceRemedyModel")]
    public class RemedyModel : HazardModelBase
    {
        public RangeFloat speed;
        public int spawnChancePercent;
    }
}