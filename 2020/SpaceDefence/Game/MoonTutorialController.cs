using Dollhouse.Tutorials.Finger;
using Coconut.Asyncs;
using UnityEngine;

namespace MiniGames.Games.SpaceDefence.Game
{
    public class SpaceDefenceTutorialController : MonoBehaviour
    {
        public Transform shield;
        public Transform leftPoint;
        public Transform rightPoint;
        public VirtualFinger virtualFinger;
        
        public AsyncState RunTutorial()
        {
            return Planner.Chain()
                    .AddTimeout(1f)
                    .AddFunc(virtualFinger.ShowDrag, shield, leftPoint)
                    .AddTimeout(1f)
                    .AddFunc(virtualFinger.ShowDrag, shield, rightPoint)
                    .AddTimeout(1f)
                ;
        }
    }
}