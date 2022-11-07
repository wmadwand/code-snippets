using Coconut.Core.Asyncs;
using Coconut.Game;
using System;
using Coconut.Game.Scenes;
using UnityEngine;
using Utils;
using Zenject;

namespace MiniGames.Games.FBLikesPower
{
    public class Dashboard : MonoBehaviour
    {
        //TODO: exctract ExerciseCardsPanel class
        [SerializeField] private ExerciseCard[] _cards;

        [SerializeField] private MathButtonsPanel _buttonsPanel;
        [SerializeField] private HeartResultPanel _heartResult;

        [SerializeField] private Transform _cardsStartPoint;
        [SerializeField] private Transform _leftCardEndPoint;
        [SerializeField] private Transform _rightCardEndPoint;

        [Header("Sound")]
        [SerializeField] private ShootSoundOnEvent _showPhotoSound;
        [SerializeField] private float _showPhotoSoundTimeout = 0f;

        [Space]
        [SerializeField] private ShootSoundOnEvent _hidePhotoSound;
        [SerializeField] private float _hidePhotoSoundTimeout = 0f;


        [Inject] private ISceneEvents _sceneEvents;
        //---------------------------------------------------

        public void Init(ComparePair cardsPair, params ProfileProxy[] profiles)
        {
            _cards[0].Init(cardsPair.leftExpression, PositionType.Left, profiles[0]);
            _cards[1].Init(cardsPair.rightExpression, PositionType.Right, profiles[1]);
        }

        public async Promise ShowChosenSymbol(MathButton button)
        {
            _buttonsPanel.SetActive(false);
            await _heartResult.ShowSymbolTween(button.SymbolSprite);
        }

        public async Promise ShowTutorial(Transform centerPoint)
        {
            await _heartResult.ShowTutorial(_buttonsPanel.MathButtons, centerPoint);
        }

        public async Promise ShowAsync()
        {
            _ = _cards[0].ShowTo(_leftCardEndPoint.position);
            _ = _cards[1].ShowTo(_rightCardEndPoint.position);

            _showPhotoSound.Play(_showPhotoSoundTimeout);

            await _heartResult.Show();

            _cards[0].StartFloating();
            _cards[1].StartFloating();
        }

        public async Promise PulseButtonsAsyncOnce()
        {
            await _buttonsPanel.PulseButtonsAsyncOnce();
        }

        public async Promise ShowButtonsPanel()
        {
            await _buttonsPanel.Show();

            _buttonsPanel.SetActive(true);
        }

        public async Promise HideAsync()
        {
            foreach (var card in _cards)
            {
                if (card == null)
                {
                    continue;
                }

                card.BacklightSetActive(false);
            }

            _ = _cards[0].HideTo(_cardsStartPoint.position);
            _ = _cards[1].HideTo(_cardsStartPoint.position);
            _ = _heartResult.Hide();

            _hidePhotoSound.Play(_hidePhotoSoundTimeout);

            await _buttonsPanel.Hide();
        }

        public async Promise SetCorrectAnswer(PositionType winCardPosition)
        {
            _cards[0].StopFloating();
            _cards[1].StopFloating();

            await _heartResult.SetCorrectAnswer(winCardPosition);
            await Timeline.Delay(0.5f);

            var winCards = new ExerciseCard[2];

            switch (winCardPosition)
            {
                case PositionType.Left:
                    winCards[0] = _cards[0];
                    break;
                case PositionType.Right:
                    winCards[0] = _cards[1];
                    break;
                case PositionType.Both:
                    winCards = _cards;
                    break;
            }

            foreach (var card in winCards)
            {
                if (card == null)
                {
                    continue;
                }

                card.SetWin(true);
                _ = card.PushUp2();
            }

            _buttonsPanel.SetActive(false);

            await Timeline.Delay(2);
        }

        public async Promise SetWrongAnswer()
        {
            //_cards[0].Blink();
            //_cards[1].Blink();

            _cards[0].StopFloating();
            _cards[1].StopFloating();

            _cards[0].ShakeWrongAnswer();
            _cards[1].ShakeWrongAnswer();

            await _heartResult.SetWrongAnswer();
            _buttonsPanel.SetActive(true);

            _cards[0].StartFloating();
            _cards[1].StartFloating();

        }

        //---------------------------------------------------

        private void Start()
        {
            ResetView();
        }

        public void ResetView()
        {
            Array.ForEach(_cards, card => card.ResetView(_cardsStartPoint.position));
            _buttonsPanel.ResetView();
            _heartResult.ResetView();
        }

        public void SetCardsStartPosition(Vector3 value)
        {
            Array.ForEach(_cards, card => card.ResetView(value));
        }
    }
}