using Anima2D;
using DG.Tweening;
using Coconut.Core.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace MiniGames.Games.BalloonsCatcher
{
    public class BalloonView : MonoBehaviour
    {
        [SerializeField] private SpriteMeshInstance _tail;
        [SerializeField] private int _topSortOrder = 85;
        [SerializeField] private int _backSortOrder = 80;
        [SerializeField] private BalloonSkinView[] _skinViews;
        [SerializeField] private Color[] _colors;
        [SerializeField] private float _fadeDuration = 0.5f;
        [SerializeField] private float _fadeValue = 0.5f;

        private SortingGroup _sortingGroup;
        private BalloonSkinView _skinView;

        public void Init()
        {
            _skinView = _skinViews.GetRandom();
            _skinView.gameObject.SetActive(true);
            var color = _colors.GetRandom();
            _skinView.SetColor(color);
            _tail.color = color;
        }

        public void ChangeSortingOrder(bool setOnTop)
        {
            _sortingGroup.sortingOrder = setOnTop ? _topSortOrder : _backSortOrder;
        }

        public void SetNumberText(string text)
        {
            _skinView.SetNumberText(text);
        }

        public Tween Fade(bool fadeIn)
        {
            return _skinView.Fade(fadeIn ? 1f : _fadeValue, _fadeDuration);
        }

        public Tween PunchScale()
        {
            return transform.DOPunchScale(transform.localScale * 0.5f, .5f, 8, 0);
        }

        public Tween IncreaseScale()
        {
            return transform.DOScale(1.4f, .2f);
        }

        private void Awake()
        {
            _sortingGroup = GetComponent<SortingGroup>();
        }
    }
}