using DG.Tweening;
using Coconut.Core.Asyncs;
using Coconut.Core.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MiniGames.Games.Bowman
{
    public class TargetsController : MonoBehaviour
    {
        [SerializeField] private Target[] _targets;
        [SerializeField] private TargetSettings _tatrgetsSettings;


        public int targetCount => _targets.Length;
        private Action<Target> _checkExerciseAnswer;
        private bool isPositionsSet;

        //---------------------------------------------------

        public void ResetTargets()
        {
            foreach (var item in _targets)
            {
                item.ResetView();
            }
        }

        public void Init(Action<Target> checkAnswerCallback, List<int> numbers)
        {
            _checkExerciseAnswer = checkAnswerCallback;

            for (var i = 0; i < _targets.Length; i++)
            {
                _targets[i].SetNumber(numbers[i]);
            }

            if (isPositionsSet)
            {
                return;
            }

            SetPositions();
        }

        public Tween ShowNumbers()
        {
            var seq = DOTween.Sequence();

            for (int i = 0; i < _targets.Length; i++)
            {
                seq.Join(_targets[i].ShowNumber());
            }

            seq.SetUpdate(UpdateType.Normal, true);

            return seq;
        }

        public async Promise RunTargets()
        {
            for (int i = 0; i < _targets.Length; i++)
            {
                _targets[i].StartMove();
            }
        }

        public void StopTargets()
        {
            for (int i = 0; i < _targets.Length; i++)
            {
                _targets[i].StopMove();
            }
        }

        //---------------------------------------------------

        //TODO: separate methods
        private void SetPositions()
        {
            ShuffleSettingsCollection(out Harmonic[] resultSettings);

            for (int i = 0; i < _targets.Length; i++)
            {
                _targets[i].Init(resultSettings[i], OnTargetClick);
            }

            isPositionsSet = true;
        }

        private void OnTargetClick(Target target)
        {
            _checkExerciseAnswer?.Invoke(target);
        }

        private void ShuffleSettingsCollection(out Harmonic[] resultSettings)
        {
            var harmonics = _tatrgetsSettings.harmonics;
            resultSettings = new Harmonic[harmonics.Length];
            harmonics.CopyTo(resultSettings, 0);
            resultSettings.Shuffle();
        }
    }
}