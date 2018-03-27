using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private TowerIcon _towerIcon;
    [SerializeField] private RectTransform _towerIconsContainer;

    [SerializeField] private Text _remainingLives;
    [SerializeField] private Text _remainingMoney;

    public void Init(IEnumerable<Tower.TowerModel> towerList, Action<Tower.TowerModel> onIconClick)
    {
        foreach (var item in towerList.Where(x => x.Main))
        {
            var model = item;

            var towerIcon = Instantiate(_towerIcon, _towerIconsContainer);
            towerIcon.Show(model, onIconClick);
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