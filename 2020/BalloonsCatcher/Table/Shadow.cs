using Coconut.Core;
using UnityEngine;

namespace MiniGames.Games.BalloonsCatcher
{
    public class Shadow : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _shadow;
        [SerializeField] private Transform _bottom;

        [SerializeField] private RangeFloat _shadowRange = new RangeFloat(0, 1);

        private void Update()
        {
            var d = Vector3.Distance(_shadow.transform.position, _bottom.transform.position);
            var fadeLevel = Mathf.Clamp01(_shadowRange.UnLerp(d));
            _shadow.color = _shadow.color.WithAlpha(1-fadeLevel);
        }
    }
}