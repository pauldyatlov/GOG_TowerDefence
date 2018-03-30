using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button _startGameButton;

    private void Awake()
    {
        _startGameButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("Gameplay");
        });
    }
}