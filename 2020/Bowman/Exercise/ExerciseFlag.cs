using DG.Tweening;
using ExerciseGeneration;
using TMPro;
using UnityEngine;

namespace MiniGames.Games.Bowman
{
    public class ExerciseFlag : MonoBehaviour
    {
        [SerializeField] private GameObject _view;
        [SerializeField] private TextMeshPro _text;
        [SerializeField] private SpriteRenderer _shadow;
        [SerializeField] private float _animationSpeed;
        private Expression _expression;
        private Vector3 _scaleOrigin;
        private Vector3 _rotationOrigin;

        public void Init(Expression expression)
        {
            //SetTextActive(false);
            _expression = expression;
            _text.text = $"<size=100%>{expression.x}<voffset=0.15em><size=50%> \u2022 </voffset><size=100%>{expression.y}";
        }

        public Tween Transition(Transform parent, bool isBackTransition)
        {
            transform.SetParent(parent);

            var scale = isBackTransition ? _scaleOrigin : Vector3.one;
            var rotation = isBackTransition ? _rotationOrigin : Vector3.zero;
            var shadowFade = isBackTransition ? 0 : 1;
            var speed = isBackTransition ? _animationSpeed /*/ 2*/ : _animationSpeed;

            return DOTween.Sequence()
                .Append(transform.DOLocalMove(Vector3.zero, speed))
                .Join(transform.DOScale(scale, speed))
                .Join(transform.DOLocalRotate(rotation, speed))
                .Join(_shadow.DOFade(shadowFade, speed))
                .SetUpdate(UpdateType.Normal, true)
                ;
        }

        public void SetTextActive(bool value)
        {
            _text.enabled = value;
        }

        public void ResetView()
        {
            _text.enabled = false;
        }

        public void SetActive(bool value)
        {
            _view.SetActive(value);
        }

        private void Awake()
        {
            _scaleOrigin = transform.localScale;
            _rotationOrigin = transform.localRotation.eulerAngles;
        }
    }
}