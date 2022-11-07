using DG.Tweening;
using MiniGames.Games.SpaceDefence.SpawnSystem;
using UnityEngine;

namespace MiniGames.Games.SpaceDefence.Hazard.Enemy
{
    public class EnemyMovement : MonoBehaviour
    {
        public float rotatingDuration = 2.5f;
        public Transform enemyViewTransfrom;

        //-------------------------------------------------

        public Tween DoAttackRebound()
        {
            return transform.DOPunchPosition(Vector3.up / 60, 3, 2, .5f);
        }

        public Tween DoJumpTo(EnemySpot point)
        {
            return transform.DOJump(point.Position, 0.1f, 1, 2.5f);
        }

        public Tween DoJumpBackTo(Vector3 position/*EnemySpot point*/)
        {
            return transform.DOJump(position/*point.EnemyStartPosition*/ /*+ new Vector3(0, .5f, 0)*/, 0.1f, 1, 2.5f);
        }

        //-------------------------------------------------

        private void Start()
        {
            SelfRotation();
        }

        private Tween SelfRotation()
        {
            return enemyViewTransfrom.DOLocalRotate(new Vector3(0f, -360f, 0f), rotatingDuration, RotateMode.LocalAxisAdd).SetEase(Ease.Linear).SetLoops(-1);
        }
    }
}