using System;
using UnityEngine;

namespace MiniGames.Games.Bowman
{
    public class PlayerAnimations : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private string _prepareTrigger = "Prepare";
        [SerializeField] private string _fireTrigger = "Fire";
        [SerializeField] private string _cancelTrigger = "Cancel";
        [SerializeField] private string[] _hideWeaponsStates = new[] {"Idle"};
        [SerializeField] private GameObject[] _weapons;
        [SerializeField] private int _pullLevel = 1;

        public void Prepare()
        {
            _animator.ResetTrigger(_fireTrigger);
            _animator.ResetTrigger(_cancelTrigger);
            _animator.SetTrigger(_prepareTrigger);
        }

        public void Fire()
        {
            _animator.ResetTrigger(_prepareTrigger);
            _animator.ResetTrigger(_cancelTrigger);
            _animator.SetTrigger(_fireTrigger);
        }

        public void Cancel()
        {
            _animator.ResetTrigger(_prepareTrigger);
            _animator.ResetTrigger(_fireTrigger);
            _animator.SetTrigger(_cancelTrigger);
        }

        private void LateUpdate()
        {
            var stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            var isWeaponVisible = true;
            foreach (var hideWeaponsState in _hideWeaponsStates)
            {
                if (stateInfo.IsName(hideWeaponsState))
                {
                    isWeaponVisible = false;
                    break;
                }
            }

            foreach (var weapon in _weapons)
            {
                weapon.SetActive(isWeaponVisible);
            }
        }

        public void SetPullLevel(float level)
        {
            _animator.SetLayerWeight(_pullLevel, Mathf.Clamp01(level));
        }
    }
}