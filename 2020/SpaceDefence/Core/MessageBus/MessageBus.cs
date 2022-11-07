using UnityEngine;

namespace MiniGames.Games.SpaceDefence.Core.MessageBus
{
    public sealed class MessageBus
    {
        public readonly Message<Transform> HazardDestroyed = new Message<Transform>();
        public readonly Message<int> PlayerDamaged = new Message<int>();
        public readonly Message<int, Transform> PlayerGetHealth = new Message<int, Transform>();        
    }
}