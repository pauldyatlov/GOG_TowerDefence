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

        foreach (var towerData in _towersData.dataArray)
        {
            var tower = Resources.Load<Tower>("Towers/tower" + towerData.KEY);
            Assert.IsTrue(tower != null);

            var sprite = Resources.Load<Sprite>("Towers/icons/" + towerData.Iconsprite);
            Assert.IsTrue(sprite != null);

            var model = new Tower.TowerModel(towerData.Main, towerData.Damage, towerData.Shootdistance,
                towerData.Firerate, towerData.Turnspeed, towerData.Price, sprite);

            tower.SetParameters(model);

            int index;
            int.TryParse(towerData.KEY, out index);

            TowerTypes.Add(model, tower);
        }
    }

    public void RegisterTower(Tower tower, List<Enemy> spawnedEnemies)
    {
        _registeredTowers.Add(tower);

        tower.UpdateEnemiesList(spawnedEnemies);
        tower.Init();
    }

    public void UpdateTowerEnemyList(List<Enemy> spawnedEnemies)
    {
        foreach (var tower in _registeredTowers)
        {
            tower.UpdateEnemiesList(spawnedEnemies);
        }
    }

    public void GetTowerReady(Tower.TowerModel model)
    {
        var tower = TowerTypes[model];

        var towerGo = Instantiate(tower);
        _matrixMap.SetSelectedTower(towerGo);
    }

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.F2))
    //    {
    //        GetTowerReady(TowerTypes.First().Value);
    //    }
    //}
}