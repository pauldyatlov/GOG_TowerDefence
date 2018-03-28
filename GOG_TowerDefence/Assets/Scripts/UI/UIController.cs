using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private TowerIcon _towerIcon;
    [SerializeField] private TowerIcon _towerUpgrade;

    [SerializeField] private RectTransform _upgradesContainer;
    [SerializeField] private RectTransform _towerIconsContainer;

    [SerializeField] private Text _remainingLives;
    [SerializeField] private Text _remainingMoney;

    private readonly List<TowerIcon> _upgradeViews = new List<TowerIcon>();
    
    public void Init(IEnumerable<Tower.TowerModel> towerList, Action<Tower.TowerModel> onIconClick)
    {
        foreach (var towerModel in towerList.Where(x => x.Main))
        {
            var model = towerModel;

            var towerIcon = Instantiate(_towerIcon, _towerIconsContainer);
            towerIcon.Show(model, onIconClick);
        }
    }

    public void ShowUpgrades(Vector3 position, List<Tower.TowerModel> upgrades, Action<Tower.TowerModel> onUpgradeClick)
    {
        foreach (var upgrade in upgrades)
        {
            var upgradeUiView = Instantiate(_towerUpgrade, _upgradesContainer);
            _upgradeViews.Add(upgradeUiView);

            upgradeUiView.Show(upgrade, arg =>
            {
                onUpgradeClick(arg);

                foreach (var view in _upgradeViews) {
                    view.Close();
                }

                _upgradeViews.Clear();
            });
        }
    }

    public void UpdateLivesCount(int count)
    {
        _remainingLives.text = count.ToString();
    }

    public void UpdateMoneyCount(int count)
    {
        _remainingMoney.text = count.ToString();
    }
}