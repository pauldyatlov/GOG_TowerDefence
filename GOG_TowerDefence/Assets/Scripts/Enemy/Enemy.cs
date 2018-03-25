using System;
using Assets.Scripts.Enemy;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyPathfinding _pathfinding;
    [SerializeField] private float _maxHealth = 15;

    private Action<Enemy> _onDeath;

    private float _health;
    private float Health
    {
        get { return _health; }
        set
        {
            _health = value;

            if (_health <= 0)
                Death();
        }
    }

    public void Init(Action<Enemy> onDeath)
    {
        Health = _maxHealth;
        _onDeath = onDeath;

        gameObject.SetActive(true);

        _pathfinding.Init();
    }

    public void TakeDamage(float damage)
    {
        Debug.Log("Took (" + damage + ") damage.");

        Health -= damage;
    }

    private void Death()
    {
        _onDeath(this);
    }
}