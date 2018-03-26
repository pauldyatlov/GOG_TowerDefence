using System.Collections.Generic;
using Assets.Scripts.Enemy;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private Assets.Scripts.Grid.MatrixMap _matrixMap;

    [SerializeField] private EnemyController _enemyController;
    [SerializeField] private UIController _uiController;

    [SerializeField] private Tower _tower;
    
    private readonly List<Tower> _registeredTowers = new List<Tower>();
    private int _currentLivesCount;

    private void Awake()
    {
        _currentLivesCount = Constants.MaxLivesCount;

        _matrixMap.Init(RegisterTower);
        _enemyController.Init(_matrixMap, EnemyPassed, arg =>
        {
            foreach (var tower in _registeredTowers) {
                tower.UpdateEnemiesList(arg);
            }
        });

        _uiController.UpdateLivesCount(_currentLivesCount);
    }

    private void EnemyPassed(Enemy enemy)
    {
        _currentLivesCount--;

        _uiController.UpdateLivesCount(_currentLivesCount);

        Destroy(enemy.gameObject);
    }

    private void RegisterTower(Tower tower)
    {
        _registeredTowers.Add(tower);

        tower.UpdateEnemiesList(_enemyController.SpawnedEnemies);
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