using System;
using System.Collections.Generic;
using Assets.Scripts.Enemy;
using Assets.Scripts.Grid;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private MatrixMap _matrixMap;

    [SerializeField] private EnemyController _enemyController;
    [SerializeField] private TowerController _towerController;
    [SerializeField] private UIController _uiController;

    private int _currentLivesCount;
    private int _currentMoneyCount;
    private int _currentWave = 0;

    private void Awake()
    {
        _currentLivesCount = Constants.MaxLivesCount;
        _currentMoneyCount = Constants.StartingMoney;

        _matrixMap.Init(this);

        _enemyController.Init(_matrixMap, this);
        _towerController.Init(_matrixMap);

        _uiController.Init(_towerController.TowerTypes.Keys, arg =>
        {
            _matrixMap.ClearSelectedTower();
            _towerController.GetTowerReady(arg);
        });

        _uiController.ShowStartNextWaveButton(0, _enemyController.WavesCount, () =>
        {
            _enemyController.SpawnNextWave(0);
        });

        _uiController.UpdateLivesCount(_currentLivesCount);
        _uiController.UpdateMoneyCount(_currentMoneyCount);
    }

    public void EnemyPassed(Enemy enemy)
    {
        _currentLivesCount--;

        if (_currentLivesCount <= 0) {
            GameLost();
        }

        _uiController.UpdateLivesCount(_currentLivesCount);
    }

    public void MoneyCountChanged(int change)
    {
        _currentMoneyCount += change;

        _uiController.UpdateMoneyCount(_currentMoneyCount);
    }

    public void UpdateTowerEnemyList(List<Enemy> enemies)
    {
        _towerController.UpdateTowerEnemyList(enemies);

        if (enemies.Count <= 0)
        {
            if (_currentWave <= _enemyController.WavesCount - 1)
            {
                _currentWave++;

                _uiController.ShowStartNextWaveButton(_currentWave, _enemyController.WavesCount, () =>
                {
                    _enemyController.SpawnNextWave(_currentWave);
                });
            }
            else
            {
                GameWon();
            }
        }
    }

    public bool EnoughMoney(int price)
    {
        if (_currentMoneyCount < price)
        {
            Debug.LogError("Not enough money!");
            return false;
        }

        return true;
    }

    public void RegisterTower(Tower tower)
    {
        Debug.Log("Register tower. Price [" + tower.Price + "]");

        _currentMoneyCount -= tower.Price;
        _uiController.UpdateMoneyCount(_currentMoneyCount);

        _towerController.RegisterTower(tower, _enemyController.SpawnedEnemies);
    }

    private void UnregisterTower(Tower tower)
    {
        _towerController.UnregisterTower(tower);
    }

    public void PlantedTowerSelected(Tower tower)
    {
        var canvasRect = _uiController.transform as RectTransform;

        var viewportPosition = Camera.main.WorldToViewportPoint(tower.transform.position);
        var worldObjectScreenPosition = new Vector2(

        viewportPosition.x * canvasRect.sizeDelta.x - (canvasRect.sizeDelta.x * 0.5f),
        viewportPosition.y * canvasRect.sizeDelta.y - (canvasRect.sizeDelta.y * 0.5f));

        _uiController.ShowUpgrades(worldObjectScreenPosition, tower.Upgrades, arg =>
        {
            var newTower = _towerController.TowerTypes[arg];
            
            if (EnoughMoney(newTower.Price))
            {
                var createdTower = Instantiate(newTower);

                RegisterTower(createdTower);

                createdTower.SetParameters(newTower.Model, newTower.Upgrades);

                createdTower.transform.SetParent(tower.ParentGridElement.transform);
                createdTower.transform.localPosition = Vector3.zero;

                UnregisterTower(tower);
            }
        }, 
        () =>
        {
            MoneyCountChanged(tower.Price / 2);

            tower.RemoveTower();
        });
    }

    public void GameWon()
    {
        Time.timeScale = 0f;
        Debug.LogError("GAME WON");
    }

    public void GameLost()
    {
        Time.timeScale = 0f;
        Debug.LogError("GAME LOST");
    }
}