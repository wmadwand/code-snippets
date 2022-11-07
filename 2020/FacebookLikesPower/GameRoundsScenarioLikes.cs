using System;
using System.Collections.Generic;
using Installers;
using Coconut.LoadSystem;
using MiniGames.Core.TaskPanel;
using MiniGames.Core.UI;
using Coconut.Core;
using Coconut.Core.Asyncs;
using Coconut.Game.Scenes;
using UnityEngine;
using Zenject;

namespace MiniGames.Core
{
    /// <summary>
    /// need IFactory<IGameScenario> injection
    /// </summary>
    public class GameRoundsScenarioLikes : GameScenario
    {
        public IProgressBar ProgressBar => _progressBar;
        public ITaskPanel TaskPanel => _taskPanel;

        [SerializeField] private int rounds;
        [Inject] private IFactory<IGameScenario> _roundsFactory;
        [InjectOptional] private IProgressBar _progressBar;
        [InjectOptional] private ITaskPanel _taskPanel;
        private readonly IDisposables _disposables = new Disposables();

        protected override async Promise OnRunScenario()
        {
            _progressBar?.SetCount(rounds);
            //_taskPanel?.ShowForDelay();
            for (var i = 0; i < rounds; i++)
            {
                await _disposables.Add(_roundsFactory.Create()).RunScenario();
                if (_progressBar != null)
                {
                    await _progressBar.Increment();
                }
            }
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
        }
    }
}