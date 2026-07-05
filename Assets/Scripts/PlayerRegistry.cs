using UnityEngine;

public class PlayerRegistry : MonoBehaviour
{
    private void OnEnable()
    {
        GameManager.Instance.SetPlayer(gameObject);
    }
}
