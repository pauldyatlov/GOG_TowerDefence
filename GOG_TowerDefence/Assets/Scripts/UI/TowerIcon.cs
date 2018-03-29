using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TowerIcon : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image _towerIcon;
    [SerializeField] private Text _price;

    private Tower.TowerModel _towerModel;
    private Action<Tower.TowerModel> _onIconClick;

    public void Show(Tower.TowerModel towerModel, Action<Tower.TowerModel> onIconClick)
    {
        _towerModel = towerModel;
        _onIconClick = onIconClick;

        gameObject.SetActive(true);

        _towerIcon.sprite = towerModel.Icon;
        _price.text = towerModel.Price.ToString();
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        _onIconClick(_towerModel);
    }

    public void Close()
    {
        Destroy(gameObject);
    }
}