using DG.Tweening;
using TMPro;
using UnityEngine;

namespace MiniGames.Games.BalloonsCatcher
{
    public class BalloonSkinView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _sprite;
        [SerializeField] private TextMeshPro _text;

        public void SetColor(Color color)
        {
            _sprite.color = color;
        }

        public void SetMaterial(Material material)
        {
            _sprite.material = material;
        }

        public Tween Fade(float fadeValue, float duration)
        {
            var seq = DOTween.Sequence();
            seq.Join(_sprite.DOFade(fadeValue, duration));
            seq.Join(_text.DOFade(fadeValue, duration));
            return seq;
        }

        public void SetNumberText(string text)
        {
            _text.text = text;
        }
    }
}