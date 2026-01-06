using TMPro;
using UnityEngine;

public class HeartNumber : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI heartNumber;

    private void OnEnable()
    {
        GameEventManager.Instance.OnHeartNumberChange += UpdateHeartNumber;
    }

    private void OnDisable()
    {
        GameEventManager.Instance.OnHeartNumberChange -= UpdateHeartNumber;
    }

    private void UpdateHeartNumber()
    {
        int currentHearts = GameManager.Instance.CurrentHeart;
        heartNumber.text = currentHearts.ToString();
    }
}
