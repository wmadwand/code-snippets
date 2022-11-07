using System;
using UnityEngine;

namespace MiniGames.Games.BalloonsCatcher
{
    public class Table : MonoBehaviour
    {
        private TableMovement _movement;
        private TableView _view;

        private int _number;

        private void Awake()
        {
            _movement = GetComponent<TableMovement>();
            _view = GetComponent<TableView>();
        }

        public void Init(int number)
        {
            _number = number;
            _view.SetNumber(number);
        }

        public void ApplyNewText()
        {
            _view.ApplyNewText();
        }
    }
}