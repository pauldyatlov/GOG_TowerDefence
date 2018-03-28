using System.Collections.Generic;
using Assets.Scripts.Enemy;
using Assets.Scripts.Grid;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public class TowerModel
    {
        public Tower TowerTemplate;
        public string Id;
        public bool Main;
        public float Damage;
        public float ShootDistance;
        public float FireRate;
        public float TurnSpeed;
        public int Price;
        public Sprite Icon;
        public string[] Upgrades;

        public TowerModel(Tower towerTemplate, string id, bool main, float damage, float shootDistance, float fireRate, float turnSpeed, int price, Sprite icon, string[] upgrades)
        {
            TowerTemplate = towerTemplate;
            Id = id;
            Main = main;
            Damage = damage;
            ShootDistance = shootDistance;
            FireRate = fireRate;
            TurnSpeed = turnSpeed;
            Price = price;
            Icon = icon;
            Upgrades = upgrades;
        }
    }
    
    [SerializeField] private Projectile _projectile;
    [SerializeField] private Transform _barrel;
    [SerializeField] private Transform _firePoint;

    [SerializeField] private float _damage = 10f;
    [SerializeField] private float _shootDistance = 10f;
    [SerializeField] private float _fireRate = 1f;
    [SerializeField] private float _turnSpeed = 1f;
    [SerializeField] private int _price = 1;

    public int Price
    {
        get { return _price; }
        private set { _price = value; }
    }

    public TowerModel Model { get; private set; }
    public List<TowerModel> Upgrades { get; private set; }
    public GridElement ParentGridElement { get; set; }

    private List<Enemy> _enemies = new List<Enemy>();

    private float _fireCountdown;
    private Enemy _targetEnemy;

    public void SetParameters(TowerModel parameters, List<TowerModel> upgrades)
    {
        Model = parameters;

        _damage = parameters.Damage;
        _shootDistance = parameters.ShootDistance;
        _fireRate = parameters.FireRate;
        _turnSpeed = parameters.TurnSpeed;

        Price = parameters.Price;

        Upgrades = upgrades;
    }

    public void Init()
    {
        //todo: change to coroutine
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

        bullet.ShootAtTarget(_firePoint.position + _firePoint.transform.forward * 50f, _damage);
    }
}