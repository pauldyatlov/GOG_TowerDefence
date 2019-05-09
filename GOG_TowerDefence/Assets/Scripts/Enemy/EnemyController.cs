using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Enemy;
using Assets.Scripts.Grid;
using Assets.Scripts.Utils;
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
    private GameController _gameController;

    private readonly Dictionary<int, Enemy> _enemyTypes = new Dictionary<int, Enemy>();

    public int WavesCount
    {
        get { return _wavesData.dataArray.Length; }
    }

    public void Init(MatrixMap matrixMap, GameController gameController)
    {
        _matrixMap = matrixMap;
        _gameController = gameController;

        foreach (var enemyData in _enemiesData.dataArray)
        {
            var enemy = Resources.Load<Enemy>("Enemies/enemy" + enemyData.KEY);
            Assert.IsTrue(enemy != null);

            enemy.SetParameters(new Enemy.EnemyModel(enemyData.Maxhealth, enemyData.Movespeed, enemyData.Reward, enemyData.Flying));

            int index;
            int.TryParse(enemyData.KEY, out index);

            _enemyTypes.Add(index, enemy);
        }
    }

    public void SpawnNextWave(int wave)
    {
        StartCoroutine(Co_SpawnEnemies(wave));
    }

    private IEnumerator Co_SpawnEnemies(int wave)
    {
        foreach (var enemy in _wavesData.dataArray[wave].Waveformula)
        {
            if (_enemyTypes.ContainsKey(enemy))
            {
                SpawnEnemy(_enemyTypes[enemy]);
            }
            else
            {
                Debug.LogError("Enemy types does not contain [" + enemy + "]!");
            }

            yield return new WaitForSeconds(0f);
        }

        Debug.Log("Wave [" + wave + "]. Spawn enemies completed. Count: " + SpawnedEnemies.Count);
    }

    private void SpawnEnemy(Enemy enemy)
    {
        var randomX = UnityEngine.Random.Range(1, 3);
        var randomY = UnityEngine.Random.Range(1, 3);

        var newEnemy = Pool.PopOrCreate(enemy, Vector3.zero, Quaternion.identity, _enemiesContainer);

        newEnemy.Init(_matrixMap, arg =>
        {
            if (RemoveEnemy(arg))
            {
                _gameController.EnemyPassed(arg);

                Debug.Log("Enemy passed: " + arg.name + ", count: " + SpawnedEnemies.Count);
            }
        }, 
        randomX, randomY, 17, 9, arg =>
        {
            if (RemoveEnemy(arg))
            {
                _gameController.MoneyCountChanged(arg.Reward);

                Debug.Log("Enemy dead: " + arg.name + ", count: " + SpawnedEnemies.Count);
            }
        });

        SpawnedEnemies.Add(newEnemy);

        _gameController.UpdateTowerEnemyList(SpawnedEnemies);
    }

    private bool RemoveEnemy(Enemy enemy)
    {
        if (!SpawnedEnemies.Contains(enemy))
            return false;

        SpawnedEnemies.Remove(enemy);

        _gameController.UpdateTowerEnemyList(SpawnedEnemies);

        enemy.Push();

        return true;
    }
}