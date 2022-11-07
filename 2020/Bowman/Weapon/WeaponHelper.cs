using UnityEngine;

public class WeaponHelper : MonoBehaviour
{
    public Transform ProjectileHolder => _projectileHolder;
    public Transform ProjectileSpawnPoint => _projectileSpawnPoint;
    public Transform ProjectileShotPoint => _projectileShotPoint;

    [SerializeField] private Transform _projectileSpawnPoint;
    [SerializeField] private Transform _projectileHolder;
    [SerializeField] private Transform _projectileShotPoint;
}
