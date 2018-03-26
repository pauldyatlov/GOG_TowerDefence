using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private Text _livesCount;

    public void UpdateLivesCount(int count)
    {
        _livesCount.text = count.ToString();
    }
}