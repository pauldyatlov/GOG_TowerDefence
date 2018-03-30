using Assets.Scripts.Enemy;
using Assets.Scripts.Utils;
using UnityEngine;

public class Projectile : PoolObject
{
    [SerializeField] private float _speed = 70f;

    private Vector3 _targetPosition;
    private Enemy _target;
    private float _damage;

    public void ShootAtTarget(Vector3 targetPosition, Enemy target, float damage)
    {
        _targetPosition = targetPosition;
        _target = target;
        _damage = damage;

        var direction = target.transform.position - transform.position;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void Update()
    {
        var direction = _target.transform.position - transform.position;
        var frameDistance = _speed * Time.deltaTime;

        //TODO: maybe change hit to distance

        if (direction.magnitude <= frameDistance)
        {
            _target.TakeDamage(_damage);
            Push();

            return;
        }

        transform.Translate(direction.normalized * frameDistance, Space.World);
    }
}