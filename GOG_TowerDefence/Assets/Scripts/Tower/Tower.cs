using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField] private Projectile _projectile;
    [SerializeField] private Transform _barrel;
    [SerializeField] private Transform _firePoint;

    [SerializeField] private float _shootDistance = 10f;
    [SerializeField] private float _fireRate = 1f;
    [SerializeField] private float _turnSpeed = 1f;

    private List<Enemy> _enemies = new List<Enemy>();

    private float _fireCountdown;
    private Enemy _targetEnemy;

    private void Awake()
    {
        //todo: change to coroutine + init
        InvokeRepeating("UpdateTargetEnemy", 0, .5f);
    }

    public void UpdateEnemiesList(List<Enemy> enemies)
    {
        _enemies = enemies;
    }

    private void UpdateTargetEnemy()
    {
        var shortestDistance = Mathf.Infinity;
        Enemy nearestEnemy = null;

        foreach (var enemy in _enemies)
        {
            var distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy != null && shortestDistance <= _shootDistance)
            _targetEnemy = nearestEnemy;
    }

    private void Update()
    {
        _fireCountdown -= Time.deltaTime;

        if (_targetEnemy == null)
            return;
        
        var dir = _targetEnemy.transform.position - transform.position;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        var lookRotation = Quaternion.AngleAxis(angle, Vector3.forward);

        _barrel.transform.rotation = Quaternion.Lerp(_barrel.rotation, lookRotation, Time.deltaTime * _turnSpeed);

        if (_fireCountdown <= 0f)
        {
            Shoot();
            _fireCountdown = 1f / _fireRate;
        }
    }

    private void Shoot()
    {
        var bullet = Instantiate(_projectile, _firePoint.position, _firePoint.rotation);

        bullet.ShootAtTarget(_firePoint.position + _firePoint.transform.forward * 50f);
    }
}