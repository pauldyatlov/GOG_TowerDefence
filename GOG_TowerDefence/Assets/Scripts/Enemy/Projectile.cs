using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _damage = 10f;
    [SerializeField] private float _speed = 70f;

    private Vector3 _targetPosition;

    public void ShootAtTarget(Vector3 targetPosition)
    {
        _targetPosition = targetPosition;
    }

    private void Update()
    {
        var direction = _targetPosition - transform.position;
        var frameDistance = _speed * Time.deltaTime;

        if (direction.magnitude <= frameDistance)
        {
            Destroy(gameObject);
            return;
        }

        transform.Translate(direction.normalized * frameDistance, Space.World);

        transform.rotation = Quaternion.identity;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        var enemy = collider.GetComponent<Enemy>();

        if (enemy != null)
        {
            enemy.TakeDamage(_damage);

            EnemyCollision();
        }
    }

    private void EnemyCollision()
    {
        Destroy(gameObject);
    }
}