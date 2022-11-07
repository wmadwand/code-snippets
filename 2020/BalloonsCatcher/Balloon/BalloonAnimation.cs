using UnityEngine;

namespace MiniGames.Games.BalloonsCatcher
{
    [RequireComponent(typeof(Animator))]
    public class BalloonAnimation : MonoBehaviour
    {
        [SerializeField] private string _stopAnimationTrigger = "StopAnimation";

        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void StopAnimation()
        {
            _animator.SetTrigger(_stopAnimationTrigger);
        }

        public void Play(BalloonTriggerState triggerState)
        {
            _animator.SetTrigger(triggerState.ToString());
        }

    }
}