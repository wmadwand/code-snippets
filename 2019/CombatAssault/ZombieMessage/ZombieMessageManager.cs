using UnityEngine;
using Zenject;

namespace ZombieMessage
{
	public class ZombieMessageManager : MonoBehaviour
	{
		[Inject] private ZombieMessageService _messageService;

		private void Awake()
		{
			Enemy.OnDie += Enemy_OnDie;
			MissionController.OnCountdown += MissionController_OnCountdown;
			PlayerHealth.OnDie += PlayerHealth_OnDie;
			EnemySpawn.OnEnemiesSpawned += EnemySpawn_OnEnemiesSpawned;
			EnemySpawn.OnAllEnemiesDestroy += EnemySpawn_OnAllEnemiesDestroy;
		}

		private void EnemySpawn_OnAllEnemiesDestroy()
		{
			object[] settings = { ZombieMessageType.Generic, "Mission complete!" };
			_messageService.OnGetNotified(settings);
		}

		private void EnemySpawn_OnEnemiesSpawned(System.Collections.Generic.Dictionary<SpawnPoint, Enemy> obj)
		{
			object[] settings = { ZombieMessageType.Generic, "Mission start!" };
			_messageService.OnGetNotified(settings);
		}

		private void PlayerHealth_OnDie()
		{
			object[] settings = { ZombieMessageType.Generic, "Player is dead" };
			_messageService.OnGetNotified(settings);
		}

		private void MissionController_OnCountdown(float obj)
		{
			object[] settings = { ZombieMessageType.NextWaveCountdown, obj };
			_messageService.OnGetNotified(settings);
		}

		private void Enemy_OnDie(Enemy obj)
		{
			object[] settings = { ZombieMessageType.TeamPlayerDead, obj.Poi.Letter };
			_messageService.OnGetNotified(settings);
		}

		private void OnDestroy()
		{
			Enemy.OnDie -= Enemy_OnDie;
			MissionController.OnCountdown -= MissionController_OnCountdown;
			PlayerHealth.OnDie -= PlayerHealth_OnDie;
			EnemySpawn.OnEnemiesSpawned -= EnemySpawn_OnEnemiesSpawned;
			EnemySpawn.OnAllEnemiesDestroy -= EnemySpawn_OnAllEnemiesDestroy;
		}
	}
}