using UnityEngine;
using Utils.Movers.Core;
using Utils.Movers.Core.LerpMove;
using Zenject;

namespace MiniGames.Games.SpaceDefence.Shield
{
    public class CameraShieldRotate: MonoBehaviour
    {

        private LerpJoint<Vector3> _joint;

        private void Awake()
        {
            _joint = LerpMover.AddJoint(new LerpClientEulerProxy(GetComponent<Transform>()));
        }

        public void SetRotation(Vector3 rotation)
        {
            _joint.SetTargetPosition(rotation);
        }

        private void OnDestroy()
        {
            _joint.Remove();
        }
    }
}