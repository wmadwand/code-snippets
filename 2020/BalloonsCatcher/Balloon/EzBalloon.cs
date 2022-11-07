using Coconut.Core.Asyncs;
using Coconut.Game;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace MiniGames.Games.BalloonsCatcher
{
    public class EzBalloon : MonoBehaviour, IPointerDownHandler
    {
        public int number { get; private set; }

        [Inject] private ExerciseController _exerciseController;

        private BalloonMovement _movement;
        private BalloonView _view;
        private BalloonAnimation _animation;
        private bool _isClickAllowed = true;

        private void Awake()
        {
            _movement = GetComponent<BalloonMovement>();
            _view = GetComponent<BalloonView>();
            _animation = GetComponent<BalloonAnimation>();
            number = _exerciseController.Dequeue();
            _view.Init();
            _view.SetNumberText(number.ToString());
            gameObject.Promise(LifeCycle);
        }

        private async Promise LifeCycle()
        {
            await _movement.Appear();
            await Timeline.Delay(0.5f);
            await _movement.MoveUp();
            Destroy(gameObject);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_isClickAllowed) return;
            _isClickAllowed = false;
            _movement.KillMoveUpTween();
            gameObject.CancelPromise(LifeCycle);
            _exerciseController.ReportClick(this);
        }

        public void StopAnimation()
        {
            _animation.StopAnimation();
        }

        public void ChangeSortingOrder()
        {
            _view.ChangeSortingOrder(false);
        }
    }
}