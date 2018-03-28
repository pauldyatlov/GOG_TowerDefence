using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Enemy;
using Assets.Scripts.Grid;
using UnityEngine;
using UnityEngine.Assertions;

public class TowerController : MonoBehaviour
{
    [SerializeField] private Towers _towersData;

    public readonly Dictionary<Tower.TowerModel, Tower> TowerTypes = new Dictionary<Tower.TowerModel, Tower>();

    private MatrixMap _matrixMap;
    private readonly List<Tower> _registeredTowers = new List<Tower>();

    public void Init(MatrixMap matrixMap)
    {
        _matrixMap = matrixMap;

        //TODO: maybe change to scriptable object
        foreach (var towerData in _towersData.dataArray)
        {
            var tower = Resources.Load<Tower>("Towers/tower" + towerData.KEY);
            Assert.IsTrue(tower != null);

            var sprite = Resources.Load<Sprite>("Towers/Icons/" + towerData.Iconsprite);
            Assert.IsTrue(sprite != null);

            var model = new Tower.TowerModel(tower, towerData.KEY, towerData.Main, towerData.Damage, towerData.Shootdistance,
                towerData.Firerate, towerData.Turnspeed, towerData.Price, sprite, towerData.Upgradetowers);

            TowerTypes.Add(model, tower);
        }

        foreach (var type in TowerTypes)
        {
            type.Value.SetParameters(type.Key, TowerTypes.Select(x => x.Key).Where(x => type.Key.Upgrades.Contains(x.Id)).ToList());
        }
    }

    public void RegisterTower(Tower tower, List<Enemy> spawnedEnemies)
    {
        _registeredTowers.Add(tower);

        tower.UpdateEnemiesList(spawnedEnemies);
        tower.Init();
    }

    public void UnregisterTower(Tower tower)
    {
        _registeredTowers.Remove(tower);

        Destroy(tower.gameObject);
    }

    public void UpdateTowerEnemyList(List<Enemy> spawnedEnemies)
    {
        foreach (var tower in _registeredTowers)
        {
            tower.UpdateEnemiesList(spawnedEnemies);
        }
    }

    public Tower GetTowerReady(Tower.TowerModel model)
    {
        var tower = TowerTypes[model];

        var createdTower = Instantiate(tower);
        createdTower.SetParameters(tower.Model, tower.Upgrades);

        _matrixMap.SetSelectedTower(createdTower);

        return createdTower;
    }
}