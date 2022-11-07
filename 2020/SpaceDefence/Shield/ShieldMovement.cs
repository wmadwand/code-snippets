using MiniGames.Games.SpaceDefence.Game;
using UnityEngine;
using Utils.Movers.Core;
using Utils.Movers.Core.LerpMove;
using Zenject;
using DG.Tweening;
using UnityEngine.EventSystems;
using Coconut.Asyncs;
using Utils.Movers;

namespace MiniGames.Games.SpaceDefence.Shield
{
    public class ShieldMovement : MonoBehaviour
    {
        [Inject] private SpaceDefenceSoundController _soundController;

        [SerializeField] private float _lerpK = 10f;
        [SerializeField] private LerpMoverController _lerpMover;

        private Collider _collider;
        private readonly float _boundsOffset = .095f;
        private LerpJoint<Vector3> _moveJoint;
        private Vector3 _posFinal;

        float soundInterval;

        //-------------------------------------------------

        public void MoveTo(Vector3 position, PointerEventData eventData)
        {
            if (_moveJoint == null) return;

            var pos01 = new Vector3(position.x, transform.position.y, transform.position.z);
            var pos02 = Camera.main.WorldToViewportPoint(pos01);
            var width = _collider.bounds.size.x / 2 + _boundsOffset;
            pos02.x = Mathf.Clamp(pos02.x, width, 1 - width);
            _posFinal = Camera.main.ViewportToWorldPoint(pos02);

            _moveJoint.target.lerpPosition = _posFinal;

            soundInterval = 1 - Mathf.Clamp(eventData.delta.x, 0, .7f);

            Debug.Log($"soundInterval {soundInterval}");

            if (eventData.dragging)
            {
                return;
            }

            _soundController.Play(AudioName.ShieldMovement);
        }

        public void TerminateMoveJoint()
        {
            _moveJoint?.Terminate();
            _moveJoint = null;
        }

        //-------------------------------------------------

        Sequence soundSequence;

        public void OnEndDrag()
        {
            soundSequence.Pause();
            //soundChain.Terminate();
        }

        public void OnBeginDrag()
        {
            soundSequence.Play();
            //SoundLoop();
        }

        private void InitSoundTween()
        {
            soundSequence = DOTween
                .Sequence()
                .AppendCallback(() => _soundController.Play(AudioName.ShieldMovement))
                .AppendInterval(.5f)
                .SetLoops(-1);

            soundSequence.Pause();
        }

        AsyncChain soundChain;

        void SoundLoop()
        {
            soundChain =  Planner.Chain()
                .AddAction(() => _soundController.Play(AudioName.ShieldMovement))
                .AddTimeout(soundInterval)
                .AddAction(SoundLoop)
                ;
        }

        private void Awake()
        {
            _collider = GetComponent<Collider>();

            _moveJoint = _lerpMover.lerpMoveService.AddJoint(new LerpClientTransform(transform));
            _moveJoint.config.k = _lerpK;

            InitSoundTween();
        }

        private void OnDestroy()
        {
            TerminateMoveJoint();
        }
    }
}