using UnityEngine;

public class CheatButton : MonoBehaviour
{
    private const int HeartAmount = 1000;

    public void OnClick()
    {
        GameManager.Instance.AddHeart(HeartAmount);
    }
}
