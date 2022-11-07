using Coconut.Asyncs;
using MiniGames.Games.SpaceDefence.Data;
using MiniGames.Games.SpaceDefence.Game;
using MiniGames.Games.SpaceDefence.Hazard.Projectile;
using MiniGames.Games.SpaceDefence.SpawnSystem;
using MiniGames.Games.SpaceDefence.SpawnSystem.SpawnSystemBuilder;
using SpaceDefence.Core;
using SpaceDefenceGeometry;
using System;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace MiniGames.Games.SpaceDefence.Hazard.Enemy
{
    public class Enemy : MonoBehaviour, IPoolable<Vector3, IMemoryPool>
    {
        [Inject] private SpaceDefenceGameController _gameController;
        [Inject] private SpaceDefenceSoundController _soundController;

        private EnemySpot _spot;
        private EnemyAttack _attack;
        private EnemyMovement _movement;
        private EnemyView _view;
        private Action<EnemySpot, bool> _onDestroy;
        private AsyncChain _asyncChain;
        private BezierCurvesFilter _splineFilter;
        private IMemoryPool _pool;
        private EnemyModel _enemyModel;
        private SpawnSystemHelper _spawnHelper;

        //-------------------------------------------------

        public void Init(EnemySpot spot, EnemyModel enemyModel, SpawnSystemHelper spawnHelper, Action<EnemySpot, bool> callback)
        {
            _spot = spot;
            _onDestroy = callback;
            _enemyModel = enemyModel;
            _spawnHelper = spawnHelper;

            _attack.Init(_enemyModel);
            _view.Init(_enemyModel);

            transform.SetPositionAndRotation(spot.EnemyStartPosition, Quaternion.identity);

            Planner.Chain()
                   .AddAction(() => _soundController.Play(AudioName.EnemyAppear))
                   .AddTween(_movement.DoJumpTo, _spot)
                   .AddTimeout(_enemyModel.initialTime.Random())
                   .AddAction(LifeCycle);
        }

        //-------------------------------------------------

        private void Awake()
        {
            _attack = GetComponent<EnemyAttack>();
            _movement = GetComponent<EnemyMovement>();
            _view = GetComponent<EnemyView>();

            _splineFilter = new BezierCurvesFilter(new[] { _gameController.ShieldSpline });
        }

        private void Update()
        {
            _splineFilter.Projection(transform.position, out BezierCurvesProjection projection);

            var normal = projection.Normal(Vector3.forward);

            var rot = Quaternion.FromToRotation(Vector3.down, normal);
            var rot2 = Quaternion.Euler(0, transform.rotation.y, rot.eulerAngles.z);
            transform.rotation = rot2;

            Debug.DrawRay(transform.position, normal, Color.cyan);
        }

        private void OnTriggerEnter(Collider other)
        {
            var projectile = other.GetComponent<ProjectileBase>();
            if (!projectile || !projectile.IsReflected || other.GetComponent<Comet>()) return;

            _onDestroy?.Invoke(_spot, true);
            TerminateAsyncChain();
            _soundController.Play(AudioName.EnemyDestroyed);

            projectile.CreateExplosion(transform.position);
            projectile.Destroy();

            _pool.Despawn(this);
        }

        private void OnDestroy()
        {
            TerminateAsyncChain();
        }

        private void LifeCycle()
        {
            _asyncChain = Planner.Chain();

            for (int i = 0; i < _enemyModel.shotCount; i++)
            {
                _asyncChain.AddAwait(WaitForAttack)
                               .AddAction(() => _attack.IsGreenLight = false)
                               .AddAction(() => _view.ShowLaserBeam(true))
                               .AddAction(() => _soundController.Play(AudioName.EnemyShowLaser))
                               .AddTween(_view.ShowPrewarmLights)
                               .AddAction(() => _attack.Shoot())
                               .AddAction(() => _soundController.Play(AudioName.EnemyAttack))
                               .AddAction(_view.HidePrewarmLights)
                               .AddTween(_movement.DoAttackRebound)
                               .AddAction(Reset)
                               ;
            }

            _asyncChain.AddAction(Reset)
                      .AddAwait(WaitForAttack)
                      .AddAction(() => _soundController.Play(AudioName.EnemyAppear))
                      .AddTween(_movement.DoJumpBackTo, _spawnHelper.GetRandomPoint(SpawnSystemHelper.PointPosition.Top))
                      .AddAction(() =>
                      {
                          _onDestroy?.Invoke(_spot, false);
                          TerminateAsyncChain();

                          _pool.Despawn(this);
                      });
        }

        private void WaitForAttack(AsyncStateInfo state)
        {
            state.IsComplete = _attack.IsReady;
        }

        private void Reset()
        {
            _attack.Reset();
            _view.ShowLaserBeam(false);
        }

        private void TerminateAsyncChain()
        {
            _asyncChain?.Terminate();
            _asyncChain = null;
        }

        void IPoolable<Vector3, IMemoryPool>.OnDespawned()
        {
            Reset();
            _pool = null;
            TerminateAsyncChain();
            transform.DOKill();
        }

        void IPoolable<Vector3, IMemoryPool>.OnSpawned(Vector3 position, IMemoryPool pool)
        {
            transform.position = position;
            _pool = pool;
        }

        //-------------------------------------------------

        [Serializable]
        public class Settings
        {
            public RangeFloat timeBetweenShots;
            public RangeFloat prewarmTime;
            public int shotCount;
        }

        public class Factory : PlaceholderFactory<Vector3, Enemy> { }
    }
}