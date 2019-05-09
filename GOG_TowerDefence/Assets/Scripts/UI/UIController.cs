using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private TowerIcon _towerIcon;
    [SerializeField] private TowerIcon _towerUpgrade;

    [SerializeField] private CellIcon _cellIcon;

    [SerializeField] private RectTransform _upgradesContainer;
    [SerializeField] private RectTransform _towerIconsContainer;

    [SerializeField] private Text _remainingLives;
    [SerializeField] private Text _remainingMoney;

    [SerializeField] private Button _startNextWaveButton;
    [SerializeField] private Text _nextWaveLabel;

    [SerializeField] private GameObject _winScreen;
    [SerializeField] private GameObject _loseScreen;
    [SerializeField] private Button _retryButton;

    private Action _nextWavePressed;
    private readonly List<TowerIcon> _upgradeViews = new List<TowerIcon>();

    private void Awake()
    {
        _retryButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("Gameplay");

            _loseScreen.SetActive(false);
            _winScreen.SetActive(false);

            _retryButton.gameObject.SetActive(false);
        });

        _startNextWaveButton.onClick.AddListener(() =>
        {
            _startNextWaveButton.gameObject.SetActive(false);

            _nextWavePressed();
        });
    }

    public void Init(IEnumerable<Tower.TowerModel> towerList, Action<Tower.TowerModel> onIconClick)
    {
        foreach (var towerModel in towerList.Where(x => x.Main))
        {
            var model = towerModel;

            var towerIcon = Instantiate(_towerIcon, _towerIconsContainer);
            towerIcon.Show(model, onIconClick);
        }
    }

    public void ShowUpgrades(Vector3 position, List<Tower.TowerModel> upgrades, Action<Tower.TowerModel> onUpgradeClick, Action onCellClick)
    {
        ClearUpgradeViews();

        _upgradesContainer.anchoredPosition = position;

        foreach (var upgrade in upgrades)
        {
            var upgradeUiView = Instantiate(_towerUpgrade, _upgradesContainer);
            _upgradeViews.Add(upgradeUiView);

            upgradeUiView.Show(upgrade, arg =>
            {
                onUpgradeClick(arg);

                ClearUpgradeViews();
            });
        }

        var cellButton = Instantiate(_cellIcon, _upgradesContainer);
        _upgradeViews.Add(cellButton);

        cellButton.Show(() =>
        {
            onCellClick();

            ClearUpgradeViews();
        });
    }

    public void ShowStartNextWaveButton(int current, int max, Action callback)
    {
        _nextWaveLabel.text = "START NEXT WAVE [" + (current + 1) + "/" + max + "]";
        _startNextWaveButton.gameObject.SetActive(true);

        _nextWavePressed = callback;
    }

    private void ClearUpgradeViews()
    {
        foreach (var view in _upgradeViews) {
            view.Close();
        }

        _upgradeViews.Clear();
    }

    public void UpdateLivesCount(int count)
    {
        _remainingLives.text = count.ToString();
    }

    public void UpdateMoneyCount(int count)
    {
        _remainingMoney.text = count.ToString();
    }

    public void GameLost()
    {
        _loseScreen.SetActive(true);
        _retryButton.gameObject.SetActive(true);
    }

    public void GameWon()
    {
        _winScreen.SetActive(true);
        _retryButton.gameObject.SetActive(true);
    }
}