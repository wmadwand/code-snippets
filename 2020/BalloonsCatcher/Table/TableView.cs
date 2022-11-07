using DG.Tweening;
using Coconut.Core.Asyncs;
using TMPro;
using UnityEngine;

namespace MiniGames.Games.BalloonsCatcher
{
    public class TableView : MonoBehaviour
    {
        [SerializeField] private TextMeshPro _text;

        private string _newText;

        public void SetNumber(int value)
        {
            _newText = value.ToString();
        }
        
        public void ApplyNewText()
        {
            _text.text = _newText;
        }
    }
}