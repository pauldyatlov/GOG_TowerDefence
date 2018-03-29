using Assets.Scripts.Enemy;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _speed = 70f;

    private Vector3 _targetPosition;
    private float _damage;

    public void ShootAtTarget(Vector3 targetPosition, float damage)
    {
        _targetPosition = targetPosition;
        _damage = damage;

        var direction = targetPosition - transform.position;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void Update()
    {
        var direction = _targetPosition - transform.position;
        var frameDistance = _speed * Time.deltaTime;

        //TODO: maybe change hit to distance

        if (direction.magnitude <= frameDistance)
        {
            Destroy(gameObject);
            return;
        }

        transform.Translate(direction.normalized * frameDistance, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        var enemy = collider.GetComponent<Enemy>();

        if (enemy != null) {
            enemy.TakeDamage(_damage);
        }

        EnemyCollision();
    }

    private void EnemyCollision()
    {
        Destroy(gameObject);
    }
}