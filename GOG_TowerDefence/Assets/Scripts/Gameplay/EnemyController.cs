using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Enemy;
using Assets.Scripts.Grid;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float _unitSpawnDelay = .2f;
    [SerializeField] private Transform _enemiesContainer;
    [SerializeField] private Waves _waves;

    public readonly List<Enemy> SpawnedEnemies = new List<Enemy>();

    private MatrixMap _matrixMap;
    private Action<List<Enemy>> _onEnemySpawn;
    private Action<Enemy> _onEnemyPassed;

    private readonly Dictionary<int, Enemy> _enemyTypes = new Dictionary<int, Enemy>();

    public void Init(MatrixMap matrixMap, Action<Enemy> onEnemyPassed, Action<List<Enemy>> onEnemySpawn)
    {
        _enemyTypes.Add(1, Resources.Load<Enemy>("Enemies/enemy01"));
        _enemyTypes.Add(2, Resources.Load<Enemy>("Enemies/enemy02"));

        _matrixMap = matrixMap;
        _onEnemySpawn = onEnemySpawn;
        _onEnemyPassed = onEnemyPassed;

        StartCoroutine(Co_SpawnEnemies());
    }

    private IEnumerator Co_SpawnEnemies()
    {
        foreach (var wave in _waves.dataArray)
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
        var randomX = UnityEngine.Random.Range(0, 3);
        var randomY = UnityEngine.Random.Range(0, 3);

        var newEnemy = Instantiate(enemy, Vector3.zero, Quaternion.identity, _enemiesContainer);
        newEnemy.Init(_matrixMap, _onEnemyPassed, randomX, randomY, 10, 10, arg =>
        {
            SpawnedEnemies.Remove(arg);

            Destroy(arg.gameObject);
        });

        SpawnedEnemies.Add(newEnemy);

        _onEnemySpawn(SpawnedEnemies);
    }
}