using System.Collections;
using System.Collections.Generic;
using SpaceDefence.Core;
using UnityEngine;

namespace MiniGames.Games.SpaceDefence.Data
{
    [CreateAssetMenu(menuName = "MiniGames/SpaceDefence/CometModel", fileName = "SpaceDefenceCometModel")]
    public class CometModel : HazardModelBase
    {
        public RangeFloat speed;
    }

    public enum CometType
    {
        Slow, Normal, Fast
    }
}