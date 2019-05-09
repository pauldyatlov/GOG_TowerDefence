using System;
using UnityEngine.EventSystems;

public class CellIcon : TowerIcon
{
    private Action _onCellClick;

    public void Show(Action onCellClick)
    {
        _onCellClick = onCellClick;

        gameObject.SetActive(true);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        _onCellClick();
    }
}