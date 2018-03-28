using UnityEngine;
using UnityEngine.UI;

public class TowerUpgrade : MonoBehaviour
{
    [SerializeField] private Image _upgradeIcon;
    [SerializeField] private Text _upgradePrice;

    public void Show(Tower.TowerModel model)
    {
        _upgradeIcon.sprite = model.Icon;
        _upgradePrice.text = model.Price.ToString();
    }
}