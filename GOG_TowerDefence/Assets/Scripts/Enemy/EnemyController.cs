using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Enemy;
using Assets.Scripts.Grid;
using UnityEngine;
using UnityEngine.Assertions;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float _unitSpawnDelay = .2f;
    [SerializeField] private Transform _enemiesContainer;

    [SerializeField] private Waves _wavesData;
    [SerializeField] private Enemies _enemiesData;

    public readonly List<Enemy> SpawnedEnemies = new List<Enemy>();

    private MatrixMap _matrixMap;
    private Action<Enemy> _onEnemyPassed;
    private Action<Enemy> _onEnemyDead;
    private Action<List<Enemy>> _onEnemySpawn;

    private readonly Dictionary<int, Enemy> _enemyTypes = new Dictionary<int, Enemy>();

    public void Init(MatrixMap matrixMap, Action<Enemy> onEnemyPassed, Action<Enemy> onEnemyDead, Action<List<Enemy>> onEnemySpawn)
    {
        _matrixMap = matrixMap;
        _onEnemySpawn = onEnemySpawn;
        _onEnemyDead = onEnemyDead;
        _onEnemyPassed = onEnemyPassed;

        foreach (var enemyData in _enemiesData.dataArray)
        {
            var enemy = Resources.Load<Enemy>("Enemies/enemy" + enemyData.KEY);
            Assert.IsTrue(enemy != null);

            enemy.SetParameters(new Enemy.EnemyModel(enemyData.Maxhealth, enemyData.Movespeed, enemyData.Reward));

            int index;
            int.TryParse(enemyData.KEY, out index);

            _enemyTypes.Add(index, enemy);
        }

        StartCoroutine(Co_SpawnEnemies());
    }

    private IEnumerator Co_SpawnEnemies()
    {
        foreach (var wave in _wavesData.dataArray)
        {
            foreach (var enemy in wave.Waveformula)
            {
                if (_enemyTypes.ContainsKey(enemy))
                {
                    SpawnEnemy(_enemyTypes[enemy]);
                }
                else
                {
                    Debug.LogError("Enemy types does not contain [" + enemy + "]!");
                }

                yield return new WaitForSeconds(_unitSpawnDelay);
            }

            yield return new WaitForSeconds(wave.Timebeforedeploy);
        }
    } 

    private void SpawnEnemy(Enemy enemy)
    {
        var randomX = UnityEngine.Random.Range(1, 3);
        var randomY = UnityEngine.Random.Range(1, 3);

        var newEnemy = Instantiate(enemy, Vector3.zero, Quaternion.identity, _enemiesContainer);
        newEnemy.Init(_matrixMap, arg =>
        {
            RemoveEnemy(arg);

            _onEnemyPassed(arg);
        }, 
        randomX, randomY, 10, 10, arg =>
        {
            RemoveEnemy(arg);

            _onEnemyDead(arg);
        });

        SpawnedEnemies.Add(newEnemy);

        _onEnemySpawn(SpawnedEnemies);
    }

    private void RemoveEnemy(Enemy enemy)
    {
        SpawnedEnemies.Remove(enemy);
        Destroy(enemy.gameObject);
    }
}