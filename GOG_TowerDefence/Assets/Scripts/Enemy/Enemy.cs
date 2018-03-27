using System;
using Assets.Scripts.Grid;
using UnityEngine;

namespace Assets.Scripts.Enemy
{
    public enum EPathFormula
    {
        Eucledian,
        EucledianSquared,
        Diagonal,
        Manhatten
    }

    public class Enemy : MonoBehaviour
    {
        public class EnemyModel
        {
            public float MaxHealth;
            public float MoveSpeed;
            public int Reward;

            public EnemyModel(float maxHealth, float moveSpeed, int reward)
            {
                MaxHealth = maxHealth;
                MoveSpeed = moveSpeed;
                Reward = reward;
            }
        }

        [SerializeField] private EnemyPathfinding _pathfinding;
        [SerializeField] private EPathFormula _formula;
        [SerializeField] private SpriteRenderer _healthBar;

        [SerializeField] private float _maxHealth = 15;
        [SerializeField] private float _moveSpeed = 1;
        [SerializeField] private int _reward = 1;

        public int Reward
        {
            get { return _reward; }
            private set { _reward = value; }
        }

        private Action<Enemy> _onDeath;

        private float _health;
        private float Health
        {
            get { return _health; }
            set
            {
                _healthBar.transform.localScale = new Vector3(1.5f * value / _maxHealth, 3, 1);

                _health = value;
                
                if (_health <= 0)
                    Death();
            }
        }

        public void SetParameters(EnemyModel parameters)
        {
            _maxHealth = parameters.MaxHealth;
            _moveSpeed = parameters.MoveSpeed;

            Reward = parameters.Reward;
        }

        public void Init(MatrixMap matrixMap, Action<Enemy> onEnemyPassed, float x, float y, int targetX, int targetY, Action<Enemy> onDeath)
        {
            Health = _maxHealth;
            _onDeath = onDeath;

            _pathfinding.Init(matrixMap, () => { onEnemyPassed(this); }, _formula, _moveSpeed, x, y, targetX, targetY);
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
}