using UnityEngine;

public class Pot : PlayerDetect
{
    private void Update()
    {
        if (isPlayerInRange)
        {
            Debug.Log("Player is in range of the pot.");
        }
    }
}
