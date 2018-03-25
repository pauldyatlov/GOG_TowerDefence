using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private MatrixMap.MatrixMap _matrixMap;

    [SerializeField] private Tower _tower;
    [SerializeField] private Enemy _enemy;
    [SerializeField] private Transform _enemiesContainer;

    private readonly List<Enemy> _spawnedEnemies = new List<Enemy>();
    private readonly List<Tower> _registeredTowers = new List<Tower>(); 

    private void Awake()
    {
        _matrixMap.Init(RegisterTower);

        var newEnemy = Instantiate(_enemy, Vector3.zero, Quaternion.identity, _enemiesContainer);
        newEnemy.Init(arg =>
        {
            _spawnedEnemies.Remove(arg);

            Destroy(arg.gameObject);
        });

        _spawnedEnemies.Add(newEnemy);

        foreach (var tower in _registeredTowers) {
            tower.UpdateEnemiesList(_spawnedEnemies);
        }
    }

    private void RegisterTower(Tower tower)
    {
        _registeredTowers.Add(tower);

        tower.UpdateEnemiesList(_spawnedEnemies);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F2))
        {
            var tower = Instantiate(_tower);
            _matrixMap.SetSelectedTower(tower);
        }
    }
}