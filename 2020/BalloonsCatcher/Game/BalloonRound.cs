using MiniGames.Core;
using MiniGames.Core.Signals;
using Coconut.Core.Asyncs;
using Zenject;

namespace MiniGames.Games.BalloonsCatcher
{
    public class BalloonRound: IGameScenario
    {
        [Inject] private IEmitter _emitter;
        [Inject] private ExerciseController _taskController;
        
        public async Promise RunScenario()
        {
            _taskController.GenerateExercise();
            await _taskController.Intro();
            _emitter.Play();
            await _taskController.WaitCompleteRound();
            _emitter.Stop();
        }

    }
}