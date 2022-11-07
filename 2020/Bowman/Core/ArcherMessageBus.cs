using Coconut.Game.Patterns;
using UnityEngine.EventSystems;

namespace MiniGames.Games.Bowman
{
    public sealed class BowmanMessageBus
    {
        public readonly Message correctAnswer = new Message();
        public readonly Message<Target> targetClick = new Message<Target>();
        public readonly Message<PointerEventData> weaponDrag = new Message<PointerEventData>();
        public readonly Message<PointerEventData> weaponEndDrag = new Message<PointerEventData>();
    } 
}