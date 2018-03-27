﻿using System;
using System.Collections.Generic;
using Assets.Scripts.Enemy;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private Assets.Scripts.Grid.MatrixMap _matrixMap;

    [SerializeField] private EnemyController _enemyController;
    [SerializeField] private TowerController _towerController;
    [SerializeField] private UIController _uiController;

    private int _currentLivesCount;
    private int _currentMoneyCount;

    private void Awake()
    {
        _currentLivesCount = Constants.MaxLivesCount;
        _currentMoneyCount = Constants.StartingMoney;

        _matrixMap.Init(RegisterTower);
        _enemyController.Init(_matrixMap, EnemyPassed, EnemyDead, UpdateTowerEnemyList);
        _towerController.Init(_matrixMap);

        _uiController.Init(_towerController.TowerTypes.Keys, arg =>
        {
            _towerController.GetTowerReady(arg);
        });

        _uiController.UpdateLivesCount(_currentLivesCount);
        _uiController.UpdateMoneyCount(_currentMoneyCount);
    }

    private void EnemyPassed(Enemy enemy)
    {
        _currentLivesCount--;

        if (_currentLivesCount <= 0)
        {
            throw new NullReferenceException();
        }

        _uiController.UpdateLivesCount(_currentLivesCount);
    }

    private void EnemyDead(Enemy enemy)
    {
        _currentMoneyCount += enemy.Reward;

        _uiController.UpdateMoneyCount(_currentMoneyCount);
    }

    private void UpdateTowerEnemyList(List<Enemy> enemies)
    {
        _towerController.UpdateTowerEnemyList(enemies);
    }

    private void RegisterTower(Tower tower)
    {
        if (_currentMoneyCount < tower.Price)
        {
            Debug.LogError("Not enough money!");
            return;
        }

        Debug.Log("Register tower. Price [" + tower.Price + "]");

        _currentMoneyCount -= tower.Price;
        _uiController.UpdateMoneyCount(_currentMoneyCount);

        _towerController.RegisterTower(tower, _enemyController.SpawnedEnemies);
    }
}